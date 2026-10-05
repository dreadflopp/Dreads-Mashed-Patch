# Record handler property coverage audit

Generated: 2026-10-05 19:51:59 +02:00

This is a static registration audit. `Covered` is an exact registration, `AggregateCovered` is inferred from a specialized handler implementation, `Partial` indicates nested/split handling, and `MissingCandidate` has no detected handler. Reviewed aliases and non-property surfaces are classified through the tracked overrides file.

Direct and inherited record/aspect properties are compared, including `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, and `MajorFlags` aliases. Common major-record identity/version/runtime interfaces are excluded; their header properties remain audited. Registration coverage does not establish complete nested comparison, setter behavior or binary preservation.

Strict verification: `powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts/Audit-RecordHandlerCoverage.ps1 -FailOnUnresolved`. This fails for stale overrides, audit errors, or any unexplained partial/missing candidate.

## Summary

| Handler | Getter | Covered | Aggregate | Partial | Missing | Classified exclusion/alias | Error |
|---|---|---:|---:|---:|---:|---:|---|
| AcousticSpaceRecordHandler.cs | IAcousticSpaceGetter | 6 | 0 | 0 | 0 | 1 |  |
| ActionRecordHandler.cs | IActionRecordGetter | 3 | 0 | 0 | 0 | 1 |  |
| ActivatorRecordHandler.cs | IActivatorGetter | 13 | 2 | 0 | 0 | 2 |  |
| ActorValueInformationRecordHandler.cs | IActorValueInformationGetter | 7 | 0 | 0 | 0 | 2 |  |
| AddonNodeRecordHandler.cs | IAddonNodeGetter | 6 | 2 | 0 | 0 | 1 |  |
| AlchemicalApparatusRecordHandler.cs | IAlchemicalApparatusGetter | 12 | 2 | 0 | 0 | 1 |  |
| AmmunitionRecordHandler.cs | IAmmunitionGetter | 15 | 2 | 0 | 0 | 3 |  |
| AnimatedObjectRecordHandler.cs | IAnimatedObjectGetter | 4 | 0 | 0 | 0 | 1 |  |
| ArmorAddonRecordHandler.cs | IArmorAddonGetter | 8 | 7 | 0 | 0 | 3 |  |
| ArmorRecordHandler.cs | IArmorGetter | 21 | 3 | 0 | 0 | 2 |  |
| ArtObjectRecordHandler.cs | IArtObjectGetter | 3 | 2 | 0 | 0 | 1 |  |
| AssociationTypeRecordHandler.cs | IAssociationTypeGetter | 5 | 0 | 0 | 0 | 1 |  |
| BodyPartDataRecordHandler.cs | IBodyPartDataGetter | 4 | 0 | 0 | 0 | 1 |  |
| BookRecordHandler.cs | IBookGetter | 17 | 2 | 0 | 0 | 2 |  |
| CameraPathRecordHandler.cs | ICameraPathGetter | 7 | 0 | 0 | 0 | 1 |  |
| CameraShotRecordHandler.cs | ICameraShotGetter | 15 | 0 | 0 | 0 | 2 |  |
| CellRecordHandler.cs | ICellGetter | 26 | 0 | 0 | 0 | 13 |  |
| ClassRecordHandler.cs | IClassGetter | 11 | 0 | 0 | 0 | 3 |  |
| ClimateRecordHandler.cs | IClimateGetter | 13 | 0 | 0 | 0 | 1 |  |
| CollisionLayerRecordHandler.cs | ICollisionLayerGetter | 8 | 0 | 0 | 0 | 1 |  |
| ColorRecordHandler.cs | IColorRecordGetter | 5 | 0 | 0 | 0 | 1 |  |
| CombatStyleRecordHandler.cs | ICombatStyleGetter | 18 | 0 | 0 | 0 | 3 |  |
| ConstructibleObjectRecordHandler.cs | IConstructibleObjectGetter | 7 | 0 | 0 | 0 | 1 |  |
| ContainerRecordHandler.cs | IContainerGetter | 10 | 2 | 0 | 0 | 2 |  |
| DebrisRecordHandler.cs | IDebrisGetter | 3 | 0 | 0 | 0 | 1 |  |
| DefaultObjectManagerRecordHandler.cs | IDefaultObjectManagerGetter | 3 | 0 | 0 | 0 | 1 |  |
| DialogBranchRecordHandler.cs | IDialogBranchGetter | 6 | 0 | 0 | 0 | 1 |  |
| DialogResponseRecordHandler.cs | IDialogResponsesGetter | 15 | 0 | 0 | 0 | 4 |  |
| DialogTopicRecordHandler.cs | IDialogTopicGetter | 10 | 0 | 0 | 0 | 4 |  |
| DialogViewRecordHandler.cs | IDialogViewGetter | 7 | 0 | 0 | 0 | 1 |  |
| DoorRecordHandler.cs | IDoorGetter | 9 | 2 | 0 | 0 | 2 |  |
| DualCastDataRecordHandler.cs | IDualCastDataGetter | 9 | 0 | 0 | 0 | 1 |  |
| EffectShaderRecordHandler.cs | IEffectShaderGetter | 7 | 100 | 0 | 0 | 2 |  |
| EncounterZoneRecordHandler.cs | IEncounterZoneGetter | 8 | 0 | 0 | 0 | 2 |  |
| EquipTypeRecordHandler.cs | IEquipTypeGetter | 4 | 0 | 0 | 0 | 1 |  |
| ExplosionRecordHandler.cs | IExplosionGetter | 19 | 2 | 0 | 0 | 2 |  |
| EyesRecordHandler.cs | IEyesGetter | 5 | 0 | 0 | 0 | 2 |  |
| FactionRecordHandler.cs | IFactionGetter | 18 | 0 | 0 | 0 | 1 |  |
| FloraRecordHandler.cs | IFloraGetter | 12 | 2 | 0 | 0 | 1 |  |
| FootstepRecordHandler.cs | IFootstepGetter | 4 | 0 | 0 | 0 | 1 |  |
| FootstepSetRecordHandler.cs | IFootstepSetGetter | 7 | 0 | 0 | 0 | 1 |  |
| FormIdRecordHandler.cs | IFormListGetter | 3 | 0 | 0 | 0 | 1 |  |
| FurnitureRecordHandler.cs | IFurnitureGetter | 13 | 2 | 0 | 0 | 2 |  |
| GameSettingBoolRecordHandler.cs | IGameSettingBoolGetter | 3 | 0 | 0 | 0 | 1 |  |
| GameSettingFloatRecordHandler.cs | IGameSettingFloatGetter | 3 | 0 | 0 | 0 | 1 |  |
| GameSettingIntRecordHandler.cs | IGameSettingIntGetter | 3 | 0 | 0 | 0 | 1 |  |
| GameSettingStringRecordHandler.cs | IGameSettingStringGetter | 3 | 0 | 0 | 0 | 1 |  |
| GlobalFloatRecordHandler.cs | IGlobalFloatGetter | 3 | 0 | 0 | 0 | 3 |  |
| GlobalIntRecordHandler.cs | IGlobalIntGetter | 3 | 0 | 0 | 0 | 3 |  |
| GlobalShortRecordHandler.cs | IGlobalShortGetter | 3 | 0 | 0 | 0 | 3 |  |
| GlobalUnknownRecordHandler.cs | IGlobalUnknownGetter | 4 | 0 | 0 | 0 | 2 |  |
| GrassRecordHandler.cs | IGrassGetter | 12 | 2 | 0 | 0 | 4 |  |
| HazardRecordHandler.cs | IHazardGetter | 14 | 2 | 0 | 0 | 1 |  |
| HeadPartRecordHandler.cs | IHeadPartGetter | 11 | 0 | 0 | 0 | 2 |  |
| IdleAnimationRecordHandler.cs | IIdleAnimationGetter | 11 | 0 | 0 | 0 | 1 |  |
| IdleMarkerRecordHandler.cs | IIdleMarkerGetter | 5 | 2 | 0 | 0 | 2 |  |
| ImageSpaceAdapterRecordHandler.cs | IImageSpaceAdapterGetter | 2 | 0 | 0 | 0 | 61 |  |
| ImageSpaceRecordHandler.cs | IImageSpaceGetter | 7 | 0 | 0 | 0 | 1 |  |
| ImpactDataSetRecordHandler.cs | IImpactDataSetGetter | 3 | 0 | 0 | 0 | 1 |  |
| ImpactRecordHandler.cs | IImpactGetter | 15 | 1 | 0 | 0 | 2 |  |
| IngestibleRecordHandler.cs | IIngestibleGetter | 17 | 2 | 0 | 0 | 2 |  |
| IngredientRecordHandler.cs | IIngredientGetter | 15 | 2 | 0 | 0 | 1 |  |
| KeyRecordHandler.cs | IKeyGetter | 11 | 2 | 0 | 0 | 2 |  |
| KeywordRecordHandler.cs | IKeywordGetter | 3 | 0 | 0 | 0 | 1 |  |
| LandscapeRecordHandler.cs | ILandscapeGetter | 8 | 0 | 0 | 0 | 1 |  |
| LandscapeTextureRecordHandler.cs | ILandscapeTextureGetter | 9 | 0 | 0 | 0 | 1 |  |
| LensFlareRecordHandler.cs | ILensFlareGetter | 2 | 3 | 0 | 0 | 1 |  |
| LeveledItemRecordHandler.cs | ILeveledItemGetter | 7 | 0 | 0 | 0 | 1 |  |
| LeveledNpcRecordHandler.cs | ILeveledNpcGetter | 6 | 2 | 0 | 0 | 1 |  |
| LeveledSpellRecordHandler.cs | ILeveledSpellGetter | 6 | 0 | 0 | 0 | 1 |  |
| LightingTemplateRecordHandler.cs | ILightingTemplateGetter | 18 | 0 | 0 | 0 | 3 |  |
| LightRecordHandler.cs | ILightGetter | 21 | 2 | 0 | 0 | 2 |  |
| LoadScreenRecordHandler.cs | ILoadScreenGetter | 11 | 0 | 0 | 0 | 2 |  |
| LocationRecordHandler.cs | ILocationGetter | 11 | 0 | 0 | 0 | 17 |  |
| LocationReferenceTypeRecordHandler.cs | ILocationReferenceTypeGetter | 3 | 0 | 0 | 0 | 1 |  |
| MagicEffectRecordHandler.cs | IMagicEffectGetter | 45 | 0 | 0 | 0 | 2 |  |
| MaterialObjectRecordHandler.cs | IMaterialObjectGetter | 13 | 0 | 0 | 0 | 2 |  |
| MaterialTypeRecordHandler.cs | IMaterialTypeGetter | 8 | 0 | 0 | 0 | 1 |  |
| MessageRecordHandler.cs | IMessageGetter | 9 | 0 | 0 | 0 | 1 |  |
| MiscItemRecordHandler.cs | IMiscItemGetter | 11 | 2 | 0 | 0 | 2 |  |
| MoveableStaticRecordHandler.cs | IMoveableStaticGetter | 6 | 2 | 0 | 0 | 2 |  |
| MovementTypeRecordHandler.cs | IMovementTypeGetter | 15 | 0 | 0 | 0 | 2 |  |
| MusicTrackRecordHandler.cs | IMusicTrackGetter | 11 | 0 | 0 | 0 | 1 |  |
| MusicTypeRecordHandler.cs | IMusicTypeGetter | 6 | 0 | 0 | 0 | 1 |  |
| NavigationMeshRecordHandler.cs | INavigationMeshGetter | 6 | 0 | 0 | 0 | 2 |  |
| NpcRecordHandler.cs | INpcGetter | 44 | 3 | 0 | 0 | 2 |  |
| ObjectEffectRecordHandler.cs | IObjectEffectGetter | 14 | 0 | 0 | 0 | 2 |  |
| OutfitRecordHandler.cs | IOutfitGetter | 3 | 0 | 0 | 0 | 1 |  |
| PackageRecordHandler.cs | IPackageGetter | 21 | 0 | 0 | 0 | 10 |  |
| PerkRecordHandler.cs | IPerkGetter | 14 | 0 | 0 | 0 | 2 |  |
| PlacedArrowRecordHandler.cs | IPlacedArrowGetter | 21 | 0 | 0 | 0 | 2 |  |
| PlacedBarrierRecordHandler.cs | IPlacedBarrierGetter | 21 | 0 | 0 | 0 | 2 |  |
| PlacedBeamRecordHandler.cs | IPlacedBeamGetter | 21 | 0 | 0 | 0 | 2 |  |
| PlacedConeRecordHandler.cs | IPlacedConeGetter | 21 | 0 | 0 | 0 | 2 |  |
| PlacedFlameRecordHandler.cs | IPlacedFlameGetter | 21 | 0 | 0 | 0 | 2 |  |
| PlacedHazardRecordHandler.cs | IPlacedHazardGetter | 21 | 0 | 0 | 0 | 2 |  |
| PlacedMissileRecordHandler.cs | IPlacedMissileGetter | 21 | 0 | 0 | 0 | 2 |  |
| PlacedNpcRecordHandler.cs | IPlacedNpcGetter | 31 | 0 | 0 | 0 | 2 |  |
| PlacedObjectRecordHandler.cs | IPlacedObjectGetter | 60 | 0 | 0 | 0 | 2 |  |
| PlacedTrapRecordHandler.cs | IPlacedTrapGetter | 21 | 0 | 0 | 0 | 2 |  |
| ProjectileRecordHandler.cs | IProjectileGetter | 12 | 21 | 0 | 0 | 2 |  |
| QuestRecordHandler.cs | IQuestGetter | 16 | 1 | 0 | 0 | 3 |  |
| RaceRecordHandler.cs | IRaceGetter | 65 | 7 | 0 | 0 | 5 |  |
| RegionRecordHandler.cs | IRegionGetter | 11 | 0 | 0 | 0 | 2 |  |
| RelationshipRecordHandler.cs | IRelationshipGetter | 7 | 0 | 0 | 0 | 3 |  |
| ReverbParametersRecordHandler.cs | IReverbParametersGetter | 2 | 11 | 0 | 0 | 2 |  |
| SceneRecordHandler.cs | ISceneGetter | 10 | 0 | 0 | 0 | 4 |  |
| ScrollRecordHandler.cs | IScrollGetter | 22 | 2 | 0 | 0 | 1 |  |
| ShaderParticleGeometryRecordHandler.cs | IShaderParticleGeometryGetter | 15 | 0 | 0 | 0 | 2 |  |
| ShoutRecordHandler.cs | IShoutGetter | 6 | 0 | 0 | 0 | 2 |  |
| SoulGemRecordHandler.cs | ISoulGemGetter | 13 | 2 | 0 | 0 | 2 |  |
| SoundCategoryRecordHandler.cs | ISoundCategoryGetter | 7 | 0 | 0 | 0 | 1 |  |
| SoundDescriptorRecordHandler.cs | ISoundDescriptorGetter | 11 | 4 | 0 | 0 | 1 |  |
| SoundMarkerRecordHandler.cs | ISoundMarkerGetter | 6 | 0 | 0 | 0 | 1 |  |
| SoundOutputModelRecordHandler.cs | ISoundOutputModelGetter | 9 | 0 | 0 | 0 | 1 |  |
| SpellRecordHandler.cs | ISpellGetter | 18 | 0 | 0 | 0 | 1 |  |
| StaticRecordHandler.cs | IStaticGetter | 6 | 2 | 0 | 0 | 4 |  |
| StoryManagerBranchNodeRecordHandler.cs | IStoryManagerBranchNodeGetter | 7 | 0 | 0 | 0 | 1 |  |
| StoryManagerEventNodeRecordHandler.cs | IStoryManagerEventNodeGetter | 8 | 0 | 0 | 0 | 1 |  |
| StoryManagerQuestNodeRecordHandler.cs | IStoryManagerQuestNodeGetter | 10 | 0 | 0 | 0 | 1 |  |
| TalkingActivatorRecordHandler.cs | ITalkingActivatorGetter | 8 | 2 | 0 | 0 | 4 |  |
| TextureSetRecordHandler.cs | ITextureSetGetter | 4 | 9 | 0 | 0 | 1 |  |
| TreeRecordHandler.cs | ITreeGetter | 11 | 2 | 0 | 0 | 3 |  |
| VisualEffectRecordHandler.cs | IVisualEffectGetter | 5 | 0 | 0 | 0 | 1 |  |
| VoiceTypeRecordHandler.cs | IVoiceTypeGetter | 3 | 0 | 0 | 0 | 1 |  |
| VolumetricLightingRecordHandler.cs | IVolumetricLightingGetter | 2 | 12 | 0 | 0 | 1 |  |
| WaterRecordHandler.cs | IWaterGetter | 61 | 0 | 0 | 0 | 10 |  |
| WeaponRecordHandler.cs | IWeaponGetter | 27 | 5 | 0 | 0 | 3 |  |
| WeatherRecordHandler.cs | IWeatherGetter | 61 | 0 | 0 | 0 | 3 |  |
| WordOfPowerRecordHandler.cs | IWordOfPowerGetter | 4 | 0 | 0 | 0 | 1 |  |
| WorldspaceRecordHandler.cs | IWorldspaceGetter | 21 | 3 | 0 | 0 | 13 |  |

## AcousticSpaceRecordHandler.cs

Getter: `IAcousticSpaceGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ActionRecordHandler.cs

Getter: `IActionRecordGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ActivatorRecordHandler.cs

Getter: `IActivatorGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ActorValueInformationRecordHandler.cs

Getter: `IActorValueInformationGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `CNAM` | `Nullable<ReadOnlyMemorySlice<Byte>>` | IntentionalExclusion |  | CNAM | Engine-managed binary data excluded by the project runtime-field policy; the winning override is preserved. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## AddonNodeRecordHandler.cs

Getter: `IAddonNodeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## AlchemicalApparatusRecordHandler.cs

Getter: `IAlchemicalApparatusGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## AmmunitionRecordHandler.cs

Getter: `IAmmunitionGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the AMMO DATA layout/breaks; it is not an independent xEdit semantic field. |
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## AnimatedObjectRecordHandler.cs

Getter: `IAnimatedObjectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ArmorAddonRecordHandler.cs

Getter: `IArmorAddonGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `UInt16` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown2` | `Byte` | IntentionalExclusion |  | Unknown2 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## ArmorRecordHandler.cs

Getter: `IArmorGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Armor.MajorFlags is a typed view over MajorRecordFlagsRaw. Armor uses one composite MajorRecordFlagsRaw handler for common and record-specific bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | SkyrimMajorRecordFlags is a typed view over MajorRecordFlagsRaw. Armor uses one composite MajorRecordFlagsRaw handler to avoid duplicate writes to the same header integer. |

## ArtObjectRecordHandler.cs

Getter: `IArtObjectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## AssociationTypeRecordHandler.cs

Getter: `IAssociationTypeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## BodyPartDataRecordHandler.cs

Getter: `IBodyPartDataGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## BookRecordHandler.cs

Getter: `IBookGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unused` | `UInt16` | IntentionalExclusion |  | Unused | Unused BOOK storage is not an independently editable semantic field; the winning value is preserved. |

## CameraPathRecordHandler.cs

Getter: `ICameraPathGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## CameraShotRecordHandler.cs

Getter: `ICameraShotGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the CAMS DATA layout/breaks; it is not an independent xEdit semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## CellRecordHandler.cs

Getter: `ICellGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Landscape` | `ILandscapeGetter` | RuntimeOrNavigation |  | Landscape | Runtime-managed LAND child data is excluded from CELL property forwarding. |
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `NavigationMeshes` | `IReadOnlyList<INavigationMeshGetter>` | RuntimeOrNavigation |  | NavigationMeshes | Navigation child data is excluded from CELL property forwarding. |
| `Persistent` | `IReadOnlyList<IPlacedGetter>` | RuntimeOrNavigation |  | Persistent | Child placed records in the CELL persistent GRUP; they are processed as their own major records rather than as a CELL property. |
| `PersistentTimestamp` | `Int32` | RuntimeOrNavigation |  | PersistentTimestamp | GRUP header metadata for persistent CELL children, not an xEdit CELL field. |
| `PersistentUnknownGroupData` | `Int32` | RuntimeOrNavigation |  | PersistentUnknownGroupData | GRUP header metadata for persistent CELL children, not an xEdit CELL field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Temporary` | `IReadOnlyList<IPlacedGetter>` | RuntimeOrNavigation |  | Temporary | Child records in the CELL temporary GRUP; they are processed as their own major records rather than as a CELL property. |
| `TemporaryTimestamp` | `Int32` | RuntimeOrNavigation |  | TemporaryTimestamp | GRUP header metadata for temporary CELL children, not an xEdit CELL field. |
| `TemporaryUnknownGroupData` | `Int32` | RuntimeOrNavigation |  | TemporaryUnknownGroupData | GRUP header metadata for temporary CELL children, not an xEdit CELL field. |
| `Timestamp` | `Int32` | RuntimeOrNavigation |  | Timestamp | CELL child-group header metadata, not a normal record property. |
| `UnknownGroupData` | `Int32` | RuntimeOrNavigation |  | UnknownGroupData | CELL child-group header metadata, not a normal record property. |
| `WaterHeight` | `Nullable<Single>` | IntentionalExclusion |  | WaterHeight | Runtime-managed CELL field excluded from conflict forwarding; the winning override is preserved. |

## ClassRecordHandler.cs

Getter: `IClassGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `Int32` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown2` | `Byte` | IntentionalExclusion |  | Unknown2 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## ClimateRecordHandler.cs

Getter: `IClimateGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## CollisionLayerRecordHandler.cs

Getter: `ICollisionLayerGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ColorRecordHandler.cs

Getter: `IColorRecordGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## CombatStyleRecordHandler.cs

Getter: `ICombatStyleGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `CSGDDataTypeState` | `CSGDDataType` | SerializationState |  | CSGDDataTypeState | Mutagen discriminator controlling the CSTY CSGD layout/breaks; it is not an independent xEdit semantic field. |
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ConstructibleObjectRecordHandler.cs

Getter: `IConstructibleObjectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ContainerRecordHandler.cs

Getter: `IContainerGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## DebrisRecordHandler.cs

Getter: `IDebrisGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## DefaultObjectManagerRecordHandler.cs

Getter: `IDefaultObjectManagerGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## DialogBranchRecordHandler.cs

Getter: `IDialogBranchGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## DialogResponseRecordHandler.cs

Getter: `IDialogResponsesGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `PreviousDialog` | `IFormLinkNullableGetter<IDialogResponsesGetter>` | RuntimeOrNavigation |  | PreviousDialog | Runtime/structural dialog linkage is preserved from the winning override rather than conflict-forwarded. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `UnknownData` | `IReadOnlyList<IDialogResponsesUnknownDataGetter>` | IntentionalExclusion |  | UnknownData | Opaque ordered SCHR/QNAM/NEXT payloads cannot be safely merged across plugins; the winning list is preserved atomically. |

## DialogTopicRecordHandler.cs

Getter: `IDialogTopicGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Responses` | `IReadOnlyList<IDialogResponsesGetter>` | RuntimeOrNavigation |  | Responses | INFO child major records stored under the DIAL group; INFO records have their own handler. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Timestamp` | `Int32` | RuntimeOrNavigation |  | Timestamp | DIAL child-group header metadata, not a normal DIAL field. |
| `Unknown` | `Int32` | RuntimeOrNavigation |  | Unknown | DIAL child-group header metadata, not a normal DIAL field. |

## DialogViewRecordHandler.cs

Getter: `IDialogViewGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## DoorRecordHandler.cs

Getter: `IDoorGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## DualCastDataRecordHandler.cs

Getter: `IDualCastDataGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## EffectShaderRecordHandler.cs

Getter: `IEffectShaderGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling DATA layout/breaks; it is not an independent xEdit semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## EncounterZoneRecordHandler.cs

Getter: `IEncounterZoneGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling DATA layout/breaks; it is not an independent xEdit semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## EquipTypeRecordHandler.cs

Getter: `IEquipTypeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ExplosionRecordHandler.cs

Getter: `IExplosionGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the EXPL DATA layout/breaks; it is not an independent xEdit semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## EyesRecordHandler.cs

Getter: `IEyesGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## FactionRecordHandler.cs

Getter: `IFactionGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## FloraRecordHandler.cs

Getter: `IFloraGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## FootstepRecordHandler.cs

Getter: `IFootstepGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## FootstepSetRecordHandler.cs

Getter: `IFootstepSetGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## FormIdRecordHandler.cs

Getter: `IFormListGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## FurnitureRecordHandler.cs

Getter: `IFurnitureGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## GameSettingBoolRecordHandler.cs

Getter: `IGameSettingBoolGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## GameSettingFloatRecordHandler.cs

Getter: `IGameSettingFloatGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## GameSettingIntRecordHandler.cs

Getter: `IGameSettingIntGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## GameSettingStringRecordHandler.cs

Getter: `IGameSettingStringGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## GlobalFloatRecordHandler.cs

Getter: `IGlobalFloatGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `TypeChar` | `Char` | SerializationState |  | TypeChar | Read-only GLOB subtype discriminator fixed by the concrete record class (f/l/s); independently selecting a character would require replacing the record variant. |

## GlobalIntRecordHandler.cs

Getter: `IGlobalIntGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `TypeChar` | `Char` | SerializationState |  | TypeChar | Read-only GLOB subtype discriminator fixed by the concrete record class (f/l/s); independently selecting a character would require replacing the record variant. |

## GlobalShortRecordHandler.cs

Getter: `IGlobalShortGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `TypeChar` | `Char` | SerializationState |  | TypeChar | Read-only GLOB subtype discriminator fixed by the concrete record class (f/l/s); independently selecting a character would require replacing the record variant. |

## GlobalUnknownRecordHandler.cs

Getter: `IGlobalUnknownGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## GrassRecordHandler.cs

Getter: `IGrassGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `Byte` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown2` | `UInt16` | IntentionalExclusion |  | Unknown2 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown3` | `ReadOnlyMemorySlice<Byte>` | IntentionalExclusion |  | Unknown3 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## HazardRecordHandler.cs

Getter: `IHazardGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## HeadPartRecordHandler.cs

Getter: `IHeadPartGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## IdleAnimationRecordHandler.cs

Getter: `IIdleAnimationGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## IdleMarkerRecordHandler.cs

Getter: `IIdleMarkerGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ImageSpaceAdapterRecordHandler.cs

Getter: `IImageSpaceAdapterGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Animatable` | `Boolean` | AliasOrDuplicate |  | Animatable | Covered by the AnimationSettings atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `BlurRadius` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | BlurRadius | Covered by the BlurRadius atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `CinematicBrightnessAdd` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | CinematicBrightnessAdd | Covered by the CinematicBrightness atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `CinematicBrightnessMult` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | CinematicBrightnessMult | Covered by the CinematicBrightness atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `CinematicContrastAdd` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | CinematicContrastAdd | Covered by the CinematicContrast atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `CinematicContrastMult` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | CinematicContrastMult | Covered by the CinematicContrast atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `CinematicSaturationAdd` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | CinematicSaturationAdd | Covered by the CinematicSaturation atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `CinematicSaturationMult` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | CinematicSaturationMult | Covered by the CinematicSaturation atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `DepthOfFieldDistance` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | DepthOfFieldDistance | Covered by the DepthOfField atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `DepthOfFieldFlags` | `DepthOfFieldFlag` | AliasOrDuplicate |  | DepthOfFieldFlags | Covered by the DepthOfField atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `DepthOfFieldRange` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | DepthOfFieldRange | Covered by the DepthOfField atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `DepthOfFieldStrength` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | DepthOfFieldStrength | Covered by the DepthOfField atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `DoubleVisionStrength` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | DoubleVisionStrength | Covered by the DoubleVisionStrength atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `Duration` | `Single` | AliasOrDuplicate |  | Duration | Covered by the AnimationSettings atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `FadeColor` | `IReadOnlyList<IColorFrameGetter>` | AliasOrDuplicate |  | FadeColor | Covered by the FadeColor atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrBloomBlurRadiusAdd` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrBloomBlurRadiusAdd | Covered by the HdrBloomBlurRadius atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrBloomBlurRadiusMult` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrBloomBlurRadiusMult | Covered by the HdrBloomBlurRadius atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrBloomScaleAdd` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrBloomScaleAdd | Covered by the HdrBloomScale atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrBloomScaleMult` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrBloomScaleMult | Covered by the HdrBloomScale atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrBloomThresholdAdd` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrBloomThresholdAdd | Covered by the HdrBloomThreshold atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrBloomThresholdMult` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrBloomThresholdMult | Covered by the HdrBloomThreshold atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrEyeAdaptSpeedAdd` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrEyeAdaptSpeedAdd | Covered by the HdrEyeAdaptSpeed atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrEyeAdaptSpeedMult` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrEyeAdaptSpeedMult | Covered by the HdrEyeAdaptSpeed atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrSkyScaleAdd` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrSkyScaleAdd | Covered by the HdrSkyScale atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrSkyScaleMult` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrSkyScaleMult | Covered by the HdrSkyScale atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrSunlightScaleAdd` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrSunlightScaleAdd | Covered by the HdrSunlightScale atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrSunlightScaleMult` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrSunlightScaleMult | Covered by the HdrSunlightScale atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrTargetLumMaxAdd` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrTargetLumMaxAdd | Covered by the HdrTargetLumMax atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrTargetLumMaxMult` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrTargetLumMaxMult | Covered by the HdrTargetLumMax atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrTargetLumMinAdd` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrTargetLumMinAdd | Covered by the HdrTargetLumMin atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `HdrTargetLumMinMult` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | HdrTargetLumMinMult | Covered by the HdrTargetLumMin atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `MotionBlurStrength` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | MotionBlurStrength | Covered by the MotionBlurStrength atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `RadialBlurCenter` | `P2Float` | AliasOrDuplicate |  | RadialBlurCenter.Absolute<br>RadialBlurCenter.Length<br>RadialBlurCenter.Magnitude<br>RadialBlurCenter.Normalized<br>RadialBlurCenter.SqrMagnitude<br>RadialBlurCenter.X<br>RadialBlurCenter.Y | Covered by the RadialBlur atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `RadialBlurDownStart` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | RadialBlurDownStart | Covered by the RadialBlur atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `RadialBlurRampDown` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | RadialBlurRampDown | Covered by the RadialBlur atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `RadialBlurRampUp` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | RadialBlurRampUp | Covered by the RadialBlur atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `RadialBlurStart` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | RadialBlurStart | Covered by the RadialBlur atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `RadialBlurStrength` | `IReadOnlyList<IKeyFrameGetter>` | AliasOrDuplicate |  | RadialBlurStrength | Covered by the RadialBlur atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `RadialBlurUseTarget` | `Boolean` | AliasOrDuplicate |  | RadialBlurUseTarget | Covered by the RadialBlur atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `TintColor` | `IReadOnlyList<IColorFrameGetter>` | AliasOrDuplicate |  | TintColor | Covered by the TintColor atomic unit; it is not registered independently because splitting the group could create an effect no source mod authored. |
| `Unknown08` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown08 | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown09` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown09 | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown0A` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown0A | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown0B` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown0B | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown0C` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown0C | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown0D` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown0D | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown0E` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown0E | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown0F` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown0F | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown10` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown10 | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown14` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown14 | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown48` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown48 | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown49` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown49 | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown4A` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown4A | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown4B` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown4B | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown4C` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown4C | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown4D` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown4D | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown4E` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown4E | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown4F` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown4F | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown50` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown50 | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |
| `Unknown54` | `IReadOnlyList<IKeyFrameGetter>` | IntentionalExclusion |  | Unknown54 | Unknown keyframe collection outside the supported semantic conflict surface; the winning collection is preserved. |

## ImageSpaceRecordHandler.cs

Getter: `IImageSpaceGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ImpactDataSetRecordHandler.cs

Getter: `IImpactDataSetGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ImpactRecordHandler.cs

Getter: `IImpactGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `Int16` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## IngestibleRecordHandler.cs

Getter: `IIngestibleGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## IngredientRecordHandler.cs

Getter: `IIngredientGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## KeyRecordHandler.cs

Getter: `IKeyGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## KeywordRecordHandler.cs

Getter: `IKeywordGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## LandscapeRecordHandler.cs

Getter: `ILandscapeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## LandscapeTextureRecordHandler.cs

Getter: `ILandscapeTextureGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## LensFlareRecordHandler.cs

Getter: `ILensFlareGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## LeveledItemRecordHandler.cs

Getter: `ILeveledItemGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## LeveledNpcRecordHandler.cs

Getter: `ILeveledNpcGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## LeveledSpellRecordHandler.cs

Getter: `ILeveledSpellGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## LightingTemplateRecordHandler.cs

Getter: `ILightingTemplateGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the LGTM DATA layout/breaks; it is not an independent xEdit semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `Int32` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## LightRecordHandler.cs

Getter: `ILightGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## LoadScreenRecordHandler.cs

Getter: `ILoadScreenGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## LocationRecordHandler.cs

Getter: `ILocationGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `EnableParentReferencesAdded` | `IReadOnlyList<IEnableParentReferenceGetter>` | IntentionalExclusion |  | EnableParentReferencesAdded | Serialized xEdit-benign LCTN Added bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `EnableParentReferencesStatic` | `IReadOnlyList<IEnableParentReferenceGetter>` | IntentionalExclusion |  | EnableParentReferencesStatic | Serialized xEdit-benign LCTN Master bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `InitiallyDisabledReferencesAdded` | `IReadOnlyList<IFormLinkGetter<IPlacedGetter>>` | IntentionalExclusion |  | InitiallyDisabledReferencesAdded | Serialized xEdit-benign LCTN Added bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `InitiallyDisabledReferencesStatic` | `IReadOnlyList<IFormLinkGetter<IPlacedGetter>>` | IntentionalExclusion |  | InitiallyDisabledReferencesStatic | Serialized xEdit-benign LCTN Master bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `LocationRefTypeReferencesAdded` | `IReadOnlyList<ILocationRefTypeReferenceGetter>` | IntentionalExclusion |  | LocationRefTypeReferencesAdded | Serialized xEdit-benign LCTN Added bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `LocationRefTypeReferencesRemoved` | `IReadOnlyList<IFormLinkGetter<IPlacedSimpleGetter>>` | IntentionalExclusion |  | LocationRefTypeReferencesRemoved | Serialized xEdit-benign LCTN Removed bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `LocationRefTypeReferencesStatic` | `IReadOnlyList<ILocationRefTypeReferenceGetter>` | IntentionalExclusion |  | LocationRefTypeReferencesStatic | Serialized xEdit-benign LCTN Master bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `PersistentActorReferencesAdded` | `IReadOnlyList<IPersistentActorReferenceGetter>` | IntentionalExclusion |  | PersistentActorReferencesAdded | Serialized xEdit-benign LCTN Added bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `PersistentActorReferencesRemoved` | `IReadOnlyList<IFormLinkGetter<IPlacedSimpleGetter>>` | IntentionalExclusion |  | PersistentActorReferencesRemoved | Serialized xEdit-benign LCTN Removed bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `PersistentActorReferencesStatic` | `IReadOnlyList<IPersistentActorReferenceGetter>` | IntentionalExclusion |  | PersistentActorReferencesStatic | Serialized xEdit-benign LCTN Master bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `UniqueActorReferencesAdded` | `IReadOnlyList<IUniqueActorReferenceGetter>` | IntentionalExclusion |  | UniqueActorReferencesAdded | Serialized xEdit-benign LCTN Added bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `UniqueActorReferencesRemoved` | `IReadOnlyList<IFormLinkGetter<INpcGetter>>` | IntentionalExclusion |  | UniqueActorReferencesRemoved | Serialized xEdit-benign LCTN Removed bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `UniqueActorReferencesStatic` | `IReadOnlyList<IUniqueActorReferenceGetter>` | IntentionalExclusion |  | UniqueActorReferencesStatic | Serialized xEdit-benign LCTN Master bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `WorldspaceCellsAdded` | `IReadOnlyList<ILocationCoordinateGetter>` | IntentionalExclusion |  | WorldspaceCellsAdded | Serialized xEdit-benign LCTN Added bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `WorldspaceCellsRemoved` | `IReadOnlyList<ILocationCoordinateGetter>` | IntentionalExclusion |  | WorldspaceCellsRemoved | Serialized xEdit-benign LCTN Removed bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `WorldspaceCellsStatic` | `IReadOnlyList<ILocationCoordinateGetter>` | IntentionalExclusion |  | WorldspaceCellsStatic | Serialized xEdit-benign LCTN Master bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |

## LocationReferenceTypeRecordHandler.cs

Getter: `ILocationReferenceTypeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## MagicEffectRecordHandler.cs

Getter: `IMagicEffectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown1` | `UInt16` | IntentionalExclusion |  | Unknown1 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## MaterialObjectRecordHandler.cs

Getter: `IMaterialObjectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the MATO DATA layout/breaks; it is not an independent xEdit semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## MaterialTypeRecordHandler.cs

Getter: `IMaterialTypeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## MessageRecordHandler.cs

Getter: `IMessageGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## MiscItemRecordHandler.cs

Getter: `IMiscItemGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## MoveableStaticRecordHandler.cs

Getter: `IMoveableStaticGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## MovementTypeRecordHandler.cs

Getter: `IMovementTypeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SPEDDataTypeState` | `SPEDDataType` | SerializationState |  | SPEDDataTypeState | Mutagen discriminator controlling the MOVT SPED layout/breaks; it is not an independent xEdit semantic field. |

## MusicTrackRecordHandler.cs

Getter: `IMusicTrackGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## MusicTypeRecordHandler.cs

Getter: `IMusicTypeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## NavigationMeshRecordHandler.cs

Getter: `INavigationMeshGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## NpcRecordHandler.cs

Getter: `INpcGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ObjectEffectRecordHandler.cs

Getter: `IObjectEffectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `ENITDataTypeState` | `ENITDataType` | SerializationState |  | ENITDataTypeState | Mutagen discriminator controlling ENIT layout/breaks; it is not an independent xEdit semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## OutfitRecordHandler.cs

Getter: `IOutfitGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## PackageRecordHandler.cs

Getter: `IPackageGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Data` | `IReadOnlyDictionary<SByte, IAPackageDataGetter>` | IntentionalExclusion |  | Data | Temporarily not forwarded: Mutagen 0.54.4 sorts PACK value records by UNAM key and changes their physical xEdit row order. |
| `DataInputVersion` | `Int32` | IntentionalExclusion |  | DataInputVersion | Temporarily not forwarded with the disabled PackageTemplateGraph atomic unit because its fields must share one owner. |
| `PackageTemplate` | `IFormLinkGetter<IPackageGetter>` | IntentionalExclusion |  | PackageTemplate | Temporarily not forwarded: PackageTemplateGraph is disabled because Mutagen 0.54.4 reorders physical PACK data rows by UNAM key while writing. |
| `ProcedureTree` | `IReadOnlyList<IPackageBranchGetter>` | IntentionalExclusion |  | ProcedureTree | Temporarily not forwarded with PackageTemplateGraph because branch DataInputIndices refer to the grouped package data dictionary. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `Byte` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown2` | `UInt16` | IntentionalExclusion |  | Unknown2 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown3` | `ReadOnlyMemorySlice<Byte>` | IntentionalExclusion |  | Unknown3 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown4` | `Nullable<Int32>` | IntentionalExclusion |  | Unknown4 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `XnamMarker` | `ReadOnlyMemorySlice<Byte>` | IntentionalExclusion |  | XnamMarker | Temporarily not forwarded with the disabled PackageTemplateGraph serialization unit. |

## PerkRecordHandler.cs

Getter: `IPerkGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | PERK-specific major flags are a typed view over MajorRecordFlagsRaw and are owned by that single composite flag handler. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Common Skyrim major flags are a typed view over MajorRecordFlagsRaw and are owned by the PERK composite flag handler. |

## PlacedArrowRecordHandler.cs

Getter: `IPlacedArrowGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## PlacedBarrierRecordHandler.cs

Getter: `IPlacedBarrierGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## PlacedBeamRecordHandler.cs

Getter: `IPlacedBeamGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## PlacedConeRecordHandler.cs

Getter: `IPlacedConeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## PlacedFlameRecordHandler.cs

Getter: `IPlacedFlameGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## PlacedHazardRecordHandler.cs

Getter: `IPlacedHazardGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## PlacedMissileRecordHandler.cs

Getter: `IPlacedMissileGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## PlacedNpcRecordHandler.cs

Getter: `IPlacedNpcGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## PlacedObjectRecordHandler.cs

Getter: `IPlacedObjectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `Int16` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## PlacedTrapRecordHandler.cs

Getter: `IPlacedTrapGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ProjectileRecordHandler.cs

Getter: `IProjectileGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the PROJ DATA layout/breaks; it is not an independent xEdit semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## QuestRecordHandler.cs

Getter: `IQuestGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `QuestFormVersion` | `Byte` | IntentionalExclusion |  | QuestFormVersion | xEdit marks QuestFormVersion cpIgnore; it is format metadata rather than an independently editable semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `Int32` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## RaceRecordHandler.cs

Getter: `IRaceGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the RACE DATA layout/breaks; it is not an independent xEdit semantic field. |
| `ExportingExtraNam2` | `Boolean` | SerializationState |  | ExportingExtraNam2 | Mutagen parser/writer state preserving an empty extra NAM2 marker; it is not an independent xEdit semantic field. |
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `Int16` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## RegionRecordHandler.cs

Getter: `IRegionGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## RelationshipRecordHandler.cs

Getter: `IRelationshipGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `Byte` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## ReverbParametersRecordHandler.cs

Getter: `IReverbParametersGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `Byte` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## SceneRecordHandler.cs

Getter: `ISceneGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unused` | `IScenePhaseUnusedDataGetter` | IntentionalExclusion |  | Unused.QNAM<br>Unused.SCDA<br>Unused.SCHR<br>Unused.SCRO<br>Unused.SCTX | Legacy SCHR/SCDA/SCTX/QNAM/SCRO storage from older Creation Kit versions is hidden by xEdit and excluded from semantic comparison and copying. |
| `Unused2` | `IScenePhaseUnusedDataGetter` | IntentionalExclusion |  | Unused2.QNAM<br>Unused2.SCDA<br>Unused2.SCHR<br>Unused2.SCRO<br>Unused2.SCTX | Legacy SCHR/SCDA/SCTX/QNAM/SCRO storage from older Creation Kit versions is hidden by xEdit and excluded from semantic comparison and copying. |
| `VirtualMachineAdapter` | `ISceneAdapterGetter` | AliasOrDuplicate |  | VirtualMachineAdapter.ObjectFormat<br>VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion<br>VirtualMachineAdapter.ScriptFragments.FileName<br>VirtualMachineAdapter.ScriptFragments.OnBegin.ExtraBindDataVersion<br>VirtualMachineAdapter.ScriptFragments.OnBegin.FragmentName<br>VirtualMachineAdapter.ScriptFragments.OnBegin.ScriptName<br>VirtualMachineAdapter.ScriptFragments.OnEnd.ExtraBindDataVersion<br>VirtualMachineAdapter.ScriptFragments.OnEnd.FragmentName<br>VirtualMachineAdapter.ScriptFragments.OnEnd.ScriptName<br>VirtualMachineAdapter.ScriptFragments.PhaseFragments<br>VirtualMachineAdapter.Scripts<br>VirtualMachineAdapter.Version | Covered without overlap by typed VMAD presence, version, object-format, scripts, and complete script-fragments handlers. |

## ScrollRecordHandler.cs

Getter: `IScrollGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ShaderParticleGeometryRecordHandler.cs

Getter: `IShaderParticleGeometryGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the SPGD DATA layout/breaks; it is not an independent xEdit semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## ShoutRecordHandler.cs

Getter: `IShoutGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## SoulGemRecordHandler.cs

Getter: `ISoulGemGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## SoundCategoryRecordHandler.cs

Getter: `ISoundCategoryGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## SoundDescriptorRecordHandler.cs

Getter: `ISoundDescriptorGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## SoundMarkerRecordHandler.cs

Getter: `ISoundMarkerGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## SoundOutputModelRecordHandler.cs

Getter: `ISoundOutputModelGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## SpellRecordHandler.cs

Getter: `ISpellGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## StaticRecordHandler.cs

Getter: `IStaticGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DNAMDataTypeState` | `DNAMDataType` | SerializationState |  | DNAMDataTypeState | Mutagen discriminator controlling DNAM layout/breaks; it is not an independent xEdit semantic field. |
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | STAT-specific major flags are a typed view over MajorRecordFlagsRaw and are owned by that single composite flag handler. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Common Skyrim major flags are a typed view over MajorRecordFlagsRaw and are owned by the STAT composite flag handler. |
| `Unused` | `ReadOnlyMemorySlice<Byte>` | IntentionalExclusion |  | Unused | Unused STAT storage is serialization-only data; the winning value is preserved. |

## StoryManagerBranchNodeRecordHandler.cs

Getter: `IStoryManagerBranchNodeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## StoryManagerEventNodeRecordHandler.cs

Getter: `IStoryManagerEventNodeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## StoryManagerQuestNodeRecordHandler.cs

Getter: `IStoryManagerQuestNodeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## TalkingActivatorRecordHandler.cs

Getter: `ITalkingActivatorGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `FNAM` | `Nullable<Int16>` | IntentionalExclusion |  | FNAM | xEdit defines TACT FNAM as unknown cpIgnore data; the winning binary value is preserved. |
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | TACT-specific major flags are a typed view over MajorRecordFlagsRaw and are owned by that single composite flag handler. |
| `PNAM` | `Nullable<Int32>` | IntentionalExclusion |  | PNAM | xEdit defines TACT PNAM as unknown cpIgnore data; the winning binary value is preserved. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Common Skyrim major flags are a typed view over MajorRecordFlagsRaw and are owned by the TACT composite flag handler. |

## TextureSetRecordHandler.cs

Getter: `ITextureSetGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Common Skyrim major flags are a typed view over MajorRecordFlagsRaw and are owned by that single raw flag handler. |

## TreeRecordHandler.cs

Getter: `ITreeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `ReadOnlyMemorySlice<Byte>` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## VisualEffectRecordHandler.cs

Getter: `IVisualEffectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## VoiceTypeRecordHandler.cs

Getter: `IVoiceTypeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## VolumetricLightingRecordHandler.cs

Getter: `IVolumetricLightingGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## WaterRecordHandler.cs

Getter: `IWaterGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DNAMDataTypeState` | `DNAMDataType` | SerializationState |  | DNAMDataTypeState | Mutagen discriminator controlling the WATR DNAM layout/breaks; it is not an independent xEdit semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `ReadOnlyMemorySlice<Byte>` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown2` | `Int32` | IntentionalExclusion |  | Unknown2 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown3` | `ReadOnlyMemorySlice<Byte>` | IntentionalExclusion |  | Unknown3 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown4` | `Int32` | IntentionalExclusion |  | Unknown4 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown5` | `ReadOnlyMemorySlice<Byte>` | IntentionalExclusion |  | Unknown5 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown6` | `Int32` | IntentionalExclusion |  | Unknown6 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown7` | `Int32` | IntentionalExclusion |  | Unknown7 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `UnusedNoisemaps` | `IReadOnlyList<String>` | IntentionalExclusion |  | UnusedNoisemaps | No exact writable Skyrim xEdit collection mapping is confirmed for UnusedNoisemaps; the winning value is preserved. |

## WeaponRecordHandler.cs

Getter: `IWeaponGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unused` | `Nullable<ReadOnlyMemorySlice<Byte>>` | IntentionalExclusion |  | Unused | Unused WEAP storage is outside the supported semantic conflict surface; the winning value is preserved. |

## WeatherRecordHandler.cs

Getter: `IWeatherGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `NAM0DataTypeState` | `NAM0DataType` | SerializationState |  | NAM0DataTypeState | Mutagen discriminator controlling the WTHR NAM0 layout/breaks; it is not an independent xEdit semantic field. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |
| `Unknown` | `UInt16` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## WordOfPowerRecordHandler.cs

Getter: `IWordOfPowerGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which combines base, common Skyrim and configured record-specific bits and preserves unknown winner bits. |

## WorldspaceRecordHandler.cs

Getter: `IWorldspaceGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `CanopyShadow` | `AssetLinkGetter<SkyrimTextureAssetType>` | IntentionalExclusion |  | CanopyShadow | NNAM is marked unused/cpIgnore by xEdit; forwarding it could revive obsolete data, so the winning override is preserved. |
| `LargeReferences` | `IReadOnlyList<IWorldspaceGridReferenceGetter>` | RuntimeOrNavigation |  | LargeReferences | Worldspace child-group/container data rather than an independent WRLD record field. |
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | WRLD-specific major flags are a typed view of MajorRecordFlagsRaw and are owned by that single composite handler. |
| `MaxHeight` | `IWorldspaceMaxHeightGetter` | IntentionalExclusion |  | MaxHeight.CellData<br>MaxHeight.Max.IsZero<br>MaxHeight.Max.X<br>MaxHeight.Max.Y<br>MaxHeight.Min.IsZero<br>MaxHeight.Min.X<br>MaxHeight.Min.Y | MHDT is generated worldspace height data marked no-copy by xEdit; it is preserved from the winning override rather than conflict-forwarded. |
| `ObjectBoundsMax` | `P2Float` | IntentionalExclusion |  | ObjectBoundsMax.Absolute<br>ObjectBoundsMax.Length<br>ObjectBoundsMax.Magnitude<br>ObjectBoundsMax.Normalized<br>ObjectBoundsMax.SqrMagnitude<br>ObjectBoundsMax.X<br>ObjectBoundsMax.Y | Generated WRLD object bounds are kept winner-owned so independent component forwarding cannot create invalid bounds. |
| `ObjectBoundsMin` | `P2Float` | IntentionalExclusion |  | ObjectBoundsMin.Absolute<br>ObjectBoundsMin.Length<br>ObjectBoundsMin.Magnitude<br>ObjectBoundsMin.Normalized<br>ObjectBoundsMin.SqrMagnitude<br>ObjectBoundsMin.X<br>ObjectBoundsMin.Y | Generated WRLD object bounds are kept winner-owned so independent component forwarding cannot create invalid bounds. |
| `OffsetData` | `Nullable<ReadOnlyMemorySlice<Byte>>` | RuntimeOrNavigation |  | OffsetData | Worldspace child-group offset data maintained with child groups rather than ordinary WRLD property forwarding. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Owned by the single MajorRecordFlagsRaw handler, which preserves all shared and WRLD-specific header bits without overlapping writes. |
| `SubCells` | `IReadOnlyList<IWorldspaceBlockGetter>` | RuntimeOrNavigation |  | SubCells | CELL/block child groups under WRLD; child records are processed separately. |
| `SubCellsTimestamp` | `Int32` | RuntimeOrNavigation |  | SubCellsTimestamp | WRLD child-group header metadata, not a normal WRLD field. |
| `SubCellsUnknown` | `Int32` | RuntimeOrNavigation |  | SubCellsUnknown | WRLD child-group header metadata, not a normal WRLD field. |
| `TopCell` | `ICellGetter` | RuntimeOrNavigation |  | TopCell | Top-level CELL child record under WRLD; CELL records are processed separately. |
| `WorldMapCellOffset` | `P3Float` | AliasOrDuplicate |  | WorldMapCellOffset.Absolute<br>WorldMapCellOffset.Length<br>WorldMapCellOffset.Magnitude<br>WorldMapCellOffset.Normalized<br>WorldMapCellOffset.SqrMagnitude<br>WorldMapCellOffset.X<br>WorldMapCellOffset.Y<br>WorldMapCellOffset.Z | Handled atomically with WorldMapOffsetScale by WorldMapOffsetHandler because both values share the required ONAM structure. |

## Registered keys by handler

- **AcousticSpaceRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ObjectBounds`, `AmbientSound`, `UseSoundFromRegion`, `EnvironmentType`
- **ActionRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Color`
- **ActivatorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Destructible`, `Keywords`, `MarkerColor`, `LoopingSound`, `ActivationSound`, `WaterType`, `ActivateTextOverride`, `Flags`, `InteractionKeyword`
- **ActorValueInformationRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Description`, `Abbreviation`, `Skill`, `PerkTree`
- **AddonNodeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ModelAndBounds`, `NodeIndex`, `Sound`, `MasterParticleSystemCap`, `Flags`
- **AlchemicalApparatusRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Icons`, `Destructible`, `PickUpSound`, `PutDownSound`, `Quality`, `Description`, `Value`, `Weight`
- **AmmunitionRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `ModelAndBounds`, `Icons`, `Destructible`, `PickUpSound`, `PutDownSound`, `Description`, `Keywords`, `Projectile`, `Flags`, `Damage`, `Value`, `Weight`, `ShortName`
- **AnimatedObjectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Model`, `UnloadEvent`
- **ArmorAddonRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `WeightSliderEnabled.Male`, `WeightSliderEnabled.Female`, `WorldModel.Male.File`, `WorldModel.Male.AlternateTextures`, `WorldModel.Female.File`, `WorldModel.Female.AlternateTextures`, `FirstPersonModel.Male.File`, `FirstPersonModel.Male.AlternateTextures`, `FirstPersonModel.Female.File`, `FirstPersonModel.Female.AlternateTextures`, `AdditionalRaces`, `BodyTemplate.FirstPersonFlags`, `BodyTemplate.Flags`, `BodyTemplate.ArmorType`, `Priority.Male`, `Priority.Female`, `DetectionSoundValue`, `WeaponAdjust`, `Race`, `FootstepSound`, `ArtObject`, `SkinTexture.Male`, `SkinTexture.Female`, `TextureSwapList.Male`, `TextureSwapList.Female`
- **ArmorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `VirtualMachineAdapter`, `ObjectEffect`, `EnchantmentAmount`, `WorldModelAndBounds`, `BodyTemplate.FirstPersonFlags`, `BodyTemplate.Flags`, `BodyTemplate.ArmorType`, `Destructible`, `PickUpSound`, `PutDownSound`, `RagdollConstraintTemplate`, `EquipmentType`, `BashImpactDataSet`, `AlternateBlockMaterial`, `Race`, `Keywords`, `Description`, `Armature`, `Value`, `Weight`, `ArmorRating`, `TemplateArmor`
- **ArtObjectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ModelAndBounds`, `Type`
- **AssociationTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ParentTitle`, `Title`, `IsFamily`
- **BodyPartDataRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Model`, `Parts`
- **BookRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `ModelAndBounds`, `Value`, `Weight`, `Description`, `PickUpSound`, `PutDownSound`, `Keywords`, `BookText`, `Destructible`, `Flags`, `Type`, `Teaches`, `InventoryArt`, `VirtualMachineAdapter`, `Icons`
- **CameraPathRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Conditions`, `RelatedPaths`, `Zoom`, `ZoomMustHaveCameraShots`, `Shots`
- **CameraShotRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Model`, `Action`, `Location`, `Target`, `Flags`, `TimeMultiplierPlayer`, `TimeMultiplierTarget`, `TimeMultiplierGlobal`, `MaxTime`, `MinTime`, `TargetPercentBetweenActors`, `NearTargetDistance`, `ImageSpaceModifier`
- **CellRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Flags`, `Regions`, `Location`, `Owner`, `Water`, `Lighting`, `LightingTemplate`, `AcousticSpace`, `EncounterZone`, `Music`, `ImageSpace`, `SkyAndWeatherFromRegion`, `Grid`, `MaxHeightData`, `WaterNoiseTexture`, `WaterVelocity`, `XWCN`, `XWCS`, `OcclusionData`, `LNAM`, `FactionRank`, `LockList`, `WaterEnvironmentMap`
- **ClassRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Description`, `Icon`, `Teaches`, `MaxTrainingLevel`, `SkillWeights`, `BleedoutDefault`, `VoicePoints`, `StatWeights`
- **ClimateRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `WeatherTypes`, `SunTexture`, `SunGlareTexture`, `Model`, `SunriseBegin`, `SunriseEnd`, `SunsetBegin`, `SunsetEnd`, `Volatility`, `Moons`, `PhaseLength`
- **CollisionLayerRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Description`, `Index`, `DebugColor`, `Flags`, `Name`, `CollidesWith`
- **ColorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Color`, `Playable`
- **CombatStyleRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `OffensiveMult`, `DefensiveMult`, `GroupOffensiveMult`, `EquipmentScoreMultMelee`, `EquipmentScoreMultMagic`, `EquipmentScoreMultRanged`, `EquipmentScoreMultShout`, `EquipmentScoreMultUnarmed`, `EquipmentScoreMultStaff`, `AvoidThreatChance`, `CSMD`, `Melee`, `CloseRange`, `LongRangeStrafeMult`, `Flight`, `Flags`
- **ConstructibleObjectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Items`, `Conditions`, `CreatedObject`, `WorkbenchKeyword`, `CreatedObjectCount`
- **ContainerRecordHandler.cs**: `Name`, `EditorID`, `MajorRecordFlagsRaw`, `ModelAndBounds`, `Weight`, `Items`, `VirtualMachineAdapter`, `Destructible`, `Flags`, `OpenSound`, `CloseSound`
- **DebrisRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Models`
- **DefaultObjectManagerRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Objects`
- **DialogBranchRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Quest`, `Category`, `Flags`, `StartingTopic`
- **DialogResponseRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `DATA`, `VirtualMachineAdapter`, `Flags`, `ResetHours`, `Topic`, `FavorLevel`, `LinkTo`, `ResponseData`, `Responses`, `Conditions`, `Prompt`, `Speaker`, `WalkAwayTopic`, `AudioOutputOverride`
- **DialogTopicRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Priority`, `Branch`, `Quest`, `TopicFlags`, `Category`, `Subtype`, `SubtypeName`
- **DialogViewRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Quest`, `Branches`, `TNAMs`, `ENAM`, `DNAM`
- **DoorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Destructible`, `OpenSound`, `CloseSound`, `LoopSound`, `Flags`
- **DualCastDataRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ObjectBounds`, `Projectile`, `Explosion`, `EffectShader`, `HitEffectArt`, `ImpactDataSet`, `InheritScale`
- **EffectShaderRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `FillTexture`, `ParticleShaderTexture`, `HolesTexture`, `MembranePaletteTexture`, `ParticlePaletteTexture`, `EffectShaderData`
- **EncounterZoneRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Owner`, `Location`, `Rank`, `MinLevel`, `MaxLevel`, `Flags`
- **EquipTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SlotParents`, `UseAllParents`
- **ExplosionRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `ObjectEffect`, `ImageSpaceModifier`, `Light`, `Sound1`, `Sound2`, `ImpactDataSet`, `PlacedObject`, `SpawnProjectile`, `Force`, `Damage`, `Radius`, `ISRadius`, `VerticalOffsetMult`, `Flags`, `SoundLevel`
- **EyesRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Icon`, `Flags`
- **FactionRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Relations`, `Ranks`, `Conditions`, `Flags`, `ExteriorJailMarker`, `FollowerWaitMarker`, `StolenGoodsContainer`, `PlayerInventoryContainer`, `SharedCrimeFactionList`, `JailOutfit`, `CrimeValues`, `VendorBuySellList`, `MerchantContainer`, `VendorValues`, `VendorLocation`
- **FloraRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Destructible`, `Keywords`, `PNAM`, `ActivateTextOverride`, `FNAM`, `Ingredient`, `HarvestSound`, `Production`
- **FootstepRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ImpactDataSet`, `Tag`
- **FootstepSetRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `WalkForwardFootsteps`, `RunForwardFootsteps`, `WalkForwardAlternateFootsteps`, `RunForwardAlternateFootsteps`, `WalkForwardAlternateFootsteps2`
- **FormIdRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Items`
- **FurnitureRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Destructible`, `Keywords`, `PNAM`, `Flags`, `InteractionKeyword`, `WorkbenchData`, `AssociatedSpell`, `Markers`, `ModelFilename`
- **GameSettingBoolRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Data`
- **GameSettingFloatRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Data`
- **GameSettingIntRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Data`
- **GameSettingStringRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Data`
- **GlobalFloatRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Data`
- **GlobalIntRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Data`
- **GlobalShortRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Data`
- **GlobalUnknownRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `TypeChar`, `Data`
- **GrassRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ModelAndBounds`, `Density`, `MinSlope`, `MaxSlope`, `UnitsFromWater`, `UnitsFromWaterType`, `PositionRange`, `HeightRange`, `ColorRange`, `WavePeriod`, `Flags`
- **HazardRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `ModelAndBounds`, `ImageSpaceModifier`, `Limit`, `Radius`, `Lifetime`, `ImageSpaceRadius`, `TargetInterval`, `Flags`, `Spell`, `Light`, `ImpactDataSet`, `Sound`
- **HeadPartRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Model`, `Flags`, `Type`, `ExtraParts`, `Parts`, `TextureSet`, `Color`, `ValidRaces`
- **IdleAnimationRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Conditions`, `Filename`, `AnimationEvent`, `RelatedIdles`, `LoopingSecondsMin`, `LoopingSecondsMax`, `Flags`, `AnimationGroupSection`, `ReplayDelay`
- **IdleMarkerRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Flags`, `IdleTimer`, `Animations`, `ModelAndBounds`
- **ImageSpaceAdapterRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `AnimationSettings`, `BlurRadius`, `DoubleVisionStrength`, `TintColor`, `FadeColor`, `RadialBlur`, `DepthOfField`, `MotionBlurStrength`, `HdrEyeAdaptSpeed`, `HdrBloomBlurRadius`, `HdrBloomThreshold`, `HdrBloomScale`, `HdrTargetLumMin`, `HdrTargetLumMax`, `HdrSunlightScale`, `HdrSkyScale`, `CinematicSaturation`, `CinematicBrightness`, `CinematicContrast`
- **ImageSpaceRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ENAM`, `Hdr`, `Cinematic`, `Tint`, `DepthOfField`
- **ImpactDataSetRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Impacts`
- **ImpactRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Model`, `Duration`, `Orientation`, `AngleThreshold`, `PlacementRadius`, `SoundLevel`, `NoDecalData`, `Result`, `Decal.Presence`, `Decal.Bounds`, `Decal.Depth`, `Decal.Shininess`, `Decal.Parallax`, `Decal.Flags`, `Decal.Color`, `TextureSet`, `SecondaryTextureSet`, `Sound1`, `Sound2`, `Hazard`
- **IngestibleRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Description`, `ModelAndBounds`, `Destructible`, `Icons`, `PickUpSound`, `PutDownSound`, `EquipmentType`, `Weight`, `Value`, `Keywords`, `Addiction`, `AddictionChance`, `ConsumeSound`, `Effects`, `Flags`
- **IngredientRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `VirtualMachineAdapter`, `ModelAndBounds`, `Icons`, `Destructible`, `EquipType`, `PickUpSound`, `PutDownSound`, `Value`, `Weight`, `IngredientValue`, `Flags`, `Effects`, `Keywords`
- **KeyRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `VirtualMachineAdapter`, `ModelAndBounds`, `Icons`, `Destructible`, `PickUpSound`, `PutDownSound`, `Keywords`, `Value`, `Weight`
- **KeywordRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Color`
- **LandscapeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Flags`, `VertexNormals`, `VertexHeightMap`, `VertexColors`, `Layers`, `Textures`
- **LandscapeTextureRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `TextureSet`, `MaterialType`, `HavokFriction`, `HavokRestitution`, `TextureSpecularExponent`, `Grasses`, `Flags`
- **LensFlareRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `FlareDefinition`
- **LeveledItemRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ObjectBounds`, `ChanceNone`, `Flags`, `Global`, `Entries`
- **LeveledNpcRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ChanceNone`, `Flags`, `Global`, `Entries`, `ModelAndBounds`
- **LeveledSpellRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ObjectBounds`, `ChanceNone`, `Flags`, `Entries`
- **LightingTemplateRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `AmbientColor`, `DirectionalColor`, `FogNearColor`, `FogNear`, `FogFar`, `DirectionalRotationXY`, `DirectionalRotationZ`, `DirectionalFade`, `FogClipDistance`, `FogPower`, `AmbientColors`, `FogFarColor`, `FogMax`, `LightFadeStartDistance`, `LightFadeEndDistance`, `DirectionalAmbientColors`
- **LightRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `VirtualMachineAdapter`, `ModelAndBounds`, `Icons`, `Destructible`, `Time`, `Radius`, `Color`, `Flags`, `FalloffExponent`, `FOV`, `NearClip`, `FlickerPeriod`, `FlickerIntensityAmplitude`, `FlickerMovementAmplitude`, `Value`, `Weight`, `FadeValue`, `Sound`, `Lens`
- **LoadScreenRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Icons`, `Description`, `Conditions`, `LoadingScreenNif`, `InitialScale`, `InitialRotation`, `RotationOffsetConstraints`, `InitialTranslationOffset`, `CameraPath`
- **LocationRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Keywords`, `ParentLocation`, `Music`, `UnreportedCrimeFaction`, `WorldLocationMarkerRef`, `WorldLocationRadius`, `HorseMarkerRef`, `Color`
- **LocationReferenceTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Color`
- **MagicEffectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `VirtualMachineAdapter`, `Description`, `BaseCost`, `Flags`, `CastType`, `TargetType`, `MagicSkill`, `ResistValue`, `SecondActorValue`, `CastingSoundLevel`, `MenuDisplayObject`, `Keywords`, `CastingLight`, `HitShader`, `EnchantShader`, `Projectile`, `Explosion`, `CastingArt`, `HitEffectArt`, `ImpactData`, `DualCastArt`, `EnchantArt`, `HitVisuals`, `EnchantVisuals`, `EquipAbility`, `ImageSpaceModifier`, `PerkToApply`, `TaperWeight`, `MinimumSkillLevel`, `SpellmakingArea`, `SpellmakingCastingTime`, `TaperCurve`, `TaperDuration`, `SecondActorValueWeight`, `SkillUsageMultiplier`, `DualCastScale`, `ScriptEffectAIScore`, `ScriptEffectAIDelayTime`, `CounterEffects`, `Sounds`, `Archetype`, `Conditions`
- **MaterialObjectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Model`, `DNAMs`, `FalloffScale`, `FalloffBias`, `NoiseUvScale`, `MaterialUvScale`, `ProjectionVector`, `NormalDampener`, `SinglePassColor`, `Flags`, `HasSnow`
- **MaterialTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Parent`, `Name`, `HavokDisplayColor`, `Buoyancy`, `Flags`, `HavokImpactDataSet`
- **MessageRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Description`, `Name`, `INAM`, `Quest`, `Flags`, `DisplayTime`, `MenuButtons`
- **MiscItemRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `VirtualMachineAdapter`, `ModelAndBounds`, `Icons`, `Destructible`, `PickUpSound`, `PutDownSound`, `Keywords`, `Value`, `Weight`
- **MoveableStaticRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `ModelAndBounds`, `Destructible`, `Flags`, `LoopingSound`
- **MovementTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `LeftWalk`, `LeftRun`, `RightWalk`, `RightRun`, `ForwardWalk`, `ForwardRun`, `BackWalk`, `BackRun`, `RotateInPlaceWalk`, `RotateInPlaceRun`, `RotateWhileMovingRun`, `AnimationChangeThresholds`
- **MusicTrackRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Type`, `Duration`, `FadeOut`, `TrackFilename`, `FinaleFilename`, `LoopData`, `CuePoints`, `Conditions`, `Tracks`
- **MusicTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Flags`, `Data`, `FadeDuration`, `Tracks`
- **NavigationMeshRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Data`, `ONAM`, `PNAM`, `NNAM`
- **NpcRecordHandler.cs**: `Name`, `MajorRecordFlagsRaw`, `DeathItem`, `CombatOverridePackageList`, `SpectatorOverridePackageList`, `Configuration.Flags`, `Configuration.MagickaOffset`, `Configuration.StaminaOffset`, `Configuration.Level`, `Configuration.CalcMinLevel`, `Configuration.CalcMaxLevel`, `Configuration.SpeedMultiplier`, `Configuration.DispositionBase`, `Configuration.TemplateFlags`, `Configuration.HealthOffset`, `Configuration.BleedoutOverride`, `EditorID`, `Class`, `AIData.Aggression`, `AIData.Confidence`, `AIData.EnergyLevel`, `AIData.Responsibility`, `AIData.Mood`, `AIData.Assistance`, `AIData.AggroRadiusBehavior`, `AIData.Warn`, `AIData.WarnOrAttack`, `AIData.Attack`, `ObserveDeadBodyOverridePackageList`, `Factions`, `Packages`, `ActorEffect`, `VirtualMachineAdapter`, `Items`, `Keywords`, `PlayerSkills.Health`, `PlayerSkills.Magicka`, `PlayerSkills.Stamina`, `PlayerSkills.FarAwayModelDistance`, `PlayerSkills.GearedUpWeapons`, `PlayerSkills.SkillValues`, `PlayerSkills.SkillOffsets`, `FaceMorph`, `FaceParts`, `TextureLighting`, `TintLayers`, `Race`, `Destructible`, `Height`, `Weight`, `ObjectBounds`, `Voice`, `Template`, `ShortName`, `NAM5`, `SoundLevel`, `HeadParts`, `WornArmor`, `AttackRace`, `HairColor`, `DefaultOutfit`, `FarAwayModel`, `Attacks`, `GuardWarnOverridePackageList`, `Perks`, `CombatStyle`, `GiftFilter`, `SleepingOutfit`, `DefaultPackageList`, `CrimeFaction`, `HeadTexture`, `Sound`
- **ObjectEffectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `ObjectBounds`, `EnchantmentCost`, `CastType`, `EnchantmentAmount`, `TargetType`, `EnchantType`, `ChargeTime`, `BaseEnchantment`, `WornRestrictions`, `Effects`, `Flags`
- **OutfitRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Items`
- **PackageRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `Flags`, `Type`, `InterruptOverride`, `PreferredSpeed`, `InterruptFlags`, `ScheduleMonth`, `ScheduleDayOfWeek`, `ScheduleDate`, `ScheduleHour`, `ScheduleMinute`, `ScheduleDurationInMinutes`, `Conditions`, `IdleAnimations`, `CombatStyle`, `OwnerQuest`, `OnBegin`, `OnEnd`, `OnChange`
- **PerkRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `Name`, `Description`, `Icons`, `Conditions`, `Trait`, `Level`, `NumRanks`, `Playable`, `Hidden`, `NextPerk`, `Effects`
- **PlacedArrowRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Projectile`, `VirtualMachineAdapter`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Reflections`, `LinkedReferences`, `ActivateParents`, `EnableParent`, `Emittance`, `MultiBoundReference`, `IgnoredBySandbox`, `LocationRefTypes`, `LocationReference`, `DistantLodData`, `Scale`, `Placement`
- **PlacedBarrierRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Projectile`, `VirtualMachineAdapter`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Reflections`, `LinkedReferences`, `ActivateParents`, `EnableParent`, `Emittance`, `MultiBoundReference`, `IgnoredBySandbox`, `LocationRefTypes`, `LocationReference`, `DistantLodData`, `Scale`, `Placement`
- **PlacedBeamRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Projectile`, `VirtualMachineAdapter`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Reflections`, `LinkedReferences`, `ActivateParents`, `EnableParent`, `Emittance`, `MultiBoundReference`, `IgnoredBySandbox`, `LocationRefTypes`, `LocationReference`, `DistantLodData`, `Scale`, `Placement`
- **PlacedConeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Projectile`, `VirtualMachineAdapter`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Reflections`, `LinkedReferences`, `ActivateParents`, `EnableParent`, `Emittance`, `MultiBoundReference`, `IgnoredBySandbox`, `LocationRefTypes`, `LocationReference`, `DistantLodData`, `Scale`, `Placement`
- **PlacedFlameRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Projectile`, `VirtualMachineAdapter`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Reflections`, `LinkedReferences`, `ActivateParents`, `EnableParent`, `Emittance`, `MultiBoundReference`, `IgnoredBySandbox`, `LocationRefTypes`, `LocationReference`, `DistantLodData`, `Scale`, `Placement`
- **PlacedHazardRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Hazard`, `VirtualMachineAdapter`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Reflections`, `LinkedReferences`, `ActivateParents`, `EnableParent`, `Emittance`, `MultiBoundReference`, `IgnoredBySandbox`, `LocationRefTypes`, `LocationReference`, `DistantLodData`, `Scale`, `Placement`
- **PlacedMissileRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Projectile`, `VirtualMachineAdapter`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Reflections`, `LinkedReferences`, `ActivateParents`, `EnableParent`, `Emittance`, `MultiBoundReference`, `IgnoredBySandbox`, `LocationRefTypes`, `LocationReference`, `DistantLodData`, `Scale`, `Placement`
- **PlacedNpcRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Base`, `EncounterZone`, `RagdollData`, `RagdollBipedData`, `Patrol`, `LevelModifier`, `MerchantContainer`, `Count`, `Radius`, `Health`, `LinkedReferences`, `ActivateParents`, `LinkedReferenceColor`, `PersistentLocation`, `LocationReference`, `IsIgnoredBySandbox`, `LocationRefTypes`, `HeadTrackingWeight`, `Horse`, `FavorCost`, `EnableParent`, `Owner`, `FactionRank`, `Emittance`, `MultiBoundReference`, `IsIgnoredBySandbox2`, `Scale`, `Placement`, `VirtualMachineAdapter`
- **PlacedObjectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Base`, `Owner`, `Scale`, `LocationReference`, `LinkedReferences`, `LinkedRooms`, `ImageSpace`, `LightingTemplate`, `BoundHalfExtents`, `Primitive`, `OcclusionPlane`, `Portals`, `RoomPortal`, `Radius`, `Reflections`, `LitWater`, `Emittance`, `TeleportMessageBox`, `MultiBoundReference`, `SpawnContainer`, `LeveledItemBaseObject`, `PersistentLocation`, `EncounterZone`, `NavigationDoorLink`, `LocationRefTypes`, `IsMultiBoundPrimitive`, `IsIgnoredBySandbox`, `IsOpenByDefault`, `FactionRank`, `ItemCount`, `Charge`, `HeadTrackingWeight`, `FavorCost`, `CollisionLayer`, `LevelModifier`, `TeleportDestination`, `ActivateParents`, `Lock`, `AttachRef`, `Action`, `LightData`, `Alpha`, `Patrol`, `MapMarker`, `Placement`, `VirtualMachineAdapter`, `EnableParent`, `WaterVelocity`, `XCZR`, `XCZC`, `XORD`, `RagdollData`, `RagdollBipedData`, `XWCN`, `XWCS`, `XCVL`, `XCZA`, `DistantLodData`
- **PlacedTrapRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Projectile`, `VirtualMachineAdapter`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Reflections`, `LinkedReferences`, `ActivateParents`, `EnableParent`, `Emittance`, `MultiBoundReference`, `IgnoredBySandbox`, `LocationRefTypes`, `LocationReference`, `DistantLodData`, `Scale`, `Placement`
- **ProjectileRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `ModelAndBounds`, `Destructible`, `Flags`, `Trajectory`, `Light`, `MuzzleFlashBehavior`, `TracerChance`, `ExplosionBehavior`, `Sound`, `FadeDuration`, `ImpactForce`, `PickupBehavior`, `DisableBehavior`, `Collision`, `DecalData`, `SoundLevel`
- **QuestRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `VirtualMachineAdapter.Presence`, `VirtualMachineAdapter.Version`, `VirtualMachineAdapter.ObjectFormat`, `VirtualMachineAdapter.Scripts`, `VirtualMachineAdapter.ExtraBindDataVersion`, `VirtualMachineAdapter.FileName`, `VirtualMachineAdapter.Fragments`, `VirtualMachineAdapter.Aliases`, `Flags`, `Priority`, `Type`, `Event`, `TextDisplayGlobals`, `Filter`, `NextAliasID`, `Description`, `DialogConditions`, `EventConditions`, `Stages`, `Objectives`, `Aliases`
- **RaceRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Description`, `ActorEffect`, `Skin`, `BodyTemplate`, `Keywords`, `SkillBoosts`, `Height`, `Weight`, `Flags`, `Starting`, `BaseCarryWeight`, `BaseMass`, `AccelerationRate`, `DecelerationRate`, `Size`, `HeadBipedObject`, `HairBipedObject`, `InjuredHealthPercent`, `ShieldBipedObject`, `Regen`, `UnarmedDamage`, `UnarmedReach`, `BodyBipedObject`, `AimAngleTolerance`, `FlightRadius`, `AngularAccelerationRate`, `AngularTolerance`, `MountData`, `SkeletalModel`, `MovementTypeNames`, `Voices`, `DecapitateArmors`, `DefaultHairColors`, `NumberOfTintsInList`, `FacegenMainClamp`, `FacegenFaceClamp`, `AttackRace`, `Attacks`, `BodyData`, `Hairs`, `Eyes`, `BodyPartData`, `BehaviorGraph`, `MaterialType`, `ImpactDataSet`, `DecapitationFX`, `OpenLootSound`, `CloseLootSound`, `BipedObjectNames`, `MovementTypes`, `EquipmentFlags`, `EquipmentSlots`, `UnarmedEquipSlot`, `FaceFxPhonemes`, `BaseMovementDefaultWalk`, `BaseMovementDefaultRun`, `BaseMovementDefaultSwim`, `BaseMovementDefaultFly`, `BaseMovementDefaultSneak`, `BaseMovementDefaultSprint`, `HeadData`, `MorphRace`, `ArmorRace`
- **RegionRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `MapColor`, `Worldspace`, `RegionAreas`, `Objects`, `Weather`, `Map`, `Land`, `Grasses`, `Sounds`
- **RelationshipRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Parent`, `Child`, `Rank`, `Flags`, `AssociationType`
- **ReverbParametersRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ReverbData`
- **SceneRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter.Version`, `VirtualMachineAdapter.ObjectFormat`, `VirtualMachineAdapter.Scripts`, `VirtualMachineAdapter.ScriptFragments`, `Flags`, `Phases`, `Actors`, `Actions`, `Quest`, `LastActionIndex`, `VNAM`, `Conditions`
- **ScrollRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Keywords`, `MenuDisplayObject`, `EquipmentType`, `Description`, `ModelAndBounds`, `Destructible`, `PickUpSound`, `PutDownSound`, `Value`, `Weight`, `BaseCost`, `Flags`, `Type`, `ChargeTime`, `CastType`, `TargetType`, `CastDuration`, `Range`, `HalfCostPerk`, `Effects`
- **ShaderParticleGeometryRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `GravityVelocity`, `RotationVelocity`, `ParticleSizeX`, `ParticleSizeY`, `CenterOffsetMin`, `CenterOffsetMax`, `InitialRotationRange`, `NumSubtexturesX`, `NumSubtexturesY`, `Type`, `BoxSize`, `ParticleDensity`, `ParticleTexture`
- **ShoutRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `MenuDisplayObject`, `Description`, `WordsOfPower`
- **SoulGemRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `ModelAndBounds`, `Icons`, `Destructible`, `PickUpSound`, `PutDownSound`, `Keywords`, `Value`, `Weight`, `ContainedSoul`, `MaximumCapacity`, `LinkedTo`
- **SoundCategoryRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Flags`, `Parent`, `StaticVolumeMultiplier`, `DefaultMenuVolume`
- **SoundDescriptorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Type`, `Category`, `AlternateSoundFor`, `SoundFiles`, `OutputModel`, `String`, `Conditions`, `LoopAndRumble`, `Pitch`, `Priority`, `Volume`
- **SoundMarkerRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ObjectBounds`, `FNAM`, `SNDD`, `SoundDescriptor`
- **SoundOutputModelRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Data`, `FNAM`, `Type`, `CNAM`, `SNAM`, `OutputChannels`, `Attenuation`
- **SpellRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `ObjectBounds`, `MenuDisplayObject`, `Description`, `Flags`, `Keywords`, `EquipmentType`, `BaseCost`, `Type`, `ChargeTime`, `CastType`, `TargetType`, `CastDuration`, `Range`, `HalfCostPerk`, `Effects`
- **StaticRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ModelAndBounds`, `MaxAngle`, `Material`, `Flags`, `Lod`
- **StoryManagerBranchNodeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `MaxConcurrentQuests`
- **StoryManagerEventNodeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `MaxConcurrentQuests`, `Type`
- **StoryManagerQuestNodeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `QuestFlags`, `MaxConcurrentQuests`, `MaxNumQuestsToRun`, `Quests`
- **TalkingActivatorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Destructible`, `Keywords`, `LoopingSound`, `Voice`
- **TextureSetRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ObjectBounds`, `TextureDefinition`, `Decal`
- **TreeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `ModelAndBounds`, `Ingredient`, `HarvestSound`, `Production`, `Name`, `TrunkFlexibility`, `BranchFlexibility`, `LeafAmplitude`, `LeafFrequency`
- **VisualEffectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `EffectArt`, `Shader`, `Flags`
- **VoiceTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Flags`
- **VolumetricLightingRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `LightingPreset`
- **WaterRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Opacity`, `Flags`, `MNAM`, `Material`, `OpenSound`, `Spell`, `ImageSpace`, `DamagePerSecond`, `SpecularSunPower`, `WaterReflectivity`, `WaterFresnel`, `FogAboveWaterDistanceNearPlane`, `FogAboveWaterDistanceFarPlane`, `ShallowColor`, `DeepColor`, `ReflectionColor`, `DisplacementStartingSize`, `DisplacementFoce`, `DisplacementVelocity`, `DisplacementFalloff`, `DisplacementDampner`, `NoiseFalloff`, `NoiseLayerOneWindDirection`, `NoiseLayerTwoWindDirection`, `NoiseLayerThreeWindDirection`, `NoiseLayerOneWindSpeed`, `NoiseLayerTwoWindSpeed`, `NoiseLayerThreeWindSpeed`, `FogAboveWaterAmount`, `FogUnderWaterAmount`, `FogUnderWaterDistanceNearPlane`, `FogUnderWaterDistanceFarPlane`, `WaterRefractionMagnitude`, `SpecularPower`, `SpecularRadius`, `SpecularBrightness`, `NoiseLayerOneUvScale`, `NoiseLayerTwoUvScale`, `NoiseLayerThreeUvScale`, `NoiseLayerOneAmplitudeScale`, `NoiseLayerTwoAmplitudeScale`, `NoiseLayerThreeAmplitudeScale`, `WaterReflectionMagnitude`, `SpecularSunSparkleMagnitude`, `SpecularSunSpecularMagnitude`, `DepthReflections`, `DepthRefraction`, `DepthNormals`, `DepthSpecularLighting`, `SpecularSunSparklePower`, `NoiseFlowmapScale`, `GNAM`, `LinearVelocity`, `AngularVelocity`, `NoiseLayerOneTexture`, `NoiseLayerTwoTexture`, `NoiseLayerThreeTexture`, `FlowNormalsNoiseTexture`
- **WeaponRecordHandler.cs**: `EditorID`, `Name`, `MajorRecordFlagsRaw`, `ModelAndBounds`, `Icons`, `Keywords`, `VirtualMachineAdapter`, `ObjectEffect`, `EnchantmentAmount`, `Destructible`, `EquipmentType`, `BlockBashImpact`, `AlternateBlockMaterial`, `PickUpSound`, `PutDownSound`, `Description`, `ScopeModel`, `ImpactDataSet`, `FirstPersonModel`, `AttackSound`, `AttackSound2D`, `AttackLoopSound`, `AttackFailSound`, `IdleSound`, `EquipSound`, `UnequipSound`, `BasicStats.Value`, `BasicStats.Weight`, `BasicStats.Damage`, `DetectionSoundLevel`, `Template`, `Data.AnimationType`, `Data.Speed`, `Data.Reach`, `Data.Flags`, `Data.SightFOV`, `Data.BaseVATStoHitChance`, `Data.AttackAnimation`, `Data.NumProjectiles`, `Data.EmbeddedWeaponAV`, `Data.RangeMin`, `Data.RangeMax`, `Data.OnHit`, `Data.AnimationAttackMult`, `Data.RumbleLeftMotorStrength`, `Data.RumbleRightMotorStrength`, `Data.RumbleDuration`, `Data.Skill`, `Data.Resist`, `Data.Stagger`, `Critical.Damage`, `Critical.PercentMult`, `Critical.Flags`, `Critical.Effect`
- **WeatherRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `DNAM`, `CNAM`, `ANAM`, `BNAM`, `LNAM`, `Precipitation`, `VisualEffect`, `ONAM`, `CloudTextures`, `Clouds`, `SkyUpperColor`, `FogNearColor`, `UnknownColor`, `AmbientColor`, `SunlightColor`, `SunColor`, `StarsColor`, `SkyLowerColor`, `HorizonColor`, `EffectLightingColor`, `CloudLodDiffuseColor`, `CloudLodAmbientColor`, `FogFarColor`, `SkyStaticsColor`, `WaterMultiplierColor`, `SunGlareColor`, `MoonGlareColor`, `FogDistanceDayNear`, `FogDistanceDayFar`, `FogDistanceNightNear`, `FogDistanceNightFar`, `FogDistanceDayPower`, `FogDistanceNightPower`, `FogDistanceDayMax`, `FogDistanceNightMax`, `WindSpeed`, `TransDelta`, `SunGlare`, `SunDamage`, `PrecipitationBeginFadeIn`, `PrecipitationEndFadeOut`, `ThunderLightningBeginFadeIn`, `ThunderLightningEndFadeOut`, `ThunderLightningFrequency`, `Flags`, `LightningColor`, `VisualEffectBegin`, `VisualEffectEnd`, `WindDirection`, `WindDirectionRange`, `Sounds`, `SkyStatics`, `ImageSpaces`, `VolumetricLighting`, `DirectionalAmbientLightingColors`, `NAM2`, `NAM3`, `Aurora`, `SunGlareLensFlare`
- **WordOfPowerRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Translation`
- **WorldspaceRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Location`, `Water`, `LodData`, `Music`, `MapData`, `MapImage`, `CloudModel`, `Flags`, `WorldMapOffset`, `DistantLodMultiplier`, `FixedDimensionsCenterCell`, `InteriorLighting`, `EncounterZone`, `Parent`, `Climate`, `LandDefaults`, `WaterNoiseTexture`, `HdLodDiffuseTexture`, `HdLodNormalTexture`, `WaterEnvironmentMap`
