using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.HeadPart;

public sealed record HeadPartAppearanceValue(IHeadPartGetter Snapshot, Mutagen.Bethesda.Skyrim.HeadPart.Flag Flags);

public sealed class HeadPartAppearanceHandler : AbstractWinningPropertyHandler<HeadPartAppearanceValue>, IPropertyGroupHandler
{
    public IReadOnlySet<string> GroupedProperties { get; } = new HashSet<string>(StringComparer.Ordinal)
        { "Model", "Type", "ExtraParts", "Parts", "TextureSet", "Color", "ValidRaces", "Flags" };
    private static readonly Mutagen.Bethesda.Skyrim.HeadPart.TranslationMask CopyMask = new(false)
    {
        Model = true, Type = true, ExtraParts = true, Parts = true,
        TextureSet = true, Color = true, ValidRaces = true
    };
    private static readonly SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.HeadPart.Flag, IHeadPart, IHeadPartGetter> Flags =
        new("Flags", preserveUnknownBits: true);

    public override string PropertyName => "Appearance";

    public override HeadPartAppearanceValue? GetValue(IMajorRecordGetter record) => record is IHeadPartGetter head
        ? new(head.DeepCopy(CopyMask), Flags.GetValue(head))
        : throw new ArgumentException("Expected an HDPT record.", nameof(record));

    public override void SetValue(IMajorRecord record, HeadPartAppearanceValue? value)
    {
        if (record is not IHeadPart head || value is null)
            throw new ArgumentException("Expected an HDPT appearance snapshot.");
        ((IHeadPartInternal)head).DeepCopyIn(value.Snapshot, out var errors, CopyMask);
        if (errors?.IsInError() == true) throw new InvalidOperationException("Could not copy the HDPT appearance snapshot.");
        Flags.SetValue(head, value.Flags);
    }

    public override bool AreValuesEqual(HeadPartAppearanceValue? left, HeadPartAppearanceValue? right) =>
        left is null || right is null ? left is null && right is null
            : HeadPartMixIn.Equals(left.Snapshot, right.Snapshot, CopyMask) && Flags.AreValuesEqual(left.Flags, right.Flags);

    public override string FormatValue(object? value) => value is HeadPartAppearanceValue head
        ? $"Type={head.Snapshot.Type}, Model={AssetPathHelper.Format(head.Snapshot.Model?.File)}, " +
          $"ExtraParts={head.Snapshot.ExtraParts.Count}, MorphFiles={head.Snapshot.Parts.Count}, {Flags.FormatValue(head.Flags)}"
        : "null";
}
