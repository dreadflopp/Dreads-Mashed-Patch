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

// Effects migration note: generalized reconciliation to the shared exact-position
// atomic handler; scroll collection access stays specialized because xEdit gives
// the outer Effects entries no stable row key.

// Header migration: raw/common flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
// Name migration: translated Name uses generated copying to retain every selected language.
// Optional null names remove the value; comparison follows Mutagen's language policy.
// Other specialized fields/flags retain their policies; translations are selected as one value.
public class ScrollRecordHandler : AbstractRecordHandler
{
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
        { "Destructible", new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, IScroll, IScrollGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals) },
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
