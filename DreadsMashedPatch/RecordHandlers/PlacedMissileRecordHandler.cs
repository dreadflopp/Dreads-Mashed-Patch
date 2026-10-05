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

// Migration note, PlacedMissile:
// - Generalized: all eighteen inherited placed fields reuse PHZD's existing handlers and xEdit keys.
// - Specialized: Projectile selects a complete PROJ identity; approved APlacedTrap header flags remain per bit.
// - Intentionally non-migrated: ACHR/REFR UDR coordination; no projectile-specific UDR policy is established.
// - Reason: xEdit uses ReferenceRecord for this signature; complete rows/aggregates preserve authored data.
// - Cleanup: this is a new route, with one registration per field and no superseded implementation.
public class PlacedMissileRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(APlacedTrap.MajorFlag)) },
        { "Projectile", new SimpleReflectionFormLinkPropertyHandler<IProjectileGetter, IPlacedMissile, IPlacedMissileGetter>("Projectile") },
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IPlacedMissile, IPlacedMissileGetter>() },
        { "EncounterZone", new SimpleReflectionFormLinkPropertyHandler<IEncounterZoneGetter, IPlacedMissile, IPlacedMissileGetter>("EncounterZone") },
        { "Owner", new SimpleReflectionFormLinkPropertyHandler<IOwnerGetter, IPlacedMissile, IPlacedMissileGetter>("Owner") },
        { "FactionRank", new SimpleReflectionPropertyHandler<int?, IPlacedMissile, IPlacedMissileGetter>("FactionRank") },
        { "HeadTrackingWeight", new SimpleReflectionPropertyHandler<float?, IPlacedMissile, IPlacedMissileGetter>("HeadTrackingWeight") },
        { "FavorCost", new SimpleReflectionPropertyHandler<float?, IPlacedMissile, IPlacedMissileGetter>("FavorCost") },
        { "Reflections", new GeneratedCopyReflectionListPropertyHandler<IWaterReflectionGetter, WaterReflection, IPlacedMissile, IPlacedMissileGetter>(
            "Reflections", ListSemantics.SortedKeyed, value => value.DeepCopy(), WaterReflectionMixIn.Equals,
            keySelector: entry => entry.Water.FormKey) },
        { "LinkedReferences", new GeneratedCopyReflectionListPropertyHandler<ILinkedReferencesGetter, LinkedReferences, IPlacedMissile, IPlacedMissileGetter>(
            "LinkedReferences", ListSemantics.SortedKeyed, value => value.DeepCopy(), LinkedReferencesMixIn.Equals,
            keySelector: entry => entry.KeywordOrReference.FormKey) },
        { "ActivateParents", new GeneratedCopyReflectionPropertyHandler<IActivateParentsGetter, ActivateParents, IPlacedMissile, IPlacedMissileGetter>(
            "ActivateParents", value => value.DeepCopy(), ActivateParentsMixIn.Equals) },
        { "EnableParent", new GeneratedCopyReflectionPropertyHandler<IEnableParentGetter, EnableParent, IPlacedMissile, IPlacedMissileGetter>(
            "EnableParent", value => value.DeepCopy(), EnableParentMixIn.Equals) },
        { "Emittance", new SimpleReflectionFormLinkPropertyHandler<IEmittanceGetter, IPlacedMissile, IPlacedMissileGetter>("Emittance") },
        { "MultiBoundReference", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IPlacedMissile, IPlacedMissileGetter>("MultiBoundReference") },
        { "IgnoredBySandbox", new SimpleReflectionBinaryDataPropertyHandler<IPlacedMissile, IPlacedMissileGetter>("IgnoredBySandbox") },
        { "LocationRefTypes", new SimpleReflectionListPropertyHandler<IFormLinkGetter<ILocationReferenceTypeGetter>, IPlacedMissile, IPlacedMissileGetter>(
            "LocationRefTypes", ListSemantics.AlignedOrdered, canBeNull: true) },
        { "LocationReference", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, IPlacedMissile, IPlacedMissileGetter>("LocationReference") },
        { "DistantLodData", new GeneratedCopyReflectionPropertyHandler<IReadOnlyList<float>, ExtendedList<float>, IPlacedMissile, IPlacedMissileGetter>(
            "DistantLodData", value => new ExtendedList<float>(value), (left, right) => left.SequenceEqual(right)) },
        { "Scale", new SimpleReflectionPropertyHandler<float?, IPlacedMissile, IPlacedMissileGetter>("Scale") },
        { "Placement", new PlacementPropertyHandler<IPlacedMissile, IPlacedMissileGetter>() }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IPlacedMissileGetter placedMissileRecord)
        {
            throw new InvalidOperationException($"Expected IPlacedMissileGetter but got {winningContext.Record.GetType()}");
        }

        return placedMissileRecord
            .ToLink<IPlacedMissileGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IPlacedMissile, IPlacedMissileGetter>(state.LinkCache)
            .ToArray();
    }
}
