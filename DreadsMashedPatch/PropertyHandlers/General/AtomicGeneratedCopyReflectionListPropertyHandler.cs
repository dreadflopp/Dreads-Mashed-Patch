using DreadsMashedPatch.PropertyHandlers.Abstracts;
using Mutagen.Bethesda.Plugins.Records;

namespace DreadsMashedPatch.PropertyHandlers.General;

/// <summary>
/// Treats a generated-copy list as one atomic property value while using Mutagen's
/// generated element copy and equality operations for overlay safety.
/// </summary>
public sealed class AtomicGeneratedCopyReflectionListPropertyHandler<TGetter, TMutable, TRecord, TRecordGetter>
    : AbstractPropertyHandler<List<TGetter>>
    where TGetter : class
    where TMutable : class, TGetter
    where TRecord : class, IMajorRecord
    where TRecordGetter : class, IMajorRecordGetter
{
    private readonly GeneratedCopyReflectionListPropertyHandler<TGetter, TMutable, TRecord, TRecordGetter> _accessor;

    public AtomicGeneratedCopyReflectionListPropertyHandler(
        string propertyName,
        Func<TGetter, TMutable> copy,
        Func<TGetter, TGetter, bool> equals,
        bool? canBeNull = null)
    {
        _accessor = new GeneratedCopyReflectionListPropertyHandler<TGetter, TMutable, TRecord, TRecordGetter>(
            propertyName,
            ListSemantics.ExactOrdered,
            copy,
            equals,
            canBeNull);
    }

    public override string PropertyName => _accessor.PropertyName;

    public override List<TGetter>? GetValue(IMajorRecordGetter record) => _accessor.GetValue(record);

    public override void SetValue(IMajorRecord record, List<TGetter>? value) => _accessor.SetValue(record, value);

    public override bool AreValuesEqual(List<TGetter>? value1, List<TGetter>? value2) =>
        _accessor.AreValuesEqual(value1, value2);

    public override string FormatValue(object? value) => _accessor.FormatValue(value);
}
