# Objects and interaction

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="acti-activator"></a>

## ACTI — Activator

| Properties | How they are patched |
|---|---|
| `Name`, `MarkerColor`, `LoopingSound`, `ActivationSound`, `WaterType`, `ActivateTextOverride`, `InteractionKeyword` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Keywords` | Merge rows by key. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="anio-animatedobject"></a>

## ANIO — Animated Object

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `AnimatedModel` (Meshes) | `Model`, `UnloadEvent` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="door-door"></a>

## DOOR — Door

| Properties | How they are patched |
|---|---|
| `Name`, `OpenSound`, `CloseSound`, `LoopSound` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="flor-flora"></a>

## FLOR — Flora

| Properties | How they are patched |
|---|---|
| `Name`, `ActivateTextOverride`, `Ingredient`, `HarvestSound` | Select each value separately. |
| `PNAM`, `FNAM`, `Production` | Select each whole value separately. |
| `Keywords` | Merge rows by key. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="furn-furniture"></a>

## FURN — Furniture

| Properties | How they are patched |
|---|---|
| `Name`, `InteractionKeyword`, `AssociatedSpell` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `PNAM`, `WorkbenchData` | Select each whole value separately. |
| `Markers` | Select each whole collection separately. |
| `Keywords` | Merge rows by key. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |
| `ModelFilename` (Meshes) | `ModelFilename` | Normal | Use the existing complete-value or collection policy. |


<a id="ligh-light"></a>

## LIGH — Light

| Properties | How they are patched |
|---|---|
| `Name`, `Time`, `Radius`, `Color`, `FalloffExponent`, `FOV`, `NearClip`, `FlickerPeriod`, `FlickerIntensityAmplitude`, `FlickerMovementAmplitude`, `Value`, `Weight`, `FadeValue`, `Sound`, `Lens` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="mstt-moveablestatic"></a>

## MSTT — Moveable Static

| Properties | How they are patched |
|---|---|
| `Name`, `LoopingSound` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="stat-static"></a>

## STAT — Static

| Properties | How they are patched |
|---|---|
| `MaxAngle`, `Material` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Unused` | Not independently forwarded. |
| `DNAMDataTypeState` | Serialization/unused state; no independent decision. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `Geometry` (Meshes) | `ModelAndBounds`, `Lod` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="tact-talkingactivator"></a>

## TACT — Talking Activator

| Properties | How they are patched |
|---|---|
| `Name`, `LoopingSound`, `Voice` | Select each value separately. |
| `Keywords` | Merge rows by key. |
| `FNAM`, `PNAM` | Not independently forwarded. |

Destruction health, stage structure, flags, explosions and debris retain one ordinary forwarding decision. The stage-mesh group includes only stage models and their model metadata; it cannot restore removed stages.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `DestructionMeshes` (Meshes) | `Destructible.StageModels` | Normal | Select stage models only; preserve the selected stage structure and gameplay data. |
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |
