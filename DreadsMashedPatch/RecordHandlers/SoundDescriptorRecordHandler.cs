using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using DreadsMashedPatch.PropertyHandlers.SoundDescriptor;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: translated text and LoopAndRumble use generated copies; scalar/link fields retain typed handlers.
    // - Kept specialized: SoundFiles preserves exact indexed paths; BNAM pitch and volume pairs are
    //   atomic semantic groups, while Priority remains independently mergeable.
    // - Intentionally non-migrated: LoopAndRumble is LNAM, not part of the BNAM grouping.
    // - Rationale: paired pitch and attenuation settings describe one adjustment, while grouping the
    //   entire packed BNAM would unnecessarily couple Priority to unrelated acoustic changes.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    public class SoundDescriptorRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Type", new SimpleReflectionPropertyHandler<SoundDescriptor.DescriptorType?, ISoundDescriptor, ISoundDescriptorGetter>("Type") },
            { "Category", new SimpleReflectionFormLinkPropertyHandler<ISoundCategoryGetter, ISoundDescriptor, ISoundDescriptorGetter>("Category") },
            { "AlternateSoundFor", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, ISoundDescriptor, ISoundDescriptorGetter>("AlternateSoundFor") },
            { "SoundFiles", new SoundFilesHandler() },
            { "OutputModel", new SimpleReflectionFormLinkPropertyHandler<ISoundOutputModelGetter, ISoundDescriptor, ISoundDescriptorGetter>("OutputModel") },
            { "String", new TranslatedStringReflectionPropertyHandler<ISoundDescriptor, ISoundDescriptorGetter>("String") },
            { "Conditions", new ConditionsHandler() },
            { "LoopAndRumble", new GeneratedCopyReflectionPropertyHandler<ISoundLoopAndRumbleGetter, SoundLoopAndRumble, ISoundDescriptor, ISoundDescriptorGetter>(
                "LoopAndRumble", value => value.DeepCopy(), SoundLoopAndRumbleMixIn.Equals) },
            { "Pitch", new SoundDescriptorPitchHandler() },
            { "Priority", new SimpleReflectionPropertyHandler<byte, ISoundDescriptor, ISoundDescriptorGetter>("Priority") },
            { "Volume", new SoundDescriptorVolumeHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ISoundDescriptorGetter soundDescriptorRecord)
            {
                throw new InvalidOperationException($"Expected ISoundDescriptorGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = soundDescriptorRecord
                .ToLink<ISoundDescriptorGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ISoundDescriptor, ISoundDescriptorGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }

        // CommitOverride and ApplyForwardedProperties are now handled by the base class
        // The base class automatically handles flag property coordination
    }
}
