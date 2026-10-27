using DreadsMashedPatch.AssetForwarding;
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
    // Clips, type, loop/rumble, pitch/volume, routing and subtitles form one playback definition.
    // Priority and Conditions remain independent so those edits cannot replace playback resources.
    public class SoundDescriptorRecordHandler : AbstractRecordHandler
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
    }
}
