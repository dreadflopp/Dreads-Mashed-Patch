# Settings, lists and other records

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header flag rules apply throughout and are not repeated. Record eligibility, Priority Mods and output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="aact-actionrecord"></a>

## AACT — Action Record

| Properties | How they are patched |
|---|---|
| `Color` | Select each value separately. |

<a id="coll-collisionlayer"></a>

## COLL — Collision Layer

| Properties | How they are patched |
|---|---|
| `Description`, `Index`, `DebugColor`, `Name` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `CollidesWith` | Merge rows by key. |

<a id="dual-dualcastdata"></a>

## DUAL — Dual Cast Data

| Properties | How they are patched |
|---|---|
| `Projectile`, `Explosion`, `EffectShader`, `HitEffectArt`, `ImpactDataSet`, `InheritScale` | Select each value separately. |
| `ObjectBounds` | Keep the effective winner's bounds; no independent forwarding. |

<a id="flst-formlist"></a>

## FLST — Form List

Items align across versions so additions and removals can be reconciled while retaining meaningful order.

| Properties | How they are patched |
|---|---|
| `Items` | Merge aligned rows in order. |

<a id="glob-globalfloat"></a>

## GLOB — Global Float

The [value-type rules](README.md#how-a-value-is-selected) apply to all GLOB variants below: the effective winner determines the type, and the latest eligible type change starts a new baseline. Earlier types are not converted or merged back in. Known variants have a fixed `TypeChar`; the Unknown variant selects `TypeChar` explicitly.

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

<a id="glob-globalint"></a>

## GLOB — Global Int

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

<a id="glob-globalshort"></a>

## GLOB — Global Short

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

<a id="glob-globalunknown"></a>

## GLOB — Global Unknown

| Properties | How they are patched |
|---|---|
| `TypeChar`, `Data` | Select each value separately. |

<a id="gmst-gamesettingbool"></a>

## GMST — Game Setting Bool

All GMST variants below follow the same value-type rules as GLOB. Priority Mods can select a complete record of another type. A preserved official `EditorID` is used only when its prefix matches the selected value type.

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

<a id="gmst-gamesettingfloat"></a>

## GMST — Game Setting Float

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

<a id="gmst-gamesettingint"></a>

## GMST — Game Setting Int

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

<a id="gmst-gamesettingstring"></a>

## GMST — Game Setting String

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

<a id="kywd-keyword"></a>

## KYWD — Keyword

| Properties | How they are patched |
|---|---|
| `Color` | Select each value separately. |

<a id="lscr-loadscreen"></a>

## LSCR — Load Screen

| Properties | How they are patched |
|---|---|
| `Description` | Select each value separately. |
| `Conditions` | Merge aligned rows in order. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `DisplayModel` (Meshes) | `LoadingScreenNif`, `InitialScale`, `InitialRotation`, `RotationOffsetConstraints`, `InitialTranslationOffset`, `CameraPath` | Normal | Select one complete source snapshot through the shared change/reversion rules. |

<a id="mesg-message"></a>

## MESG — Message

| Properties | How they are patched |
|---|---|
| `Description`, `Name`, `Quest`, `DisplayTime` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `INAM` | Select the complete binary payload. |
| `MenuButtons` | Merge aligned rows in order. |
