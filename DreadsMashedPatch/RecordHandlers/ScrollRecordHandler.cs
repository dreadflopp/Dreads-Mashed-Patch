using DreadsMashedPatch.AssetForwarding;
using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Scroll;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// xEdit gives outer Effects entries no stable key; each declaration position is one atomic row.
public class ScrollRecordHandler : AbstractRecordHandler
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
        { "Name", new TranslatedStringReflectionPropertyHandler<IScroll, IScrollGetter>("Name") },
        { "Keywords", new KeywordListHandler() },
        { "MenuDisplayObject", new SimpleReflectionFormLinkPropertyHandler<IStaticGetter, IScroll, IScrollGetter>("MenuDisplayObject") },
        { "EquipmentType", new SimpleReflectionFormLinkPropertyHandler<IEquipTypeGetter, IScroll, IScrollGetter>("EquipmentType") },
        { "Description", new TranslatedStringReflectionPropertyHandler<IScroll, IScrollGetter>("Description") },
        { "ModelAndBounds", new ModelBoundsHandler() },
        { "Destructible", DestructibleDataHandlers.Data(new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, IScroll, IScrollGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals)) },
        { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, IScroll, IScrollGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals)) },
        { "PickUpSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IScroll, IScrollGetter>("PickUpSound") },
        { "PutDownSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IScroll, IScrollGetter>("PutDownSound") },
        { "Value", new SimpleReflectionPropertyHandler<uint, IScroll, IScrollGetter>("Value") },
        { "Weight", new SimpleReflectionPropertyHandler<float, IScroll, IScrollGetter>("Weight") },
        { "BaseCost", new SimpleReflectionPropertyHandler<uint, IScroll, IScrollGetter>("BaseCost") },
        { "Flags", new SimpleReflectionFlagPropertyHandler<SpellDataFlag, IScroll, IScrollGetter>("Flags") },
        { "Type", new SimpleReflectionPropertyHandler<SpellType, IScroll, IScrollGetter>("Type") },
        { "ChargeTime", new SimpleReflectionPropertyHandler<float, IScroll, IScrollGetter>("ChargeTime", 0.001f) },
        { "CastType", new SimpleReflectionPropertyHandler<CastType, IScroll, IScrollGetter>("CastType") },
        { "TargetType", new SimpleReflectionPropertyHandler<TargetType, IScroll, IScrollGetter>("TargetType") },
        { "CastDuration", new SimpleReflectionPropertyHandler<float, IScroll, IScrollGetter>("CastDuration", 0.001f) },
        { "Range", new SimpleReflectionPropertyHandler<float, IScroll, IScrollGetter>("Range", 0.001f) },
        { "HalfCostPerk", new SimpleReflectionFormLinkPropertyHandler<IPerkGetter, IScroll, IScrollGetter>("HalfCostPerk") },
        { "Effects", new EffectsHandler() }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IScrollGetter scrollRecord)
        {
            throw new InvalidOperationException($"Expected IScrollGetter but got {winningContext.Record.GetType()}");
        }

        return scrollRecord
            .ToLink<IScrollGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IScroll, IScrollGetter>(state.LinkCache)
            .ToArray();
    }
}
