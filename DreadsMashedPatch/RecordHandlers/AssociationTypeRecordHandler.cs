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
    // IGenderedItem is enumerable but represents fixed male/female slots, not a mergeable list.
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
