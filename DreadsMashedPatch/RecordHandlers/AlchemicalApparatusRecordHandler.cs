using DreadsMashedPatch.AssetForwarding;
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
    public class AlchemicalApparatusRecordHandler : AbstractRecordHandler
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
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IAlchemicalApparatus, IAlchemicalApparatusGetter>() },
            { "Name", new TranslatedStringReflectionPropertyHandler<IAlchemicalApparatus, IAlchemicalApparatusGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Icons", new IconsHandler() },
            { "Destructible", DestructibleDataHandlers.Data(new DestructibleHandler()) },
            { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new DestructibleHandler()) },
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
