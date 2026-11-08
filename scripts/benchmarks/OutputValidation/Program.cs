// Opt-in, read-only vanilla benchmark. Does not run merging or write a plugin.
// Import and patch construction are outside timings. The cold lookup baseline
// measures the former resolver alone; the current measurement validates fully.
using System.Diagnostics;
using System.Reflection;
using Loqui;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;

if (args.Length != 1)
    throw new ArgumentException("Pass the Skyrim Special Edition Data directory as the only argument.");
var files = new[] { "Skyrim.esm", "Update.esm", "Dawnguard.esm", "HearthFires.esm", "Dragonborn.esm" };
var mods = files.Select(file => SkyrimMod.CreateFromBinaryOverlay(Path.Combine(args[0], file), SkyrimRelease.SkyrimSE)).ToArray();
try
{
    var sample = mods[0].Npcs.Take(500).Cast<IMajorRecordGetter>().Concat(mods[0].Weapons.Take(500))
        .Concat(mods[0].LeveledItems.Take(500)).ToArray();
    var links = sample.SelectMany(r => r.EnumerateFormLinks(false)).Where(l => !l.IsNull)
        .Select(FormLinkInformation.Factory).Distinct().ToArray();
    Console.WriteLine($"Input: five vanilla masters; output sample: {sample.Length} records; unique links: {links.Length}.");

    using (var oldCache = mods.Cast<ISkyrimModGetter>().ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>())
    {
        var found = 0;
        Measure("Former simple-context resolver, fresh cache", () =>
        {
            foreach (var link in links)
                if (oldCache.TryResolveSimpleContext(link, out _)) found++;
        });
        Console.WriteLine($"Resolved {found}/{links.Length} links.");
        Measure("Former simple-context resolver, warmed cache", () =>
        {
            foreach (var link in links) oldCache.TryResolveSimpleContext(link, out _);
        });
    }

    var patch = new SkyrimMod(ModKey.FromNameAndExtension("ValidationBenchmark.esp"), SkyrimRelease.SkyrimSE);
    foreach (var record in sample)
        patch.GetTopLevelGroup(((ILoquiObject)record).Registration.GetterType).SetUntyped(record.DeepCopy());
    var listings = mods.Select(mod => new ModListing<ISkyrimModGetter>(mod)).ToArray();
    var order = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
    using var cache = order.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
    var runArgs = new RunSynthesisMutagenPatcher
    {
        OutputPath = Path.Combine(Path.GetTempPath(), patch.ModKey.FileName.String), DataFolderPath = args[0],
        LoadOrderFilePath = Path.Combine(Path.GetTempPath(), "plugins.txt"), GameRelease = GameRelease.SkyrimSE
    };
#pragma warning disable CS0618 // Complete IPatcherState fixture, matching existing benchmarks/tests.
    using var state = new SynthesisState<ISkyrimMod, ISkyrimModGetter>(runArgs,
        listings.Select(l => new LoadOrderListing(l.ModKey, l.Enabled)).ToArray(),
        order, cache, null!, patch, null, null, null, CancellationToken.None, null);
#pragma warning restore CS0618
    var validate = typeof(DreadsMashedPatch.Program).Assembly.GetType("DreadsMashedPatch.OutputPatchValidator")!
        .GetMethod("Validate", BindingFlags.Static | BindingFlags.Public)!
        .CreateDelegate<Action<IPatcherState<ISkyrimMod, ISkyrimModGetter>>>();
    Measure("Current complete validator, fresh cache", () => validate(state));
    Console.WriteLine($"Retained {patch.EnumerateMajorRecords().Count()}/{sample.Length} output records.");
}
finally
{
    foreach (var mod in mods) mod.Dispose();
}

static void Measure(string name, Action action)
{
    var before = GC.GetTotalAllocatedBytes();
    var clock = Stopwatch.StartNew();
    action();
    clock.Stop();
    Console.WriteLine($"{name}: {clock.Elapsed.TotalSeconds:F3}s; {(GC.GetTotalAllocatedBytes() - before) / 1024 / 1024} MiB allocated (not peak memory).");
}
