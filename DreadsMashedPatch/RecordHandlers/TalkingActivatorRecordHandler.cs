using DreadsMashedPatch.AssetForwarding;
using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// PNAM and FNAM are cpIgnore in xEdit; they are not independently conflict-resolved.
public class TalkingActivatorRecordHandler : AbstractRecordHandler
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
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(TalkingActivator.MajorFlag)) },
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<ITalkingActivator, ITalkingActivatorGetter>() },
        { "Name", new TranslatedStringReflectionPropertyHandler<ITalkingActivator, ITalkingActivatorGetter>("Name") },
        { "ModelAndBounds", new ModelBoundsHandler() },
        { "Destructible", DestructibleDataHandlers.Data(new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, ITalkingActivator, ITalkingActivatorGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals)) },
        { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, ITalkingActivator, ITalkingActivatorGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals)) },
        { "Keywords", new KeywordListHandler() },
        { "LoopingSound", new SimpleReflectionFormLinkPropertyHandler<ISoundMarkerGetter, ITalkingActivator, ITalkingActivatorGetter>("LoopingSound") },
        { "Voice", new SimpleReflectionFormLinkPropertyHandler<IVoiceTypeGetter, ITalkingActivator, ITalkingActivatorGetter>("Voice") }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not ITalkingActivatorGetter talkingActivatorRecord)
        {
            throw new InvalidOperationException($"Expected ITalkingActivatorGetter but got {winningContext.Record.GetType()}");
        }

        return talkingActivatorRecord
            .ToLink<ITalkingActivatorGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter>(state.LinkCache)
            .ToArray();
    }
}
