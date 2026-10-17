using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Tree;

namespace DreadsMashedPatch.RecordHandlers;

// Migration note:
// - Generalized: metadata/harvesting retain shared handlers and atomic seasonal Production.
// - Specialized: CNAM is one complete TreeData snapshot including its 32-byte animation block.
// - Restricted forwarding (default): model/bounds, CNAM, VMAD and approved header
//   flags retain the effective winner; disabling the restriction uses the same handlers normally.
// - Removed: four independent CNAM float registrations and optional model/wind selection;
//   Unknown now travels inside TreeData. No separate model/wind selection path remains.
// - Rationale: TREE stays enabled and users can opt out of preserving asset-dependent fields.

// Header migration: raw/common/Tree.MajorFlag flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
// Name migration: translated Name uses generated copying to retain every selected language.
// Optional null names remove the value; comparison follows Mutagen's language policy.
// Other specialized fields/flags retain their policies; translations are selected as one value.
public class TreeRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Tree.MajorFlag)) },
        // The shared VMAD handler retains winner metadata; restrictive forwarding resolves its context from the winner.
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<ITree, ITreeGetter>() },
        { "ModelAndBounds", new ModelBoundsHandler() },
        { "Ingredient", new SimpleReflectionFormLinkPropertyHandler<IHarvestTargetGetter, ITree, ITreeGetter>("Ingredient") },
        { "HarvestSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, ITree, ITreeGetter>("HarvestSound") },
        { "Production", new GeneratedCopyReflectionPropertyHandler<ISeasonalIngredientProductionGetter, SeasonalIngredientProduction, ITree, ITreeGetter>(
            "Production", value => value.DeepCopy(), SeasonalIngredientProductionMixIn.Equals) },
        { "Name", new TranslatedStringReflectionPropertyHandler<ITree, ITreeGetter>("Name") },
        { "TreeData", new TreeDataHandler() },
    };

    private static readonly string[] WinnerOwnedProperties =
        ["MajorRecordFlagsRaw", "VirtualMachineAdapter", "ModelAndBounds", "TreeData"];

    protected override void InitializePropertyContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> originalContext,
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext)
    {
        base.InitializePropertyContexts(originalContext, winningContext);
        if (!PatcherSettings.RestrictTreeForwarding) return;

        foreach (var property in WinnerOwnedProperties)
        {
            PropertyHandlers[property].InitializeContext(winningContext, winningContext, PropertyContexts[property]);
            PropertyContexts[property].IsResolved = true;
        }
    }

    protected override PropertyForwardingCoordination CoordinateForwardedProperties(
        IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state) =>
        PatcherSettings.RestrictTreeForwarding
            ? new(new Dictionary<string, object?>(),
                "TREE restricted forwarding: model/bounds, CNAM, scripts and flags retain the winning override to avoid mixing incompatible appearance and behavior.", "TreeData")
            : PropertyForwardingCoordination.None;

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not ITreeGetter treeRecord)
        {
            throw new InvalidOperationException($"Expected ITreeGetter but got {winningContext.Record.GetType()}");
        }

        return treeRecord
            .ToLink<ITreeGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ITree, ITreeGetter>(state.LinkCache)
            .ToArray();
    }
}
