using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: IsFamily via a reflection handler.
    // - Kept specialized: ParentTitle and Title use typed gendered-string handling.
    // - Rationale: IGenderedItem is enumerable but represents fixed male/female slots, not a list.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    public class AssociationTypeRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "ParentTitle", new GenderedItemHandler<string?, string?, IAssociationType, IAssociationTypeGetter>("ParentTitle", record => record.ParentTitle, (record, value) => record.ParentTitle = value, value => value, (left, right) => StringComparisonHelper.EqualsNormalized(left, right)) },
            { "Title", new GenderedItemHandler<string?, string?, IAssociationType, IAssociationTypeGetter>("Title", record => record.Title, (record, value) => record.Title = value, value => value, (left, right) => StringComparisonHelper.EqualsNormalized(left, right)) },
            { "IsFamily", new SimpleReflectionPropertyHandler<bool?, IAssociationType, IAssociationTypeGetter>("IsFamily") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IAssociationTypeGetter associationType)
            {
                throw new InvalidOperationException($"Expected IAssociationTypeGetter but got {winningContext.Record.GetType()}");
            }

            return associationType
                .ToLink<IAssociationTypeGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IAssociationType, IAssociationTypeGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
