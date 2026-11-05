using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("CoverageFixes")]
public sealed class PlacedReferenceForwardingTests : IDisposable
{
    private static readonly ModKey[] ModKeys =
    [
        CoverageTestHistory.Original, CoverageTestHistory.Earlier, CoverageTestHistory.Winner,
        ModKey.FromNameAndExtension("Last.esp")
    ];
    private static readonly FormKey CellKey = new(ModKeys[0], 0x900);
    private static readonly FormKey CustomParent = new(ModKeys[0], 0x901);
    private static readonly FormKey OtherParent = new(ModKeys[0], 0x902);

    public void Dispose() => CoverageTestHistory.Reset();

    public static IEnumerable<object[]> Permissions() =>
        from npc in new[] { false, true }
        from permission in new[] { "none", "declared", "virtual" }
        select new object[] { npc, permission };

    public static IEnumerable<object[]> Transitions() =>
        from row in Permissions()
        from originalDisabled in new[] { false, true }
        select new object[] { row[0], row[1], originalDisabled };

    [Theory]
    [MemberData(nameof(Permissions))]
    public void LuxStyleParentRemovalWithUnchangedDisabledStateRespectsPermission(bool npc, string permission)
    {
        var original = Create(npc);
        var disabled = SafeDisabled(original);
        var winner = disabled.DeepCopy();
        ((IPlaced)winner).EnableParent = null;

        var output = Run([original, disabled, winner], permission, owner: 1);

        Assert.True(IsDisabled(output));
        AssertPlacement(disabled, output);
        AssertParent(permission == "none" ? disabled : winner, output);
    }

    [Theory]
    [MemberData(nameof(Permissions))]
    public void TransformChangesDoNotAuthorizeEnableParentReversions(bool npc, string permission)
    {
        var original = Create(npc);
        var disabled = SafeDisabled(original);
        var parentEdit = disabled.DeepCopy();
        SetParent(parentEdit, CustomParent);
        var winner = parentEdit.DeepCopy();
        SetDisabled(winner, false);
        SetPlacement(winner, -29000, 0.5f);
        ((IPlaced)winner).EnableParent = null;

        var output = Run([original, disabled, parentEdit, winner], permission, owner: 2);

        Assert.True(IsDisabled(output));
        AssertPlacement(winner, output);
        AssertParent(permission == "none" ? parentEdit : winner, output);
    }

    [Theory]
    [MemberData(nameof(Permissions))]
    public void TransformReversionsDoNotBlockNewEnableParentValues(bool npc, string permission)
    {
        var original = Create(npc);
        var disabled = SafeDisabled(original);
        var placementEdit = disabled.DeepCopy();
        SetPlacement(placementEdit, -29000, 0.5f);
        var winner = placementEdit.DeepCopy();
        SetDisabled(winner, false);
        ((IPlaced)winner).Placement = ((IPlaced)original).Placement!.DeepCopy();
        SetParent(winner, CustomParent);

        var output = Run([original, disabled, placementEdit, winner], permission, owner: 2);

        Assert.True(IsDisabled(output));
        AssertPlacement(permission == "none" ? placementEdit : winner, output);
        AssertParent(winner, output);
        Assert.Equal(((IPlacedGetter)original).Scale, ((IPlacedGetter)output).Scale);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void DeniedFlagReversionStillAllowsNewParentAndTransformValues(bool npc, bool originalDisabled)
    {
        var original = Create(npc, originalDisabled);
        var changed = original.DeepCopy();
        SetDisabled(changed, !originalDisabled);
        SetPlacement(changed, 20, 0.5f);
        SetParent(changed, CustomParent);
        var winner = original.DeepCopy();
        SetPlacement(winner, 30, 1);
        SetParent(winner, OtherParent);
        ((IPlaced)winner).Scale = 3;
        winner.MajorRecordFlagsRaw |= 0x0200; // An unrelated approved header bit.

        var output = Run([original, changed, winner]);

        Assert.Equal(!originalDisabled, IsDisabled(output));
        AssertPlacement(winner, output);
        AssertParent(winner, output);
        Assert.Equal(3f, ((IPlacedGetter)output).Scale);
        Assert.NotEqual(0, output.MajorRecordFlagsRaw & 0x0200);
    }

    [Theory]
    [MemberData(nameof(Transitions))]
    public void FlagReversionPermissionDoesNotAuthorizeParentOrTransformReversions(bool npc, string permission, bool originalDisabled)
    {
        var original = Create(npc, originalDisabled);
        var disabled = SafeDisabled(original);
        SetDisabled(disabled, !originalDisabled);
        var edit = disabled.DeepCopy();
        SetPlacement(edit, -29000, 0.5f);
        SetParent(edit, CustomParent);
        var winner = original.DeepCopy();

        var output = Run([original, disabled, edit, winner], permission, owner: 1);

        Assert.Equal(permission == "none" ? !originalDisabled : originalDisabled, IsDisabled(output));
        AssertPlacement(edit, output);
        AssertParent(edit, output);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DeniedFlagReversionStillTransfersOwnershipOfAcceptedPropertyEdits(bool npc)
    {
        var original = Create(npc);
        var disabled = SafeDisabled(original);
        var rejected = original.DeepCopy();
        SetPlacement(rejected, 30, 1);
        SetParent(rejected, CustomParent);
        var winner = disabled.DeepCopy();
        ((IPlaced)winner).Placement = ((IPlaced)original).Placement!.DeepCopy();
        ((IPlaced)winner).EnableParent = null;

        var output = Run([original, disabled, rejected, winner], "virtual", owner: 1);

        Assert.True(IsDisabled(output));
        AssertPlacement(rejected, output);
        AssertParent(rejected, output);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void AcceptedFlagChangeDoesNotForceParentOrTransformSnapshots(bool npc)
    {
        var original = Create(npc);
        var first = original.DeepCopy();
        SetPlacement(first, 20, 0.5f);
        SetParent(first, CustomParent);
        var winner = original.DeepCopy();
        SetDisabled(winner, true);

        var output = Run([original, first, winner]);

        Assert.True(IsDisabled(output));
        AssertPlacement(first, output);
        AssertParent(first, output);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void InitiallyDisabledBaselineAlsoAllowsMatchingStatePropertyEdits(bool npc)
    {
        var original = Create(npc, true);
        var first = original.DeepCopy();
        SetPlacement(first, 20, 0.5f);
        SetParent(first, CustomParent);
        var winner = original.DeepCopy();
        SetPlacement(winner, 30, 1);
        SetParent(winner, OtherParent);

        var output = Run([original, first, winner]);

        Assert.True(IsDisabled(output));
        AssertPlacement(winner, output);
        AssertParent(winner, output);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EnableParentReferenceAndFlagsSelectOneCompleteAuthoredValue(bool npc)
    {
        var original = Create(npc);
        SetParent(original, CustomParent);
        ((IPlaced)original).EnableParent!.Flags = EnableParent.Flag.SetEnableStateToOppositeOfParent;
        var first = original.DeepCopy();
        SetDisabled(first, true);
        ((IPlaced)first).EnableParent!.Reference = new FormLink<IPlacedGetter>(OtherParent);
        var winner = original.DeepCopy();
        ((IPlaced)winner).EnableParent!.Flags = 0;

        var output = Run([original, first, winner]);

        Assert.True(IsDisabled(output));
        AssertParent(winner, output);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CotnStyleDeniedFlagReversionStillAllowsLuxPlacement(bool npc)
    {
        var original = Create(npc);
        ((IPlaced)original).Placement = new Placement
        {
            Position = new P3Float(-1697.795776f, -385.479858f, 141.353683f),
            Rotation = new P3Float(6.133188f, 0, 3.083192f)
        };
        ((IPlaced)original).Scale = 1.0699999f;
        var disabled = SafeDisabled(original);
        ((IPlaced)disabled).Scale = 1.07f;
        var winner = original.DeepCopy();
        ((IPlaced)winner).Placement = new Placement
        {
            Position = new P3Float(-709.832092f, -118.654533f, 141.353683f),
            Rotation = new P3Float(6.133188f, 0, 1.483198f)
        };
        ((IPlaced)winner).Scale = 1.07f;

        var output = Run([original, disabled, winner]);

        Assert.True(IsDisabled(output));
        AssertPlacement(winner, output);
        Assert.Equal(1.07f, ((IPlacedGetter)output).Scale);
        AssertParent(disabled, output);
    }

    private static ISkyrimMajorRecord Create(bool npc, bool disabled = false)
    {
        ISkyrimMajorRecord record = npc
            ? new PlacedNpc(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE)
            : new PlacedObject(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE);
        SetPlacement(record, 100, 0);
        SetDisabled(record, disabled);
        return record;
    }

    private static ISkyrimMajorRecord SafeDisabled(ISkyrimMajorRecord original)
    {
        var record = original.DeepCopy();
        SetDisabled(record, true);
        var placement = ((IPlaced)record).Placement!;
        placement.Position = new P3Float(placement.Position.X, placement.Position.Y, -30000);
        SetParent(record, Constants.Player.FormKey);
        ((IPlaced)record).EnableParent!.Flags = EnableParent.Flag.SetEnableStateToOppositeOfParent;
        return record;
    }

    private static void SetPlacement(ISkyrimMajorRecord record, float z, float rotation) =>
        ((IPlaced)record).Placement = new Placement
        {
            Position = new P3Float(10, 0, z), Rotation = new P3Float(0, 0, rotation)
        };

    private static void SetParent(ISkyrimMajorRecord record, FormKey parent) =>
        ((IPlaced)record).EnableParent = new EnableParent { Reference = new FormLink<IPlacedGetter>(parent) };

    private static void SetDisabled(ISkyrimMajorRecord record, bool disabled)
    {
        var flag = (int)SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled;
        record.MajorRecordFlagsRaw = disabled ? record.MajorRecordFlagsRaw | flag : record.MajorRecordFlagsRaw & ~flag;
    }

    private static bool IsDisabled(ISkyrimMajorRecordGetter record) =>
        record.SkyrimMajorRecordFlags.HasFlag(SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled);

    private static void AssertPlacement(ISkyrimMajorRecordGetter expected, ISkyrimMajorRecordGetter actual)
    {
        Assert.Equal(((IPlacedGetter)expected).Placement!.Position, ((IPlacedGetter)actual).Placement!.Position);
        Assert.Equal(((IPlacedGetter)expected).Placement!.Rotation, ((IPlacedGetter)actual).Placement!.Rotation);
    }

    private static void AssertParent(ISkyrimMajorRecordGetter expected, ISkyrimMajorRecordGetter actual)
    {
        var left = ((IPlacedGetter)expected).EnableParent;
        var right = ((IPlacedGetter)actual).EnableParent;
        Assert.Equal(left?.Reference.FormKey, right?.Reference.FormKey);
        Assert.Equal(left?.Flags, right?.Flags);
    }

    private static ISkyrimMajorRecord Run(ISkyrimMajorRecord[] records, string permission = "none", int owner = 1)
    {
        CoverageTestHistory.Configure();
        PatcherSettings.Apply(new PatcherConfiguration
        {
            CompatibilityRules = permission == "virtual"
                ? [new VirtualMasterRule { TargetMod = ModKeys[records.Length - 1].FileName.String,
                    VirtualMasters = [ModKeys[owner].FileName.String] }]
                : []
        });
        var streams = new List<MemoryStream>();
        var overlays = new List<ISkyrimModDisposableGetter>();
        try
        {
            for (var i = 0; i < records.Length; i++)
            {
                var mod = new SkyrimMod(ModKeys[i], SkyrimRelease.SkyrimSE);
                if (i > 0) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = ModKeys[0] });
                if (i == records.Length - 1 && permission == "declared")
                    mod.ModHeader.MasterReferences.Add(new MasterReference { Master = ModKeys[owner] });
                var cell = new Cell(CellKey, SkyrimRelease.SkyrimSE) { Flags = Cell.Flag.IsInteriorCell };
                cell.Temporary.Add((IPlaced)records[i]);
                var sub = new CellSubBlock(); sub.Cells.Add(cell);
                var block = new CellBlock(); block.SubBlocks.Add(sub); mod.Cells.Add(block);
                var stream = new MemoryStream(); streams.Add(stream);
                mod.WriteToBinary(stream, new BinaryWriteParameters { MastersListContent = MastersListContentOption.NoCheck });
                stream.Position = 0;
                overlays.Add(SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey));
            }

            using var state = CoverageTestHistory.State(overlays.ToArray());
            var report = Program.RunPatchWithReport(state);
            Assert.Empty(report.Errors);
            Assert.Empty(report.SkippedRecords);
            var selected = state.PatchMod.EnumerateMajorRecords<ISkyrimMajorRecordGetter>()
                .SingleOrDefault(record => record.FormKey == CoverageTestHistory.RecordKey);
            if (selected == null) return records[^1].DeepCopy();
            using var outputStream = new MemoryStream();
            state.PatchMod.WriteToBinary(outputStream);
            outputStream.Position = 0;
            using var output = SkyrimMod.CreateFromBinaryOverlay(outputStream, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
            return output.EnumerateMajorRecords<ISkyrimMajorRecordGetter>()
                .Single(record => record.FormKey == CoverageTestHistory.RecordKey).DeepCopy();
        }
        finally
        {
            foreach (var overlay in overlays) overlay.Dispose();
            foreach (var stream in streams) stream.Dispose();
        }
    }
}
