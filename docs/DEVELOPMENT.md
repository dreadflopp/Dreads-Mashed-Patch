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

This table selects implementation patterns; it does not authorize changing an existing property's policy. Record-specific exceptions are in the [property tables](record-patching/INDEX.md). For example, NPC protection, ARMA filename/data comparison, QUST/SCEN adapter presence, and model/bounds coupling retain their specialized behavior. WATR's nullable `Flags` currently selects a whole scalar value; document that existing exception rather than assuming every enum is merged per bit.

Generated mutable copies must be detached from source records, including nested lists and links. Do not directly reuse a mutable source merely because its concrete type matches. Match nullability and setter normalization to the actual interface and existing policy. The shared keyword setter distinguishes absent from present-empty lists; that does not establish a universal null-removal rule.

Translated names retain the target language and available translations. Equality follows `TranslatedString.DefaultLanguageComparisonOnly`. Required CLAS/EYES/FLOR/KEYM names normalize null to a fresh empty translated value; optional names permit removal. MATT Name remains a plain string. Ordinary script-list adapters retain destination Version/ObjectFormat when replacing scripts; whole adapters and QUST/SCEN child fields have separate policies.

VOLI’s `LightingPresetHandler` and LENS’s `FlareDefinitionHandler` follow the existing grouped-snapshot pattern: each complete authored preset has one owner, with no parallel leaf handlers. LENS copies sprites with generated deep copies but compares tint as serialized RGB; the extra CLR alpha component is not in LFSD. Texture comparison uses `GivenPath` through the existing asset helper. Its embedded flags travel inside the atomic definition.

All seven placed-projectile handlers reuse PHZD’s inherited-field patterns and concrete narrowing from the shared `IAPlacedTrap` query. ACHR/REFR UDR coordination remains outside these routes. LTEX uses the approved nullable snow flag handler; nullable presence and zero are distinct. Its xEdit HNAM friction/restitution fields remain separate scalar decisions, and GNAM grasses use sorted FormID keys. See [record-specific completion evidence](record-patching/REVIEW.md#additional-record-support-verification).

Appearance groups use `AbstractWinningPropertyHandler` to accept each complete chronological snapshot, including original-valued restoration. NPC/HDPT use generated masked copies plus approved masked flag setters. TemplateAndTraits couples winning appearance inheritance with its template while reusing ordinary pointer forwarding for gameplay-only inheritance; RACE uses typed gendered copying and generated HeadData equality because its generated record copy bypasses some gendered masks. `IPropertyGroupHandler` declares exact grouped ownership for coverage checks. TREE resolves asset-dependent contexts from the effective winner when its default-enabled forwarding restriction is active; disabling it uses the same shared handlers normally. See the [migration evidence](record-patching/REVIEW.md#appearance-and-tree-forwarding).

### TREE Smart Forwarding design

The [public TREE description](record-patching/world.md#tree-tree) explains the restriction checkbox, excluded fields and asset limits. Keep detailed user-facing behavior out of the root README; its settings table should link to the public description. Developer rationale and validation evidence belong here and in internal review documents.

TREE stays enabled. `ForwardingSettings.RestrictTreeForwarding` defaults to true, follows the existing boolean forwarding-setting pattern, is copied into the runtime snapshot, and binds directly to the Smart Forwarding checkbox. Missing settings receive the restrictive default. Explicit false survives serialization, configuration copying and runtime application. The retired TreePolicy JSON property is still ignored and cannot restore model/wind coordination.

In [TreeRecordHandler](../DreadsMashedPatch/RecordHandlers/TreeRecordHandler.cs), all contexts initialize through the base implementation. When restricted, model/bounds, TreeData, VMAD and header flag contexts are reinitialized from the effective winner and marked resolved. The ordinary pass then skips them. When unrestricted, this early resolution is skipped, so the same registered handlers perform normal value, model/bounds, script and approved flag selection. Priority record selection still precedes either behavior. No handler is swapped or duplicated, and approved flag handlers remain in use.

The specialized TreeData snapshot always keeps all twelve wind values together: the four named Mutagen floats plus eight values represented by its 32-byte `Unknown` block. Copy that block exactly into detached storage. In unrestricted forwarding, CNAM is selected independently from the model through the normal whole-value ownership rules. No geometry-owner tracking or filename compatibility heuristic is reintroduced. TREE's coordination hook only emits an exclusion explanation when restricted and returns the shared no-op result otherwise.

The checkbox help leads with models, bounds, wind settings, scripts and flags that are excluded, then explains that the restriction avoids mixing incompatible appearance and behavior. This policy preserves the winning record even when it restores an earlier value. It cannot infer deployed mesh ownership or whether a later override accidentally copied original appearance data. An authored compatibility patch provides an explicit choice when an earlier replacement should survive. The [checkbox implementation evidence](record-patching/REVIEW.md#tree-restriction-checkbox--2026-10-17) records the toggle and validation; the earlier coordination evaluation remains historical.

The [appearance and TREE regression tests](../DreadsMashedPatch.Tests/AppearanceAndTreeForwardingTests.cs) cover legacy settings, both checkbox values through JSON/runtime application, excluded fields, unrestricted whole-CNAM selection, priority precedence and binary copying. [Configuration tests](../DreadsMashedPatch.Tests/PatcherConfigurationTests.cs) verify the default and snapshot copying. The [model/bounds tests](../DreadsMashedPatch.Tests/ModelBoundsHandlerTests.cs) retain shared behavior coverage. Keep actual build, test and audit results in internal validation evidence with their stated limits.

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

## Migration completion

Update all call sites together, remove superseded classes/imports and redundant overrides, and search the workspace for obsolete references. Each property must have exactly one active implementation. Keep approved flag handlers. Add a short decision note per record type stating what was generalized, what stayed specialized or unregistered, and why. Report removed dead code and actual diagnostics/build/test results with their validation limits.

Update affected current guides, tables, inventory, and known limits in the same change. Historical rationale belongs in [migration history](history/README.md) or a dated [review-evidence section](record-patching/REVIEW.md); preserve previous test counts with their baseline instead of treating them as current results.
