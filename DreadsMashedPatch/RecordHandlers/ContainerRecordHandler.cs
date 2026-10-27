using DreadsMashedPatch.AssetForwarding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Container;
using DreadsMashedPatch.PropertyHandlers.General;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Inventory snapshots share NPC duplicate/COED reconciliation, but CONT permits new counts
    // and requires ownership only for baseline reversions. Count writes must retain COED metadata.
    public class ContainerRecordHandler : AbstractRecordHandler
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
            { "Name", new TranslatedStringReflectionPropertyHandler<IContainer, IContainerGetter>("Name") },
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Container.MajorFlag)) },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Weight", new SimpleReflectionPropertyHandler<float, IContainer, IContainerGetter>("Weight") },
            { "Items", new ItemHandler() },
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IContainer, IContainerGetter>() },
            { "Destructible", DestructibleDataHandlers.Data(new DestructibleHandler()) },
            { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new DestructibleHandler()) },
            { "Flags", new FlagsHandler() },
            { "OpenSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IContainer, IContainerGetter>("OpenSound") },
            { "CloseSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IContainer, IContainerGetter>("CloseSound") },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IContainerGetter containerRecord)
            {
                throw new InvalidOperationException($"Expected IContainerGetter but got {winningContext.Record.GetType()}");
            }
            return containerRecord
                .ToLink<IContainerGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
