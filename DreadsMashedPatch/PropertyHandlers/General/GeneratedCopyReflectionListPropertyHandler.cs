using DreadsMashedPatch.PropertyHandlers.Abstracts;
using Mutagen.Bethesda.Plugins.Records;
using Noggog;

namespace DreadsMashedPatch.PropertyHandlers.General;

/// <summary>
/// Reflection access for a generated Mutagen list property, with compile-time supplied
/// element copy and equality operations. No overlay collection or element is assigned
/// directly to the mutable record.
/// </summary>
public sealed class GeneratedCopyReflectionListPropertyHandler<TGetter, TMutable, TRecord, TRecordGetter>
    : AbstractListPropertyHandler<TGetter>
    where TGetter : class
    where TMutable : class, TGetter
    where TRecord : class, IMajorRecord
    where TRecordGetter : class, IMajorRecordGetter
{
    private readonly System.Reflection.PropertyInfo _getterProperty;
    private readonly System.Reflection.PropertyInfo _setterProperty;
    private readonly Func<TGetter, TMutable> _copy;
    private readonly Func<TGetter, TGetter, bool> _equals;
    private readonly Func<TGetter, object?>? _keySelector;
    private readonly ListSemantics _semantics;
    private readonly bool _canBeNull;

    public GeneratedCopyReflectionListPropertyHandler(
        string propertyName,
        ListSemantics semantics,
        Func<TGetter, TMutable> copy,
        Func<TGetter, TGetter, bool> equals,
        bool? canBeNull = null,
        Func<TGetter, object?>? keySelector = null)
    {
        PropertyName = propertyName;
        _semantics = semantics;
        _copy = copy;
        _equals = equals;
        _keySelector = keySelector;
        _getterProperty = ReflectionPropertyResolver.Find(typeof(TRecordGetter), propertyName)
            ?? throw new ArgumentException($"Property '{propertyName}' not found on {typeof(TRecordGetter).Name}");
        _setterProperty = ReflectionPropertyResolver.Find(typeof(TRecord), propertyName)
            ?? throw new ArgumentException($"Property '{propertyName}' not found on {typeof(TRecord).Name}");
        _canBeNull = canBeNull ?? ReflectionPropertyResolver.IsNullable(_getterProperty);
    }

    public override string PropertyName { get; }
    public override ListSemantics Semantics => _semantics;
    protected override bool CanBeNull => _canBeNull;

    public override List<TGetter>? GetValue(IMajorRecordGetter record)
    {
        if (record is not TRecordGetter typedRecord)
        {
            return null;
        }

        return _getterProperty.GetValue(typedRecord) is IEnumerable<TGetter> items
            ? items.ToList()
            : null;
    }

    public override void SetValue(IMajorRecord record, List<TGetter>? value)
    {
        if (record is not TRecord typedRecord)
        {
            return;
        }

        if (value == null && _canBeNull)
        {
            if (!_setterProperty.CanWrite)
            {
                throw new InvalidOperationException($"Nullable list '{PropertyName}' is read-only on {typeof(TRecord).Name}");
            }

            _setterProperty.SetValue(typedRecord, null);
            return;
        }

        var target = _setterProperty.GetValue(typedRecord);
        if (target == null)
        {
            if (!_setterProperty.CanWrite)
            {
                throw new InvalidOperationException($"Mutable list '{PropertyName}' was null and read-only on {typeof(TRecord).Name}");
            }

            var newList = new ExtendedList<TMutable>();
            _setterProperty.SetValue(typedRecord, newList);
            target = newList;
        }

        if (target is not ICollection<TMutable> list)
        {
            throw new InvalidOperationException(
                $"Mutable property '{PropertyName}' does not accept {typeof(TMutable).Name} elements");
        }

        list.Clear();
        if (value != null)
        {
            foreach (var item in value)
            {
                list.Add(_copy(item));
            }
        }
    }

    protected override bool IsItemEqual(TGetter? item1, TGetter? item2)
    {
        if (item1 == null || item2 == null)
        {
            return item1 == null && item2 == null;
        }

        return _equals(item1, item2);
    }

    protected override bool IsItemIdentityEqual(TGetter? item1, TGetter? item2)
    {
        if (_keySelector == null)
        {
            return base.IsItemIdentityEqual(item1, item2);
        }

        if (item1 == null || item2 == null)
        {
            return item1 == null && item2 == null;
        }

        return AreKeysEqual(_keySelector(item1), _keySelector(item2));
    }

    protected override IReadOnlyList<object?> GetSortKey(TGetter item)
    {
        if (_keySelector == null)
        {
            return base.GetSortKey(item);
        }

        var key = _keySelector(item);
        return key is IReadOnlyList<object?> composite ? composite : [key];
    }

    internal static bool AreKeysEqual(object? left, object? right)
    {
        if (ReferenceEquals(left, right)) return true;
        if (left == null || right == null) return false;
        if (left is string leftString && right is string rightString)
        {
            return StringComparer.OrdinalIgnoreCase.Equals(leftString, rightString);
        }

        if (left is IReadOnlyList<object?> leftParts && right is IReadOnlyList<object?> rightParts)
        {
            return leftParts.Count == rightParts.Count
                && leftParts.Zip(rightParts, AreKeysEqual).All(equal => equal);
        }

        return Equals(left, right);
    }
}
