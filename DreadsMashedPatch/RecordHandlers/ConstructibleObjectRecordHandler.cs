using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.ConstructibleObject;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: CreatedObject, WorkbenchKeyword, CreatedObjectCount via reflection handlers.
    // - Kept specialized: Items and Conditions list semantics via dedicated list handlers.
    // - Rationale: uses project-approved list/conditions behavior for complex recipe data.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    public class ConstructibleObjectRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Items", new ItemsHandler() },
            { "Conditions", new ConditionsHandler() },
            { "CreatedObject", new SimpleReflectionFormLinkPropertyHandler<IConstructibleGetter, IConstructibleObject, IConstructibleObjectGetter>("CreatedObject") },
            { "WorkbenchKeyword", new SimpleReflectionFormLinkPropertyHandler<IKeywordGetter, IConstructibleObject, IConstructibleObjectGetter>("WorkbenchKeyword") },
            { "CreatedObjectCount", new SimpleReflectionPropertyHandler<ushort?, IConstructibleObject, IConstructibleObjectGetter>("CreatedObjectCount") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IConstructibleObjectGetter constructibleObject)
            {
                throw new InvalidOperationException($"Expected IConstructibleObjectGetter but got {winningContext.Record.GetType()}");
            }

            return constructibleObject
                .ToLink<IConstructibleObjectGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IConstructibleObject, IConstructibleObjectGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
