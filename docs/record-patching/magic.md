# Magic and projectiles

[Patching guide](README.md) · [Known issues](KNOWN-ISSUES.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="ench-objecteffect"></a>

## ENCH — Object Effect

Effects merges by position. Each effect’s BaseEffect, Data and nested Conditions travel together.

| Properties | How they are patched |
|---|---|
| `Name`, `EnchantmentCost`, `CastType`, `EnchantmentAmount`, `TargetType`, `EnchantType`, `ChargeTime`, `BaseEnchantment`, `WornRestrictions` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ObjectBounds` | Select each whole value separately. |
| `Effects` | Merge rows by position. |
| `ENITDataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ObjectEffectRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="expl-explosion"></a>

## EXPL — Explosion

| Properties | How they are patched |
|---|---|
| `Name`, `ObjectEffect`, `ImageSpaceModifier`, `Light`, `Sound1`, `Sound2`, `ImpactDataSet`, `PlacedObject`, `SpawnProjectile`, `Force`, `Damage`, `Radius`, `ISRadius`, `VerticalOffsetMult`, `SoundLevel` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `VirtualMachineAdapter` | Merge rows by key. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ExplosionRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="hazd-hazard"></a>

## HAZD — Hazard

| Properties | How they are patched |
|---|---|
| `Name`, `ImageSpaceModifier`, `Limit`, `Radius`, `Lifetime`, `ImageSpaceRadius`, `TargetInterval`, `Spell`, `Light`, `ImpactDataSet`, `Sound` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/HazardRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="lvsp-leveledspell"></a>

## LVSP — Leveled Spell

Entries match by (Level, Reference), with complete row payloads/counts.

| Properties | How they are patched |
|---|---|
| `ChanceNone` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ObjectBounds` | Select each whole value separately. |
| `Entries` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/LeveledSpellRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="mgef-magiceffect"></a>

## MGEF — Magic Effect

Archetype copies the selected subtype whole. Type, ActorValue and AssociationKey determine whether it changed; these cover the current Mutagen 0.54.4 subtype surfaces, whose typed Association is represented by AssociationKey. No additional semantic subtype fields were found in the [known-issue re-evaluation](KNOWN-ISSUES.md#re-evaluation-and-verification).

| Properties | How they are patched |
|---|---|
| `Name`, `Description`, `BaseCost`, `CastType`, `TargetType`, `MagicSkill`, `ResistValue`, `SecondActorValue`, `CastingSoundLevel`, `MenuDisplayObject`, `CastingLight`, `HitShader`, `EnchantShader`, `Projectile`, `Explosion`, `CastingArt`, `HitEffectArt`, `ImpactData`, `DualCastArt`, `EnchantArt`, `HitVisuals`, `EnchantVisuals`, `EquipAbility`, `ImageSpaceModifier`, `PerkToApply`, `TaperWeight`, `MinimumSkillLevel`, `SpellmakingArea`, `SpellmakingCastingTime`, `TaperCurve`, `TaperDuration`, `SecondActorValueWeight`, `SkillUsageMultiplier`, `DualCastScale`, `ScriptEffectAIScore`, `ScriptEffectAIDelayTime` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Archetype` | Select each whole value separately. |
| `VirtualMachineAdapter`, `Keywords`, `CounterEffects`, `Sounds` | Merge rows by key. |
| `Conditions` | Merge aligned rows in order. |
| `Unknown1` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/MagicEffectRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="proj-projectile"></a>

## PROJ — Projectile

Trajectory groups Type, Gravity, Speed, Range, ConeSpread, Lifetime and RelaunchInterval. ExplosionBehavior groups the Explosion/AltTrigger bits, Explosion, proximity, timer and CountdownSound. MuzzleFlashBehavior groups its bit, link, duration, model and texture hashes. PickupBehavior groups CanBePickedUp with DefaultWeaponSource; DisableBehavior groups CanBeDisabled with DisaleSound. Collision groups radius and layer. The separate Flags field owns only Hitscan, Supersonic, PinsLimbs, PassThroughSmallTransparent, DisableCombatAimCorrection and Rotation.

| Properties | How they are patched |
|---|---|
| `Name`, `Light`, `TracerChance`, `Sound`, `FadeDuration`, `ImpactForce`, `DecalData`, `SoundLevel` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Destructible`, `Trajectory`, `MuzzleFlashBehavior`, `ExplosionBehavior`, `PickupBehavior`, `DisableBehavior`, `Collision` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ProjectileRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="scrl-scroll"></a>

## SCRL — Scroll

Effects merges by position. Each effect’s BaseEffect, Data and nested Conditions travel together.

| Properties | How they are patched |
|---|---|
| `Name`, `MenuDisplayObject`, `EquipmentType`, `Description`, `PickUpSound`, `PutDownSound`, `Value`, `Weight`, `BaseCost`, `Type`, `ChargeTime`, `CastType`, `TargetType`, `CastDuration`, `Range`, `HalfCostPerk` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Keywords` | Merge rows by key. |
| `Effects` | Merge rows by position. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ScrollRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="shou-shout"></a>

## SHOU — Shout

| Properties | How they are patched |
|---|---|
| `Name`, `MenuDisplayObject`, `Description` | Select each value separately. |

| `WordsOfPower` | Select each whole collection separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ShoutRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="spel-spell"></a>

## SPEL — Spell

Effects merges by position. Each effect’s BaseEffect, Data and nested Conditions travel together.

| Properties | How they are patched |
|---|---|
| `Name`, `MenuDisplayObject`, `Description`, `EquipmentType`, `BaseCost`, `Type`, `ChargeTime`, `CastType`, `TargetType`, `CastDuration`, `Range`, `HalfCostPerk` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ObjectBounds` | Select each whole value separately. |
| `Keywords` | Merge rows by key. |
| `Effects` | Merge rows by position. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/SpellRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="woop-wordofpower"></a>

## WOOP — Word Of Power

| Properties | How they are patched |
|---|---|
| `Name`, `Translation` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/WordOfPowerRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).
