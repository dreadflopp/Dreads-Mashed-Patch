using DreadsMashedPatch.AssetForwarding;
using System;
using System.Drawing;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Light;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.RecordHandlers
{
    public class LightRecordHandler : AbstractRecordHandler
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
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Light.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<ILight, ILightGetter>("Name") },
            { "VirtualMachineAdapter", new VirtualMachineAdapterHandler() },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Icons", new IconsHandler() },
            { "Destructible", DestructibleDataHandlers.Data(new DestructibleHandler()) },
            { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new DestructibleHandler()) },
            { "Time", new SimpleReflectionPropertyHandler<int, ILight, ILightGetter>("Time") },
            { "Radius", new SimpleReflectionPropertyHandler<uint, ILight, ILightGetter>("Radius") },
            { "Color", new SimpleReflectionPropertyHandler<Color, ILight, ILightGetter>("Color") },
            { "Flags", new FlagsHandler() },
            { "FalloffExponent", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FalloffExponent") },
            { "FOV", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FOV") },
            { "NearClip", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("NearClip") },
            { "FlickerPeriod", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FlickerPeriod") },
            { "FlickerIntensityAmplitude", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FlickerIntensityAmplitude") },
            { "FlickerMovementAmplitude", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FlickerMovementAmplitude") },
            { "Value", new ValueHandler() },
            { "Weight", new WeightHandler() },
            { "FadeValue", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FadeValue") },
            { "Sound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, ILight, ILightGetter>("Sound") },
            { "Lens", new SimpleReflectionFormLinkPropertyHandler<ILensFlareGetter, ILight, ILightGetter>("Lens") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ILightGetter lightRecord)
            {
                throw new InvalidOperationException($"Expected ILightGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = lightRecord
                .ToLink<ILightGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
