using DreadsMashedPatch.AssetForwarding;
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
    public class ActivatorRecordHandler : AbstractRecordHandler
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

        private readonly Dictionary<string, IPropertyHandler> _propertyHandlers;

        public ActivatorRecordHandler()
        {
            _propertyHandlers = new Dictionary<string, IPropertyHandler>
            {
                { "EditorID", new EditorIDHandler() },
                { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Mutagen.Bethesda.Skyrim.Activator.MajorFlag)) },
                { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IActivator, IActivatorGetter>() },
                { "Name", new TranslatedStringReflectionPropertyHandler<IActivator, IActivatorGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
                { "Destructible", DestructibleDataHandlers.Data(new DestructibleHandler()) },
                { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new DestructibleHandler()) },
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
