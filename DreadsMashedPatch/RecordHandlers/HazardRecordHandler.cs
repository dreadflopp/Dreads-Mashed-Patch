using DreadsMashedPatch.AssetForwarding;
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
    public class HazardRecordHandler : AbstractRecordHandler
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
            { "Name", new TranslatedStringReflectionPropertyHandler<IHazard, IHazardGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "ImageSpaceModifier", new SimpleReflectionFormLinkPropertyHandler<IImageSpaceAdapterGetter, IHazard, IHazardGetter>("ImageSpaceModifier") },
            { "Limit", new SimpleReflectionPropertyHandler<uint, IHazard, IHazardGetter>("Limit") },
            { "Radius", new SimpleReflectionPropertyHandler<float, IHazard, IHazardGetter>("Radius", 0.001f) },
            { "Lifetime", new SimpleReflectionPropertyHandler<float, IHazard, IHazardGetter>("Lifetime", 0.001f) },
            { "ImageSpaceRadius", new SimpleReflectionPropertyHandler<float, IHazard, IHazardGetter>("ImageSpaceRadius", 0.001f) },
            { "TargetInterval", new SimpleReflectionPropertyHandler<float, IHazard, IHazardGetter>("TargetInterval", 0.001f) },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.Hazard.Flag, IHazard, IHazardGetter>("Flags") },
            { "Spell", new SimpleReflectionFormLinkPropertyHandler<IEffectRecordGetter, IHazard, IHazardGetter>("Spell") },
            { "Light", new SimpleReflectionFormLinkPropertyHandler<ILightGetter, IHazard, IHazardGetter>("Light") },
            { "ImpactDataSet", new SimpleReflectionFormLinkPropertyHandler<IImpactDataSetGetter, IHazard, IHazardGetter>("ImpactDataSet") },
            { "Sound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IHazard, IHazardGetter>("Sound") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IHazardGetter hazard)
            {
                throw new InvalidOperationException($"Expected IHazardGetter but got {winningContext.Record.GetType()}");
            }

            return hazard
                .ToLink<IHazardGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IHazard, IHazardGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
