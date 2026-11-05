using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("CoverageFixes")]
public sealed class PlacedObjectGroupTests : IDisposable
{
    private static readonly FormKey CellKey = new(CoverageTestHistory.Original, 0x900);
    private static readonly FormKey OwnerA = new(CoverageTestHistory.Original, 0xA00);
    private static readonly FormKey OwnerB = new(CoverageTestHistory.Original, 0xA01);
    private static readonly string[] Groups = ["Transform", "Ownership", "RagdollPose"];

    public void Dispose() => CoverageTestHistory.Reset();

    [Fact]
    public void GroupMembershipIsDisjointAndHasNoParallelLeafRegistrations()
    {
        var handlers = new PlacedObjectRecordHandler().PropertyHandlers;
        var members = new HashSet<string>();
        foreach (var name in Groups)
        foreach (var member in Assert.IsAssignableFrom<IPropertyGroupHandler>(handlers[name]).GroupedProperties)
        {
            Assert.True(members.Add(member), member);
            Assert.DoesNotContain(member, handlers.Keys);
        }
        Assert.Equal(new[] { "FactionRank", "Owner", "Placement", "RagdollBipedData", "RagdollData", "Scale" }, members.Order());
    }

    public static IEnumerable<object[]> CompetingCases() => Groups.SelectMany(group =>
        new[] { false, true }.Select(reverse => new object[] { group, reverse }));

    [Theory]
    [MemberData(nameof(CompetingCases))]
    public void CompetingMemberEditsSelectOneAuthoredGroupInEitherOrder(string group, bool reverse)
    {
        var original = Populated();
        var first = original.DeepCopy();
        var second = original.DeepCopy();
        switch (group)
        {
            case "Transform": first.Placement!.Position = new P3Float(20, 0, 0); second.Scale = 3; break;
            case "Ownership": first.FactionRank = 4; second.Owner.SetTo(OwnerB); break;
            case "RagdollPose": first.RagdollData = Bytes(2, 28); second.RagdollBipedData = Bytes(3, 12); break;
        }
        var earlier = reverse ? second : first;
        var winner = reverse ? first : second;
        earlier.ItemCount = 9; // Ensure an output override also exercises unrelated forwarding.
        winner.MajorRecordFlagsRaw = 0x00200000;

        var output = Run([original, earlier, winner]);

        AssertGroup(group, winner, output);
        Assert.Equal(9, output.ItemCount);
        Assert.Equal(0x00200000, output.MajorRecordFlagsRaw);
    }

    public static IEnumerable<object[]> RestorationCases() => Groups.SelectMany(group =>
        new[] { "none", "declared", "virtual" }.SelectMany(permission =>
            new[] { false, true }.Select(empty => new object[] { group, permission, empty })));

    [Theory]
    [MemberData(nameof(RestorationCases))]
    public void CompleteGroupRestorationAndRemovalRespectPermissions(string group, string permission, bool empty)
    {
        var original = empty ? Empty() : Populated();
        var earlier = Changed(original, group);
        earlier.ItemCount = 9;
        var winner = original.DeepCopy();
        var output = Run([original, earlier, winner], permission);
        AssertGroup(group, permission == "none" ? earlier : winner, output);
    }

    [Theory]
    [InlineData("Transform")]
    [InlineData("Ownership")]
    [InlineData("RagdollPose")]
    public void CopiedUnchangedGroupDoesNotTakeOwnership(string group)
    {
        var original = Populated();
        var earlier = Changed(original, group);
        var copied = earlier.DeepCopy();
        var winner = original.DeepCopy();
        // The last plugin knows the author of the edit, but not its later copier.
        var output = Run([original, earlier, copied, winner], "declared");
        AssertGroup(group, winner, output);
    }

    [Theory]
    [InlineData("Transform", "ignored")]
    [InlineData("Ownership", "ignored")]
    [InlineData("RagdollPose", "ignored")]
    [InlineData("Transform", "priority")]
    [InlineData("Ownership", "priority")]
    [InlineData("RagdollPose", "priority")]
    public void IgnoredAndPrioritySourcesRetainTheirPolicies(string group, string policy)
    {
        var original = Populated();
        var earlier = Changed(original, group);
        var winner = original.DeepCopy();
        var output = Run([original, earlier, winner], policy: policy);
        AssertGroup(group, policy == "priority" ? earlier : winner, output);
    }

    [Fact]
    public void TransformRetainsPrecisionAndOptionalScaleSemantics()
    {
        var handler = new TransformPropertyHandler<IPlacedObject, IPlacedObjectGetter>();
        var first = Populated();
        first.Placement!.Position = new P3Float(1.0000001f, 0, 0);
        first.Placement.Rotation = new P3Float(0, 0, 0);
        var second = first.DeepCopy();
        second.Placement!.Position = new P3Float(1.0000004f, 0, 0);
        second.Placement.Rotation = new P3Float(0, 0, MathF.Tau);
        second.Scale += 0.00001f;
        Assert.True(handler.AreValuesEqual(handler.GetValue(first), handler.GetValue(second)));
        second.Scale = null;
        Assert.False(handler.AreValuesEqual(handler.GetValue(first), handler.GetValue(second)));
        second.Scale = 1.01f;
        Assert.False(handler.AreValuesEqual(handler.GetValue(first), handler.GetValue(second)));
    }

    [Fact]
    public void GroupSnapshotsAndWritesDetachPlacementLinksAndBytes()
    {
        var source = Populated();
        var expected = source.DeepCopy();
        var handlers = new PlacedObjectRecordHandler().PropertyHandlers;
        var values = Groups.ToDictionary(name => name, name => handlers[name].GetValue(source));
        source.Placement!.Position = new P3Float(99, 0, 0);
        source.Owner.SetTo(OwnerB);
        var sourceBones = source.RagdollData!.Value;
        sourceBones[0] = 99;
        var target = Empty();
        foreach (var name in Groups) handlers[name].SetValue(target, values[name]);
        foreach (var name in Groups) AssertGroup(name, expected, target);
        target.Placement!.Position = new P3Float(88, 0, 0);
        target.Owner.SetTo(OwnerB);
        var targetBiped = target.RagdollBipedData!.Value;
        targetBiped[0] = 88;
        var secondTarget = Empty();
        foreach (var name in Groups) handlers[name].SetValue(secondTarget, values[name]);
        foreach (var name in Groups) AssertGroup(name, expected, secondTarget);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void PartialPoseAndAbsentScaleSurviveOverlayCopyAndOutput(bool bonesPresent, bool bipedPresent)
    {
        var source = Empty();
        source.RagdollData = bonesPresent ? Array.Empty<byte>() : (MemorySlice<byte>?)null;
        source.RagdollBipedData = bipedPresent ? Bytes(1, 12) : (MemorySlice<byte>?)null;
        var changed = source.DeepCopy();
        changed.ItemCount = 9;
        var result = Run([source, changed, source.DeepCopy()]);
        Assert.Null(result.Scale);
        Assert.Equal(bonesPresent, result.RagdollData.HasValue);
        Assert.Equal(bipedPresent, result.RagdollBipedData.HasValue);
        if (bonesPresent) Assert.Empty(result.RagdollData!.Value.ToArray());
    }

    [Theory]
    [InlineData("safe", false)]
    [InlineData("safe", true)]
    [InlineData("restored", false)]
    [InlineData("restored", true)]
    [InlineData("ordinary", false)]
    [InlineData("ordinary", true)]
    public void DisabledFlagsDoNotChangeTransformGroupSelection(string kind, bool absentScale)
    {
        var original = Populated();
        var earlier = original.DeepCopy();
        earlier.SkyrimMajorRecordFlags = SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled;
        earlier.Placement!.Position = new P3Float(20, 0, kind == "ordinary" ? 10 : -30000);
        if (kind != "ordinary") earlier.EnableParent = new EnableParent
        {
            Reference = new FormLink<IPlacedGetter>(Constants.Player.FormKey),
            Flags = EnableParent.Flag.SetEnableStateToOppositeOfParent
        };
        earlier.Scale = 2;
        earlier.ItemCount = 9;
        var winner = original.DeepCopy();
        winner.Scale = absentScale ? null : 3;
        var output = Run([original, earlier, winner], kind == "restored" ? "declared" : "none");

        Assert.Equal(winner.Scale, output.Scale);
        Assert.Equal(winner.Placement!.Position, output.Placement!.Position);
        Assert.Equal(kind != "restored", output.SkyrimMajorRecordFlags.HasFlag(SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled));
        if (kind == "safe") Assert.Equal(Constants.Player.FormKey, output.EnableParent!.Reference.FormKey);
        else Assert.Null(output.EnableParent);
    }

    private static PlacedObject Changed(PlacedObject original, string group)
    {
        var result = original.DeepCopy();
        switch (group)
        {
            case "Transform": result.Placement!.Position = new P3Float(30, 0, 0); result.Scale = 2; break;
            case "Ownership": result.Owner.SetTo(OwnerB); result.FactionRank = 5; break;
            case "RagdollPose": result.RagdollData = Bytes(2, 28); result.RagdollBipedData = Bytes(3, 12); break;
        }
        return result;
    }

    private static PlacedObject Empty() => new(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE)
    {
        Placement = new Placement { Position = new P3Float(10, 0, 0) }, ItemCount = 1
    };

    private static PlacedObject Populated()
    {
        var record = Empty();
        record.Scale = 1;
        record.Owner.SetTo(OwnerA);
        record.FactionRank = 1;
        record.RagdollData = Bytes(1, 28);
        record.RagdollBipedData = Bytes(1, 12);
        return record;
    }

    private static byte[] Bytes(byte first, int length)
    {
        var bytes = new byte[length];
        bytes[0] = first;
        return bytes;
    }

    private static void AssertGroup(string group, IPlacedObjectGetter expected, IPlacedObjectGetter actual)
    {
        switch (group)
        {
            case "Transform":
                Assert.Equal(expected.Placement!.Position, actual.Placement!.Position);
                Assert.Equal(expected.Placement.Rotation, actual.Placement.Rotation);
                Assert.Equal(expected.Scale, actual.Scale);
                break;
            case "Ownership":
                Assert.Equal(expected.Owner.FormKeyNullable, actual.Owner.FormKeyNullable);
                Assert.Equal(expected.FactionRank, actual.FactionRank);
                break;
            case "RagdollPose":
                Assert.Equal(expected.RagdollData?.ToArray(), actual.RagdollData?.ToArray());
                Assert.Equal(expected.RagdollBipedData?.ToArray(), actual.RagdollBipedData?.ToArray());
                break;
        }
    }

    private static PlacedObject Run(PlacedObject[] records, string permission = "none", string? policy = null)
    {
        CoverageTestHistory.Configure();
        var keys = new[] { CoverageTestHistory.Original, CoverageTestHistory.Earlier, CoverageTestHistory.Winner,
            ModKey.FromNameAndExtension("Last.esp") };
        var config = new PatcherConfiguration { CompatibilityRules = [] };
        if (permission == "virtual") config.CompatibilityRules = [new VirtualMasterRule
            { TargetMod = keys[records.Length - 1].FileName.String, VirtualMasters = [keys[1].FileName.String] }];
        if (policy == "ignored") config.IgnoredMods.Add(keys[1].FileName.String);
        if (policy == "priority") config.AlwaysWinningMods.Add(keys[1].FileName.String);
        PatcherSettings.Apply(config);
        var streams = new List<MemoryStream>();
        var overlays = new List<ISkyrimModDisposableGetter>();
        try
        {
            for (var i = 0; i < records.Length; i++)
            {
                var mod = new SkyrimMod(keys[i], SkyrimRelease.SkyrimSE);
                if (i > 0) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = keys[0] });
                if (i == records.Length - 1 && permission == "declared")
                    mod.ModHeader.MasterReferences.Add(new MasterReference { Master = keys[1] });
                var cell = new Cell(CellKey, SkyrimRelease.SkyrimSE);
                cell.Temporary.Add(records[i]);
                var subBlock = new CellSubBlock();
                subBlock.Cells.Add(cell);
                var block = new CellBlock();
                block.SubBlocks.Add(subBlock);
                mod.Cells.Records.Add(block);
                // Retain declared dependencies even if this fixture has no link to them.
                var stream = new MemoryStream();
                mod.WriteToBinary(stream, new BinaryWriteParameters { MastersListContent = MastersListContentOption.NoCheck });
                stream.Position = 0;
                streams.Add(stream);
                overlays.Add(SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, keys[i]));
            }
            using var state = CoverageTestHistory.State(overlays.ToArray());
            Program.RunPatch(state);
            var selected = state.PatchMod.EnumerateMajorRecords<IPlacedObjectGetter>().SingleOrDefault();
            if (selected is null) return records[^1].DeepCopy();
            using var binary = Write(state.PatchMod);
            using var output = SkyrimMod.CreateFromBinaryOverlay(binary, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
            return Assert.Single(output.EnumerateMajorRecords<IPlacedObjectGetter>()).DeepCopy();
        }
        finally
        {
            foreach (var overlay in overlays) overlay.Dispose();
            foreach (var stream in streams) stream.Dispose();
        }
    }

    private static MemoryStream Write(ISkyrimModGetter mod)
    {
        var stream = new MemoryStream();
        mod.WriteToBinary(stream);
        stream.Position = 0;
        return stream;
    }
}
