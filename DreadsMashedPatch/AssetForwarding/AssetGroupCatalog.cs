using DreadsMashedPatch.RecordHandlers;

namespace DreadsMashedPatch.AssetForwarding;

/// <summary>Explicit source-reviewed ownership. No extension-based or reflection-based asset policy.</summary>
public static class AssetGroupCatalog
{
    private static readonly IReadOnlyDictionary<Type, IReadOnlyList<AssetGroup>> Groups =
        new Dictionary<Type, IReadOnlyList<AssetGroup>>
        {
            [typeof(ActivatorRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(AddonNodeRecordHandler)] =
            [
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(AlchemicalApparatusRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(AmmunitionRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(AnimatedObjectRecordHandler)] =
            [
                new("AnimatedModel", ForwardingRule.Meshes, "Model", "UnloadEvent"),
            ],
            [typeof(ArmorRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("WorldModelAndBounds", ForwardingRule.Meshes, "WorldModelAndBounds"),
                new("MaleIcons", ForwardingRule.IconsAndImages, "WorldModel.Male.Icons"),
                new("FemaleIcons", ForwardingRule.IconsAndImages, "WorldModel.Female.Icons"),
                new("WornMeshes", ForwardingRule.Meshes, "Armature", "BodyTemplate.FirstPersonFlags"),
            ],
            [typeof(ArmorAddonRecordHandler)] =
            [
                new("MaleMeshes", ForwardingRule.Meshes, "WorldModel.Male.File", "WorldModel.Male.AlternateTextures", "FirstPersonModel.Male.File", "FirstPersonModel.Male.AlternateTextures", "WeightSliderEnabled.Male"),
                new("MaleSkinTextures", ForwardingRule.Textures, "SkinTexture.Male", "TextureSwapList.Male"),
                new("FemaleMeshes", ForwardingRule.Meshes, "WorldModel.Female.File", "WorldModel.Female.AlternateTextures", "FirstPersonModel.Female.File", "FirstPersonModel.Female.AlternateTextures", "WeightSliderEnabled.Female"),
                new("FemaleSkinTextures", ForwardingRule.Textures, "SkinTexture.Female", "TextureSwapList.Female"),
            ],
            [typeof(ArtObjectRecordHandler)] =
            [
                new("ArtModel", ForwardingRule.Meshes, "ModelAndBounds", "Type"),
            ],
            [typeof(BodyPartDataRecordHandler)] =
            [
                new("BodyModel", ForwardingRule.Meshes, "Model"),
                new("LimbMeshes", ForwardingRule.Meshes, "Parts.LimbMeshes"),
            ],
            [typeof(BookRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(CameraShotRecordHandler)] =
            [
                new("CameraModel", ForwardingRule.Meshes, "Model"),
            ],
            [typeof(CellRecordHandler)] =
            [
                new("WaterDisplay", ForwardingRule.Textures, "WaterNoiseTexture", "WaterEnvironmentMap"),
            ],
            [typeof(ClassRecordHandler)] =
            [
                new("Icon", ForwardingRule.IconsAndImages, "Icon"),
            ],
            [typeof(ClimateRecordHandler)] =
            [
                new("Model", ForwardingRule.Meshes, "Model"),
                new("SunImages", ForwardingRule.Textures, "SunTexture", "SunGlareTexture"),
            ],
            [typeof(ContainerRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(DebrisRecordHandler)] =
            [
                new("Models", ForwardingRule.Meshes, "Models"),
            ],
            [typeof(DialogResponseRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(DoorRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(EffectShaderRecordHandler)] =
            [
                new("MembraneTextures", ForwardingRule.Textures, "FillTexture", "HolesTexture", "MembranePaletteTexture", "FillTextureCoordinates"),
                new("ParticleTextures", ForwardingRule.Textures, "ParticleShaderTexture", "ParticlePaletteTexture", "ParticleAtlas"),
            ],
            [typeof(ExplosionRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(EyesRecordHandler)] =
            [
                new("EyeTexture", ForwardingRule.Textures, "Icon"),
            ],
            [typeof(FloraRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(FurnitureRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
                new("ModelFilename", ForwardingRule.Meshes, "ModelFilename"),
            ],
            [typeof(GrassRecordHandler)] =
            [
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(HazardRecordHandler)] =
            [
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(HeadPartRecordHandler)] =
            [
                new("HeadMesh", ForwardingRule.Meshes, "Mesh"),
            ],
            [typeof(IdleAnimationRecordHandler)] =
            [
                new("IdleReference", ForwardingRule.Animations, "Filename", "AnimationEvent"),
            ],
            [typeof(IdleMarkerRecordHandler)] =
            [
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
                new("IdleSelection", ForwardingRule.Animations, "Animations", "Flags", "IdleTimer"),
            ],
            [typeof(ImpactRecordHandler)] =
            [
                new("ImpactModel", ForwardingRule.Meshes, "Model"),
            ],
            [typeof(IngestibleRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(IngredientRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(KeyRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(LandscapeTextureRecordHandler)] =
            [
                new("LandscapeMaterial", ForwardingRule.Textures, "TextureSet"),
            ],
            [typeof(LensFlareRecordHandler)] =
            [
                new("FlareSprites", ForwardingRule.Textures, "FlareSprites"),
            ],
            [typeof(LeveledNpcRecordHandler)] =
            [
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(LightRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(LoadScreenRecordHandler)] =
            [
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("DisplayModel", ForwardingRule.Meshes, "LoadingScreenNif", "InitialScale", "InitialRotation", "RotationOffsetConstraints", "InitialTranslationOffset", "CameraPath"),
            ],
            [typeof(MagicEffectRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(MaterialObjectRecordHandler)] =
            [
                new("MaterialModel", ForwardingRule.Meshes, "Model"),
            ],
            [typeof(MiscItemRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(MoveableStaticRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(MusicTrackRecordHandler)] =
            [
                new("MusicPlayback", ForwardingRule.Music, "Type", "Duration", "FadeOut", "TrackFilename", "FinaleFilename", "LoopData", "CuePoints", "Tracks"),
            ],
            [typeof(NpcRecordHandler)] =
            [
                new("Appearance", ForwardingRule.Appearance, "Appearance"),
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("AnimationChoice", ForwardingRule.Animations, "Configuration.Flags.OppositeGenderAnims"),
            ],
            [typeof(PackageRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("IdleSelection", ForwardingRule.Animations, "IdleAnimations"),
            ],
            [typeof(PerkRecordHandler)] =
            [
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("ScriptBindings", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(PlacedArrowRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(PlacedBarrierRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(PlacedBeamRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(PlacedConeRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(PlacedFlameRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(PlacedHazardRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(PlacedMissileRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(PlacedNpcRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(PlacedObjectRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(PlacedTrapRecordHandler)] =
            [
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
            ],
            [typeof(ProjectileRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
                new("MuzzleFlashBehavior", ForwardingRule.Meshes, "MuzzleFlashBehavior"),
            ],
            [typeof(QuestRecordHandler)] =
            [
                new("ScriptBindings", ForwardingRule.Scripts, "VirtualMachineAdapter.Presence", "VirtualMachineAdapter.Version", "VirtualMachineAdapter.ObjectFormat", "VirtualMachineAdapter.Scripts", "VirtualMachineAdapter.ExtraBindDataVersion", "VirtualMachineAdapter.FileName", "VirtualMachineAdapter.Fragments", "VirtualMachineAdapter.Aliases"),
            ],
            [typeof(RaceRecordHandler)] =
            [
                new("HeadShape", ForwardingRule.Appearance, "HeadData.Male.Shape", "HeadData.Female.Shape", "MorphRace", "FacegenMainClamp", "FacegenFaceClamp"),
                new("SkinMesh", ForwardingRule.Meshes, "Skin"),
                new("BodyTextures", ForwardingRule.Textures, "BodyData"),
                new("MaleHeadMesh", ForwardingRule.Meshes, "HeadData.Male.Mesh"),
                new("FemaleHeadMesh", ForwardingRule.Meshes, "HeadData.Female.Mesh"),
                new("MaleFaceTextures", ForwardingRule.Textures, "HeadData.Male.FaceTextures"),
                new("FemaleFaceTextures", ForwardingRule.Textures, "HeadData.Female.FaceTextures"),
                new("TintDefinitions", ForwardingRule.Textures, "HeadData.Male.Tints", "HeadData.Female.Tints", "NumberOfTintsInList"),
                new("MaleRig", ForwardingRule.Animations, "SkeletalModel.Male", "BehaviorGraph.Male"),
                new("FemaleRig", ForwardingRule.Animations, "SkeletalModel.Female", "BehaviorGraph.Female"),
                new("BodyPartReference", ForwardingRule.Animations, "BodyPartData"),
            ],
            [typeof(RegionRecordHandler)] =
            [
                new("ObjectsIcons", ForwardingRule.IconsAndImages, "Objects.Icons"),
                new("WeatherIcons", ForwardingRule.IconsAndImages, "Weather.Icons"),
                new("MapIcons", ForwardingRule.IconsAndImages, "Map.Icons"),
                new("LandIcons", ForwardingRule.IconsAndImages, "Land.Icons"),
                new("GrassesIcons", ForwardingRule.IconsAndImages, "Grasses.Icons"),
                new("SoundsIcons", ForwardingRule.IconsAndImages, "Sounds.Icons"),
                new("RegionSounds", ForwardingRule.Sounds, "Sounds.Sounds"),
                new("RegionMusic", ForwardingRule.Music, "Sounds.Music"),
            ],
            [typeof(SceneRecordHandler)] =
            [
                new("ScriptBindings", ForwardingRule.Scripts, "VirtualMachineAdapter.Presence", "VirtualMachineAdapter.ScriptFragments", "VirtualMachineAdapter.Version", "VirtualMachineAdapter.ObjectFormat", "VirtualMachineAdapter.Scripts"),
            ],
            [typeof(ScrollRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(ShaderParticleGeometryRecordHandler)] =
            [
                new("ParticleTexture", ForwardingRule.Textures, "NumSubtexturesX", "NumSubtexturesY", "ParticleTexture"),
            ],
            [typeof(SoulGemRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(SoundDescriptorRecordHandler)] =
            [
                new("SoundPlayback", ForwardingRule.Sounds, "Type", "SoundFiles", "LoopAndRumble"),
            ],
            [typeof(StaticRecordHandler)] =
            [
                new("Geometry", ForwardingRule.Meshes, "ModelAndBounds", "Lod"),
            ],
            [typeof(TalkingActivatorRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
            ],
            [typeof(TextureSetRecordHandler)] =
            [
                new("TextureDefinition", ForwardingRule.Textures, "TextureDefinition"),
            ],
            [typeof(TreeRecordHandler)] =
            [
                new("TreeAssets", ForwardingRule.Trees, "ModelAndBounds", "TreeData", "VirtualMachineAdapter", "MajorRecordFlagsRaw"),
            ],
            [typeof(WaterRecordHandler)] =
            [
                new("NoiseLayerOne", ForwardingRule.Textures, "NoiseLayerOneTexture", "NoiseLayerOneUvScale"),
                new("NoiseLayerTwo", ForwardingRule.Textures, "NoiseLayerTwoTexture", "NoiseLayerTwoUvScale"),
                new("NoiseLayerThree", ForwardingRule.Textures, "NoiseLayerThreeTexture", "NoiseLayerThreeUvScale"),
                new("FlowTexture", ForwardingRule.Textures, "FlowNormalsNoiseTexture", "NoiseFlowmapScale", "Flags.Surface"),
            ],
            [typeof(WeaponRecordHandler)] =
            [
                new("DestructionMeshes", ForwardingRule.Meshes, "Destructible.StageModels"),
                new("Icons", ForwardingRule.IconsAndImages, "Icons"),
                new("VirtualMachineAdapter", ForwardingRule.Scripts, "VirtualMachineAdapter"),
                new("ModelAndBounds", ForwardingRule.Meshes, "ModelAndBounds"),
                new("Scope", ForwardingRule.Meshes, "ScopeModel", "Data.SightFOV", "Data.Flags.HasScope"),
                new("FirstPersonGeometry", ForwardingRule.Meshes, "FirstPersonModel"),
            ],
            [typeof(WeatherRecordHandler)] =
            [
                new("Aurora", ForwardingRule.Meshes, "Aurora"),
                new("CloudLayers", ForwardingRule.Textures, "CloudTextures", "Clouds"),
            ],
            [typeof(WorldspaceRecordHandler)] =
            [
                new("CloudModel", ForwardingRule.Meshes, "CloudModel"),
                new("WaterDisplay", ForwardingRule.Textures, "WaterNoiseTexture", "WaterEnvironmentMap"),
                new("DistantTextures", ForwardingRule.Textures, "HdLodDiffuseTexture", "HdLodNormalTexture"),
                new("MapDisplay", ForwardingRule.Textures, "MapImage", "MapData", "WorldMapOffset"),
            ],
        };

    public static IReadOnlyList<AssetGroup> For(Type recordHandlerType)
    {
        for (Type? current = recordHandlerType; current != null; current = current.BaseType)
            if (Groups.TryGetValue(current, out var groups)) return groups;
        return [];
    }
}
