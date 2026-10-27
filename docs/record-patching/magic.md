# Magic and projectiles

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="ench-objecteffect"></a>

## ENCH — Object Effect

Effects merges by position. Each effect’s BaseEffect, Data and nested Conditions travel together.

| Properties | How they are patched |
|---|---|
| `Name`, `EnchantmentCost`, `CastType`, `EnchantmentAmount`, `TargetType`, `EnchantType`, `ChargeTime`, `BaseEnchantment`, `WornRestrictions` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ObjectBounds` | Keep the effective winner's bounds; no independent forwarding. |
| `Effects` | Merge rows by position. |
| `ENITDataTypeState` | Serialization/unused state; no independent decision. |


<a id="expl-explosion"></a>

## EXPL — Explosion

| Properties | How they are patched |
|---|---|
| `Name`, `ObjectEffect`, `ImageSpaceModifier`, `Light`, `Sound1`, `Sound2`, `ImpactDataSet`, `PlacedObject`, `SpawnProjectile`, `Force`, `Damage`, `Radius`, `ISRadius`, `VerticalOffsetMult`, `SoundLevel` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="hazd-hazard"></a>

## HAZD — Hazard

| Properties | How they are patched |
|---|---|
| `Name`, `ImageSpaceModifier`, `Limit`, `Radius`, `Lifetime`, `ImageSpaceRadius`, `TargetInterval`, `Spell`, `Light`, `ImpactDataSet`, `Sound` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="lvsp-leveledspell"></a>

## LVSP — Leveled Spell

Entries match by (Level, Reference), with complete row payloads/counts.

| Properties | How they are patched |
|---|---|
| `ChanceNone` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ObjectBounds` | Keep the effective winner's bounds; no independent forwarding. |
| `Entries` | Merge rows by key. |


<a id="mgef-magiceffect"></a>

## MGEF — Magic Effect

Archetype copies the selected subtype whole. Type, ActorValue and AssociationKey determine whether it changed; these cover the current Mutagen 0.54.4 subtype surfaces, whose typed Association is represented by AssociationKey.

| Properties | How they are patched |
|---|---|
| `Name`, `Description`, `BaseCost`, `CastType`, `TargetType`, `MagicSkill`, `ResistValue`, `SecondActorValue`, `CastingSoundLevel`, `MenuDisplayObject`, `CastingLight`, `HitShader`, `EnchantShader`, `Projectile`, `Explosion`, `CastingArt`, `HitEffectArt`, `ImpactData`, `DualCastArt`, `EnchantArt`, `HitVisuals`, `EnchantVisuals`, `EquipAbility`, `ImageSpaceModifier`, `PerkToApply`, `TaperWeight`, `MinimumSkillLevel`, `SpellmakingArea`, `SpellmakingCastingTime`, `TaperCurve`, `TaperDuration`, `SecondActorValueWeight`, `SkillUsageMultiplier`, `DualCastScale`, `ScriptEffectAIScore`, `ScriptEffectAIDelayTime` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Archetype` | Select each whole value separately. |
| `Keywords`, `CounterEffects`, `Sounds` | Merge rows by key. |
| `Conditions` | Merge aligned rows in order. |
| `Unknown1` | Not independently forwarded. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="proj-projectile"></a>

## PROJ — Projectile

Trajectory groups Type, Gravity, Speed, Range, ConeSpread, Lifetime and RelaunchInterval. ExplosionBehavior groups the Explosion/AltTrigger bits, Explosion, proximity, timer and CountdownSound. MuzzleFlashBehavior groups its bit, link, duration, model and texture hashes. PickupBehavior groups CanBePickedUp with DefaultWeaponSource; DisableBehavior groups CanBeDisabled with DisaleSound. Collision groups radius and layer. The separate Flags field owns only Hitscan, Supersonic, PinsLimbs, PassThroughSmallTransparent, DisableCombatAimCorrection and Rotation.

| Properties | How they are patched |
|---|---|
| `Name`, `Light`, `TracerChance`, `Sound`, `FadeDuration`, `ImpactForce`, `DecalData`, `SoundLevel` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Trajectory`, `ExplosionBehavior`, `PickupBehavior`, `DisableBehavior`, `Collision` | Select each whole value separately. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |
| `MuzzleFlashBehavior` (Meshes) | `MuzzleFlashBehavior` | Normal | Use the existing complete-value or collection policy. |


<a id="scrl-scroll"></a>

## SCRL — Scroll

Effects merges by position. Each effect’s BaseEffect, Data and nested Conditions travel together.

| Properties | How they are patched |
|---|---|
| `Name`, `MenuDisplayObject`, `EquipmentType`, `Description`, `PickUpSound`, `PutDownSound`, `Value`, `Weight`, `BaseCost`, `Type`, `ChargeTime`, `CastType`, `TargetType`, `CastDuration`, `Range`, `HalfCostPerk` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Keywords` | Merge rows by key. |
| `Effects` | Merge rows by position. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="shou-shout"></a>

## SHOU — Shout

| Properties | How they are patched |
|---|---|
| `Name`, `MenuDisplayObject`, `Description` | Select each value separately. |
| `WordsOfPower` | Select each whole collection separately. |


<a id="spel-spell"></a>

## SPEL — Spell

Effects merges by position. Each effect’s BaseEffect, Data and nested Conditions travel together.

| Properties | How they are patched |
|---|---|
| `Name`, `MenuDisplayObject`, `Description`, `EquipmentType`, `BaseCost`, `Type`, `ChargeTime`, `CastType`, `TargetType`, `CastDuration`, `Range`, `HalfCostPerk` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ObjectBounds` | Keep the effective winner's bounds; no independent forwarding. |
| `Keywords` | Merge rows by key. |
| `Effects` | Merge rows by position. |


<a id="woop-wordofpower"></a>

## WOOP — Word Of Power

| Properties | How they are patched |
|---|---|
| `Name`, `Translation` | Select each value separately. |
