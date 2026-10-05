using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: all footstep lists use the shared atomic form-link-list handler.
    // - Kept specialized: none.
    // - Rationale: each logical list is an ordered value with meaningful duplicates; Mutagen derives XCNT and DATA during serialization.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    public class FootstepSetRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "WalkForwardFootsteps", new AtomicFormLinkListPropertyHandler<IFootstepGetter, IFootstepSet, IFootstepSetGetter>("WalkForwardFootsteps") },
            { "RunForwardFootsteps", new AtomicFormLinkListPropertyHandler<IFootstepGetter, IFootstepSet, IFootstepSetGetter>("RunForwardFootsteps") },
            { "WalkForwardAlternateFootsteps", new AtomicFormLinkListPropertyHandler<IFootstepGetter, IFootstepSet, IFootstepSetGetter>("WalkForwardAlternateFootsteps") },
            { "RunForwardAlternateFootsteps", new AtomicFormLinkListPropertyHandler<IFootstepGetter, IFootstepSet, IFootstepSetGetter>("RunForwardAlternateFootsteps") },
            { "WalkForwardAlternateFootsteps2", new AtomicFormLinkListPropertyHandler<IFootstepGetter, IFootstepSet, IFootstepSetGetter>("WalkForwardAlternateFootsteps2") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IFootstepSetGetter footstepSet)
            {
                throw new InvalidOperationException($"Expected IFootstepSetGetter but got {winningContext.Record.GetType()}");
            }

            return footstepSet
                .ToLink<IFootstepSetGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IFootstepSet, IFootstepSetGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
