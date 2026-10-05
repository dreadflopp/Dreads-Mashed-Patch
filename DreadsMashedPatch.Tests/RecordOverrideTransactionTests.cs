using DreadsMashedPatch.Enums;
using DreadsMashedPatch.PropertyHandlers.General;
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

[Collection("LogCollector")]
public sealed class RecordOverrideTransactionTests : IDisposable
{
    private static readonly ModKey Original = ModKey.FromNameAndExtension("Skyrim.esm");
    private static readonly ModKey Earlier = ModKey.FromNameAndExtension("Earlier.esp");
    private static readonly ModKey Winner = ModKey.FromNameAndExtension("Winner.esp");
    private static readonly ModKey Patch = ModKey.FromNameAndExtension("MashedPatch.esp");
    private static readonly FormKey FailedKey = new(Original, 0x800);
    private static readonly FormKey HealthyKey = new(Original, 0x801);
    private static readonly FormKey WorldKey = new(Original, 0x900);
    private static readonly FormKey CellKey = new(Original, 0x901);

    public RecordOverrideTransactionTests()
    {
        PatcherSettings.Apply(new PatcherConfiguration
        {
            Forwarding = new ForwardingSettings { EditorIdPolicy = EditorIdForwardingPolicy.StandardForwarding }
        });
        Utility.InitializeVanillaMods([Original], [], false);
    }

    public void Dispose()
    {
        PatcherSettings.Apply(new PatcherConfiguration());
        LogCollector.Clear();
    }

    [Theory]
    [InlineData("before")]
    [InlineData("after")]
    [InlineData("logged")]
    [InlineData("silent")]
    [InlineData("flushed")]
    public void FailedSetterDiscardsWholeRecordAndProcessingContinues(string failure)
    {
        var mods = History((mod, key, edid) => mod.Weapons.Add(new Weapon(key, SkyrimRelease.SkyrimSE) { EditorID = edid }));
        using var state = State(mods);
        foreach (var mod in mods)
            foreach (var weapon in mod.Weapons)
                weapon.Name = mod.ModKey == Earlier ? "Selected Name" : "Original Name";
        var handler = new OrderedWeaponHandler();
        handler.PropertyHandlers["EditorID"] = new FaultyEditorIdHandler(failure);
        var contexts = Winners(state, typeof(IWeaponGetter));

        var report = handler.Process(state, contexts.OrderBy(c => c.Record.FormKey.ID).ToArray());

        Assert.False(report.Succeeded);
        Assert.Contains(report.Errors, error => error.Record == FailedKey);
        Assert.Throws<PatchRunFailedException>(report.ThrowIfFailed);
        var healthy = Assert.Single(state.PatchMod.Weapons);
        Assert.Equal(HealthyKey, healthy.FormKey);
        Assert.Equal("Selected", healthy.EditorID);
        Assert.Equal("Selected Name", healthy.Name!.String);
        Assert.All(mods, mod => Assert.Equal(mod.ModKey == Earlier ? "Selected" : "Original", mod.Weapons[FailedKey].EditorID));
    }

    [Fact]
    public void ReflectionSetterReportedErrorCannotPublishCandidate()
    {
        var mods = History((mod, key, edid) => mod.Weapons.Add(new Weapon(key, SkyrimRelease.SkyrimSE) { EditorID = edid }));
        using var state = State(mods);
        var handler = new WeaponRecordHandler();
        // Getter remains valid; setter's incompatible record surface reports an error and returns.
        handler.PropertyHandlers["EditorID"] = new SimpleReflectionPropertyHandler<string, IArmor, IWeaponGetter>("EditorID");
        var report = handler.Process(state, Winners(state, typeof(IWeaponGetter)));
        Assert.False(report.Succeeded);
        Assert.Contains(report.Errors, error => error.Stage == "EditorID" && error.Message.Contains("IArmor"));
        Assert.Empty(state.PatchMod.Weapons);
    }

    [Fact]
    public void WarningOnlySetterCanPublishCompleteCandidate()
    {
        var mods = History((mod, key, edid) => mod.Weapons.Add(new Weapon(key, SkyrimRelease.SkyrimSE) { EditorID = edid }));
        using var state = State(mods);
        var handler = new WeaponRecordHandler();
        handler.PropertyHandlers["EditorID"] = new FaultyEditorIdHandler("warning");
        Assert.True(handler.Process(state, Winners(state, typeof(IWeaponGetter))).Succeeded);
        Assert.Equal(2, state.PatchMod.Weapons.Count);
        Assert.All(state.PatchMod.Weapons, record => Assert.Equal("Selected", record.EditorID));
    }

    [Theory]
    [InlineData("interior", false)]
    [InlineData("interior", true)]
    [InlineData("top", false)]
    [InlineData("top", true)]
    [InlineData("exterior", false)]
    [InlineData("exterior", true)]
    [InlineData("dialog", false)]
    [InlineData("dialog", true)]
    public void NestedSetterFailurePreservesParentsAndExistingOutput(string nesting, bool existingOutput)
    {
        var source = NestedMod(nesting);
        using var state = State(source);
        var context = Winners(state, nesting == "dialog" ? typeof(IDialogResponsesGetter) : typeof(IPlacedObjectGetter))
            .Single(c => c.Record.FormKey == FailedKey);
        DreadsMashedPatch.RecordHandlers.Abstracts.AbstractRecordHandler handler = nesting == "dialog"
            ? new DialogResponseRecordHandler() : new PlacedObjectRecordHandler();
        if (existingOutput)
        {
            handler.CommitOverride(context, state, new() { ["EditorID"] = "Existing" });
        }
        var before = SkyrimModMixIn.DeepCopy(state.PatchMod);
        handler.PropertyHandlers["EditorID"] = new FaultyEditorIdHandler("after");

        Assert.Throws<InvalidOperationException>(() => handler.CommitOverride(context, state, new() { ["EditorID"] = "Selected" }));

        Assert.True(before.Equals(state.PatchMod));
        Assert.Equal("Source", context.Record.EditorID);
        if (!existingOutput) Assert.Empty(state.PatchMod.EnumerateMajorRecords());
    }

    [Theory]
    [InlineData("interior")]
    [InlineData("top")]
    [InlineData("exterior")]
    [InlineData("dialog")]
    public void SuccessfulNestedCommitPreservesPriorChildrenAndHeaders(string nesting)
    {
        using var state = State(NestedMod(nesting));
        var contexts = Winners(state, nesting == "dialog" ? typeof(IDialogResponsesGetter) : typeof(IPlacedObjectGetter));
        DreadsMashedPatch.RecordHandlers.Abstracts.AbstractRecordHandler handler = nesting == "dialog"
            ? new DialogResponseRecordHandler() : new PlacedObjectRecordHandler();
        handler.CommitOverride(contexts.Single(c => c.Record.FormKey == HealthyKey), state, new() { ["EditorID"] = "First" });
        var parent = state.PatchMod.EnumerateMajorRecords().Single(r => r.FormKey == CellKey);
        parent.EditorID = "PatchedParent";

        handler.CommitOverride(contexts.Single(c => c.Record.FormKey == FailedKey), state, new() { ["EditorID"] = "Second" });

        Assert.Equal("First", state.PatchMod.EnumerateMajorRecords().Single(r => r.FormKey == HealthyKey).EditorID);
        Assert.Equal("Second", state.PatchMod.EnumerateMajorRecords().Single(r => r.FormKey == FailedKey).EditorID);
        Assert.Equal("PatchedParent", state.PatchMod.EnumerateMajorRecords().Single(r => r.FormKey == CellKey).EditorID);
        using var stream = new MemoryStream();
        state.PatchMod.WriteToBinary(stream);
        Assert.True(stream.Length > 0);
    }

    [Fact]
    public void ContextInsertionFailureCannotLeakCreatedParents()
    {
        using var state = State(new SkyrimMod(Winner, SkyrimRelease.SkyrimSE));
        var weapon = new Weapon(FailedKey, SkyrimRelease.SkyrimSE);
        var context = new ModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(Winner, weapon,
            (mod, _) =>
            {
                mod.Worldspaces.Add(new Worldspace(WorldKey, SkyrimRelease.SkyrimSE));
                mod.Weapons.Add(weapon.DeepCopy());
                throw new InvalidDataException("Insertion failed after parent creation");
            }, (_, _, _, _) => throw new NotSupportedException());

        Assert.ThrowsAny<Exception>(() => new WeaponRecordHandler().CommitOverride(context, state));
        Assert.Empty(state.PatchMod.EnumerateMajorRecords());
    }

    [Fact]
    public void NpcUsesTheSharedCommitPath()
    {
        using var state = State(new SkyrimMod(Winner, SkyrimRelease.SkyrimSE));
        var npc = new Npc(FailedKey, SkyrimRelease.SkyrimSE) { EditorID = "Source" };
        var context = new ModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(Winner, npc,
            (mod, record) => mod.Npcs.GetOrAddAsOverride((INpcGetter)record), (_, _, _, _) => throw new NotSupportedException());
        var handler = new NpcRecordHandler();
        handler.PropertyHandlers["EditorID"] = new FaultyEditorIdHandler("after");
        Assert.Throws<InvalidOperationException>(() => handler.CommitOverride(context, state, new() { ["EditorID"] = "Selected" }));
        Assert.Empty(state.PatchMod.Npcs);
        handler.PropertyHandlers["EditorID"] = new EditorIDHandler();
        handler.CommitOverride(context, state, new() { ["EditorID"] = "Selected" });
        Assert.Equal("Selected", Assert.Single(state.PatchMod.Npcs).EditorID);
    }

    [Theory]
    [InlineData("weapon")]
    [InlineData("npc")]
    [InlineData("world")]
    [InlineData("interior")]
    [InlineData("top")]
    [InlineData("exterior")]
    [InlineData("dialog")]
    public void OverlayCommitsPreserveExistingOutputAndRollBackFailedSetters(string nesting)
    {
        var source = nesting is "weapon" or "npc" or "world"
            ? new SkyrimMod(Winner, SkyrimRelease.SkyrimSE) : NestedMod(nesting);
        if (nesting == "weapon") source.Weapons.Add(new Weapon(FailedKey, SkyrimRelease.SkyrimSE) { EditorID = "Source" });
        if (nesting == "npc") source.Npcs.Add(new Npc(FailedKey, SkyrimRelease.SkyrimSE) { EditorID = "Source" });
        if (nesting == "world") source.Worldspaces.Add(new Worldspace(FailedKey, SkyrimRelease.SkyrimSE) { EditorID = "Source" });
        using var stream = new MemoryStream();
        source.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, source.ModKey);
        using var state = State(overlay);
        var context = Winners(state, typeof(IMajorRecordGetter)).Single(c => c.Record.FormKey == FailedKey);
        Assert.EndsWith("BinaryOverlay", context.Record.GetType().Name);
        DreadsMashedPatch.RecordHandlers.Abstracts.AbstractRecordHandler handler = nesting switch
        {
            "weapon" => new WeaponRecordHandler(),
            "npc" => new NpcRecordHandler(),
            "world" => new WorldspaceRecordHandler(),
            "dialog" => new DialogResponseRecordHandler(),
            _ => new PlacedObjectRecordHandler()
        };

        handler.CommitOverride(context, state, new() { ["EditorID"] = "First" });
        Assert.Equal("First", state.PatchMod.EnumerateMajorRecords().Single(r => r.FormKey == FailedKey).EditorID);
        if (nesting is "interior" or "top" or "exterior" or "dialog")
        {
            var sibling = Winners(state, typeof(IMajorRecordGetter)).Single(c => c.Record.FormKey == HealthyKey);
            handler.CommitOverride(sibling, state, new() { ["EditorID"] = "Sibling" });
            state.PatchMod.EnumerateMajorRecords().Single(r => r.FormKey == CellKey).EditorID = "PatchedParent";
        }
        handler.CommitOverride(context, state, new() { ["EditorID"] = "Second" });
        var beforeFailure = SkyrimModMixIn.DeepCopy(state.PatchMod);
        handler.PropertyHandlers["EditorID"] = new FaultyEditorIdHandler("after");
        Assert.Throws<InvalidOperationException>(() => handler.CommitOverride(context, state, new() { ["EditorID"] = "Failed" }));
        Assert.True(beforeFailure.Equals(state.PatchMod));
        Assert.Equal("Second", state.PatchMod.EnumerateMajorRecords().Single(r => r.FormKey == FailedKey).EditorID);
        Assert.Equal("Source", context.Record.EditorID);
        if (nesting is "interior" or "top" or "exterior" or "dialog")
        {
            Assert.Equal("Sibling", state.PatchMod.EnumerateMajorRecords().Single(r => r.FormKey == HealthyKey).EditorID);
            Assert.Equal("PatchedParent", state.PatchMod.EnumerateMajorRecords().Single(r => r.FormKey == CellKey).EditorID);
        }
        using var outputStream = new MemoryStream();
        state.PatchMod.WriteToBinary(outputStream);
        outputStream.Position = 0;
        using var reloaded = SkyrimMod.CreateFromBinaryOverlay(outputStream, SkyrimRelease.SkyrimSE, Patch);
        Assert.Equal("Second", reloaded.EnumerateMajorRecords().Single(r => r.FormKey == FailedKey).EditorID);
    }

    [Fact]
    public void FailedRunReportWithholdsPrimaryAndSplitFileCommit()
    {
        var mods = History((mod, key, edid) => mod.Weapons.Add(new Weapon(key, SkyrimRelease.SkyrimSE) { EditorID = edid }));
        using var state = State(mods);
        var handler = new WeaponRecordHandler();
        handler.PropertyHandlers["EditorID"] = new FaultyEditorIdHandler("logged");
        var report = handler.Process(state, Winners(state, typeof(IWeaponGetter)));
        var directory = Path.Combine(Path.GetTempPath(), $"FailedPatch-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            var output = Path.Combine(directory, "MashedPatch.esp");
            var split = Path.Combine(directory, "MashedPatch_2.esp");
            File.WriteAllText(output, "old primary");
            File.WriteAllText(split, "old split");
            using (var transaction = new PatchOutputTransaction(output))
            {
                File.WriteAllText(transaction.StagedOutputPath, "new primary");
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(transaction.StagedOutputPath)!, "MashedPatch_2.esp"), "new split");
                Assert.Throws<PatchRunFailedException>(() => transaction.Commit(report));
            }
            Assert.Equal("old primary", File.ReadAllText(output));
            Assert.Equal("old split", File.ReadAllText(split));
            Assert.Equal(2, Directory.GetFileSystemEntries(directory).Length);
        }
        finally { Directory.Delete(directory, true); }
    }

    [Fact]
    public void SuccessfulRunReportDoesNotCarryErrorsFromPreviousProcessing()
    {
        var mods = History((mod, key, edid) => mod.Weapons.Add(new Weapon(key, SkyrimRelease.SkyrimSE) { EditorID = edid }));
        using var failedState = State(mods);
        var handler = new WeaponRecordHandler();
        handler.PropertyHandlers["EditorID"] = new FaultyEditorIdHandler("after");
        Assert.False(handler.Process(failedState, Winners(failedState, typeof(IWeaponGetter))).Succeeded);
        using var cleanState = State(mods);
        Assert.True(Program.RunPatchWithReport(cleanState).Succeeded);
    }

    [Fact]
    public void RecordSpecificExceptionAfterSuccessfulSettersDiscardsCandidate()
    {
        var mods = History((mod, key, edid) => mod.Weapons.Add(new Weapon(key, SkyrimRelease.SkyrimSE) { EditorID = edid }));
        using var state = State(mods);
        var report = new FailingWeaponHandler().Process(state, Winners(state, typeof(IWeaponGetter)));
        Assert.False(report.Succeeded);
        Assert.Equal(HealthyKey, Assert.Single(state.PatchMod.Weapons).FormKey);
        Assert.Contains(report.Errors, error => error.Record == FailedKey && error.Exception?.Message == "Record validation failed");
    }

    private sealed class FailingWeaponHandler : WeaponRecordHandler
    {
        public override void ApplyForwardedProperties(IMajorRecord record, Dictionary<string, object?> values)
        {
            base.ApplyForwardedProperties(record, values);
            if (record.FormKey == FailedKey) throw new InvalidDataException("Record validation failed");
        }
    }

    [Fact]
    public void FullRunReportsCaughtRecordErrorsAndSynthesisEntryPointRejectsOutput()
    {
        var mods = History((mod, key, edid) => mod.Weapons.Add(new Weapon(key, SkyrimRelease.SkyrimSE) { EditorID = edid }));
        mods[1].Weapons[FailedKey].VirtualMachineAdapter = new VirtualMachineAdapter();
        mods[1].Weapons[FailedKey].VirtualMachineAdapter!.Scripts.Add(null!);
        using var state = State(mods);

        var report = Program.RunPatchWithReport(state);

        Assert.False(report.Succeeded);
        Assert.Contains(report.Errors, error => error.Record == FailedKey && error.Exception != null);
        Assert.DoesNotContain(state.PatchMod.Weapons, record => record.FormKey == FailedKey);
        Assert.Contains(state.PatchMod.Weapons, record => record.FormKey == HealthyKey);
        using var secondState = State(mods);
        var failure = Assert.Throws<PatchRunFailedException>(() => Program.RunPatch(secondState));
        Assert.False(failure.Report.Succeeded);
    }

    // Name is applied successfully before the failing EditorID setter.
    private sealed class OrderedWeaponHandler : WeaponRecordHandler
    {
        public override Dictionary<string, DreadsMashedPatch.PropertyHandlers.Interfaces.IPropertyHandler> PropertyHandlers { get; } =
            new WeaponRecordHandler().PropertyHandlers.OrderBy(pair => pair.Key == "EditorID")
                .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    private sealed class FaultyEditorIdHandler(string failure) : EditorIDHandler
    {
        public override void SetValue(IMajorRecord record, string? value)
        {
            if (record.FormKey != FailedKey) { base.SetValue(record, value); return; }
            if (failure == "before") throw new InvalidOperationException("Setter failed before mutation");
            if (failure == "silent") return;
            record.EditorID = value;
            if (failure == "warning")
            {
                LogCollector.AddWarning(PropertyName, "Complete value recovered using a supported fallback");
                return;
            }
            if (failure is "logged" or "flushed")
            {
                LogCollector.AddError(PropertyName, "Setter caught a copy failure", new InvalidDataException("Bad payload"));
                if (failure == "flushed") LogCollector.PrintAllAndClear();
                return;
            }
            throw new InvalidOperationException("Setter failed after mutation");
        }
    }

    private static SkyrimMod[] History(Action<SkyrimMod, FormKey, string> add)
    {
        SkyrimMod[] mods = [new(Original, SkyrimRelease.SkyrimSE), new(Earlier, SkyrimRelease.SkyrimSE), new(Winner, SkyrimRelease.SkyrimSE)];
        foreach (var mod in mods)
        {
            if (mod.ModKey != Original) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = Original });
            add(mod, FailedKey, mod.ModKey == Earlier ? "Selected" : "Original");
            add(mod, HealthyKey, mod.ModKey == Earlier ? "Selected" : "Original");
        }
        return mods;
    }

    private static SkyrimMod NestedMod(string nesting)
    {
        var mod = new SkyrimMod(Winner, SkyrimRelease.SkyrimSE);
        if (nesting == "dialog")
        {
            var topic = new DialogTopic(CellKey, SkyrimRelease.SkyrimSE) { EditorID = "Parent" };
            topic.Responses.Add(new DialogResponses(FailedKey, SkyrimRelease.SkyrimSE) { EditorID = "Source" });
            topic.Responses.Add(new DialogResponses(HealthyKey, SkyrimRelease.SkyrimSE) { EditorID = "Source" });
            mod.DialogTopics.Add(topic);
            return mod;
        }
        var cell = new Cell(CellKey, SkyrimRelease.SkyrimSE) { EditorID = "Parent" };
        cell.Persistent.Add(new PlacedObject(FailedKey, SkyrimRelease.SkyrimSE) { EditorID = "Source" });
        cell.Persistent.Add(new PlacedObject(HealthyKey, SkyrimRelease.SkyrimSE) { EditorID = "Source" });
        if (nesting == "interior")
        {
            var block = new CellBlock { BlockNumber = 1, GroupType = GroupTypeEnum.InteriorCellBlock };
            var sub = new CellSubBlock { BlockNumber = 2, GroupType = GroupTypeEnum.InteriorCellSubBlock };
            sub.Cells.Add(cell); block.SubBlocks.Add(sub); mod.Cells.Add(block);
        }
        else
        {
            var world = new Worldspace(WorldKey, SkyrimRelease.SkyrimSE);
            if (nesting == "top") world.TopCell = cell;
            else
            {
                var block = new WorldspaceBlock { BlockNumberX = 1, BlockNumberY = 2, GroupType = GroupTypeEnum.ExteriorCellBlock };
                var sub = new WorldspaceSubBlock { BlockNumberX = 3, BlockNumberY = 4, GroupType = GroupTypeEnum.ExteriorCellSubBlock };
                sub.Items.Add(cell); block.Items.Add(sub); world.SubCells.Add(block);
            }
            mod.Worldspaces.Add(world);
        }
        return mod;
    }

    private static IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] Winners(
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state, Type type) => state.LoadOrder.PriorityOrder
        .WinningContextOverrides<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(state.LinkCache)
        .Where(context => type.IsInstanceOfType(context.Record)).ToArray();

#pragma warning disable CS0618
    private static SynthesisState<ISkyrimMod, ISkyrimModGetter> State(params ISkyrimModGetter[] mods)
    {
        var patch = new SkyrimMod(Patch, SkyrimRelease.SkyrimSE);
        var listings = mods.Cast<ISkyrimModGetter>().Append(patch).Select(mod => new ModListing<ISkyrimModGetter>(mod)).ToArray();
        var order = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
        var args = new RunSynthesisMutagenPatcher { OutputPath = Path.Combine(Path.GetTempPath(), "MashedPatch.esp"),
            DataFolderPath = Path.GetTempPath(), LoadOrderFilePath = Path.Combine(Path.GetTempPath(), "plugins.txt"), GameRelease = GameRelease.SkyrimSE };
        return new SynthesisState<ISkyrimMod, ISkyrimModGetter>(args,
            listings.Select(l => new LoadOrderListing(l.ModKey, l.Enabled)).ToArray(), order,
            order.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(), null!, patch, null, null, null, CancellationToken.None, null);
    }
#pragma warning restore CS0618
}
