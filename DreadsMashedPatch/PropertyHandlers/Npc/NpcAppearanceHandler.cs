using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Formatting;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.Npc;

public sealed record NpcAppearanceValue(
    INpcGetter Snapshot, NpcConfiguration.Flag AppearanceFlags);

// Generated masked snapshots follow the project's complete-value copy pattern.
// Approved masked flag handlers preserve protection, other configuration bits and unknown bits.
public sealed class NpcAppearanceHandler : AbstractPropertyHandler<NpcAppearanceValue>, IPropertyGroupHandler
{
    public IReadOnlySet<string> GroupedProperties { get; } = new HashSet<string>(StringComparer.Ordinal)
    {
        "HeadParts", "FaceMorph", "FaceParts", "TintLayers", "HairColor", "HeadTexture", "TextureLighting",
        "Race", "Height", "Weight", "WornArmor", "Configuration.Flags.Female", "Configuration.Flags.OppositeGenderAnims"
    };
    private static readonly Mutagen.Bethesda.Skyrim.Npc.TranslationMask CopyMask = CreateMask(includeLists: true);
    private static readonly Mutagen.Bethesda.Skyrim.Npc.TranslationMask EqualityMask = CreateMask(includeLists: false);

    private static Mutagen.Bethesda.Skyrim.Npc.TranslationMask CreateMask(bool includeLists) => new(false)
    {
        HeadParts = includeLists, FaceMorph = true, FaceParts = true, TintLayers = includeLists,
        HairColor = true, HeadTexture = true, TextureLighting = true,
        Race = true, Height = true, Weight = true, WornArmor = true
    };

    private static readonly SimpleReflectionFlagPropertyHandler<NpcConfiguration.Flag, INpc, INpcGetter> AppearanceFlags =
        new("Configuration.Flags", preserveUnknownBits: true,
            includedFlags: [NpcConfiguration.Flag.Female, NpcConfiguration.Flag.OppositeGenderAnims]);

    public override string PropertyName => "Appearance";

    public override NpcAppearanceValue? GetValue(IMajorRecordGetter record) => record is INpcGetter npc
        ? new(npc.DeepCopy(CopyMask), AppearanceFlags.GetValue(npc))
        : throw new ArgumentException("Expected an NPC record.", nameof(record));

    public override void SetValue(IMajorRecord record, NpcAppearanceValue? value)
    {
        if (record is not INpc npc || value is null)
            throw new ArgumentException("Expected an NPC appearance snapshot.");

        ((INpcInternal)npc).DeepCopyIn(value.Snapshot, out var errors, CopyMask);
        if (errors?.IsInError() == true) throw new InvalidOperationException("Could not copy the NPC appearance snapshot.");
        AppearanceFlags.SetValue(npc, value.AppearanceFlags);
    }

    public override bool AreValuesEqual(NpcAppearanceValue? left, NpcAppearanceValue? right) =>
        left is null || right is null ? left is null && right is null
            : NpcMixIn.Equals(left.Snapshot, right.Snapshot, EqualityMask)
                // xEdit defines both collections as sorted. Compare complete contents without
                // treating a reorder as an appearance edit; copying still retains authored order.
                && CollectionComparisonHelper.UnorderedEquals(left.Snapshot.HeadParts, right.Snapshot.HeadParts,
                    (a, b) => a.FormKey == b.FormKey)
                && CollectionComparisonHelper.UnorderedEquals(left.Snapshot.TintLayers, right.Snapshot.TintLayers,
                    (a, b) => TintLayerMixIn.Equals(a, b))
                && AppearanceFlags.AreValuesEqual(left.AppearanceFlags, right.AppearanceFlags);

    public override string FormatValue(object? value) => value is NpcAppearanceValue appearance
        ? $"Race={appearance.Snapshot.Race.FormKey}, HeadParts=[{string.Join(", ", appearance.Snapshot.HeadParts.Select(part => part.FormKey))}], " +
          $"TintLayers={DiagnosticValueFormatter.Format(appearance.Snapshot.TintLayers)}, " +
          $"FaceMorph={DiagnosticValueFormatter.Format(appearance.Snapshot.FaceMorph)}, " +
          $"FaceParts={DiagnosticValueFormatter.Format(appearance.Snapshot.FaceParts)}, Weight={appearance.Snapshot.Weight}, " +
          $"{AppearanceFlags.FormatValue(appearance.AppearanceFlags)}"
        : "null";
}
