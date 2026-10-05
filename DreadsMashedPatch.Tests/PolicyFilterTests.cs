using System.Drawing;
using DreadsMashedPatch.Enums;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("AlwaysWinningModTests")]
public sealed class PolicyFilterTests : IDisposable
{
    private static readonly ModKey Skyrim = ModKey.FromNameAndExtension("Skyrim.esm");
    private static readonly ModKey Update = ModKey.FromNameAndExtension("Update.esm");
    private static readonly ModKey Priority = ModKey.FromNameAndExtension("Priority.esp");
    private static readonly ModKey Later = ModKey.FromNameAndExtension("Later.esp");
    private static readonly ModKey Patch = ModKey.FromNameAndExtension("Patch.esp");
    private static readonly FormKey Key = new(Skyrim, 0x800);

    public void Dispose()
    {
        PatcherSettings.Apply(new PatcherConfiguration());
        Utility.InitializeVanillaMods([Skyrim, Update], [], false);
    }

    [Theory]
    [InlineData("two")]
    [InlineData("previous-vanilla")]
    [InlineData("vanilla-winner")]
    [InlineData("effective-vanilla-winner")]
    [InlineData("older-than-three")]
    public void FullRunHonorsPriorityAcrossEveryEarlyExit(string scenario)
    {
        var source = Mod(scenario == "two" ? Priority : Skyrim, "PriorityValue", 0x40000000);
        List<SkyrimMod> mods = [source];
        if (scenario is "previous-vanilla" or "vanilla-winner" or "effective-vanilla-winner")
            mods.Add(Mod(Update, "OfficialValue"));
        if (scenario == "older-than-three")
        {
            mods.Add(Mod(Priority, "Intermediate"));
            mods.Add(Mod(ModKey.FromNameAndExtension("Second.esp"), "Second"));
            mods.Add(Mod(ModKey.FromNameAndExtension("Third.esp"), "Third"));
        }
        if (scenario != "vanilla-winner") mods.Add(Mod(Later, "LaterValue"));
        PatcherSettings.Apply(new PatcherConfiguration
        {
            AlwaysWinningMods = [source.ModKey.FileName.String],
            IgnoredMods = scenario == "effective-vanilla-winner" ? [Later.FileName.String] : []
        });
        using var state = State(mods.ToArray());

        Program.RunPatch(state);

        var result = Assert.Single(state.PatchMod.Keywords);
        Assert.Equal("PriorityValue", result.EditorID);
        Assert.Equal(0x40000000, result.MajorRecordFlagsRaw);
        Assert.Equal(source.Keywords[Key].Color, result.Color);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ConfigurationOrderSelectsPriorityIndependentOfLoadOrder(bool reverse)
    {
        var first = Mod(Priority, "First");
        var second = Mod(Later, "Second");
        PatcherSettings.Apply(new PatcherConfiguration
        {
            AlwaysWinningMods = reverse
                ? [Later.FileName.String, Priority.FileName.String]
                : [Priority.FileName.String, Later.FileName.String]
        });
        using var state = State(first, second, Mod(ModKey.FromNameAndExtension("Last.esp"), "Last"));

        Program.RunPatch(state);

        Assert.Equal(reverse ? "First" : "Second", Assert.Single(state.PatchMod.Keywords).EditorID);
    }

    [Theory]
    [InlineData("ignored")]
    [InlineData("absent")]
    [InlineData("does-not-edit")]
    [InlineData("already-wins")]
    [InlineData("disabled-record")]
    public void IneligibleOrAlreadyWinningPriorityDoesNotCreateAnOverride(string scenario)
    {
        var priority = Mod(Priority, "Priority");
        if (scenario == "does-not-edit") priority.Keywords.Clear();
        PatcherSettings.Apply(new PatcherConfiguration
        {
            AlwaysWinningMods = [Priority.FileName.String],
            IgnoredMods = scenario == "ignored" ? [Priority.FileName.String] : [],
            DisabledRecordTypes = scenario == "disabled-record" ? [typeof(IKeywordGetter).FullName!] : []
        });
        var mods = scenario == "absent" ? new[] { Mod(Later, "Later") }
            : scenario == "already-wins" ? new[] { priority }
            : new[] { priority, Mod(Later, "Later") };
        using var state = State(mods);

        Program.RunPatch(state);

        Assert.Empty(state.PatchMod.Keywords);
    }

    [Theory]
    [InlineData("Baseline")]
    [InlineData(null)]
    public void ShortHistoryPreservesBaselineEdidAndAllOtherWinnerValues(string? baseline)
    {
        PreserveEdids();
        var winner = Mod(Later, "Changed", 0x40000000);
        winner.Keywords[Key].Color = Color.FromArgb(1, 2, 3);
        using var state = State(Mod(Skyrim, baseline), winner);

        Program.RunPatch(state);

        var result = Assert.Single(state.PatchMod.Keywords);
        Assert.Equal(baseline, result.EditorID);
        Assert.Equal(winner.Keywords[Key].Color, result.Color);
        Assert.Equal(winner.Keywords[Key].MajorRecordFlagsRaw, result.MajorRecordFlagsRaw);
    }

    [Fact]
    public void ImmediatelyPrecedingOfficialVersionSuppliesLatestBaseline()
    {
        PreserveEdids();
        using var state = State(Mod(Skyrim, "Original"), Mod(Update, "UpdatedOfficial"), Mod(Later, "Changed"));

        Program.RunPatch(state);

        Assert.Equal("UpdatedOfficial", Assert.Single(state.PatchMod.Keywords).EditorID);
    }

    [Fact]
    public void NormalMergingFindsBaselineBeyondFirstThreeContexts()
    {
        PreserveEdids();
        using var state = State(Mod(Skyrim, "Original"), Mod(Update, "UpdatedOfficial"),
            Mod(Priority, "FirstChange"), Mod(ModKey.FromNameAndExtension("Middle.esp"), "SecondChange"),
            Mod(Later, "Changed"));

        Program.RunPatch(state);

        Assert.Equal("UpdatedOfficial", Assert.Single(state.PatchMod.Keywords).EditorID);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CreationClubBaselineMembershipRemainsConfigurable(bool includeCreationClub)
    {
        var creation = ModKey.FromNameAndExtension("ccPolicy.esl");
        PatcherSettings.Apply(new PatcherConfiguration
        {
            Forwarding = new ForwardingSettings
            {
                EditorIdPolicy = EditorIdForwardingPolicy.PreserveBaseline,
                TreatCreationClubAsVanilla = includeCreationClub
            }
        }, [creation]);
        using var state = State(Mod(Skyrim, "Original"), Mod(creation, "Creation"), Mod(Later, "Changed"));

        Program.RunPatch(state);

        Assert.Equal(includeCreationClub ? "Creation" : "Original", Assert.Single(state.PatchMod.Keywords).EditorID);
    }

    [Theory]
    [InlineData("ignored-latest")]
    [InlineData("ignored-only")]
    [InlineData("no-baseline")]
    [InlineData("same")]
    [InlineData("normalized-same")]
    [InlineData("single-official")]
    [InlineData("disabled-record")]
    public void BaselineEligibilityAndEqualityAreRespected(string scenario)
    {
        PatcherSettings.Apply(new PatcherConfiguration
        {
            Forwarding = new ForwardingSettings { EditorIdPolicy = EditorIdForwardingPolicy.PreserveBaseline },
            IgnoredMods = scenario == "ignored-latest" ? [Update.FileName.String]
                : scenario == "ignored-only" ? [Skyrim.FileName.String] : [],
            DisabledRecordTypes = scenario == "disabled-record" ? [typeof(IKeywordGetter).FullName!] : []
        });
        List<SkyrimMod> mods = [];
        if (scenario != "no-baseline") mods.Add(Mod(Skyrim, "Original"));
        if (scenario == "ignored-latest") mods.Add(Mod(Update, "IgnoredOfficial"));
        if (scenario != "single-official")
            mods.Add(Mod(Later, scenario == "same" ? "Original"
                : scenario == "normalized-same" ? "Original " : "Changed"));
        using var state = State(mods.ToArray());

        Program.RunPatch(state);

        if (scenario == "ignored-latest") Assert.Equal("Original", Assert.Single(state.PatchMod.Keywords).EditorID);
        else Assert.Empty(state.PatchMod.Keywords);
    }

    [Theory]
    [InlineData(EditorIdForwardingPolicy.StandardForwarding)]
    [InlineData(EditorIdForwardingPolicy.ForwardOnlyWithOtherChanges)]
    public void OtherEdidPoliciesRetainOrdinaryShortHistoryFilter(EditorIdForwardingPolicy policy)
    {
        PatcherSettings.Apply(new PatcherConfiguration { Forwarding = new ForwardingSettings { EditorIdPolicy = policy } });
        using var state = State(Mod(Skyrim, "Original"), Mod(Later, "Changed"));

        Program.RunPatch(state);

        Assert.Empty(state.PatchMod.Keywords);
    }

    [Theory]
    [InlineData("priority")]
    [InlineData("priority-already-wins")]
    [InlineData("edid")]
    [InlineData("ordinary-skip")]
    public void PolicyOnlyProcessingNeverLoadsFullMergeHistory(string policy)
    {
        PatcherSettings.Apply(new PatcherConfiguration
        {
            AlwaysWinningMods = policy.StartsWith("priority", StringComparison.Ordinal) ? [Skyrim.FileName.String] : [],
            Forwarding = new ForwardingSettings { EditorIdPolicy = EditorIdForwardingPolicy.PreserveBaseline }
        });
        var mods = policy == "priority-already-wins" ? new[] { Mod(Skyrim, "Original") }
            : new[] { Mod(Skyrim, "Original"), Mod(Later, policy == "ordinary-skip" ? "Original" : "Changed") };
        using var state = State(mods);
        var handler = new NoHistoryKeywordHandler();

        var report = handler.Process(state, Winners(state));

        Assert.True(report.Succeeded);
        Assert.Equal(0, handler.HistoryLoads);
        if (policy is "priority" or "edid") Assert.Equal("Original", Assert.Single(state.PatchMod.Keywords).EditorID);
        else Assert.Empty(state.PatchMod.Keywords);
    }

    [Fact]
    public void PrioritySnapshotTakesPrecedenceOverBaselineEdidPreservation()
    {
        PatcherSettings.Apply(new PatcherConfiguration
        {
            AlwaysWinningMods = [Priority.FileName.String],
            Forwarding = new ForwardingSettings { EditorIdPolicy = EditorIdForwardingPolicy.PreserveBaseline }
        });
        using var state = State(Mod(Skyrim, "Original"), Mod(Priority, "Priority"), Mod(Later, "Changed"));

        Program.RunPatch(state);

        Assert.Equal("Priority", Assert.Single(state.PatchMod.Keywords).EditorID);
    }

    [Fact]
    public void BaselineCacheIsRebuiltForEachRun()
    {
        PreserveEdids();
        foreach (var baseline in new[] { "FirstBaseline", "SecondBaseline" })
        {
            using var state = State(Mod(Skyrim, baseline), Mod(Later, "Changed"));
            Program.RunPatch(state);
            Assert.Equal(baseline, Assert.Single(state.PatchMod.Keywords).EditorID);
        }
    }

    [Fact]
    public void OlderPrioritySourceDoesNotRequireFullMergeHistory()
    {
        PatcherSettings.Apply(new PatcherConfiguration { AlwaysWinningMods = [Skyrim.FileName.String] });
        using var state = State(Mod(Skyrim, "Original"), Mod(Priority, "Priority"),
            Mod(ModKey.FromNameAndExtension("Middle.esp"), "Middle"), Mod(Later, "Changed"));
        var handler = new NoHistoryKeywordHandler();

        Assert.True(handler.Process(state, Winners(state)).Succeeded);

        Assert.Equal(0, handler.HistoryLoads);
        Assert.Equal("Original", Assert.Single(state.PatchMod.Keywords).EditorID);
    }

    [Theory]
    [InlineData("WEAP", false)]
    [InlineData("WEAP", true)]
    [InlineData("NPC_", false)]
    [InlineData("NPC_", true)]
    [InlineData("GLOB", false)]
    [InlineData("GLOB", true)]
    [InlineData("CELL", false)]
    [InlineData("CELL", true)]
    [InlineData("REFR", false)]
    [InlineData("REFR", true)]
    public void TypedAndNestedRecordsUseSharedPolicyPaths(string kind, bool priority)
    {
        PatcherSettings.Apply(new PatcherConfiguration
        {
            AlwaysWinningMods = priority ? [Skyrim.FileName.String] : [],
            DisabledRecordTypes = kind == "CELL" ? [typeof(IPlacedObjectGetter).FullName!] : [],
            Forwarding = new ForwardingSettings { EditorIdPolicy = EditorIdForwardingPolicy.PreserveBaseline }
        });
        var original = new SkyrimMod(Skyrim, SkyrimRelease.SkyrimSE);
        var winner = new SkyrimMod(Later, SkyrimRelease.SkyrimSE);
        AddTypedRecord(original, kind, "Original", 0x40000000);
        AddTypedRecord(winner, kind, "Changed", 0);
        using var state = State(original, winner);

        Program.RunPatch(state);

        var result = state.PatchMod.EnumerateMajorRecords().Single(record => record.FormKey == Key);
        Assert.Equal("Original", result.EditorID);
        Assert.Equal(priority ? 0x40000000 : 0, result.MajorRecordFlagsRaw);
        if (kind == "CELL") Assert.Empty(state.PatchMod.EnumerateMajorRecords<IPlacedObjectGetter>());
        if (kind == "REFR") Assert.Equal("Parent", Assert.Single(state.PatchMod.EnumerateMajorRecords<ICellGetter>()).EditorID);
    }

    private static void AddTypedRecord(SkyrimMod mod, string kind, string editorId, int flags)
    {
        IMajorRecord record;
        switch (kind)
        {
            case "WEAP":
                var weapon = new Weapon(Key, SkyrimRelease.SkyrimSE);
                mod.Weapons.Add(weapon);
                record = weapon;
                break;
            case "NPC_":
                var npc = new Npc(Key, SkyrimRelease.SkyrimSE);
                mod.Npcs.Add(npc);
                record = npc;
                break;
            case "GLOB":
                var global = new GlobalInt(Key, SkyrimRelease.SkyrimSE);
                mod.Globals.Add(global);
                record = global;
                break;
            default:
                var cell = new Cell(kind == "CELL" ? Key : new FormKey(Skyrim, 0x900), SkyrimRelease.SkyrimSE)
                {
                    EditorID = "Parent", Flags = Cell.Flag.IsInteriorCell
                };
                var placed = new PlacedObject(kind == "REFR" ? Key : new FormKey(Skyrim, 0x901), SkyrimRelease.SkyrimSE);
                cell.Persistent.Add(placed);
                var block = new CellBlock { BlockNumber = 1, GroupType = GroupTypeEnum.InteriorCellBlock };
                var sub = new CellSubBlock { BlockNumber = 2, GroupType = GroupTypeEnum.InteriorCellSubBlock };
                sub.Cells.Add(cell);
                block.SubBlocks.Add(sub);
                mod.Cells.Records.Add(block);
                record = kind == "CELL" ? cell : placed;
                break;
        }
        record.EditorID = editorId;
        record.MajorRecordFlagsRaw = flags;
    }

    private static void PreserveEdids() => PatcherSettings.Apply(new PatcherConfiguration
    {
        Forwarding = new ForwardingSettings { EditorIdPolicy = EditorIdForwardingPolicy.PreserveBaseline }
    });

    private static SkyrimMod Mod(ModKey modKey, string? editorId, int flags = 0)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        mod.Keywords.Add(new Keyword(Key, SkyrimRelease.SkyrimSE)
        {
            EditorID = editorId,
            MajorRecordFlagsRaw = flags,
            Color = Color.FromArgb(10, 20, 30)
        });
        return mod;
    }

    private static IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] Winners(
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state) =>
        state.LoadOrder.PriorityOrder.WinningContextOverrides<ISkyrimMod, ISkyrimModGetter, IKeyword, IKeywordGetter>(state.LinkCache)
            .ToArray();

#pragma warning disable CS0618
    private static SynthesisState<ISkyrimMod, ISkyrimModGetter> State(params SkyrimMod[] mods)
    {
        Utility.InitializeVanillaMods(mods.Select(mod => mod.ModKey), PatcherSettings.CreationClubPlugins,
            PatcherSettings.TreatCreationClubAsVanilla);
        var patch = new SkyrimMod(Patch, SkyrimRelease.SkyrimSE);
        var listings = mods.Cast<ISkyrimModGetter>().Append(patch)
            .Select(mod => new ModListing<ISkyrimModGetter>(mod)).ToArray();
        var loadOrder = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
        var cache = loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        var arguments = new RunSynthesisMutagenPatcher
        {
            OutputPath = "PolicyPatch.esp", DataFolderPath = "Data", LoadOrderFilePath = "plugins.txt",
            GameRelease = GameRelease.SkyrimSE
        };
        return new SynthesisState<ISkyrimMod, ISkyrimModGetter>(arguments,
            listings.Select(listing => new LoadOrderListing(listing.ModKey, listing.Enabled)).ToArray(),
            loadOrder, cache, null!, patch, null, null, null, CancellationToken.None, null);
    }
#pragma warning restore CS0618

    private sealed class NoHistoryKeywordHandler : KeywordRecordHandler
    {
        public int HistoryLoads { get; private set; }
        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            HistoryLoads++;
            throw new InvalidOperationException("Policy-only processing must not load the merge history.");
        }
    }
}
