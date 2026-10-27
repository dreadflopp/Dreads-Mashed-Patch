using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using DreadsMashedPatch.PropertyHandlers.Spell;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // xEdit gives outer Effects entries no stable key; each declaration position is one atomic row.
    public class SpellRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<ISpell, ISpellGetter>("Name") },
            { "ObjectBounds", new ObjectBoundsHandler() },
            { "MenuDisplayObject", new SimpleReflectionFormLinkPropertyHandler<IStaticGetter, ISpell, ISpellGetter>("MenuDisplayObject") },
            { "Description", new DescriptionHandler() },
            { "Flags", new FlagsHandler() },
            { "Keywords", new KeywordListHandler() },
            { "EquipmentType", new SimpleReflectionFormLinkPropertyHandler<IEquipTypeGetter, ISpell, ISpellGetter>("EquipmentType") },
            { "BaseCost", new SimpleReflectionPropertyHandler<uint, ISpell, ISpellGetter>("BaseCost") },
            { "Type", new SimpleReflectionPropertyHandler<SpellType, ISpell, ISpellGetter>("Type") },
            { "ChargeTime", new SimpleReflectionPropertyHandler<float, ISpell, ISpellGetter>("ChargeTime", 0.001f) },
            { "CastType", new SimpleReflectionPropertyHandler<CastType, ISpell, ISpellGetter>("CastType") },
            { "TargetType", new SimpleReflectionPropertyHandler<TargetType, ISpell, ISpellGetter>("TargetType") },
            { "CastDuration", new SimpleReflectionPropertyHandler<float, ISpell, ISpellGetter>("CastDuration", 0.001f) },
            { "Range", new SimpleReflectionPropertyHandler<float, ISpell, ISpellGetter>("Range", 0.001f) },
            { "HalfCostPerk", new SimpleReflectionFormLinkPropertyHandler<IPerkGetter, ISpell, ISpellGetter>("HalfCostPerk") },
            { "Effects", new EffectsHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ISpellGetter spellRecord)
            {
                throw new InvalidOperationException($"Expected ISpellGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = spellRecord
                .ToLink<ISpellGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ISpell, ISpellGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
