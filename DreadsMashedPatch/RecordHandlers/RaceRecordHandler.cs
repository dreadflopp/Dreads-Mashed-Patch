using DreadsMashedPatch.AssetForwarding;
using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Aspects;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Race;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Head shape keeps parts/morph definitions and clamps together; tint count travels with both lists.
// Parent/gender presence gates child writes, and typed masked copies preserve sibling selections.
// Each gender owns a skeleton/behavior pair; combat and movement edits cannot claim rig ownership.
// Skill boosts use xEdit Skill keys rather than physical slots; AttackData needs generated overlay copies.
public class RaceRecordHandler : AbstractRecordHandler
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
        { "HeadData.Presence", new RaceHeadDataPresenceHandler() },
        { "HeadData.Male.Shape", new RaceHeadDataHandler(MaleFemaleGender.Male, RaceHeadDataPart.Shape) },
        { "HeadData.Female.Shape", new RaceHeadDataHandler(MaleFemaleGender.Female, RaceHeadDataPart.Shape) },
        { "HeadData.Male.Mesh", new RaceHeadDataHandler(MaleFemaleGender.Male, RaceHeadDataPart.Mesh) },
        { "HeadData.Female.Mesh", new RaceHeadDataHandler(MaleFemaleGender.Female, RaceHeadDataPart.Mesh) },
        { "HeadData.Male.FaceTextures", new RaceHeadDataHandler(MaleFemaleGender.Male, RaceHeadDataPart.FaceTextures) },
        { "HeadData.Female.FaceTextures", new RaceHeadDataHandler(MaleFemaleGender.Female, RaceHeadDataPart.FaceTextures) },
        { "HeadData.Male.Tints", new RaceHeadDataHandler(MaleFemaleGender.Male, RaceHeadDataPart.Tints) },
        { "HeadData.Female.Tints", new RaceHeadDataHandler(MaleFemaleGender.Female, RaceHeadDataPart.Tints) },
        { "HeadData.Male.RacePresets", new RaceHeadChoicesHandler<INpcGetter>(MaleFemaleGender.Male, "RacePresets", h => h.RacePresets, h => h.RacePresets) },
        { "HeadData.Female.RacePresets", new RaceHeadChoicesHandler<INpcGetter>(MaleFemaleGender.Female, "RacePresets", h => h.RacePresets, h => h.RacePresets) },
        { "HeadData.Male.AvailableHairColors", new RaceHeadChoicesHandler<IColorRecordGetter>(MaleFemaleGender.Male, "AvailableHairColors", h => h.AvailableHairColors, h => h.AvailableHairColors) },
        { "HeadData.Female.AvailableHairColors", new RaceHeadChoicesHandler<IColorRecordGetter>(MaleFemaleGender.Female, "AvailableHairColors", h => h.AvailableHairColors, h => h.AvailableHairColors) },
        { "MorphRace", new SimpleReflectionFormLinkPropertyHandler<IRaceGetter, IRace, IRaceGetter>("MorphRace") },
        { "NumberOfTintsInList", new SimpleReflectionPropertyHandler<ushort?, IRace, IRaceGetter>("NumberOfTintsInList") },
        { "FacegenMainClamp", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("FacegenMainClamp") },
        { "FacegenFaceClamp", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("FacegenFaceClamp") },
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Race.MajorFlag)) },
        { "Name", new TranslatedStringReflectionPropertyHandler<IRace, IRaceGetter>("Name") },
        { "Description", new TranslatedStringReflectionPropertyHandler<IRace, IRaceGetter>("Description") },
        { "ActorEffect", new SimpleReflectionListPropertyHandler<IFormLinkGetter<ISpellRecordGetter>, IRace, IRaceGetter>("ActorEffect", ListSemantics.SortedKeyed) },
        { "Skin", new SimpleReflectionFormLinkPropertyHandler<IArmorGetter, IRace, IRaceGetter>("Skin") },
        { "BodyTemplate", new GeneratedCopyReflectionPropertyHandler<IBodyTemplateGetter, BodyTemplate, IRace, IRaceGetter>(
            "BodyTemplate", value => value.DeepCopy(), BodyTemplateMixIn.Equals) },
        { "Keywords", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IKeywordGetter>, IRace, IRaceGetter>("Keywords", ListSemantics.SortedKeyed) },
        { "SkillBoosts", new RaceSkillBoostsHandler() },
        { "Height", new GenderedItemHandler<float, float, IRace, IRaceGetter>("Height", record => record.Height, (record, value) => { if (value != null) record.Height = value; }, value => value) },
        { "Weight", new GenderedItemHandler<float, float, IRace, IRaceGetter>("Weight", record => record.Weight, (record, value) => { if (value != null) record.Weight = value; }, value => value) },
        { "Flags", new SimpleReflectionFlagPropertyHandler<Race.Flag, IRace, IRaceGetter>("Flags", preserveUnknownBits: true) },
        { "Starting", new RaceStartingHandler() },
        { "BaseCarryWeight", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("BaseCarryWeight") },
        { "BaseMass", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("BaseMass") },
        { "AccelerationRate", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("AccelerationRate") },
        { "DecelerationRate", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("DecelerationRate") },
        { "Size", new SimpleReflectionPropertyHandler<Size, IRace, IRaceGetter>("Size") },
        { "HeadBipedObject", new SimpleReflectionPropertyHandler<BipedObject, IRace, IRaceGetter>("HeadBipedObject") },
        { "HairBipedObject", new SimpleReflectionPropertyHandler<BipedObject, IRace, IRaceGetter>("HairBipedObject") },
        { "InjuredHealthPercent", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("InjuredHealthPercent") },
        { "ShieldBipedObject", new SimpleReflectionPropertyHandler<BipedObject, IRace, IRaceGetter>("ShieldBipedObject") },
        { "Regen", new RaceRegenHandler() },
        { "UnarmedDamage", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("UnarmedDamage") },
        { "UnarmedReach", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("UnarmedReach") },
        { "BodyBipedObject", new SimpleReflectionPropertyHandler<BipedObject, IRace, IRaceGetter>("BodyBipedObject") },
        { "AimAngleTolerance", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("AimAngleTolerance") },
        { "FlightRadius", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("FlightRadius") },
        { "AngularAccelerationRate", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("AngularAccelerationRate") },
        { "AngularTolerance", new SimpleReflectionPropertyHandler<float, IRace, IRaceGetter>("AngularTolerance") },
        { "MountData", new GeneratedCopyReflectionPropertyHandler<IMountDataGetter, MountData, IRace, IRaceGetter>(
            "MountData", value => value.DeepCopy(), MountDataMixIn.Equals) },
        { "SkeletalModel.Male", new GenderedItemSideHandler<ISimpleModelGetter?, SimpleModel?, IRace, IRaceGetter>("SkeletalModel", MaleFemaleGender.Male, record => record.SkeletalModel, (record, value) => record.SkeletalModel = value == null || (value.Male == null && value.Female == null) ? null : value, value => value?.DeepCopy(), (left, right) => left == null ? right == null : right != null && left.Equals(right)) },
        { "SkeletalModel.Female", new GenderedItemSideHandler<ISimpleModelGetter?, SimpleModel?, IRace, IRaceGetter>("SkeletalModel", MaleFemaleGender.Female, record => record.SkeletalModel, (record, value) => record.SkeletalModel = value == null || (value.Male == null && value.Female == null) ? null : value, value => value?.DeepCopy(), (left, right) => left == null ? right == null : right != null && left.Equals(right)) },
        { "MovementTypeNames", new SimpleReflectionListPropertyHandler<string, IRace, IRaceGetter>("MovementTypeNames", ListSemantics.SortedKeyed) },
        { "Voices", new GenderedItemHandler<IFormLinkGetter<IVoiceTypeGetter>, IFormLinkGetter<IVoiceTypeGetter>, IRace, IRaceGetter>("Voices", record => record.Voices, (record, value) => { if (value != null) record.Voices = value; }, value => new FormLink<IVoiceTypeGetter>(value.FormKey), (left, right) => left.FormKey == right.FormKey) },
        { "DecapitateArmors", new GenderedItemHandler<IFormLinkGetter<IArmorGetter>, IFormLinkGetter<IArmorGetter>, IRace, IRaceGetter>("DecapitateArmors", record => record.DecapitateArmors, (record, value) => record.DecapitateArmors = value, value => new FormLink<IArmorGetter>(value.FormKey), (left, right) => left.FormKey == right.FormKey) },
        { "DefaultHairColors", new GenderedItemHandler<IFormLinkGetter<IColorRecordGetter>, IFormLinkGetter<IColorRecordGetter>, IRace, IRaceGetter>("DefaultHairColors", record => record.DefaultHairColors, (record, value) => record.DefaultHairColors = value, value => new FormLink<IColorRecordGetter>(value.FormKey), (left, right) => left.FormKey == right.FormKey) },
        { "AttackRace", new SimpleReflectionFormLinkPropertyHandler<IRaceGetter, IRace, IRaceGetter>("AttackRace") },
        { "Attacks", new AttacksHandler() },
        { "BodyData", new GenderedItemHandler<IBodyDataGetter?, BodyData?, IRace, IRaceGetter>("BodyData", record => record.BodyData, (record, value) => { if (value != null) record.BodyData = value; }, value => value?.DeepCopy(), (left, right) => left == null ? right == null : right != null && left.Equals(right)) },
        { "Hairs", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IHairGetter>, IRace, IRaceGetter>("Hairs", ListSemantics.SortedKeyed) },
        { "Eyes", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IEyesGetter>, IRace, IRaceGetter>("Eyes", ListSemantics.SortedKeyed) },
        { "BodyPartData", new SimpleReflectionFormLinkPropertyHandler<IBodyPartDataGetter, IRace, IRaceGetter>("BodyPartData") },
        { "BehaviorGraph.Male", new GenderedItemSideHandler<IModelBehaviorGetter?, ModelBehavior?, IRace, IRaceGetter>("BehaviorGraph", MaleFemaleGender.Male, record => record.BehaviorGraph, (record, value) => { if (value != null) record.BehaviorGraph = value; }, value => value?.DeepCopy(), (left, right) => left == null ? right == null : right != null && left.Equals(right)) },
        { "BehaviorGraph.Female", new GenderedItemSideHandler<IModelBehaviorGetter?, ModelBehavior?, IRace, IRaceGetter>("BehaviorGraph", MaleFemaleGender.Female, record => record.BehaviorGraph, (record, value) => { if (value != null) record.BehaviorGraph = value; }, value => value?.DeepCopy(), (left, right) => left == null ? right == null : right != null && left.Equals(right)) },
        { "MaterialType", new SimpleReflectionFormLinkPropertyHandler<IMaterialTypeGetter, IRace, IRaceGetter>("MaterialType") },
        { "ImpactDataSet", new SimpleReflectionFormLinkPropertyHandler<IImpactDataSetGetter, IRace, IRaceGetter>("ImpactDataSet") },
        { "DecapitationFX", new SimpleReflectionFormLinkPropertyHandler<IArtObjectGetter, IRace, IRaceGetter>("DecapitationFX") },
        { "OpenLootSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IRace, IRaceGetter>("OpenLootSound") },
        { "CloseLootSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IRace, IRaceGetter>("CloseLootSound") },
        { "BipedObjectNames", new RaceBipedObjectNamesHandler() },
        { "MovementTypes", new GeneratedCopyReflectionListPropertyHandler<IRaceMovementTypeGetter, RaceMovementType, IRace, IRaceGetter>(
            "MovementTypes", ListSemantics.SortedKeyed, value => value.DeepCopy(), RaceMovementTypeMixIn.Equals,
            keySelector: movement => movement.MovementType.FormKey) },
        { "EquipmentFlags", new SimpleReflectionPropertyHandler<EquipTypeFlag?, IRace, IRaceGetter>("EquipmentFlags") },
        { "EquipmentSlots", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IEquipTypeGetter>, IRace, IRaceGetter>("EquipmentSlots", ListSemantics.SortedKeyed) },
        { "UnarmedEquipSlot", new SimpleReflectionFormLinkPropertyHandler<IEquipTypeGetter, IRace, IRaceGetter>("UnarmedEquipSlot") },
        { "FaceFxPhonemes", new GeneratedCopyReflectionPropertyHandler<IFaceFxPhonemesGetter, FaceFxPhonemes, IRace, IRaceGetter>(
            "FaceFxPhonemes", value => value.DeepCopy(), FaceFxPhonemesMixIn.Equals) },
        { "BaseMovementDefaultWalk", new SimpleReflectionFormLinkPropertyHandler<IMovementTypeGetter, IRace, IRaceGetter>("BaseMovementDefaultWalk") },
        { "BaseMovementDefaultRun", new SimpleReflectionFormLinkPropertyHandler<IMovementTypeGetter, IRace, IRaceGetter>("BaseMovementDefaultRun") },
        { "BaseMovementDefaultSwim", new SimpleReflectionFormLinkPropertyHandler<IMovementTypeGetter, IRace, IRaceGetter>("BaseMovementDefaultSwim") },
        { "BaseMovementDefaultFly", new SimpleReflectionFormLinkPropertyHandler<IMovementTypeGetter, IRace, IRaceGetter>("BaseMovementDefaultFly") },
        { "BaseMovementDefaultSneak", new SimpleReflectionFormLinkPropertyHandler<IMovementTypeGetter, IRace, IRaceGetter>("BaseMovementDefaultSneak") },
        { "BaseMovementDefaultSprint", new SimpleReflectionFormLinkPropertyHandler<IMovementTypeGetter, IRace, IRaceGetter>("BaseMovementDefaultSprint") },
        { "ArmorRace", new SimpleReflectionFormLinkPropertyHandler<IRaceGetter, IRace, IRaceGetter>("ArmorRace") },

    };

    public override void ApplyForwardedProperties(IMajorRecord record, Dictionary<string, object?> propertiesToForward)
    {
        if (record is not IRace race) throw new ArgumentException("Expected a RACE record.", nameof(record));
        const string presenceKey = "HeadData.Presence";
        var hasPresence = propertiesToForward.TryGetValue(presenceKey, out var value);
        var presence = hasPresence
            ? value as RaceHeadDataPresence ?? throw new ArgumentException("Expected RACE head presence.")
            : new RaceHeadDataPresence(race.HeadData is not null, race.HeadData?.Male is not null, race.HeadData?.Female is not null);
        var coordinated = new Dictionary<string, object?>();
        if (hasPresence) coordinated[presenceKey] = presence;
        foreach (var (key, selected) in propertiesToForward)
        {
            if (key == presenceKey) continue;
            if (key.StartsWith("HeadData.Male.", StringComparison.Ordinal) && (!presence.Present || !presence.Male)) continue;
            if (key.StartsWith("HeadData.Female.", StringComparison.Ordinal) && (!presence.Present || !presence.Female)) continue;
            coordinated[key] = selected;
        }
        base.ApplyForwardedProperties(record, coordinated);
    }

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IRaceGetter record)
        {
            throw new InvalidOperationException($"Expected IRaceGetter but got {winningContext.Record.GetType()}");
        }

        return record
            .ToLink<IRaceGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IRace, IRaceGetter>(state.LinkCache)
            .ToArray();
    }
}
