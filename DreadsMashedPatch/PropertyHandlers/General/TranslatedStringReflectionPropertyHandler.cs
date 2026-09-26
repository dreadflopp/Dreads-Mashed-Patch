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
    public TranslatedStringReflectionPropertyHandler(string propertyName)
        : base(propertyName, value => value.DeepCopy(), AreTranslatedStringsEqual)
    {
    }

    private static bool AreTranslatedStringsEqual(
        ITranslatedStringGetter left,
        ITranslatedStringGetter right)
        => (TranslatedString.DefaultLanguageComparisonOnly
                ? TranslatedString.OnlyDefaultComparer
                : TranslatedString.AllLanguageComparer)
            .Equals(left, right);
}
