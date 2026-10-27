using DreadsMashedPatch.AssetForwarding;
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
    // Race, Female, weight and skin stay with the face to preserve its interpretation.
    // Height, HairColor and OppositeGenderAnims have separate owners; their setters must preserve face fields.
    // TemplateAndTraits selects the template pointer and Traits bit together. Inventory uses the shared
    // NPC/CONT snapshots, with NPC-specific ownership checks for both count and COED edits.
    public class NpcRecordHandler : AbstractRecordHandler
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
            { "Appearance", new NpcAppearanceHandler() },
            { "HairColor", new SimpleReflectionFormLinkPropertyHandler<IColorRecordGetter, INpc, INpcGetter>("HairColor") },
            { "Height", new SimpleReflectionPropertyHandler<float, INpc, INpcGetter>("Height") },
            // The key identifies the bit's context; reflection reads/writes its containing enum field.
            { "Configuration.Flags.OppositeGenderAnims", new SimpleReflectionFlagPropertyHandler<NpcConfiguration.Flag, INpc, INpcGetter>(
                "Configuration.Flags", preserveUnknownBits: true, includedFlags: [NpcConfiguration.Flag.OppositeGenderAnims]) },
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
            { "Destructible", DestructibleDataHandlers.Data(new DestructibleHandler()) },
            { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new DestructibleHandler()) },
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
    }
}
