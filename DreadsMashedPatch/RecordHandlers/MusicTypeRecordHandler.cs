using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.RecordHandlers;
public class MusicTypeRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Flags", new SimpleReflectionFlagPropertyHandler<MusicType.Flag, IMusicType, IMusicTypeGetter>("Flags") },
        { "Data", new GeneratedCopyReflectionPropertyHandler<IMusicTypeDataGetter, MusicTypeData, IMusicType, IMusicTypeGetter>(
            "Data", value => value.DeepCopy(), MusicTypeDataMixIn.Equals) },
        { "FadeDuration", new SimpleReflectionPropertyHandler<float?, IMusicType, IMusicTypeGetter>("FadeDuration") },
        { "Tracks", new AtomicReflectionListPropertyHandler<IFormLinkGetter<IMusicTrackGetter>, IMusicType, IMusicTypeGetter>("Tracks", canBeNull: true) }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IMusicTypeGetter musicTypeRecord)
        {
            throw new InvalidOperationException($"Expected IMusicTypeGetter but got {winningContext.Record.GetType()}");
        }

        return musicTypeRecord
            .ToLink<IMusicTypeGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IMusicType, IMusicTypeGetter>(state.LinkCache)
            .ToArray();
    }
}
