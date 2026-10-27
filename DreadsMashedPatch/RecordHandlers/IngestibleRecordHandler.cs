using DreadsMashedPatch.AssetForwarding;
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
    // xEdit gives outer Effects entries no stable key; each declaration position is one atomic row.
    public class IngestibleRecordHandler : AbstractRecordHandler
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
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Ingestible.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IIngestible, IIngestibleGetter>("Name") },
            { "Description", new TranslatedStringReflectionPropertyHandler<IIngestible, IIngestibleGetter>("Description") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Destructible", DestructibleDataHandlers.Data(new DestructibleHandler()) },
            { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new DestructibleHandler()) },
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
    }
}
