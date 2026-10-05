using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Strings;

namespace DreadsMashedPatch.PropertyHandlers.General;

/// <summary>
/// Copies localized strings through Mutagen's generated copy contract. This preserves
/// every available language instead of rebuilding an English-only value via reflection.
/// </summary>
public sealed class TranslatedStringReflectionPropertyHandler<TRecord, TRecordGetter>
    : GeneratedCopyReflectionPropertyHandler<ITranslatedStringGetter, TranslatedString, TRecord, TRecordGetter>
    where TRecord : class, IMajorRecord
    where TRecordGetter : class, IMajorRecordGetter
{
    private readonly bool _required;

    public TranslatedStringReflectionPropertyHandler(string propertyName, bool required = false)
        : base(propertyName, value => value.DeepCopy(), AreTranslatedStringsEqual)
    {
        _required = required;
    }

    // Required names retain the former null-to-empty behavior without reducing
    // non-null names to their default-language string.
    public override void SetValue(IMajorRecord record, ITranslatedStringGetter? value)
        => base.SetValue(record, value ?? (_required
            ? new TranslatedString(TranslatedString.DefaultLanguage, string.Empty)
            : null));

    private static bool AreTranslatedStringsEqual(
        ITranslatedStringGetter left,
        ITranslatedStringGetter right)
        => (TranslatedString.DefaultLanguageComparisonOnly
                ? TranslatedString.OnlyDefaultComparer
                : TranslatedString.AllLanguageComparer)
            .Equals(left, right);
}
