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

// Migration note, PlacedTrap:
// - Generalized: all eighteen inherited placed fields reuse PHZD's existing handlers and xEdit keys.
// - Specialized: Projectile selects a complete PROJ identity; approved APlacedTrap header flags remain per bit.
// - Intentionally non-migrated: ACHR/REFR UDR coordination; no projectile-specific UDR policy is established.
// - Reason: xEdit uses ReferenceRecord for this signature; complete rows/aggregates preserve authored data.
// - Cleanup: this is a new route, with one registration per field and no superseded implementation.
public class PlacedTrapRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(APlacedTrap.MajorFlag)) },
        { "Projectile", new SimpleReflectionFormLinkPropertyHandler<IProjectileGetter, IPlacedTrap, IPlacedTrapGetter>("Projectile") },
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IPlacedTrap, IPlacedTrapGetter>() },
        { "EncounterZone", new SimpleReflectionFormLinkPropertyHandler<IEncounterZoneGetter, IPlacedTrap, IPlacedTrapGetter>("EncounterZone") },
        { "Owner", new SimpleReflectionFormLinkPropertyHandler<IOwnerGetter, IPlacedTrap, IPlacedTrapGetter>("Owner") },
        { "FactionRank", new SimpleReflectionPropertyHandler<int?, IPlacedTrap, IPlacedTrapGetter>("FactionRank") },
        { "HeadTrackingWeight", new SimpleReflectionPropertyHandler<float?, IPlacedTrap, IPlacedTrapGetter>("HeadTrackingWeight") },
        { "FavorCost", new SimpleReflectionPropertyHandler<float?, IPlacedTrap, IPlacedTrapGetter>("FavorCost") },
        { "Reflections", new GeneratedCopyReflectionListPropertyHandler<IWaterReflectionGetter, WaterReflection, IPlacedTrap, IPlacedTrapGetter>(
            "Reflections", ListSemantics.SortedKeyed, value => value.DeepCopy(), WaterReflectionMixIn.Equals,
            keySelector: entry => entry.Water.FormKey) },
        { "LinkedReferences", new GeneratedCopyReflectionListPropertyHandler<ILinkedReferencesGetter, LinkedReferences, IPlacedTrap, IPlacedTrapGetter>(
            "LinkedReferences", ListSemantics.SortedKeyed, value => value.DeepCopy(), LinkedReferencesMixIn.Equals,
            keySelector: entry => entry.KeywordOrReference.FormKey) },
        { "ActivateParents", new GeneratedCopyReflectionPropertyHandler<IActivateParentsGetter, ActivateParents, IPlacedTrap, IPlacedTrapGetter>(
            "ActivateParents", value => value.DeepCopy(), ActivateParentsMixIn.Equals) },
        { "EnableParent", new GeneratedCopyReflectionPropertyHandler<IEnableParentGetter, EnableParent, IPlacedTrap, IPlacedTrapGetter>(
            "EnableParent", value => value.DeepCopy(), EnableParentMixIn.Equals) },
        { "Emittance", new SimpleReflectionFormLinkPropertyHandler<IEmittanceGetter, IPlacedTrap, IPlacedTrapGetter>("Emittance") },
        { "MultiBoundReference", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IPlacedTrap, IPlacedTrapGetter>("MultiBoundReference") },
        { "IgnoredBySandbox", new SimpleReflectionBinaryDataPropertyHandler<IPlacedTrap, IPlacedTrapGetter>("IgnoredBySandbox") },
        { "LocationRefTypes", new SimpleReflectionListPropertyHandler<IFormLinkGetter<ILocationReferenceTypeGetter>, IPlacedTrap, IPlacedTrapGetter>(
            "LocationRefTypes", ListSemantics.AlignedOrdered, canBeNull: true) },
        { "LocationReference", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, IPlacedTrap, IPlacedTrapGetter>("LocationReference") },
        { "DistantLodData", new GeneratedCopyReflectionPropertyHandler<IReadOnlyList<float>, ExtendedList<float>, IPlacedTrap, IPlacedTrapGetter>(
            "DistantLodData", value => new ExtendedList<float>(value), (left, right) => left.SequenceEqual(right)) },
        { "Scale", new SimpleReflectionPropertyHandler<float?, IPlacedTrap, IPlacedTrapGetter>("Scale") },
        { "Placement", new PlacementPropertyHandler<IPlacedTrap, IPlacedTrapGetter>() }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IPlacedTrapGetter placedTrapRecord)
        {
            throw new InvalidOperationException($"Expected IPlacedTrapGetter but got {winningContext.Record.GetType()}");
        }

        return placedTrapRecord
            .ToLink<IPlacedTrapGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IPlacedTrap, IPlacedTrapGetter>(state.LinkCache)
            .ToArray();
    }
}
