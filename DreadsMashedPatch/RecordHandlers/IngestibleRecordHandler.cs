using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Ingestible;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: ObjectBounds, Description, PickUpSound, PutDownSound, EquipmentType, Addiction, AddictionChance, ConsumeSound.
    // - Generalized Effects reconciliation to the shared exact-position atomic handler.
    // - Kept specialized: Destructible, Icons, Effects collection access, Flags. Header aliases now use the composite raw handler.
    // - Rationale: translated text uses generated copying; xEdit gives outer Effects
    //   entries no row key, while collection access and flag handling remain record-specific.

    // Header migration: raw/common/Ingestible.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class IngestibleRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Ingestible.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IIngestible, IIngestibleGetter>("Name") },
            { "Description", new TranslatedStringReflectionPropertyHandler<IIngestible, IIngestibleGetter>("Description") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Destructible", new DestructibleHandler() },
            { "Icons", new IconsHandler() },
            { "PickUpSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IIngestible, IIngestibleGetter>("PickUpSound") },
            { "PutDownSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IIngestible, IIngestibleGetter>("PutDownSound") },
            { "EquipmentType", new SimpleReflectionFormLinkPropertyHandler<IEquipTypeGetter, IIngestible, IIngestibleGetter>("EquipmentType") },
            { "Weight", new WeightHandler() },
            { "Value", new ValueHandler() },
            { "Keywords", new KeywordListHandler() },
            { "Addiction", new SimpleReflectionFormLinkPropertyHandler<ISkyrimMajorRecordGetter, IIngestible, IIngestibleGetter>("Addiction") },
            { "AddictionChance", new SimpleReflectionPropertyHandler<float, IIngestible, IIngestibleGetter>("AddictionChance", 0.001f) },
            { "ConsumeSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IIngestible, IIngestibleGetter>("ConsumeSound") },
            { "Effects", new EffectHandler() },
            { "Flags", new FlagsHandler() },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IIngestibleGetter ingestibleRecord)
            {
                throw new InvalidOperationException($"Expected IIngestibleGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = ingestibleRecord
                .ToLink<IIngestibleGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IIngestible, IIngestibleGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }

        // CommitOverride and ApplyForwardedProperties are now handled by the base class
        // The base class automatically handles flag property coordination
    }
}
