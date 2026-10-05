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

// Migration note:
// - Generalized: SOUN marker fields via existing bounds/binary/form-link handlers.
// - Kept specialized: none.
// - Rationale: ISoundMarker has a compact field surface that maps cleanly to generic handlers.

// Header migration: raw/common flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
public class SoundMarkerRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "ObjectBounds", new ObjectBoundsHandler() },
        { "FNAM", new SimpleReflectionBinaryDataPropertyHandler<ISoundMarker, ISoundMarkerGetter>("FNAM") },
        { "SNDD", new SimpleReflectionBinaryDataPropertyHandler<ISoundMarker, ISoundMarkerGetter>("SNDD") },
        { "SoundDescriptor", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, ISoundMarker, ISoundMarkerGetter>("SoundDescriptor") }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not ISoundMarkerGetter soundMarkerRecord)
        {
            throw new InvalidOperationException($"Expected ISoundMarkerGetter but got {winningContext.Record.GetType()}");
        }

        return soundMarkerRecord
            .ToLink<ISoundMarkerGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ISoundMarker, ISoundMarkerGetter>(state.LinkCache)
            .ToArray();
    }
}