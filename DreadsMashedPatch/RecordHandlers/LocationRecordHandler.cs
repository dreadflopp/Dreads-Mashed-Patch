using System;
using System.Drawing;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: flags, names, keywords, links, and scalar values use project-approved shared handlers.
    // - Intentionally non-migrated: Added/Static/Removed bookkeeping collections; conflict resolution is unnecessary for them.
    // - Rationale: only semantic Location values participate in forwarding.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class LocationRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<ILocation, ILocationGetter>("Name") },
            { "Keywords", new KeywordListHandler() },
            { "ParentLocation", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, ILocation, ILocationGetter>("ParentLocation") },
            { "Music", new SimpleReflectionFormLinkPropertyHandler<IMusicTypeGetter, ILocation, ILocationGetter>("Music") },
            { "UnreportedCrimeFaction", new SimpleReflectionFormLinkPropertyHandler<IFactionGetter, ILocation, ILocationGetter>("UnreportedCrimeFaction") },
            { "WorldLocationMarkerRef", new SimpleReflectionFormLinkPropertyHandler<IPlacedSimpleGetter, ILocation, ILocationGetter>("WorldLocationMarkerRef") },
            { "WorldLocationRadius", new SimpleReflectionPropertyHandler<float?, ILocation, ILocationGetter>("WorldLocationRadius") },
            { "HorseMarkerRef", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, ILocation, ILocationGetter>("HorseMarkerRef") },
            { "Color", new SimpleReflectionPropertyHandler<Color?, ILocation, ILocationGetter>("Color") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ILocationGetter locationRecord)
            {
                throw new InvalidOperationException($"Expected ILocationGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = locationRecord
                .ToLink<ILocationGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ILocation, ILocationGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
