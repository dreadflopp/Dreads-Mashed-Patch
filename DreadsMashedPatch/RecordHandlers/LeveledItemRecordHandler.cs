using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Noggog;
using DreadsMashedPatch.PropertyHandlers.LeveledItem;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // OwnerTarget generated copies preserve the owner union, including UntypedOwner.
    public class LeveledItemRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "ObjectBounds", new ObjectBoundsHandler() },
            { "ChanceNone", new SimpleReflectionPropertyHandler<Percent, ILeveledItem, ILeveledItemGetter>("ChanceNone") },
            { "Flags", new FlagsHandler() },
            { "Global", new SimpleReflectionFormLinkPropertyHandler<IGlobalGetter, ILeveledItem, ILeveledItemGetter>("Global") },
            { "Entries", new EntriesHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ILeveledItemGetter leveledItemRecord)
            {
                throw new InvalidOperationException($"Expected ILeveledItemGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = leveledItemRecord
                .ToLink<ILeveledItemGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ILeveledItem, ILeveledItemGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
