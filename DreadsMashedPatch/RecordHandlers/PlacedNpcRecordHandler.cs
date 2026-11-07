using DreadsMashedPatch.AssetForwarding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using DreadsMashedPatch.PropertyHandlers.PlacedNpc;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Interior placement, Scale and enable state retain independent selection owners.
    // LocationReference is selected within the accepted parent cell.
    public class PlacedNpcRecordHandler : AbstractRecordHandler
    {
        protected override void InitializePropertyContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> originalContext,
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext)
        {
            base.InitializePropertyContexts(originalContext, winningContext);
            AssetForwardingPolicy.ResolveMemberContexts(GetType(), PropertyHandlers, PropertyContexts);
            foreach (var key in InteriorReferencePlacement.SelectedProperties) PropertyContexts[key].IsResolved = true;
        }

        protected override PropertyForwardingCoordination CoordinateForwardedProperties(
            IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var assets = AssetForwardingPolicy.Coordinate(GetType(), PropertyHandlers, recordContexts, state,
                AtomicOwnershipTriggerProperties);
            return InteriorReferencePlacement.Coordinate(recordContexts, state, PropertyHandlers, PropertyContexts, assets);
        }

        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(PlacedNpc.MajorFlag)) },
            { "Base", new SimpleReflectionFormLinkPropertyHandler<INpcGetter, IPlacedNpc, IPlacedNpcGetter>("Base") },
            { "EncounterZone", new SimpleReflectionFormLinkPropertyHandler<IEncounterZoneGetter, IPlacedNpc, IPlacedNpcGetter>("EncounterZone") },
            { "RagdollPose", new RagdollPosePropertyHandler<IPlacedNpc, IPlacedNpcGetter>() },
            { "Patrol", new GeneratedCopyReflectionPropertyHandler<IPatrolGetter, Patrol, IPlacedNpc, IPlacedNpcGetter>(
                "Patrol", value => value.DeepCopy(), PatrolMixIn.Equals) },
            { "LevelModifier", new SimpleReflectionPropertyHandler<Level?, IPlacedNpc, IPlacedNpcGetter>("LevelModifier") },
            { "MerchantContainer", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IPlacedNpc, IPlacedNpcGetter>("MerchantContainer") },
            { "Count", new SimpleReflectionPropertyHandler<int?, IPlacedNpc, IPlacedNpcGetter>("Count") },
            { "Radius", new SimpleReflectionPropertyHandler<float?, IPlacedNpc, IPlacedNpcGetter>("Radius") },
            { "Health", new SimpleReflectionPropertyHandler<float?, IPlacedNpc, IPlacedNpcGetter>("Health") },
            { "LinkedReferences", new GeneratedCopyReflectionListPropertyHandler<ILinkedReferencesGetter, LinkedReferences, IPlacedNpc, IPlacedNpcGetter>(
                "LinkedReferences", ListSemantics.SortedKeyed, value => value.DeepCopy(), LinkedReferencesMixIn.Equals,
                keySelector: entry => entry.KeywordOrReference.FormKey) },
            { "ActivateParents", new GeneratedCopyReflectionPropertyHandler<IActivateParentsGetter, ActivateParents, IPlacedNpc, IPlacedNpcGetter>("ActivateParents", value => value.DeepCopy(), ActivateParentsMixIn.Equals) },
            { "LinkedReferenceColor", new GeneratedCopyReflectionPropertyHandler<ILinkedReferenceColorGetter, LinkedReferenceColor, IPlacedNpc, IPlacedNpcGetter>(
                "LinkedReferenceColor", value => value.DeepCopy(), LinkedReferenceColorMixIn.Equals) },
            { "PersistentLocation", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, IPlacedNpc, IPlacedNpcGetter>("PersistentLocation") },
            { "LocationReference", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, IPlacedNpc, IPlacedNpcGetter>("LocationReference") },
            { "IsIgnoredBySandbox", new SimpleReflectionPropertyHandler<bool, IPlacedNpc, IPlacedNpcGetter>("IsIgnoredBySandbox") },
            { "LocationRefTypes", new SimpleReflectionListPropertyHandler<IFormLinkGetter<ILocationReferenceTypeGetter>, IPlacedNpc, IPlacedNpcGetter>("LocationRefTypes", ListSemantics.AlignedOrdered, canBeNull: true) },
            { "HeadTrackingWeight", new SimpleReflectionPropertyHandler<float?, IPlacedNpc, IPlacedNpcGetter>("HeadTrackingWeight") },
            { "Horse", new SimpleReflectionFormLinkPropertyHandler<IPlacedNpcGetter, IPlacedNpc, IPlacedNpcGetter>("Horse") },
            { "FavorCost", new SimpleReflectionPropertyHandler<float?, IPlacedNpc, IPlacedNpcGetter>("FavorCost") },
            { "EnableParent", new GeneratedCopyReflectionPropertyHandler<IEnableParentGetter, EnableParent, IPlacedNpc, IPlacedNpcGetter>(
                "EnableParent", value => value.DeepCopy(), EnableParentMixIn.Equals) },
            { "Ownership", new OwnershipPropertyHandler<IPlacedNpc, IPlacedNpcGetter, int?>() },
            { "Emittance", new SimpleReflectionFormLinkPropertyHandler<IEmittanceGetter, IPlacedNpc, IPlacedNpcGetter>("Emittance") },
            { "MultiBoundReference", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IPlacedNpc, IPlacedNpcGetter>("MultiBoundReference") },
            { "IsIgnoredBySandbox2", new SimpleReflectionPropertyHandler<bool, IPlacedNpc, IPlacedNpcGetter>("IsIgnoredBySandbox2") },
            { "Placement", new PlacementPropertyHandler<IPlacedNpc, IPlacedNpcGetter>() },
            { "Scale", new SimpleReflectionPropertyHandler<float?, IPlacedNpc, IPlacedNpcGetter>("Scale") },
            { "VirtualMachineAdapter", new VirtualMachineAdapterHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IPlacedNpcGetter placedNpcRecord)
            {
                throw new InvalidOperationException($"Expected IPlacedNpcGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = placedNpcRecord
                .ToLink<IPlacedNpcGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IPlacedNpc, IPlacedNpcGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
