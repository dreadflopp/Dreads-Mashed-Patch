using DreadsMashedPatch.AssetForwarding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.EffectShader;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Texture coordinates/atlas indices travel with texture paths; effect tuning has separate ownership.
    // DATADataTypeState controls serializer layout and does not participate in conflict resolution.
    public class EffectShaderRecordHandler : AbstractRecordHandler
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
            { "FillTexture", new FillTextureHandler() },
            { "ParticleShaderTexture", new ParticleShaderTextureHandler() },
            { "HolesTexture", new HolesTextureHandler() },
            { "MembranePaletteTexture", new MembranePaletteTextureHandler() },
            { "ParticlePaletteTexture", new ParticlePaletteTextureHandler() },
            { "EffectShaderData", EffectShaderDataHandlers.Data() },
            { "FillTextureCoordinates", EffectShaderDataHandlers.Fill() },
            { "ParticleAtlas", EffectShaderDataHandlers.Particles() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IEffectShaderGetter effectShaderRecord)
            {
                throw new InvalidOperationException($"Expected IEffectShaderGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = effectShaderRecord
                .ToLink<IEffectShaderGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IEffectShader, IEffectShaderGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
