using System;
using Noggog;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Landscape;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: flags/form links use typed handlers; height-map and layer values use generated copy/equality.
    // - Kept specialized: vertex normal/color arrays.
    // - Rationale: overlays require generated mutable layer copies and typed mutable Array2d copies.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    public class LandscapeRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Flags", new SimpleReflectionPropertyHandler<Mutagen.Bethesda.Skyrim.Landscape.Flag?, ILandscape, ILandscapeGetter>("Flags") },
            { "VertexNormals", new LandscapeArray2dHandler(vertexNormals: true) },
            { "VertexHeightMap", new GeneratedCopyReflectionPropertyHandler<ILandscapeVertexHeightMapGetter, LandscapeVertexHeightMap, ILandscape, ILandscapeGetter>(
                "VertexHeightMap", value => value.DeepCopy(), LandscapeVertexHeightMapMixIn.Equals) },
            { "VertexColors", new LandscapeArray2dHandler(vertexNormals: false) },
            { "Layers", new GeneratedCopyReflectionListPropertyHandler<IBaseLayerGetter, BaseLayer, ILandscape, ILandscapeGetter>(
                "Layers", ListSemantics.Unordered, value => value.DeepCopy(), BaseLayerMixIn.Equals) },
            { "Textures", new SimpleReflectionListPropertyHandler<IFormLinkGetter<ILandscapeTextureGetter>, ILandscape, ILandscapeGetter>("Textures", ListSemantics.Unordered, canBeNull: true) }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ILandscapeGetter landscapeRecord)
            {
                throw new InvalidOperationException($"Expected ILandscapeGetter but got {winningContext.Record.GetType()}");
            }

            return landscapeRecord
                .ToLink<ILandscapeGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ILandscape, ILandscapeGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
