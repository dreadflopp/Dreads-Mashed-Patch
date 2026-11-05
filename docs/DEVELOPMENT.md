# Development guide

[Documentation index](README.md) · [Repository instructions](../AGENTS.md)

## Architecture and prerequisites

The core targets .NET 10 and uses Mutagen.Bethesda.Skyrim 0.54.4 and Mutagen.Bethesda.Synthesis 0.36.6. The Windows desktop host uses WPF and invokes the same core patch implementation; it configures the pipeline rather than duplicating record forwarding. Package versions are defined in the project files.

- [Program](../DreadsMashedPatch/Program.cs) owns supported/excluded catalogs, queries, filtering, dispatch, and run reporting.
- [PatcherConfiguration](../DreadsMashedPatch/PatcherConfiguration.cs) defines new-configuration defaults; persisted configurations may differ.
- [AbstractRecordHandler](../DreadsMashedPatch/RecordHandlers/Abstracts/AbstractRecordHandler.cs) owns selection, policy application, and staged record publication.
- [Property handlers](../DreadsMashedPatch/PropertyHandlers) and [contexts](../DreadsMashedPatch/Contexts) define value equality, copying, and ownership.
- [RecordPolicySources](../DreadsMashedPatch/RecordPolicySources.cs) performs targeted priority/baseline identifier lookups. Policy-only records need not expand complete histories; ordinary merging still does.
- [RecordOverrideTransaction](../DreadsMashedPatch/RecordOverrideTransaction.cs) stages detached records and ancestry. [PatcherRunner](../DreadsMashedPatch.App/Services/PatcherRunner.cs) uses [PatchOutputTransaction](../DreadsMashedPatch/PatchOutputTransaction.cs) to publish files after a successful run.

Initial history lookups normally use the record's registered getter type, matching the typed history lookups during merging. GLOB and GMST use their shared group getter throughout filtering, policy lookup and merging: Mutagen's typed contexts can cast an older record of another subtype when its Record is read. Their effective winner is resolved before subtype dispatch. `AbstractSubtypeRecordHandler<TGetter>` retains only the contiguous eligible history of that subtype; its oldest remaining version becomes the merge baseline. Ignored plugins do not create boundaries. Priority snapshots can select any subtype in the same group, and official GMST EditorID preservation requires a compatible type prefix. Filtering still retains at most three eligible contexts, without constructing a broad `IMajorRecordGetter` index.

Use the [standalone development guide](STANDALONE_DEVELOPMENT.md#build-and-publish) for complete build/publish instructions. Linux can build the Windows app and run platform-neutral tests; its WPF interface runs through Proton. The self-contained published executable includes the runtime, while building requires the .NET 10 SDK.

## Handler selection

### Source comments and decision notes

Comments should explain constraints that a reader cannot infer from names and
control flow: ownership boundaries, serialization and overlay limitations,
null/presence semantics, ordering evidence, and reasons for specialized copying.
Put them beside the affected decision or on the shared implementation. Avoid
assignment/loop narration, copied member documentation, repeated registration
lists, completed cleanup checklists, and historical wording such as “now handled
by the base class.” Describe current behavior and check comments when it changes.

Keep concise per-record decisions in the [record inventory](record-patching/INVENTORY.md)
and regression rationale/test pointers in the [regression reference](record-patching/REVIEW.md).
These satisfy the per-record decision-note requirement: record what uses shared
handlers, what remains specialized or excluded, and why. Routine migrations do
not need an additional source-code history block. Document shared header-flag,
translated-name, script-metadata and asset-ownership rules once in their shared
implementation and current guides instead of repeating them in every record.

### Shared forwarding patterns

All record types share one original-to-winning forwarding pass. The former
winning-to-original second pass and its quick-check branch were removed because
every property handler required the complete override history. The constant
`RequiresFullLoadOrderProcessing` interface member and implementations were also
removed. Scalar, collection, and flag properties keep their existing handlers,
equality, ownership, and copying policies. Atomic ownership resets, record-specific
coordination, and `IsResolved` early exits (such as NPC protection and Impact Data
Set fallback) remain specialized because they affect forwarding decisions.

Find an existing equivalent handler in the same layer before adding one. Verify the exact getter/setter surface in repository usage and, when necessary, the [local decompiled references](REFERENCE_SOURCES.md). Decompiled sources are reference-only and must never be edited or compiled into the project. Ask for additional interface documentation only if behavior remains unclear after those checks.

| Value | Existing pattern |
|---|---|
| Scalar/value type or plain string | `SimpleReflectionPropertyHandler`; complex reference types are rejected. |
| Form reference | `SimpleReflectionFormLinkPropertyHandler` or the existing typed adapter; compare identity by FormKey. |
| Flag field | Approved flag handlers such as `SimpleReflectionFlagPropertyHandler` / `SimpleReflectionNullableFlagPropertyHandler`, or the existing specialized flag policy. Keep flag handlers during migrations. |
| Record-header flags | Exactly one `MajorRecordFlagsRawHandler` registration combining base/common/applicable typed bits. Do not register overlapping header views. Unknown winner bits remain intact. |
| Translated text | `TranslatedStringReflectionPropertyHandler` and generated copying. Keep `ITranslatedStringGetter` in selection; reducing it to `.String` loses other languages. |
| Asset path | `SimpleReflectionAssetLinkPropertyHandler` or a typed asset aggregate. Read/write `GivenPath`; compare through the existing path helpers. `DataRelativePath` is lookup information. |
| Complex Mutagen structure | `GeneratedCopyReflectionPropertyHandler` with the generated copy function and reviewed semantic comparison, or an equivalent specialized handler. |
| Collection | Explicit [collection semantics](COLLECTION_SEMANTICS.md), with a reviewed xEdit key, order, presence, duplicates, and ownership policy. Use generated-copy handlers for complex rows. |
| Binary payload | Existing typed binary handler; copy the whole detached payload and preserve its documented presence policy. Binary data is not categorically unsupported. |

This table selects implementation patterns; it does not authorize changing an existing property's policy. Record-specific exceptions are in the [property tables](record-patching/INDEX.md). For example, NPC protection, ARMA filename/data comparison, QUST/SCEN adapter presence, and model/bounds coupling retain their specialized behavior. WATR uses disjoint approved damage/rendering flag masks and retains destination nullable presence for zero clears; see the asset forwarding guide.

Generated mutable copies must be detached from source records, including nested lists and links. Do not directly reuse a mutable source merely because its concrete type matches. Match nullability and setter normalization to the actual interface and existing policy. The shared keyword setter distinguishes absent from present-empty lists; that does not establish a universal null-removal rule.

Translated names retain the target language and available translations. Equality follows `TranslatedString.DefaultLanguageComparisonOnly`. Required CLAS/EYES/FLOR/KEYM names normalize null to a fresh empty translated value; optional names permit removal. MATT Name remains a plain string. Ordinary script-list adapters retain destination Version/ObjectFormat when replacing scripts; whole adapters and QUST/SCEN child fields have separate policies.

VOLI’s `LightingPresetHandler` keeps one complete authored preset. LENS reuses `FlareDefinitionHandler` through a projection owning only ordered sprites and their texture/data; both globals have ordinary scalar handlers. Projection writes preserve the globals, so no field has parallel selection owners. LENS copies sprites with generated deep copies but compares tint as serialized RGB; the extra CLR alpha component is not in LFSD. Texture comparison uses `GivenPath` through the existing asset helper. Its embedded flags travel inside the atomic definition.

All seven placed-projectile handlers reuse PHZD’s inherited-field patterns, including shared Transform and Ownership groups, and concrete narrowing from the shared `IAPlacedTrap` query. All placed routes resolve Transform independently of InitiallyDisabled and EnableParent. LTEX uses the approved nullable snow flag handler; nullable presence and zero are distinct. Its xEdit HNAM friction/restitution fields remain separate scalar decisions, and GNAM grasses use sorted FormID keys. See [record-specific completion evidence](record-patching/REVIEW.md#regression-map).

Appearance always forwards as a group. There is no appearance setting. NPC retains its typed masked snapshot; RACE HeadShape coordinates typed per-gender head-part/morph slices with MorphRace and clamps. Meshes, textures, tint definitions/count and race choices have narrower ownership described in [asset forwarding](ASSET_FORWARDING.md#character-resource-boundaries-and-migration-decisions). HDPT is a mesh definition, using generated masked copying and approved rendering flags; applicability remains ordinary. `IPropertyGroupHandler` exposes exact ownership to UI help and coverage. Its implementations may require constructor arguments; audit actual record registrations rather than creating handler types with guessed defaults. TemplateAndTraits retains atomic pointer/Traits selection.

NPC appearance owns only the Female configuration bit. Height uses `SimpleReflectionPropertyHandler<float, INpc, INpcGetter>` with the shared 0.0001 tolerance. The independent `Configuration.Flags.OppositeGenderAnims` registration uses the approved masked flag handler against the actual `Configuration.Flags` enum property; its dictionary key labels a bit, not a reflection member. Appearance, animation and protection setters own disjoint masks and preserve unrelated/unknown bits. Race, sex, weight and WornArmor remain with the face. HairColor uses ordinary forwarding through the shared nullable form-link handler, without an asset group or gate; it is excluded from face snapshots, comparisons and ownership. RACE’s separate resource groups and TemplateAndTraits retain their own policies. See the [appearance decisions and regressions](record-patching/REVIEW.md#appearance-boundaries).

Animation forwarding has narrowly scoped reference packages: male/female RACE rig
pairs, a separate body-part reference, IDLE filename/event, IDLM idle list/flags/timer,
PACK PackageIdles and NPC animation gender. Combat/movement tuning and IDLE legacy
DATA keep ordinary handlers. The [animation architecture](ASSET_FORWARDING.md#animation-and-skeleton-boundaries)
records generic side copying, nullable flags, exact source provenance and regressions.

### TREE Smart Forwarding design

TREE has one TreeAssets group under ForwardingRule.Trees. ForwardTreeGroups defaults
false; other resource and appearance groups always forward. Its record hooks use the
standard catalog initialization and final-coordination pattern, with no additional
winner-context reset. The [TREE architecture](ASSET_FORWARDING.md#tree-selection-boundary)
records retained model/bounds, VMAD metadata, CNAM and flag invariants and regression
entry points. [Public TREE guidance](record-patching/world.md#tree-tree) explains
its single checkbox and keeps user-facing behavior out of the root README.

## Asset group policies

Read the [asset forwarding guide](ASSET_FORWARDING.md) and generated
[ownership manifest](asset-forwarding-groups.json) before changing a resource field.
Record-local hooks disable independent member decisions; final coordination owns
one choice per group and reuses registered setters/comparers. The common application loop orders ordinary container writes before nested reference writes. Multi-member collections are atomic snapshots rather than
independently merged lists. Existing whole-record/TREE precedence, structural
resets, bounds provenance and approved disjoint flag masks must remain intact.
Resource/appearance policy is fixed true in FixedForwardingRules, with no serialized controls. ForwardTreeGroups defaults off. Current behavior and exact per-record members are in [grouped forwarding](record-patching/assets.md). Restoration steps for future controls are retained in [the architecture guide](ASSET_FORWARDING.md#restoring-asset-or-appearance-controls-later).

## Output and diagnostics

Use `AbstractRecordHandler.CommitOverride` for output mutations, including whole-record policies. It stages context insertion, property writes, and validation before publishing the completed record and required ancestry. `ApplyForwardedProperties` mutates detached candidates; it is not a publication API.

Use `LogCollector.AddError` for unrecovered read/write/copy failures. Setter failures must propagate; do not silently discard malformed rows or downgrade failed writes to warnings. Recovered complete-value fallbacks can remain nonfatal. Use the shared diagnostic classifier so embedded text such as `<Error: Unknown lstring ID ...>` is not mistaken for a severity prefix.

`Program.RunPatchWithReport` returns errors and skipped-record metadata. A record-local failure discards the whole candidate; only the record-processing catch marks that scope's errors as recovered and adds the FormKey, record type, and reason to `SkippedRecords`. Other records continue. This publishes a partial patch by default in both hosts, with no setting or opt-in. `Succeeded` means no fatal errors remain; `IsPartial` distinguishes a run with skipped records from a clean run. Error and skipped-record counts are separate because one failed record can produce several errors. Reports survive log flushing.

`Program.RunPatch` rejects fatal reports. The desktop host also requires the patch callback to have run, no pipeline errors, and the primary staged output before publishing primary/split files together. Load/import, record-type processing, serialization, and publication failures remain blocking and preserve previous output files. Record diagnostics have an explicit `[Error] [Record]` prefix so asynchronous log flushing cannot turn them into pipeline failures; this prefix alone does not recover a structured error. The desktop completion status identifies partial patches, and the run log lists every skipped record. Shared post-write validation compares non-null values with the registered semantic comparer; null/default normalization and specialized graph validation retain their own limits.

## Validation

Run from the repository root:

```bash
dotnet restore DreadsMashedPatch.sln
dotnet build DreadsMashedPatch.sln --no-restore --verbosity minimal
dotnet test DreadsMashedPatch.Tests/DreadsMashedPatch.Tests.csproj --no-restore --verbosity minimal
```

For collection changes, verify the tracked manifest against runtime registrations and independently reviewed expectations:

```bash
pwsh -NoProfile -File scripts/Audit-CollectionSemantics.ps1 -Verify -FailOnUnresolved
```

Regenerate property coverage after registration/interface changes, and review both report diffs:

```bash
pwsh -NoProfile -File scripts/Audit-RecordHandlerCoverage.ps1 -FailOnUnresolved
```

On Windows, the scripts also accept `powershell -NoProfile -ExecutionPolicy Bypass -File`. [Collection semantics](COLLECTION_SEMANTICS.md) explains regeneration separately from verification; [reference sources](REFERENCE_SOURCES.md) explains prerequisite runtimes and pinned source exports. `scripts/Audit-SerializationStateHandlers.ps1` provides an additional source scan for accidentally registered serializer-state properties.

Tests should establish behavior beyond registration counts: selection with declared/virtual-master permission, removal/restoration, duplicate/order handling where applicable, detached copies, and binary overlay/write/reload for affected fields. Passing audits do not establish gameplay safety or complete on-disk preservation.

Keep expected results independent of the implementation being checked. Default compatibility rules may grow or change; validate filenames, unique targets and independent instances rather than freezing counts or repeating curated relationships in assertions. Copy tests should use explicit fixture values instead of assuming the current default keyword count. Exact counts remain useful for controlled fixtures when they detect lost or duplicated rows.

Use `CollectionSemanticsManifestTests` and its reviewed manifest/xEdit expectations as the shared collection registration checks; do not repeat subsets in separate tests. Keep record-specific checks for disjoint grouped ownership, approved flag handlers and deliberately excluded fields, because those enforce patching safety boundaries. Prefer exercising registered handlers and checking their output over asserting an incidental class name. When removing a duplicate test, confirm the retained test covers its assertion; when comparing one changed field, keep unrelated inputs equal.

## Migration completion

Update all call sites together, remove superseded classes/imports and redundant overrides, and search the workspace for obsolete references. Each property must have exactly one active implementation. Keep approved flag handlers. Add a short decision note per record type stating what was generalized, what stayed specialized or unregistered, and why. Report removed dead code and actual diagnostics/build/test results with their validation limits.

Update affected current guides, tables, inventory and known limits in the same change. Preserve useful rationale and regression pointers in the [decision references](history/README.md) or [regression map](record-patching/REVIEW.md). Remove completed plans and duplicate migration transcripts after consolidating their unique constraints. Validation results need a date and scope; old aggregate test counts are not ongoing release certification.
