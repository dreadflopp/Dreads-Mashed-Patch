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
public class MoveableStaticRecordHandler : AbstractRecordHandler
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
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(MoveableStatic.MajorFlag)) },
        { "Name", new TranslatedStringReflectionPropertyHandler<IMoveableStatic, IMoveableStaticGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
        { "Destructible", DestructibleDataHandlers.Data(new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, IMoveableStatic, IMoveableStaticGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals)) },
        { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, IMoveableStatic, IMoveableStaticGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals)) },
        { "Flags", new SimpleReflectionFlagPropertyHandler<MoveableStatic.Flag, IMoveableStatic, IMoveableStaticGetter>("Flags") },
        { "LoopingSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IMoveableStatic, IMoveableStaticGetter>("LoopingSound") },

    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IMoveableStaticGetter moveableStaticRecord)
        {
            throw new InvalidOperationException($"Expected IMoveableStaticGetter but got {winningContext.Record.GetType()}");
        }

        return moveableStaticRecord
            .ToLink<IMoveableStaticGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter>(state.LinkCache)
            .ToArray();
    }
}
