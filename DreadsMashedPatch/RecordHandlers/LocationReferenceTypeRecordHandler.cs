using System;
using System.Drawing;
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
    // - Generalized: color and core record metadata via reflection handlers.
    // - Kept specialized: none.
    // - Rationale: the surface is a single nullable color property on a standard major record.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    public class LocationReferenceTypeRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Color", new SimpleReflectionPropertyHandler<Color?, ILocationReferenceType, ILocationReferenceTypeGetter>("Color") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ILocationReferenceTypeGetter locationReferenceType)
            {
                throw new InvalidOperationException($"Expected ILocationReferenceTypeGetter but got {winningContext.Record.GetType()}");
            }

            return locationReferenceType
                .ToLink<ILocationReferenceTypeGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ILocationReferenceType, ILocationReferenceTypeGetter>(state.LinkCache)
                .ToArray();
        }
    }
}