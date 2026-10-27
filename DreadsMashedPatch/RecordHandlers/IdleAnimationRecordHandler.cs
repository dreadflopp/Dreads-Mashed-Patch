using DreadsMashedPatch.AssetForwarding;
using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.IdleAnimation;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Filename/AnimationEvent select together. Legacy DATA is marked unused by xEdit,
    // so looping/flags/group/replay edits must not claim file-reference ownership.
    public class IdleAnimationRecordHandler : AbstractRecordHandler
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
            { "Conditions", new ConditionsHandler() },
            { "Filename", new SimpleReflectionAssetLinkPropertyHandler<SkyrimBehaviorAssetType, IIdleAnimation, IIdleAnimationGetter>("Filename") },
            { "AnimationEvent", new SimpleReflectionPropertyHandler<string?, IIdleAnimation, IIdleAnimationGetter>("AnimationEvent") },
            // RelatedIdles is deliberately excluded; runtime sorts these relations.
            { "LoopingSecondsMin", new SimpleReflectionPropertyHandler<byte, IIdleAnimation, IIdleAnimationGetter>("LoopingSecondsMin") },
            { "LoopingSecondsMax", new SimpleReflectionPropertyHandler<byte, IIdleAnimation, IIdleAnimationGetter>("LoopingSecondsMax") },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.IdleAnimation.Flag, IIdleAnimation, IIdleAnimationGetter>("Flags", preserveUnknownBits: true) },
            { "AnimationGroupSection", new SimpleReflectionPropertyHandler<byte, IIdleAnimation, IIdleAnimationGetter>("AnimationGroupSection") },
            { "ReplayDelay", new SimpleReflectionPropertyHandler<ushort, IIdleAnimation, IIdleAnimationGetter>("ReplayDelay") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IIdleAnimationGetter idleAnimation)
            {
                throw new InvalidOperationException($"Expected IIdleAnimationGetter but got {winningContext.Record.GetType()}");
            }

            return idleAnimation
                .ToLink<IIdleAnimationGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IIdleAnimation, IIdleAnimationGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
