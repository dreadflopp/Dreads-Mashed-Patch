# Actors and character data

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="astp-associationtype"></a>

## ASTP — Association Type

| Properties | How they are patched |
|---|---|
| `IsFamily` | Select each value separately. |
| `ParentTitle`, `Title` | Select each whole value separately. |


<a id="avif-actorvalueinformation"></a>

## AVIF — Actor Value Information

| Properties | How they are patched |
|---|---|
| `Name`, `Description`, `Abbreviation` | Select each value separately. |
| `Skill` | Select each whole value separately. |
| `PerkTree` | Select each whole collection separately. |
| `CNAM` | Not independently forwarded. |


<a id="bptd-bodypartdata"></a>

## BPTD — Body Part Data

Parts merge by PartNode, retaining duplicate occurrences. Damage, health, flags, gore effects and targeting follow that ordinary row decision. The parent model and limb replacement models have separate mesh decisions. Limb scale and texture-file hashes stay with each limb model; removed rows stay removed. PoseMatching remains ordinary text.

| Properties | How they are patched |
|---|---|
| `Parts` | Merge ordinary row data by PartNode within its keyed list policy; resource fields have a separate decision. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `BodyModel` (Meshes) | `Model` | Normal | Use the existing complete-value or collection policy. |
| `LimbMeshes` (Meshes) | `Parts.LimbMeshes` | Normal | Select limb models, scale and model hashes for surviving PartNode occurrences. |


<a id="clas-class"></a>

## CLAS — Class

| Properties | How they are patched |
|---|---|
| `Name`, `Description`, `Teaches`, `MaxTrainingLevel`, `BleedoutDefault`, `VoicePoints` | Select each value separately. |
| `SkillWeights`, `StatWeights` | Select each whole collection separately. |
| `Unknown`, `Unknown2` | Not independently forwarded. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `Icon` (IconsAndImages) | `Icon` | Normal | Use the existing complete-value or collection policy. |


<a id="clfm-colorrecord"></a>

## CLFM — Color Record

| Properties | How they are patched |
|---|---|
| `Name`, `Color`, `Playable` | Select each value separately. |


<a id="csty-combatstyle"></a>

## CSTY — Combat Style

| Properties | How they are patched |
|---|---|
| `OffensiveMult`, `DefensiveMult`, `GroupOffensiveMult`, `EquipmentScoreMultMelee`, `EquipmentScoreMultMagic`, `EquipmentScoreMultRanged`, `EquipmentScoreMultShout`, `EquipmentScoreMultUnarmed`, `EquipmentScoreMultStaff`, `AvoidThreatChance`, `CSMD`, `LongRangeStrafeMult` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Melee`, `CloseRange`, `Flight` | Select each whole value separately. |
| `CSGDDataTypeState` | Serialization/unused state; no independent decision. |


<a id="equp-equiptype"></a>

## EQUP — Equip Type

| Properties | How they are patched |
|---|---|
| `UseAllParents` | Select each value separately. |
| `SlotParents` | Merge aligned rows in order. |


<a id="eyes-eyes"></a>

## EYES — Eyes

| Properties | How they are patched |
|---|---|
| `Name` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `EyeTexture` (Textures) | `Icon` | Normal | Use the existing complete-value or collection policy. |


<a id="fact-faction"></a>

## FACT — Faction

| Properties | How they are patched |
|---|---|
| `Name`, `ExteriorJailMarker`, `FollowerWaitMarker`, `StolenGoodsContainer`, `PlayerInventoryContainer`, `SharedCrimeFactionList`, `JailOutfit`, `VendorBuySellList`, `MerchantContainer` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `CrimeValues`, `VendorValues`, `VendorLocation` | Select each whole value separately. |
| `Relations`, `Ranks` | Merge rows by key. |
| `Conditions` | Merge aligned rows in order. |


<a id="hdpt-headpart"></a>

## HDPT — Head Part

| Properties | How they are patched |
|---|---|
| `Name`, `ValidRaces` | Select each value independently. |
| `Flags.Playable`, `Flags.Male`, `Flags.Female` | Merge these registered flag bits separately. |

The model and its morph/extra-part assets and rendering overrides come from one selected override through normal grouped forwarding; returning to the original snapshot requires permission from its current owner under the [shared rules](README.md#how-a-value-is-selected). ValidRaces and Playable/Male/Female flags remain independent. Extra-part sorting alone is not an edit; duplicate counts and morph-file order remain significant. Header flags and EditorID retain their shared policies. Referenced NPC faces still depend on compatible winning mesh and FaceGen resources.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `HeadMesh` (Meshes) | `Mesh` | Normal | Use the existing complete-value or collection policy. |


<a id="lvln-levelednpc"></a>

## LVLN — Leveled Npc

Entries match by (Level, Reference), with complete row payloads/counts.

| Properties | How they are patched |
|---|---|
| `ChanceNone`, `Global` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Entries` | Merge rows by key. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="movt-movementtype"></a>

## MOVT — Movement Type

| Properties | How they are patched |
|---|---|
| `Name`, `LeftWalk`, `LeftRun`, `RightWalk`, `RightRun`, `ForwardWalk`, `ForwardRun`, `BackWalk`, `BackRun`, `RotateInPlaceWalk`, `RotateInPlaceRun`, `RotateWhileMovingRun` | Select each value separately. |
| `AnimationChangeThresholds` | Select each whole value separately. |
| `SPEDDataTypeState` | Serialization/unused state; no independent decision. |


<a id="npc_-npc"></a>

## NPC_ — Npc

Configuration.Flags selects only Essential/Protected together; other configuration bits are retained by the setter. Essential wins if both are set. The default permits protection upgrades and requires ownership permission for downgrades; HighestWins and ordinary forwarding are alternatives. Level is one fixed/player-level value. Factions and Perks match by reference and require permission for rank changes. Items use the shared CONT/NPC inventory rules: they match by item reference, reserve unchanged duplicate rows before matching remaining occurrences, and handle count, extra-data presence, condition and coherent owner data with permission checks. Source snapshots and output rows are complete independent generated copies. Attacks match by AttackEvent.

| Properties | How they are patched |
|---|---|
| `Height` | Select independently through ordinary change/reversion rules, comparing with the actual original height rather than assuming 1.0. |
| `HairColor` | Select the color-record reference independently through ordinary change/reversion rules, separately from the appearance group. |
| `TemplateAndTraits`: `Template`, template `Traits` | Select the template pointer and Traits bit as one value through ordinary change/reversion rules. Other template bits remain independent. |
| `Name`, `DeathItem`, `CombatOverridePackageList`, `SpectatorOverridePackageList`, `Configuration.MagickaOffset`, `Configuration.StaminaOffset`, `Configuration.CalcMinLevel`, `Configuration.CalcMaxLevel`, `Configuration.SpeedMultiplier`, `Configuration.DispositionBase`, `Configuration.HealthOffset`, `Configuration.BleedoutOverride`, `Class`, `AIData.Aggression`, `AIData.Confidence`, `AIData.EnergyLevel`, `AIData.Responsibility`, `AIData.Mood`, `AIData.Assistance`, `AIData.AggroRadiusBehavior`, `AIData.Warn`, `AIData.WarnOrAttack`, `AIData.Attack`, `ObserveDeadBodyOverridePackageList`, `PlayerSkills.Health`, `PlayerSkills.Magicka`, `PlayerSkills.Stamina`, `PlayerSkills.FarAwayModelDistance`, `PlayerSkills.GearedUpWeapons`, `Voice`, `ShortName`, `NAM5`, `SoundLevel`, `AttackRace`, `DefaultOutfit`, `FarAwayModel`, `GuardWarnOverridePackageList`, `CombatStyle`, `GiftFilter`, `SleepingOutfit`, `DefaultPackageList`, `CrimeFaction` | Select each value separately. |
| `Configuration.TemplateFlags` | Merge registered bits except `Traits`, handled by TemplateAndTraits. |
| `Configuration.Level`, `Sound` | Select each whole value separately. |
| `ObjectBounds` | Keep the effective winner's bounds; no independent forwarding. |
| `Configuration.Flags` | Select Essential/Protected together. Female stays with Appearance; OppositeGenderAnims is selected independently; other bits retain the winning state. |
| `PlayerSkills.SkillValues`, `PlayerSkills.SkillOffsets` | Select each whole collection separately. |
| `Factions`, `ActorEffect`, `Items`, `Keywords`, `Attacks`, `Perks` | Merge rows by key. |
| `Packages` | Merge aligned rows in order. |
| `AIData.Unused`, `PlayerSkills.Unused`, `PlayerSkills.Unused2` | Serialization/unused state; no independent decision. |

Appearance selects the complete group listed below, including race, sex, weight and skin. It does not merge face parts or tint layers. Head-part/tint-layer sorting alone is not an edit; complete entry contents and duplicate counts remain significant. Appearance forwards normally: an unrelated later override carrying the original appearance cannot erase an earlier change without reversion permission. Traits and its template pointer use the same atomic selection rules; protection and other gameplay fields continue through their own rules. This preserves the selected record data but does not generate or validate FaceGen assets.

Height, HairColor and OppositeGenderAnims can come from different mods than the selected face. An edit to only one of these fields cannot replace the face or take ownership of it. Height keeps its ordinary ownership rules.

HairColor selects its own color-record reference through ordinary change/reversion rules, separately from the Appearance group. The race’s available hair colors are a list of color-record references, not an index used by the NPC’s HairColor field. Selecting a color reference does not guarantee that every custom hairstyle or FaceGen mesh will display it.

The animation bit forwards independently; restoring its original value requires a declared or virtual master relationship with its current owner under the [shared rules](README.md#how-a-value-is-selected).

OppositeGenderAnims is relative to the selected sex and the race's animation behavior; forwarding the bit does not preserve an absolute masculine/feminine animation choice if another mod changes sex or race. WornArmor stays with the face because custom skin armor can supply its body, textures or wigs. When Traits inheritance is enabled, the engine can use the template's traits instead of local values; the patcher does not resolve that inheritance or validate compatibility between the selected face and other winning race, head-part or skin resources.

Selected Perks are written even when the winning NPC has no perk list. Absence and a present-empty list remain distinct in memory, and copied rows and perk links are independent of their sources.

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `Appearance` (Appearance) | `Appearance` | Normal | Use the existing complete-value or collection policy. |
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `AnimationChoice` (Animations) | `Configuration.Flags.OppositeGenderAnims` | Normal | Use the existing complete-value or collection policy. |


<a id="otft-outfit"></a>

## OTFT — Outfit

| Properties | How they are patched |
|---|---|
| `Items` | Merge rows by key. |


<a id="race-race"></a>

## RACE — Race

SkillBoosts owns all seven physical skill slots. Padding is ignored; output is sorted and padded to seven. Duplicate skills or more than seven active boosts are rejected. Height, Weight and other intact gendered aggregates retain their existing paired selection. Head resources have the narrower groups below. AttackEvent identifies independently reconciled attack rows. Combat and movement tuning do not select rig references.

| Properties | How they are patched |
|---|---|
| `Name`, `Description`, `BaseCarryWeight`, `BaseMass`, `AccelerationRate`, `DecelerationRate`, `Size`, `HeadBipedObject`, `HairBipedObject`, `InjuredHealthPercent`, `ShieldBipedObject`, `UnarmedDamage`, `UnarmedReach`, `BodyBipedObject`, `AimAngleTolerance`, `FlightRadius`, `AngularAccelerationRate`, `AngularTolerance`, `AttackRace`, `MaterialType`, `ImpactDataSet`, `DecapitationFX`, `OpenLootSound`, `CloseLootSound`, `EquipmentFlags`, `UnarmedEquipSlot`, `BaseMovementDefaultWalk`, `BaseMovementDefaultRun`, `BaseMovementDefaultSwim`, `BaseMovementDefaultFly`, `BaseMovementDefaultSneak`, `BaseMovementDefaultSprint`, `ArmorRace` | Select each value separately. |
| `Flags` | Merge registered flag bits separately, including FaceGenHead. |
| `BodyTemplate`, `Height`, `Weight`, `MountData`, `Voices`, `DecapitateArmors`, `DefaultHairColors`, `FaceFxPhonemes` | Select each whole value separately. |
| `Starting`, `Regen`, `BipedObjectNames` | Select each whole collection separately. |
| `ActorEffect`, `Keywords`, `SkillBoosts`, `Attacks`, `MovementTypeNames`, `MovementTypes`, `Hairs`, `Eyes`, `EquipmentSlots` | Merge rows by key. |
| `HeadData.Male.RacePresets`, `HeadData.Female.RacePresets`, `HeadData.Male.AvailableHairColors`, `HeadData.Female.AvailableHairColors` | Merge each list by key independently. |
| `HeadData.Presence` | Select parent and male/female presence; absent parents gate all child writes. |
| `Unknown` | Not independently forwarded. |
| `DATADataTypeState`, `ExportingExtraNam2` | Serialization/unused state; no independent decision. |

Head shape keeps both genders’ head-part definitions and available morphs with MorphRace
and FaceGen clamps as one appearance group. Head meshes and Skin use
Meshes; face textures, body EGT resources and tint definitions use Textures. Both tint
lists and NumberOfTintsInList select the same authored source. Race presets and available
hair colors merge as independent FormKey lists for each gender. HeadData and gender
presence have a separate structural decision; an absent parent suppresses child writes.

Each gender’s skeleton and behavior graph select together as a group,
independently of the other gender. BodyPartData has its own reference decision under
that classification. AttackRace and the keyed attack/movement collections use ordinary
forwarding alongside those groups. The patcher does not validate
attack-event names against behavior files or body-part node names against skeletons.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `HeadShape` (Appearance) | `HeadData.Male.Shape`, `HeadData.Female.Shape`, `MorphRace`, `FacegenMainClamp`, `FacegenFaceClamp` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `SkinMesh` (Meshes) | `Skin` | Normal | Use the existing complete-value or collection policy. |
| `BodyTextures` (Textures) | `BodyData` | Normal | Use the existing complete-value or collection policy. |
| `MaleHeadMesh` (Meshes) | `HeadData.Male.Mesh` | Normal | Use the existing complete-value or collection policy. |
| `FemaleHeadMesh` (Meshes) | `HeadData.Female.Mesh` | Normal | Use the existing complete-value or collection policy. |
| `MaleFaceTextures` (Textures) | `HeadData.Male.FaceTextures` | Normal | Use the existing complete-value or collection policy. |
| `FemaleFaceTextures` (Textures) | `HeadData.Female.FaceTextures` | Normal | Use the existing complete-value or collection policy. |
| `TintDefinitions` (Textures) | `HeadData.Male.Tints`, `HeadData.Female.Tints`, `NumberOfTintsInList` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `MaleRig` (Animations) | `SkeletalModel.Male`, `BehaviorGraph.Male` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `FemaleRig` (Animations) | `SkeletalModel.Female`, `BehaviorGraph.Female` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `BodyPartReference` (Animations) | `BodyPartData` | Normal | Use the existing complete-value or collection policy. |


<a id="rela-relationship"></a>

## RELA — Relationship

| Properties | How they are patched |
|---|---|
| `Parent`, `Child`, `Rank`, `AssociationType` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Unknown` | Not independently forwarded. |


<a id="vtyp-voicetype"></a>

## VTYP — Voice Type

| Properties | How they are patched |
|---|---|
| `Flags` | Merge registered flag bits separately. |
