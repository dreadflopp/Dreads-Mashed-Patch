using DreadsMashedPatch.AssetForwarding;
using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    public class AddonNodeRecordHandler : AbstractRecordHandler
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
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "NodeIndex", new SimpleReflectionPropertyHandler<int, IAddonNode, IAddonNodeGetter>("NodeIndex") },
            { "Sound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IAddonNode, IAddonNodeGetter>("Sound") },
            { "MasterParticleSystemCap", new SimpleReflectionPropertyHandler<ushort, IAddonNode, IAddonNodeGetter>("MasterParticleSystemCap") },
            { "Flags", new SimpleReflectionFlagPropertyHandler<AddonNode.Flag, IAddonNode, IAddonNodeGetter>("Flags", preserveUnknownBits: true) }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IAddonNodeGetter addonNode)
            {
                throw new InvalidOperationException($"Expected IAddonNodeGetter but got {winningContext.Record.GetType()}");
            }

            return addonNode
                .ToLink<IAddonNodeGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IAddonNode, IAddonNodeGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
