using DreadsMashedPatch.AssetForwarding;
using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using Noggog;

namespace DreadsMashedPatch.RecordHandlers;

// Inherited placed fields follow xEdit ReferenceRecord semantics.
public class PlacedHazardRecordHandler : AbstractRecordHandler
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
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(APlacedTrap.MajorFlag)) },
        { "Hazard", new SimpleReflectionFormLinkPropertyHandler<IHazardGetter, IPlacedHazard, IPlacedHazardGetter>("Hazard") },
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IPlacedHazard, IPlacedHazardGetter>() },
        { "EncounterZone", new SimpleReflectionFormLinkPropertyHandler<IEncounterZoneGetter, IPlacedHazard, IPlacedHazardGetter>("EncounterZone") },
        { "Ownership", new OwnershipPropertyHandler<IPlacedHazard, IPlacedHazardGetter, int?>() },
        { "HeadTrackingWeight", new SimpleReflectionPropertyHandler<float?, IPlacedHazard, IPlacedHazardGetter>("HeadTrackingWeight") },
        { "FavorCost", new SimpleReflectionPropertyHandler<float?, IPlacedHazard, IPlacedHazardGetter>("FavorCost") },
        { "Reflections", new GeneratedCopyReflectionListPropertyHandler<IWaterReflectionGetter, WaterReflection, IPlacedHazard, IPlacedHazardGetter>(
            "Reflections", ListSemantics.SortedKeyed, value => value.DeepCopy(), WaterReflectionMixIn.Equals,
            keySelector: entry => entry.Water.FormKey) },
        { "LinkedReferences", new GeneratedCopyReflectionListPropertyHandler<ILinkedReferencesGetter, LinkedReferences, IPlacedHazard, IPlacedHazardGetter>(
            "LinkedReferences", ListSemantics.SortedKeyed, value => value.DeepCopy(), LinkedReferencesMixIn.Equals,
            keySelector: entry => entry.KeywordOrReference.FormKey) },
        { "ActivateParents", new GeneratedCopyReflectionPropertyHandler<IActivateParentsGetter, ActivateParents, IPlacedHazard, IPlacedHazardGetter>(
            "ActivateParents", value => value.DeepCopy(), ActivateParentsMixIn.Equals) },
        { "EnableParent", new GeneratedCopyReflectionPropertyHandler<IEnableParentGetter, EnableParent, IPlacedHazard, IPlacedHazardGetter>(
            "EnableParent", value => value.DeepCopy(), EnableParentMixIn.Equals) },
        { "Emittance", new SimpleReflectionFormLinkPropertyHandler<IEmittanceGetter, IPlacedHazard, IPlacedHazardGetter>("Emittance") },
        { "MultiBoundReference", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IPlacedHazard, IPlacedHazardGetter>("MultiBoundReference") },
        { "IgnoredBySandbox", new SimpleReflectionBinaryDataPropertyHandler<IPlacedHazard, IPlacedHazardGetter>("IgnoredBySandbox") },
        { "LocationRefTypes", new SimpleReflectionListPropertyHandler<IFormLinkGetter<ILocationReferenceTypeGetter>, IPlacedHazard, IPlacedHazardGetter>(
            "LocationRefTypes", ListSemantics.AlignedOrdered, canBeNull: true) },
        { "LocationReference", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, IPlacedHazard, IPlacedHazardGetter>("LocationReference") },
        { "DistantLodData", new GeneratedCopyReflectionPropertyHandler<IReadOnlyList<float>, ExtendedList<float>, IPlacedHazard, IPlacedHazardGetter>(
            "DistantLodData", value => new ExtendedList<float>(value), (left, right) => left.SequenceEqual(right)) },
        { "Transform", new TransformPropertyHandler<IPlacedHazard, IPlacedHazardGetter>() }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IPlacedHazardGetter placedHazardRecord)
        {
            throw new InvalidOperationException($"Expected IPlacedHazardGetter but got {winningContext.Record.GetType()}");
        }

        return placedHazardRecord
            .ToLink<IPlacedHazardGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IPlacedHazard, IPlacedHazardGetter>(state.LinkCache)
            .ToArray();
    }
}
