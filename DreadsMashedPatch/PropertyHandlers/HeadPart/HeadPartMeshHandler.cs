using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.HeadPart;

public sealed record HeadPartMeshValue(IHeadPartGetter Snapshot, Mutagen.Bethesda.Skyrim.HeadPart.Flag Flags);

public sealed class HeadPartMeshHandler : AbstractPropertyHandler<HeadPartMeshValue>, IPropertyGroupHandler
{
    public IReadOnlySet<string> GroupedProperties { get; } = new HashSet<string>(StringComparer.Ordinal)
        { "Model", "Type", "ExtraParts", "Parts", "TextureSet", "Color", "Flags.IsExtraPart", "Flags.UseSolidTint" };
    private static readonly Mutagen.Bethesda.Skyrim.HeadPart.TranslationMask CopyMask = CreateMask(includeExtraParts: true);
    private static readonly Mutagen.Bethesda.Skyrim.HeadPart.TranslationMask EqualityMask = CreateMask(includeExtraParts: false);

    private static Mutagen.Bethesda.Skyrim.HeadPart.TranslationMask CreateMask(bool includeExtraParts) => new(false)
    {
        Model = true, Type = true, ExtraParts = includeExtraParts, Parts = true,
        TextureSet = true, Color = true
    };
    private static readonly SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.HeadPart.Flag, IHeadPart, IHeadPartGetter> Flags =
        new("Flags", preserveUnknownBits: true, includedFlags: [Mutagen.Bethesda.Skyrim.HeadPart.Flag.IsExtraPart, Mutagen.Bethesda.Skyrim.HeadPart.Flag.UseSolidTint]);

    public override string PropertyName => "Mesh";

    public override HeadPartMeshValue? GetValue(IMajorRecordGetter record) => record is IHeadPartGetter head
        ? new(head.DeepCopy(CopyMask), Flags.GetValue(head))
        : throw new ArgumentException("Expected an HDPT record.", nameof(record));

    public override void SetValue(IMajorRecord record, HeadPartMeshValue? value)
    {
        if (record is not IHeadPart head || value is null)
            throw new ArgumentException("Expected an HDPT mesh snapshot.");
        ((IHeadPartInternal)head).DeepCopyIn(value.Snapshot, out var errors, CopyMask);
        if (errors?.IsInError() == true) throw new InvalidOperationException("Could not copy the HDPT mesh snapshot.");
        Flags.SetValue(head, value.Flags);
    }

    public override bool AreValuesEqual(HeadPartMeshValue? left, HeadPartMeshValue? right) =>
        left is null || right is null ? left is null && right is null
            : HeadPartMixIn.Equals(left.Snapshot, right.Snapshot, EqualityMask)
                && CollectionComparisonHelper.UnorderedEquals(left.Snapshot.ExtraParts, right.Snapshot.ExtraParts,
                    (a, b) => a.FormKey == b.FormKey)
                && Flags.AreValuesEqual(left.Flags, right.Flags);

    public override string FormatValue(object? value) => value is HeadPartMeshValue head
        ? $"Type={head.Snapshot.Type}, Model={AssetPathHelper.Format(head.Snapshot.Model?.File)}, " +
          $"ExtraParts={head.Snapshot.ExtraParts.Count}, MorphFiles={head.Snapshot.Parts.Count}, {Flags.FormatValue(head.Flags)}"
        : "null";
}
