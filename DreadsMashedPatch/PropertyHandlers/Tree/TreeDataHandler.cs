using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.Tree;

public sealed record TreeDataValue(float TrunkFlexibility, float BranchFlexibility,
    byte[] Unknown, float LeafAmplitude, float LeafFrequency);

// CNAM is twelve floats: Mutagen exposes the middle eight as a 32-byte block.
// Follow the complete preset pattern used for VOLI; preserve the opaque bytes exactly.
public sealed class TreeDataHandler : AbstractPropertyHandler<TreeDataValue>, IPropertyGroupHandler
{
    public IReadOnlySet<string> GroupedProperties { get; } = new HashSet<string>(StringComparer.Ordinal)
        { "TrunkFlexibility", "BranchFlexibility", "Unknown", "LeafAmplitude", "LeafFrequency" };
    public override string PropertyName => "TreeData";

    public override TreeDataValue? GetValue(IMajorRecordGetter record) => record is ITreeGetter tree
        ? new(tree.TrunkFlexibility, tree.BranchFlexibility, tree.Unknown.ToArray(), tree.LeafAmplitude, tree.LeafFrequency)
        : throw new ArgumentException("Expected a TREE record.", nameof(record));

    public override void SetValue(IMajorRecord record, TreeDataValue? value)
    {
        if (record is not ITree tree || value is null || value.Unknown.Length != 32)
            throw new ArgumentException("Expected complete TREE data with a 32-byte animation block.");
        tree.TrunkFlexibility = value.TrunkFlexibility;
        tree.BranchFlexibility = value.BranchFlexibility;
        tree.Unknown = value.Unknown.ToArray();
        tree.LeafAmplitude = value.LeafAmplitude;
        tree.LeafFrequency = value.LeafFrequency;
    }

    public override bool AreValuesEqual(TreeDataValue? left, TreeDataValue? right) =>
        left is null || right is null ? left is null && right is null
            : left.TrunkFlexibility.Equals(right.TrunkFlexibility) && left.BranchFlexibility.Equals(right.BranchFlexibility)
                && left.LeafAmplitude.Equals(right.LeafAmplitude) && left.LeafFrequency.Equals(right.LeafFrequency)
                && left.Unknown.AsSpan().SequenceEqual(right.Unknown);

    public override string FormatValue(object? value) => value is TreeDataValue tree
        ? $"Trunk={tree.TrunkFlexibility}, Branch={tree.BranchFlexibility}, LeafAmplitude={tree.LeafAmplitude}, " +
          $"LeafFrequency={tree.LeafFrequency}, AnimationBlock={Convert.ToHexString(tree.Unknown)}"
        : "null";
}
