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
    // Migration note:
    // - Generalized: VM/bounds/name/model, keywords, binary data, links, and generated copies for workbench/marker data.
    // - Kept specialized: Destructible and nullable furniture flags via dedicated handlers.
    // - Rationale: generated copies safely materialize overlay marker rows while preserving destructible and flag semantics.

    // Header migration: raw/common/Furniture.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class FurnitureRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Furniture.MajorFlag)) },
            // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IFurniture, IFurnitureGetter>() },
            { "Name", new TranslatedStringReflectionPropertyHandler<IFurniture, IFurnitureGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Destructible", new DestructibleHandler() },
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
