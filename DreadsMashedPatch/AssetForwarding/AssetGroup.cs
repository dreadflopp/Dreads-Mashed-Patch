namespace DreadsMashedPatch.AssetForwarding;

/// <summary>
/// Policy ownership of registered values. Member handlers remain the sole copy/comparison
/// implementations; their independent update contexts are disabled for these members.
/// </summary>
public sealed record AssetGroup(string Name, ForwardingRule Family, IReadOnlyList<string> Members)
{
    public AssetGroup(string name, ForwardingRule family, params string[] members)
        : this(name, family, (IReadOnlyList<string>)members) { }
}
