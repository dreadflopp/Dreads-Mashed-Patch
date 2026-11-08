using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("CoverageFixes")]
public sealed class OutputPatchValidatorTests : IDisposable
{
    private static FormKey Key(uint id) => new(CoverageTestHistory.Original, id);
    private static readonly FormKey Actor = Key(0x800);
    private static readonly FormKey Target = Key(0x801);
    public OutputPatchValidatorTests() => CoverageTestHistory.Configure();
    public void Dispose() => CoverageTestHistory.Reset();

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FullRunOmitsReintroducedMissingPackageAndKeepsHealthyOverride(bool priority)
    {
        if (priority) PatcherSettings.Apply(new PatcherConfiguration { AlwaysWinningMods = [CoverageTestHistory.Earlier.FileName.String] });
        var mods = CoverageTestHistory.Mods();
        foreach (var mod in mods)
        {
            mod.Npcs.Add(new Npc(Actor, SkyrimRelease.SkyrimSE) { EditorID = "ReportedNpc" });
            mod.Weapons.Add(new Weapon(Key(0x810), SkyrimRelease.SkyrimSE)
            { Name = mod == mods[1] ? "Forward this name" : "Original name" });
        }
        mods[1].Npcs[Actor].Packages.Add(new FormLink<IPackageGetter>(Target));
        using var state = CoverageTestHistory.State(mods);
        var previous = Console.Out;
        using var log = new StringWriter();
        PatchRunReport report;
        try { Console.SetOut(log); report = Program.RunPatchWithReport(state); }
        finally { Console.SetOut(previous); }

        Assert.True(report.Succeeded);
        Assert.True(report.IsPartial);
        Assert.Empty(report.Errors);
        Assert.Equal(Actor, Assert.Single(report.SkippedRecords).Record);
        Assert.Empty(state.PatchMod.Npcs);
        Assert.Equal("Forward this name", Assert.Single(state.PatchMod.Weapons).Name!.String);
        Assert.Single(mods[1].Npcs[Actor].Packages);
        Assert.Empty(mods[2].Npcs[Actor].Packages);
        Assert.Contains("[Warning] Omitted output override", log.ToString());
        Assert.Contains("ReportedNpc", log.ToString());
        Assert.Contains(Actor.ToString(), log.ToString());
        Assert.Contains(Target.ToString(), log.ToString());
        Assert.Contains("Packages[0]", log.ToString());
        Assert.Contains("Earlier.esp", log.ToString());
        Assert.Contains("Repair or replace", log.ToString());
        Assert.DoesNotContain("[Error]", log.ToString());
        Assert.Single(log.ToString().Split(Environment.NewLine),
            line => LogCollector.IsDiagnostic(line, "Warning") && line.Contains(Actor.ToString(), StringComparison.Ordinal));
        using var stream = new MemoryStream();
        state.PatchMod.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
        Assert.Empty(overlay.Npcs);
        Assert.Single(overlay.Weapons);
    }

    [Theory]
    [InlineData("wrong-type")]
    [InlineData("deleted-package")]
    [InlineData("nested-perk")]
    public void InvalidReferencesRejectWholeOverrideWithSpecificDiagnostics(string kind)
    {
        var mod = CoverageTestHistory.Mods()[0];
        var npc = new Npc(Actor, SkyrimRelease.SkyrimSE);
        mod.Npcs.Add(npc);
        switch (kind)
        {
            case "wrong-type":
                mod.Weapons.Add(new Weapon(Target, SkyrimRelease.SkyrimSE));
                npc.Packages.Add(new FormLink<IPackageGetter>(Target));
                break;
            case "deleted-package":
                mod.Packages.Add(new Package(Target, SkyrimRelease.SkyrimSE) { IsDeleted = true });
                npc.Packages.Add(new FormLink<IPackageGetter>(Target));
                break;
            default:
                npc.Perks = [new PerkPlacement { Perk = new FormLink<IPerkGetter>(Target), Rank = 1 }];
                break;
        }
        using var state = CoverageTestHistory.State(mod);
        state.PatchMod.Npcs.Add(npc.DeepCopy());
        var (report, log) = Validate(state);
        Assert.Empty(state.PatchMod.Npcs);
        Assert.Single(mod.Npcs);
        Assert.Single(report.SkippedRecords);
        Assert.Empty(report.Errors);
        Assert.Contains(Target.ToString(), log);
        Assert.Contains(kind == "nested-perk" ? "Perks[0].Perk" : "Packages[0]", log);
        if (kind == "deleted-package") Assert.Contains("deleted", log);
    }

    [Fact]
    public void EnginePlayerEnableParentIsValidWithoutAPluginRecordAndSurvivesWriting()
    {
        using var state = CoverageTestHistory.State(CoverageTestHistory.Mods()[0]);
        Assert.False(state.LinkCache.TryResolveSimpleContext(Constants.Player.FormKey, typeof(IPlacedGetter), out _));
        var placed = new PlacedObject(Actor, SkyrimRelease.SkyrimSE)
        {
            EnableParent = new EnableParent
            {
                Reference = new FormLink<IPlacedGetter>(Constants.Player.FormKey),
                Flags = EnableParent.Flag.SetEnableStateToOppositeOfParent
            }
        };
        AddPlacedRecord(state.PatchMod, placed);

        var (report, log) = Validate(state);

        Assert.False(report.IsPartial);
        Assert.DoesNotContain("[Warning]", log);
        using var stream = new MemoryStream();
        state.PatchMod.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
        var written = Assert.Single(overlay.EnumerateMajorRecords<IPlacedObjectGetter>());
        Assert.Equal(Constants.Player.FormKey, written.EnableParent!.Reference.FormKey);
        Assert.Equal(EnableParent.Flag.SetEnableStateToOppositeOfParent, written.EnableParent.Flags);
    }

    [Fact]
    public void EnginePlayerIsAlsoValidInAnUntypedScriptObjectProperty()
    {
        using var state = CoverageTestHistory.State(CoverageTestHistory.Mods()[0]);
        var npc = new Npc(Actor, SkyrimRelease.SkyrimSE) { VirtualMachineAdapter = new VirtualMachineAdapter() };
        var script = new ScriptEntry { Name = "Example" };
        script.Properties.Add(new ScriptObjectProperty
        { Name = "Player", Object = new FormLink<ISkyrimMajorRecordGetter>(Constants.Player.FormKey) });
        npc.VirtualMachineAdapter.Scripts.Add(script);
        state.PatchMod.Npcs.Add(npc);
        var (report, log) = Validate(state);
        Assert.False(report.IsPartial);
        Assert.DoesNotContain("[Warning]", log);
        Assert.Same(npc, Assert.Single(state.PatchMod.Npcs));
    }

    [Theory]
    [InlineData(false, 0x15)]
    [InlineData(false, 0x700)]
    [InlineData(true, 0x14)]
    public void MissingLowFormIdsAreNotExemptedByTheEnginePlayerRule(bool anotherPlugin, int id)
    {
        using var state = CoverageTestHistory.State(CoverageTestHistory.Mods()[0]);
        var key = new FormKey(anotherPlugin ? ModKey.FromNameAndExtension("Another.esp") : Constants.Skyrim, (uint)id);
        AddPlacedRecord(state.PatchMod, new PlacedObject(Actor, SkyrimRelease.SkyrimSE)
        { EnableParent = new EnableParent { Reference = new FormLink<IPlacedGetter>(key) } });
        var (report, log) = Validate(state);
        Assert.Equal(Actor, Assert.Single(report.SkippedRecords).Record);
        Assert.Empty(state.PatchMod.EnumerateMajorRecords<IPlacedGetter>());
        Assert.Contains(key.ToString(), log);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EnginePlayerCannotBeUsedAsAPackageOrAnNpcBase(bool npcBase)
    {
        using var state = CoverageTestHistory.State(CoverageTestHistory.Mods()[0]);
        var npc = new Npc(Actor, SkyrimRelease.SkyrimSE);
        if (npcBase) npc.Template.SetTo(Constants.Player.FormKey);
        else npc.Packages.Add(new FormLink<IPackageGetter>(Constants.Player.FormKey));
        state.PatchMod.Npcs.Add(npc);
        var (report, log) = Validate(state);
        Assert.Equal(Actor, Assert.Single(report.SkippedRecords).Record);
        Assert.Empty(state.PatchMod.Npcs);
        Assert.Contains(Constants.Player.FormKey.ToString(), log);
    }

    [Fact]
    public void PermittedNullsUnionTargetsAndUnknownFlagsSurviveValidation()
    {
        var mod = CoverageTestHistory.Mods()[0];
        mod.Weapons.Add(new Weapon(Target, SkyrimRelease.SkyrimSE));
        var list = new LeveledItem(Actor, SkyrimRelease.SkyrimSE)
        { Entries = [Item(Target)], MajorRecordFlagsRaw = unchecked((int)0x80000000) };
        mod.LeveledItems.Add(list);
        var npc = new Npc(Key(0x802), SkyrimRelease.SkyrimSE); // Default optional/null links are legal here.
        using var state = CoverageTestHistory.State(mod);
        state.PatchMod.LeveledItems.Add(list.DeepCopy());
        state.PatchMod.Npcs.Add(npc);
        var (report, log) = Validate(state);
        Assert.False(report.IsPartial);
        Assert.DoesNotContain("[Warning]", log);
        Assert.Equal(unchecked((int)0x80000000), Assert.Single(state.PatchMod.LeveledItems).MajorRecordFlagsRaw);
        Assert.Single(state.PatchMod.Npcs);
    }

    [Fact]
    public void RejectedTargetExposesDeletedInputWinnerAndRemainingOutputIsRechecked()
    {
        var mod = CoverageTestHistory.Mods()[0];
        mod.Npcs.Add(new Npc(Target, SkyrimRelease.SkyrimSE) { IsDeleted = true });
        using var state = CoverageTestHistory.State(mod);
        var npc = new Npc(Target, SkyrimRelease.SkyrimSE);
        npc.Race.SetTo(Key(0xBAD));
        state.PatchMod.Npcs.Add(npc);
        state.PatchMod.LeveledNpcs.Add(new LeveledNpc(Actor, SkyrimRelease.SkyrimSE)
        { Entries = [new LeveledNpcEntry { Data = new LeveledNpcEntryData
            { Reference = new FormLink<INpcSpawnGetter>(Target), Count = 1, Level = 1 } }] });
        var (report, _) = Validate(state);
        Assert.Equal(2, report.SkippedRecords.Count);
        Assert.Empty(state.PatchMod.Npcs);
        Assert.Empty(state.PatchMod.LeveledNpcs);
        Assert.True(mod.Npcs[Target].IsDeleted);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CyclesThroughUnchangedInputListsOmitOnlyParticipatingOutput(bool selfCycle)
    {
        var mod = CoverageTestHistory.Mods()[0];
        mod.LeveledItems.Add(new LeveledItem(Actor, SkyrimRelease.SkyrimSE));
        mod.LeveledItems.Add(new LeveledItem(Target, SkyrimRelease.SkyrimSE) { Entries = [Item(Actor)] });
        mod.LeveledItems.Add(new LeveledItem(Key(0x802), SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mod);
        state.PatchMod.LeveledItems.Add(new LeveledItem(Actor, SkyrimRelease.SkyrimSE)
        { Entries = [Item(selfCycle ? Actor : Target)] });
        state.PatchMod.LeveledItems.Add(new LeveledItem(Key(0x802), SkyrimRelease.SkyrimSE));
        var (report, log) = Validate(state);
        Assert.Equal(Actor, Assert.Single(report.SkippedRecords).Record);
        Assert.Equal(Key(0x802), Assert.Single(state.PatchMod.LeveledItems).FormKey);
        Assert.Contains("circular leveled-list", log);
        Assert.Empty(mod.LeveledItems[Actor].Entries ?? []);
        Assert.Single(mod.LeveledItems[Target].Entries!);
    }

    [Fact]
    public void EveryPatchedMemberOfCycleIsOmittedButSharedAcyclicBranchesRemain()
    {
        var mod = CoverageTestHistory.Mods()[0];
        foreach (var id in new uint[] { 0x800, 0x801, 0x802, 0x803 })
            mod.LeveledItems.Add(new LeveledItem(Key(id), SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mod);
        state.PatchMod.LeveledItems.Add(new LeveledItem(Actor, SkyrimRelease.SkyrimSE) { Entries = [Item(Target), Item(Key(0x803))] });
        state.PatchMod.LeveledItems.Add(new LeveledItem(Target, SkyrimRelease.SkyrimSE) { Entries = [Item(Actor)] });
        state.PatchMod.LeveledItems.Add(new LeveledItem(Key(0x802), SkyrimRelease.SkyrimSE) { Entries = [Item(Key(0x803))] });
        var (report, _) = Validate(state);
        Assert.Equal(2, report.SkippedRecords.Count);
        Assert.Equal(Key(0x802), Assert.Single(state.PatchMod.LeveledItems).FormKey);
    }

    [Fact]
    public void RemovingInvalidOverrideCannotExposeAnUncheckedCycle()
    {
        var mod = CoverageTestHistory.Mods()[0];
        mod.LeveledItems.Add(new LeveledItem(Actor, SkyrimRelease.SkyrimSE) { Entries = [Item(Target)] });
        mod.LeveledItems.Add(new LeveledItem(Target, SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mod);
        var first = new LeveledItem(Actor, SkyrimRelease.SkyrimSE);
        first.Global.SetTo(Key(0xBAD));
        state.PatchMod.LeveledItems.Add(first);
        state.PatchMod.LeveledItems.Add(new LeveledItem(Target, SkyrimRelease.SkyrimSE) { Entries = [Item(Actor)] });
        var (report, log) = Validate(state);
        Assert.Equal(2, report.SkippedRecords.Count);
        Assert.Empty(state.PatchMod.LeveledItems);
        Assert.Contains("circular leveled-list", log);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NpcAndSpellListCyclesUseTheirOwnRecordTypes(bool spell)
    {
        using var state = CoverageTestHistory.State(CoverageTestHistory.Mods()[0]);
        if (spell) state.PatchMod.LeveledSpells.Add(new LeveledSpell(Actor, SkyrimRelease.SkyrimSE)
            { Entries = [new LeveledSpellEntry { Data = new LeveledSpellEntryData
                { Reference = new FormLink<ISpellRecordGetter>(Actor), Count = 1, Level = 1 } }] });
        else state.PatchMod.LeveledNpcs.Add(new LeveledNpc(Actor, SkyrimRelease.SkyrimSE)
            { Entries = [new LeveledNpcEntry { Data = new LeveledNpcEntryData
                { Reference = new FormLink<INpcSpawnGetter>(Actor), Count = 1, Level = 1 } }] });
        var (report, log) = Validate(state);
        Assert.Single(report.SkippedRecords);
        Assert.Empty(state.PatchMod.EnumerateMajorRecords());
        Assert.Contains("circular leveled-list", log);
    }

    [Theory]
    [InlineData("item", 255)]
    [InlineData("item", 256)]
    [InlineData("npc", 256)]
    [InlineData("spell", 256)]
    public void LeveledListCounterLimitRejectsWithoutTruncating(string kind, int count)
    {
        var mod = CoverageTestHistory.Mods()[0];
        mod.Weapons.Add(new Weapon(Target, SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mod);
        switch (kind)
        {
            case "item": state.PatchMod.LeveledItems.Add(new LeveledItem(Actor, SkyrimRelease.SkyrimSE)
                { Entries = new ExtendedList<LeveledItemEntry>(Enumerable.Range(0, count).Select(_ => Item(Target))) }); break;
            case "npc": state.PatchMod.LeveledNpcs.Add(new LeveledNpc(Actor, SkyrimRelease.SkyrimSE)
                { Entries = new ExtendedList<LeveledNpcEntry>(Enumerable.Range(0, count).Select(_ => new LeveledNpcEntry())) }); break;
            default: state.PatchMod.LeveledSpells.Add(new LeveledSpell(Actor, SkyrimRelease.SkyrimSE)
                { Entries = new ExtendedList<LeveledSpellEntry>(Enumerable.Range(0, count).Select(_ => new LeveledSpellEntry())) }); break;
        }
        var (report, log) = Validate(state);
        Assert.Equal(count > 255, report.IsPartial);
        Assert.Equal(count > 255 ? 0 : 1, state.PatchMod.EnumerateMajorRecords().Count());
        if (count > 255) Assert.Contains("LLCT", log);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void NestedRemovalPreservesSiblingsAndReportsChildrenOfRejectedParent(bool invalidParent)
    {
        var mod = CoverageTestHistory.Mods()[0];
        using var state = CoverageTestHistory.State(mod);
        var bad = new PlacedObject(Actor, SkyrimRelease.SkyrimSE);
        bad.Base.SetTo(Target);
        var healthy = new PlacedNpc(Key(0x802), SkyrimRelease.SkyrimSE);
        AddPlacedRecord(state.PatchMod, bad);
        var cell = state.PatchMod.EnumerateMajorRecords<ICell>().Single();
        cell.Temporary.Add(healthy);
        if (invalidParent) cell.Location.SetTo(Key(0xBAD));
        var (report, log) = Validate(state);
        Assert.Equal(invalidParent ? 3 : 1, report.SkippedRecords.Count);
        Assert.Equal(invalidParent ? 0 : 2, state.PatchMod.EnumerateMajorRecords().Count());
        if (!invalidParent) Assert.Equal(healthy.FormKey, Assert.Single(cell.Temporary).FormKey);
        Assert.Contains(Actor.ToString(), log);
    }

    [Fact]
    public void DeletedOverridesDoNotValidateUnusedResidualPayload()
    {
        using var state = CoverageTestHistory.State(CoverageTestHistory.Mods()[0]);
        var npc = new Npc(Actor, SkyrimRelease.SkyrimSE) { IsDeleted = true };
        npc.Packages.Add(new FormLink<IPackageGetter>(Target));
        state.PatchMod.Npcs.Add(npc);
        var (report, _) = Validate(state);
        Assert.False(report.IsPartial);
        Assert.True(Assert.Single(state.PatchMod.Npcs).IsDeleted);
    }

    public static IEnumerable<object[]> UntypedFactionRankCases() =>
        from kind in new[] { "npc", "container", "leveled-item", "leveled-npc" }
        from lightSource in new[] { false, true }
        from rank in new (uint Id, int Expected)[]
        {
            (0u, 0), (1u, 1), (6u, 6), (0xFFFu, 4095), (0x2710u, 10000),
            (0x7FFFFFu, 8388607), (0x800000u, 8388608), (0x800001u, -8388607),
            (0xFFD8F0u, -10000), (0xFFFC18u, -1000), (0xFFFF7Fu, -129),
            (0xFFFF80u, -128), (0xFFFFFEu, -2), (0xFFFFFFu, -1)
        }
        select new object[] { kind, lightSource, rank.Id, rank.Expected };

    [Theory]
    [MemberData(nameof(UntypedFactionRankCases))]
    public void UntypedFactionRanksAreNormalizedAndSurviveBinaryOutput(string kind, bool lightSource, uint id, int expected)
    {
        var mod = CoverageTestHistory.Mods()[0];
        if (lightSource) mod.ModHeader.Flags |= SkyrimModHeader.HeaderFlag.Small;
        mod.Factions.Add(new Faction(Key(0xDB1), SkyrimRelease.SkyrimSE));
        mod.Weapons.Add(new Weapon(Target, SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mod);
        var raw = new UntypedOwner
        {
            OwnerData = new FormLink<ISkyrimMajorRecordGetter>(Key(0xDB1)),
            VariableData = new FormLink<ISkyrimMajorRecordGetter>(id == 0 ? FormKey.Null : new FormKey(CoverageTestHistory.Earlier, id))
        };
        AddOwnerRecord(state.PatchMod, kind, raw);
        var (report, log) = Validate(state);
        Assert.False(report.IsPartial);
        Assert.Single(state.PatchMod.EnumerateMajorRecords());
        var converted = Assert.IsType<FactionOwner>(OwnerData(Assert.Single(state.PatchMod.EnumerateMajorRecords())).Owner);
        Assert.Equal(Key(0xDB1), converted.Faction.FormKey);
        Assert.Equal(expected, converted.RequiredRank);
        Assert.Equal(1, OwnerData(Assert.Single(state.PatchMod.EnumerateMajorRecords())).ItemCondition);
        Assert.DoesNotContain("[Warning]", log);
        Assert.Equal(id, raw.VariableData.FormKey.ID);
        var directory = Path.Combine(Path.GetTempPath(), "Dmp-Coed-Normalized-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var path = Path.Combine(directory, state.PatchMod.ModKey.FileName.String);
            state.PatchMod.WriteToBinary(path);
            using var reloaded = SkyrimMod.CreateFromBinaryOverlay(path, SkyrimRelease.SkyrimSE,
                new BinaryReadParameters { LinkCache = state.LinkCache });
            var written = Assert.IsAssignableFrom<IFactionOwnerGetter>(OwnerData(Assert.Single(reloaded.EnumerateMajorRecords())).Owner);
            Assert.Equal(expected, written.RequiredRank);
            Assert.Equal(Key(0xDB1), written.Faction.FormKey);
            // The converted scalar no longer adds its mistaken plugin as a master.
            Assert.DoesNotContain(reloaded.MasterReferences, m => m.Master == CoverageTestHistory.Earlier);
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    [Theory]
    [InlineData("npc", 0xFFFFFFu)]
    [InlineData("npc", 6u)]
    [InlineData("npc", 0xFFFu)]
    [InlineData("missing", 0xFFFFFFu)]
    [InlineData("missing", 6u)]
    [InlineData("weapon", 0xFFFFFFu)]
    [InlineData("weapon", 6u)]
    public void FactionRankWorkaroundDoesNotConvertOtherOwners(string ownerType, uint value)
    {
        var mod = CoverageTestHistory.Mods()[0];
        var ownerKey = Key(0xDB1);
        if (ownerType == "npc") mod.Npcs.Add(new Npc(ownerKey, SkyrimRelease.SkyrimSE));
        else if (ownerType == "weapon") mod.Weapons.Add(new Weapon(ownerKey, SkyrimRelease.SkyrimSE));
        mod.Weapons.Add(new Weapon(Target, SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mod);
        AddOwnerRecord(state.PatchMod, "npc", new UntypedOwner
        {
            OwnerData = new FormLink<ISkyrimMajorRecordGetter>(ownerKey),
            VariableData = new FormLink<ISkyrimMajorRecordGetter>(new FormKey(CoverageTestHistory.Earlier, value))
        });
        var npc = state.PatchMod.Npcs[Actor];
        var (report, log) = Validate(state);
        Assert.Equal(Actor, Assert.Single(report.SkippedRecords).Record);
        Assert.IsType<UntypedOwner>(OwnerData(npc).Owner);
        Assert.Empty(state.PatchMod.Npcs);
        Assert.Contains("does not resolve", log);
    }

    [Theory]
    [InlineData("typed")]
    [InlineData("typed-positive")]
    [InlineData("typed-large")]
    [InlineData("typed-negative")]
    [InlineData("unowned")]
    public void TypedFactionRanksAndUnusedUnownedDataAreNotReferences(string kind)
    {
        var mod = CoverageTestHistory.Mods()[0];
        mod.Factions.Add(new Faction(Key(0xDB1), SkyrimRelease.SkyrimSE));
        mod.Weapons.Add(new Weapon(Target, SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mod);
        IOwnerTargetGetter owner = kind switch
        {
            "unowned" => new UntypedOwner { VariableData = new FormLink<ISkyrimMajorRecordGetter>(new FormKey(CoverageTestHistory.Earlier, 0xFFFFFF)) },
            _ => new FactionOwner { Faction = new FormLink<IFactionGetter>(Key(0xDB1)),
                RequiredRank = kind switch { "typed-positive" => 6, "typed-large" => 0xFFFFFF, "typed-negative" => -2, _ => -1 } }
        };
        AddOwnerRecord(state.PatchMod, "npc", owner);
        var (report, log) = Validate(state);
        Assert.False(report.IsPartial);
        Assert.DoesNotContain("[Warning]", log);
        Assert.Single(state.PatchMod.Npcs);
        Assert.Equal(owner, OwnerData(state.PatchMod.Npcs[Actor]).Owner);
    }

    [Theory]
    [InlineData(-8388608, 8388608)]
    [InlineData(-8388607, -8388607)]
    [InlineData(-10000, -10000)]
    [InlineData(-1000, -1000)]
    [InlineData(-129, -129)]
    [InlineData(-128, -128)]
    [InlineData(-2, -2)]
    [InlineData(-1, -1)]
    [InlineData(0, 0)]
    [InlineData(6, 6)]
    [InlineData(127, 127)]
    [InlineData(10000, 10000)]
    [InlineData(8388607, 8388607)]
    [InlineData(8388608, 8388608)]
    public void ExternalFactionRankSurvivesTypedImportAndOutputWorkaroundFixesUntypedCopy(int rank, int expected)
    {
        var mods = CoverageTestHistory.Mods();
        mods[0].Factions.Add(new Faction(Key(0xDB1), SkyrimRelease.SkyrimSE));
        mods[0].Weapons.Add(new Weapon(Target, SkyrimRelease.SkyrimSE));
        AddOwnerRecord(mods[1], "npc", new FactionOwner
        { Faction = new FormLink<IFactionGetter>(Key(0xDB1)), RequiredRank = rank });
        using var input = new MemoryStream();
        mods[1].WriteToBinary(input);
        var directory = Path.Combine(Path.GetTempPath(), "Dmp-Coed-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        try
        {
            var inputPath = Path.Combine(directory, mods[1].ModKey.FileName.String);
            File.WriteAllBytes(inputPath, input.ToArray());
            // COED needs the record-type cache supplied by the path-based reader.
            using var untyped = SkyrimMod.CreateFromBinaryOverlay(inputPath, SkyrimRelease.SkyrimSE);
            Assert.IsAssignableFrom<IUntypedOwnerGetter>(untyped.Npcs[Actor].Items![0].Data!.Owner);
            using var state = CoverageTestHistory.State(mods[0], untyped);
            state.PatchMod.Npcs.Add(untyped.Npcs[Actor].DeepCopy());

            // An explicitly type-aware read proves the original scalar was valid.
            using var typed = SkyrimMod.CreateFromBinaryOverlay(inputPath, SkyrimRelease.SkyrimSE,
                new BinaryReadParameters { LinkCache = state.LinkCache });
            Assert.Equal(rank, Assert.IsAssignableFrom<IFactionOwnerGetter>(typed.Npcs[Actor].Items![0].Data!.Owner).RequiredRank);
            var typedPatch = new SkyrimMod(ModKey.FromNameAndExtension("TypedRankOutput.esp"), SkyrimRelease.SkyrimSE);
            typedPatch.Npcs.Add(typed.Npcs[Actor].DeepCopy());
            var typedOutputPath = Path.Combine(directory, typedPatch.ModKey.FileName.String);
            typedPatch.WriteToBinary(typedOutputPath);
            using var typedReloaded = SkyrimMod.CreateFromBinaryOverlay(typedOutputPath, SkyrimRelease.SkyrimSE,
                new BinaryReadParameters { LinkCache = state.LinkCache });
            Assert.Equal(rank, Assert.IsAssignableFrom<IFactionOwnerGetter>(typedReloaded.Npcs[Actor].Items![0].Data!.Owner).RequiredRank);

            // Negative ranks change when untyped. Small positive ranks may survive only
            // because their mistaken reference still maps to master index zero.
            var outputPath = Path.Combine(directory, state.PatchMod.ModKey.FileName.String);
            state.PatchMod.WriteToBinary(outputPath);
            using (var reloaded = SkyrimMod.CreateFromBinaryOverlay(outputPath, SkyrimRelease.SkyrimSE,
                new BinaryReadParameters { LinkCache = state.LinkCache }))
            {
                var unnormalized = Assert.IsAssignableFrom<IFactionOwnerGetter>(reloaded.Npcs[Actor].Items![0].Data!.Owner).RequiredRank;
                if (rank < 0) Assert.NotEqual(rank, unnormalized);
                else Assert.Equal(rank, unnormalized);
            }
            var (report, log) = Validate(state);
            Assert.False(report.IsPartial);
            Assert.Single(state.PatchMod.Npcs);
            Assert.DoesNotContain("[Warning]", log);
            state.PatchMod.WriteToBinary(outputPath);
            using var fixedOutput = SkyrimMod.CreateFromBinaryOverlay(outputPath, SkyrimRelease.SkyrimSE,
                new BinaryReadParameters { LinkCache = state.LinkCache });
            Assert.Equal(expected, Assert.IsAssignableFrom<IFactionOwnerGetter>(fixedOutput.Npcs[Actor].Items![0].Data!.Owner).RequiredRank);
            Assert.IsAssignableFrom<IUntypedOwnerGetter>(untyped.Npcs[Actor].Items![0].Data!.Owner);
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    [Theory]
    [InlineData(6u)]
    [InlineData(0xFFFu)]
    [InlineData(0xFFFFFFu)]
    public void UntypedNpcOwnerKeepsItsActualGlobalReference(uint id)
    {
        var mod = CoverageTestHistory.Mods()[0];
        var ownerKey = Key(0xDB1);
        var globalKey = Key(id);
        mod.Npcs.Add(new Npc(ownerKey, SkyrimRelease.SkyrimSE));
        mod.Globals.Add(new GlobalFloat(globalKey, SkyrimRelease.SkyrimSE));
        mod.Weapons.Add(new Weapon(Target, SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mod);
        AddOwnerRecord(state.PatchMod, "npc", new UntypedOwner
        {
            OwnerData = new FormLink<ISkyrimMajorRecordGetter>(ownerKey),
            VariableData = new FormLink<ISkyrimMajorRecordGetter>(globalKey)
        });
        var (report, log) = Validate(state);
        Assert.False(report.IsPartial);
        Assert.DoesNotContain("[Warning]", log);
        Assert.DoesNotContain("Normalized", log);
        var owner = Assert.IsType<UntypedOwner>(OwnerData(state.PatchMod.Npcs[Actor]).Owner);
        Assert.Equal(ownerKey, owner.OwnerData.FormKey);
        Assert.Equal(globalKey, owner.VariableData.FormKey);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ScalarOwnerExceptionDoesNotHideTheSameMissingLinkInAnotherField(bool factionOwner)
    {
        var mod = CoverageTestHistory.Mods()[0];
        if (factionOwner) mod.Factions.Add(new Faction(Key(0xDB1), SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mod);
        var raw = new UntypedOwner
        {
            OwnerData = new FormLink<ISkyrimMajorRecordGetter>(factionOwner ? Key(0xDB1) : FormKey.Null),
            VariableData = new FormLink<ISkyrimMajorRecordGetter>(Key(0xFFFFFF))
        };
        AddOwnerRecord(state.PatchMod, "npc", raw);
        var npc = state.PatchMod.Npcs[Actor];
        npc.VirtualMachineAdapter = new VirtualMachineAdapter();
        var script = new ScriptEntry { Name = "Example" };
        script.Properties.Add(new ScriptObjectProperty
        { Name = "Broken", Object = new FormLink<ISkyrimMajorRecordGetter>(Key(0xFFFFFF)) });
        npc.VirtualMachineAdapter.Scripts.Add(script);
        // Supply the item so the only genuine unresolved link is the script property.
        state.PatchMod.Weapons.Add(new Weapon(Target, SkyrimRelease.SkyrimSE));
        var (report, log) = Validate(state);
        Assert.Equal(Actor, Assert.Single(report.SkippedRecords).Record);
        Assert.Contains("does not resolve", log);
    }

    [Fact]
    public void FactionRankWorkaroundPreservesDuplicateRowsAndIsIdempotent()
    {
        var mod = CoverageTestHistory.Mods()[0];
        mod.Factions.Add(new Faction(Key(0xDB1), SkyrimRelease.SkyrimSE));
        mod.Weapons.Add(new Weapon(Target, SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mod);
        var raw = new UntypedOwner
        {
            OwnerData = new FormLink<ISkyrimMajorRecordGetter>(Key(0xDB1)),
            VariableData = new FormLink<ISkyrimMajorRecordGetter>(new FormKey(CoverageTestHistory.Earlier, 0xFFFFFF))
        };
        AddOwnerRecord(state.PatchMod, "npc", raw);
        var npc = state.PatchMod.Npcs[Actor];
        npc.Items![0].Data!.ItemCondition = 0.5f;
        npc.Items.Add(npc.Items[0].DeepCopy());
        npc.Items[1].Item.Count = 2;
        npc.Items[1].Data!.ItemCondition = 0.75f;
        var (report, log) = Validate(state);
        Assert.False(report.IsPartial);
        Assert.Equal(new[] { 1, 2 }, npc.Items.Select(i => i.Item.Count));
        Assert.Equal(new[] { 0.5f, 0.75f }, npc.Items.Select(i => i.Data!.ItemCondition));
        Assert.All(npc.Items, row =>
            Assert.Equal(-1, Assert.IsType<FactionOwner>(row.Data!.Owner).RequiredRank));
        Assert.Contains($"[Info] Normalized 2 untyped COED faction rank(s) in {Actor}.", log);
        Assert.IsType<UntypedOwner>(raw);
        var (secondReport, secondLog) = Validate(state);
        Assert.False(secondReport.IsPartial);
        Assert.DoesNotContain("Normalized", secondLog);
    }

    [Fact]
    public void InputWinnerIndexRejectsAConflictingRecordTypeInsteadOfFindingAnOlderCompatibleType()
    {
        var mods = CoverageTestHistory.Mods();
        mods[0].Packages.Add(new Package(Target, SkyrimRelease.SkyrimSE));
        mods[1].Weapons.Add(new Weapon(Target, SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mods);
        var npc = new Npc(Actor, SkyrimRelease.SkyrimSE);
        npc.Packages.Add(new FormLink<IPackageGetter>(Target));
        state.PatchMod.Npcs.Add(npc);
        var (report, _) = Validate(state);
        Assert.Equal(Actor, Assert.Single(report.SkippedRecords).Record);
        Assert.Empty(state.PatchMod.Npcs);
    }

    [Fact]
    public void TransitiveCycleTraversalUsesTheActualWinnerEvenWhenItsRecordTypeChanged()
    {
        var mods = CoverageTestHistory.Mods();
        var indirect = Key(0x802);
        mods[0].LeveledItems.Add(new LeveledItem(Target, SkyrimRelease.SkyrimSE) { Entries = [Item(indirect)] });
        mods[0].LeveledItems.Add(new LeveledItem(indirect, SkyrimRelease.SkyrimSE) { Entries = [Item(Actor)] });
        mods[1].Weapons.Add(new Weapon(indirect, SkyrimRelease.SkyrimSE));
        using var state = CoverageTestHistory.State(mods);
        state.PatchMod.LeveledItems.Add(new LeveledItem(Actor, SkyrimRelease.SkyrimSE) { Entries = [Item(Target)] });
        var (report, log) = Validate(state);
        Assert.False(report.IsPartial);
        Assert.DoesNotContain("[Warning]", log);
        Assert.Single(state.PatchMod.LeveledItems);
    }

    private static void AddOwnerRecord(ISkyrimMod mod, string kind, IOwnerTargetGetter owner)
    {
        var extra = new ExtraData { Owner = owner.DeepCopy(), ItemCondition = 1 };
        var item = new ContainerEntry { Item = new ContainerItem { Item = new FormLink<IItemGetter>(Target), Count = 1 }, Data = extra };
        switch (kind)
        {
            case "npc": mod.Npcs.Add(new Npc(Actor, SkyrimRelease.SkyrimSE) { Items = [item] }); break;
            case "container": mod.Containers.Add(new Container(Actor, SkyrimRelease.SkyrimSE) { Items = [item] }); break;
            case "leveled-item": mod.LeveledItems.Add(new LeveledItem(Actor, SkyrimRelease.SkyrimSE)
                { Entries = [new LeveledItemEntry { ExtraData = extra }] }); break;
            default: mod.LeveledNpcs.Add(new LeveledNpc(Actor, SkyrimRelease.SkyrimSE)
                { Entries = [new LeveledNpcEntry { ExtraData = extra }] }); break;
        }
    }

    private static IExtraDataGetter OwnerData(Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter record) => record switch
    {
        INpcGetter npc => npc.Items![0].Data!,
        IContainerGetter container => container.Items![0].Data!,
        ILeveledItemGetter item => item.Entries![0].ExtraData!,
        ILeveledNpcGetter npc => npc.Entries![0].ExtraData!,
        _ => throw new ArgumentException("Record has no COED fixture", nameof(record))
    };

    private static LeveledItemEntry Item(FormKey key) => new()
    { Data = new LeveledItemEntryData { Reference = new FormLink<IItemGetter>(key), Count = 1, Level = 1 } };

    private static void AddPlacedRecord(ISkyrimMod mod, IPlaced placed)
    {
        var cell = new Cell(Key(0x900), SkyrimRelease.SkyrimSE) { Flags = Cell.Flag.IsInteriorCell };
        cell.Temporary.Add(placed);
        var sub = new CellSubBlock { BlockNumber = 0, GroupType = GroupTypeEnum.InteriorCellSubBlock };
        sub.Cells.Add(cell);
        var block = new CellBlock { BlockNumber = 0, GroupType = GroupTypeEnum.InteriorCellBlock };
        block.SubBlocks.Add(sub);
        mod.Cells.Add(block);
    }

    private static (PatchRunReport Report, string Log) Validate(Mutagen.Bethesda.Synthesis.IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        using var diagnostics = new PatchDiagnostics();
        var previous = Console.Out;
        using var log = new StringWriter();
        try { Console.SetOut(log); OutputPatchValidator.Validate(state); }
        finally { Console.SetOut(previous); }
        Assert.True(diagnostics.Report.Succeeded);
        return (diagnostics.Report, log.ToString());
    }
}
