# Actors and character data

[Patching guide](README.md) · [Known issues](KNOWN-ISSUES.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated.

<a id="astp-associationtype"></a>

## ASTP — Association Type

| Properties | How they are patched |
|---|---|
| `IsFamily` | Select each value separately. |
| `ParentTitle`, `Title` | Select each whole value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/AssociationTypeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="avif-actorvalueinformation"></a>

## AVIF — Actor Value Information

| Properties | How they are patched |
|---|---|
| `Name`, `Description`, `Abbreviation` | Select each value separately. |
| `Skill` | Select each whole value separately. |
| `PerkTree` | Select each whole collection separately. |
| `CNAM` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ActorValueInformationRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="bptd-bodypartdata"></a>

## BPTD — Body Part Data

| Properties | How they are patched |
|---|---|
| `Model` | Select each whole value separately. |
| `Parts` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/BodyPartDataRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="clas-class"></a>

## CLAS — Class

| Properties | How they are patched |
|---|---|
| `Name`, `Description`, `Icon`, `Teaches`, `MaxTrainingLevel`, `BleedoutDefault`, `VoicePoints` | Select each value separately. |
| `SkillWeights`, `StatWeights` | Select each whole collection separately. |
| `Unknown`, `Unknown2` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ClassRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="clfm-colorrecord"></a>

## CLFM — Color Record

| Properties | How they are patched |
|---|---|
| `Name`, `Color`, `Playable` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ColorRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="csty-combatstyle"></a>

## CSTY — Combat Style

| Properties | How they are patched |
|---|---|
| `OffensiveMult`, `DefensiveMult`, `GroupOffensiveMult`, `EquipmentScoreMultMelee`, `EquipmentScoreMultMagic`, `EquipmentScoreMultRanged`, `EquipmentScoreMultShout`, `EquipmentScoreMultUnarmed`, `EquipmentScoreMultStaff`, `AvoidThreatChance`, `CSMD`, `LongRangeStrafeMult` | Select each value separately. |
| `Flags`, `MajorFlags` | Merge registered flag bits separately. |
| `Melee`, `CloseRange`, `Flight` | Select each whole value separately. |
| `CSGDDataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/CombatStyleRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="equp-equiptype"></a>

## EQUP — Equip Type

| Properties | How they are patched |
|---|---|
| `UseAllParents` | Select each value separately. |
| `SlotParents` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/EquipTypeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="eyes-eyes"></a>

## EYES — Eyes

| Properties | How they are patched |
|---|---|
| `Name`, `Icon` | Select each value separately. |
| `Flags`, `MajorFlags` | Merge registered flag bits separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/EyesRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="fact-faction"></a>

## FACT — Faction

| Properties | How they are patched |
|---|---|
| `Name`, `ExteriorJailMarker`, `FollowerWaitMarker`, `StolenGoodsContainer`, `PlayerInventoryContainer`, `SharedCrimeFactionList`, `JailOutfit`, `VendorBuySellList`, `MerchantContainer` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `CrimeValues`, `VendorValues`, `VendorLocation` | Select each whole value separately. |
| `Relations`, `Ranks` | Merge rows by key. |
| `Conditions` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/FactionRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="hdpt-headpart"></a>

## HDPT — Head Part

| Properties | How they are patched |
|---|---|
| `Name`, `Type`, `TextureSet`, `Color`, `ValidRaces` | Select each value separately. |
| `Flags`, `MajorFlags` | Merge registered flag bits separately. |
| `Model` | Select each whole value separately. |
| `ExtraParts` | Merge rows by key. |
| `Parts` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/HeadPartRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="lvln-levelednpc"></a>

## LVLN — Leveled Npc

Entries match by (Level, Reference), with complete row payloads/counts.

| Properties | How they are patched |
|---|---|
| `ChanceNone`, `Global` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Entries` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/LeveledNpcRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="movt-movementtype"></a>

## MOVT — Movement Type

| Properties | How they are patched |
|---|---|
| `Name`, `LeftWalk`, `LeftRun`, `RightWalk`, `RightRun`, `ForwardWalk`, `ForwardRun`, `BackWalk`, `BackRun`, `RotateInPlaceWalk`, `RotateInPlaceRun`, `RotateWhileMovingRun` | Select each value separately. |
| `AnimationChangeThresholds` | Select each whole value separately. |
| `SPEDDataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/MovementTypeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="npc_-npc"></a>

## NPC_ — Npc

Configuration.Flags selects only Essential/Protected together; other configuration bits are retained by the setter. Essential wins if both are set. The default permits protection upgrades and requires ownership permission for downgrades; HighestWins and ordinary forwarding are alternatives. Level is one fixed/player-level value. Factions and Perks match by reference and require permission for rank changes. Items match by item reference, reconcile duplicates, and handle count, extra-data presence, condition and coherent owner data with permission checks. Attacks match by AttackEvent.

| Properties | How they are patched |
|---|---|
| `Name`, `DeathItem`, `CombatOverridePackageList`, `SpectatorOverridePackageList`, `Configuration.MagickaOffset`, `Configuration.StaminaOffset`, `Configuration.CalcMinLevel`, `Configuration.CalcMaxLevel`, `Configuration.SpeedMultiplier`, `Configuration.DispositionBase`, `Configuration.HealthOffset`, `Configuration.BleedoutOverride`, `Class`, `AIData.Aggression`, `AIData.Confidence`, `AIData.EnergyLevel`, `AIData.Responsibility`, `AIData.Mood`, `AIData.Assistance`, `AIData.AggroRadiusBehavior`, `AIData.Warn`, `AIData.WarnOrAttack`, `AIData.Attack`, `ObserveDeadBodyOverridePackageList`, `PlayerSkills.Health`, `PlayerSkills.Magicka`, `PlayerSkills.Stamina`, `PlayerSkills.FarAwayModelDistance`, `PlayerSkills.GearedUpWeapons`, `TextureLighting`, `Race`, `Height`, `Weight`, `Voice`, `Template`, `ShortName`, `NAM5`, `SoundLevel`, `WornArmor`, `AttackRace`, `HairColor`, `DefaultOutfit`, `FarAwayModel`, `GuardWarnOverridePackageList`, `CombatStyle`, `GiftFilter`, `SleepingOutfit`, `DefaultPackageList`, `CrimeFaction`, `HeadTexture` | Select each value separately. |
| `MajorFlags`, `Configuration.TemplateFlags` | Merge registered flag bits separately. |
| `Configuration.Level`, `FaceMorph`, `FaceParts`, `Destructible`, `ObjectBounds`, `Sound` | Select each whole value separately. |
| `Configuration.Flags` | Select Essential/Protected together; preserve other bits. See the protection rule above. |
| `PlayerSkills.SkillValues`, `PlayerSkills.SkillOffsets` | Select each whole collection separately. |
| `Factions`, `ActorEffect`, `VirtualMachineAdapter`, `Items`, `Keywords`, `TintLayers`, `HeadParts`, `Attacks`, `Perks` | Merge rows by key. |
| `Packages` | Merge aligned rows in order. |
| `AIData.Unused`, `PlayerSkills.Unused`, `PlayerSkills.Unused2` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/NpcRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="otft-outfit"></a>

## OTFT — Outfit

| Properties | How they are patched |
|---|---|
| `Items` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/OutfitRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="race-race"></a>

## RACE — Race

SkillBoosts owns all seven physical skill slots. Padding is ignored; output is sorted and padded to seven. Duplicate skills or more than seven active boosts are rejected. Each gendered aggregate keeps male and female together. Attacks match by AttackEvent; the complete attack data travels with each row.

| Properties | How they are patched |
|---|---|
| `Name`, `Description`, `Skin`, `BaseCarryWeight`, `BaseMass`, `AccelerationRate`, `DecelerationRate`, `Size`, `HeadBipedObject`, `HairBipedObject`, `InjuredHealthPercent`, `ShieldBipedObject`, `UnarmedDamage`, `UnarmedReach`, `BodyBipedObject`, `AimAngleTolerance`, `FlightRadius`, `AngularAccelerationRate`, `AngularTolerance`, `NumberOfTintsInList`, `FacegenMainClamp`, `FacegenFaceClamp`, `AttackRace`, `BodyPartData`, `MaterialType`, `ImpactDataSet`, `DecapitationFX`, `OpenLootSound`, `CloseLootSound`, `EquipmentFlags`, `UnarmedEquipSlot`, `BaseMovementDefaultWalk`, `BaseMovementDefaultRun`, `BaseMovementDefaultSwim`, `BaseMovementDefaultFly`, `BaseMovementDefaultSneak`, `BaseMovementDefaultSprint`, `MorphRace`, `ArmorRace` | Select each value separately. |
| `Flags`, `MajorFlags` | Merge registered flag bits separately. |
| `BodyTemplate`, `Height`, `Weight`, `MountData`, `SkeletalModel`, `Voices`, `DecapitateArmors`, `DefaultHairColors`, `BodyData`, `BehaviorGraph`, `FaceFxPhonemes`, `HeadData` | Select each whole value separately. |
| `Starting`, `Regen`, `BipedObjectNames` | Select each whole collection separately. |
| `ActorEffect`, `Keywords`, `SkillBoosts`, `MovementTypeNames`, `Attacks`, `Hairs`, `Eyes`, `MovementTypes`, `EquipmentSlots` | Merge rows by key. |
| `Unknown` | Not independently forwarded. |
| `DATADataTypeState`, `ExportingExtraNam2` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/RaceRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="rela-relationship"></a>

## RELA — Relationship

| Properties | How they are patched |
|---|---|
| `Parent`, `Child`, `Rank`, `AssociationType` | Select each value separately. |
| `Flags`, `MajorFlags` | Merge registered flag bits separately. |
| `Unknown` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/RelationshipRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="vtyp-voicetype"></a>

## VTYP — Voice Type

| Properties | How they are patched |
|---|---|
| `Flags` | Merge registered flag bits separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/VoiceTypeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).
