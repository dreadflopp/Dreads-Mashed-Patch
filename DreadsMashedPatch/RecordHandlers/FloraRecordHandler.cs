using DreadsMashedPatch.AssetForwarding;
using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Flora;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Noggog;

namespace DreadsMashedPatch.RecordHandlers
{
    public class FloraRecordHandler : AbstractRecordHandler
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
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IFlora, IFloraGetter>() },
            { "Name", new TranslatedStringReflectionPropertyHandler<IFlora, IFloraGetter>("Name", required: true) },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Destructible", DestructibleDataHandlers.Data(new DestructibleHandler()) },
            { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new DestructibleHandler()) },
            { "Keywords", new KeywordListHandler() },
            { "PNAM", new SimpleReflectionBinaryDataPropertyHandler<IFlora, IFloraGetter>("PNAM") },
            { "ActivateTextOverride", new TranslatedStringReflectionPropertyHandler<IFlora, IFloraGetter>("ActivateTextOverride") },
            { "FNAM", new SimpleReflectionBinaryDataPropertyHandler<IFlora, IFloraGetter>("FNAM") },
            { "Ingredient", new SimpleReflectionFormLinkPropertyHandler<IHarvestTargetGetter, IFlora, IFloraGetter>("Ingredient") },
            { "HarvestSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IFlora, IFloraGetter>("HarvestSound") },
            { "Production", new GeneratedCopyReflectionPropertyHandler<ISeasonalIngredientProductionGetter, SeasonalIngredientProduction, IFlora, IFloraGetter>(
                "Production", value => value.DeepCopy(), SeasonalIngredientProductionMixIn.Equals) }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IFloraGetter flora)
            {
                throw new InvalidOperationException($"Expected IFloraGetter but got {winningContext.Record.GetType()}");
            }

            return flora
                .ToLink<IFloraGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IFlora, IFloraGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
