namespace DreadsMashedPatch.PropertyHandlers.Interfaces;

/// <summary>Semantic fields owned by one complete snapshot, for coverage and overlap audits.</summary>
public interface IPropertyGroupHandler
{
    IReadOnlySet<string> GroupedProperties { get; }
}
