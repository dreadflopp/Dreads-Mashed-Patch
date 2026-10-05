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

// Migration note, PlacedFlame:
// - Generalized: all eighteen inherited placed fields reuse PHZD's existing handlers and xEdit keys.
// - Specialized: Projectile selects a complete PROJ identity; approved APlacedTrap header flags remain per bit.
// - Intentionally non-migrated: ACHR/REFR UDR coordination; no projectile-specific UDR policy is established.
// - Reason: xEdit uses ReferenceRecord for this signature; complete rows/aggregates preserve authored data.
// - Cleanup: this is a new route, with one registration per field and no superseded implementation.
public class PlacedFlameRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(APlacedTrap.MajorFlag)) },
        { "Projectile", new SimpleReflectionFormLinkPropertyHandler<IProjectileGetter, IPlacedFlame, IPlacedFlameGetter>("Projectile") },
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IPlacedFlame, IPlacedFlameGetter>() },
        { "EncounterZone", new SimpleReflectionFormLinkPropertyHandler<IEncounterZoneGetter, IPlacedFlame, IPlacedFlameGetter>("EncounterZone") },
        { "Owner", new SimpleReflectionFormLinkPropertyHandler<IOwnerGetter, IPlacedFlame, IPlacedFlameGetter>("Owner") },
        { "FactionRank", new SimpleReflectionPropertyHandler<int?, IPlacedFlame, IPlacedFlameGetter>("FactionRank") },
        { "HeadTrackingWeight", new SimpleReflectionPropertyHandler<float?, IPlacedFlame, IPlacedFlameGetter>("HeadTrackingWeight") },
        { "FavorCost", new SimpleReflectionPropertyHandler<float?, IPlacedFlame, IPlacedFlameGetter>("FavorCost") },
        { "Reflections", new GeneratedCopyReflectionListPropertyHandler<IWaterReflectionGetter, WaterReflection, IPlacedFlame, IPlacedFlameGetter>(
            "Reflections", ListSemantics.SortedKeyed, value => value.DeepCopy(), WaterReflectionMixIn.Equals,
            keySelector: entry => entry.Water.FormKey) },
        { "LinkedReferences", new GeneratedCopyReflectionListPropertyHandler<ILinkedReferencesGetter, LinkedReferences, IPlacedFlame, IPlacedFlameGetter>(
            "LinkedReferences", ListSemantics.SortedKeyed, value => value.DeepCopy(), LinkedReferencesMixIn.Equals,
            keySelector: entry => entry.KeywordOrReference.FormKey) },
        { "ActivateParents", new GeneratedCopyReflectionPropertyHandler<IActivateParentsGetter, ActivateParents, IPlacedFlame, IPlacedFlameGetter>(
            "ActivateParents", value => value.DeepCopy(), ActivateParentsMixIn.Equals) },
        { "EnableParent", new GeneratedCopyReflectionPropertyHandler<IEnableParentGetter, EnableParent, IPlacedFlame, IPlacedFlameGetter>(
            "EnableParent", value => value.DeepCopy(), EnableParentMixIn.Equals) },
        { "Emittance", new SimpleReflectionFormLinkPropertyHandler<IEmittanceGetter, IPlacedFlame, IPlacedFlameGetter>("Emittance") },
        { "MultiBoundReference", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IPlacedFlame, IPlacedFlameGetter>("MultiBoundReference") },
        { "IgnoredBySandbox", new SimpleReflectionBinaryDataPropertyHandler<IPlacedFlame, IPlacedFlameGetter>("IgnoredBySandbox") },
        { "LocationRefTypes", new SimpleReflectionListPropertyHandler<IFormLinkGetter<ILocationReferenceTypeGetter>, IPlacedFlame, IPlacedFlameGetter>(
            "LocationRefTypes", ListSemantics.AlignedOrdered, canBeNull: true) },
        { "LocationReference", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, IPlacedFlame, IPlacedFlameGetter>("LocationReference") },
        { "DistantLodData", new GeneratedCopyReflectionPropertyHandler<IReadOnlyList<float>, ExtendedList<float>, IPlacedFlame, IPlacedFlameGetter>(
            "DistantLodData", value => new ExtendedList<float>(value), (left, right) => left.SequenceEqual(right)) },
        { "Scale", new SimpleReflectionPropertyHandler<float?, IPlacedFlame, IPlacedFlameGetter>("Scale") },
        { "Placement", new PlacementPropertyHandler<IPlacedFlame, IPlacedFlameGetter>() }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IPlacedFlameGetter placedFlameRecord)
        {
            throw new InvalidOperationException($"Expected IPlacedFlameGetter but got {winningContext.Record.GetType()}");
        }

        return placedFlameRecord
            .ToLink<IPlacedFlameGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IPlacedFlame, IPlacedFlameGetter>(state.LinkCache)
            .ToArray();
    }
}
