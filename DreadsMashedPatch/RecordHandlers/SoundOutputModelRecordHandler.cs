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

// Generated copies materialize nested overlay attenuation bytes and channels; direct reflection
// assignment cannot safely convert them to mutable values.
public class SoundOutputModelRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Data", new GeneratedCopyReflectionPropertyHandler<ISoundOutputDataGetter, SoundOutputData, ISoundOutputModel, ISoundOutputModelGetter>("Data", value => value.DeepCopy(), SoundOutputDataMixIn.Equals) },
        { "FNAM", new SimpleReflectionBinaryDataPropertyHandler<ISoundOutputModel, ISoundOutputModelGetter>("FNAM") },
        { "Type", new SimpleReflectionPropertyHandler<SoundOutputModel.TypeEnum?, ISoundOutputModel, ISoundOutputModelGetter>("Type") },
        { "CNAM", new SimpleReflectionBinaryDataPropertyHandler<ISoundOutputModel, ISoundOutputModelGetter>("CNAM") },
        { "SNAM", new SimpleReflectionBinaryDataPropertyHandler<ISoundOutputModel, ISoundOutputModelGetter>("SNAM") },
        { "OutputChannels", new GeneratedCopyReflectionPropertyHandler<ISoundOutputChannelsGetter, SoundOutputChannels, ISoundOutputModel, ISoundOutputModelGetter>("OutputChannels", value => value.DeepCopy(), SoundOutputChannelsMixIn.Equals) },
        { "Attenuation", new GeneratedCopyReflectionPropertyHandler<ISoundOutputAttenuationGetter, SoundOutputAttenuation, ISoundOutputModel, ISoundOutputModelGetter>("Attenuation", value => value.DeepCopy(), SoundOutputAttenuationMixIn.Equals) }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not ISoundOutputModelGetter soundOutputModelRecord)
        {
            throw new InvalidOperationException($"Expected ISoundOutputModelGetter but got {winningContext.Record.GetType()}");
        }

        return soundOutputModelRecord
            .ToLink<ISoundOutputModelGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ISoundOutputModel, ISoundOutputModelGetter>(state.LinkCache)
            .ToArray();
    }
}
