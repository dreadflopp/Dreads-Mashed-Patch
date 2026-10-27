using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.ImpactDataSet;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Material is the PNAM row key. Matching by the complete pair would duplicate a material
    // instead of replacing its Impact value.
    public class ImpactDataSetRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Impacts", new ImpactsHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IImpactDataSetGetter impactDataSet)
            {
                throw new InvalidOperationException($"Expected IImpactDataSetGetter but got {winningContext.Record.GetType()}");
            }

            return impactDataSet
                .ToLink<IImpactDataSetGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IImpactDataSet, IImpactDataSetGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
