using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using DreadsMashedPatch.PropertyHandlers.Npc;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: semantic AIData, Configuration, and scalar PlayerSkills leaves use exact dotted shared handlers.
    // - Appearance migration: complete atomic facial/body snapshots use ordinary ownership/reversion forwarding.
    //   Generated masked copying retains nullable subrecords, order, duplicates and overlay assets.
    //   Female/OppositeGenderAnims and Traits use approved masked flag setters; protection and other template bits remain specialized.
    //   Race, height, weight and skin stay with the face because they control its interpretation.
    //   TemplateAndTraits selects the pointer and Traits bit atomically through the same shared algorithm.
    //   Sorted head/tint collections compare without order changes claiming ownership; copies retain source order.
    //   Other NPC gameplay properties retain their existing handlers.
    // - Generalized: Items' complete generated snapshots, duplicate matching, and COED reconciliation share AbstractInventoryItemsHandler with CONT.
    // - Kept specialized: inventory count/COED edits retain NPC's ownership permission checks; only the Items surface adapter is record-specific.
    // - Specialized: polymorphic Level, protection policy, skill dictionaries, float tolerance, remaining NPC collection merging, and typed attacks remain explicit.
    // - Intentionally excluded: AIData.Unused and PlayerSkills.Unused* are serialization-only fields.
    // - Rationale: Level uses generated subtype dispatch; unused storage is not an xEdit-visible conflict surface.

    // Header migration: raw/common/Npc.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    // Output migration: shared staged context insertion replaces the separate NPC group path.
    // NPC property/flag/ownership policies stay specialized; insertion needs no NPC exception.
    public class NpcRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "Appearance", new NpcAppearanceHandler() },
            { "TemplateAndTraits", new TemplateAndTraitsHandler() },
            { "Name", new TranslatedStringReflectionPropertyHandler<INpc, INpcGetter>("Name") },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Npc.MajorFlag)) },
            { "DeathItem", new SimpleReflectionFormLinkPropertyHandler<ILeveledItemGetter, INpc, INpcGetter>("DeathItem") },
            { "CombatOverridePackageList", new SimpleReflectionFormLinkPropertyHandler<IFormListGetter, INpc, INpcGetter>("CombatOverridePackageList") },
            { "SpectatorOverridePackageList", new SimpleReflectionFormLinkPropertyHandler<IFormListGetter, INpc, INpcGetter>("SpectatorOverridePackageList") },
            { "Configuration.Flags", new ProtectionFlagsHandler() },
            { "Configuration.MagickaOffset", new SimpleReflectionPropertyHandler<short, INpc, INpcGetter>("Configuration.MagickaOffset") },
            { "Configuration.StaminaOffset", new SimpleReflectionPropertyHandler<short, INpc, INpcGetter>("Configuration.StaminaOffset") },
            { "Configuration.Level", new NpcLevelHandler() },
            { "Configuration.CalcMinLevel", new SimpleReflectionPropertyHandler<short, INpc, INpcGetter>("Configuration.CalcMinLevel") },
            { "Configuration.CalcMaxLevel", new SimpleReflectionPropertyHandler<short, INpc, INpcGetter>("Configuration.CalcMaxLevel") },
            { "Configuration.SpeedMultiplier", new SimpleReflectionPropertyHandler<short, INpc, INpcGetter>("Configuration.SpeedMultiplier") },
            { "Configuration.DispositionBase", new SimpleReflectionPropertyHandler<short, INpc, INpcGetter>("Configuration.DispositionBase") },
            { "Configuration.TemplateFlags", new SimpleReflectionFlagPropertyHandler<NpcConfiguration.TemplateFlag, INpc, INpcGetter>("Configuration.TemplateFlags", preserveUnknownBits: true,
                includedFlags: Enum.GetValues<NpcConfiguration.TemplateFlag>().Where(flag => flag != NpcConfiguration.TemplateFlag.Traits)) },
            { "Configuration.HealthOffset", new SimpleReflectionPropertyHandler<short, INpc, INpcGetter>("Configuration.HealthOffset") },
            { "Configuration.BleedoutOverride", new SimpleReflectionPropertyHandler<short, INpc, INpcGetter>("Configuration.BleedoutOverride") },
            { "EditorID", new EditorIDHandler() },
            { "Class", new SimpleReflectionFormLinkPropertyHandler<IClassGetter, INpc, INpcGetter>("Class") },
            { "AIData.Aggression", new SimpleReflectionPropertyHandler<Aggression, INpc, INpcGetter>("AIData.Aggression") },
            { "AIData.Confidence", new SimpleReflectionPropertyHandler<Confidence, INpc, INpcGetter>("AIData.Confidence") },
            { "AIData.EnergyLevel", new SimpleReflectionPropertyHandler<byte, INpc, INpcGetter>("AIData.EnergyLevel") },
            { "AIData.Responsibility", new SimpleReflectionPropertyHandler<Responsibility, INpc, INpcGetter>("AIData.Responsibility") },
            { "AIData.Mood", new SimpleReflectionPropertyHandler<Mood, INpc, INpcGetter>("AIData.Mood") },
            { "AIData.Assistance", new SimpleReflectionPropertyHandler<Assistance, INpc, INpcGetter>("AIData.Assistance") },
            { "AIData.AggroRadiusBehavior", new SimpleReflectionPropertyHandler<bool, INpc, INpcGetter>("AIData.AggroRadiusBehavior") },
            { "AIData.Warn", new SimpleReflectionPropertyHandler<uint, INpc, INpcGetter>("AIData.Warn") },
            { "AIData.WarnOrAttack", new SimpleReflectionPropertyHandler<uint, INpc, INpcGetter>("AIData.WarnOrAttack") },
            { "AIData.Attack", new SimpleReflectionPropertyHandler<uint, INpc, INpcGetter>("AIData.Attack") },
            { "ObserveDeadBodyOverridePackageList", new SimpleReflectionFormLinkPropertyHandler<IFormListGetter, INpc, INpcGetter>("ObserveDeadBodyOverridePackageList") },
            { "Factions", new FactionHandler() },
            { "Packages", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IPackageGetter>, INpc, INpcGetter>("Packages", ListSemantics.AlignedOrdered) },
            { "ActorEffect", new SimpleReflectionListPropertyHandler<IFormLinkGetter<ISpellRecordGetter>, INpc, INpcGetter>("ActorEffect", ListSemantics.SortedKeyed) },
            // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
            { "VirtualMachineAdapter", new VirtualMachineAdapterHandler() },
            { "Items", new ItemHandler() },
            { "Keywords", new KeywordListHandler() },
            { "PlayerSkills.Health", new SimpleReflectionPropertyHandler<ushort, INpc, INpcGetter>("PlayerSkills.Health") },
            { "PlayerSkills.Magicka", new SimpleReflectionPropertyHandler<ushort, INpc, INpcGetter>("PlayerSkills.Magicka") },
            { "PlayerSkills.Stamina", new SimpleReflectionPropertyHandler<ushort, INpc, INpcGetter>("PlayerSkills.Stamina") },
            { "PlayerSkills.FarAwayModelDistance", new PlayerSkillsFarAwayModelDistanceHandler() },
            { "PlayerSkills.GearedUpWeapons", new SimpleReflectionPropertyHandler<byte, INpc, INpcGetter>("PlayerSkills.GearedUpWeapons") },
            { "PlayerSkills.SkillValues", new PlayerSkillsSkillValuesHandler() },
            { "PlayerSkills.SkillOffsets", new PlayerSkillsSkillOffsetsHandler() },
            { "Destructible", new DestructibleHandler() },
            { "ObjectBounds", new ObjectBoundsHandler() },
            { "Voice", new SimpleReflectionFormLinkPropertyHandler<IVoiceTypeGetter, INpc, INpcGetter>("Voice") },
            { "ShortName", new TranslatedStringReflectionPropertyHandler<INpc, INpcGetter>("ShortName") },
            { "NAM5", new SimpleReflectionPropertyHandler<ushort, INpc, INpcGetter>("NAM5") },
            { "SoundLevel", new SimpleReflectionPropertyHandler<SoundLevel, INpc, INpcGetter>("SoundLevel") },
            { "AttackRace", new SimpleReflectionFormLinkPropertyHandler<IRaceGetter, INpc, INpcGetter>("AttackRace") },
            { "DefaultOutfit", new SimpleReflectionFormLinkPropertyHandler<IOutfitGetter, INpc, INpcGetter>("DefaultOutfit") },
            { "FarAwayModel", new SimpleReflectionFormLinkPropertyHandler<IArmorGetter, INpc, INpcGetter>("FarAwayModel") },
            { "Attacks", new AttacksHandler() },
            { "GuardWarnOverridePackageList", new SimpleReflectionFormLinkPropertyHandler<IFormListGetter, INpc, INpcGetter>("GuardWarnOverridePackageList") },
            { "Perks", new PerksHandler() },
            { "CombatStyle", new SimpleReflectionFormLinkPropertyHandler<ICombatStyleGetter, INpc, INpcGetter>("CombatStyle") },
            { "GiftFilter", new SimpleReflectionFormLinkPropertyHandler<IFormListGetter, INpc, INpcGetter>("GiftFilter") },
            { "SleepingOutfit", new SimpleReflectionFormLinkPropertyHandler<IOutfitGetter, INpc, INpcGetter>("SleepingOutfit") },
            { "DefaultPackageList", new SimpleReflectionFormLinkPropertyHandler<IFormListGetter, INpc, INpcGetter>("DefaultPackageList") },
            { "CrimeFaction", new SimpleReflectionFormLinkPropertyHandler<IFactionGetter, INpc, INpcGetter>("CrimeFaction") },
            { "Sound", new SoundHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            return winningContext.Record
                .ToLink<INpcGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INpc, INpcGetter>(state.LinkCache)
                .ToArray();
        }

        // ApplyForwardedProperties is now handled by the base class
        // The base class automatically handles flag property coordination
    }
}
