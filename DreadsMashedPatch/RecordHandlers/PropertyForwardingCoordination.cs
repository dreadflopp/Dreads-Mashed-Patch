using Context = Mutagen.Bethesda.Plugins.Cache.IModContext<Mutagen.Bethesda.Skyrim.ISkyrimMod, Mutagen.Bethesda.Skyrim.ISkyrimModGetter, Mutagen.Bethesda.Plugins.Records.IMajorRecord, Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter>;

namespace DreadsMashedPatch.RecordHandlers;

public sealed class PropertyForwardingCoordination
{
    public static PropertyForwardingCoordination None { get; } = new(
        new Dictionary<string, object?>(),
        null);

    public PropertyForwardingCoordination(
        IReadOnlyDictionary<string, object?> forwardValues,
        string? auditMessage,
        string auditProperty = "Coordination",
        Context? overrideContext = null)
    {
        ForwardValues = forwardValues;
        AuditMessage = auditMessage;
        AuditProperty = auditProperty;
        OverrideContext = overrideContext;
    }

    public IReadOnlyDictionary<string, object?> ForwardValues { get; }

    public string? AuditMessage { get; }

    public string AuditProperty { get; }

    public Context? OverrideContext { get; }
}
