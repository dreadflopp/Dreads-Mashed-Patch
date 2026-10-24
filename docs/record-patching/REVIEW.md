# Implementation decisions and regression reference

[Internal index](../README.md) · [Development guide](../DEVELOPMENT.md) · [Inventory](INVENTORY.md) · [Known limits](KNOWN-ISSUES.md)

Use this map to find the tests and invariants relevant to a change. Completed
migration plans and sequential build logs are not maintained here. Registrations
and per-record decision notes live with the handlers; current field lists live
in the inventory and public record pages.

## Regression map

| Area | Preserve this behavior and rationale | Tests |
|---|---|---|
| Record headers | One composite MajorRecordFlagsRaw handler owns the supported raw/common/typed bits. Independent header aliases or OR-based reapplication can resurrect selected clears. Unknown bits stay winner-owned; ACHR/REFR UDR reads the integer flag context. | [RecordHeaderFlagsTests](../../DreadsMashedPatch.Tests/RecordHeaderFlagsTests.cs), [PlacedReferenceUdrCoordinatorTests](../../DreadsMashedPatch.Tests/PlacedReferenceUdrCoordinatorTests.cs) |
| CONT/NPC inventory | Complete generated row copies retain duplicate occurrences, nullable COED, condition and all polymorphic owners. Reserve unchanged duplicates first. NPC count permission and COED permission precede ownership transfer; a new count cannot authorize a same-row COED reversion. CONT retains its count policy. | [ContainerItemExtraDataTests](../../DreadsMashedPatch.Tests/ContainerItemExtraDataTests.cs), [NpcItemExtraDataTests](../../DreadsMashedPatch.Tests/NpcItemExtraDataTests.cs) |
| Script-list VMAD | Replace scripts without resetting destination Version/ObjectFormat; defaults 5/2 apply only when creating an adapter. Script rows remain complete values. INFO/PACK/PERK whole adapters and QUST/SCEN child registrations have different boundaries. | [ScriptAdapterMetadataTests](../../DreadsMashedPatch.Tests/ScriptAdapterMetadataTests.cs), [VirtualMachineAdapterHandlerTests](../../DreadsMashedPatch.Tests/VirtualMachineAdapterHandlerTests.cs) |
| Translated names | Retain target language and all translations in detached generated copies. Respect Mutagen's language-comparison mode. Required CLAS/EYES/FLOR/KEYM names normalize null to empty; optional names can be absent; MATT Name is a plain string. | [NameTranslationTests](../../DreadsMashedPatch.Tests/NameTranslationTests.cs) |
| Output transactions | Stage context insertion, setters and ancestry before publication. Roll back failed candidates without losing prior parent/sibling edits. All output paths, including priority and NPC, share the transaction. | [RecordOverrideTransactionTests](../../DreadsMashedPatch.Tests/RecordOverrideTransactionTests.cs), [PatchOutputTransactionTests](../../DreadsMashedPatch.Tests/PatchOutputTransactionTests.cs) |
| Binary-overlay groups | Group lookup uses the registered getter interface; an overlay's runtime class is not a supported group key. Do not replace it with root.GetType(). | [RecordOverrideTransactionTests](../../DreadsMashedPatch.Tests/RecordOverrideTransactionTests.cs) |
| Partial publication and diagnostics | Record-local errors discard and report that candidate while successful records publish. Fatal import/type/serialization/publication failures still block files. Structured errors survive flushing. Severity must be a prefix; embedded `<Error: Unknown lstring ID ...>` is ordinary source text. | [PartialPublicationTests](../../DreadsMashedPatch.Tests/PartialPublicationTests.cs), [PatchOutputTransactionTests](../../DreadsMashedPatch.Tests/PatchOutputTransactionTests.cs), [LoggingWarningTests](../../DreadsMashedPatch.Tests/LoggingWarningTests.cs) |
| Initial policies | Priority sources, PreserveBaseline EditorID and Tamriel CELL handling run before ordinary short-history/vanilla exits. Ignored/disabled sources remain ineligible. Targeted identifier indexes avoid expanding every history merely to check policy. | [PolicyFilterTests](../../DreadsMashedPatch.Tests/PolicyFilterTests.cs), [AlwaysWinningModTests](../../DreadsMashedPatch.Tests/AlwaysWinningModTests.cs), [TamrielPersistentCellPolicyTests](../../DreadsMashedPatch.Tests/TamrielPersistentCellPolicyTests.cs) |
| Keyword presence | Selected null removes the list, present-empty remains distinct, and written links are detached. WEAP exclusivity is an additional policy, not a replacement setter. | [KeywordRemovalTests](../../DreadsMashedPatch.Tests/KeywordRemovalTests.cs), [WeaponKeywordListHandlerTests](../../DreadsMashedPatch.Tests/WeaponKeywordListHandlerTests.cs) |
| PHZD and placed projectiles | Review inherited getter/aspect fields, not only declared fields. Reuse compatible PHZD handlers with concrete context narrowing. Their IgnoredBySandbox is a nullable byte slice, not ACHR/REFR's boolean. UDR does not extend to these routes. | [PlacedHazardRecordHandlerTests](../../DreadsMashedPatch.Tests/PlacedHazardRecordHandlerTests.cs), [PlacedProjectileRecordHandlerTests](../../DreadsMashedPatch.Tests/PlacedProjectileRecordHandlerTests.cs) |
| VOLI | All twelve nullable lighting parameters form one authored preset; independent leaves would mix presets. | [AdditionalRecordSupportTests](../../DreadsMashedPatch.Tests/AdditionalRecordSupportTests.cs) |
| LENS | Both globals and the ordered nullable sprite sequence form one value. Retain order, duplicates, nullability and detached nested data. Compare serialized RGB separately from Opacity; do not compare non-serialized CLR alpha. LFSP counts are generated. | [AdditionalRecordSupportTests](../../DreadsMashedPatch.Tests/AdditionalRecordSupportTests.cs) |
| LTEX | Scalar friction/restitution remain independent; snow uses approved nullable flag handling. Absent INAM differs from present zero. Grass membership is sorted by FormID. | [AdditionalRecordSupportTests](../../DreadsMashedPatch.Tests/AdditionalRecordSupportTests.cs) |
| NPC Perks and GLOB/GMST | Perks must populate an absent destination and preserve selected presence. Subtype histories resolve through the shared group getter and stop at the latest eligible subtype boundary; no older subtype reconnection or scalar conversion. | [UserReportedFailureRegressionTests](../../DreadsMashedPatch.Tests/UserReportedFailureRegressionTests.cs), [diagnostic details](../USER-REPORTED-FAILURES-2026-10-10.md) |
| Asset paths and model bounds | Preserve GivenPath; accepted filename changes carry bounds. Bounds-only selection requires matching the selected filename identity, including both ARMO gender paths. Keep the shared selection gate rather than adding record-specific handlers. See the [model/bounds decisions](#model-and-bounds). ARMA model metadata has a separate comparison policy. | [AssetPathHandlingTests](../../DreadsMashedPatch.Tests/AssetPathHandlingTests.cs), [ModelBoundsHandlerTests](../../DreadsMashedPatch.Tests/ModelBoundsHandlerTests.cs), [ModelBoundsForwardingTests](../../DreadsMashedPatch.Tests/ModelBoundsForwardingTests.cs), [ModelHandlerRegressionTests](../../DreadsMashedPatch.Tests/ModelHandlerRegressionTests.cs) |
| Collection identity | Sorted keys, aligned occurrence IDs, exact positions and whole sequences are distinct policies. Do not use full row equality as a substitute for the reviewed identity key. | [XEditListAlignmentTests](../../DreadsMashedPatch.Tests/XEditListAlignmentTests.cs), [ListSemanticsTests](../../DreadsMashedPatch.Tests/ListSemanticsTests.cs), [ExactOrderedEffectsTests](../../DreadsMashedPatch.Tests/ExactOrderedEffectsTests.cs), [CollectionSemanticsBinaryRoundTripTests](../../DreadsMashedPatch.Tests/CollectionSemanticsBinaryRoundTripTests.cs) |

## Model and bounds

All record types registered with `ModelAndBounds` and ARMO's `WorldModelAndBounds`
share `ModelBoundsHandler`. An accepted filename change takes the source's bounds;
a bounds-only edit must match the selected normalized filename identity before
ordinary addition/reversion permission is evaluated. ARMO uses both male/female
world-model paths, including absent sides. Metadata/icons remain part of model
selection but do not define filename identity. Case and slash direction do not
distinguish filenames. Accepted declared/virtual-master model reversions still
take the reverting source's bounds.

The rejected approach was to let every adjacent bounds difference proceed through
ordinary selection: a later source carrying Original.nif plus new bounds could
have its filename rejected while replacing Replacement.nif's bounds. An inherited
bounds edit could do the same. Comparing adjacent bounds still identifies candidate
edits; matching the selected filename makes that candidate eligible. Do not replace
this with unconditional atomic grouping, which would lose valid bounds-only fixes.

The [inventory](INVENTORY.md#record-and-property-matrix) gives each record's shared
registration and specialized exceptions. ASPC, DUAL, ENCH, LVLI, LVSP, NPC_, SOUN,
SPEL and TXST retain independent complete bounds:
they have no corresponding main model filename to coordinate. NPC appearance and
referenced models are outside this direct model/bounds rule. TREE retains its
winner-owned restriction by default; unrestricted forwarding uses the shared gate.
No handler was replaced, no duplicate implementation was added, and approved flag
handlers remain unchanged.

`ModelBoundsForwardingTests` discovers every shared registration and covers both
rejected-filename scenarios through patch processing and binary write/reload,
plus matching-filename corrections. It also covers both armor genders, absent
counterparts, path spelling, declared/virtual-master reversions, restricted TREE,
and the independent-bounds registrations. These synthetic tests do not inspect
the deployed mesh for a same-path replacement or establish gameplay compatibility.

## Appearance boundaries

| Record | Shared selection | Specialized behavior and reason |
|---|---|---|
| NPC_ | Ordinary atomic value ownership and declared/virtual-master restoration. Height uses the existing scalar handler; OppositeGenderAnims uses its own approved masked flag handler. | Face fields, Race, Female, Weight and WornArmor retain generated masked copying as one coherent value. Height/animation edits do not replace or take ownership of that face. Appearance, animation and protection writers own disjoint flag masks. |
| RACE | Ordinary atomic selection of HeadData, MorphRace, tint count and FaceGen clamps. Height/Weight are independent gendered values. | Typed gendered copying and deep HeadData equality are required: the generated record-level gendered copy bypasses some masks and wrapper equality is insufficient. |
| HDPT | Ordinary atomic selection of model, type, extra parts, morph files, texture/color, valid races and approved flags. | Keep model and morph resources together. ExtraParts equality ignores sorting; morph-file order remains significant. |
| TREE | Default RestrictTreeForwarding resolves model/bounds, TreeData, scripts and header flags from the effective winner. Disabling it uses the same registered handlers normally. | Preserve full CNAM, including eight floats represented as 32 Unknown bytes. There is no model-filename/geometry-owner heuristic to infer compatibility with wind data or same-path mesh replacers. Priority snapshots still take precedence. |

NPC head/tint sorting and HDPT extra-part sorting alone do not claim ownership;
complete entry contents and duplicate counts still matter. Copies retain source
order. Do not reintroduce unconditional winning-appearance selection or independent
head/tint row merging. TemplateAndTraits remains a separate atomic pointer/Traits
value. Effective template inheritance and deployed FaceGen compatibility are not
resolved by selecting local fields.

Tests: [AppearanceAndTreeForwardingTests](../../DreadsMashedPatch.Tests/AppearanceAndTreeForwardingTests.cs),
[HeadPartRecordHandlerTests](../../DreadsMashedPatch.Tests/HeadPartRecordHandlerTests.cs),
[ProtectionFlagsHandlerTests](../../DreadsMashedPatch.Tests/ProtectionFlagsHandlerTests.cs),
[DiagnosticValueFormatterTests](../../DreadsMashedPatch.Tests/DiagnosticValueFormatterTests.cs).

## Reference evidence

The reviewed xEdit revision is `93cc0bc5a1251936c3c7859eee3150eda12a62d7`.
In `Core/wbDefinitionsTES5.pas`, LTEX is at lines 8000–8020 and VOLI/LENS at
10206–10243; placed-projectile registrations at 4184–4191 use the shared
ReferenceRecord definition. Collection evidence is retained in the
[ordering references](../history/README.md). Regenerate ignored Mutagen and
xEdit trees with the [reference exporters](../REFERENCE_SOURCES.md) after version
changes. They are reference-only inputs, not project code.

## Validation scope

The last implementation run, on 2026-10-24 before the documentation cleanup,
built the solution/core/WPF projects with zero warnings/errors, passed 1,809
workspace tests, audited 131 handlers and 1,750 registrations with zero
missing/partial candidates, and verified 204 collection entries with zero
unresolved entries/failures. The count included the workspace's untracked
DisabledPluginLoadOrderTests.cs. Commands and future validation requirements
live in the [development guide](../DEVELOPMENT.md#validation); this dated snapshot
is not a claim about later changes.

A 2026-10-05 disk-backed check used the desktop runner services through a temporary
native Linux harness with the maintainer's real load order. Output reloaded with
2,838 unique major records and 131 masters. This established that run's input,
transaction and binary publication path, not universal field preservation,
Proton desktop integration, xEdit conflict review or gameplay correctness.
UI rendering evidence and platform gaps are summarized in
[Standalone development](../STANDALONE_DEVELOPMENT.md#validation-scope).
