using DreadsMashedPatch.Enums;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using Xunit;

namespace DreadsMashedPatch.Tests;

[CollectionDefinition("TamrielPersistentCellPolicyTests", DisableParallelization = true)]
public sealed class TamrielPersistentCellPolicyTestCollection;

[Collection("TamrielPersistentCellPolicyTests")]
public sealed class TamrielPersistentCellPolicyTests : IDisposable
{
    private static readonly ModKey SkyrimKey = ModKey.FromNameAndExtension("Skyrim.esm");
    private static readonly ModKey DawnguardKey = ModKey.FromNameAndExtension("Dawnguard.esm");
    private static readonly ModKey WinningKey = ModKey.FromNameAndExtension("Winning.esp");
    private static readonly ModKey PatchKey = ModKey.FromNameAndExtension("MashedPatch.esp");
    private static readonly FormKey TamrielCellKey = new(SkyrimKey, 0x000D74);
    private static readonly FormKey TamrielWorldspaceKey = new(SkyrimKey, 0x00003C);

    public void Dispose() => PatcherSettings.Apply(new PatcherConfiguration());

    [Fact]
    public void DefaultHybridPolicyUsesDawnguardWhenWinningHeaderEqualsSkyrim()
    {
        using var state = CreateState();
        PatcherSettings.Apply(new PatcherConfiguration());

        ProcessTamrielCell(state);

        var patchedCell = Assert.Single(state.PatchMod.EnumerateMajorRecords<ICellGetter>());
        Assert.Equal("DawnguardState", patchedCell.EditorID);
        Assert.Empty(patchedCell.Persistent);
    }

    [Fact]
    public void HybridPolicyUsesNormalForwardingWhenWinningHeaderDiffersFromSkyrim()
    {
        using var state = CreateState("WinningState");
        PatcherSettings.Apply(new PatcherConfiguration());

        ProcessTamrielCell(state);

        Assert.Empty(state.PatchMod.EnumerateMajorRecords<ICellGetter>());
    }

    [Fact]
    public void DawnguardPolicyAlwaysCopiesDawnguardHeader()
    {
        using var state = CreateState("WinningState");
        PatcherSettings.Apply(Configuration(TamrielPersistentCellPolicy.PreferDawnguard));

        ProcessTamrielCell(state);

        Assert.Equal("DawnguardState", Assert.Single(
            state.PatchMod.EnumerateMajorRecords<ICellGetter>()).EditorID);
    }

    [Fact]
    public void SkyrimPolicyCopiesCompleteOriginalSnapshot()
    {
        using var state = CreateState();
        PatcherSettings.Apply(Configuration(TamrielPersistentCellPolicy.PreferSkyrim));

        ProcessTamrielCell(state);

        Assert.Equal("SkyrimState", Assert.Single(
            state.PatchMod.EnumerateMajorRecords<ICellGetter>()).EditorID);
    }

    [Fact]
    public void KeepWinningPolicyCopiesWinningHeaderWithoutChildRecords()
    {
        using var state = CreateState("WinningState");
        PatcherSettings.Apply(Configuration(TamrielPersistentCellPolicy.KeepWinning));

        ProcessTamrielCell(state);

        var patchedCell = Assert.Single(state.PatchMod.EnumerateMajorRecords<ICellGetter>());
        Assert.Equal("WinningState", patchedCell.EditorID);
        Assert.Empty(patchedCell.Persistent);
    }

    [Fact]
    public void PriorityModRuleTakesPrecedenceOverSmartDefault()
    {
        using var state = CreateState();
        var configuration = new PatcherConfiguration
        {
            AlwaysWinningMods = [SkyrimKey.FileName.String]
        };
        PatcherSettings.Apply(configuration);

        ProcessTamrielCell(state);

        Assert.Equal("SkyrimState", Assert.Single(
            state.PatchMod.EnumerateMajorRecords<ICellGetter>()).EditorID);
    }

    [Fact]
    public void StandardPolicyUsesOrdinaryPrefiltering()
    {
        PatcherSettings.Apply(Configuration(TamrielPersistentCellPolicy.StandardForwarding));

        Assert.False(CellRecordHandler.RequiresSmartPolicyProcessing(TamrielCellKey));
        Assert.False(CellRecordHandler.RequiresSmartPolicyProcessing(
            new FormKey(SkyrimKey, 0x000D75)));
    }

    private static PatcherConfiguration Configuration(TamrielPersistentCellPolicy policy) => new()
    {
        Forwarding = new ForwardingSettings
        {
            TamrielPersistentCellPolicy = policy
        }
    };

#pragma warning disable CS0618 // SynthesisState remains useful as a complete test implementation of IPatcherState.
    private static void ProcessTamrielCell(SynthesisState<ISkyrimMod, ISkyrimModGetter> state)
    {
        var contexts = state.LoadOrder.PriorityOrder
            .WinningContextOverrides<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(state.LinkCache)
            .ToArray();

        new CellRecordHandler().Process(state, contexts);
    }

    private static SynthesisState<ISkyrimMod, ISkyrimModGetter> CreateState(
        string winningEditorId = "SkyrimState")
    {
        var skyrim = CreateMod(SkyrimKey, "SkyrimState");
        var dawnguard = CreateMod(DawnguardKey, "DawnguardState");
        var winning = CreateMod(WinningKey, winningEditorId);
        var patch = new SkyrimMod(PatchKey, SkyrimRelease.SkyrimSE);
        SkyrimMod[] mods = [skyrim, dawnguard, winning, patch];
        var listings = mods
            .Cast<ISkyrimModGetter>()
            .Select(mod => new ModListing<ISkyrimModGetter>(mod))
            .ToArray();
        var loadOrder = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
        var linkCache = loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        var arguments = new RunSynthesisMutagenPatcher
        {
            OutputPath = @"C:\Temp\MashedPatch.esp",
            DataFolderPath = @"C:\Temp\Data",
            LoadOrderFilePath = @"C:\Temp\plugins.txt",
            GameRelease = GameRelease.SkyrimSE
        };

        return new SynthesisState<ISkyrimMod, ISkyrimModGetter>(
            arguments,
            listings.Select(listing => new LoadOrderListing(listing.ModKey, listing.Enabled)).ToArray(),
            loadOrder,
            linkCache,
            null!,
            patch,
            null,
            null,
            null,
            CancellationToken.None,
            null);
    }
#pragma warning restore CS0618

    private static SkyrimMod CreateMod(ModKey modKey, string editorId)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        var cell = new Cell(TamrielCellKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = editorId
        };
        cell.Persistent.Add(new PlacedObject(new FormKey(modKey, 0x000800), SkyrimRelease.SkyrimSE));
        var tamriel = new Worldspace(TamrielWorldspaceKey, SkyrimRelease.SkyrimSE)
        {
            EditorID = "Tamriel",
            TopCell = cell
        };
        mod.Worldspaces.Add(tamriel);
        return mod;
    }
}
