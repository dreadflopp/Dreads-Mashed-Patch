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
public class SoulGemRecordHandler : AbstractRecordHandler
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
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(SoulGem.MajorFlag)) },
        { "Name", new TranslatedStringReflectionPropertyHandler<ISoulGem, ISoulGemGetter>("Name") },
        { "ModelAndBounds", new ModelBoundsHandler() },
        { "Icons", new SimpleReflectionIconsPropertyHandler<ISoulGem, ISoulGemGetter>("Icons") },
        { "Destructible", DestructibleDataHandlers.Data(new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, ISoulGem, ISoulGemGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals)) },
        { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, ISoulGem, ISoulGemGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals)) },
        { "PickUpSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, ISoulGem, ISoulGemGetter>("PickUpSound") },
        { "PutDownSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, ISoulGem, ISoulGemGetter>("PutDownSound") },
        { "Keywords", new KeywordListHandler() },
        { "Value", new ValueHandler() },
        { "Weight", new WeightHandler() },
        { "ContainedSoul", new SimpleReflectionPropertyHandler<SoulGem.Level, ISoulGem, ISoulGemGetter>("ContainedSoul") },
        { "MaximumCapacity", new SimpleReflectionPropertyHandler<SoulGem.Level, ISoulGem, ISoulGemGetter>("MaximumCapacity") },
        { "LinkedTo", new SimpleReflectionFormLinkPropertyHandler<ISoulGemGetter, ISoulGem, ISoulGemGetter>("LinkedTo") },

    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not ISoulGemGetter soulGemRecord)
        {
            throw new InvalidOperationException($"Expected ISoulGemGetter but got {winningContext.Record.GetType()}");
        }

        return soulGemRecord
            .ToLink<ISoulGemGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ISoulGem, ISoulGemGetter>(state.LinkCache)
            .ToArray();
    }
}
