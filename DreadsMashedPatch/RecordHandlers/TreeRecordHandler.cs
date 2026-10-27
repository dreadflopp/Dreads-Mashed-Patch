using DreadsMashedPatch.AssetForwarding;
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

// One Trees selection keeps model/bounds, CNAM wind data, scripts and header flags together.
// It defaults off; name, EDID and harvesting follow ordinary policies. CNAM includes the 32-byte
// wind block, and script writes preserve destination VMAD metadata.
public class TreeRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Tree.MajorFlag)) },
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<ITree, ITreeGetter>() },
        { "ModelAndBounds", new ModelBoundsHandler() },
        { "Ingredient", new SimpleReflectionFormLinkPropertyHandler<IHarvestTargetGetter, ITree, ITreeGetter>("Ingredient") },
        { "HarvestSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, ITree, ITreeGetter>("HarvestSound") },
        { "Production", new GeneratedCopyReflectionPropertyHandler<ISeasonalIngredientProductionGetter, SeasonalIngredientProduction, ITree, ITreeGetter>(
            "Production", value => value.DeepCopy(), SeasonalIngredientProductionMixIn.Equals) },
        { "Name", new TranslatedStringReflectionPropertyHandler<ITree, ITreeGetter>("Name") },
        { "TreeData", new TreeDataHandler() },
    };

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
