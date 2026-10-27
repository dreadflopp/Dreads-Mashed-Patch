using DreadsMashedPatch.PropertyHandlers.General;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.IdleMarker;

/// <summary>Approved masked flag copying with the same absent-zero normalization as water flags.</summary>
public sealed class FlagsHandler() : SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.IdleMarker.Flag,
    IIdleMarker, IIdleMarkerGetter>("Flags", preserveUnknownBits: true)
{
    public override void SetValue(IMajorRecord record, Mutagen.Bethesda.Skyrim.IdleMarker.Flag value)
    {
        if (record is not IIdleMarker marker)
            throw new ArgumentException("Expected an IDLM record.", nameof(record));
        var wasAbsent = marker.Flags is null;
        base.SetValue(record, value);
        if (wasAbsent && marker.Flags.GetValueOrDefault() == 0) marker.Flags = null;
    }
}
