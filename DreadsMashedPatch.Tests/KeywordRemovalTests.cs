using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using Xunit;

namespace DreadsMashedPatch.Tests;

[CollectionDefinition("CoverageFixes", DisableParallelization = true)]
public sealed class CoverageFixesCollection;

[Collection("CoverageFixes")]
public sealed class KeywordRemovalTests : IDisposable
{
    public void Dispose() => CoverageTestHistory.Reset();

    public static IEnumerable<object[]> Registrations() => typeof(AbstractRecordHandler).Assembly.GetTypes()
        .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(AbstractRecordHandler)))
        .Where(type => ((AbstractRecordHandler)System.Activator.CreateInstance(type)!).PropertyHandlers
            .TryGetValue("Keywords", out var handler) && handler is KeywordListHandler)
        .Select(type => new object[] { type });

    public static IEnumerable<object[]> Histories() => Registrations().SelectMany(row =>
        new[] { false, true }.SelectMany(empty => new[] { false, true }
            .Select(authorized => new object[] { row[0], empty, authorized })));

    [Theory]
    [MemberData(nameof(Registrations))]
    public void EverySharedSetterWritesAbsenceAndEmptyAndDetachesLinks(Type handlerType)
    {
        var handler = ((AbstractRecordHandler)System.Activator.CreateInstance(handlerType)!).PropertyHandlers["Keywords"];
        var record = NewRecord(handlerType);
        var sourceLink = new FormLink<IKeywordGetter>(CoverageTestHistory.LinkKey);
        handler.SetValue(record, new List<IFormLinkGetter<IKeywordGetter>> { sourceLink });
        var saved = Assert.Single(Assert.IsAssignableFrom<IKeywordedGetter<IKeywordGetter>>(record).Keywords!);
        Assert.NotSame(sourceLink, saved);
        sourceLink.SetTo(new FormKey(CoverageTestHistory.Original, 0x802));
        Assert.Equal(CoverageTestHistory.LinkKey, saved.FormKey);

        handler.SetValue(record, null);
        Assert.Null(handler.GetValue(record));
        handler.SetValue(record, new List<IFormLinkGetter<IKeywordGetter>>());
        Assert.Empty(Assert.IsAssignableFrom<IKeywordedGetter<IKeywordGetter>>(record).Keywords!);
        Assert.False(handler.AreValuesEqual(null, handler.GetValue(record)));
    }

    [Theory]
    [MemberData(nameof(Histories))]
    public void FullRunRemovalAndRestorationRespectPermissionsAndSurviveBinaryOutput(
        Type handlerType, bool empty, bool authorized)
    {
        CoverageTestHistory.Configure(authorized);
        var mods = CoverageTestHistory.Mods();
        var property = ((AbstractRecordHandler)System.Activator.CreateInstance(handlerType)!).PropertyHandlers["Keywords"];
        var sources = mods.Select(_ => NewRecord(handlerType)).ToArray();
        for (var i = 0; i < mods.Length; i++)
        {
            property.SetValue(sources[i], i == 1
                ? (empty ? new List<IFormLinkGetter<IKeywordGetter>>() : null)
                : new List<IFormLinkGetter<IKeywordGetter>> { new FormLink<IKeywordGetter>(CoverageTestHistory.LinkKey) });
            mods[i].TryGetTopLevelGroup(sources[i].GetType())!.SetUntyped(sources[i]);
        }
        using var state = CoverageTestHistory.State(mods);

        Program.RunPatch(state);

        if (authorized) Assert.Empty(state.PatchMod.EnumerateMajorRecords());
        else
        {
            var output = Assert.Single(state.PatchMod.EnumerateMajorRecords());
            Verify(output);
            using var stream = new MemoryStream();
            state.PatchMod.WriteToBinary(stream);
            stream.Position = 0;
            using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
            Verify(Assert.Single(overlay.EnumerateMajorRecords()));
        }
        Assert.Single(Assert.IsAssignableFrom<IKeywordedGetter<IKeywordGetter>>(sources[0]).Keywords!);
        Assert.Single(Assert.IsAssignableFrom<IKeywordedGetter<IKeywordGetter>>(sources[2]).Keywords!);

        void Verify(IMajorRecordGetter output)
        {
            var keywords = Assert.IsAssignableFrom<IKeywordedGetter<IKeywordGetter>>(output).Keywords;
            if (empty) Assert.Empty(keywords!);
            else Assert.Null(keywords);
        }
    }

    private static IMajorRecord NewRecord(Type handlerType)
    {
        var type = typeof(SkyrimMod).Assembly.GetType("Mutagen.Bethesda.Skyrim." + handlerType.Name.Replace("RecordHandler", ""))!;
        return (IMajorRecord)System.Activator.CreateInstance(type, CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE)!;
    }
}

internal static class CoverageTestHistory
{
    internal static readonly ModKey Original = ModKey.FromNameAndExtension("Skyrim.esm");
    internal static readonly ModKey Earlier = ModKey.FromNameAndExtension("Earlier.esp");
    internal static readonly ModKey Winner = ModKey.FromNameAndExtension("Winner.esp");
    internal static readonly FormKey RecordKey = new(Original, 0x800);
    internal static readonly FormKey LinkKey = new(Original, 0x801);

    internal static void Configure(bool authorized = false)
    {
        PatcherSettings.Apply(new PatcherConfiguration
        {
            CompatibilityRules = authorized
                ? [new VirtualMasterRule { TargetMod = Winner.FileName.String, VirtualMasters = [Earlier.FileName.String] }]
                : []
        });
        Utility.InitializeVanillaMods([Original], [], false);
        LogCollector.Clear();
    }

    internal static void Reset()
    {
        PatcherSettings.Apply(new PatcherConfiguration());
        LogCollector.Clear();
    }

    internal static SkyrimMod[] Mods()
    {
        SkyrimMod[] mods = [new(Original, SkyrimRelease.SkyrimSE), new(Earlier, SkyrimRelease.SkyrimSE), new(Winner, SkyrimRelease.SkyrimSE)];
        foreach (var mod in mods.Skip(1)) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = Original });
        return mods;
    }

#pragma warning disable CS0618
    internal static SynthesisState<ISkyrimMod, ISkyrimModGetter> State(params SkyrimMod[] mods)
    {
        var patch = new SkyrimMod(ModKey.FromNameAndExtension("CoverageFixTests.esp"), SkyrimRelease.SkyrimSE);
        var listings = mods.Cast<ISkyrimModGetter>().Append(patch).Select(mod => new ModListing<ISkyrimModGetter>(mod)).ToArray();
        var order = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
        var args = new RunSynthesisMutagenPatcher
        {
            OutputPath = Path.Combine(Path.GetTempPath(), patch.ModKey.FileName.String), DataFolderPath = Path.GetTempPath(),
            LoadOrderFilePath = Path.Combine(Path.GetTempPath(), "plugins.txt"), GameRelease = GameRelease.SkyrimSE
        };
        return new SynthesisState<ISkyrimMod, ISkyrimModGetter>(args,
            listings.Select(listing => new LoadOrderListing(listing.ModKey, listing.Enabled)).ToArray(),
            order, order.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(), null!, patch, null, null, null, CancellationToken.None, null);
    }
#pragma warning restore CS0618
}
