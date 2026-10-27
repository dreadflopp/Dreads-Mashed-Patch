using DreadsMashedPatch.AssetForwarding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Armor;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Forwarding decision: meshes/bounds and the armature selector use Meshes; each gender's icons
    // use IconsAndImages. Body-template gameplay flags, armor class and race retain ordinary rules.
    // Mesh writers preserve icons; icon writers preserve meshes.
    // Armature is one authored list: unioning addons can invent an equipped model combination.
    // BodyTemplate.ActsLike44 selects BOD2 versus BODT layout and is not forwarded independently.
    public class ArmorRecordHandler : AbstractRecordHandler
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
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Mutagen.Bethesda.Skyrim.Armor.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IArmor, IArmorGetter>("Name") },
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IArmor, IArmorGetter>() },
            { "ObjectEffect", new SimpleReflectionFormLinkPropertyHandler<IEffectRecordGetter, IArmor, IArmorGetter>("ObjectEffect") },
            { "EnchantmentAmount", new SimpleReflectionPropertyHandler<ushort?, IArmor, IArmorGetter>("EnchantmentAmount") },
            { "WorldModelAndBounds", ModelBoundsHandler.ForArmorWorldModel() },
            { "WorldModel.Male.Icons", new ArmorIconsHandler(MaleFemaleGender.Male) },
            { "WorldModel.Female.Icons", new ArmorIconsHandler(MaleFemaleGender.Female) },
            { "BodyTemplate.FirstPersonFlags", new SimpleReflectionFlagPropertyHandler<BipedObjectFlag, IArmor, IArmorGetter>("BodyTemplate.FirstPersonFlags", preserveUnknownBits: true, includeUnnamedBits: true) },
            { "BodyTemplate.Flags", new SimpleReflectionFlagPropertyHandler<BodyTemplate.Flag, IArmor, IArmorGetter>("BodyTemplate.Flags", preserveUnknownBits: true) },
            { "BodyTemplate.ArmorType", new SimpleReflectionPropertyHandler<ArmorType, IArmor, IArmorGetter>("BodyTemplate.ArmorType") },
            { "Destructible", DestructibleDataHandlers.Data(new DestructibleHandler()) },
            { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new DestructibleHandler()) },
            { "PickUpSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IArmor, IArmorGetter>("PickUpSound") },
            { "PutDownSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IArmor, IArmorGetter>("PutDownSound") },
            { "RagdollConstraintTemplate", new SimpleReflectionPropertyHandler<string?, IArmor, IArmorGetter>("RagdollConstraintTemplate") },
            { "EquipmentType", new SimpleReflectionFormLinkPropertyHandler<IEquipTypeGetter, IArmor, IArmorGetter>("EquipmentType") },
            { "BashImpactDataSet", new SimpleReflectionFormLinkPropertyHandler<IImpactDataSetGetter, IArmor, IArmorGetter>("BashImpactDataSet") },
            { "AlternateBlockMaterial", new SimpleReflectionFormLinkPropertyHandler<IMaterialTypeGetter, IArmor, IArmorGetter>("AlternateBlockMaterial") },
            { "Race", new SimpleReflectionFormLinkPropertyHandler<IRaceGetter, IArmor, IArmorGetter>("Race") },
            { "Keywords", new KeywordListHandler() },
            { "Description", new TranslatedStringReflectionPropertyHandler<IArmor, IArmorGetter>("Description") },
            { "Armature", new AtomicFormLinkListPropertyHandler<IArmorAddonGetter, IArmor, IArmorGetter>("Armature") },
            { "Value", new ValueHandler() },
            { "Weight", new WeightHandler() },
            { "ArmorRating", new SimpleReflectionPropertyHandler<float, IArmor, IArmorGetter>("ArmorRating") },
            { "TemplateArmor", new SimpleReflectionFormLinkPropertyHandler<IArmorGetter, IArmor, IArmorGetter>("TemplateArmor") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IArmorGetter armorRecord)
            {
                throw new InvalidOperationException($"Expected IArmorGetter but got {winningContext.Record.GetType()}");
            }

            return armorRecord
                .ToLink<IArmorGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IArmor, IArmorGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
