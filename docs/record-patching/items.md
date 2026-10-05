# Items and equipment

[Patching guide](README.md) · [Known issues](KNOWN-ISSUES.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="alch-ingestible"></a>

## ALCH — Ingestible

Effects merges by position. Each effect’s BaseEffect, Data and nested Conditions travel together.

| Properties | How they are patched |
|---|---|
| `Name`, `Description`, `PickUpSound`, `PutDownSound`, `EquipmentType`, `Weight`, `Value`, `Addiction`, `AddictionChance`, `ConsumeSound` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Destructible`, `Icons` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Keywords` | Merge rows by key. |
| `Effects` | Merge rows by position. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/IngestibleRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="ammo-ammunition"></a>

## AMMO — Ammunition

| Properties | How they are patched |
|---|---|
| `Name`, `PickUpSound`, `PutDownSound`, `Description`, `Projectile`, `Damage`, `Value`, `Weight`, `ShortName` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Icons`, `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Keywords` | Merge rows by key. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/AmmunitionRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="appa-alchemicalapparatus"></a>

## APPA — Alchemical Apparatus

| Properties | How they are patched |
|---|---|
| `Name`, `PickUpSound`, `PutDownSound`, `Quality`, `Description`, `Value`, `Weight` | Select each value separately. |
| `Icons`, `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `VirtualMachineAdapter` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/AlchemicalApparatusRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="arma-armoraddon"></a>

## ARMA — Armor Addon

Male and female fields are separate. Each model filename is separate from its alternate textures, which match by (Name, Index). Selected filenames also carry model data bytes, but byte-only differences do not trigger forwarding. This is explicit migration policy, confirmed for all four filename handlers in the [comparison audit](COVERAGE-COMPARISON-AUDIT.md#pack-arma-and-navi-boundaries). BodyTemplate.ActsLike44 is not independently patched.

| Properties | How they are patched |
|---|---|
| `WeightSliderEnabled.Male`, `WeightSliderEnabled.Female`, `BodyTemplate.ArmorType`, `Priority.Male`, `Priority.Female`, `DetectionSoundValue`, `WeaponAdjust`, `Race`, `FootstepSound`, `ArtObject`, `SkinTexture.Male`, `SkinTexture.Female`, `TextureSwapList.Male`, `TextureSwapList.Female` | Select each value separately. |
| `BodyTemplate.FirstPersonFlags`, `BodyTemplate.Flags` | Merge registered flag bits separately. |
| `WorldModel.Male.File`, `WorldModel.Female.File`, `FirstPersonModel.Male.File`, `FirstPersonModel.Female.File` | Select each whole value separately. |
| `WorldModel.Male.AlternateTextures`, `WorldModel.Female.AlternateTextures`, `FirstPersonModel.Male.AlternateTextures`, `FirstPersonModel.Female.AlternateTextures`, `AdditionalRaces` | Merge rows by key. |
| `Unknown`, `Unknown2`, `WorldModel.Male.Data`, `WorldModel.Female.Data`, `FirstPersonModel.Male.Data`, `FirstPersonModel.Female.Data` | Not independently forwarded. |
| `BodyTemplate.ActsLike44` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ArmorAddonRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="armo-armor"></a>

## ARMO — Armor

The whole male/female world-model-and-icon value follows the model/bounds rule. BodyTemplate flags and ArmorType remain separate; ActsLike44 is not independently patched.

| Properties | How they are patched |
|---|---|
| `Name`, `ObjectEffect`, `EnchantmentAmount`, `BodyTemplate.ArmorType`, `PickUpSound`, `PutDownSound`, `RagdollConstraintTemplate`, `EquipmentType`, `BashImpactDataSet`, `AlternateBlockMaterial`, `Race`, `Description`, `Value`, `Weight`, `ArmorRating`, `TemplateArmor` | Select each value separately. |
| `BodyTemplate.FirstPersonFlags`, `BodyTemplate.Flags` | Merge registered flag bits separately. |
| `Destructible` | Select each whole value separately. |
| `WorldModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Armature` | Select each whole collection separately. |
| `VirtualMachineAdapter`, `Keywords` | Merge rows by key. |
| `BodyTemplate.ActsLike44` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ArmorRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="book-book"></a>

## BOOK — Book

Teaches keeps the teaching variant and its value together. It does not combine skill and spell teaching.

| Properties | How they are patched |
|---|---|
| `Name`, `Value`, `Weight`, `Description`, `PickUpSound`, `PutDownSound`, `BookText`, `Type`, `InventoryArt` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Destructible`, `Teaches`, `Icons` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Keywords`, `VirtualMachineAdapter` | Merge rows by key. |
| `Unused` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/BookRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="cobj-constructibleobject"></a>

## COBJ — Constructible Object

| Properties | How they are patched |
|---|---|
| `CreatedObject`, `WorkbenchKeyword`, `CreatedObjectCount` | Select each value separately. |
| `Items` | Merge rows by key. |
| `Conditions` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ConstructibleObjectRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="cont-container"></a>

## CONT — Container

Items match by item reference, retaining duplicate rows and their complete ownership/condition extra data (COED). Unchanged duplicate rows match first; remaining occurrences match by metadata-change cost. Newly different counts can forward independently; returning to the baseline count requires ownership permission. COED presence, condition and coherent owner changes require the row owner to be a declared or configured virtual master. Count edits do not grant COED permission in the same row. Absent COED stays distinct from a present default group. See the [completed fix and verification](KNOWN-ISSUES.md#container-item-extra-data).

| Properties | How they are patched |
|---|---|
| `Name`, `Weight`, `OpenSound`, `CloseSound` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Items`, `VirtualMachineAdapter` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ContainerRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="ingr-ingredient"></a>

## INGR — Ingredient

Effects merges by position. Each effect’s BaseEffect, Data and nested Conditions travel together.

| Properties | How they are patched |
|---|---|
| `Name`, `EquipType`, `PickUpSound`, `PutDownSound`, `Value`, `Weight`, `IngredientValue` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Icons`, `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `VirtualMachineAdapter`, `Keywords` | Merge rows by key. |
| `Effects` | Merge rows by position. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/IngredientRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="keym-key"></a>

## KEYM — Key

| Properties | How they are patched |
|---|---|
| `Name`, `PickUpSound`, `PutDownSound`, `Value`, `Weight` | Select each value separately. |

| `Icons`, `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `VirtualMachineAdapter`, `Keywords` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/KeyRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="lvli-leveleditem"></a>

## LVLI — Leveled Item

Entries match by (Level, Reference), with complete row payloads/counts. Entries are sorted on read/write and extra data is copied.

| Properties | How they are patched |
|---|---|
| `ChanceNone`, `Global` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ObjectBounds` | Select each whole value separately. |
| `Entries` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/LeveledItemRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="misc-miscitem"></a>

## MISC — Misc Item

| Properties | How they are patched |
|---|---|
| `Name`, `PickUpSound`, `PutDownSound`, `Value`, `Weight` | Select each value separately. |

| `Icons`, `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `VirtualMachineAdapter`, `Keywords` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/MiscItemRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="slgm-soulgem"></a>

## SLGM — Soul Gem

| Properties | How they are patched |
|---|---|
| `Name`, `PickUpSound`, `PutDownSound`, `Value`, `Weight`, `ContainedSoul`, `MaximumCapacity`, `LinkedTo` | Select each value separately. |

| `Icons`, `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Keywords` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/SoulGemRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="weap-weapon"></a>

## WEAP — Weapon

By default, a newly accepted single configured vanilla weapon-type keyword replaces the other configured weapon types. Multiple types explicitly supplied together are preserved; no type makes no exclusive choice. Other keywords merge normally.

| Properties | How they are patched |
|---|---|
| `Name`, `ObjectEffect`, `EnchantmentAmount`, `EquipmentType`, `BlockBashImpact`, `AlternateBlockMaterial`, `PickUpSound`, `PutDownSound`, `Description`, `ImpactDataSet`, `FirstPersonModel`, `AttackSound`, `AttackSound2D`, `AttackLoopSound`, `AttackFailSound`, `IdleSound`, `EquipSound`, `UnequipSound`, `BasicStats.Value`, `BasicStats.Weight`, `BasicStats.Damage`, `DetectionSoundLevel`, `Template`, `Data.AnimationType`, `Data.Speed`, `Data.Reach`, `Data.SightFOV`, `Data.BaseVATStoHitChance`, `Data.AttackAnimation`, `Data.NumProjectiles`, `Data.EmbeddedWeaponAV`, `Data.RangeMin`, `Data.RangeMax`, `Data.OnHit`, `Data.AnimationAttackMult`, `Data.RumbleLeftMotorStrength`, `Data.RumbleRightMotorStrength`, `Data.RumbleDuration`, `Data.Skill`, `Data.Resist`, `Data.Stagger`, `Critical.Damage`, `Critical.PercentMult`, `Critical.Effect` | Select each value separately. |
| `Data.Flags`, `Critical.Flags` | Merge registered flag bits separately. |
| `Icons`, `Destructible`, `ScopeModel` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Keywords`, `VirtualMachineAdapter` | Merge rows by key. |
| `Unused`, `Data.Unknown`, `Data.Unknown2`, `Data.Unknown3`, `Data.Unknown4`, `Data.Unknown5` | Not independently forwarded. |
| `Critical.Versioning`, `Critical.Unused`, `Critical.Unused2`, `Critical.Unused3`, `Critical.Unused4`, `Data.Unused`, `Data.Unused2` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/WeaponRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).
