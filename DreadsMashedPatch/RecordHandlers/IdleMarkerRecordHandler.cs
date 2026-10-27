using DreadsMashedPatch.AssetForwarding;
using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.IdleMarker;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Idle references, order/flags and timer form one selection; model/bounds are independent.
    // Nullable idle flags normalize zero/absence through the approved masked handler.
    public class IdleMarkerRecordHandler : AbstractRecordHandler
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
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(IdleMarker.MajorFlag)) },
            { "Flags", new FlagsHandler() },
            { "IdleTimer", new SimpleReflectionPropertyHandler<float?, IIdleMarker, IIdleMarkerGetter>("IdleTimer", 0.001f) },
            { "Animations", new AtomicReflectionListPropertyHandler<IFormLinkGetter<IIdleAnimationGetter>, IIdleMarker, IIdleMarkerGetter>("Animations", true) },
            { "ModelAndBounds", new ModelBoundsHandler() },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IIdleMarkerGetter idleMarker)
            {
                throw new InvalidOperationException($"Expected IIdleMarkerGetter but got {winningContext.Record.GetType()}");
            }

            return idleMarker
                .ToLink<IIdleMarkerGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IIdleMarker, IIdleMarkerGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
