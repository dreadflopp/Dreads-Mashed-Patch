# Settings, lists and other records

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="aact-actionrecord"></a>

## AACT — Action Record

| Properties | How they are patched |
|---|---|
| `Color` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ActionRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="coll-collisionlayer"></a>

## COLL — Collision Layer

| Properties | How they are patched |
|---|---|
| `Description`, `Index`, `DebugColor`, `Name` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `CollidesWith` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/CollisionLayerRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="dual-dualcastdata"></a>

## DUAL — Dual Cast Data

| Properties | How they are patched |
|---|---|
| `Projectile`, `Explosion`, `EffectShader`, `HitEffectArt`, `ImpactDataSet`, `InheritScale` | Select each value separately. |
| `ObjectBounds` | Select each whole value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/DualCastDataRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="flst-formlist"></a>

## FLST — Form List

This route uses FormIdRecordHandler despite the record’s FormList name. Items align across versions.

| Properties | How they are patched |
|---|---|
| `Items` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/FormIdRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="glob-globalfloat"></a>

## GLOB — Global Float

Discovered through the global-record query and narrowed to the concrete value type. Known variants have a fixed TypeChar; Unknown patches TypeChar explicitly. There is no cross-type conversion.

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/GlobalFloatRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="glob-globalint"></a>

## GLOB — Global Int

Discovered through the global-record query and narrowed to the concrete value type. Known variants have a fixed TypeChar; Unknown patches TypeChar explicitly. There is no cross-type conversion.

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/GlobalIntRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="glob-globalshort"></a>

## GLOB — Global Short

Discovered through the global-record query and narrowed to the concrete value type. Known variants have a fixed TypeChar; Unknown patches TypeChar explicitly. There is no cross-type conversion.

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/GlobalShortRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="glob-globalunknown"></a>

## GLOB — Global Unknown

Discovered through the global-record query and narrowed to the concrete value type. Known variants have a fixed TypeChar; Unknown patches TypeChar explicitly. There is no cross-type conversion.

| Properties | How they are patched |
|---|---|
| `TypeChar`, `Data` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/GlobalUnknownRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="gmst-gamesettingbool"></a>

## GMST — Game Setting Bool

Discovered through the game-setting query and narrowed to the concrete value type. There is no cross-type conversion.

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/GameSettingBoolRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="gmst-gamesettingfloat"></a>

## GMST — Game Setting Float

Discovered through the game-setting query and narrowed to the concrete value type. There is no cross-type conversion.

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/GameSettingFloatRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="gmst-gamesettingint"></a>

## GMST — Game Setting Int

Discovered through the game-setting query and narrowed to the concrete value type. There is no cross-type conversion.

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/GameSettingIntRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="gmst-gamesettingstring"></a>

## GMST — Game Setting String

Discovered through the game-setting query and narrowed to the concrete value type. There is no cross-type conversion.

| Properties | How they are patched |
|---|---|
| `Data` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/GameSettingStringRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="kywd-keyword"></a>

## KYWD — Keyword

| Properties | How they are patched |
|---|---|
| `Color` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/KeywordRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="lscr-loadscreen"></a>

## LSCR — Load Screen

| Properties | How they are patched |
|---|---|
| `Description`, `LoadingScreenNif`, `InitialScale`, `InitialRotation`, `InitialTranslationOffset`, `CameraPath` | Select each value separately. |
| `Icons`, `RotationOffsetConstraints` | Select each whole value separately. |
| `Conditions` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/LoadScreenRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="mesg-message"></a>

## MESG — Message

| Properties | How they are patched |
|---|---|
| `Description`, `Name`, `Quest`, `DisplayTime` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `INAM` | Select each whole value separately. |
| `MenuButtons` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/MessageRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).
