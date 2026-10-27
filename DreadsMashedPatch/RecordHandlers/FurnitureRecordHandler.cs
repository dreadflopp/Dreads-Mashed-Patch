using DreadsMashedPatch.AssetForwarding;
using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Furniture;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Noggog;

namespace DreadsMashedPatch.RecordHandlers
{
    public class FurnitureRecordHandler : AbstractRecordHandler
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
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Furniture.MajorFlag)) },
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IFurniture, IFurnitureGetter>() },
            { "Name", new TranslatedStringReflectionPropertyHandler<IFurniture, IFurnitureGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Destructible", DestructibleDataHandlers.Data(new DestructibleHandler()) },
            { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new DestructibleHandler()) },
            { "Keywords", new KeywordListHandler() },
            { "PNAM", new SimpleReflectionBinaryDataPropertyHandler<IFurniture, IFurnitureGetter>("PNAM") },
            { "Flags", new FlagsHandler() },
            { "InteractionKeyword", new SimpleReflectionFormLinkPropertyHandler<IKeywordGetter, IFurniture, IFurnitureGetter>("InteractionKeyword") },
            { "WorkbenchData", new GeneratedCopyReflectionPropertyHandler<IWorkbenchDataGetter, WorkbenchData, IFurniture, IFurnitureGetter>(
                "WorkbenchData", value => value.DeepCopy(), WorkbenchDataMixIn.Equals) },
            { "AssociatedSpell", new SimpleReflectionFormLinkPropertyHandler<ISpellGetter, IFurniture, IFurnitureGetter>("AssociatedSpell") },
            { "Markers", new AtomicGeneratedCopyReflectionListPropertyHandler<IFurnitureMarkerGetter, FurnitureMarker, IFurniture, IFurnitureGetter>(
                "Markers", value => value.DeepCopy(), FurnitureMarkerMixIn.Equals, canBeNull: true) },
            { "ModelFilename", new SimpleReflectionAssetLinkPropertyHandler<SkyrimModelAssetType, IFurniture, IFurnitureGetter>("ModelFilename") },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IFurnitureGetter furniture)
            {
                throw new InvalidOperationException($"Expected IFurnitureGetter but got {winningContext.Record.GetType()}");
            }

            return furniture
                .ToLink<IFurnitureGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IFurniture, IFurnitureGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
