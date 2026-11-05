using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace DreadsMashedPatch.PropertyHandlers.General;

public sealed record TransformValue(IPlacementGetter? Placement, float? Scale);
public sealed record OwnershipValue<TRank>(IFormLinkNullableGetter<IOwnerGetter>? Owner, TRank Rank);
public sealed record RagdollPoseValue(
    ReadOnlyMemorySlice<byte>? RagdollData, ReadOnlyMemorySlice<byte>? RagdollBipedData);

// Like TemplateAndTraits, these groups reuse the member comparers/setters but
// give the complete authored value one selection owner and reversion permission.
public sealed class TransformPropertyHandler<TRecord, TRecordGetter> : AbstractPropertyHandler<TransformValue>, IPropertyGroupHandler
    where TRecord : class, IMajorRecord
    where TRecordGetter : class, IMajorRecordGetter
{
    private readonly PlacementPropertyHandler<TRecord, TRecordGetter> _placement = new();
    private readonly SimpleReflectionPropertyHandler<float?, TRecord, TRecordGetter> _scale = new("Scale");

    public override string PropertyName => "Transform";
    public IReadOnlySet<string> GroupedProperties { get; } = new HashSet<string>(StringComparer.Ordinal)
        { "Placement", "Scale" };

    public override TransformValue GetValue(IMajorRecordGetter record) =>
        new(_placement.GetValue(record)?.DeepCopy(), _scale.GetValue(record));

    public override void SetValue(IMajorRecord record, TransformValue? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _placement.SetValue(record, value.Placement);
        _scale.SetValue(record, value.Scale);
    }

    public override bool AreValuesEqual(TransformValue? left, TransformValue? right) =>
        left is null || right is null ? left is null && right is null
            : _placement.AreValuesEqual(left.Placement, right.Placement) && _scale.AreValuesEqual(left.Scale, right.Scale);

    public override string FormatValue(object? value) => value is TransformValue transform
        ? $"{_placement.FormatValue(transform.Placement)}, Scale={_scale.FormatValue(transform.Scale)}"
        : "null";
}

public class OwnershipPropertyHandler<TRecord, TRecordGetter, TRank>(string rankPropertyName = "FactionRank")
    : AbstractPropertyHandler<OwnershipValue<TRank>>, IPropertyGroupHandler
    where TRecord : class, IMajorRecord
    where TRecordGetter : class, IMajorRecordGetter
{
    private readonly SimpleReflectionFormLinkPropertyHandler<IOwnerGetter, TRecord, TRecordGetter> _owner = new("Owner");
    private readonly SimpleReflectionPropertyHandler<TRank, TRecord, TRecordGetter> _rank = new(rankPropertyName);

    public override string PropertyName => "Ownership";
    public IReadOnlySet<string> GroupedProperties { get; } = new HashSet<string>(StringComparer.Ordinal)
        { "Owner", rankPropertyName };

    public override OwnershipValue<TRank> GetValue(IMajorRecordGetter record)
    {
        var owner = _owner.GetValue(record);
        return new(owner is null ? null : new FormLinkNullable<IOwnerGetter>(owner.FormKeyNullable), _rank.GetValue(record)!);
    }

    public override void SetValue(IMajorRecord record, OwnershipValue<TRank>? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _owner.SetValue(record, value.Owner);
        // ECZN's generated Rank setter expands short DATA even for an unchanged
        // zero. Let actual rank edits promote the layout, without doing so for owner-only edits.
        if (!_rank.AreValuesEqual(_rank.GetValue(record), value.Rank))
            _rank.SetValue(record, value.Rank);
    }

    public override bool AreValuesEqual(OwnershipValue<TRank>? left, OwnershipValue<TRank>? right) =>
        left is null || right is null ? left is null && right is null
            : _owner.AreValuesEqual(left.Owner, right.Owner) && _rank.AreValuesEqual(left.Rank, right.Rank);

    public override string FormatValue(object? value) => value is OwnershipValue<TRank> ownership
        ? $"Owner={_owner.FormatValue(ownership.Owner)}, {rankPropertyName}={_rank.FormatValue(ownership.Rank)}"
        : "null";
}

public sealed class RagdollPosePropertyHandler<TRecord, TRecordGetter> : AbstractPropertyHandler<RagdollPoseValue>, IPropertyGroupHandler
    where TRecord : class, IMajorRecord
    where TRecordGetter : class, IMajorRecordGetter
{
    private readonly SimpleReflectionBinaryDataPropertyHandler<TRecord, TRecordGetter> _bones = new("RagdollData");
    private readonly SimpleReflectionBinaryDataPropertyHandler<TRecord, TRecordGetter> _biped = new("RagdollBipedData");

    public override string PropertyName => "RagdollPose";
    public IReadOnlySet<string> GroupedProperties { get; } = new HashSet<string>(StringComparer.Ordinal)
        { "RagdollData", "RagdollBipedData" };

    public override RagdollPoseValue GetValue(IMajorRecordGetter record) =>
        new(Copy(_bones.GetValue(record)), Copy(_biped.GetValue(record)));

    private static ReadOnlyMemorySlice<byte>? Copy(ReadOnlyMemorySlice<byte>? value) =>
        value is { } bytes ? new ReadOnlyMemorySlice<byte>(bytes.ToArray()) : (ReadOnlyMemorySlice<byte>?)null;

    public override void SetValue(IMajorRecord record, RagdollPoseValue? value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _bones.SetValue(record, value.RagdollData);
        _biped.SetValue(record, value.RagdollBipedData);
    }

    public override bool AreValuesEqual(RagdollPoseValue? left, RagdollPoseValue? right) =>
        left is null || right is null ? left is null && right is null
            : _bones.AreValuesEqual(left.RagdollData, right.RagdollData)
                && _biped.AreValuesEqual(left.RagdollBipedData, right.RagdollBipedData);

    public override string FormatValue(object? value) => value is RagdollPoseValue pose
        ? $"Bones={_bones.FormatValue(pose.RagdollData)}, Biped={_biped.FormatValue(pose.RagdollBipedData)}"
        : "null";
}
