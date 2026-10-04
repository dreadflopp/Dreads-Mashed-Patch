# Record handler property coverage audit

Generated: 2026-09-26 19:22:17 +02:00

This is a static registration audit. `Covered` is an exact registration, `AggregateCovered` is inferred from a specialized handler implementation, `Partial` indicates nested/split handling, and `MissingCandidate` has no detected handler. Reviewed aliases and non-property surfaces are classified through the tracked overrides file.

Direct record properties plus the project-standard inherited `EditorID`, `MajorRecordFlagsRaw`, and `SkyrimMajorRecordFlags` fields are compared. Identity/version metadata is excluded.

Strict verification: `powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts/Audit-RecordHandlerCoverage.ps1 -FailOnUnresolved`. This fails for stale overrides, audit errors, or any unexplained partial/missing candidate.

## Summary

| Handler | Getter | Covered | Aggregate | Partial | Missing | Classified exclusion/alias | Error |
|---|---|---:|---:|---:|---:|---:|---|
| AcousticSpaceRecordHandler.cs | IAcousticSpaceGetter | 7 | 0 | 0 | 0 | 0 |  |
| ActionRecordHandler.cs | IActionRecordGetter | 4 | 0 | 0 | 0 | 0 |  |
| ActivatorRecordHandler.cs | IActivatorGetter | 15 | 2 | 0 | 0 | 0 |  |
| ActorValueInformationRecordHandler.cs | IActorValueInformationGetter | 8 | 0 | 0 | 0 | 1 |  |
| AddonNodeRecordHandler.cs | IAddonNodeGetter | 7 | 2 | 0 | 0 | 0 |  |
| AlchemicalApparatusRecordHandler.cs | IAlchemicalApparatusGetter | 13 | 2 | 0 | 0 | 0 |  |
| AmmunitionRecordHandler.cs | IAmmunitionGetter | 17 | 2 | 0 | 0 | 1 |  |
| AnimatedObjectRecordHandler.cs | IAnimatedObjectGetter | 5 | 0 | 0 | 0 | 0 |  |
| ArmorAddonRecordHandler.cs | IArmorAddonGetter | 9 | 7 | 0 | 0 | 2 |  |
| ArmorRecordHandler.cs | IArmorGetter | 21 | 3 | 0 | 0 | 2 |  |
| ArtObjectRecordHandler.cs | IArtObjectGetter | 4 | 2 | 0 | 0 | 0 |  |
| AssociationTypeRecordHandler.cs | IAssociationTypeGetter | 6 | 0 | 0 | 0 | 0 |  |
| BodyPartDataRecordHandler.cs | IBodyPartDataGetter | 5 | 0 | 0 | 0 | 0 |  |
| BookRecordHandler.cs | IBookGetter | 18 | 2 | 0 | 0 | 1 |  |
| CameraPathRecordHandler.cs | ICameraPathGetter | 8 | 0 | 0 | 0 | 0 |  |
| CameraShotRecordHandler.cs | ICameraShotGetter | 16 | 0 | 0 | 0 | 1 |  |
| CellRecordHandler.cs | ICellGetter | 28 | 0 | 0 | 0 | 11 |  |
| ClassRecordHandler.cs | IClassGetter | 12 | 0 | 0 | 0 | 2 |  |
| ClimateRecordHandler.cs | IClimateGetter | 14 | 0 | 0 | 0 | 0 |  |
| CollisionLayerRecordHandler.cs | ICollisionLayerGetter | 9 | 0 | 0 | 0 | 0 |  |
| ColorRecordHandler.cs | IColorRecordGetter | 6 | 0 | 0 | 0 | 0 |  |
| CombatStyleRecordHandler.cs | ICombatStyleGetter | 20 | 0 | 0 | 0 | 1 |  |
| ConstructibleObjectRecordHandler.cs | IConstructibleObjectGetter | 8 | 0 | 0 | 0 | 0 |  |
| ContainerRecordHandler.cs | IContainerGetter | 12 | 2 | 0 | 0 | 0 |  |
| DebrisRecordHandler.cs | IDebrisGetter | 4 | 0 | 0 | 0 | 0 |  |
| DefaultObjectManagerRecordHandler.cs | IDefaultObjectManagerGetter | 4 | 0 | 0 | 0 | 0 |  |
| DialogBranchRecordHandler.cs | IDialogBranchGetter | 7 | 0 | 0 | 0 | 0 |  |
| DialogResponseRecordHandler.cs | IDialogResponsesGetter | 17 | 0 | 0 | 0 | 2 |  |
| DialogTopicRecordHandler.cs | IDialogTopicGetter | 11 | 0 | 0 | 0 | 3 |  |
| DialogViewRecordHandler.cs | IDialogViewGetter | 8 | 0 | 0 | 0 | 0 |  |
| DoorRecordHandler.cs | IDoorGetter | 11 | 2 | 0 | 0 | 0 |  |
| DualCastDataRecordHandler.cs | IDualCastDataGetter | 10 | 0 | 0 | 0 | 0 |  |
| EffectShaderRecordHandler.cs | IEffectShaderGetter | 8 | 100 | 0 | 0 | 1 |  |
| EncounterZoneRecordHandler.cs | IEncounterZoneGetter | 9 | 0 | 0 | 0 | 1 |  |
| EquipTypeRecordHandler.cs | IEquipTypeGetter | 5 | 0 | 0 | 0 | 0 |  |
| ExplosionRecordHandler.cs | IExplosionGetter | 20 | 2 | 0 | 0 | 1 |  |
| EyesRecordHandler.cs | IEyesGetter | 7 | 0 | 0 | 0 | 0 |  |
| FactionRecordHandler.cs | IFactionGetter | 19 | 0 | 0 | 0 | 0 |  |
| FloraRecordHandler.cs | IFloraGetter | 13 | 2 | 0 | 0 | 0 |  |
| FootstepRecordHandler.cs | IFootstepGetter | 5 | 0 | 0 | 0 | 0 |  |
| FootstepSetRecordHandler.cs | IFootstepSetGetter | 8 | 0 | 0 | 0 | 0 |  |
| FormIdRecordHandler.cs | IFormListGetter | 4 | 0 | 0 | 0 | 0 |  |
| FurnitureRecordHandler.cs | IFurnitureGetter | 15 | 2 | 0 | 0 | 0 |  |
| GameSettingBoolRecordHandler.cs | IGameSettingBoolGetter | 4 | 0 | 0 | 0 | 0 |  |
| GameSettingFloatRecordHandler.cs | IGameSettingFloatGetter | 4 | 0 | 0 | 0 | 0 |  |
| GameSettingIntRecordHandler.cs | IGameSettingIntGetter | 4 | 0 | 0 | 0 | 0 |  |
| GameSettingStringRecordHandler.cs | IGameSettingStringGetter | 4 | 0 | 0 | 0 | 0 |  |
| GlobalFloatRecordHandler.cs | IGlobalFloatGetter | 4 | 0 | 0 | 0 | 0 |  |
| GlobalIntRecordHandler.cs | IGlobalIntGetter | 4 | 0 | 0 | 0 | 0 |  |
| GlobalShortRecordHandler.cs | IGlobalShortGetter | 4 | 0 | 0 | 0 | 0 |  |
| GlobalUnknownRecordHandler.cs | IGlobalUnknownGetter | 5 | 0 | 0 | 0 | 0 |  |
| GrassRecordHandler.cs | IGrassGetter | 13 | 2 | 0 | 0 | 3 |  |
| HazardRecordHandler.cs | IHazardGetter | 15 | 2 | 0 | 0 | 0 |  |
| HeadPartRecordHandler.cs | IHeadPartGetter | 13 | 0 | 0 | 0 | 0 |  |
| IdleAnimationRecordHandler.cs | IIdleAnimationGetter | 12 | 0 | 0 | 0 | 0 |  |
| IdleMarkerRecordHandler.cs | IIdleMarkerGetter | 7 | 2 | 0 | 0 | 0 |  |
| ImageSpaceAdapterRecordHandler.cs | IImageSpaceAdapterGetter | 3 | 0 | 0 | 0 | 60 |  |
| ImageSpaceRecordHandler.cs | IImageSpaceGetter | 8 | 0 | 0 | 0 | 0 |  |
| ImpactDataSetRecordHandler.cs | IImpactDataSetGetter | 4 | 0 | 0 | 0 | 0 |  |
| ImpactRecordHandler.cs | IImpactGetter | 16 | 1 | 0 | 0 | 1 |  |
| IngestibleRecordHandler.cs | IIngestibleGetter | 19 | 2 | 0 | 0 | 0 |  |
| IngredientRecordHandler.cs | IIngredientGetter | 16 | 2 | 0 | 0 | 0 |  |
| KeyRecordHandler.cs | IKeyGetter | 13 | 2 | 0 | 0 | 0 |  |
| KeywordRecordHandler.cs | IKeywordGetter | 4 | 0 | 0 | 0 | 0 |  |
| LandscapeRecordHandler.cs | ILandscapeGetter | 9 | 0 | 0 | 0 | 0 |  |
| LandscapeTextureRecordHandler.cs | ILandscapeTextureGetter | 10 | 0 | 0 | 0 | 0 |  |
| LeveledItemRecordHandler.cs | ILeveledItemGetter | 8 | 0 | 0 | 0 | 0 |  |
| LeveledNpcRecordHandler.cs | ILeveledNpcGetter | 7 | 2 | 0 | 0 | 0 |  |
| LeveledSpellRecordHandler.cs | ILeveledSpellGetter | 7 | 0 | 0 | 0 | 0 |  |
| LightingTemplateRecordHandler.cs | ILightingTemplateGetter | 19 | 0 | 0 | 0 | 2 |  |
| LightRecordHandler.cs | ILightGetter | 23 | 2 | 0 | 0 | 0 |  |
| LoadScreenRecordHandler.cs | ILoadScreenGetter | 13 | 0 | 0 | 0 | 0 |  |
| LocationRecordHandler.cs | ILocationGetter | 12 | 0 | 0 | 0 | 16 |  |
| LocationReferenceTypeRecordHandler.cs | ILocationReferenceTypeGetter | 4 | 0 | 0 | 0 | 0 |  |
| MagicEffectRecordHandler.cs | IMagicEffectGetter | 46 | 0 | 0 | 0 | 1 |  |
| MaterialObjectRecordHandler.cs | IMaterialObjectGetter | 14 | 0 | 0 | 0 | 1 |  |
| MaterialTypeRecordHandler.cs | IMaterialTypeGetter | 9 | 0 | 0 | 0 | 0 |  |
| MessageRecordHandler.cs | IMessageGetter | 10 | 0 | 0 | 0 | 0 |  |
| MiscItemRecordHandler.cs | IMiscItemGetter | 13 | 2 | 0 | 0 | 0 |  |
| MoveableStaticRecordHandler.cs | IMoveableStaticGetter | 8 | 2 | 0 | 0 | 0 |  |
| MovementTypeRecordHandler.cs | IMovementTypeGetter | 16 | 0 | 0 | 0 | 1 |  |
| MusicTrackRecordHandler.cs | IMusicTrackGetter | 12 | 0 | 0 | 0 | 0 |  |
| MusicTypeRecordHandler.cs | IMusicTypeGetter | 7 | 0 | 0 | 0 | 0 |  |
| NavigationMeshRecordHandler.cs | INavigationMeshGetter | 8 | 0 | 0 | 0 | 0 |  |
| NpcRecordHandler.cs | INpcGetter | 46 | 3 | 0 | 0 | 0 |  |
| ObjectEffectRecordHandler.cs | IObjectEffectGetter | 15 | 0 | 0 | 0 | 1 |  |
| OutfitRecordHandler.cs | IOutfitGetter | 4 | 0 | 0 | 0 | 0 |  |
| PackageRecordHandler.cs | IPackageGetter | 22 | 0 | 0 | 0 | 9 |  |
| PerkRecordHandler.cs | IPerkGetter | 14 | 0 | 0 | 0 | 2 |  |
| PlacedHazardRecordHandler.cs | IPlacedHazardGetter | 4 | 0 | 0 | 0 | 0 |  |
| PlacedNpcRecordHandler.cs | IPlacedNpcGetter | 33 | 0 | 0 | 0 | 0 |  |
| PlacedObjectRecordHandler.cs | IPlacedObjectGetter | 61 | 0 | 0 | 0 | 1 |  |
| ProjectileRecordHandler.cs | IProjectileGetter | 13 | 21 | 0 | 0 | 1 |  |
| QuestRecordHandler.cs | IQuestGetter | 17 | 1 | 0 | 0 | 2 |  |
| RaceRecordHandler.cs | IRaceGetter | 67 | 7 | 0 | 0 | 3 |  |
| RegionRecordHandler.cs | IRegionGetter | 13 | 0 | 0 | 0 | 0 |  |
| RelationshipRecordHandler.cs | IRelationshipGetter | 9 | 0 | 0 | 0 | 1 |  |
| ReverbParametersRecordHandler.cs | IReverbParametersGetter | 3 | 11 | 0 | 0 | 1 |  |
| SceneRecordHandler.cs | ISceneGetter | 11 | 0 | 0 | 0 | 3 |  |
| ScrollRecordHandler.cs | IScrollGetter | 23 | 2 | 0 | 0 | 0 |  |
| ShaderParticleGeometryRecordHandler.cs | IShaderParticleGeometryGetter | 16 | 0 | 0 | 0 | 1 |  |
| ShoutRecordHandler.cs | IShoutGetter | 8 | 0 | 0 | 0 | 0 |  |
| SoulGemRecordHandler.cs | ISoulGemGetter | 15 | 2 | 0 | 0 | 0 |  |
| SoundCategoryRecordHandler.cs | ISoundCategoryGetter | 8 | 0 | 0 | 0 | 0 |  |
| SoundDescriptorRecordHandler.cs | ISoundDescriptorGetter | 12 | 4 | 0 | 0 | 0 |  |
| SoundMarkerRecordHandler.cs | ISoundMarkerGetter | 7 | 0 | 0 | 0 | 0 |  |
| SoundOutputModelRecordHandler.cs | ISoundOutputModelGetter | 10 | 0 | 0 | 0 | 0 |  |
| SpellRecordHandler.cs | ISpellGetter | 19 | 0 | 0 | 0 | 0 |  |
| StaticRecordHandler.cs | IStaticGetter | 6 | 2 | 0 | 0 | 4 |  |
| StoryManagerBranchNodeRecordHandler.cs | IStoryManagerBranchNodeGetter | 5 | 0 | 0 | 0 | 0 |  |
| StoryManagerEventNodeRecordHandler.cs | IStoryManagerEventNodeGetter | 6 | 0 | 0 | 0 | 0 |  |
| StoryManagerQuestNodeRecordHandler.cs | IStoryManagerQuestNodeGetter | 8 | 0 | 0 | 0 | 0 |  |
| TalkingActivatorRecordHandler.cs | ITalkingActivatorGetter | 8 | 2 | 0 | 0 | 4 |  |
| TextureSetRecordHandler.cs | ITextureSetGetter | 4 | 9 | 0 | 0 | 1 |  |
| TreeRecordHandler.cs | ITreeGetter | 13 | 2 | 0 | 0 | 1 |  |
| VisualEffectRecordHandler.cs | IVisualEffectGetter | 6 | 0 | 0 | 0 | 0 |  |
| VoiceTypeRecordHandler.cs | IVoiceTypeGetter | 4 | 0 | 0 | 0 | 0 |  |
| WaterRecordHandler.cs | IWaterGetter | 62 | 0 | 0 | 0 | 9 |  |
| WeaponRecordHandler.cs | IWeaponGetter | 29 | 5 | 0 | 0 | 1 |  |
| WeatherRecordHandler.cs | IWeatherGetter | 62 | 0 | 0 | 0 | 2 |  |
| WordOfPowerRecordHandler.cs | IWordOfPowerGetter | 5 | 0 | 0 | 0 | 0 |  |
| WorldspaceRecordHandler.cs | IWorldspaceGetter | 21 | 3 | 0 | 0 | 13 |  |

## ActorValueInformationRecordHandler.cs

Getter: `IActorValueInformationGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `CNAM` | `Nullable<ReadOnlyMemorySlice<Byte>>` | IntentionalExclusion |  | CNAM | Engine-managed binary data excluded by the project runtime-field policy; the winning override is preserved. |

## AmmunitionRecordHandler.cs

Getter: `IAmmunitionGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the AMMO DATA layout/breaks; it is not an independent xEdit semantic field. |

## ArmorAddonRecordHandler.cs

Getter: `IArmorAddonGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Unknown` | `UInt16` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown2` | `Byte` | IntentionalExclusion |  | Unknown2 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## ArmorRecordHandler.cs

Getter: `IArmorGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | Armor.MajorFlags is a typed view over MajorRecordFlagsRaw. Armor uses one composite MajorRecordFlagsRaw handler for common and record-specific bits. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | SkyrimMajorRecordFlags is a typed view over MajorRecordFlagsRaw. Armor uses one composite MajorRecordFlagsRaw handler to avoid duplicate writes to the same header integer. |

## BookRecordHandler.cs

Getter: `IBookGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Unused` | `UInt16` | IntentionalExclusion |  | Unused | Unused BOOK storage is not an independently editable semantic field; the winning value is preserved. |

## CameraShotRecordHandler.cs

Getter: `ICameraShotGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the CAMS DATA layout/breaks; it is not an independent xEdit semantic field. |

## CellRecordHandler.cs

Getter: `ICellGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Landscape` | `ILandscapeGetter` | RuntimeOrNavigation |  | Landscape | Runtime-managed LAND child data is excluded from CELL property forwarding. |
| `NavigationMeshes` | `IReadOnlyList<INavigationMeshGetter>` | RuntimeOrNavigation |  | NavigationMeshes | Navigation child data is excluded from CELL property forwarding. |
| `Persistent` | `IReadOnlyList<IPlacedGetter>` | RuntimeOrNavigation |  | Persistent | Child placed records in the CELL persistent GRUP; they are processed as their own major records rather than as a CELL property. |
| `PersistentTimestamp` | `Int32` | RuntimeOrNavigation |  | PersistentTimestamp | GRUP header metadata for persistent CELL children, not an xEdit CELL field. |
| `PersistentUnknownGroupData` | `Int32` | RuntimeOrNavigation |  | PersistentUnknownGroupData | GRUP header metadata for persistent CELL children, not an xEdit CELL field. |
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
| `Unknown` | `Int32` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown2` | `Byte` | IntentionalExclusion |  | Unknown2 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## CombatStyleRecordHandler.cs

Getter: `ICombatStyleGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `CSGDDataTypeState` | `CSGDDataType` | SerializationState |  | CSGDDataTypeState | Mutagen discriminator controlling the CSTY CSGD layout/breaks; it is not an independent xEdit semantic field. |

## DialogResponseRecordHandler.cs

Getter: `IDialogResponsesGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `PreviousDialog` | `IFormLinkNullableGetter<IDialogResponsesGetter>` | RuntimeOrNavigation |  | PreviousDialog | Runtime/structural dialog linkage is preserved from the winning override rather than conflict-forwarded. |
| `UnknownData` | `IReadOnlyList<IDialogResponsesUnknownDataGetter>` | IntentionalExclusion |  | UnknownData | Opaque ordered SCHR/QNAM/NEXT payloads cannot be safely merged across plugins; the winning list is preserved atomically. |

## DialogTopicRecordHandler.cs

Getter: `IDialogTopicGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Responses` | `IReadOnlyList<IDialogResponsesGetter>` | RuntimeOrNavigation |  | Responses | INFO child major records stored under the DIAL group; INFO records have their own handler. |
| `Timestamp` | `Int32` | RuntimeOrNavigation |  | Timestamp | DIAL child-group header metadata, not a normal DIAL field. |
| `Unknown` | `Int32` | RuntimeOrNavigation |  | Unknown | DIAL child-group header metadata, not a normal DIAL field. |

## EffectShaderRecordHandler.cs

Getter: `IEffectShaderGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling DATA layout/breaks; it is not an independent xEdit semantic field. |

## EncounterZoneRecordHandler.cs

Getter: `IEncounterZoneGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling DATA layout/breaks; it is not an independent xEdit semantic field. |

## ExplosionRecordHandler.cs

Getter: `IExplosionGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the EXPL DATA layout/breaks; it is not an independent xEdit semantic field. |

## GrassRecordHandler.cs

Getter: `IGrassGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Unknown` | `Byte` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown2` | `UInt16` | IntentionalExclusion |  | Unknown2 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |
| `Unknown3` | `ReadOnlyMemorySlice<Byte>` | IntentionalExclusion |  | Unknown3 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

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

## ImpactRecordHandler.cs

Getter: `IImpactGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Unknown` | `Int16` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## LightingTemplateRecordHandler.cs

Getter: `ILightingTemplateGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the LGTM DATA layout/breaks; it is not an independent xEdit semantic field. |
| `Unknown` | `Int32` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

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
| `UniqueActorReferencesAdded` | `IReadOnlyList<IUniqueActorReferenceGetter>` | IntentionalExclusion |  | UniqueActorReferencesAdded | Serialized xEdit-benign LCTN Added bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `UniqueActorReferencesRemoved` | `IReadOnlyList<IFormLinkGetter<INpcGetter>>` | IntentionalExclusion |  | UniqueActorReferencesRemoved | Serialized xEdit-benign LCTN Removed bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `UniqueActorReferencesStatic` | `IReadOnlyList<IUniqueActorReferenceGetter>` | IntentionalExclusion |  | UniqueActorReferencesStatic | Serialized xEdit-benign LCTN Master bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `WorldspaceCellsAdded` | `IReadOnlyList<ILocationCoordinateGetter>` | IntentionalExclusion |  | WorldspaceCellsAdded | Serialized xEdit-benign LCTN Added bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `WorldspaceCellsRemoved` | `IReadOnlyList<ILocationCoordinateGetter>` | IntentionalExclusion |  | WorldspaceCellsRemoved | Serialized xEdit-benign LCTN Removed bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |
| `WorldspaceCellsStatic` | `IReadOnlyList<ILocationCoordinateGetter>` | IntentionalExclusion |  | WorldspaceCellsStatic | Serialized xEdit-benign LCTN Master bookkeeping; the project deliberately preserves the winning generated membership data instead of conflict-resolving it. |

## MagicEffectRecordHandler.cs

Getter: `IMagicEffectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Unknown1` | `UInt16` | IntentionalExclusion |  | Unknown1 | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## MaterialObjectRecordHandler.cs

Getter: `IMaterialObjectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the MATO DATA layout/breaks; it is not an independent xEdit semantic field. |

## MovementTypeRecordHandler.cs

Getter: `IMovementTypeGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `SPEDDataTypeState` | `SPEDDataType` | SerializationState |  | SPEDDataTypeState | Mutagen discriminator controlling the MOVT SPED layout/breaks; it is not an independent xEdit semantic field. |

## ObjectEffectRecordHandler.cs

Getter: `IObjectEffectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `ENITDataTypeState` | `ENITDataType` | SerializationState |  | ENITDataTypeState | Mutagen discriminator controlling ENIT layout/breaks; it is not an independent xEdit semantic field. |

## PackageRecordHandler.cs

Getter: `IPackageGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Data` | `IReadOnlyDictionary<SByte, IAPackageDataGetter>` | IntentionalExclusion |  | Data | Temporarily not forwarded: Mutagen 0.54.4 sorts PACK value records by UNAM key and changes their physical xEdit row order. |
| `DataInputVersion` | `Int32` | IntentionalExclusion |  | DataInputVersion | Temporarily not forwarded with the disabled PackageTemplateGraph atomic unit because its fields must share one owner. |
| `PackageTemplate` | `IFormLinkGetter<IPackageGetter>` | IntentionalExclusion |  | PackageTemplate | Temporarily not forwarded: PackageTemplateGraph is disabled because Mutagen 0.54.4 reorders physical PACK data rows by UNAM key while writing. |
| `ProcedureTree` | `IReadOnlyList<IPackageBranchGetter>` | IntentionalExclusion |  | ProcedureTree | Temporarily not forwarded with PackageTemplateGraph because branch DataInputIndices refer to the grouped package data dictionary. |
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

## PlacedObjectRecordHandler.cs

Getter: `IPlacedObjectGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Unknown` | `Int16` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## ProjectileRecordHandler.cs

Getter: `IProjectileGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the PROJ DATA layout/breaks; it is not an independent xEdit semantic field. |

## QuestRecordHandler.cs

Getter: `IQuestGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `QuestFormVersion` | `Byte` | IntentionalExclusion |  | QuestFormVersion | xEdit marks QuestFormVersion cpIgnore; it is format metadata rather than an independently editable semantic field. |
| `Unknown` | `Int32` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## RaceRecordHandler.cs

Getter: `IRaceGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the RACE DATA layout/breaks; it is not an independent xEdit semantic field. |
| `ExportingExtraNam2` | `Boolean` | SerializationState |  | ExportingExtraNam2 | Mutagen parser/writer state preserving an empty extra NAM2 marker; it is not an independent xEdit semantic field. |
| `Unknown` | `Int16` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## RelationshipRecordHandler.cs

Getter: `IRelationshipGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Unknown` | `Byte` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## ReverbParametersRecordHandler.cs

Getter: `IReverbParametersGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Unknown` | `Byte` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## SceneRecordHandler.cs

Getter: `ISceneGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `Unused` | `IScenePhaseUnusedDataGetter` | IntentionalExclusion |  | Unused.QNAM<br>Unused.SCDA<br>Unused.SCHR<br>Unused.SCRO<br>Unused.SCTX | Legacy SCHR/SCDA/SCTX/QNAM/SCRO storage from older Creation Kit versions is hidden by xEdit and excluded from semantic comparison and copying. |
| `Unused2` | `IScenePhaseUnusedDataGetter` | IntentionalExclusion |  | Unused2.QNAM<br>Unused2.SCDA<br>Unused2.SCHR<br>Unused2.SCRO<br>Unused2.SCTX | Legacy SCHR/SCDA/SCTX/QNAM/SCRO storage from older Creation Kit versions is hidden by xEdit and excluded from semantic comparison and copying. |
| `VirtualMachineAdapter` | `ISceneAdapterGetter` | AliasOrDuplicate |  | VirtualMachineAdapter.ObjectFormat<br>VirtualMachineAdapter.ScriptFragments.ExtraBindDataVersion<br>VirtualMachineAdapter.ScriptFragments.FileName<br>VirtualMachineAdapter.ScriptFragments.OnBegin.ExtraBindDataVersion<br>VirtualMachineAdapter.ScriptFragments.OnBegin.FragmentName<br>VirtualMachineAdapter.ScriptFragments.OnBegin.ScriptName<br>VirtualMachineAdapter.ScriptFragments.OnEnd.ExtraBindDataVersion<br>VirtualMachineAdapter.ScriptFragments.OnEnd.FragmentName<br>VirtualMachineAdapter.ScriptFragments.OnEnd.ScriptName<br>VirtualMachineAdapter.ScriptFragments.PhaseFragments<br>VirtualMachineAdapter.Scripts<br>VirtualMachineAdapter.Version | Covered without overlap by typed VMAD presence, version, object-format, scripts, and complete script-fragments handlers. |

## ShaderParticleGeometryRecordHandler.cs

Getter: `IShaderParticleGeometryGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DATADataTypeState` | `DATADataType` | SerializationState |  | DATADataTypeState | Mutagen discriminator controlling the SPGD DATA layout/breaks; it is not an independent xEdit semantic field. |

## StaticRecordHandler.cs

Getter: `IStaticGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DNAMDataTypeState` | `DNAMDataType` | SerializationState |  | DNAMDataTypeState | Mutagen discriminator controlling DNAM layout/breaks; it is not an independent xEdit semantic field. |
| `MajorFlags` | `MajorFlag` | AliasOrDuplicate |  | MajorFlags | STAT-specific major flags are a typed view over MajorRecordFlagsRaw and are owned by that single composite flag handler. |
| `SkyrimMajorRecordFlags` | `SkyrimMajorRecordFlag` | AliasOrDuplicate |  | SkyrimMajorRecordFlags | Common Skyrim major flags are a typed view over MajorRecordFlagsRaw and are owned by the STAT composite flag handler. |
| `Unused` | `ReadOnlyMemorySlice<Byte>` | IntentionalExclusion |  | Unused | Unused STAT storage is serialization-only data; the winning value is preserved. |

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
| `Unknown` | `ReadOnlyMemorySlice<Byte>` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

## WaterRecordHandler.cs

Getter: `IWaterGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `DNAMDataTypeState` | `DNAMDataType` | SerializationState |  | DNAMDataTypeState | Mutagen discriminator controlling the WATR DNAM layout/breaks; it is not an independent xEdit semantic field. |
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
| `Unused` | `Nullable<ReadOnlyMemorySlice<Byte>>` | IntentionalExclusion |  | Unused | Unused WEAP storage is outside the supported semantic conflict surface; the winning value is preserved. |

## WeatherRecordHandler.cs

Getter: `IWeatherGetter`

| Property | Type | Status | Possible handler keys | Nested leaves | Reason |
|---|---|---|---|---|---|
| `NAM0DataTypeState` | `NAM0DataType` | SerializationState |  | NAM0DataTypeState | Mutagen discriminator controlling the WTHR NAM0 layout/breaks; it is not an independent xEdit semantic field. |
| `Unknown` | `UInt16` | IntentionalExclusion |  | Unknown | Unknown engine field outside the supported semantic conflict surface; the winning override is preserved. |

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

- **AcousticSpaceRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ObjectBounds`, `AmbientSound`, `UseSoundFromRegion`, `EnvironmentType`
- **ActionRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Color`
- **ActivatorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Destructible`, `Keywords`, `MarkerColor`, `LoopingSound`, `ActivationSound`, `WaterType`, `ActivateTextOverride`, `Flags`, `MajorFlags`, `InteractionKeyword`
- **ActorValueInformationRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Description`, `Abbreviation`, `Skill`, `PerkTree`
- **AddonNodeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ModelAndBounds`, `NodeIndex`, `Sound`, `MasterParticleSystemCap`, `Flags`
- **AlchemicalApparatusRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Icons`, `Destructible`, `PickUpSound`, `PutDownSound`, `Quality`, `Description`, `Value`, `Weight`
- **AmmunitionRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `ModelAndBounds`, `Icons`, `Destructible`, `PickUpSound`, `PutDownSound`, `Description`, `Keywords`, `Projectile`, `Flags`, `Damage`, `Value`, `Weight`, `ShortName`, `MajorFlags`
- **AnimatedObjectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Model`, `UnloadEvent`
- **ArmorAddonRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `WeightSliderEnabled.Male`, `WeightSliderEnabled.Female`, `WorldModel.Male.File`, `WorldModel.Male.AlternateTextures`, `WorldModel.Female.File`, `WorldModel.Female.AlternateTextures`, `FirstPersonModel.Male.File`, `FirstPersonModel.Male.AlternateTextures`, `FirstPersonModel.Female.File`, `FirstPersonModel.Female.AlternateTextures`, `AdditionalRaces`, `BodyTemplate.FirstPersonFlags`, `BodyTemplate.Flags`, `BodyTemplate.ArmorType`, `Priority.Male`, `Priority.Female`, `DetectionSoundValue`, `WeaponAdjust`, `Race`, `FootstepSound`, `ArtObject`, `SkinTexture.Male`, `SkinTexture.Female`, `TextureSwapList.Male`, `TextureSwapList.Female`
- **ArmorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `VirtualMachineAdapter`, `ObjectEffect`, `EnchantmentAmount`, `WorldModelAndBounds`, `BodyTemplate.FirstPersonFlags`, `BodyTemplate.Flags`, `BodyTemplate.ArmorType`, `Destructible`, `PickUpSound`, `PutDownSound`, `RagdollConstraintTemplate`, `EquipmentType`, `BashImpactDataSet`, `AlternateBlockMaterial`, `Race`, `Keywords`, `Description`, `Armature`, `Value`, `Weight`, `ArmorRating`, `TemplateArmor`
- **ArtObjectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ModelAndBounds`, `Type`
- **AssociationTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ParentTitle`, `Title`, `IsFamily`
- **BodyPartDataRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Model`, `Parts`
- **BookRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `ModelAndBounds`, `Value`, `Weight`, `Description`, `PickUpSound`, `PutDownSound`, `Keywords`, `BookText`, `Destructible`, `Flags`, `Type`, `Teaches`, `InventoryArt`, `VirtualMachineAdapter`, `Icons`
- **CameraPathRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Conditions`, `RelatedPaths`, `Zoom`, `ZoomMustHaveCameraShots`, `Shots`
- **CameraShotRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Model`, `Action`, `Location`, `Target`, `Flags`, `TimeMultiplierPlayer`, `TimeMultiplierTarget`, `TimeMultiplierGlobal`, `MaxTime`, `MinTime`, `TargetPercentBetweenActors`, `NearTargetDistance`, `ImageSpaceModifier`
- **CellRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Flags`, `MajorFlags`, `Regions`, `Location`, `Owner`, `Water`, `Lighting`, `LightingTemplate`, `AcousticSpace`, `EncounterZone`, `Music`, `ImageSpace`, `SkyAndWeatherFromRegion`, `Grid`, `MaxHeightData`, `WaterNoiseTexture`, `WaterVelocity`, `XWCN`, `XWCS`, `OcclusionData`, `LNAM`, `FactionRank`, `LockList`, `WaterEnvironmentMap`
- **ClassRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Description`, `Icon`, `Teaches`, `MaxTrainingLevel`, `SkillWeights`, `BleedoutDefault`, `VoicePoints`, `StatWeights`
- **ClimateRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `WeatherTypes`, `SunTexture`, `SunGlareTexture`, `Model`, `SunriseBegin`, `SunriseEnd`, `SunsetBegin`, `SunsetEnd`, `Volatility`, `Moons`, `PhaseLength`
- **CollisionLayerRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Description`, `Index`, `DebugColor`, `Flags`, `Name`, `CollidesWith`
- **ColorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Color`, `Playable`
- **CombatStyleRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `OffensiveMult`, `DefensiveMult`, `GroupOffensiveMult`, `EquipmentScoreMultMelee`, `EquipmentScoreMultMagic`, `EquipmentScoreMultRanged`, `EquipmentScoreMultShout`, `EquipmentScoreMultUnarmed`, `EquipmentScoreMultStaff`, `AvoidThreatChance`, `CSMD`, `Melee`, `CloseRange`, `LongRangeStrafeMult`, `Flight`, `Flags`, `MajorFlags`
- **ConstructibleObjectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Items`, `Conditions`, `CreatedObject`, `WorkbenchKeyword`, `CreatedObjectCount`
- **ContainerRecordHandler.cs**: `Name`, `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ModelAndBounds`, `Weight`, `Items`, `VirtualMachineAdapter`, `Destructible`, `Flags`, `OpenSound`, `CloseSound`, `MajorFlags`
- **DebrisRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Models`
- **DefaultObjectManagerRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Objects`
- **DialogBranchRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Quest`, `Category`, `Flags`, `StartingTopic`
- **DialogResponseRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `DATA`, `VirtualMachineAdapter`, `Flags`, `MajorFlags`, `ResetHours`, `Topic`, `FavorLevel`, `LinkTo`, `ResponseData`, `Responses`, `Conditions`, `Prompt`, `Speaker`, `WalkAwayTopic`, `AudioOutputOverride`
- **DialogTopicRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Priority`, `Branch`, `Quest`, `TopicFlags`, `Category`, `Subtype`, `SubtypeName`
- **DialogViewRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Quest`, `Branches`, `TNAMs`, `ENAM`, `DNAM`
- **DoorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Destructible`, `OpenSound`, `CloseSound`, `LoopSound`, `Flags`, `MajorFlags`
- **DualCastDataRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ObjectBounds`, `Projectile`, `Explosion`, `EffectShader`, `HitEffectArt`, `ImpactDataSet`, `InheritScale`
- **EffectShaderRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `FillTexture`, `ParticleShaderTexture`, `HolesTexture`, `MembranePaletteTexture`, `ParticlePaletteTexture`, `EffectShaderData`
- **EncounterZoneRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Owner`, `Location`, `Rank`, `MinLevel`, `MaxLevel`, `Flags`
- **EquipTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `SlotParents`, `UseAllParents`
- **ExplosionRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `ObjectEffect`, `ImageSpaceModifier`, `Light`, `Sound1`, `Sound2`, `ImpactDataSet`, `PlacedObject`, `SpawnProjectile`, `Force`, `Damage`, `Radius`, `ISRadius`, `VerticalOffsetMult`, `Flags`, `SoundLevel`
- **EyesRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Icon`, `Flags`, `MajorFlags`
- **FactionRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Relations`, `Ranks`, `Conditions`, `Flags`, `ExteriorJailMarker`, `FollowerWaitMarker`, `StolenGoodsContainer`, `PlayerInventoryContainer`, `SharedCrimeFactionList`, `JailOutfit`, `CrimeValues`, `VendorBuySellList`, `MerchantContainer`, `VendorValues`, `VendorLocation`
- **FloraRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Destructible`, `Keywords`, `PNAM`, `ActivateTextOverride`, `FNAM`, `Ingredient`, `HarvestSound`, `Production`
- **FootstepRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ImpactDataSet`, `Tag`
- **FootstepSetRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `WalkForwardFootsteps`, `RunForwardFootsteps`, `WalkForwardAlternateFootsteps`, `RunForwardAlternateFootsteps`, `WalkForwardAlternateFootsteps2`
- **FormIdRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Items`
- **FurnitureRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Destructible`, `Keywords`, `PNAM`, `Flags`, `InteractionKeyword`, `WorkbenchData`, `AssociatedSpell`, `Markers`, `ModelFilename`, `MajorFlags`
- **GameSettingBoolRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Data`
- **GameSettingFloatRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Data`
- **GameSettingIntRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Data`
- **GameSettingStringRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Data`
- **GlobalFloatRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `MajorFlags`, `Data`
- **GlobalIntRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `MajorFlags`, `Data`
- **GlobalShortRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `MajorFlags`, `Data`
- **GlobalUnknownRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `TypeChar`, `MajorFlags`, `Data`
- **GrassRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ModelAndBounds`, `Density`, `MinSlope`, `MaxSlope`, `UnitsFromWater`, `UnitsFromWaterType`, `PositionRange`, `HeightRange`, `ColorRange`, `WavePeriod`, `Flags`
- **HazardRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `ModelAndBounds`, `ImageSpaceModifier`, `Limit`, `Radius`, `Lifetime`, `ImageSpaceRadius`, `TargetInterval`, `Flags`, `Spell`, `Light`, `ImpactDataSet`, `Sound`
- **HeadPartRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Model`, `Flags`, `Type`, `ExtraParts`, `Parts`, `TextureSet`, `Color`, `ValidRaces`, `MajorFlags`
- **IdleAnimationRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Conditions`, `Filename`, `AnimationEvent`, `RelatedIdles`, `LoopingSecondsMin`, `LoopingSecondsMax`, `Flags`, `AnimationGroupSection`, `ReplayDelay`
- **IdleMarkerRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Flags`, `IdleTimer`, `Animations`, `ModelAndBounds`, `MajorFlags`
- **ImageSpaceAdapterRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `AnimationSettings`, `BlurRadius`, `DoubleVisionStrength`, `TintColor`, `FadeColor`, `RadialBlur`, `DepthOfField`, `MotionBlurStrength`, `HdrEyeAdaptSpeed`, `HdrBloomBlurRadius`, `HdrBloomThreshold`, `HdrBloomScale`, `HdrTargetLumMin`, `HdrTargetLumMax`, `HdrSunlightScale`, `HdrSkyScale`, `CinematicSaturation`, `CinematicBrightness`, `CinematicContrast`
- **ImageSpaceRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ENAM`, `Hdr`, `Cinematic`, `Tint`, `DepthOfField`
- **ImpactDataSetRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Impacts`
- **ImpactRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Model`, `Duration`, `Orientation`, `AngleThreshold`, `PlacementRadius`, `SoundLevel`, `NoDecalData`, `Result`, `Decal.Presence`, `Decal.Bounds`, `Decal.Depth`, `Decal.Shininess`, `Decal.Parallax`, `Decal.Flags`, `Decal.Color`, `TextureSet`, `SecondaryTextureSet`, `Sound1`, `Sound2`, `Hazard`
- **IngestibleRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Description`, `ModelAndBounds`, `Destructible`, `Icons`, `PickUpSound`, `PutDownSound`, `EquipmentType`, `Weight`, `Value`, `Keywords`, `Addiction`, `AddictionChance`, `ConsumeSound`, `Effects`, `Flags`, `MajorFlags`
- **IngredientRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `VirtualMachineAdapter`, `ModelAndBounds`, `Icons`, `Destructible`, `EquipType`, `PickUpSound`, `PutDownSound`, `Value`, `Weight`, `IngredientValue`, `Flags`, `Effects`, `Keywords`
- **KeyRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `VirtualMachineAdapter`, `ModelAndBounds`, `Icons`, `Destructible`, `PickUpSound`, `PutDownSound`, `Keywords`, `Value`, `Weight`, `MajorFlags`
- **KeywordRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Color`
- **LandscapeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Flags`, `VertexNormals`, `VertexHeightMap`, `VertexColors`, `Layers`, `Textures`
- **LandscapeTextureRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `TextureSet`, `MaterialType`, `HavokFriction`, `HavokRestitution`, `TextureSpecularExponent`, `Grasses`, `Flags`
- **LeveledItemRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ObjectBounds`, `ChanceNone`, `Flags`, `Global`, `Entries`
- **LeveledNpcRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ChanceNone`, `Flags`, `Global`, `Entries`, `ModelAndBounds`
- **LeveledSpellRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ObjectBounds`, `ChanceNone`, `Flags`, `Entries`
- **LightingTemplateRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `AmbientColor`, `DirectionalColor`, `FogNearColor`, `FogNear`, `FogFar`, `DirectionalRotationXY`, `DirectionalRotationZ`, `DirectionalFade`, `FogClipDistance`, `FogPower`, `AmbientColors`, `FogFarColor`, `FogMax`, `LightFadeStartDistance`, `LightFadeEndDistance`, `DirectionalAmbientColors`
- **LightRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `MajorFlags`, `Name`, `VirtualMachineAdapter`, `ModelAndBounds`, `Icons`, `Destructible`, `Time`, `Radius`, `Color`, `Flags`, `FalloffExponent`, `FOV`, `NearClip`, `FlickerPeriod`, `FlickerIntensityAmplitude`, `FlickerMovementAmplitude`, `Value`, `Weight`, `FadeValue`, `Sound`, `Lens`
- **LoadScreenRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Icons`, `Description`, `Conditions`, `LoadingScreenNif`, `InitialScale`, `InitialRotation`, `RotationOffsetConstraints`, `InitialTranslationOffset`, `CameraPath`, `MajorFlags`
- **LocationRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Keywords`, `ParentLocation`, `Music`, `UnreportedCrimeFaction`, `WorldLocationMarkerRef`, `WorldLocationRadius`, `HorseMarkerRef`, `Color`
- **LocationReferenceTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Color`
- **MagicEffectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `VirtualMachineAdapter`, `Description`, `BaseCost`, `Flags`, `CastType`, `TargetType`, `MagicSkill`, `ResistValue`, `SecondActorValue`, `CastingSoundLevel`, `MenuDisplayObject`, `Keywords`, `CastingLight`, `HitShader`, `EnchantShader`, `Projectile`, `Explosion`, `CastingArt`, `HitEffectArt`, `ImpactData`, `DualCastArt`, `EnchantArt`, `HitVisuals`, `EnchantVisuals`, `EquipAbility`, `ImageSpaceModifier`, `PerkToApply`, `TaperWeight`, `MinimumSkillLevel`, `SpellmakingArea`, `SpellmakingCastingTime`, `TaperCurve`, `TaperDuration`, `SecondActorValueWeight`, `SkillUsageMultiplier`, `DualCastScale`, `ScriptEffectAIScore`, `ScriptEffectAIDelayTime`, `CounterEffects`, `Sounds`, `Archetype`, `Conditions`
- **MaterialObjectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Model`, `DNAMs`, `FalloffScale`, `FalloffBias`, `NoiseUvScale`, `MaterialUvScale`, `ProjectionVector`, `NormalDampener`, `SinglePassColor`, `Flags`, `HasSnow`
- **MaterialTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Parent`, `Name`, `HavokDisplayColor`, `Buoyancy`, `Flags`, `HavokImpactDataSet`
- **MessageRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Description`, `Name`, `INAM`, `Quest`, `Flags`, `DisplayTime`, `MenuButtons`
- **MiscItemRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `VirtualMachineAdapter`, `ModelAndBounds`, `Icons`, `Destructible`, `PickUpSound`, `PutDownSound`, `Keywords`, `Value`, `Weight`, `MajorFlags`
- **MoveableStaticRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `ModelAndBounds`, `Destructible`, `Flags`, `LoopingSound`, `MajorFlags`
- **MovementTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `LeftWalk`, `LeftRun`, `RightWalk`, `RightRun`, `ForwardWalk`, `ForwardRun`, `BackWalk`, `BackRun`, `RotateInPlaceWalk`, `RotateInPlaceRun`, `RotateWhileMovingRun`, `AnimationChangeThresholds`
- **MusicTrackRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Type`, `Duration`, `FadeOut`, `TrackFilename`, `FinaleFilename`, `LoopData`, `CuePoints`, `Conditions`, `Tracks`
- **MusicTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Flags`, `Data`, `FadeDuration`, `Tracks`
- **NavigationMeshRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Data`, `ONAM`, `PNAM`, `NNAM`, `MajorFlags`
- **NpcRecordHandler.cs**: `Name`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `MajorFlags`, `DeathItem`, `CombatOverridePackageList`, `SpectatorOverridePackageList`, `Configuration.Flags`, `Configuration.MagickaOffset`, `Configuration.StaminaOffset`, `Configuration.Level`, `Configuration.CalcMinLevel`, `Configuration.CalcMaxLevel`, `Configuration.SpeedMultiplier`, `Configuration.DispositionBase`, `Configuration.TemplateFlags`, `Configuration.HealthOffset`, `Configuration.BleedoutOverride`, `EditorID`, `Class`, `AIData.Aggression`, `AIData.Confidence`, `AIData.EnergyLevel`, `AIData.Responsibility`, `AIData.Mood`, `AIData.Assistance`, `AIData.AggroRadiusBehavior`, `AIData.Warn`, `AIData.WarnOrAttack`, `AIData.Attack`, `ObserveDeadBodyOverridePackageList`, `Factions`, `Packages`, `ActorEffect`, `VirtualMachineAdapter`, `Items`, `Keywords`, `PlayerSkills.Health`, `PlayerSkills.Magicka`, `PlayerSkills.Stamina`, `PlayerSkills.FarAwayModelDistance`, `PlayerSkills.GearedUpWeapons`, `PlayerSkills.SkillValues`, `PlayerSkills.SkillOffsets`, `FaceMorph`, `FaceParts`, `TextureLighting`, `TintLayers`, `Race`, `Destructible`, `Height`, `Weight`, `ObjectBounds`, `Voice`, `Template`, `ShortName`, `NAM5`, `SoundLevel`, `HeadParts`, `WornArmor`, `AttackRace`, `HairColor`, `DefaultOutfit`, `FarAwayModel`, `Attacks`, `GuardWarnOverridePackageList`, `Perks`, `CombatStyle`, `GiftFilter`, `SleepingOutfit`, `DefaultPackageList`, `CrimeFaction`, `HeadTexture`, `Sound`
- **ObjectEffectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `ObjectBounds`, `EnchantmentCost`, `CastType`, `EnchantmentAmount`, `TargetType`, `EnchantType`, `ChargeTime`, `BaseEnchantment`, `WornRestrictions`, `Effects`, `Flags`
- **OutfitRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Items`
- **PackageRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `VirtualMachineAdapter`, `Flags`, `Type`, `InterruptOverride`, `PreferredSpeed`, `InterruptFlags`, `ScheduleMonth`, `ScheduleDayOfWeek`, `ScheduleDate`, `ScheduleHour`, `ScheduleMinute`, `ScheduleDurationInMinutes`, `Conditions`, `IdleAnimations`, `CombatStyle`, `OwnerQuest`, `OnBegin`, `OnEnd`, `OnChange`
- **PerkRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `Name`, `Description`, `Icons`, `Conditions`, `Trait`, `Level`, `NumRanks`, `Playable`, `Hidden`, `NextPerk`, `Effects`
- **PlacedHazardRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Hazard`
- **PlacedNpcRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `MajorFlags`, `Base`, `EncounterZone`, `RagdollData`, `RagdollBipedData`, `Patrol`, `LevelModifier`, `MerchantContainer`, `Count`, `Radius`, `Health`, `LinkedReferences`, `ActivateParents`, `LinkedReferenceColor`, `PersistentLocation`, `LocationReference`, `IsIgnoredBySandbox`, `LocationRefTypes`, `HeadTrackingWeight`, `Horse`, `FavorCost`, `EnableParent`, `Owner`, `FactionRank`, `Emittance`, `MultiBoundReference`, `IsIgnoredBySandbox2`, `Scale`, `Placement`, `VirtualMachineAdapter`
- **PlacedObjectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Base`, `Owner`, `Scale`, `LocationReference`, `LinkedReferences`, `LinkedRooms`, `ImageSpace`, `LightingTemplate`, `BoundHalfExtents`, `Primitive`, `OcclusionPlane`, `Portals`, `RoomPortal`, `Radius`, `Reflections`, `LitWater`, `Emittance`, `TeleportMessageBox`, `MultiBoundReference`, `SpawnContainer`, `LeveledItemBaseObject`, `PersistentLocation`, `EncounterZone`, `NavigationDoorLink`, `LocationRefTypes`, `IsMultiBoundPrimitive`, `IsIgnoredBySandbox`, `IsOpenByDefault`, `FactionRank`, `ItemCount`, `Charge`, `HeadTrackingWeight`, `FavorCost`, `CollisionLayer`, `LevelModifier`, `TeleportDestination`, `ActivateParents`, `Lock`, `AttachRef`, `Action`, `LightData`, `Alpha`, `Patrol`, `MapMarker`, `Placement`, `VirtualMachineAdapter`, `EnableParent`, `WaterVelocity`, `XCZR`, `XCZC`, `XORD`, `RagdollData`, `RagdollBipedData`, `XWCN`, `XWCS`, `XCVL`, `XCZA`, `DistantLodData`
- **ProjectileRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `ModelAndBounds`, `Destructible`, `Flags`, `Trajectory`, `Light`, `MuzzleFlashBehavior`, `TracerChance`, `ExplosionBehavior`, `Sound`, `FadeDuration`, `ImpactForce`, `PickupBehavior`, `DisableBehavior`, `Collision`, `DecalData`, `SoundLevel`
- **QuestRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `VirtualMachineAdapter.Presence`, `VirtualMachineAdapter.Version`, `VirtualMachineAdapter.ObjectFormat`, `VirtualMachineAdapter.Scripts`, `VirtualMachineAdapter.ExtraBindDataVersion`, `VirtualMachineAdapter.FileName`, `VirtualMachineAdapter.Fragments`, `VirtualMachineAdapter.Aliases`, `Flags`, `Priority`, `Type`, `Event`, `TextDisplayGlobals`, `Filter`, `NextAliasID`, `Description`, `DialogConditions`, `EventConditions`, `Stages`, `Objectives`, `Aliases`
- **RaceRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Description`, `ActorEffect`, `Skin`, `BodyTemplate`, `Keywords`, `SkillBoosts`, `Height`, `Weight`, `Flags`, `Starting`, `BaseCarryWeight`, `BaseMass`, `AccelerationRate`, `DecelerationRate`, `Size`, `HeadBipedObject`, `HairBipedObject`, `InjuredHealthPercent`, `ShieldBipedObject`, `Regen`, `UnarmedDamage`, `UnarmedReach`, `BodyBipedObject`, `AimAngleTolerance`, `FlightRadius`, `AngularAccelerationRate`, `AngularTolerance`, `MountData`, `SkeletalModel`, `MovementTypeNames`, `Voices`, `DecapitateArmors`, `DefaultHairColors`, `NumberOfTintsInList`, `FacegenMainClamp`, `FacegenFaceClamp`, `AttackRace`, `Attacks`, `BodyData`, `Hairs`, `Eyes`, `BodyPartData`, `BehaviorGraph`, `MaterialType`, `ImpactDataSet`, `DecapitationFX`, `OpenLootSound`, `CloseLootSound`, `BipedObjectNames`, `MovementTypes`, `EquipmentFlags`, `EquipmentSlots`, `UnarmedEquipSlot`, `FaceFxPhonemes`, `BaseMovementDefaultWalk`, `BaseMovementDefaultRun`, `BaseMovementDefaultSwim`, `BaseMovementDefaultFly`, `BaseMovementDefaultSneak`, `BaseMovementDefaultSprint`, `HeadData`, `MorphRace`, `ArmorRace`, `MajorFlags`
- **RegionRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `MapColor`, `Worldspace`, `RegionAreas`, `Objects`, `Weather`, `Map`, `Land`, `Grasses`, `Sounds`, `MajorFlags`
- **RelationshipRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Parent`, `Child`, `Rank`, `Flags`, `AssociationType`, `MajorFlags`
- **ReverbParametersRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ReverbData`
- **SceneRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `VirtualMachineAdapter.Version`, `VirtualMachineAdapter.ObjectFormat`, `VirtualMachineAdapter.Scripts`, `VirtualMachineAdapter.ScriptFragments`, `Flags`, `Phases`, `Actors`, `Actions`, `Quest`, `LastActionIndex`, `VNAM`, `Conditions`
- **ScrollRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Keywords`, `MenuDisplayObject`, `EquipmentType`, `Description`, `ModelAndBounds`, `Destructible`, `PickUpSound`, `PutDownSound`, `Value`, `Weight`, `BaseCost`, `Flags`, `Type`, `ChargeTime`, `CastType`, `TargetType`, `CastDuration`, `Range`, `HalfCostPerk`, `Effects`
- **ShaderParticleGeometryRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `GravityVelocity`, `RotationVelocity`, `ParticleSizeX`, `ParticleSizeY`, `CenterOffsetMin`, `CenterOffsetMax`, `InitialRotationRange`, `NumSubtexturesX`, `NumSubtexturesY`, `Type`, `BoxSize`, `ParticleDensity`, `ParticleTexture`
- **ShoutRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `MenuDisplayObject`, `Description`, `WordsOfPower`, `MajorFlags`
- **SoulGemRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `ModelAndBounds`, `Icons`, `Destructible`, `PickUpSound`, `PutDownSound`, `Keywords`, `Value`, `Weight`, `ContainedSoul`, `MaximumCapacity`, `LinkedTo`, `MajorFlags`
- **SoundCategoryRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Flags`, `Parent`, `StaticVolumeMultiplier`, `DefaultMenuVolume`
- **SoundDescriptorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Type`, `Category`, `AlternateSoundFor`, `SoundFiles`, `OutputModel`, `String`, `Conditions`, `LoopAndRumble`, `Pitch`, `Priority`, `Volume`
- **SoundMarkerRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `ObjectBounds`, `FNAM`, `SNDD`, `SoundDescriptor`
- **SoundOutputModelRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Data`, `FNAM`, `Type`, `CNAM`, `SNAM`, `OutputChannels`, `Attenuation`
- **SpellRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `ObjectBounds`, `MenuDisplayObject`, `Description`, `Flags`, `Keywords`, `EquipmentType`, `BaseCost`, `Type`, `ChargeTime`, `CastType`, `TargetType`, `CastDuration`, `Range`, `HalfCostPerk`, `Effects`
- **StaticRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ModelAndBounds`, `MaxAngle`, `Material`, `Flags`, `Lod`
- **StoryManagerBranchNodeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `MaxConcurrentQuests`
- **StoryManagerEventNodeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `MaxConcurrentQuests`, `Type`
- **StoryManagerQuestNodeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `QuestFlags`, `MaxConcurrentQuests`, `MaxNumQuestsToRun`, `Quests`
- **TalkingActivatorRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `VirtualMachineAdapter`, `Name`, `ModelAndBounds`, `Destructible`, `Keywords`, `LoopingSound`, `Voice`
- **TextureSetRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `ObjectBounds`, `TextureDefinition`, `Decal`
- **TreeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `VirtualMachineAdapter`, `ModelAndBounds`, `Ingredient`, `HarvestSound`, `Production`, `Name`, `TrunkFlexibility`, `BranchFlexibility`, `LeafAmplitude`, `LeafFrequency`, `MajorFlags`
- **VisualEffectRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `EffectArt`, `Shader`, `Flags`
- **VoiceTypeRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Flags`
- **WaterRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Opacity`, `Flags`, `MNAM`, `Material`, `OpenSound`, `Spell`, `ImageSpace`, `DamagePerSecond`, `SpecularSunPower`, `WaterReflectivity`, `WaterFresnel`, `FogAboveWaterDistanceNearPlane`, `FogAboveWaterDistanceFarPlane`, `ShallowColor`, `DeepColor`, `ReflectionColor`, `DisplacementStartingSize`, `DisplacementFoce`, `DisplacementVelocity`, `DisplacementFalloff`, `DisplacementDampner`, `NoiseFalloff`, `NoiseLayerOneWindDirection`, `NoiseLayerTwoWindDirection`, `NoiseLayerThreeWindDirection`, `NoiseLayerOneWindSpeed`, `NoiseLayerTwoWindSpeed`, `NoiseLayerThreeWindSpeed`, `FogAboveWaterAmount`, `FogUnderWaterAmount`, `FogUnderWaterDistanceNearPlane`, `FogUnderWaterDistanceFarPlane`, `WaterRefractionMagnitude`, `SpecularPower`, `SpecularRadius`, `SpecularBrightness`, `NoiseLayerOneUvScale`, `NoiseLayerTwoUvScale`, `NoiseLayerThreeUvScale`, `NoiseLayerOneAmplitudeScale`, `NoiseLayerTwoAmplitudeScale`, `NoiseLayerThreeAmplitudeScale`, `WaterReflectionMagnitude`, `SpecularSunSparkleMagnitude`, `SpecularSunSpecularMagnitude`, `DepthReflections`, `DepthRefraction`, `DepthNormals`, `DepthSpecularLighting`, `SpecularSunSparklePower`, `NoiseFlowmapScale`, `GNAM`, `LinearVelocity`, `AngularVelocity`, `NoiseLayerOneTexture`, `NoiseLayerTwoTexture`, `NoiseLayerThreeTexture`, `FlowNormalsNoiseTexture`
- **WeaponRecordHandler.cs**: `EditorID`, `Name`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `MajorFlags`, `ModelAndBounds`, `Icons`, `Keywords`, `VirtualMachineAdapter`, `ObjectEffect`, `EnchantmentAmount`, `Destructible`, `EquipmentType`, `BlockBashImpact`, `AlternateBlockMaterial`, `PickUpSound`, `PutDownSound`, `Description`, `ScopeModel`, `ImpactDataSet`, `FirstPersonModel`, `AttackSound`, `AttackSound2D`, `AttackLoopSound`, `AttackFailSound`, `IdleSound`, `EquipSound`, `UnequipSound`, `BasicStats.Value`, `BasicStats.Weight`, `BasicStats.Damage`, `DetectionSoundLevel`, `Template`, `Data.AnimationType`, `Data.Speed`, `Data.Reach`, `Data.Flags`, `Data.SightFOV`, `Data.BaseVATStoHitChance`, `Data.AttackAnimation`, `Data.NumProjectiles`, `Data.EmbeddedWeaponAV`, `Data.RangeMin`, `Data.RangeMax`, `Data.OnHit`, `Data.AnimationAttackMult`, `Data.RumbleLeftMotorStrength`, `Data.RumbleRightMotorStrength`, `Data.RumbleDuration`, `Data.Skill`, `Data.Resist`, `Data.Stagger`, `Critical.Damage`, `Critical.PercentMult`, `Critical.Flags`, `Critical.Effect`
- **WeatherRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `DNAM`, `CNAM`, `ANAM`, `BNAM`, `LNAM`, `Precipitation`, `VisualEffect`, `ONAM`, `CloudTextures`, `Clouds`, `SkyUpperColor`, `FogNearColor`, `UnknownColor`, `AmbientColor`, `SunlightColor`, `SunColor`, `StarsColor`, `SkyLowerColor`, `HorizonColor`, `EffectLightingColor`, `CloudLodDiffuseColor`, `CloudLodAmbientColor`, `FogFarColor`, `SkyStaticsColor`, `WaterMultiplierColor`, `SunGlareColor`, `MoonGlareColor`, `FogDistanceDayNear`, `FogDistanceDayFar`, `FogDistanceNightNear`, `FogDistanceNightFar`, `FogDistanceDayPower`, `FogDistanceNightPower`, `FogDistanceDayMax`, `FogDistanceNightMax`, `WindSpeed`, `TransDelta`, `SunGlare`, `SunDamage`, `PrecipitationBeginFadeIn`, `PrecipitationEndFadeOut`, `ThunderLightningBeginFadeIn`, `ThunderLightningEndFadeOut`, `ThunderLightningFrequency`, `Flags`, `LightningColor`, `VisualEffectBegin`, `VisualEffectEnd`, `WindDirection`, `WindDirectionRange`, `Sounds`, `SkyStatics`, `ImageSpaces`, `VolumetricLighting`, `DirectionalAmbientLightingColors`, `NAM2`, `NAM3`, `Aurora`, `SunGlareLensFlare`
- **WordOfPowerRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `SkyrimMajorRecordFlags`, `Name`, `Translation`
- **WorldspaceRecordHandler.cs**: `EditorID`, `MajorRecordFlagsRaw`, `Name`, `Location`, `Water`, `LodData`, `Music`, `MapData`, `MapImage`, `CloudModel`, `Flags`, `WorldMapOffset`, `DistantLodMultiplier`, `FixedDimensionsCenterCell`, `InteriorLighting`, `EncounterZone`, `Parent`, `Climate`, `LandDefaults`, `WaterNoiseTexture`, `HdLodDiffuseTexture`, `HdLodNormalTexture`, `WaterEnvironmentMap`
