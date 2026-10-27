using DreadsMashedPatch.AssetForwarding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.ArmorAddon;
using DreadsMashedPatch.PropertyHandlers.General;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Each gender selects a complete first/third-person mesh package with its weight-slider bit.
    // Typed model comparison includes filename/Data semantics; skin/swap links have separate ownership.
    // ActsLike44 is a serialization discriminator, not an independently forwarded field.
    public class ArmorAddonRecordHandler : AbstractRecordHandler
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
            { "WeightSliderEnabled.Male", new GenderedItemSideHandler<bool, bool, IArmorAddon, IArmorAddonGetter>("WeightSliderEnabled", MaleFemaleGender.Male, record => record.WeightSliderEnabled, (record, value) => { if (value != null) record.WeightSliderEnabled = value; }, value => value) },
            { "WeightSliderEnabled.Female", new GenderedItemSideHandler<bool, bool, IArmorAddon, IArmorAddonGetter>("WeightSliderEnabled", MaleFemaleGender.Female, record => record.WeightSliderEnabled, (record, value) => { if (value != null) record.WeightSliderEnabled = value; }, value => value) },
            { "WorldModel.Male.File", new GenderedModelFileHandler<IArmorAddon, IArmorAddonGetter>("WorldModel", MaleFemaleGender.Male, record => record.WorldModel, (record, value) => record.WorldModel = value) },
            { "WorldModel.Male.AlternateTextures", new GenderedModelAlternateTexturesHandler<IArmorAddon, IArmorAddonGetter>("WorldModel", MaleFemaleGender.Male, record => record.WorldModel, (record, value) => record.WorldModel = value) },
            { "WorldModel.Female.File", new GenderedModelFileHandler<IArmorAddon, IArmorAddonGetter>("WorldModel", MaleFemaleGender.Female, record => record.WorldModel, (record, value) => record.WorldModel = value) },
            { "WorldModel.Female.AlternateTextures", new GenderedModelAlternateTexturesHandler<IArmorAddon, IArmorAddonGetter>("WorldModel", MaleFemaleGender.Female, record => record.WorldModel, (record, value) => record.WorldModel = value) },
            { "FirstPersonModel.Male.File", new GenderedModelFileHandler<IArmorAddon, IArmorAddonGetter>("FirstPersonModel", MaleFemaleGender.Male, record => record.FirstPersonModel, (record, value) => record.FirstPersonModel = value) },
            { "FirstPersonModel.Male.AlternateTextures", new GenderedModelAlternateTexturesHandler<IArmorAddon, IArmorAddonGetter>("FirstPersonModel", MaleFemaleGender.Male, record => record.FirstPersonModel, (record, value) => record.FirstPersonModel = value) },
            { "FirstPersonModel.Female.File", new GenderedModelFileHandler<IArmorAddon, IArmorAddonGetter>("FirstPersonModel", MaleFemaleGender.Female, record => record.FirstPersonModel, (record, value) => record.FirstPersonModel = value) },
            { "FirstPersonModel.Female.AlternateTextures", new GenderedModelAlternateTexturesHandler<IArmorAddon, IArmorAddonGetter>("FirstPersonModel", MaleFemaleGender.Female, record => record.FirstPersonModel, (record, value) => record.FirstPersonModel = value) },
            { "AdditionalRaces", new AdditionalRacesHandler() },
            { "BodyTemplate.FirstPersonFlags", new SimpleReflectionFlagPropertyHandler<BipedObjectFlag, IArmorAddon, IArmorAddonGetter>("BodyTemplate.FirstPersonFlags", preserveUnknownBits: true, includeUnnamedBits: true) },
            { "BodyTemplate.Flags", new SimpleReflectionFlagPropertyHandler<BodyTemplate.Flag, IArmorAddon, IArmorAddonGetter>("BodyTemplate.Flags", preserveUnknownBits: true) },
            { "BodyTemplate.ArmorType", new SimpleReflectionPropertyHandler<ArmorType, IArmorAddon, IArmorAddonGetter>("BodyTemplate.ArmorType") },
            { "Priority.Male", new GenderedItemSideHandler<byte, byte, IArmorAddon, IArmorAddonGetter>("Priority", MaleFemaleGender.Male, record => record.Priority, (record, value) => { if (value != null) record.Priority = value; }, value => value) },
            { "Priority.Female", new GenderedItemSideHandler<byte, byte, IArmorAddon, IArmorAddonGetter>("Priority", MaleFemaleGender.Female, record => record.Priority, (record, value) => { if (value != null) record.Priority = value; }, value => value) },
            { "DetectionSoundValue", new SimpleReflectionPropertyHandler<byte, IArmorAddon, IArmorAddonGetter>("DetectionSoundValue") },
            { "WeaponAdjust", new SimpleReflectionPropertyHandler<float, IArmorAddon, IArmorAddonGetter>("WeaponAdjust") },
            { "Race", new SimpleReflectionFormLinkPropertyHandler<IRaceGetter, IArmorAddon, IArmorAddonGetter>("Race") },
            { "FootstepSound", new SimpleReflectionFormLinkPropertyHandler<IFootstepSetGetter, IArmorAddon, IArmorAddonGetter>("FootstepSound") },
            { "ArtObject", new SimpleReflectionFormLinkPropertyHandler<IArtObjectGetter, IArmorAddon, IArmorAddonGetter>("ArtObject") },
            { "SkinTexture.Male", new GenderedItemSideHandler<IFormLinkNullableGetter<ITextureSetGetter>, IFormLinkNullableGetter<ITextureSetGetter>, IArmorAddon, IArmorAddonGetter>("SkinTexture", MaleFemaleGender.Male, record => record.SkinTexture, (record, value) => record.SkinTexture = value, value => value == null ? new FormLinkNullable<ITextureSetGetter>() : new FormLinkNullable<ITextureSetGetter>(value.FormKey), (left, right) => left?.FormKey == right?.FormKey) },
            { "SkinTexture.Female", new GenderedItemSideHandler<IFormLinkNullableGetter<ITextureSetGetter>, IFormLinkNullableGetter<ITextureSetGetter>, IArmorAddon, IArmorAddonGetter>("SkinTexture", MaleFemaleGender.Female, record => record.SkinTexture, (record, value) => record.SkinTexture = value, value => value == null ? new FormLinkNullable<ITextureSetGetter>() : new FormLinkNullable<ITextureSetGetter>(value.FormKey), (left, right) => left?.FormKey == right?.FormKey) },
            { "TextureSwapList.Male", new GenderedItemSideHandler<IFormLinkNullableGetter<IFormListGetter>, IFormLinkNullableGetter<IFormListGetter>, IArmorAddon, IArmorAddonGetter>("TextureSwapList", MaleFemaleGender.Male, record => record.TextureSwapList, (record, value) => record.TextureSwapList = value, value => value == null ? new FormLinkNullable<IFormListGetter>() : new FormLinkNullable<IFormListGetter>(value.FormKey), (left, right) => left?.FormKey == right?.FormKey) },
            { "TextureSwapList.Female", new GenderedItemSideHandler<IFormLinkNullableGetter<IFormListGetter>, IFormLinkNullableGetter<IFormListGetter>, IArmorAddon, IArmorAddonGetter>("TextureSwapList", MaleFemaleGender.Female, record => record.TextureSwapList, (record, value) => record.TextureSwapList = value, value => value == null ? new FormLinkNullable<IFormListGetter>() : new FormLinkNullable<IFormListGetter>(value.FormKey), (left, right) => left?.FormKey == right?.FormKey) }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IArmorAddonGetter armorAddonRecord)
            {
                throw new InvalidOperationException($"Expected IArmorAddonGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = armorAddonRecord
                .ToLink<IArmorAddonGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IArmorAddon, IArmorAddonGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
