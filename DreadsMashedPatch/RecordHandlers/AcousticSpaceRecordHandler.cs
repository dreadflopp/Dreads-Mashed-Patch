using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: AmbientSound, UseSoundFromRegion, EnvironmentType via reflection form-link handlers.
    // - Kept specialized: ObjectBounds via shared handler.
    // - Rationale: preserves existing object-bound behavior and keeps remaining fields simple.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    public class AcousticSpaceRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "ObjectBounds", new ObjectBoundsHandler() },
            { "AmbientSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IAcousticSpace, IAcousticSpaceGetter>("AmbientSound") },
            { "UseSoundFromRegion", new SimpleReflectionFormLinkPropertyHandler<IRegionGetter, IAcousticSpace, IAcousticSpaceGetter>("UseSoundFromRegion") },
            { "EnvironmentType", new SimpleReflectionFormLinkPropertyHandler<IReverbParametersGetter, IAcousticSpace, IAcousticSpaceGetter>("EnvironmentType") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IAcousticSpaceGetter acousticSpace)
            {
                throw new InvalidOperationException($"Expected IAcousticSpaceGetter but got {winningContext.Record.GetType()}");
            }

            return acousticSpace
                .ToLink<IAcousticSpaceGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IAcousticSpace, IAcousticSpaceGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
