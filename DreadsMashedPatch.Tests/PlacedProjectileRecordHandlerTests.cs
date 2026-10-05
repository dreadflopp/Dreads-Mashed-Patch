using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("CoverageFixes")]
public sealed class PlacedProjectileRecordHandlerTests : IDisposable
{
    private static readonly FormKey CellKey = new(CoverageTestHistory.Original, 0x900);
    private static readonly string[] InheritedFields = ["VirtualMachineAdapter", "EncounterZone", "Owner", "FactionRank",
        "HeadTrackingWeight", "FavorCost", "Reflections", "LinkedReferences", "ActivateParents", "EnableParent", "Emittance",
        "MultiBoundReference", "IgnoredBySandbox", "LocationRefTypes", "LocationReference", "DistantLodData", "Scale", "Placement"];

    public void Dispose() => CoverageTestHistory.Reset();
    private static readonly string[] Kinds = ["Arrow", "Barrier", "Beam", "Cone", "Flame", "Trap", "Missile"];
    private static DreadsMashedPatch.RecordHandlers.Abstracts.AbstractRecordHandler Handler(string kind) =>
        (DreadsMashedPatch.RecordHandlers.Abstracts.AbstractRecordHandler)System.Activator.CreateInstance(
            typeof(Program).Assembly.GetType($"DreadsMashedPatch.RecordHandlers.Placed{kind}RecordHandler")!)!;
    public static IEnumerable<object[]> Fields() => Kinds.SelectMany(kind => InheritedFields.Append("Projectile")
        .Select(field => new object[] { kind, field }));
    public static IEnumerable<object[]> Histories() => Kinds.SelectMany(kind => InheritedFields.Append("Projectile").SelectMany(field =>
        new[] { false, true }.SelectMany(authorized => new[] { false, true }
            // DATA placement is required in xEdit; exercise its value reversion,
            // rather than constructing a source without placement for a removal test.
            .Where(removal => !removal || field != "Placement")
            .Select(removal => new object[] { kind, field, authorized, removal }))));

    [Theory]
    [InlineData("Arrow")]
    [InlineData("Barrier")]
    [InlineData("Beam")]
    [InlineData("Cone")]
    [InlineData("Flame")]
    [InlineData("Trap")]
    [InlineData("Missile")]
    public void RegistersAllInheritedFieldsWithReviewedOwnershipAndHeaderPolicy(string kind)
    {
        var handlers = Handler(kind).PropertyHandlers;
        Assert.Equal(InheritedFields.Append("EditorID").Append("MajorRecordFlagsRaw").Append("Projectile").Order(), handlers.Keys.Order());
        Assert.IsType<MajorRecordFlagsRawHandler>(handlers["MajorRecordFlagsRaw"]);
        Assert.Equal(ListSemantics.SortedKeyed,
            Assert.IsAssignableFrom<AbstractListPropertyHandler<ILinkedReferencesGetter>>(handlers["LinkedReferences"]).Semantics);
        Assert.Equal(ListSemantics.SortedKeyed,
            Assert.IsAssignableFrom<AbstractListPropertyHandler<IWaterReflectionGetter>>(handlers["Reflections"]).Semantics);
        Assert.Equal(ListSemantics.AlignedOrdered,
            Assert.IsAssignableFrom<AbstractListPropertyHandler<IFormLinkGetter<ILocationReferenceTypeGetter>>>(handlers["LocationRefTypes"]).Semantics);
        Assert.StartsWith("SimplePropertyContext", handlers["DistantLodData"].CreatePropertyContext().GetType().Name);
    }

    [Theory]
    [MemberData(nameof(Fields))]
    public void OverlayBackedValuesCopyAndRoundTripThroughEveryInheritedHandler(string kind, string field)
    {
        var source = new SkyrimMod(CoverageTestHistory.Original, SkyrimRelease.SkyrimSE);
        Add(source, Populated(kind));
        using var sourceStream = Write(source);
        using var sourceOverlay = SkyrimMod.CreateFromBinaryOverlay(sourceStream, SkyrimRelease.SkyrimSE, source.ModKey);
        var getter = Assert.Single(sourceOverlay.EnumerateMajorRecords().OfType<IAPlacedTrapGetter>());
        var handler = Handler(kind).PropertyHandlers[field];
        var value = handler.GetValue(getter);
        Assert.NotNull(value);
        var target = Empty(kind);
        handler.SetValue(target, value);
        Assert.True(handler.AreValuesEqual(value, handler.GetValue(target)), field);

        var patch = new SkyrimMod(ModKey.FromNameAndExtension("ProjectileCopy.esp"), SkyrimRelease.SkyrimSE);
        Add(patch, target);
        using var outputStream = Write(patch);
        using var outputOverlay = SkyrimMod.CreateFromBinaryOverlay(outputStream, SkyrimRelease.SkyrimSE, patch.ModKey);
        Assert.True(handler.AreValuesEqual(value, handler.GetValue(
            Assert.Single(outputOverlay.EnumerateMajorRecords().OfType<IAPlacedTrapGetter>()))), field);

        handler.SetValue(target, null);
        var result = handler.GetValue(target);
        if (field is "Reflections" or "LinkedReferences") Assert.Empty(Assert.IsAssignableFrom<System.Collections.IEnumerable>(result));
        else Assert.Null(result);
        Assert.True(handler.AreValuesEqual(value, handler.GetValue(getter)), "Source overlay was altered");
    }

    [Theory]
    [MemberData(nameof(Histories))]
    public void FullRunInheritedFieldForwardingAndReversionRespectPermissions(string kind, string field, bool authorized, bool removal)
    {
        CoverageTestHistory.Configure(authorized);
        var mods = CoverageTestHistory.Mods();
        var records = mods.Select(_ => removal ? Populated(kind) : Empty(kind)).ToArray();
        var handler = Handler(kind).PropertyHandlers[field];
        handler.SetValue(records[1], removal ? null : handler.GetValue(Populated(kind)));
        var selected = handler.GetValue(records[1]);
        for (var i = 0; i < mods.Length; i++) Add(mods[i], records[i]);
        using var state = CoverageTestHistory.State(mods);

        Program.RunPatch(state);

        if (authorized) Assert.Empty(state.PatchMod.EnumerateMajorRecords());
        else
        {
            var output = Assert.Single(state.PatchMod.EnumerateMajorRecords().OfType<IAPlacedTrapGetter>());
            Assert.True(handler.AreValuesEqual(selected, handler.GetValue(output)), field);
            using var stream = Write(state.PatchMod);
            using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
            Assert.True(handler.AreValuesEqual(selected, handler.GetValue(
                Assert.Single(overlay.EnumerateMajorRecords().OfType<IAPlacedTrapGetter>()))), field);
        }
        Assert.True(handler.AreValuesEqual(selected, handler.GetValue(records[1])), "Earlier source was altered");
        Assert.True(handler.AreValuesEqual(handler.GetValue(records[0]), handler.GetValue(records[2])), "Winner was altered");
    }

    [Theory]
    [InlineData("Arrow", false)]
    [InlineData("Arrow", true)]
    [InlineData("Barrier", false)]
    [InlineData("Barrier", true)]
    [InlineData("Beam", false)]
    [InlineData("Beam", true)]
    [InlineData("Cone", false)]
    [InlineData("Cone", true)]
    [InlineData("Flame", false)]
    [InlineData("Flame", true)]
    [InlineData("Trap", false)]
    [InlineData("Trap", true)]
    [InlineData("Missile", false)]
    [InlineData("Missile", true)]
    public void Xis2AbsentAndPresentEmptyRemainDistinctInBinaryOutput(string kind, bool present)
    {
        var record = Empty(kind);
        record.IgnoredBySandbox = present ? new byte[0] : null;
        var mod = new SkyrimMod(CoverageTestHistory.Original, SkyrimRelease.SkyrimSE);
        Add(mod, record);
        using var stream = Write(mod);
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey);
        var value = Assert.Single(overlay.EnumerateMajorRecords().OfType<IAPlacedTrapGetter>()).IgnoredBySandbox;
        Assert.Equal(present, value.HasValue);
        if (present) Assert.Empty(value!.Value.ToArray());
    }

    [Theory]
    [InlineData("Arrow")]
    [InlineData("Barrier")]
    [InlineData("Beam")]
    [InlineData("Cone")]
    [InlineData("Flame")]
    [InlineData("Trap")]
    [InlineData("Missile")]
    public void MutableAggregatesListsLinksAndBytesAreDetachedBeforeOutputMutation(string kind)
    {
        var source = Populated(kind);
        var before = source.DeepCopy();
        var output = Empty(kind);
        foreach (var handler in Handler(kind).PropertyHandlers.Values)
            handler.SetValue(output, handler.GetValue(source));

        output.Placement!.Position = new P3Float(99, 99, 99);
        output.LinkedReferences[0].Reference.SetTo(new FormKey(CoverageTestHistory.Original, 0x990));
        output.Reflections[0].Type = WaterReflection.Flag.Refraction;
        output.ActivateParents!.Parents[0].Delay = 99;
        output.EnableParent!.Reference.SetTo(new FormKey(CoverageTestHistory.Original, 0x991));
        output.LocationRefTypes!.Clear();
        output.DistantLodData![0] = 99;
        var bytes = output.IgnoredBySandbox!.Value;
        bytes[0] = 99;
        output.VirtualMachineAdapter!.Scripts[0].Name = "Changed";
        Assert.True(source.Equals(before));
    }

    [Theory]
    [InlineData("Arrow")]
    [InlineData("Barrier")]
    [InlineData("Beam")]
    [InlineData("Cone")]
    [InlineData("Flame")]
    [InlineData("Trap")]
    [InlineData("Missile")]
    public void KeyedLinkedReferenceEditsReplaceTheRowRatherThanAddADuplicate(string kind)
    {
        CoverageTestHistory.Configure();
        var mods = CoverageTestHistory.Mods();
        var records = mods.Select(_ => Populated(kind)).ToArray();
        records[1].LinkedReferences[0].Reference.SetTo(new FormKey(CoverageTestHistory.Original, 0x910));
        for (var i = 0; i < mods.Length; i++) Add(mods[i], records[i]);
        using var state = CoverageTestHistory.State(mods);
        Program.RunPatch(state);
        var output = Assert.Single(state.PatchMod.EnumerateMajorRecords().OfType<IAPlacedTrapGetter>());
        Assert.Equal(new FormKey(CoverageTestHistory.Original, 0x910), Assert.Single(output.LinkedReferences).Reference.FormKey);
    }

    [Theory]
    [InlineData("Arrow")]
    [InlineData("Barrier")]
    [InlineData("Beam")]
    [InlineData("Cone")]
    [InlineData("Flame")]
    [InlineData("Trap")]
    [InlineData("Missile")]
    public void MixedBinaryOverlayVariantsUseTypedNarrowingAndHonorDisabledFamilies(string disabledKind)
    {
        CoverageTestHistory.Configure();
        var disabledGetter = typeof(IPlacedHazardGetter).Assembly.GetType($"Mutagen.Bethesda.Skyrim.IPlaced{disabledKind}Getter")!;
        var configuration = new PatcherConfiguration { CompatibilityRules = [] };
        configuration.DisabledRecordTypes.Add(disabledGetter.FullName!);
        PatcherSettings.Apply(configuration);
        var mods = CoverageTestHistory.Mods();
        for (var i = 0; i < mods.Length; i++)
        {
            var cell = new Cell(CellKey, SkyrimRelease.SkyrimSE) { Flags = Cell.Flag.IsInteriorCell };
            for (var index = 0; index < Kinds.Length; index++)
            {
                var template = Empty(Kinds[index]);
                var record = template.Duplicate(new FormKey(CoverageTestHistory.Original, (uint)(0x810 + index)));
                record.Scale = i == 1 ? 2 : 1;
                cell.Temporary.Add(record);
            }
            cell.Temporary.Add(new PlacedHazard(new FormKey(CoverageTestHistory.Original, 0x820), SkyrimRelease.SkyrimSE)
                { Placement = new Placement(), Scale = i == 1 ? 2 : 1 });
            var block = new CellBlock { BlockNumber = 0, GroupType = GroupTypeEnum.InteriorCellBlock };
            var sub = new CellSubBlock { BlockNumber = 0, GroupType = GroupTypeEnum.InteriorCellSubBlock };
            sub.Cells.Add(cell); block.SubBlocks.Add(sub); mods[i].Cells.Add(block);
        }
        var streams = mods.Select(Write).ToArray();
        var overlays = streams.Select((stream, i) => SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mods[i].ModKey)).ToArray();
        try
        {
            using var state = CoverageTestHistory.State(overlays);
            Program.RunPatch(state);
            var output = state.PatchMod.EnumerateMajorRecords().OfType<IAPlacedTrapGetter>().ToArray();
            Assert.Equal(7, output.Length); // Six enabled projectile variants plus PHZD.
            Assert.DoesNotContain(output, disabledGetter.IsInstanceOfType);
            Assert.All(output, record => Assert.Equal(2f, record.Scale));
            Assert.Equal(7, output.Select(record => record.FormKey).Distinct().Count());
            using var binary = Write(state.PatchMod);
            using var reload = SkyrimMod.CreateFromBinaryOverlay(binary, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
            var written = reload.EnumerateMajorRecords().OfType<IAPlacedTrapGetter>().ToArray();
            Assert.Equal(output.Select(record => record.FormKey).Order(), written.Select(record => record.FormKey).Order());
            Assert.All(written, record => Assert.Equal(2f, record.Scale));
        }
        finally
        {
            foreach (var overlay in overlays) overlay.Dispose();
            foreach (var stream in streams) stream.Dispose();
        }
    }

    private static APlacedTrap Empty(string kind)
    {
        var projectile = new FormLink<IProjectileGetter>(new FormKey(CoverageTestHistory.Original, 0x880));
        APlacedTrap record = kind switch
        {
            "Arrow" => new PlacedArrow(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE) { Projectile = projectile },
            "Barrier" => new PlacedBarrier(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE) { Projectile = projectile },
            "Beam" => new PlacedBeam(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE) { Projectile = projectile },
            "Cone" => new PlacedCone(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE) { Projectile = projectile },
            "Flame" => new PlacedFlame(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE) { Projectile = projectile },
            "Trap" => new PlacedTrap(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE) { Projectile = projectile },
            "Missile" => new PlacedMissile(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE) { Projectile = projectile },
            _ => throw new ArgumentOutOfRangeException(nameof(kind))
        };
        record.Placement = new Placement();
        return record;
    }

    private static APlacedTrap Populated(string kind)
    {
        var record = Empty(kind);
        record.GetType().GetProperty("Projectile")!.SetValue(record, new FormLink<IProjectileGetter>(CoverageTestHistory.LinkKey));
        record.EncounterZone.SetTo(CoverageTestHistory.LinkKey);
        record.Owner.SetTo(CoverageTestHistory.LinkKey);
        record.Emittance.SetTo(CoverageTestHistory.LinkKey);
        record.MultiBoundReference.SetTo(CoverageTestHistory.LinkKey);
        record.LocationReference.SetTo(CoverageTestHistory.LinkKey);
        record.FactionRank = 2; record.HeadTrackingWeight = 0.25f; record.FavorCost = 3; record.Scale = 1.5f;
        record.Placement = new Placement { Position = new P3Float(10, 20, 30), Rotation = new P3Float(0.25f, 0.5f, 0.75f) };
        record.LinkedReferences.Add(new LinkedReferences
        {
            KeywordOrReference = new FormLink<IKeywordLinkedReferenceGetter>(CoverageTestHistory.LinkKey),
            Reference = new FormLink<IPlacedGetter>(new FormKey(CoverageTestHistory.Original, 0x881))
        });
        record.Reflections.Add(new WaterReflection
        {
            Water = new FormLink<IPlacedObjectGetter>(CoverageTestHistory.LinkKey), Type = WaterReflection.Flag.Reflection
        });
        record.ActivateParents = new ActivateParents();
        record.ActivateParents.Parents.Add(new ActivateParent
        {
            Reference = new FormLink<IPlacedGetter>(CoverageTestHistory.LinkKey), Delay = 0.75f
        });
        record.EnableParent = new EnableParent { Reference = new FormLink<IPlacedGetter>(CoverageTestHistory.LinkKey) };
        record.IgnoredBySandbox = new byte[] { 8, 9 };
        record.LocationRefTypes = [new FormLink<ILocationReferenceTypeGetter>(new FormKey(CoverageTestHistory.Original, 0x803)),
            new FormLink<ILocationReferenceTypeGetter>(CoverageTestHistory.LinkKey)];
        record.DistantLodData = [1.25f, 2.5f, 3.75f];
        record.VirtualMachineAdapter = new VirtualMachineAdapter { Version = 4, ObjectFormat = 1 };
        var script = new ScriptEntry { Name = "Test" };
        script.Properties.Add(new ScriptIntProperty { Name = "Value", Data = 42 });
        record.VirtualMachineAdapter.Scripts.Add(script);
        return record;
    }

    private static void Add(SkyrimMod mod, APlacedTrap record)
    {
        var cell = new Cell(CellKey, SkyrimRelease.SkyrimSE) { Flags = Cell.Flag.IsInteriorCell };
        cell.Temporary.Add(record);
        var block = new CellBlock { BlockNumber = 0, GroupType = GroupTypeEnum.InteriorCellBlock };
        var sub = new CellSubBlock { BlockNumber = 0, GroupType = GroupTypeEnum.InteriorCellSubBlock };
        sub.Cells.Add(cell); block.SubBlocks.Add(sub); mod.Cells.Add(block);
    }

    private static MemoryStream Write(ISkyrimModGetter mod)
    {
        var stream = new MemoryStream();
        mod.WriteToBinary(stream); stream.Position = 0;
        return stream;
    }
}
