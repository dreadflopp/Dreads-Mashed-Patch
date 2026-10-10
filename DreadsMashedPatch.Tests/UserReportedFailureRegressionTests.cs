using DreadsMashedPatch.PropertyHandlers.Npc;
using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using DreadsMashedPatch.Enums;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("LogCollector")]
public sealed class UserReportedFailureRegressionTests : IDisposable
{
    private static readonly ModKey Original = ModKey.FromNameAndExtension("Skyrim.esm");
    private static readonly ModKey Earlier = ModKey.FromNameAndExtension("Earlier.esp");
    private static readonly ModKey Winner = ModKey.FromNameAndExtension("Winner.esp");
    private static readonly ModKey Patch = ModKey.FromNameAndExtension("InvestigationPatch.esp");
    private static readonly FormKey Key = new(Original, 0x800);
    private static readonly FormKey PerkKey = new(Original, 0x801);

    public void Dispose()
    {
        PatcherSettings.Apply(new PatcherConfiguration());
        LogCollector.Clear();
    }

    [Theory]
    [InlineData("short", false)]
    [InlineData("float", false)]
    [InlineData("int", false)]
    [InlineData("unknown", false)]
    [InlineData("string", false)]
    [InlineData("gmst-int", false)]
    [InlineData("gmst-float", false)]
    [InlineData("gmst-bool", false)]
    [InlineData("short", true)]
    [InlineData("float", true)]
    [InlineData("int", true)]
    [InlineData("unknown", true)]
    [InlineData("string", true)]
    [InlineData("gmst-int", true)]
    [InlineData("gmst-float", true)]
    [InlineData("gmst-bool", true)]
    public void UnchangedSubtypeHistoryResolvesWithoutCastingErrors(string variant, bool binaryOverlay)
    {
        var mods = new[] { Original, Earlier, Winner }.Select(modKey => ScalarMod(modKey, variant)).ToArray();
        using var fixtures = new OverlayFixtures(mods, binaryOverlay);
        using var state = State(fixtures.Mods);
        var winner = Winners(state).Single();
        var handler = ScalarHandler(variant);
        var contexts = handler.GetRecordContexts(winner, state);
        Assert.Equal(3, contexts.Length);
        Assert.Equal(new[] { Winner, Earlier, Original }, contexts.Select(c => c.ModKey));
        Assert.All(contexts, c => Assert.Equal(Key, c.Record.FormKey));
    }

    [Theory]
    [InlineData("short", "float", false)]
    [InlineData("short", "float", true)]
    [InlineData("float", "short", true)]
    [InlineData("int", "short", true)]
    [InlineData("unknown", "float", true)]
    [InlineData("string", "gmst-int", false)]
    [InlineData("string", "gmst-int", true)]
    [InlineData("gmst-int", "string", true)]
    [InlineData("gmst-float", "gmst-int", true)]
    [InlineData("gmst-bool", "gmst-int", true)]
    public void ChangedSubtypeHistoryStartsAtLatestTypeBoundary(
        string winningVariant, string originalVariant, bool binaryOverlay)
    {
        var mods = new[] { ScalarMod(Original, originalVariant), ScalarMod(Earlier, winningVariant),
            ScalarMod(Winner, winningVariant) };
        using var fixtures = new OverlayFixtures(mods, binaryOverlay);
        using var state = State(fixtures.Mods);
        var winner = Winners(state).Single();
        var contexts = ScalarHandler(winningVariant).GetRecordContexts(winner, state);
        Assert.Equal(new[] { Winner, Earlier }, contexts.Select(c => c.ModKey));
        Assert.All(contexts, c => Assert.Equal(Key, c.Record.FormKey));
        Assert.All(contexts, c => Assert.True(GetterType(winningVariant).IsInstanceOfType(c.Record)));
    }

    [Theory]
    [InlineData("short")]
    [InlineData("float")]
    [InlineData("int")]
    [InlineData("unknown")]
    [InlineData("string")]
    [InlineData("gmst-int")]
    [InlineData("gmst-float")]
    [InlineData("gmst-bool")]
    public void FullRunPatchesAndReloadsSameTypeAfterTransition(string variant)
    {
        Utility.InitializeVanillaMods([Original], [], false);
        PatcherSettings.Apply(new PatcherConfiguration());
        var oldVariant = variant is "string" or "gmst-int" or "gmst-float" or "gmst-bool"
            ? (variant == "string" ? "gmst-int" : "string") : (variant == "float" ? "short" : "float");
        var boundary = ModKey.FromNameAndExtension("Boundary.esp");
        var mods = new[] { ScalarMod(Original, oldVariant), ScalarMod(boundary, variant),
            ScalarMod(Earlier, variant), ScalarMod(Winner, variant) };
        var registered = ScalarHandler(variant);
        var selected = SelectedData(variant);
        registered.PropertyHandlers["Data"].SetValue(mods[2].EnumerateMajorRecords().Single(), selected);
        using var fixtures = new OverlayFixtures(mods, true);
        using var stateForPatch = State(fixtures.Mods);
        var handler = ScalarHandler(variant);
        var report = Program.RunPatchWithReport(stateForPatch);
        Assert.True(report.Succeeded);
        var patched = Assert.Single(stateForPatch.PatchMod.EnumerateMajorRecords());
        Assert.True(GetterType(variant).IsInstanceOfType(patched));
        var data = handler.PropertyHandlers["Data"];
        Assert.True(data.AreValuesEqual(selected, data.GetValue(patched)));
        using var stream = new MemoryStream();
        stateForPatch.PatchMod.WriteToBinary(stream);
        stream.Position = 0;
        using var reloaded = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        var onDisk = Assert.Single(reloaded.EnumerateMajorRecords());
        Assert.True(GetterType(variant).IsInstanceOfType(onDisk));
        Assert.True(data.AreValuesEqual(selected, data.GetValue(onDisk)));
    }

    [Fact]
    public void SubtypeHistoryDoesNotRejoinOlderMatchingTypesAcrossTransition()
    {
        var mods = new[] { ScalarMod(Original, "short"), ScalarMod(Earlier, "float"), ScalarMod(Winner, "short") };
        using var fixtures = new OverlayFixtures(mods, true);
        using var state = State(fixtures.Mods);
        var handler = ScalarHandler("short");
        Assert.Equal(Winner, Assert.Single(handler.GetRecordContexts(Winners(state).Single(), state)).ModKey);
    }

    [Theory]
    [InlineData("short", "float", false)]
    [InlineData("float", "short", false)]
    [InlineData("int", "unknown", false)]
    [InlineData("unknown", "int", false)]
    [InlineData("string", "gmst-int", false)]
    [InlineData("gmst-int", "string", false)]
    [InlineData("gmst-float", "gmst-bool", false)]
    [InlineData("gmst-bool", "gmst-float", false)]
    [InlineData("short", "float", true)]
    [InlineData("string", "gmst-int", true)]
    public void IgnoredSubtypeWinnerDispatchesByEffectiveTypeAndRespectsDisabledTypes(
        string effectiveVariant, string ignoredVariant, bool disableEffectiveType)
    {
        var ignored = ModKey.FromNameAndExtension("Ignored.esp");
        var mods = new[] { ScalarMod(Original, effectiveVariant), ScalarMod(Earlier, effectiveVariant),
            ScalarMod(Winner, effectiveVariant), ScalarMod(ignored, ignoredVariant) };
        var handler = ScalarHandler(effectiveVariant);
        var selected = SelectedData(effectiveVariant);
        handler.PropertyHandlers["Data"].SetValue(mods[1].EnumerateMajorRecords().Single(), selected);
        PatcherSettings.Apply(new PatcherConfiguration
        {
            IgnoredMods = [ignored.FileName.String],
            DisabledRecordTypes = [GetterType(disableEffectiveType ? effectiveVariant : ignoredVariant).FullName!]
        });
        using var fixtures = new OverlayFixtures(mods, true);
        using var state = State(fixtures.Mods);
        var report = Program.RunPatchWithReport(state);
        Assert.True(report.Succeeded);
        if (disableEffectiveType) Assert.Empty(state.PatchMod.EnumerateMajorRecords());
        else
        {
            var patched = Assert.Single(state.PatchMod.EnumerateMajorRecords());
            Assert.True(GetterType(effectiveVariant).IsInstanceOfType(patched));
            Assert.True(handler.PropertyHandlers["Data"].AreValuesEqual(selected,
                handler.PropertyHandlers["Data"].GetValue(patched)));
            AssertReloadedData(state, effectiveVariant, selected);
        }
    }

    [Fact]
    public void IgnoredIntermediateSubtypeDoesNotResetOwnership()
    {
        var ignored = ModKey.FromNameAndExtension("Ignored.esp");
        var mods = new[] { ScalarMod(Original, "short"), ScalarMod(Earlier, "short"),
            ScalarMod(ignored, "float"), ScalarMod(Winner, "short") };
        ScalarHandler("short").PropertyHandlers["Data"].SetValue(mods[1].Globals.Single(), (short)7);
        PatcherSettings.Apply(new PatcherConfiguration { IgnoredMods = [ignored.FileName.String] });
        using var fixtures = new OverlayFixtures(mods, true);
        using var state = State(fixtures.Mods);
        Assert.True(Program.RunPatchWithReport(state).Succeeded);
        AssertReloadedData(state, "short", (short)7);
    }

    [Theory]
    [InlineData("short", "float")]
    [InlineData("float", "short")]
    [InlineData("int", "unknown")]
    [InlineData("unknown", "int")]
    [InlineData("string", "gmst-int")]
    [InlineData("gmst-int", "string")]
    [InlineData("gmst-float", "gmst-bool")]
    [InlineData("gmst-bool", "gmst-float")]
    public void PrioritySnapshotCanSelectAnotherSubtypeAndKeepsItsEditorId(string winningVariant, string priorityVariant)
    {
        var mods = new[] { ScalarMod(Original, winningVariant), ScalarMod(Earlier, priorityVariant), ScalarMod(Winner, winningVariant) };
        var selected = SelectedData(priorityVariant);
        ScalarHandler(priorityVariant).PropertyHandlers["Data"].SetValue(mods[1].EnumerateMajorRecords().Single(), selected);
        mods[1].EnumerateMajorRecords().Single().MajorRecordFlagsRaw = 0x40000000;
        PatcherSettings.Apply(new PatcherConfiguration
        {
            AlwaysWinningMods = [Earlier.FileName.String],
            Forwarding = new ForwardingSettings { EditorIdPolicy = EditorIdForwardingPolicy.PreserveBaseline }
        });
        using var fixtures = new OverlayFixtures(mods, true);
        using var state = State(fixtures.Mods);
        Assert.True(Program.RunPatchWithReport(state).Succeeded);
        var patched = Assert.Single(state.PatchMod.EnumerateMajorRecords());
        Assert.Equal(mods[1].EnumerateMajorRecords().Single().EditorID, patched.EditorID);
        Assert.Equal(0x40000000, patched.MajorRecordFlagsRaw);
        AssertReloadedData(state, priorityVariant, selected);
    }

    [Theory]
    [InlineData("string", "gmst-int", false)]
    [InlineData("gmst-int", "string", false)]
    [InlineData("gmst-float", "gmst-bool", false)]
    [InlineData("gmst-bool", "gmst-float", false)]
    [InlineData("string", "string", true)]
    [InlineData("gmst-int", "gmst-int", true)]
    [InlineData("gmst-float", "gmst-float", true)]
    [InlineData("gmst-bool", "gmst-bool", true)]
    public void PreservedBaselineGameSettingNameMustMatchSelectedSubtype(
        string winningVariant, string officialVariant, bool shouldPreserve)
    {
        var mods = new[] { ScalarMod(Original, officialVariant), ScalarMod(Winner, winningVariant) };
        var officialName = mods[0].GameSettings.Single().EditorID!;
        mods[1].GameSettings.Single().EditorID += "Renamed";
        PatcherSettings.Apply(new PatcherConfiguration
        {
            Forwarding = new ForwardingSettings { EditorIdPolicy = EditorIdForwardingPolicy.PreserveBaseline }
        });
        using var fixtures = new OverlayFixtures(mods, true);
        using var state = State(fixtures.Mods);
        Assert.True(Program.RunPatchWithReport(state).Succeeded);
        if (!shouldPreserve) Assert.Empty(state.PatchMod.GameSettings);
        else
        {
            Assert.Equal(officialName, Assert.Single(state.PatchMod.GameSettings).EditorID);
            AssertReloadedData(state, winningVariant,
                ScalarHandler(winningVariant).PropertyHandlers["Data"].GetValue(mods[1].GameSettings.Single())!);
        }
    }

    [Theory]
    [InlineData("string", "gmst-int")]
    [InlineData("gmst-int", "string")]
    [InlineData("gmst-float", "gmst-bool")]
    [InlineData("gmst-bool", "gmst-float")]
    public void OrdinaryMergeKeepsCompatibleGameSettingNameWhenOfficialSubtypeDiffers(string variant, string officialVariant)
    {
        var boundary = ModKey.FromNameAndExtension("Boundary.esp");
        var mods = new[] { ScalarMod(Original, officialVariant), ScalarMod(boundary, variant),
            ScalarMod(Earlier, variant), ScalarMod(Winner, variant) };
        var selected = SelectedData(variant);
        ScalarHandler(variant).PropertyHandlers["Data"].SetValue(mods[2].GameSettings.Single(), selected);
        PatcherSettings.Apply(new PatcherConfiguration
        {
            Forwarding = new ForwardingSettings { EditorIdPolicy = EditorIdForwardingPolicy.PreserveBaseline }
        });
        using var fixtures = new OverlayFixtures(mods, true);
        using var state = State(fixtures.Mods);
        Assert.True(Program.RunPatchWithReport(state).Succeeded);
        Assert.Equal(mods[3].GameSettings.Single().EditorID, Assert.Single(state.PatchMod.GameSettings).EditorID);
        AssertReloadedData(state, variant, selected);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void PerkRankRestorationStillRequiresOwnershipPermission(bool declaresOwner)
    {
        var mods = new[] { Original, Earlier, Winner }.Select(modKey =>
        {
            var mod = NewMod(modKey);
            if (modKey == Original) mod.Perks.Add(new Perk(PerkKey, SkyrimRelease.SkyrimSE));
            var rows = Perks().Cast<PerkPlacement>().ToList();
            rows[0].Rank = modKey == Earlier ? (byte)2 : (byte)1;
            mod.Npcs.Add(new Npc(Key, SkyrimRelease.SkyrimSE) { Perks = new ExtendedList<PerkPlacement>(rows) });
            return mod;
        }).ToArray();
        if (declaresOwner) mods[2].ModHeader.MasterReferences.Add(new MasterReference { Master = Earlier });
        using var fixtures = new OverlayFixtures(mods, true);
        Assert.Equal(declaresOwner, fixtures.Mods[2].MasterReferences.Any(m => m.Master == Earlier));
        using var state = State(fixtures.Mods);
        Assert.True(Program.RunPatchWithReport(state).Succeeded);
        if (declaresOwner) Assert.Empty(state.PatchMod.Npcs);
        else Assert.Equal(2, Assert.Single(Assert.Single(state.PatchMod.Npcs).Perks!).Rank);
    }

    [Theory]
    [InlineData("null", "absent")]
    [InlineData("empty", "absent")]
    [InlineData("addition", "absent")]
    [InlineData("null", "present")]
    [InlineData("empty", "present")]
    [InlineData("addition", "present")]
    public void PerkSetterPreservesSelectedPresenceAndDetachedRows(string selection, string destination)
    {
        var npc = new Npc(Key, SkyrimRelease.SkyrimSE);
        if (destination == "present") npc.Perks = new ExtendedList<PerkPlacement>(Perks().Cast<PerkPlacement>());
        List<IPerkPlacementGetter>? selected = selection switch { "null" => null, "empty" => [], _ => Perks() };
        var handler = new PerksHandler();
        handler.SetValue(npc, selected);
        Assert.True(handler.AreValuesEqual(selected, handler.GetValue(npc)));
        if (selected is { Count: > 0 })
        {
            Assert.NotSame(selected[0], npc.Perks![0]);
            npc.Perks[0].Rank = 9;
            npc.Perks[0].Perk.SetTo(new FormKey(Original, 0x802));
            Assert.Equal(2, selected[0].Rank);
            Assert.Equal(PerkKey, selected[0].Perk.FormKey);
        }
    }

    [Fact]
    public void PerkSetterRetainsEmptySelectionAndRemovesNullSelection()
    {
        var handler = new PerksHandler();
        var absent = new Npc(Key, SkyrimRelease.SkyrimSE);
        handler.SetValue(absent, []);
        Assert.NotNull(absent.Perks);
        Assert.Empty(absent.Perks);
        var present = new Npc(Key, SkyrimRelease.SkyrimSE) { Perks = new ExtendedList<PerkPlacement>(Perks().Cast<PerkPlacement>()) };
        handler.SetValue(present, null);
        Assert.Null(present.Perks);
        Assert.True(handler.AreValuesEqual(null, handler.GetValue(present)));
    }

    [Fact]
    public void PerkSetterAppliesAdditionsWhenDestinationListIsAbsent()
    {
        var npc = new Npc(Key, SkyrimRelease.SkyrimSE);
        var selected = Perks();
        var handler = new PerksHandler();
        Assert.Null(npc.Perks);

        handler.SetValue(npc, selected);

        Assert.True(handler.AreValuesEqual(selected, handler.GetValue(npc)));
        var recordHandler = new NpcRecordHandler();
        recordHandler.ApplyForwardedProperties(npc, new() { ["Perks"] = selected });
        Assert.True(handler.AreValuesEqual(selected, handler.GetValue(npc)));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FullRunForwardsPerkAdditionsIntoAbsentWinnerList(bool binaryOverlay)
    {
        Utility.InitializeVanillaMods([Original], [], false);
        PatcherSettings.Apply(new PatcherConfiguration());
        var mods = new[] { Original, Earlier, Winner }.Select(modKey =>
        {
            var mod = NewMod(modKey);
            if (modKey == Original) mod.Perks.Add(new Perk(PerkKey, SkyrimRelease.SkyrimSE));
            var npc = new Npc(Key, SkyrimRelease.SkyrimSE) { EditorID = "ReportedNpc" };
            if (modKey == Earlier) npc.Perks = new ExtendedList<PerkPlacement>(Perks().Cast<PerkPlacement>());
            mod.Npcs.Add(npc);
            return mod;
        }).ToArray();
        using var fixtures = new OverlayFixtures(mods, binaryOverlay);
        using var stateForPatch = State(fixtures.Mods);
        var handler = new NpcRecordHandler();
        var report = Program.RunPatchWithReport(stateForPatch);
        Assert.True(report.Succeeded);
        var patched = Assert.Single(stateForPatch.PatchMod.Npcs);
        Assert.True(handler.PropertyHandlers["Perks"].AreValuesEqual(Perks(),
            handler.PropertyHandlers["Perks"].GetValue(patched)));
        Assert.Null(mods[0].Npcs[Key].Perks);
        Assert.Null(mods[2].Npcs[Key].Perks);
        Assert.NotSame(mods[1].Npcs[Key].Perks![0], patched.Perks![0]);
        using var stream = new MemoryStream();
        stateForPatch.PatchMod.WriteToBinary(stream);
        stream.Position = 0;
        using var reloaded = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        var perk = Assert.Single(Assert.Single(reloaded.Npcs).Perks!);
        Assert.Equal(PerkKey, perk.Perk.FormKey);
        Assert.Equal(2, perk.Rank);
    }

    private static List<IPerkPlacementGetter> Perks() =>
        [new PerkPlacement { Perk = new FormLink<IPerkGetter>(PerkKey), Rank = 2 }];

    private static object SelectedData(string variant) => variant switch
    {
        "short" => (object)(short)7, "int" or "gmst-int" => 7,
        "float" or "unknown" or "gmst-float" => 7.5f, "gmst-bool" => true,
        _ => new Mutagen.Bethesda.Strings.TranslatedString(Mutagen.Bethesda.Strings.Language.English, "selected")
    };

    private static void AssertReloadedData(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, string variant, object selected)
    {
        using var stream = new MemoryStream();
        state.PatchMod.WriteToBinary(stream);
        stream.Position = 0;
        using var reloaded = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        var onDisk = Assert.Single(reloaded.EnumerateMajorRecords());
        Assert.True(GetterType(variant).IsInstanceOfType(onDisk));
        var handler = ScalarHandler(variant).PropertyHandlers["Data"];
        Assert.True(handler.AreValuesEqual(selected, handler.GetValue(onDisk)));
    }

    private static AbstractRecordHandler ScalarHandler(string variant) => variant switch
    {
        "short" => new GlobalShortRecordHandler(),
        "float" => new GlobalFloatRecordHandler(),
        "int" => new GlobalIntRecordHandler(),
        "unknown" => new GlobalUnknownRecordHandler(),
        "string" => new GameSettingStringRecordHandler(),
        "gmst-int" => new GameSettingIntRecordHandler(),
        "gmst-float" => new GameSettingFloatRecordHandler(),
        "gmst-bool" => new GameSettingBoolRecordHandler(),
        _ => throw new ArgumentOutOfRangeException(nameof(variant))
    };

    private static Type GetterType(string variant) => variant switch
    {
        "short" => typeof(IGlobalShortGetter), "float" => typeof(IGlobalFloatGetter),
        "int" => typeof(IGlobalIntGetter), "unknown" => typeof(IGlobalUnknownGetter),
        "string" => typeof(IGameSettingStringGetter), "gmst-int" => typeof(IGameSettingIntGetter),
        "gmst-float" => typeof(IGameSettingFloatGetter), "gmst-bool" => typeof(IGameSettingBoolGetter),
        _ => throw new ArgumentOutOfRangeException(nameof(variant))
    };

    private static SkyrimMod ScalarMod(ModKey modKey, string variant)
    {
        var mod = NewMod(modKey);
        switch (variant)
        {
            case "short": mod.Globals.Add(new GlobalShort(Key, SkyrimRelease.SkyrimSE) { Data = 1 }); break;
            case "float": mod.Globals.Add(new GlobalFloat(Key, SkyrimRelease.SkyrimSE) { Data = 1.5f }); break;
            case "int": mod.Globals.Add(new GlobalInt(Key, SkyrimRelease.SkyrimSE) { Data = 1 }); break;
            case "unknown": mod.Globals.Add(new GlobalUnknown(Key, SkyrimRelease.SkyrimSE) { TypeChar = 'x', Data = 1.5f }); break;
            case "string": mod.GameSettings.Add(new GameSettingString(Key, SkyrimRelease.SkyrimSE) { EditorID = "sInvestigation", Data = "test" }); break;
            case "gmst-int": mod.GameSettings.Add(new GameSettingInt(Key, SkyrimRelease.SkyrimSE) { EditorID = "iInvestigation", Data = 1 }); break;
            case "gmst-float": mod.GameSettings.Add(new GameSettingFloat(Key, SkyrimRelease.SkyrimSE) { EditorID = "fInvestigation", Data = 1.5f }); break;
            case "gmst-bool": mod.GameSettings.Add(new GameSettingBool(Key, SkyrimRelease.SkyrimSE) { EditorID = "bInvestigation", Data = false }); break;
            default: throw new ArgumentOutOfRangeException(nameof(variant));
        }
        return mod;
    }

    private static SkyrimMod NewMod(ModKey modKey)
    {
        var mod = new SkyrimMod(modKey, SkyrimRelease.SkyrimSE);
        if (modKey != Original) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = Original });
        return mod;
    }

    private sealed class OverlayFixtures : IDisposable
    {
        private readonly List<MemoryStream> _streams = [];
        private readonly List<IDisposable> _overlays = [];
        public ISkyrimModGetter[] Mods { get; }

        public OverlayFixtures(SkyrimMod[] mods, bool binaryOverlay)
        {
            Mods = mods.Select(mod =>
            {
                if (!binaryOverlay) return (ISkyrimModGetter)mod;
                var stream = new MemoryStream();
                _streams.Add(stream);
                // Retain deliberately declared permission relationships even when
                // no FormLink currently requires that master in the synthetic record.
                mod.WriteToBinary(stream, new BinaryWriteParameters { MastersListContent = MastersListContentOption.NoCheck });
                stream.Position = 0;
                var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey);
                _overlays.Add(overlay);
                return overlay;
            }).ToArray();
        }

        public void Dispose()
        {
            foreach (var overlay in _overlays) overlay.Dispose();
            foreach (var stream in _streams) stream.Dispose();
        }
    }

    private static IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] Winners(
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state) => state.LoadOrder.PriorityOrder
        .WinningContextOverrides<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(state.LinkCache).ToArray();

#pragma warning disable CS0618
    private static SynthesisState<ISkyrimMod, ISkyrimModGetter> State(params ISkyrimModGetter[] mods)
    {
        var patch = new SkyrimMod(Patch, SkyrimRelease.SkyrimSE);
        var listings = mods.Append(patch).Select(mod => new ModListing<ISkyrimModGetter>(mod)).ToArray();
        var order = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
        var args = new RunSynthesisMutagenPatcher
        {
            OutputPath = Path.Combine(Path.GetTempPath(), "InvestigationPatch.esp"),
            DataFolderPath = Path.GetTempPath(), LoadOrderFilePath = Path.Combine(Path.GetTempPath(), "plugins.txt"),
            GameRelease = GameRelease.SkyrimSE
        };
        return new SynthesisState<ISkyrimMod, ISkyrimModGetter>(args,
            listings.Select(l => new LoadOrderListing(l.ModKey, l.Enabled)).ToArray(), order,
            order.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(), null!, patch, null, null, null,
            CancellationToken.None, null);
    }
#pragma warning restore CS0618
}
