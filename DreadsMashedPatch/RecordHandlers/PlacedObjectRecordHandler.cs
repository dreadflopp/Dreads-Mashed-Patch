using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using DreadsMashedPatch;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.PlacedObject;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;
using Noggog;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: placed-object links use typed handlers; nested aggregates, portals, reflections, and linked-reference rows use Mutagen-generated copies.
    //   XLRL/LocationReference intentionally stays on this conflict-aware path: a newly added value that
    //   survives into the winner already produces no patch. A later omission may remove it only when that
    //   mod has the adding mod as an actual or configured virtual master; otherwise the addition is retained.
    // - Specialized: Placement is one cohesive value with xEdit-precision position equality, circular normalized-angle
    //   equality, and degree diagnostics. A recognized safe UDR keeps Initially Disabled, Placement, and EnableParent
    //   on the snapshot selected by the approved flag handler. REFR LinkedReferences preserves exact positional order
    //   because Skyrim xEdit defines it as an unsorted wbRArray with no StructSK key. Placement and ActivateParents retain generated copying.
    // - Intentionally non-migrated: ordinary Initially Disabled references are not treated as UDRs, and
    //   LocationReference remains independently conflict-resolved because it is not part of the safe-disable bundle.
    // - Nullable aggregates: list presence is inferred from Mutagen metadata, and VMAD preserves absent versus present-empty state.
    // - Intentionally excluded: Unknown is outside the semantic conflict surface.
    // - Removed: duplicate Placement.Position and Placement.Rotation registrations; the cohesive Placement handler is the sole path.
    // - Rationale: PlacementBinaryOverlay has no useful ToString(), generated exact float equality reports changes
    //   below xEdit-visible precision, and independent UDR fields can otherwise produce contradictory hybrid states.
    public class PlacedObjectRecordHandler : AbstractRecordHandler
    {
        protected override PropertyForwardingCoordination CoordinateForwardedProperties(
            IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts)
        {
            return PlacedReferenceUdrCoordinator.Coordinate(recordContexts, PropertyContexts);
        }

        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler() },
            { "SkyrimMajorRecordFlags", new SkyrimMajorRecordFlagsHandler() },
            { "Base", new SimpleReflectionFormLinkPropertyHandler<IPlaceableObjectGetter, IPlacedObject, IPlacedObjectGetter>("Base") },
            { "Owner", new SimpleReflectionFormLinkPropertyHandler<IOwnerGetter, IPlacedObject, IPlacedObjectGetter>("Owner") },
            { "Scale", new SimpleReflectionPropertyHandler<float?, IPlacedObject, IPlacedObjectGetter>("Scale") },
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
            { "FactionRank", new SimpleReflectionPropertyHandler<int?, IPlacedObject, IPlacedObjectGetter>("FactionRank") },
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
            { "Placement", new PlacementHandler() },
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IPlacedObject, IPlacedObjectGetter>() },
            { "EnableParent", new GeneratedCopyReflectionPropertyHandler<IEnableParentGetter, EnableParent, IPlacedObject, IPlacedObjectGetter>(
                "EnableParent", value => value.DeepCopy(), EnableParentMixIn.Equals) },
            { "WaterVelocity", new GeneratedCopyReflectionPropertyHandler<IWaterVelocityGetter, WaterVelocity, IPlacedObject, IPlacedObjectGetter>(
                "WaterVelocity", value => value.DeepCopy(), WaterVelocityMixIn.Equals) },
            { "XCZR", new SimpleReflectionFormLinkPropertyHandler<ILinkedReferenceGetter, IPlacedObject, IPlacedObjectGetter>("XCZR") },
            { "XCZC", new SimpleReflectionFormLinkPropertyHandler<ICellGetter, IPlacedObject, IPlacedObjectGetter>("XCZC") },
            { "XORD", new SimpleReflectionBinaryDataPropertyHandler<IPlacedObject, IPlacedObjectGetter>("XORD") },
            { "RagdollData", new SimpleReflectionBinaryDataPropertyHandler<IPlacedObject, IPlacedObjectGetter>("RagdollData") },
            { "RagdollBipedData", new SimpleReflectionBinaryDataPropertyHandler<IPlacedObject, IPlacedObjectGetter>("RagdollBipedData") },
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

        // GetOverrideRecord and ApplyForwardedProperties are now handled by the base class
        // The base class automatically handles flag property coordination
    }
}
