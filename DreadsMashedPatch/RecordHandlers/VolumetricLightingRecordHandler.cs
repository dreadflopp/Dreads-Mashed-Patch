using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.VolumetricLighting;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// All twelve nullable parameters form one authored lighting preset; do not mix their sources.
public class VolumetricLightingRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "LightingPreset", new LightingPresetHandler() },
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IVolumetricLightingGetter volumetricLightingRecord)
        {
            throw new InvalidOperationException($"Expected IVolumetricLightingGetter but got {winningContext.Record.GetType()}");
        }

        return volumetricLightingRecord
            .ToLink<IVolumetricLightingGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IVolumetricLighting, IVolumetricLightingGetter>(state.LinkCache)
            .ToArray();
    }
}
