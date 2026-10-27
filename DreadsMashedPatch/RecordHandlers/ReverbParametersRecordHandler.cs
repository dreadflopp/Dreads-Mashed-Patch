using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.ReverbParameters;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// The packed DATA subrecord is one acoustically coupled preset, including its opaque byte.
public class ReverbParametersRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "ReverbData", new ReverbDataHandler() },
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IReverbParametersGetter reverbParametersRecord)
        {
            throw new InvalidOperationException($"Expected IReverbParametersGetter but got {winningContext.Record.GetType()}");
        }

        return reverbParametersRecord
            .ToLink<IReverbParametersGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IReverbParameters, IReverbParametersGetter>(state.LinkCache)
            .ToArray();
    }
}
