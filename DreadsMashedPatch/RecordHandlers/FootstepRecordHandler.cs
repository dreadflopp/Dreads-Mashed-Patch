using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    public class FootstepRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "ImpactDataSet", new SimpleReflectionFormLinkPropertyHandler<IImpactDataSetGetter, IFootstep, IFootstepGetter>("ImpactDataSet") },
            { "Tag", new SimpleReflectionPropertyHandler<string, IFootstep, IFootstepGetter>("Tag") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IFootstepGetter footstep)
            {
                throw new InvalidOperationException($"Expected IFootstepGetter but got {winningContext.Record.GetType()}");
            }

            return footstep
                .ToLink<IFootstepGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IFootstep, IFootstepGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
