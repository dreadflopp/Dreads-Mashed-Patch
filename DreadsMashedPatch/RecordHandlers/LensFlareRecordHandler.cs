using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.LensFlare;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Migration note, LensFlare:
// - Generalized: EditorID and header bits retain the approved shared handlers.
// - Specialized: Global flare parameters and ordered sprite definitions share one owner; metadata/header flags remain independent.
// - Intentionally non-migrated: no gameplay fields omitted; nested data is not independently merged.
// - Reason: selecting complete authored definitions avoids mixed visual presets.
// - Cleanup: new route with one aggregate registration; no parallel scalar implementation.
public class LensFlareRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "FlareDefinition", new FlareDefinitionHandler() },
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not ILensFlareGetter lensFlareRecord)
        {
            throw new InvalidOperationException($"Expected ILensFlareGetter but got {winningContext.Record.GetType()}");
        }

        return lensFlareRecord
            .ToLink<ILensFlareGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter>(state.LinkCache)
            .ToArray();
    }
}
