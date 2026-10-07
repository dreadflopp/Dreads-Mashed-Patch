# Objects and interaction

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="acti-activator"></a>

## ACTI — Activator

| Properties | How they are patched |
|---|---|
| `Name`, `MarkerColor`, `LoopingSound`, `ActivationSound`, `WaterType`, `ActivateTextOverride`, `InteractionKeyword` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `VirtualMachineAdapter`, `Keywords` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ActivatorRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="anio-animatedobject"></a>

## ANIO — Animated Object

| Properties | How they are patched |
|---|---|
| `UnloadEvent` | Select each value separately. |
| `Model` | Select each whole value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/AnimatedObjectRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="door-door"></a>

## DOOR — Door

| Properties | How they are patched |
|---|---|
| `Name`, `OpenSound`, `CloseSound`, `LoopSound` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `VirtualMachineAdapter` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/DoorRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="flor-flora"></a>

## FLOR — Flora

| Properties | How they are patched |
|---|---|
| `Name`, `ActivateTextOverride`, `Ingredient`, `HarvestSound` | Select each value separately. |
| `Destructible`, `PNAM`, `FNAM`, `Production` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `VirtualMachineAdapter`, `Keywords` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/FloraRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="furn-furniture"></a>

## FURN — Furniture

| Properties | How they are patched |
|---|---|
| `Name`, `InteractionKeyword`, `AssociatedSpell`, `ModelFilename` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Destructible`, `PNAM`, `WorkbenchData` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Markers` | Select each whole collection separately. |
| `VirtualMachineAdapter`, `Keywords` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/FurnitureRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="ligh-light"></a>

## LIGH — Light

| Properties | How they are patched |
|---|---|
| `Name`, `Time`, `Radius`, `Color`, `FalloffExponent`, `FOV`, `NearClip`, `FlickerPeriod`, `FlickerIntensityAmplitude`, `FlickerMovementAmplitude`, `Value`, `Weight`, `FadeValue`, `Sound`, `Lens` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Icons`, `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `VirtualMachineAdapter` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/LightRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="mstt-moveablestatic"></a>

## MSTT — Moveable Static

| Properties | How they are patched |
|---|---|
| `Name`, `LoopingSound` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/MoveableStaticRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="stat-static"></a>

## STAT — Static

| Properties | How they are patched |
|---|---|
| `MaxAngle`, `Material` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Lod` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Unused` | Not independently forwarded. |
| `DNAMDataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/StaticRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="tact-talkingactivator"></a>

## TACT — Talking Activator

| Properties | How they are patched |
|---|---|
| `Name`, `LoopingSound`, `Voice` | Select each value separately. |
| `Destructible` | Select each whole value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `VirtualMachineAdapter`, `Keywords` | Merge rows by key. |
| `FNAM`, `PNAM` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/TalkingActivatorRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).
