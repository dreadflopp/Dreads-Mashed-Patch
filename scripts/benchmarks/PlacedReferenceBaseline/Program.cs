// Opt-in baseline benchmark; not a test of a relocation implementation.
// See docs/PLACED_REFERENCE_RELOCATION_PLAN.md for measurement scope and limits.
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Context = Mutagen.Bethesda.Plugins.Cache.IModContext<Mutagen.Bethesda.Skyrim.ISkyrimMod, Mutagen.Bethesda.Skyrim.ISkyrimModGetter, Mutagen.Bethesda.Plugins.Records.IMajorRecord, Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter>;

var original = ModKey.FromNameAndExtension("Source.esm");
var patchKey = ModKey.FromNameAndExtension("Patch.esp");
long sink = 0;
var transaction = typeof(PlacedNpcRecordHandler).Assembly.GetType("DreadsMashedPatch.RecordOverrideTransaction")!;
var commit = transaction.GetMethod("Apply", BindingFlags.Static | BindingFlags.Public)!
    .CreateDelegate<Action<Context, ISkyrimMod, Action<IMajorRecord>?>>();
var findings = new List<object>();

SkyrimMod MakeMod(ModKey key, int count, int cells, bool exterior = false)
{
    var mod = new SkyrimMod(key, SkyrimRelease.SkyrimSE);
    var block = new CellBlock { BlockNumber = 0, GroupType = GroupTypeEnum.InteriorCellBlock };
    var sub = new CellSubBlock { BlockNumber = 0, GroupType = GroupTypeEnum.InteriorCellSubBlock };
    var allCells = Enumerable.Range(0, cells).Select(i => new Cell(new FormKey(original, (uint)(0x1000 + i)), SkyrimRelease.SkyrimSE)
    {
        EditorID = "Cell" + i, Flags = exterior ? 0 : Cell.Flag.IsInteriorCell
    }).ToArray();
    foreach (var cell in allCells) sub.Cells.Add(cell);
    block.SubBlocks.Add(sub);
    if (!exterior) mod.Cells.Add(block);
    else
    {
        var world = new Worldspace(new FormKey(original, 0x800), SkyrimRelease.SkyrimSE);
        var worldBlock = new WorldspaceBlock { BlockNumberX = 0, BlockNumberY = 0, GroupType = GroupTypeEnum.ExteriorCellBlock };
        var worldSub = new WorldspaceSubBlock { BlockNumberX = 0, BlockNumberY = 0, GroupType = GroupTypeEnum.ExteriorCellSubBlock };
        foreach (var cell in allCells) worldSub.Items.Add(cell);
        worldBlock.Items.Add(worldSub); world.SubCells.Add(worldBlock); mod.Worldspaces.Add(world);
    }
    for (var i = 0; i < count; i++)
        allCells[i % cells].Temporary.Add(new PlacedNpc(new FormKey(original, (uint)(0x10000 + i)), SkyrimRelease.SkyrimSE)
        {
            EditorID = "Actor" + i,
            Placement = new Placement { Position = new P3Float(i % 4000, i % 2000, 100), Rotation = new P3Float(0, 0, 0) }
        });
    return mod;
}

(double Milliseconds, long Bytes) Measure(Action action)
{
    var before = GC.GetAllocatedBytesForCurrentThread();
    var clock = Stopwatch.StartNew(); action(); clock.Stop();
    return (clock.Elapsed.TotalMilliseconds, GC.GetAllocatedBytesForCurrentThread() - before);
}

double Median(IEnumerable<double> values) => values.Order().ElementAt(values.Count() / 2);

// Setup, first context-index construction, JIT and source allocation are outside timed regions.
foreach (var exterior in new[] { false, true })
{
    var mods = new[] { original, ModKey.FromNameAndExtension("Earlier.esp"), ModKey.FromNameAndExtension("Winner.esp") }
        .Select(key => MakeMod(key, 10000, 100, exterior)).ToArray();
    var order = new LoadOrder<IModListing<ISkyrimModGetter>>(mods.Select(m => new ModListing<ISkyrimModGetter>(m)));
    using var cache = order.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
    var histories = Enumerable.Range(0, 10000).Select(i => cache.ResolveAllContexts(new FormKey(original, (uint)(0x10000 + i)), typeof(IPlacedNpcGetter)).ToArray()).ToArray();
    void Projection(bool ancestry)
    {
        long total = 0;
        for (var repeat = 0; repeat < 40; repeat++)
        foreach (var history in histories)
        foreach (var context in history)
        {
            var placed = (IPlacedNpcGetter)context.Record;
            total += (int)placed.Placement!.Position.X;
            total += (int)placed.Placement.Rotation.Z;
            if (!ancestry) continue;
            for (var parent = context.Parent; parent != null; parent = parent.Parent)
                if (parent.Record is ICellGetter cell) total += cell.FormKey.ID;
                else if (parent.Record is IWorldspaceGetter world) total += world.FormKey.ID;
        }
        sink = total;
    }
    Projection(false); Projection(true);
    var pairs = Enumerable.Range(0, 7).Select(round =>
    {
        // Alternate measurement order to reduce ordering bias.
        (double Milliseconds, long Bytes) a, b;
        if (round % 2 == 0) { a = Measure(() => Projection(false)); b = Measure(() => Projection(true)); }
        else { b = Measure(() => Projection(true)); a = Measure(() => Projection(false)); }
        return (a,b);
    }).ToArray();
    findings.Add(new { Kind = "projection", Exterior = exterior, Records = 10000, HistoryDepth = 3, Repeats = 40,
        PlainMs = Median(pairs.Select(p => p.a.Milliseconds)), WithParentsMs = Median(pairs.Select(p => p.b.Milliseconds)),
        ExtraAllocatedBytes = Median(pairs.Select(p => (double)(p.b.Bytes - p.a.Bytes))) });
}

// Actual existing transaction, including ancestry staging, per-reference insertion and a scalar write.
foreach (var exterior in new[] { false, true })
foreach (var cells in new[] { 1, 50 })
foreach (var count in new[] { 500, 1000, 2000 })
{
    var source = MakeMod(original, count, cells, exterior);
    var order = new LoadOrder<IModListing<ISkyrimModGetter>>(new[] { new ModListing<ISkyrimModGetter>(source) });
    using var cache = order.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
    var contexts = Enumerable.Range(0, count).Select(i => cache.ResolveAllContexts(new FormKey(original, (uint)(0x10000 + i)), typeof(IPlacedNpcGetter)).Single()).ToArray();
    void Publish()
    {
        var patch = new SkyrimMod(patchKey, SkyrimRelease.SkyrimSE);
        foreach (var context in contexts) commit(context, patch, candidate => ((IPlacedNpc)candidate).Scale = 1.1f);
        sink = patch.EnumerateMajorRecords<IPlacedNpcGetter>().Count();
        if (sink != count) throw new InvalidOperationException("Output count mismatch");
    }
    Publish();
    var runs = Enumerable.Range(0, 5).Select(_ => Measure(Publish)).ToArray();
    findings.Add(new { Kind = "current_transaction", Exterior = exterior, Cells = cells, Records = count,
        Milliseconds = Median(runs.Select(x => x.Milliseconds)), AllocatedBytes = Median(runs.Select(x => (double)x.Bytes)) });
}
Console.WriteLine(JsonSerializer.Serialize(findings, new JsonSerializerOptions { WriteIndented = true }));
GC.KeepAlive(sink);
