using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("CoverageFixes")]
public sealed class SharedReferenceGroupTests : IDisposable
{
    private static readonly string[] PlacedKinds = ["PlacedObject", "PlacedNpc", "PlacedHazard", "PlacedArrow",
        "PlacedBarrier", "PlacedBeam", "PlacedCone", "PlacedFlame", "PlacedTrap", "PlacedMissile"];
    private static readonly FormKey OwnerA = new(CoverageTestHistory.Original, 0xA00);
    private static readonly FormKey OwnerB = new(CoverageTestHistory.Original, 0xA01);
    private static readonly FormKey CellKey = new(CoverageTestHistory.Original, 0x900);
    public void Dispose() => CoverageTestHistory.Reset();

    public static IEnumerable<object[]> Groups() => PlacedKinds.SelectMany(kind =>
        (kind is "PlacedNpc" or "PlacedObject" ? new[] { "Ownership", "RagdollPose" } : new[] { "Transform", "Ownership" })
            .Select(group => new object[] { kind, group }))
        .Concat(new[] { new object[] { "Cell", "Ownership" }, new object[] { "EncounterZone", "Ownership" } });
    public static IEnumerable<object[]> Conflicts() => Groups().SelectMany(row =>
        new[] { false, true }.Select(reverse => new object[] { row[0], row[1], reverse }));
    public static IEnumerable<object[]> Restorations() => Groups().SelectMany(row =>
        new[] { "none", "declared", "virtual" }.SelectMany(permission => new[] { false, true }
            .Select(empty => new object[] { row[0], row[1], permission, empty })));

    [Theory]
    [MemberData(nameof(Groups))]
    public void RegisteredGroupsOwnEachMemberExactlyOnce(string kind, string group)
    {
        var handlers = Handler(kind).PropertyHandlers;
        var membership = Assert.IsAssignableFrom<IPropertyGroupHandler>(handlers[group]).GroupedProperties;
        Assert.Equal(2, membership.Count);
        foreach (var member in membership)
        {
            Assert.DoesNotContain(member, handlers.Keys);
            Assert.Single(handlers.Values.OfType<IPropertyGroupHandler>(), other => other.GroupedProperties.Contains(member));
        }
    }

    [Theory]
    [MemberData(nameof(Conflicts))]
    public void CompetingMembersCannotProduceAnUnauthoredCombination(string kind, string group, bool reverse)
    {
        var original = New(kind);
        var first = New(kind);
        var second = New(kind);
        if (group == "Transform")
        {
            ((IPlaced)first).Placement!.Position = new P3Float(20, 0, 0);
            ((IPlaced)second).Scale = 3;
        }
        else if (group == "Ownership")
        {
            SetRank(first, 7);
            SetOwner(second, OwnerB);
        }
        else
        {
            SetPose(first, 2, 1);
            SetPose(second, 1, 3);
        }
        var earlier = reverse ? second : first;
        var winner = reverse ? first : second;
        AssertGroup(group, winner, Run([original, earlier, winner]));
    }

    [Theory]
    [MemberData(nameof(Restorations))]
    public void SourceOverlaysAndPatchOutputRespectCompleteGroupReversions(string kind, string group, string permission, bool empty)
    {
        var original = New(kind, empty);
        var earlier = New(kind);
        if (group == "Transform")
        {
            ((IPlaced)earlier).Placement!.Position = new P3Float(30, 0, 0);
            ((IPlaced)earlier).Scale = 2;
        }
        else if (group == "Ownership") { SetOwner(earlier, OwnerB); SetRank(earlier, 5); }
        else SetPose(earlier, 2, 3);
        var winner = New(kind, empty);
        AssertGroup(group, permission == "none" ? earlier : winner, Run([original, earlier, winner], permission));
    }

    [Theory]
    [InlineData("safe")]
    [InlineData("restored")]
    [InlineData("ordinary")]
    public void AchrPlacementAndScaleKeepIndependentOwnersFromDisabledFlags(string kind)
    {
        var original = (PlacedNpc)New("PlacedNpc");
        var earlier = (PlacedNpc)New("PlacedNpc");
        earlier.SkyrimMajorRecordFlags = SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled;
        earlier.Placement!.Position = new P3Float(20, 0, kind == "ordinary" ? 10 : -30000);
        earlier.Scale = 2;
        if (kind != "ordinary") earlier.EnableParent = new EnableParent
        {
            Reference = new FormLink<IPlacedGetter>(Constants.Player.FormKey),
            Flags = EnableParent.Flag.SetEnableStateToOppositeOfParent
        };
        var winner = (PlacedNpc)New("PlacedNpc");
        winner.Scale = 3;
        var result = (IPlacedNpcGetter)Run([original, earlier, winner], kind == "restored" ? "declared" : "none");
        Assert.Equal(3f, result.Scale);
        Assert.Equal((kind == "restored" ? winner : earlier).Placement!.Position, result.Placement!.Position);
        Assert.Equal(kind != "restored", result.SkyrimMajorRecordFlags.HasFlag(SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled));
        if (kind == "safe") Assert.Equal(Constants.Player.FormKey, result.EnableParent!.Reference.FormKey);
        else Assert.Null(result.EnableParent);
    }

    public static IEnumerable<object[]> TrapKinds() => PlacedKinds.Where(kind => kind is not "PlacedObject" and not "PlacedNpc")
        .Select(kind => new object[] { kind });

    [Theory]
    [MemberData(nameof(TrapKinds))]
    public void DisabledProjectileAndHazardEditsKeepOrdinaryTransformSelection(string kind)
    {
        var original = New(kind);
        var earlier = New(kind);
        earlier.SkyrimMajorRecordFlags = SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled;
        ((IPlaced)earlier).Placement!.Position = new P3Float(20, 0, -30000);
        ((IPlaced)earlier).EnableParent = new EnableParent
        {
            Reference = new FormLink<IPlacedGetter>(Constants.Player.FormKey),
            Flags = EnableParent.Flag.SetEnableStateToOppositeOfParent
        };
        var winner = New(kind);
        ((IPlaced)winner).Scale = 3;
        Assert.Equal(new P3Float(10, 0, 0), ((IPlacedGetter)Run([original, earlier, winner])).Placement!.Position);
    }

    [Theory]
    [InlineData((byte)0, false)]
    [InlineData((byte)5, true)]
    [InlineData((byte)255, true)]
    public void EncounterZoneWritesSelectedRankWithoutDroppingDestinationTail(byte rank, bool expands)
    {
        var source = (EncounterZone)New("EncounterZone");
        source.Rank = rank;
        source.Owner.SetTo(OwnerB);
        var target = (EncounterZone)New("EncounterZone");
        target.MinLevel = 13;
        target.MaxLevel = 28;
        target.Flags = EncounterZone.Flag.NeverResets;
        target.Rank = 0;
        target.DATADataTypeState = EncounterZone.DATADataType.Break0 | (EncounterZone.DATADataType)2;
        var handler = Handler("EncounterZone").PropertyHandlers["Ownership"];
        handler.SetValue(target, handler.GetValue(source));
        Assert.Equal(!expands, target.DATADataTypeState.HasFlag(EncounterZone.DATADataType.Break0));
        Assert.True(target.DATADataTypeState.HasFlag((EncounterZone.DATADataType)2));
        var written = (IEncounterZoneGetter)RoundTrip(target);
        Assert.Equal(OwnerB, written.Owner.FormKey);
        Assert.Equal(rank, written.Rank);
        Assert.Equal(expands ? (byte)13 : (byte)0, written.MinLevel);
        Assert.Equal(expands ? (byte)28 : (byte)0, written.MaxLevel);
        Assert.Equal(expands ? EncounterZone.Flag.NeverResets : 0, written.Flags);
        Assert.Equal(!expands, written.DATADataTypeState.HasFlag(EncounterZone.DATADataType.Break0));
    }

    [Fact]
    public void EncounterZoneShortSourceClearsRankWithoutShrinkingFullDestination()
    {
        var source = (EncounterZone)New("EncounterZone");
        source.DATADataTypeState = EncounterZone.DATADataType.Break0;
        source.Owner.SetTo(OwnerB);
        var overlaySource = RoundTrip(source);
        var target = (EncounterZone)New("EncounterZone");
        target.Rank = 255;
        target.MinLevel = 13;
        target.MaxLevel = 28;
        target.Flags = EncounterZone.Flag.NeverResets;
        var handler = Handler("EncounterZone").PropertyHandlers["Ownership"];
        handler.SetValue(target, handler.GetValue(overlaySource));
        var written = (IEncounterZoneGetter)RoundTrip(target);
        Assert.Equal(OwnerB, written.Owner.FormKey);
        Assert.Equal((byte)0, written.Rank);
        Assert.Equal((byte)13, written.MinLevel);
        Assert.Equal((byte)28, written.MaxLevel);
        Assert.Equal(EncounterZone.Flag.NeverResets, written.Flags);
        Assert.False(written.DATADataTypeState.HasFlag(EncounterZone.DATADataType.Break0));
    }

    [Theory]
    [InlineData((byte)0)]
    [InlineData((byte)5)]
    [InlineData((byte)255)]
    public void EncounterZoneLegacyWinnerPublishesTheSelectedOwnershipInTheRequiredLayout(byte rank)
    {
        var original = (EncounterZone)New("EncounterZone");
        original.Rank = 0;
        original.DATADataTypeState = EncounterZone.DATADataType.Break0;
        var earlier = (EncounterZone)New("EncounterZone");
        earlier.Owner.SetTo(OwnerB);
        earlier.Rank = rank;
        var winner = original.DeepCopy();
        var written = (IEncounterZoneGetter)Run([original, earlier, winner]);
        Assert.Equal(OwnerB, written.Owner.FormKey);
        Assert.Equal(rank, written.Rank);
        Assert.Equal(rank == 0, written.DATADataTypeState.HasFlag(EncounterZone.DATADataType.Break0));
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void AchrPartialPoseAndAbsentScaleSurviveBinaryCopies(bool bonesPresent, bool bipedPresent)
    {
        var record = (PlacedNpc)New("PlacedNpc", empty: true);
        record.RagdollData = bonesPresent ? Array.Empty<byte>() : (MemorySlice<byte>?)null;
        record.RagdollBipedData = bipedPresent ? new byte[12] : (MemorySlice<byte>?)null;
        var source = (IPlacedNpcGetter)RoundTrip(record);
        var target = (PlacedNpc)New("PlacedNpc");
        var handlers = Handler("PlacedNpc").PropertyHandlers;
        foreach (var group in new[] { "Placement", "Scale", "RagdollPose" })
            handlers[group].SetValue(target, handlers[group].GetValue(source));
        var written = (IPlacedNpcGetter)RoundTrip(target);
        Assert.Null(written.Scale);
        Assert.Equal(bonesPresent, written.RagdollData.HasValue);
        Assert.Equal(bipedPresent, written.RagdollBipedData.HasValue);
        if (bonesPresent) Assert.Empty(written.RagdollData!.Value.ToArray());
    }

    private static AbstractRecordHandler Handler(string kind) => (AbstractRecordHandler)System.Activator.CreateInstance(
        typeof(Program).Assembly.GetType($"DreadsMashedPatch.RecordHandlers.{kind}RecordHandler")!)!;

    private static ISkyrimMajorRecord New(string kind, bool empty = false)
    {
        var result = (ISkyrimMajorRecord)System.Activator.CreateInstance(typeof(SkyrimMod).Assembly.GetType($"Mutagen.Bethesda.Skyrim.{kind}")!,
            CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE)!;
        if (result is IPlaced placed)
        {
            placed.Placement = new Placement { Position = new P3Float(10, 0, 0) };
            placed.Scale = empty ? null : 1;
        }
        if (result is ICell cell) cell.Flags = Cell.Flag.IsInteriorCell;
        SetOwner(result, empty ? null : OwnerA);
        SetRank(result, empty ? null : 1);
        if (!empty && kind is "PlacedObject" or "PlacedNpc") SetPose(result, 1, 1);
        return result;
    }

    private static void SetOwner(ISkyrimMajorRecord record, FormKey? owner)
    {
        switch (record)
        {
            case IPlaced placed: placed.Owner.SetTo(owner); break;
            case ICell cell: cell.Owner.SetTo(owner); break;
            case IEncounterZone zone: zone.Owner.SetTo(owner ?? FormKey.Null); break;
        }
    }

    private static void SetRank(ISkyrimMajorRecord record, int? rank)
    {
        switch (record)
        {
            case IPlaced placed: placed.FactionRank = rank; break;
            case ICell cell: cell.FactionRank = rank; break;
            case IEncounterZone zone: zone.Rank = (byte)(rank ?? 0); break;
        }
    }

    private static void SetPose(ISkyrimMajorRecord record, byte bones, byte biped)
    {
        var data = new byte[28]; data[0] = bones;
        var rotation = new byte[12]; rotation[0] = biped;
        if (record is IPlacedNpc npc) { npc.RagdollData = data; npc.RagdollBipedData = rotation; }
        else if (record is IPlacedObject obj) { obj.RagdollData = data; obj.RagdollBipedData = rotation; }
    }

    private static void AssertGroup(string group, ISkyrimMajorRecordGetter expected, ISkyrimMajorRecordGetter actual)
    {
        if (group == "Transform")
        {
            var left = (IPlacedGetter)expected; var right = (IPlacedGetter)actual;
            Assert.Equal(left.Placement!.Position, right.Placement!.Position);
            Assert.Equal(left.Placement.Rotation, right.Placement.Rotation);
            Assert.Equal(left.Scale, right.Scale);
        }
        else if (group == "Ownership")
        {
            var left = Ownership(expected); var right = Ownership(actual);
            Assert.Equal(left, right);
        }
        else if (expected is IPlacedNpcGetter npc)
        {
            var result = (IPlacedNpcGetter)actual;
            Assert.Equal(npc.RagdollData?.ToArray(), result.RagdollData?.ToArray());
            Assert.Equal(npc.RagdollBipedData?.ToArray(), result.RagdollBipedData?.ToArray());
        }
        else
        {
            var left = (IPlacedObjectGetter)expected; var right = (IPlacedObjectGetter)actual;
            Assert.Equal(left.RagdollData?.ToArray(), right.RagdollData?.ToArray());
            Assert.Equal(left.RagdollBipedData?.ToArray(), right.RagdollBipedData?.ToArray());
        }
    }

    private static (FormKey? Owner, int? Rank) Ownership(ISkyrimMajorRecordGetter record) => record switch
    {
        IPlacedGetter placed => (placed.Owner.FormKeyNullable, placed.FactionRank),
        ICellGetter cell => (cell.Owner.FormKeyNullable, cell.FactionRank),
        IEncounterZoneGetter zone => (zone.Owner.FormKey.IsNull ? null : zone.Owner.FormKey, zone.Rank),
        _ => throw new ArgumentException("Expected a record with ownership.")
    };

    private static void Add(SkyrimMod mod, ISkyrimMajorRecord record)
    {
        if (record is EncounterZone zone) { mod.EncounterZones.Add(zone); return; }
        var cell = record as Cell ?? new Cell(CellKey, SkyrimRelease.SkyrimSE) { Flags = Cell.Flag.IsInteriorCell };
        if (record is IPlaced placed) cell.Temporary.Add(placed);
        var block = new CellBlock(); var sub = new CellSubBlock();
        sub.Cells.Add(cell); block.SubBlocks.Add(sub); mod.Cells.Add(block);
    }

    private static ISkyrimMajorRecord Run(ISkyrimMajorRecord[] records, string permission = "none")
    {
        CoverageTestHistory.Configure(permission == "virtual");
        var mods = CoverageTestHistory.Mods();
        if (permission == "declared") mods[2].ModHeader.MasterReferences.Add(new MasterReference { Master = CoverageTestHistory.Earlier });
        var streams = new List<MemoryStream>();
        var overlays = new List<ISkyrimModDisposableGetter>();
        try
        {
            for (var i = 0; i < mods.Length; i++)
            {
                Add(mods[i], records[i]);
                var binary = new MemoryStream(); streams.Add(binary);
                mods[i].WriteToBinary(binary, new BinaryWriteParameters { MastersListContent = MastersListContentOption.NoCheck });
                binary.Position = 0;
                overlays.Add(SkyrimMod.CreateFromBinaryOverlay(binary, SkyrimRelease.SkyrimSE, mods[i].ModKey));
            }
            using var state = CoverageTestHistory.State(overlays.ToArray());
            Program.RunPatch(state);
            var selected = state.PatchMod.EnumerateMajorRecords<ISkyrimMajorRecordGetter>()
                .SingleOrDefault(record => record.FormKey == CoverageTestHistory.RecordKey);
            return selected is null ? records[2] : RoundTrip(selected.DeepCopy());
        }
        finally
        {
            foreach (var overlay in overlays) overlay.Dispose();
            foreach (var stream in streams) stream.Dispose();
        }
    }

    private static ISkyrimMajorRecord RoundTrip(ISkyrimMajorRecord record)
    {
        var mod = new SkyrimMod(ModKey.FromNameAndExtension("SharedGroupCopy.esp"), SkyrimRelease.SkyrimSE);
        Add(mod, record);
        using var stream = new MemoryStream(); mod.WriteToBinary(stream); stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey);
        return overlay.EnumerateMajorRecords<ISkyrimMajorRecordGetter>()
            .Single(result => result.FormKey == record.FormKey).DeepCopy();
    }
}
