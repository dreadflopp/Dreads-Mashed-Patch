using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.AlchemicalApparatus;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: Description uses generated localized-string copying; scalar/link fields use typed handlers.
    // - Kept specialized: ObjectBounds/Model/Icons/Destructible/Value/Weight via existing shared handlers.
    // - Rationale: follows established item handler pattern used by adjacent record handlers.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class AlchemicalApparatusRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IAlchemicalApparatus, IAlchemicalApparatusGetter>() },
            { "Name", new TranslatedStringReflectionPropertyHandler<IAlchemicalApparatus, IAlchemicalApparatusGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Icons", new IconsHandler() },
            { "Destructible", new DestructibleHandler() },
            { "PickUpSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>("PickUpSound") },
            { "PutDownSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>("PutDownSound") },
            { "Quality", new SimpleReflectionPropertyHandler<QualityLevel?, IAlchemicalApparatus, IAlchemicalApparatusGetter>("Quality") },
            { "Description", new TranslatedStringReflectionPropertyHandler<IAlchemicalApparatus, IAlchemicalApparatusGetter>("Description") },
            { "Value", new ValueHandler() },
            { "Weight", new WeightHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IAlchemicalApparatusGetter apparatus)
            {
                throw new InvalidOperationException($"Expected IAlchemicalApparatusGetter but got {winningContext.Record.GetType()}");
            }

            return apparatus
                .ToLink<IAlchemicalApparatusGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IAlchemicalApparatus, IAlchemicalApparatusGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
