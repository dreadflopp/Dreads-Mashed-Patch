namespace DreadsMashedPatch.RecordHandlers;

public sealed class PropertyForwardingCoordination
{
    public static PropertyForwardingCoordination None { get; } = new(
        new Dictionary<string, object?>(),
        null);

    public PropertyForwardingCoordination(
        IReadOnlyDictionary<string, object?> forwardValues,
        string? auditMessage,
        string auditProperty = "Coordination")
    {
        ForwardValues = forwardValues;
        AuditMessage = auditMessage;
        AuditProperty = auditProperty;
    }

    public IReadOnlyDictionary<string, object?> ForwardValues { get; }

    public string? AuditMessage { get; }

    public string AuditProperty { get; }
}
