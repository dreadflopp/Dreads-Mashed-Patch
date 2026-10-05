using System.Drawing;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Activator;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: scalar, translated-text, form-link, model, and VMAD fields use shared semantic handlers.
    // - Kept specialized: destructible data remains atomic; record and major flags retain approved flag handlers.
    // - Rationale: aggregate copying preserves nested binary/model state while independent fields remain mergeable.

    // Header migration: raw/common/Activator.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class ActivatorRecordHandler : AbstractRecordHandler
    {
        private readonly Dictionary<string, IPropertyHandler> _propertyHandlers;

        public ActivatorRecordHandler()
        {
            // Initialize property handlers for Activator records.
            // Uses reflection-based handlers from General where possible (same approach as PlacedObjectRecordHandler).
            _propertyHandlers = new Dictionary<string, IPropertyHandler>
            {
                { "EditorID", new EditorIDHandler() },
                { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Mutagen.Bethesda.Skyrim.Activator.MajorFlag)) },
                // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
                { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IActivator, IActivatorGetter>() },
                { "Name", new TranslatedStringReflectionPropertyHandler<IActivator, IActivatorGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
                { "Destructible", new DestructibleHandler() },
                { "Keywords", new KeywordListHandler() },
                { "MarkerColor", new SimpleReflectionPropertyHandler<Color?, IActivator, IActivatorGetter>("MarkerColor") },
                { "LoopingSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IActivator, IActivatorGetter>("LoopingSound") },
                { "ActivationSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IActivator, IActivatorGetter>("ActivationSound") },
                { "WaterType", new SimpleReflectionFormLinkPropertyHandler<IWaterGetter, IActivator, IActivatorGetter>("WaterType") },
                { "ActivateTextOverride", new TranslatedStringReflectionPropertyHandler<IActivator, IActivatorGetter>("ActivateTextOverride") },
                { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.Activator.Flag, IActivator, IActivatorGetter>("Flags") },

                { "InteractionKeyword", new SimpleReflectionFormLinkPropertyHandler<IKeywordGetter, IActivator, IActivatorGetter>("InteractionKeyword") }
            };
        }

        public override Dictionary<string, IPropertyHandler> PropertyHandlers => _propertyHandlers;

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IActivatorGetter activatorRecord)
            {
                throw new InvalidOperationException($"Expected IActivatorGetter but got {winningContext.Record.GetType()}");
            }
            return activatorRecord
                .ToLink<IActivatorGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IActivator, IActivatorGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
