using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;
public class RelationshipRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Relationship.MajorFlag)) },
        { "Parent", new SimpleReflectionFormLinkPropertyHandler<INpcGetter, IRelationship, IRelationshipGetter>("Parent") },
        { "Child", new SimpleReflectionFormLinkPropertyHandler<INpcGetter, IRelationship, IRelationshipGetter>("Child") },
        { "Rank", new SimpleReflectionPropertyHandler<Relationship.RankType, IRelationship, IRelationshipGetter>("Rank") },
        { "Flags", new SimpleReflectionFlagPropertyHandler<Relationship.Flag, IRelationship, IRelationshipGetter>("Flags") },
        { "AssociationType", new SimpleReflectionFormLinkPropertyHandler<IAssociationTypeGetter, IRelationship, IRelationshipGetter>("AssociationType") },

    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IRelationshipGetter relationshipRecord)
        {
            throw new InvalidOperationException($"Expected IRelationshipGetter but got {winningContext.Record.GetType()}");
        }

        return relationshipRecord
            .ToLink<IRelationshipGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IRelationship, IRelationshipGetter>(state.LinkCache)
            .ToArray();
    }
}
