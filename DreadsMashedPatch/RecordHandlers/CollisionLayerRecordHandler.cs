using System;
using System.Drawing;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.CollisionLayer;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: Description uses generated localized-string copying; scalar fields use typed handlers.
    // - Kept specialized: Name with shared string handler and CollidesWith list handler.
    // - Rationale: list link handling requires explicit concrete copy semantics.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    public class CollisionLayerRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Description", new TranslatedStringReflectionPropertyHandler<ICollisionLayer, ICollisionLayerGetter>("Description") },
            { "Index", new SimpleReflectionPropertyHandler<uint, ICollisionLayer, ICollisionLayerGetter>("Index") },
            { "DebugColor", new SimpleReflectionPropertyHandler<Color, ICollisionLayer, ICollisionLayerGetter>("DebugColor") },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.CollisionLayer.Flag, ICollisionLayer, ICollisionLayerGetter>("Flags") },
            { "Name", new SimpleReflectionPropertyHandler<string, ICollisionLayer, ICollisionLayerGetter>("Name") },
            { "CollidesWith", new CollidesWithHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ICollisionLayerGetter collisionLayer)
            {
                throw new InvalidOperationException($"Expected ICollisionLayerGetter but got {winningContext.Record.GetType()}");
            }

            return collisionLayer
                .ToLink<ICollisionLayerGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ICollisionLayer, ICollisionLayerGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
