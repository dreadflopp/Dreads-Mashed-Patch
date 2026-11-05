using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.EncounterZone;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // DATADataTypeState controls serializer layout and is not an independently forwarded field.
    public class EncounterZoneRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Ownership", new OwnershipPropertyHandler<IEncounterZone, IEncounterZoneGetter, byte>("Rank") },
            { "Location", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, IEncounterZone, IEncounterZoneGetter>("Location") },
            { "MinLevel", new SimpleReflectionPropertyHandler<byte, IEncounterZone, IEncounterZoneGetter>("MinLevel") },
            { "MaxLevel", new SimpleReflectionPropertyHandler<byte, IEncounterZone, IEncounterZoneGetter>("MaxLevel") },
            { "Flags", new FlagsHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IEncounterZoneGetter encounterZoneRecord)
            {
                throw new InvalidOperationException($"Expected IEncounterZoneGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = encounterZoneRecord
                .ToLink<IEncounterZoneGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IEncounterZone, IEncounterZoneGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
