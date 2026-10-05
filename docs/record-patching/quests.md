# Quests, dialogue and AI

[Patching guide](README.md) · [Known issues](KNOWN-ISSUES.md)

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

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/DialogTopicRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="dlbr-dialogbranch"></a>

## DLBR — Dialog Branch

**Disabled by default; these rules apply when enabled.**

| Properties | How they are patched |
|---|---|
| `Quest`, `Category`, `StartingTopic` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/DialogBranchRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="dlvw-dialogview"></a>

## DLVW — Dialog View

**Disabled by default; these rules apply when enabled.**

| Properties | How they are patched |
|---|---|
| `Quest` | Select each value separately. |
| `ENAM`, `DNAM` | Select each whole value separately. |
| `TNAMs` | Select each whole collection separately. |
| `Branches` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/DialogViewRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="idle-idleanimation"></a>

## IDLE — Idle Animation

| Properties | How they are patched |
|---|---|
| `Filename`, `AnimationEvent`, `LoopingSecondsMin`, `LoopingSecondsMax`, `AnimationGroupSection`, `ReplayDelay` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Conditions` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/IdleAnimationRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="idlm-idlemarker"></a>

## IDLM — Idle Marker

| Properties | How they are patched |
|---|---|
| `IdleTimer` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Animations` | Select each whole collection separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/IdleMarkerRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="info-dialogresponses"></a>

## INFO — Dialog Responses

**Disabled by default; these rules apply when enabled.**

Responses, Conditions and LinkTo align across versions. Responses also harmonize whitespace-only text changes. VirtualMachineAdapter includes fragments as a whole adapter. PreviousDialog is not independently forwarded.

| Properties | How they are patched |
|---|---|
| `ResetHours`, `Topic`, `FavorLevel`, `ResponseData`, `Prompt`, `Speaker`, `WalkAwayTopic`, `AudioOutputOverride` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `DATA`, `VirtualMachineAdapter` | Select each whole value separately. |
| `LinkTo`, `Responses`, `Conditions` | Merge aligned rows in order. |
| `UnknownData` | Not independently forwarded. |
| `PreviousDialog` | Child/group surfaces; no independent property merge. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/DialogResponseRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="pack-package"></a>

## PACK — Package

**Disabled by default; these rules apply when enabled.**

The package template graph is inactive even when packages are enabled: PackageTemplate, DataInputVersion, Data, XnamMarker and ProcedureTree are not forwarded independently. The local writer and binary regression confirm key-sorted data rows instead of the source declaration order, while preserving index/value associations. This does not establish an in-game failure. Winner/priority snapshots still use that writer; disabling graph selection does not guarantee physical row-order preservation. See the [PACK audit](COVERAGE-COMPARISON-AUDIT.md#pack-arma-and-navi-boundaries). VMAD, IdleAnimations and the three event structures are separate whole values.

| Properties | How they are patched |
|---|---|
| `Type`, `InterruptOverride`, `PreferredSpeed`, `ScheduleMonth`, `ScheduleDate`, `ScheduleHour`, `ScheduleMinute`, `ScheduleDurationInMinutes`, `CombatStyle`, `OwnerQuest` | Select each value separately. |
| `Flags`, `InterruptFlags`, `ScheduleDayOfWeek` | Merge registered flag bits separately. |
| `VirtualMachineAdapter`, `IdleAnimations`, `OnBegin`, `OnEnd`, `OnChange` | Select each whole value separately. |
| `Conditions` | Merge aligned rows in order. |
| `Data`, `DataInputVersion`, `PackageTemplate`, `ProcedureTree`, `Unknown`, `Unknown2`, `Unknown3`, `Unknown4`, `XnamMarker` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/PackageRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="perk-perk"></a>

## PERK — Perk

**Ownership reset:** a change to `VirtualMachineAdapter`, `Conditions`, `Trait`, `Level`, `NumRanks`, `Playable`, `Hidden`, `NextPerk`, `Effects` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Name`, `Description`, `Trait`, `Level`, `NumRanks`, `Playable`, `Hidden`, `NextPerk` | Select each value separately. |
| `VirtualMachineAdapter`, `Icons` | Select each whole value separately. |
| `Effects` | Merge rows by key. |
| `Conditions` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/PerkRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="qust-quest"></a>

## QUST — Quest

Adapter presence gates its child fields. Structural changes are proposed and validated for alias IDs/references, stage/objective indices, fragment stage/log-entry references and supported object formats. Stages include log entries; objectives include targets and conditions; aliases are complete rows. The ownership reset below limits independent list merging. Validation does not provide general rollback of an already-created override.

**Ownership reset:** a change to `VirtualMachineAdapter.Presence`, `VirtualMachineAdapter.Version`, `VirtualMachineAdapter.ObjectFormat`, `VirtualMachineAdapter.Scripts`, `VirtualMachineAdapter.ExtraBindDataVersion`, `VirtualMachineAdapter.FileName`, `VirtualMachineAdapter.Fragments`, `VirtualMachineAdapter.Aliases`, `Type`, `Event`, `TextDisplayGlobals`, `DialogConditions`, `EventConditions`, `Stages`, `Objectives`, `NextAliasID`, `Aliases` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Name`, `VirtualMachineAdapter.Presence`, `VirtualMachineAdapter.Version`, `VirtualMachineAdapter.ObjectFormat`, `VirtualMachineAdapter.ExtraBindDataVersion`, `VirtualMachineAdapter.FileName`, `Priority`, `Type`, `Event`, `Filter`, `NextAliasID`, `Description` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `VirtualMachineAdapter.Scripts`, `VirtualMachineAdapter.Fragments`, `VirtualMachineAdapter.Aliases`, `Stages`, `Objectives` | Merge rows by key. |
| `TextDisplayGlobals`, `DialogConditions`, `EventConditions`, `Aliases` | Merge aligned rows in order. |
| `QuestFormVersion`, `Unknown` | Not independently forwarded. |
| `VirtualMachineAdapter.Versioning` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/QuestRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="scen-scene"></a>

## SCEN — Scene

Phases, Actors and Actions are separate whole lists. Phase conditions and action package lists stay inside their rows. Adapter presence gates child writes; ScriptFragments is one complete structure. Selected phase/action copies clear unused payloads, which are ignored in comparison. Scripts, Version and ObjectFormat do not trigger the broader ownership reset.

**Ownership reset:** a change to `VirtualMachineAdapter.Presence`, `VirtualMachineAdapter.ScriptFragments`, `Phases`, `Actors`, `Actions`, `Quest`, `LastActionIndex` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `VirtualMachineAdapter.Presence`, `VirtualMachineAdapter.Version`, `VirtualMachineAdapter.ObjectFormat`, `Quest`, `LastActionIndex` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `VirtualMachineAdapter.ScriptFragments`, `VNAM` | Select each whole value separately. |
| `Phases`, `Actors`, `Actions` | Select each whole collection separately. |
| `VirtualMachineAdapter.Scripts` | Merge rows by key. |
| `Conditions` | Merge aligned rows in order. |
| `Unused`, `Unused2` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/SceneRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="smbn-storymanagerbranchnode"></a>

## SMBN — Story Manager Branch Node

**Ownership reset:** a change to `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `MaxConcurrentQuests` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Parent`, `PreviousSibling`, `MaxConcurrentQuests` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Conditions` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/StoryManagerBranchNodeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="smen-storymanagereventnode"></a>

## SMEN — Story Manager Event Node

**Ownership reset:** a change to `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `MaxConcurrentQuests`, `Type` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Parent`, `PreviousSibling`, `MaxConcurrentQuests`, `Type` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Conditions` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/StoryManagerEventNodeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="smqn-storymanagerquestnode"></a>

## SMQN — Story Manager Quest Node

Quests is not an ownership-reset trigger: aligned quest rows may merge while the node configuration stays stable.

**Ownership reset:** a change to `Parent`, `PreviousSibling`, `Conditions`, `Flags`, `QuestFlags`, `MaxConcurrentQuests`, `MaxNumQuestsToRun` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Parent`, `PreviousSibling`, `MaxConcurrentQuests`, `MaxNumQuestsToRun` | Select each value separately. |
| `Flags`, `QuestFlags` | Merge registered flag bits separately. |
| `Conditions`, `Quests` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/StoryManagerQuestNodeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).
