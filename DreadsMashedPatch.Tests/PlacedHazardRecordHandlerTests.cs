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
public sealed class PlacedHazardRecordHandlerTests : IDisposable
{
    private static readonly FormKey CellKey = new(CoverageTestHistory.Original, 0x900);
    private static readonly string[] InheritedFields = ["VirtualMachineAdapter", "EncounterZone", "Owner", "FactionRank",
        "HeadTrackingWeight", "FavorCost", "Reflections", "LinkedReferences", "ActivateParents", "EnableParent", "Emittance",
        "MultiBoundReference", "IgnoredBySandbox", "LocationRefTypes", "LocationReference", "DistantLodData", "Scale", "Placement"];

    public void Dispose() => CoverageTestHistory.Reset();
    public static IEnumerable<object[]> Fields() => InheritedFields.Select(field => new object[] { field });
    public static IEnumerable<object[]> Histories() => InheritedFields.SelectMany(field =>
        new[] { false, true }.SelectMany(authorized => new[] { false, true }
            // DATA placement is required in xEdit; exercise its value reversion,
            // rather than constructing a source without placement for a removal test.
            .Where(removal => !removal || field != "Placement")
            .Select(removal => new object[] { field, authorized, removal })));

    [Fact]
    public void RegistersAllInheritedFieldsWithReviewedOwnershipAndHeaderPolicy()
    {
        var handlers = new PlacedHazardRecordHandler().PropertyHandlers;
        Assert.Equal(InheritedFields.Append("EditorID").Append("MajorRecordFlagsRaw").Append("Hazard").Order(), handlers.Keys.Order());
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
    public void OverlayBackedValuesCopyAndRoundTripThroughEveryInheritedHandler(string field)
    {
        var source = new SkyrimMod(CoverageTestHistory.Original, SkyrimRelease.SkyrimSE);
        Add(source, Populated());
        using var sourceStream = Write(source);
        using var sourceOverlay = SkyrimMod.CreateFromBinaryOverlay(sourceStream, SkyrimRelease.SkyrimSE, source.ModKey);
        var getter = Assert.Single(sourceOverlay.EnumerateMajorRecords().OfType<IPlacedHazardGetter>());
        var handler = new PlacedHazardRecordHandler().PropertyHandlers[field];
        var value = handler.GetValue(getter);
        Assert.NotNull(value);
        var target = Empty();
        handler.SetValue(target, value);
        Assert.True(handler.AreValuesEqual(value, handler.GetValue(target)), field);

        var patch = new SkyrimMod(ModKey.FromNameAndExtension("HazardCopy.esp"), SkyrimRelease.SkyrimSE);
        Add(patch, target);
        using var outputStream = Write(patch);
        using var outputOverlay = SkyrimMod.CreateFromBinaryOverlay(outputStream, SkyrimRelease.SkyrimSE, patch.ModKey);
        Assert.True(handler.AreValuesEqual(value, handler.GetValue(
            Assert.Single(outputOverlay.EnumerateMajorRecords().OfType<IPlacedHazardGetter>()))), field);

        handler.SetValue(target, null);
        var result = handler.GetValue(target);
        if (field is "Reflections" or "LinkedReferences") Assert.Empty(Assert.IsAssignableFrom<System.Collections.IEnumerable>(result));
        else Assert.Null(result);
        Assert.True(handler.AreValuesEqual(value, handler.GetValue(getter)), "Source overlay was altered");
    }

    [Theory]
    [MemberData(nameof(Histories))]
    public void FullRunInheritedFieldForwardingAndReversionRespectPermissions(string field, bool authorized, bool removal)
    {
        CoverageTestHistory.Configure(authorized);
        var mods = CoverageTestHistory.Mods();
        var records = mods.Select(_ => removal ? Populated() : Empty()).ToArray();
        var handler = new PlacedHazardRecordHandler().PropertyHandlers[field];
        handler.SetValue(records[1], removal ? null : handler.GetValue(Populated()));
        var selected = handler.GetValue(records[1]);
        for (var i = 0; i < mods.Length; i++) Add(mods[i], records[i]);
        using var state = CoverageTestHistory.State(mods);

        Program.RunPatch(state);

        if (authorized) Assert.Empty(state.PatchMod.EnumerateMajorRecords());
        else
        {
            var output = Assert.Single(state.PatchMod.EnumerateMajorRecords().OfType<IPlacedHazardGetter>());
            Assert.True(handler.AreValuesEqual(selected, handler.GetValue(output)), field);
            using var stream = Write(state.PatchMod);
            using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
            Assert.True(handler.AreValuesEqual(selected, handler.GetValue(
                Assert.Single(overlay.EnumerateMajorRecords().OfType<IPlacedHazardGetter>()))), field);
        }
        Assert.True(handler.AreValuesEqual(selected, handler.GetValue(records[1])), "Earlier source was altered");
        Assert.True(handler.AreValuesEqual(handler.GetValue(records[0]), handler.GetValue(records[2])), "Winner was altered");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Xis2AbsentAndPresentEmptyRemainDistinctInBinaryOutput(bool present)
    {
        var record = Empty();
        record.IgnoredBySandbox = present ? new byte[0] : null;
        var mod = new SkyrimMod(CoverageTestHistory.Original, SkyrimRelease.SkyrimSE);
        Add(mod, record);
        using var stream = Write(mod);
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey);
        var value = Assert.Single(overlay.EnumerateMajorRecords().OfType<IPlacedHazardGetter>()).IgnoredBySandbox;
        Assert.Equal(present, value.HasValue);
        if (present) Assert.Empty(value!.Value.ToArray());
    }

    [Fact]
    public void MutableAggregatesListsLinksAndBytesAreDetachedBeforeOutputMutation()
    {
        var source = Populated();
        var before = source.DeepCopy();
        var output = Empty();
        foreach (var handler in new PlacedHazardRecordHandler().PropertyHandlers.Values)
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

    [Fact]
    public void KeyedLinkedReferenceEditsReplaceTheRowRatherThanAddADuplicate()
    {
        CoverageTestHistory.Configure();
        var mods = CoverageTestHistory.Mods();
        var records = mods.Select(_ => Populated()).ToArray();
        records[1].LinkedReferences[0].Reference.SetTo(new FormKey(CoverageTestHistory.Original, 0x910));
        for (var i = 0; i < mods.Length; i++) Add(mods[i], records[i]);
        using var state = CoverageTestHistory.State(mods);
        Program.RunPatch(state);
        var output = Assert.Single(state.PatchMod.EnumerateMajorRecords().OfType<IPlacedHazardGetter>());
        Assert.Equal(new FormKey(CoverageTestHistory.Original, 0x910), Assert.Single(output.LinkedReferences).Reference.FormKey);
    }

    private static PlacedHazard Empty() => new(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE)
    {
        Hazard = new FormLink<IHazardGetter>(new FormKey(CoverageTestHistory.Original, 0x880)),
        Placement = new Placement()
    };

    private static PlacedHazard Populated()
    {
        var record = Empty();
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

    private static void Add(SkyrimMod mod, PlacedHazard record)
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
