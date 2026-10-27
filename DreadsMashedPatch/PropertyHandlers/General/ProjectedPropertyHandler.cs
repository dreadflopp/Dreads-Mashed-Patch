using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins.Records;

namespace DreadsMashedPatch.PropertyHandlers.General;

/// <summary>
/// Selects a typed projection of an existing aggregate. The merge writes only that
/// projection, preserving the destination fields owned by another handler.
/// </summary>
public class ProjectedPropertyHandler<T>(
    string propertyName,
    IPropertyHandler aggregate,
    Func<T?, T?> project,
    Func<T?, T?, T?> merge,
    params string[] properties) : AbstractPropertyHandler<T>, IPropertyGroupHandler where T : class
{
    public IReadOnlySet<string> GroupedProperties { get; } = properties.ToHashSet(StringComparer.Ordinal);
    protected IPropertyHandler Aggregate { get; } = aggregate;
    protected T? GetAggregate(IMajorRecordGetter record) => (T?)Aggregate.GetValue(record);
    public override string PropertyName => propertyName;
    public override T? GetValue(IMajorRecordGetter record) => project(GetAggregate(record));
    public override void SetValue(IMajorRecord record, T? value) =>
        Aggregate.SetValue(record, merge(value, GetAggregate(record)));
    public override bool AreValuesEqual(T? left, T? right) => Aggregate.AreValuesEqual(left, right);
}

/// <summary>Runs after parent writers and restricts its snapshot to surviving parents/rows.</summary>
public interface INestedAssetPropertyHandler
{
    object? RestrictToTarget(IMajorRecordGetter record, object? value);
}

public sealed class NestedAssetPropertyHandler<T>(
    string propertyName,
    IPropertyHandler aggregate,
    Func<T?, T?> project,
    Func<T?, T?, T?> merge,
    Func<T?, T?, T?> restrict,
    params string[] properties) : ProjectedPropertyHandler<T>(propertyName, aggregate, project, merge, properties),
        INestedAssetPropertyHandler where T : class
{
    public object? RestrictToTarget(IMajorRecordGetter record, object? value) =>
        restrict((T?)value, GetAggregate(record));
}
