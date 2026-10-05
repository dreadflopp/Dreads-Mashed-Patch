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

// Migration note, PlacedArrow:
// - Generalized: all eighteen inherited placed fields reuse PHZD's existing handlers and xEdit keys.
// - Specialized: Projectile selects a complete PROJ identity; approved APlacedTrap header flags remain per bit.
// - Intentionally non-migrated: ACHR/REFR UDR coordination; no projectile-specific UDR policy is established.
// - Reason: xEdit uses ReferenceRecord for this signature; complete rows/aggregates preserve authored data.
// - Cleanup: this is a new route, with one registration per field and no superseded implementation.
public class PlacedArrowRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(APlacedTrap.MajorFlag)) },
        { "Projectile", new SimpleReflectionFormLinkPropertyHandler<IProjectileGetter, IPlacedArrow, IPlacedArrowGetter>("Projectile") },
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IPlacedArrow, IPlacedArrowGetter>() },
        { "EncounterZone", new SimpleReflectionFormLinkPropertyHandler<IEncounterZoneGetter, IPlacedArrow, IPlacedArrowGetter>("EncounterZone") },
        { "Owner", new SimpleReflectionFormLinkPropertyHandler<IOwnerGetter, IPlacedArrow, IPlacedArrowGetter>("Owner") },
        { "FactionRank", new SimpleReflectionPropertyHandler<int?, IPlacedArrow, IPlacedArrowGetter>("FactionRank") },
        { "HeadTrackingWeight", new SimpleReflectionPropertyHandler<float?, IPlacedArrow, IPlacedArrowGetter>("HeadTrackingWeight") },
        { "FavorCost", new SimpleReflectionPropertyHandler<float?, IPlacedArrow, IPlacedArrowGetter>("FavorCost") },
        { "Reflections", new GeneratedCopyReflectionListPropertyHandler<IWaterReflectionGetter, WaterReflection, IPlacedArrow, IPlacedArrowGetter>(
            "Reflections", ListSemantics.SortedKeyed, value => value.DeepCopy(), WaterReflectionMixIn.Equals,
            keySelector: entry => entry.Water.FormKey) },
        { "LinkedReferences", new GeneratedCopyReflectionListPropertyHandler<ILinkedReferencesGetter, LinkedReferences, IPlacedArrow, IPlacedArrowGetter>(
            "LinkedReferences", ListSemantics.SortedKeyed, value => value.DeepCopy(), LinkedReferencesMixIn.Equals,
            keySelector: entry => entry.KeywordOrReference.FormKey) },
        { "ActivateParents", new GeneratedCopyReflectionPropertyHandler<IActivateParentsGetter, ActivateParents, IPlacedArrow, IPlacedArrowGetter>(
            "ActivateParents", value => value.DeepCopy(), ActivateParentsMixIn.Equals) },
        { "EnableParent", new GeneratedCopyReflectionPropertyHandler<IEnableParentGetter, EnableParent, IPlacedArrow, IPlacedArrowGetter>(
            "EnableParent", value => value.DeepCopy(), EnableParentMixIn.Equals) },
        { "Emittance", new SimpleReflectionFormLinkPropertyHandler<IEmittanceGetter, IPlacedArrow, IPlacedArrowGetter>("Emittance") },
        { "MultiBoundReference", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IPlacedArrow, IPlacedArrowGetter>("MultiBoundReference") },
        { "IgnoredBySandbox", new SimpleReflectionBinaryDataPropertyHandler<IPlacedArrow, IPlacedArrowGetter>("IgnoredBySandbox") },
        { "LocationRefTypes", new SimpleReflectionListPropertyHandler<IFormLinkGetter<ILocationReferenceTypeGetter>, IPlacedArrow, IPlacedArrowGetter>(
            "LocationRefTypes", ListSemantics.AlignedOrdered, canBeNull: true) },
        { "LocationReference", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, IPlacedArrow, IPlacedArrowGetter>("LocationReference") },
        { "DistantLodData", new GeneratedCopyReflectionPropertyHandler<IReadOnlyList<float>, ExtendedList<float>, IPlacedArrow, IPlacedArrowGetter>(
            "DistantLodData", value => new ExtendedList<float>(value), (left, right) => left.SequenceEqual(right)) },
        { "Scale", new SimpleReflectionPropertyHandler<float?, IPlacedArrow, IPlacedArrowGetter>("Scale") },
        { "Placement", new PlacementPropertyHandler<IPlacedArrow, IPlacedArrowGetter>() }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IPlacedArrowGetter placedArrowRecord)
        {
            throw new InvalidOperationException($"Expected IPlacedArrowGetter but got {winningContext.Record.GetType()}");
        }

        return placedArrowRecord
            .ToLink<IPlacedArrowGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IPlacedArrow, IPlacedArrowGetter>(state.LinkCache)
            .ToArray();
    }
}
