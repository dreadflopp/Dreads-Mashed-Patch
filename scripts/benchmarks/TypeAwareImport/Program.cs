// Opt-in investigation of the reported Inigo COED import. Input files are read
// only. The binary round trip uses a temporary directory, removed on completion.
// This probe does not change the application's import or patch-selection paths.
using System.Diagnostics;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim;

if (args.Length != 3)
    throw new ArgumentException("Pass the Skyrim Data directory, Inigo.esp path, and Zoldyg_inigo_replacer.esp path.");
var paths = new[] { "Skyrim.esm", "Update.esm", "Dawnguard.esm", "HearthFires.esm", "Dragonborn.esm" }
    .Select(file => Path.Combine(args[0], file)).Concat(args.Skip(1)).ToArray();
var inputs = new List<ISkyrimModDisposableGetter>();
try
{
    Measure("Open seven original overlays (headers, not full record decoding)", () =>
    {
        foreach (var path in paths) inputs.Add(SkyrimMod.CreateFromBinaryOverlay(path, SkyrimRelease.SkyrimSE));
    });
    using var cache = inputs.Cast<ISkyrimModGetter>().ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
    var key = new FormKey(ModKey.FromNameAndExtension("Inigo.esp"), 0xD62);
    var original = inputs[^1].Npcs[key];
    var untyped = original.Items?.Count(row => row.Data?.Owner is IUntypedOwnerGetter) ?? 0;
    Console.WriteLine($"Original {key}: {untyped} untyped ownership groups.");

    Npc? copied = null;
    Measure("Type-aware open/read/copy, fresh owner lookup", () => copied = ReadTypedCopy());
    Measure("Type-aware open/read/copy, warmed owner lookup", () => _ = ReadTypedCopy());
    var expected = Ranks(copied!);
    if (expected.Length != 5 || expected.Any(rank => rank != -1))
        throw new InvalidOperationException("This installed replacer does not match the five rank -1 groups from the report.");

    var directory = Path.Combine(Path.GetTempPath(), $"DmpTypedInigo-{Guid.NewGuid():N}");
    Directory.CreateDirectory(directory);
    try
    {
        var patch = new SkyrimMod(ModKey.FromNameAndExtension("TypeAwareInvestigation.esp"), SkyrimRelease.SkyrimSE);
        patch.Npcs.Add(copied!);
        var path = Path.Combine(directory, patch.ModKey.FileName.String);
        patch.WriteToBinary(path);
        using var reloaded = SkyrimMod.CreateFromBinaryOverlay(path, SkyrimRelease.SkyrimSE,
            new BinaryReadParameters { LinkCache = cache });
        var actual = Ranks(reloaded.Npcs[key]);
        if (!expected.SequenceEqual(actual)) throw new InvalidOperationException("Faction ranks changed on write.");
        Console.WriteLine($"Binary round trip preserved ranks: {string.Join(", ", actual)}.");
    }
    finally { Directory.Delete(directory, recursive: true); }

    Npc ReadTypedCopy()
    {
        // Keep the bootstrap cache and original overlays alive while the typed
        // overlay reads external owner types. Copy before disposing this view.
        using var typed = SkyrimMod.CreateFromBinaryOverlay(paths[^1], SkyrimRelease.SkyrimSE,
            new BinaryReadParameters { LinkCache = cache });
        return typed.Npcs[key].DeepCopy();
    }
}
finally
{
    foreach (var input in inputs) input.Dispose();
}

static int[] Ranks(INpcGetter npc) => npc.Items!.Where(row => row.Data != null)
    .Select(row => row.Data!.Owner is IFactionOwnerGetter faction
        ? faction.RequiredRank : throw new InvalidOperationException("Expected faction ownership.")).ToArray();

static void Measure(string label, Action action)
{
    var before = GC.GetTotalAllocatedBytes();
    var watch = Stopwatch.StartNew();
    action();
    watch.Stop();
    Console.WriteLine($"{label}: {watch.Elapsed.TotalMilliseconds:F1}ms; " +
        $"{(GC.GetTotalAllocatedBytes() - before) / 1024.0 / 1024:F2} MiB allocated (not peak memory).");
}
