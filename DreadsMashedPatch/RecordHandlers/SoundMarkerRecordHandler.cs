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