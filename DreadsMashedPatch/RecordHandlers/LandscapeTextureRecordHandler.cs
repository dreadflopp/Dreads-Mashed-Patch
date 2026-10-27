using DreadsMashedPatch.AssetForwarding;
using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Noggog;

namespace DreadsMashedPatch.RecordHandlers
{
    // HNAM friction/restitution remain independent; GNAM grasses use sorted FormID keys.
    // Nullable INAM snow flags distinguish absent from present-zero.
    public class LandscapeTextureRecordHandler : AbstractRecordHandler
    {
        protected override void InitializePropertyContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> originalContext,
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext)
        {
            base.InitializePropertyContexts(originalContext, winningContext);
            AssetForwardingPolicy.ResolveMemberContexts(GetType(), PropertyHandlers, PropertyContexts);
        }

        protected override PropertyForwardingCoordination CoordinateForwardedProperties(
            IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state) =>
            AssetForwardingPolicy.Coordinate(GetType(), PropertyHandlers, recordContexts, state, AtomicOwnershipTriggerProperties);

        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "TextureSet", new SimpleReflectionFormLinkPropertyHandler<ITextureSetGetter, ILandscapeTexture, ILandscapeTextureGetter>("TextureSet") },
            { "MaterialType", new SimpleReflectionFormLinkPropertyHandler<IMaterialTypeGetter, ILandscapeTexture, ILandscapeTextureGetter>("MaterialType") },
            { "HavokFriction", new SimpleReflectionPropertyHandler<byte, ILandscapeTexture, ILandscapeTextureGetter>("HavokFriction") },
            { "HavokRestitution", new SimpleReflectionPropertyHandler<byte, ILandscapeTexture, ILandscapeTextureGetter>("HavokRestitution") },
            { "TextureSpecularExponent", new SimpleReflectionPropertyHandler<byte, ILandscapeTexture, ILandscapeTextureGetter>("TextureSpecularExponent") },
            { "Grasses", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IGrassGetter>, ILandscapeTexture, ILandscapeTextureGetter>("Grasses", ListSemantics.SortedKeyed) },
            { "Flags", new SimpleReflectionNullableFlagPropertyHandler<Mutagen.Bethesda.Skyrim.LandscapeTexture.Flag, ILandscapeTexture, ILandscapeTextureGetter>("Flags") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ILandscapeTextureGetter landscapeTexture)
            {
                throw new InvalidOperationException($"Expected ILandscapeTextureGetter but got {winningContext.Record.GetType()}");
            }

            return landscapeTexture
                .ToLink<ILandscapeTextureGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ILandscapeTexture, ILandscapeTextureGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
