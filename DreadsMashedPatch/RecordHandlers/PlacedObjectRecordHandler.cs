using DreadsMashedPatch.AssetForwarding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.PlacedObject;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;
using Noggog;

namespace DreadsMashedPatch.RecordHandlers
{
    // Header flags, Transform and EnableParent retain independent selection owners.
    // LocationReference stays independent of transform and requires ownership for removal.
    // LinkedReferences is positional: xEdit declares an unsorted wbRArray without a StructSK key.
    // Unknown is outside the semantic conflict surface.
    public class PlacedObjectRecordHandler : AbstractRecordHandler
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
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            return AssetForwardingPolicy.Coordinate(GetType(), PropertyHandlers, recordContexts, state,
                AtomicOwnershipTriggerProperties);
        }

        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Base", new SimpleReflectionFormLinkPropertyHandler<IPlaceableObjectGetter, IPlacedObject, IPlacedObjectGetter>("Base") },
            { "Ownership", new OwnershipPropertyHandler<IPlacedObject, IPlacedObjectGetter, int?>() },
            { "LocationReference", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, IPlacedObject, IPlacedObjectGetter>("LocationReference") },
            { "LinkedReferences", new GeneratedCopyReflectionListPropertyHandler<ILinkedReferencesGetter, LinkedReferences, IPlacedObject, IPlacedObjectGetter>(
                "LinkedReferences", ListSemantics.ExactOrdered, value => value.DeepCopy(), LinkedReferencesMixIn.Equals) },
            { "LinkedRooms", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IPlacedObjectGetter>, IPlacedObject, IPlacedObjectGetter>("LinkedRooms", ListSemantics.SortedKeyed) },
            { "ImageSpace", new SimpleReflectionFormLinkPropertyHandler<IImageSpaceGetter, IPlacedObject, IPlacedObjectGetter>("ImageSpace") },
            { "LightingTemplate", new SimpleReflectionFormLinkPropertyHandler<ILightingTemplateGetter, IPlacedObject, IPlacedObjectGetter>("LightingTemplate") },
            { "BoundHalfExtents", new SimpleReflectionPropertyHandler<P3Float?, IPlacedObject, IPlacedObjectGetter>("BoundHalfExtents") },
            { "Primitive", new GeneratedCopyReflectionPropertyHandler<IPlacedPrimitiveGetter, PlacedPrimitive, IPlacedObject, IPlacedObjectGetter>(
                "Primitive", value => value.DeepCopy(), PlacedPrimitiveMixIn.Equals) },
            { "OcclusionPlane", new GeneratedCopyReflectionPropertyHandler<IBoundingGetter, Bounding, IPlacedObject, IPlacedObjectGetter>(
                "OcclusionPlane", value => value.DeepCopy(), BoundingMixIn.Equals) },
            { "Portals", new AtomicGeneratedCopyReflectionListPropertyHandler<IPortalGetter, Portal, IPlacedObject, IPlacedObjectGetter>(
                "Portals", value => value.DeepCopy(), PortalMixIn.Equals) },
            { "RoomPortal", new GeneratedCopyReflectionPropertyHandler<IBoundingGetter, Bounding, IPlacedObject, IPlacedObjectGetter>(
                "RoomPortal", value => value.DeepCopy(), BoundingMixIn.Equals) },
            { "Radius", new SimpleReflectionPropertyHandler<float?, IPlacedObject, IPlacedObjectGetter>("Radius") },
            { "Reflections", new GeneratedCopyReflectionListPropertyHandler<IWaterReflectionGetter, WaterReflection, IPlacedObject, IPlacedObjectGetter>(
                "Reflections", ListSemantics.SortedKeyed, value => value.DeepCopy(), WaterReflectionMixIn.Equals,
                keySelector: entry => entry.Water.FormKey) },
            { "LitWater", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IPlacedObjectGetter>, IPlacedObject, IPlacedObjectGetter>("LitWater", ListSemantics.SortedKeyed) },
            { "Emittance", new SimpleReflectionFormLinkPropertyHandler<IEmittanceGetter, IPlacedObject, IPlacedObjectGetter>("Emittance") },
            { "TeleportMessageBox", new SimpleReflectionFormLinkPropertyHandler<IMessageGetter, IPlacedObject, IPlacedObjectGetter>("TeleportMessageBox") },
            { "MultiBoundReference", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IPlacedObject, IPlacedObjectGetter>("MultiBoundReference") },
            { "SpawnContainer", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IPlacedObject, IPlacedObjectGetter>("SpawnContainer") },
            { "LeveledItemBaseObject", new SimpleReflectionFormLinkPropertyHandler<ILeveledItemGetter, IPlacedObject, IPlacedObjectGetter>("LeveledItemBaseObject") },
            { "PersistentLocation", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, IPlacedObject, IPlacedObjectGetter>("PersistentLocation") },
            { "EncounterZone", new SimpleReflectionFormLinkPropertyHandler<IEncounterZoneGetter, IPlacedObject, IPlacedObjectGetter>("EncounterZone") },
            { "NavigationDoorLink", new GeneratedCopyReflectionPropertyHandler<INavigationDoorLinkGetter, NavigationDoorLink, IPlacedObject, IPlacedObjectGetter>(
                "NavigationDoorLink", value => value.DeepCopy(), NavigationDoorLinkMixIn.Equals) },
            { "LocationRefTypes", new SimpleReflectionListPropertyHandler<IFormLinkGetter<ILocationReferenceTypeGetter>, IPlacedObject, IPlacedObjectGetter>("LocationRefTypes", ListSemantics.AlignedOrdered, canBeNull: true) },
            { "IsMultiBoundPrimitive", new SimpleReflectionPropertyHandler<bool, IPlacedObject, IPlacedObjectGetter>("IsMultiBoundPrimitive") },
            { "IsIgnoredBySandbox", new SimpleReflectionPropertyHandler<bool, IPlacedObject, IPlacedObjectGetter>("IsIgnoredBySandbox") },
            { "IsOpenByDefault", new SimpleReflectionPropertyHandler<bool, IPlacedObject, IPlacedObjectGetter>("IsOpenByDefault") },
            { "ItemCount", new SimpleReflectionPropertyHandler<int?, IPlacedObject, IPlacedObjectGetter>("ItemCount") },
            { "Charge", new SimpleReflectionPropertyHandler<float?, IPlacedObject, IPlacedObjectGetter>("Charge") },
            { "HeadTrackingWeight", new SimpleReflectionPropertyHandler<float?, IPlacedObject, IPlacedObjectGetter>("HeadTrackingWeight") },
            { "FavorCost", new SimpleReflectionPropertyHandler<float?, IPlacedObject, IPlacedObjectGetter>("FavorCost") },
            { "CollisionLayer", new SimpleReflectionPropertyHandler<uint?, IPlacedObject, IPlacedObjectGetter>("CollisionLayer") },
            { "LevelModifier", new SimpleReflectionPropertyHandler<Level?, IPlacedObject, IPlacedObjectGetter>("LevelModifier") },
            { "TeleportDestination", new GeneratedCopyReflectionPropertyHandler<ITeleportDestinationGetter, TeleportDestination, IPlacedObject, IPlacedObjectGetter>(
                "TeleportDestination", value => value.DeepCopy(), TeleportDestinationMixIn.Equals) },
            { "ActivateParents", new GeneratedCopyReflectionPropertyHandler<IActivateParentsGetter, ActivateParents, IPlacedObject, IPlacedObjectGetter>("ActivateParents", value => value.DeepCopy(), ActivateParentsMixIn.Equals) },
            { "Lock", new GeneratedCopyReflectionPropertyHandler<ILockDataGetter, LockData, IPlacedObject, IPlacedObjectGetter>(
                "Lock", value => value.DeepCopy(), LockDataMixIn.Equals) },
            { "AttachRef", new SimpleReflectionFormLinkPropertyHandler<IPlacedThingGetter, IPlacedObject, IPlacedObjectGetter>("AttachRef") },
            { "Action", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.PlacedObject.ActionFlag, IPlacedObject, IPlacedObjectGetter>("Action") },
            { "LightData", new LightDataHandler() },
            { "Alpha", new GeneratedCopyReflectionPropertyHandler<IAlphaGetter, Alpha, IPlacedObject, IPlacedObjectGetter>(
                "Alpha", value => value.DeepCopy(), AlphaMixIn.Equals) },
            { "Patrol", new GeneratedCopyReflectionPropertyHandler<IPatrolGetter, Patrol, IPlacedObject, IPlacedObjectGetter>(
                "Patrol", value => value.DeepCopy(), PatrolMixIn.Equals) },
            { "MapMarker", new GeneratedCopyReflectionPropertyHandler<IMapMarkerGetter, MapMarker, IPlacedObject, IPlacedObjectGetter>(
                "MapMarker", value => value.DeepCopy(), MapMarkerMixIn.Equals) },
            { "Transform", new TransformPropertyHandler<IPlacedObject, IPlacedObjectGetter>() },
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IPlacedObject, IPlacedObjectGetter>() },
            { "EnableParent", new GeneratedCopyReflectionPropertyHandler<IEnableParentGetter, EnableParent, IPlacedObject, IPlacedObjectGetter>(
                "EnableParent", value => value.DeepCopy(), EnableParentMixIn.Equals) },
            { "WaterVelocity", new GeneratedCopyReflectionPropertyHandler<IWaterVelocityGetter, WaterVelocity, IPlacedObject, IPlacedObjectGetter>(
                "WaterVelocity", value => value.DeepCopy(), WaterVelocityMixIn.Equals) },
            { "XCZR", new SimpleReflectionFormLinkPropertyHandler<ILinkedReferenceGetter, IPlacedObject, IPlacedObjectGetter>("XCZR") },
            { "XCZC", new SimpleReflectionFormLinkPropertyHandler<ICellGetter, IPlacedObject, IPlacedObjectGetter>("XCZC") },
            { "XORD", new SimpleReflectionBinaryDataPropertyHandler<IPlacedObject, IPlacedObjectGetter>("XORD") },
            { "RagdollPose", new RagdollPosePropertyHandler<IPlacedObject, IPlacedObjectGetter>() },
            { "XWCN", new SimpleReflectionBinaryDataPropertyHandler<IPlacedObject, IPlacedObjectGetter>("XWCN") },
            { "XWCS", new SimpleReflectionBinaryDataPropertyHandler<IPlacedObject, IPlacedObjectGetter>("XWCS") },
            { "XCVL", new SimpleReflectionBinaryDataPropertyHandler<IPlacedObject, IPlacedObjectGetter>("XCVL") },
            { "XCZA", new SimpleReflectionBinaryDataPropertyHandler<IPlacedObject, IPlacedObjectGetter>("XCZA") },
            { "DistantLodData", new SimpleReflectionBinaryDataPropertyHandler<IPlacedObject, IPlacedObjectGetter>("DistantLodData") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IPlacedObjectGetter placedObjectRecord)
            {
                throw new InvalidOperationException($"Expected IPlacedObjectGetter but got {winningContext.Record.GetType()}");
            }
            return placedObjectRecord
                .ToLink<IPlacedObjectGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IPlacedObject, IPlacedObjectGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
