# Items and equipment

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="alch-ingestible"></a>

## ALCH — Ingestible

Effects merges by position. Each effect’s BaseEffect, Data and nested Conditions travel together.

| Properties | How they are patched |
|---|---|
| `Name`, `Description`, `PickUpSound`, `PutDownSound`, `EquipmentType`, `Weight`, `Value`, `Addiction`, `AddictionChance`, `ConsumeSound` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Keywords` | Merge rows by key. |
| `Effects` | Merge rows by position. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="ammo-ammunition"></a>

## AMMO — Ammunition

| Properties | How they are patched |
|---|---|
| `Name`, `PickUpSound`, `PutDownSound`, `Description`, `Projectile`, `Damage`, `Value`, `Weight`, `ShortName` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Keywords` | Merge rows by key. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="appa-alchemicalapparatus"></a>

## APPA — Alchemical Apparatus

| Properties | How they are patched |
|---|---|
| `Name`, `PickUpSound`, `PutDownSound`, `Quality`, `Description`, `Value`, `Weight` | Select each value separately. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="arma-armoraddon"></a>

## ARMA — Armor Addon

Each gender’s first/third-person meshes and alternate textures stay with its shared weight-slider switch under Meshes. Skin/swap references form a separate Textures group per gender. Body-template fields and supported races use ordinary forwarding. Selected filenames carry model data bytes, but byte-only differences do not claim ownership. BodyTemplate.ActsLike44 is not independently patched.

| Properties | How they are patched |
|---|---|
| `Priority.Male`, `Priority.Female`, `DetectionSoundValue`, `WeaponAdjust`, `FootstepSound`, `ArtObject` | Select each value separately. |
| `Unknown`, `Unknown2`, `WorldModel.Male.Data`, `WorldModel.Female.Data`, `FirstPersonModel.Male.Data`, `FirstPersonModel.Female.Data` | Not independently forwarded. |
| `BodyTemplate.FirstPersonFlags`, `BodyTemplate.Flags` | Merge registered flag bits separately. |
| `BodyTemplate.ArmorType`, `Race` | Select each value independently. |
| `AdditionalRaces` | Merge rows by key. |
| `BodyTemplate.ActsLike44` | Serialization/unused state; no independent decision. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `MaleMeshes` (Meshes) | `WorldModel.Male.File`, `WorldModel.Male.AlternateTextures`, `FirstPersonModel.Male.File`, `FirstPersonModel.Male.AlternateTextures`, `WeightSliderEnabled.Male` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `MaleSkinTextures` (Textures) | `SkinTexture.Male`, `TextureSwapList.Male` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `FemaleMeshes` (Meshes) | `WorldModel.Female.File`, `WorldModel.Female.AlternateTextures`, `FirstPersonModel.Female.File`, `FirstPersonModel.Female.AlternateTextures`, `WeightSliderEnabled.Female` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `FemaleSkinTextures` (Textures) | `SkinTexture.Female`, `TextureSwapList.Female` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="armo-armor"></a>

## ARMO — Armor

Dropped/world meshes follow the model/bounds rule under Meshes. Armature references and the complete biped slot mask (`BodyTemplate.FirstPersonFlags`) share one Meshes decision, so slots cannot be combined with another source's addon list. Male/female inventory icons use IconsAndImages independently. Armor type, general BodyTemplate flags and Race use ordinary forwarding. ActsLike44 is not independently patched.

| Properties | How they are patched |
|---|---|
| `Name`, `ObjectEffect`, `EnchantmentAmount`, `PickUpSound`, `PutDownSound`, `RagdollConstraintTemplate`, `EquipmentType`, `BashImpactDataSet`, `AlternateBlockMaterial`, `Description`, `Value`, `Weight`, `ArmorRating`, `TemplateArmor` | Select each value separately. |
| `Keywords` | Merge rows by key. |
| `BodyTemplate.Flags` | Merge registered flag bits separately. |
| `BodyTemplate.ArmorType`, `Race` | Select each value independently. |
| `BodyTemplate.ActsLike44` | Serialization/unused state; no independent decision. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `WorldModelAndBounds` (Meshes) | `WorldModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |
| `MaleIcons` (IconsAndImages) | `WorldModel.Male.Icons` | Normal | Use the existing complete-value or collection policy. |
| `FemaleIcons` (IconsAndImages) | `WorldModel.Female.Icons` | Normal | Use the existing complete-value or collection policy. |
| `WornMeshes` (Meshes) | `Armature`, `BodyTemplate.FirstPersonFlags` | Normal | Select the complete addon list and slot mask from one source through the shared change/reversion rules. A slot-only change also selects the complete pair. |


<a id="book-book"></a>

## BOOK — Book

BookText forwards normally as complete translated text, including embedded image markup. Icons forwards separately; it does not rewrite or freeze book text.

Teaches keeps the teaching variant and its value together. It does not combine skill and spell teaching.

| Properties | How they are patched |
|---|---|
| `BookText` | Select complete translated text, including embedded image markup. |
| `Name`, `Value`, `Weight`, `Description`, `PickUpSound`, `PutDownSound`, `Type`, `InventoryArt` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Teaches` | Select each whole value separately. |
| `Keywords` | Merge rows by key. |
| `Unused` | Not independently forwarded. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="cobj-constructibleobject"></a>

## COBJ — Constructible Object

| Properties | How they are patched |
|---|---|
| `CreatedObject`, `WorkbenchKeyword`, `CreatedObjectCount` | Select each value separately. |
| `Items` | Merge rows by key. |
| `Conditions` | Merge aligned rows in order. |


<a id="cont-container"></a>

## CONT — Container

Items match by item reference, retaining duplicate rows and their complete ownership/condition extra data (COED). Unchanged duplicate rows match first; remaining occurrences match by metadata-change cost. Newly different counts can forward independently; returning to the baseline count requires ownership permission. COED presence, condition and coherent owner changes require the row owner to be a declared or configured virtual master. Count edits do not grant COED permission in the same row. Absent COED stays distinct from a present default group.

| Properties | How they are patched |
|---|---|
| `Name`, `Weight`, `OpenSound`, `CloseSound` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Items` | Merge rows by key. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="ingr-ingredient"></a>

## INGR — Ingredient

Effects merges by position. Each effect’s BaseEffect, Data and nested Conditions travel together.

| Properties | How they are patched |
|---|---|
| `Name`, `EquipType`, `PickUpSound`, `PutDownSound`, `Value`, `Weight`, `IngredientValue` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Keywords` | Merge rows by key. |
| `Effects` | Merge rows by position. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="keym-key"></a>

## KEYM — Key

| Properties | How they are patched |
|---|---|
| `Name`, `PickUpSound`, `PutDownSound`, `Value`, `Weight` | Select each value separately. |
| `Keywords` | Merge rows by key. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="lvli-leveleditem"></a>

## LVLI — Leveled Item

Entries match by (Level, Reference), with complete row payloads/counts. Entries are sorted on read/write and extra data is copied.

| Properties | How they are patched |
|---|---|
| `ChanceNone`, `Global` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ObjectBounds` | Keep the effective winner's bounds; no independent forwarding. |
| `Entries` | Merge rows by key. |


<a id="misc-miscitem"></a>

## MISC — Misc Item

| Properties | How they are patched |
|---|---|
| `Name`, `PickUpSound`, `PutDownSound`, `Value`, `Weight` | Select each value separately. |
| `Keywords` | Merge rows by key. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="slgm-soulgem"></a>

## SLGM — Soul Gem

| Properties | How they are patched |
|---|---|
| `Name`, `PickUpSound`, `PutDownSound`, `Value`, `Weight`, `ContainedSoul`, `MaximumCapacity`, `LinkedTo` | Select each value separately. |
| `Keywords` | Merge rows by key. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="weap-weapon"></a>

## WEAP — Weapon

By default, a newly accepted single configured vanilla weapon-type keyword replaces the other configured weapon types. Multiple types explicitly supplied together are preserved; no type makes no exclusive choice. Other keywords merge normally.

| Properties | How they are patched |
|---|---|
| `Name`, `ObjectEffect`, `EnchantmentAmount`, `EquipmentType`, `BlockBashImpact`, `AlternateBlockMaterial`, `PickUpSound`, `PutDownSound`, `Description`, `ImpactDataSet`, `AttackSound`, `AttackSound2D`, `AttackLoopSound`, `AttackFailSound`, `IdleSound`, `EquipSound`, `UnequipSound`, `BasicStats.Value`, `BasicStats.Weight`, `BasicStats.Damage`, `DetectionSoundLevel`, `Template`, `Data.AnimationType`, `Data.Speed`, `Data.Reach`, `Data.BaseVATStoHitChance`, `Data.AttackAnimation`, `Data.NumProjectiles`, `Data.EmbeddedWeaponAV`, `Data.RangeMin`, `Data.RangeMax`, `Data.OnHit`, `Data.AnimationAttackMult`, `Data.RumbleLeftMotorStrength`, `Data.RumbleRightMotorStrength`, `Data.RumbleDuration`, `Data.Skill`, `Data.Resist`, `Data.Stagger`, `Critical.Damage`, `Critical.PercentMult`, `Critical.Effect` | Select each value separately. |
| `Data.Flags`, `Critical.Flags` | Merge registered flag bits separately. |
| `Keywords` | Merge rows by key. |
| `Unused`, `Data.Unknown`, `Data.Unknown2`, `Data.Unknown3`, `Data.Unknown4`, `Data.Unknown5` | Not independently forwarded. |
| `Critical.Versioning`, `Critical.Unused`, `Critical.Unused2`, `Critical.Unused3`, `Critical.Unused4`, `Data.Unused`, `Data.Unused2` | Serialization/unused state; no independent decision. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |
| `Scope` (Meshes) | `ScopeModel`, `Data.SightFOV`, `Data.Flags.HasScope` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `FirstPersonGeometry` (Meshes) | `FirstPersonModel` | Normal | Use the existing complete-value or collection policy. |

`Data.Flags` merges the remaining registered bits; HasScope is owned only by the Scope group. Main weapon geometry, scope and first-person references have separate groups.
