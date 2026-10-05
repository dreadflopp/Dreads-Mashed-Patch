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

namespace DreadsMashedPatch.RecordHandlers;

// Migration note:
// - Generalized: TREE scalars/links use typed handlers; Production uses generated copy/equality.
// - Kept specialized: none.
// - Intentionally excluded: Unknown is outside the semantic conflict surface.
// - Rationale: surface aligns with existing flora/static forwarding patterns.

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
        // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<ITree, ITreeGetter>() },
        { "ModelAndBounds", new ModelBoundsHandler() },
        { "Ingredient", new SimpleReflectionFormLinkPropertyHandler<IHarvestTargetGetter, ITree, ITreeGetter>("Ingredient") },
        { "HarvestSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, ITree, ITreeGetter>("HarvestSound") },
        { "Production", new GeneratedCopyReflectionPropertyHandler<ISeasonalIngredientProductionGetter, SeasonalIngredientProduction, ITree, ITreeGetter>(
            "Production", value => value.DeepCopy(), SeasonalIngredientProductionMixIn.Equals) },
        { "Name", new TranslatedStringReflectionPropertyHandler<ITree, ITreeGetter>("Name") },
        { "TrunkFlexibility", new SimpleReflectionPropertyHandler<float, ITree, ITreeGetter>("TrunkFlexibility") },
        { "BranchFlexibility", new SimpleReflectionPropertyHandler<float, ITree, ITreeGetter>("BranchFlexibility") },
        { "LeafAmplitude", new SimpleReflectionPropertyHandler<float, ITree, ITreeGetter>("LeafAmplitude") },
        { "LeafFrequency", new SimpleReflectionPropertyHandler<float, ITree, ITreeGetter>("LeafFrequency") },

    };

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
