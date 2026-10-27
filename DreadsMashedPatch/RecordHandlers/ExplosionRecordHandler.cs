using DreadsMashedPatch.AssetForwarding;
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
    // DATADataTypeState controls serializer layout and is not an independently forwarded field.
    public class ExplosionRecordHandler : AbstractRecordHandler
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
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IExplosion, IExplosionGetter>() },
            { "Name", new TranslatedStringReflectionPropertyHandler<IExplosion, IExplosionGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "ObjectEffect", new SimpleReflectionFormLinkPropertyHandler<IEffectRecordGetter, IExplosion, IExplosionGetter>("ObjectEffect") },
            { "ImageSpaceModifier", new SimpleReflectionFormLinkPropertyHandler<IImageSpaceAdapterGetter, IExplosion, IExplosionGetter>("ImageSpaceModifier") },
            { "Light", new SimpleReflectionFormLinkPropertyHandler<ILightGetter, IExplosion, IExplosionGetter>("Light") },
            { "Sound1", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IExplosion, IExplosionGetter>("Sound1") },
            { "Sound2", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IExplosion, IExplosionGetter>("Sound2") },
            { "ImpactDataSet", new SimpleReflectionFormLinkPropertyHandler<IImpactDataSetGetter, IExplosion, IExplosionGetter>("ImpactDataSet") },
            { "PlacedObject", new SimpleReflectionFormLinkPropertyHandler<IExplodeSpawnGetter, IExplosion, IExplosionGetter>("PlacedObject") },
            { "SpawnProjectile", new SimpleReflectionFormLinkPropertyHandler<IProjectileGetter, IExplosion, IExplosionGetter>("SpawnProjectile") },
            { "Force", new SimpleReflectionPropertyHandler<float, IExplosion, IExplosionGetter>("Force", 0.001f) },
            { "Damage", new SimpleReflectionPropertyHandler<float, IExplosion, IExplosionGetter>("Damage", 0.001f) },
            { "Radius", new SimpleReflectionPropertyHandler<float, IExplosion, IExplosionGetter>("Radius", 0.001f) },
            { "ISRadius", new SimpleReflectionPropertyHandler<float, IExplosion, IExplosionGetter>("ISRadius", 0.001f) },
            { "VerticalOffsetMult", new SimpleReflectionPropertyHandler<float, IExplosion, IExplosionGetter>("VerticalOffsetMult", 0.001f) },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.Explosion.Flag, IExplosion, IExplosionGetter>("Flags") },
            { "SoundLevel", new SimpleReflectionPropertyHandler<SoundLevel, IExplosion, IExplosionGetter>("SoundLevel") },
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IExplosionGetter explosion)
            {
                throw new InvalidOperationException($"Expected IExplosionGetter but got {winningContext.Record.GetType()}");
            }

            return explosion
                .ToLink<IExplosionGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IExplosion, IExplosionGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
