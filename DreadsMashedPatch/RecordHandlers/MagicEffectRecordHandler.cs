using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.MagicEffect;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: semantic MGEF scalar/link fields and sound rows use shared handlers with generated copies.
    // - Kept specialized: flags, conditions, and polymorphic archetype copying/equality.
    // - Intentionally excluded: Unknown1 is outside the semantic conflict surface.
    // - Rationale: the winning override retains excluded engine-managed data.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class MagicEffectRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IMagicEffect, IMagicEffectGetter>("Name") },
            // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
            { "VirtualMachineAdapter", new VirtualMachineAdapterHandler() },
            { "Description", new TranslatedStringReflectionPropertyHandler<IMagicEffect, IMagicEffectGetter>("Description") },
            { "BaseCost", new SimpleReflectionPropertyHandler<float, IMagicEffect, IMagicEffectGetter>("BaseCost") },
            { "Flags", new FlagsHandler() },
            { "CastType", new SimpleReflectionPropertyHandler<CastType, IMagicEffect, IMagicEffectGetter>("CastType") },
            { "TargetType", new SimpleReflectionPropertyHandler<TargetType, IMagicEffect, IMagicEffectGetter>("TargetType") },
            { "MagicSkill", new SimpleReflectionPropertyHandler<ActorValue, IMagicEffect, IMagicEffectGetter>("MagicSkill") },
            { "ResistValue", new SimpleReflectionPropertyHandler<ActorValue, IMagicEffect, IMagicEffectGetter>("ResistValue") },
            { "SecondActorValue", new SimpleReflectionPropertyHandler<ActorValue, IMagicEffect, IMagicEffectGetter>("SecondActorValue") },
            { "CastingSoundLevel", new SimpleReflectionPropertyHandler<SoundLevel, IMagicEffect, IMagicEffectGetter>("CastingSoundLevel") },
            { "MenuDisplayObject", new SimpleReflectionFormLinkPropertyHandler<IStaticGetter, IMagicEffect, IMagicEffectGetter>("MenuDisplayObject") },
            { "Keywords", new KeywordListHandler() },
            { "CastingLight", new SimpleReflectionFormLinkPropertyHandler<ILightGetter, IMagicEffect, IMagicEffectGetter>("CastingLight") },
            { "HitShader", new SimpleReflectionFormLinkPropertyHandler<IEffectShaderGetter, IMagicEffect, IMagicEffectGetter>("HitShader") },
            { "EnchantShader", new SimpleReflectionFormLinkPropertyHandler<IEffectShaderGetter, IMagicEffect, IMagicEffectGetter>("EnchantShader") },
            { "Projectile", new SimpleReflectionFormLinkPropertyHandler<IProjectileGetter, IMagicEffect, IMagicEffectGetter>("Projectile") },
            { "Explosion", new SimpleReflectionFormLinkPropertyHandler<IExplosionGetter, IMagicEffect, IMagicEffectGetter>("Explosion") },
            { "CastingArt", new SimpleReflectionFormLinkPropertyHandler<IArtObjectGetter, IMagicEffect, IMagicEffectGetter>("CastingArt") },
            { "HitEffectArt", new SimpleReflectionFormLinkPropertyHandler<IArtObjectGetter, IMagicEffect, IMagicEffectGetter>("HitEffectArt") },
            { "ImpactData", new SimpleReflectionFormLinkPropertyHandler<IImpactDataSetGetter, IMagicEffect, IMagicEffectGetter>("ImpactData") },
            { "DualCastArt", new SimpleReflectionFormLinkPropertyHandler<IDualCastDataGetter, IMagicEffect, IMagicEffectGetter>("DualCastArt") },
            { "EnchantArt", new SimpleReflectionFormLinkPropertyHandler<IArtObjectGetter, IMagicEffect, IMagicEffectGetter>("EnchantArt") },
            { "HitVisuals", new SimpleReflectionFormLinkPropertyHandler<IVisualEffectGetter, IMagicEffect, IMagicEffectGetter>("HitVisuals") },
            { "EnchantVisuals", new SimpleReflectionFormLinkPropertyHandler<IVisualEffectGetter, IMagicEffect, IMagicEffectGetter>("EnchantVisuals") },
            { "EquipAbility", new SimpleReflectionFormLinkPropertyHandler<ISpellGetter, IMagicEffect, IMagicEffectGetter>("EquipAbility") },
            { "ImageSpaceModifier", new SimpleReflectionFormLinkPropertyHandler<IImageSpaceAdapterGetter, IMagicEffect, IMagicEffectGetter>("ImageSpaceModifier") },
            { "PerkToApply", new SimpleReflectionFormLinkPropertyHandler<IPerkGetter, IMagicEffect, IMagicEffectGetter>("PerkToApply") },
            { "TaperWeight", new SimpleReflectionPropertyHandler<float, IMagicEffect, IMagicEffectGetter>("TaperWeight") },
            { "MinimumSkillLevel", new SimpleReflectionPropertyHandler<uint, IMagicEffect, IMagicEffectGetter>("MinimumSkillLevel") },
            { "SpellmakingArea", new SimpleReflectionPropertyHandler<uint, IMagicEffect, IMagicEffectGetter>("SpellmakingArea") },
            { "SpellmakingCastingTime", new SimpleReflectionPropertyHandler<float, IMagicEffect, IMagicEffectGetter>("SpellmakingCastingTime") },
            { "TaperCurve", new SimpleReflectionPropertyHandler<float, IMagicEffect, IMagicEffectGetter>("TaperCurve") },
            { "TaperDuration", new SimpleReflectionPropertyHandler<float, IMagicEffect, IMagicEffectGetter>("TaperDuration") },
            { "SecondActorValueWeight", new SimpleReflectionPropertyHandler<float, IMagicEffect, IMagicEffectGetter>("SecondActorValueWeight") },
            { "SkillUsageMultiplier", new SimpleReflectionPropertyHandler<float, IMagicEffect, IMagicEffectGetter>("SkillUsageMultiplier") },
            { "DualCastScale", new SimpleReflectionPropertyHandler<float, IMagicEffect, IMagicEffectGetter>("DualCastScale") },
            { "ScriptEffectAIScore", new SimpleReflectionPropertyHandler<float, IMagicEffect, IMagicEffectGetter>("ScriptEffectAIScore") },
            { "ScriptEffectAIDelayTime", new SimpleReflectionPropertyHandler<float, IMagicEffect, IMagicEffectGetter>("ScriptEffectAIDelayTime") },
            { "CounterEffects", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IMagicEffectGetter>, IMagicEffect, IMagicEffectGetter>("CounterEffects", ListSemantics.SortedKeyed) },
            { "Sounds", new GeneratedCopyReflectionListPropertyHandler<IMagicEffectSoundGetter, MagicEffectSound, IMagicEffect, IMagicEffectGetter>(
                "Sounds", ListSemantics.SortedKeyed, value => value.DeepCopy(), MagicEffectSoundMixIn.Equals,
                keySelector: sound => sound.Type) },
            { "Archetype", new ArchetypeHandler() },
            { "Conditions", new ConditionsHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IMagicEffectGetter magicEffectRecord)
            {
                throw new InvalidOperationException($"Expected IMagicEffectGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = magicEffectRecord
                .ToLink<IMagicEffectGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IMagicEffect, IMagicEffectGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
