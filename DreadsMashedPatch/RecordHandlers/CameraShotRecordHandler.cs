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
    public class CameraShotRecordHandler : AbstractRecordHandler
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
            { "Model", new ModelHandler() },
            { "Action", new SimpleReflectionPropertyHandler<Mutagen.Bethesda.Skyrim.CameraShot.ActionType, ICameraShot, ICameraShotGetter>("Action") },
            { "Location", new SimpleReflectionPropertyHandler<Mutagen.Bethesda.Skyrim.CameraShot.LocationType, ICameraShot, ICameraShotGetter>("Location") },
            { "Target", new SimpleReflectionPropertyHandler<Mutagen.Bethesda.Skyrim.CameraShot.LocationType, ICameraShot, ICameraShotGetter>("Target") },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.CameraShot.Flag, ICameraShot, ICameraShotGetter>("Flags", preserveUnknownBits: true) },
            { "TimeMultiplierPlayer", new SimpleReflectionPropertyHandler<float, ICameraShot, ICameraShotGetter>("TimeMultiplierPlayer") },
            { "TimeMultiplierTarget", new SimpleReflectionPropertyHandler<float, ICameraShot, ICameraShotGetter>("TimeMultiplierTarget") },
            { "TimeMultiplierGlobal", new SimpleReflectionPropertyHandler<float, ICameraShot, ICameraShotGetter>("TimeMultiplierGlobal") },
            { "MaxTime", new SimpleReflectionPropertyHandler<float, ICameraShot, ICameraShotGetter>("MaxTime") },
            { "MinTime", new SimpleReflectionPropertyHandler<float, ICameraShot, ICameraShotGetter>("MinTime") },
            { "TargetPercentBetweenActors", new SimpleReflectionPropertyHandler<float, ICameraShot, ICameraShotGetter>("TargetPercentBetweenActors") },
            { "NearTargetDistance", new SimpleReflectionPropertyHandler<float, ICameraShot, ICameraShotGetter>("NearTargetDistance") },
            { "ImageSpaceModifier", new SimpleReflectionFormLinkPropertyHandler<IImageSpaceAdapterGetter, ICameraShot, ICameraShotGetter>("ImageSpaceModifier") },
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ICameraShotGetter cameraShot)
            {
                throw new InvalidOperationException($"Expected ICameraShotGetter but got {winningContext.Record.GetType()}");
            }

            return cameraShot
                .ToLink<ICameraShotGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ICameraShot, ICameraShotGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
