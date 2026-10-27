using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.Water;

/// <summary>Disjoint damage/rendering ownership. Approved masked handlers compare and
/// copy bits; zero clears retain destination presence and other/unknown bits.</summary>
public sealed class WaterFlagsHandler : AbstractPropertyHandler<Mutagen.Bethesda.Skyrim.Water.Flag?>
{
    private readonly bool _damageFlags;
    private readonly Mutagen.Bethesda.Skyrim.Water.Flag _mask;
    private readonly SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.Water.Flag, IWater, IWaterGetter> _flags;

    public WaterFlagsHandler(bool damageFlags)
    {
        _damageFlags = damageFlags;
        _mask = damageFlags ? Mutagen.Bethesda.Skyrim.Water.Flag.CausesDamage
            : Mutagen.Bethesda.Skyrim.Water.Flag.EnableFlowmap | Mutagen.Bethesda.Skyrim.Water.Flag.BlendNormals;
        _flags = new("Flags", preserveUnknownBits: true,
            includedFlags: damageFlags ? [Mutagen.Bethesda.Skyrim.Water.Flag.CausesDamage]
                : [Mutagen.Bethesda.Skyrim.Water.Flag.EnableFlowmap, Mutagen.Bethesda.Skyrim.Water.Flag.BlendNormals]);
    }

    public override string PropertyName => _damageFlags ? "Flags" : "Flags.Surface";
    public override Mutagen.Bethesda.Skyrim.Water.Flag? GetValue(IMajorRecordGetter record)
    {
        var value = ((IWaterGetter)record).Flags;
        return value.GetValueOrDefault() & _mask;
    }

    public override void SetValue(IMajorRecord record, Mutagen.Bethesda.Skyrim.Water.Flag? value)
    {
        var water = (IWater)record;
        var wasAbsent = water.Flags == null;
        _flags.SetValue(record, value.GetValueOrDefault() & _mask);
        // Zero clears retain destination presence. A nonzero selected bit necessarily
        // materializes the field, but an asset-only change cannot forward present-zero.
        if (water.Flags.GetValueOrDefault() == 0 && wasAbsent)
            water.Flags = null;
    }

    public override bool AreValuesEqual(Mutagen.Bethesda.Skyrim.Water.Flag? left,
        Mutagen.Bethesda.Skyrim.Water.Flag? right) =>
        _flags.AreValuesEqual(left.GetValueOrDefault(), right.GetValueOrDefault());
}
