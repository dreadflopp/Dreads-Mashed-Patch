using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Weapon;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: inherited EditorID and reflection-safe WEAP aggregate leaves use shared handlers.
    // - Kept specialized: model, script, destructible, description, flags, and the weapon-only mutually-exclusive type keyword rule.
    // - Intentionally excluded: Unused*, Data.Unused*, Critical.Unused*, and Data.Unknown* are outside the semantic conflict surface.
    // - Rationale: exact dotted registrations expose semantic leaves while preserving dedicated copy/flag behavior;
    //   only WEAP keywords need the configured type-family ownership rule, so other record keyword handlers stay generic.

    // Header migration: raw/common/Weapon.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class WeaponRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            // General properties (using existing handlers)
            { "EditorID", new EditorIDHandler() },
            { "Name", new TranslatedStringReflectionPropertyHandler<IWeapon, IWeaponGetter>("Name") },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Weapon.MajorFlag)) },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Icons", new IconsHandler() },
            { "Keywords", new WeaponKeywordListHandler() },
            // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
            { "VirtualMachineAdapter", new VirtualMachineAdapterHandler() },

            // Weapon-specific properties
            { "ObjectEffect", new SimpleReflectionFormLinkPropertyHandler<IEffectRecordGetter, IWeapon, IWeaponGetter>("ObjectEffect") },
            { "EnchantmentAmount", new SimpleReflectionPropertyHandler<ushort?, IWeapon, IWeaponGetter>("EnchantmentAmount") },
            { "Destructible", new DestructibleHandler() },
            { "EquipmentType", new SimpleReflectionFormLinkPropertyHandler<IEquipTypeGetter, IWeapon, IWeaponGetter>("EquipmentType") },
            { "BlockBashImpact", new SimpleReflectionFormLinkPropertyHandler<IImpactDataSetGetter, IWeapon, IWeaponGetter>("BlockBashImpact") },
            { "AlternateBlockMaterial", new SimpleReflectionFormLinkPropertyHandler<IMaterialTypeGetter, IWeapon, IWeaponGetter>("AlternateBlockMaterial") },
            { "PickUpSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IWeapon, IWeaponGetter>("PickUpSound") },
            { "PutDownSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IWeapon, IWeaponGetter>("PutDownSound") },
            { "Description", new DescriptionHandler() },
            { "ScopeModel", new ScopeModelHandler() },
            { "ImpactDataSet", new SimpleReflectionFormLinkPropertyHandler<IImpactDataSetGetter, IWeapon, IWeaponGetter>("ImpactDataSet") },
            { "FirstPersonModel", new SimpleReflectionFormLinkPropertyHandler<IStaticGetter, IWeapon, IWeaponGetter>("FirstPersonModel") },
            { "AttackSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IWeapon, IWeaponGetter>("AttackSound") },
            { "AttackSound2D", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IWeapon, IWeaponGetter>("AttackSound2D") },
            { "AttackLoopSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IWeapon, IWeaponGetter>("AttackLoopSound") },
            { "AttackFailSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IWeapon, IWeaponGetter>("AttackFailSound") },
            { "IdleSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IWeapon, IWeaponGetter>("IdleSound") },
            { "EquipSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IWeapon, IWeaponGetter>("EquipSound") },
            { "UnequipSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IWeapon, IWeaponGetter>("UnequipSound") },
            { "BasicStats.Value", new ValueHandler() },
            { "BasicStats.Weight", new WeightHandler() },
            { "BasicStats.Damage", new SimpleReflectionPropertyHandler<ushort, IWeapon, IWeaponGetter>("BasicStats.Damage") },
            { "DetectionSoundLevel", new SimpleReflectionPropertyHandler<SoundLevel?, IWeapon, IWeaponGetter>("DetectionSoundLevel") },
            { "Template", new SimpleReflectionFormLinkPropertyHandler<IWeaponGetter, IWeapon, IWeaponGetter>("Template") },
            { "Data.AnimationType", new SimpleReflectionPropertyHandler<WeaponAnimationType, IWeapon, IWeaponGetter>("Data.AnimationType") },
            { "Data.Speed", new SimpleReflectionPropertyHandler<float, IWeapon, IWeaponGetter>("Data.Speed") },
            { "Data.Reach", new SimpleReflectionPropertyHandler<float, IWeapon, IWeaponGetter>("Data.Reach") },
            { "Data.Flags", new SimpleReflectionFlagPropertyHandler<WeaponData.Flag, IWeapon, IWeaponGetter>("Data.Flags", preserveUnknownBits: true) },
            { "Data.SightFOV", new SimpleReflectionPropertyHandler<float, IWeapon, IWeaponGetter>("Data.SightFOV") },
            { "Data.BaseVATStoHitChance", new SimpleReflectionPropertyHandler<byte, IWeapon, IWeaponGetter>("Data.BaseVATStoHitChance") },
            { "Data.AttackAnimation", new SimpleReflectionPropertyHandler<WeaponData.AttackAnimationType, IWeapon, IWeaponGetter>("Data.AttackAnimation") },
            { "Data.NumProjectiles", new SimpleReflectionPropertyHandler<byte, IWeapon, IWeaponGetter>("Data.NumProjectiles") },
            { "Data.EmbeddedWeaponAV", new SimpleReflectionPropertyHandler<byte, IWeapon, IWeaponGetter>("Data.EmbeddedWeaponAV") },
            { "Data.RangeMin", new SimpleReflectionPropertyHandler<float, IWeapon, IWeaponGetter>("Data.RangeMin") },
            { "Data.RangeMax", new SimpleReflectionPropertyHandler<float, IWeapon, IWeaponGetter>("Data.RangeMax") },
            { "Data.OnHit", new SimpleReflectionPropertyHandler<WeaponData.OnHitType, IWeapon, IWeaponGetter>("Data.OnHit") },
            { "Data.AnimationAttackMult", new SimpleReflectionPropertyHandler<float, IWeapon, IWeaponGetter>("Data.AnimationAttackMult") },
            { "Data.RumbleLeftMotorStrength", new SimpleReflectionPropertyHandler<float, IWeapon, IWeaponGetter>("Data.RumbleLeftMotorStrength") },
            { "Data.RumbleRightMotorStrength", new SimpleReflectionPropertyHandler<float, IWeapon, IWeaponGetter>("Data.RumbleRightMotorStrength") },
            { "Data.RumbleDuration", new SimpleReflectionPropertyHandler<float, IWeapon, IWeaponGetter>("Data.RumbleDuration") },
            { "Data.Skill", new SimpleReflectionPropertyHandler<Skill?, IWeapon, IWeaponGetter>("Data.Skill") },
            { "Data.Resist", new SimpleReflectionPropertyHandler<ActorValue, IWeapon, IWeaponGetter>("Data.Resist") },
            { "Data.Stagger", new SimpleReflectionPropertyHandler<float, IWeapon, IWeaponGetter>("Data.Stagger") },
            // CriticalData
            { "Critical.Damage", new SimpleReflectionPropertyHandler<ushort, IWeapon, IWeaponGetter>("Critical.Damage") },
            { "Critical.PercentMult", new SimpleReflectionPropertyHandler<float, IWeapon, IWeaponGetter>("Critical.PercentMult") },
            { "Critical.Flags", new SimpleReflectionFlagPropertyHandler<CriticalData.Flag, IWeapon, IWeaponGetter>("Critical.Flags", preserveUnknownBits: true) },
            { "Critical.Effect", new CriticalEffectHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IWeaponGetter weaponRecord)
            {
                throw new InvalidOperationException($"Expected IWeaponGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = weaponRecord
                .ToLink<IWeaponGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IWeapon, IWeaponGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }

        // CommitOverride and ApplyForwardedProperties are now handled by the base class
        // The base class automatically handles flag property coordination
    }
}
