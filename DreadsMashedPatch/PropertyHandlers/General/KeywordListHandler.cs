using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Noggog;
using DreadsMashedPatch.PropertyHandlers.Abstracts;

namespace DreadsMashedPatch.PropertyHandlers.General
{
    public class KeywordListHandler : AbstractListPropertyHandler<IFormLinkGetter<IKeywordGetter>>
    {
        public override string PropertyName => "Keywords";
        public override ListSemantics Semantics => ListSemantics.SortedKeyed;
        protected override bool CanBeNull => true;

        protected override IReadOnlyList<object?> GetSortKey(IFormLinkGetter<IKeywordGetter> item) => [item.FormKey];

        public override void SetValue(IMajorRecord record, List<IFormLinkGetter<IKeywordGetter>>? value)
        {
            if (record is IKeyworded<IKeywordGetter> keyworded)
            {
                // Skyrim's keyword aspect is nullable: absence and present-empty
                // have separate ownership. Copy links as well as the collection.
                keyworded.Keywords = value == null ? null
                    : new ExtendedList<IFormLinkGetter<IKeywordGetter>>(value.Select(link =>
                        (IFormLinkGetter<IKeywordGetter>)new FormLink<IKeywordGetter>(link.FormKey)));
            }
            else
            {
                LogCollector.AddError(PropertyName, $"Error: Record does not implement IKeyworded<IKeywordGetter> for {PropertyName}");
            }
        }

        public override List<IFormLinkGetter<IKeywordGetter>>? GetValue(IMajorRecordGetter record)
        {
            if (record is IKeywordedGetter<IKeywordGetter> keyworded)
            {
                return keyworded.Keywords?.ToList();
            }
            else
            {
                LogCollector.AddError(PropertyName, $"Error: Record does not implement IKeywordedGetter<IKeywordGetter> for {PropertyName}");
            }
            return null;
        }

        protected override bool IsItemEqual(IFormLinkGetter<IKeywordGetter>? item1, IFormLinkGetter<IKeywordGetter>? item2)
        {
            if (item1 == null && item2 == null) return true;
            if (item1 == null || item2 == null) return false;
            return item1.FormKey.Equals(item2.FormKey);
        }

        protected override string FormatItem(IFormLinkGetter<IKeywordGetter>? item)
        {
            return item?.FormKey.ToString() ?? "null";
        }
    }
}
