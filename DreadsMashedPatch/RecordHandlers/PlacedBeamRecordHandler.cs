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

// Migration note, PlacedBeam:
// - Generalized: all eighteen inherited placed fields reuse PHZD's existing handlers and xEdit keys.
// - Specialized: Projectile selects a complete PROJ identity; approved APlacedTrap header flags remain per bit.
// - Intentionally non-migrated: ACHR/REFR UDR coordination; no projectile-specific UDR policy is established.
// - Reason: xEdit uses ReferenceRecord for this signature; complete rows/aggregates preserve authored data.
// - Cleanup: this is a new route, with one registration per field and no superseded implementation.
public class PlacedBeamRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(APlacedTrap.MajorFlag)) },
        { "Projectile", new SimpleReflectionFormLinkPropertyHandler<IProjectileGetter, IPlacedBeam, IPlacedBeamGetter>("Projectile") },
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IPlacedBeam, IPlacedBeamGetter>() },
        { "EncounterZone", new SimpleReflectionFormLinkPropertyHandler<IEncounterZoneGetter, IPlacedBeam, IPlacedBeamGetter>("EncounterZone") },
        { "Owner", new SimpleReflectionFormLinkPropertyHandler<IOwnerGetter, IPlacedBeam, IPlacedBeamGetter>("Owner") },
        { "FactionRank", new SimpleReflectionPropertyHandler<int?, IPlacedBeam, IPlacedBeamGetter>("FactionRank") },
        { "HeadTrackingWeight", new SimpleReflectionPropertyHandler<float?, IPlacedBeam, IPlacedBeamGetter>("HeadTrackingWeight") },
        { "FavorCost", new SimpleReflectionPropertyHandler<float?, IPlacedBeam, IPlacedBeamGetter>("FavorCost") },
        { "Reflections", new GeneratedCopyReflectionListPropertyHandler<IWaterReflectionGetter, WaterReflection, IPlacedBeam, IPlacedBeamGetter>(
            "Reflections", ListSemantics.SortedKeyed, value => value.DeepCopy(), WaterReflectionMixIn.Equals,
            keySelector: entry => entry.Water.FormKey) },
        { "LinkedReferences", new GeneratedCopyReflectionListPropertyHandler<ILinkedReferencesGetter, LinkedReferences, IPlacedBeam, IPlacedBeamGetter>(
            "LinkedReferences", ListSemantics.SortedKeyed, value => value.DeepCopy(), LinkedReferencesMixIn.Equals,
            keySelector: entry => entry.KeywordOrReference.FormKey) },
        { "ActivateParents", new GeneratedCopyReflectionPropertyHandler<IActivateParentsGetter, ActivateParents, IPlacedBeam, IPlacedBeamGetter>(
            "ActivateParents", value => value.DeepCopy(), ActivateParentsMixIn.Equals) },
        { "EnableParent", new GeneratedCopyReflectionPropertyHandler<IEnableParentGetter, EnableParent, IPlacedBeam, IPlacedBeamGetter>(
            "EnableParent", value => value.DeepCopy(), EnableParentMixIn.Equals) },
        { "Emittance", new SimpleReflectionFormLinkPropertyHandler<IEmittanceGetter, IPlacedBeam, IPlacedBeamGetter>("Emittance") },
        { "MultiBoundReference", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IPlacedBeam, IPlacedBeamGetter>("MultiBoundReference") },
        { "IgnoredBySandbox", new SimpleReflectionBinaryDataPropertyHandler<IPlacedBeam, IPlacedBeamGetter>("IgnoredBySandbox") },
        { "LocationRefTypes", new SimpleReflectionListPropertyHandler<IFormLinkGetter<ILocationReferenceTypeGetter>, IPlacedBeam, IPlacedBeamGetter>(
            "LocationRefTypes", ListSemantics.AlignedOrdered, canBeNull: true) },
        { "LocationReference", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, IPlacedBeam, IPlacedBeamGetter>("LocationReference") },
        { "DistantLodData", new GeneratedCopyReflectionPropertyHandler<IReadOnlyList<float>, ExtendedList<float>, IPlacedBeam, IPlacedBeamGetter>(
            "DistantLodData", value => new ExtendedList<float>(value), (left, right) => left.SequenceEqual(right)) },
        { "Scale", new SimpleReflectionPropertyHandler<float?, IPlacedBeam, IPlacedBeamGetter>("Scale") },
        { "Placement", new PlacementPropertyHandler<IPlacedBeam, IPlacedBeamGetter>() }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IPlacedBeamGetter placedBeamRecord)
        {
            throw new InvalidOperationException($"Expected IPlacedBeamGetter but got {winningContext.Record.GetType()}");
        }

        return placedBeamRecord
            .ToLink<IPlacedBeamGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IPlacedBeam, IPlacedBeamGetter>(state.LinkCache)
            .ToArray();
    }
}
