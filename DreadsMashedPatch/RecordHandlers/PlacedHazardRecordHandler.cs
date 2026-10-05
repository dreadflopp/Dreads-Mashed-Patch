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

// Migration note:
// - Generalized: all eighteen inherited placed fields now use existing shared handlers.
//   Scalars/links forward individually; generated aggregates and Placement retain whole-value ownership.
// - Kept specialized: VMAD uses the shared script-name/metadata policy; header flags keep the approved composite.
//   xEdit ReferenceRecord sorts LinkedReferences by Keyword/Ref and Reflections by Water; LocationRefTypes align
//   in declaration order. DistantLodData is one complete XLOD vector, not independently merged coordinates.
// - Intentionally non-migrated: UDR coordination remains scoped to ACHR/REFR; no PHZD UDR policy is established.
// - Rationale: inherited interfaces expose writable semantic data; XIS2 retains nullable binary marker/payload
//   handling rather than pretending it is ACHR/REFR's boolean. No old handler path or classes were superseded.

// Header migration: raw/common/APlacedTrap.MajorFlag flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
public class PlacedHazardRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(APlacedTrap.MajorFlag)) },
        { "Hazard", new SimpleReflectionFormLinkPropertyHandler<IHazardGetter, IPlacedHazard, IPlacedHazardGetter>("Hazard") },
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IPlacedHazard, IPlacedHazardGetter>() },
        { "EncounterZone", new SimpleReflectionFormLinkPropertyHandler<IEncounterZoneGetter, IPlacedHazard, IPlacedHazardGetter>("EncounterZone") },
        { "Owner", new SimpleReflectionFormLinkPropertyHandler<IOwnerGetter, IPlacedHazard, IPlacedHazardGetter>("Owner") },
        { "FactionRank", new SimpleReflectionPropertyHandler<int?, IPlacedHazard, IPlacedHazardGetter>("FactionRank") },
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
        { "Scale", new SimpleReflectionPropertyHandler<float?, IPlacedHazard, IPlacedHazardGetter>("Scale") },
        { "Placement", new PlacementPropertyHandler<IPlacedHazard, IPlacedHazardGetter>() }
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
