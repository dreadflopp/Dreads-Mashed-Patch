# Quests, dialogue and AI

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="dial-dialogtopic"></a>

## DIAL — Dialog Topic

**Disabled by default; these rules apply when enabled.**

Responses are separately queried INFO records, not a merged DIAL child list.

| Properties | How they are patched |
|---|---|
| `Name`, `Priority`, `Branch`, `Quest`, `Category`, `Subtype`, `SubtypeName` | Select each value separately. |
| `TopicFlags` | Merge registered flag bits separately. |
| `Responses`, `Timestamp`, `Unknown` | Child/group surfaces; no independent property merge. |


<a id="dlbr-dialogbranch"></a>

## DLBR — Dialog Branch

**Disabled by default; these rules apply when enabled.**

| Properties | How they are patched |
|---|---|
| `Quest`, `Category`, `StartingTopic` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |


<a id="dlvw-dialogview"></a>

## DLVW — Dialog View

**Disabled by default; these rules apply when enabled.**

| Properties | How they are patched |
|---|---|
| `Quest` | Select each value separately. |
| `ENAM`, `DNAM` | Select each whole value separately. |
| `TNAMs` | Select each whole collection separately. |
| `Branches` | Merge aligned rows in order. |


<a id="idle-idleanimation"></a>

## IDLE — Idle Animation

The behavior filename and animation event select together. Conditions and legacy DATA fields remain independent; those fields cannot select a different filename/event pair. RelatedIdles is not independently forwarded.

| Properties | How they are patched |
|---|---|
| `LoopingSecondsMin`, `LoopingSecondsMax`, `AnimationGroupSection`, `ReplayDelay` | Select each value independently. |
| `Flags` | Merge registered flag bits separately. |
| `Conditions` | Merge aligned rows in order. |
| `RelatedIdles` | Not independently forwarded. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `IdleReference` (Animations) | `Filename`, `AnimationEvent` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="idlm-idlemarker"></a>

## IDLM — Idle Marker

The ordered idle-reference list, its selection flags and timer select together under
the animation rule. The marker mesh/bounds use Meshes independently. Flag bits
without a registered rule retain the winner’s state; an absent flags field is not
created by forwarding a marker mesh.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |
| `IdleSelection` (Animations) | `Animations`, `Flags`, `IdleTimer` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="info-dialogresponses"></a>

## INFO — Dialog Responses

**Disabled by default; these rules apply when enabled.**

Responses, Conditions and LinkTo align across versions. Responses also harmonize whitespace-only text changes. VirtualMachineAdapter includes fragments as a whole adapter. PreviousDialog is not independently forwarded.

| Properties | How they are patched |
|---|---|
| `ResetHours`, `Topic`, `FavorLevel`, `ResponseData`, `Prompt`, `Speaker`, `WalkAwayTopic`, `AudioOutputOverride` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `DATA` | Select each whole value separately. |
| `LinkTo`, `Responses`, `Conditions` | Merge aligned rows in order. |
| `UnknownData` | Not independently forwarded. |
| `PreviousDialog` | Child/group surfaces; no independent property merge. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="pack-package"></a>

## PACK — Package

**Disabled by default; these rules apply when enabled.**

The package template graph is inactive even when packages are enabled: PackageTemplate, DataInputVersion, Data, XnamMarker and ProcedureTree are not forwarded independently. The local writer and binary regression confirm key-sorted data rows instead of the source declaration order, while preserving index/value associations. This does not establish an in-game failure. Winner/priority snapshots still use that writer; disabling graph selection does not guarantee physical row-order preservation. VMAD, the idle selection and the three event structures are separate whole values. The idle selection and VMAD each forward normally as separate groups.

| Properties | How they are patched |
|---|---|
| `Type`, `InterruptOverride`, `PreferredSpeed`, `ScheduleMonth`, `ScheduleDate`, `ScheduleHour`, `ScheduleMinute`, `ScheduleDurationInMinutes`, `CombatStyle`, `OwnerQuest` | Select each value separately. |
| `Flags`, `InterruptFlags`, `ScheduleDayOfWeek` | Merge registered flag bits separately. |
| `OnBegin`, `OnEnd`, `OnChange` | Select each whole value separately. |
| `Conditions` | Merge aligned rows in order. |
| `Data`, `DataInputVersion`, `PackageTemplate`, `ProcedureTree`, `Unknown`, `Unknown2`, `Unknown3`, `Unknown4`, `XnamMarker` | Not independently forwarded. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
| `IdleSelection` (Animations) | `IdleAnimations` | Normal | Use the existing complete-value or collection policy. |


<a id="perk-perk"></a>

## PERK — Perk

VMAD forwards as one group. Gameplay-tree ownership still governs conditions, ranks, effects and links; they are separate from VMAD selection.

**Ownership reset:** a change to `VirtualMachineAdapter`, `Conditions`, `Trait`, `Level`, `NumRanks`, `Playable`, `Hidden`, `NextPerk`, `Effects` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Trait`, `Level`, `NumRanks`, `Playable`, `Hidden`, `NextPerk` | Select each value within the structural ownership baseline. |
| `Conditions` | Merge aligned rows within the structural ownership baseline. |
| `Effects` | Merge keyed effects within the structural ownership baseline. |
| `Name`, `Description` | Select each value separately. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `Icons` (IconsAndImages) | `Icons` | Normal | Use the existing complete-value or collection policy. |
| `ScriptBindings` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="qust-quest"></a>

## QUST — Quest

VMAD scripts, metadata and bindings forward together. Stages, objectives, aliases, conditions and events retain the existing structural ownership and graph validation rules.

Adapter presence gates its child fields. Structural changes are proposed and validated for alias IDs/references, stage/objective indices, fragment stage/log-entry references and supported object formats. Stages include log entries; objectives include targets and conditions; aliases are complete rows. The ownership reset below limits independent list merging. Application and validation run inside the shared record transaction; a failed candidate is discarded, and recorded errors reject the run before output publication.

**Ownership reset:** a change to `VirtualMachineAdapter.Presence`, `VirtualMachineAdapter.Version`, `VirtualMachineAdapter.ObjectFormat`, `VirtualMachineAdapter.Scripts`, `VirtualMachineAdapter.ExtraBindDataVersion`, `VirtualMachineAdapter.FileName`, `VirtualMachineAdapter.Fragments`, `VirtualMachineAdapter.Aliases`, `Type`, `Event`, `TextDisplayGlobals`, `DialogConditions`, `EventConditions`, `Stages`, `Objectives`, `NextAliasID`, `Aliases` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Type`, `Event`, `NextAliasID` | Select each value within the structural ownership baseline. |
| `Stages`, `Objectives` | Use each list's keyed row policy within the structural ownership baseline. |
| `TextDisplayGlobals`, `DialogConditions`, `EventConditions`, `Aliases` | Use each list's aligned row policy within the structural ownership baseline. |
| `Name`, `Priority`, `Filter`, `Description` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `QuestFormVersion`, `Unknown` | Not independently forwarded. |
| `VirtualMachineAdapter.Versioning` | Serialization/unused state; no independent decision. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `ScriptBindings` (Scripts) | `VirtualMachineAdapter.Presence`, `VirtualMachineAdapter.Version`, `VirtualMachineAdapter.ObjectFormat`, `VirtualMachineAdapter.Scripts`, `VirtualMachineAdapter.ExtraBindDataVersion`, `VirtualMachineAdapter.FileName`, `VirtualMachineAdapter.Fragments`, `VirtualMachineAdapter.Aliases` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="scen-scene"></a>

## SCEN — Scene

Phases, Actors and Actions retain their structural ownership rules separately from VMAD selection. Phase conditions and action package lists stay inside their rows. Adapter presence gates child writes; ScriptFragments is one complete structure. Selected phase/action copies clear unused payloads, which are ignored in comparison. Scripts, Version and ObjectFormat do not themselves trigger the broader ownership reset; they stay with VMAD bindings under the script rule.

**Ownership reset:** a change to `VirtualMachineAdapter.Presence`, `VirtualMachineAdapter.ScriptFragments`, `Phases`, `Actors`, `Actions`, `Quest`, `LastActionIndex` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Phases`, `Actors`, `Actions` | Select each whole structural sequence within the ownership baseline. |
| `Quest`, `LastActionIndex` | Select each value within the structural ownership baseline. |
| `Flags` | Merge registered flag bits separately. |
| `VNAM` | Select each whole value separately. |
| `Conditions` | Merge aligned rows in order. |
| `Unused`, `Unused2` | Not independently forwarded. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `ScriptBindings` (Scripts) | `VirtualMachineAdapter.Presence`, `VirtualMachineAdapter.ScriptFragments`, `VirtualMachineAdapter.Version`, `VirtualMachineAdapter.ObjectFormat`, `VirtualMachineAdapter.Scripts` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="smbn-storymanagerbranchnode"></a>

## SMBN — Story Manager Branch Node

**Ownership reset:** a change to `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `MaxConcurrentQuests` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Parent`, `PreviousSibling`, `MaxConcurrentQuests` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Conditions` | Merge aligned rows in order. |


<a id="smen-storymanagereventnode"></a>

## SMEN — Story Manager Event Node

**Ownership reset:** a change to `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `MaxConcurrentQuests`, `Type` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Parent`, `PreviousSibling`, `MaxConcurrentQuests`, `Type` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Conditions` | Merge aligned rows in order. |


<a id="smqn-storymanagerquestnode"></a>

## SMQN — Story Manager Quest Node

Quests is not an ownership-reset trigger: aligned quest rows may merge while the node configuration stays stable.

**Ownership reset:** a change to `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `QuestFlags`, `MaxConcurrentQuests`, `MaxNumQuestsToRun` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Parent`, `PreviousSibling`, `MaxConcurrentQuests`, `MaxNumQuestsToRun` | Select each value separately. |
| `Flags`, `QuestFlags` | Merge registered flag bits separately. |
| `Conditions`, `Quests` | Merge aligned rows in order. |
