using DreadsMashedPatch.AssetForwarding;
using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Door;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    public class DoorRecordHandler : AbstractRecordHandler
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
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Door.MajorFlag)) },
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IDoor, IDoorGetter>() },
            { "Name", new TranslatedStringReflectionPropertyHandler<IDoor, IDoorGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Destructible", DestructibleDataHandlers.Data(new DestructibleHandler()) },
            { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new DestructibleHandler()) },
            { "OpenSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IDoor, IDoorGetter>("OpenSound") },
            { "CloseSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IDoor, IDoorGetter>("CloseSound") },
            { "LoopSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IDoor, IDoorGetter>("LoopSound") },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.Door.Flag, IDoor, IDoorGetter>("Flags") },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IDoorGetter door)
            {
                throw new InvalidOperationException($"Expected IDoorGetter but got {winningContext.Record.GetType()}");
            }

            return door
                .ToLink<IDoorGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
