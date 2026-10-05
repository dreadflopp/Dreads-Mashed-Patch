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

Use the [application guide](STANDALONE_APP.md#build-and-publish) for complete build/publish instructions. Linux can build the Windows app and run platform-neutral tests; its WPF interface runs through Proton. The self-contained published executable includes the runtime, while building requires the .NET 10 SDK.

## Handler selection

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

## Output and diagnostics

Use `AbstractRecordHandler.CommitOverride` for output mutations, including whole-record policies. It stages context insertion, property writes, and validation before publishing the completed record and required ancestry. `ApplyForwardedProperties` mutates detached candidates; it is not a publication API.

Use `LogCollector.AddError` for unrecovered read/write/copy failures. Setter failures must propagate; do not silently discard malformed rows or downgrade failed writes to warnings. Recovered complete-value fallbacks can remain nonfatal. Use the shared diagnostic classifier so embedded text such as `<Error: Unknown lstring ID ...>` is not mistaken for a severity prefix.

`Program.RunPatchWithReport` returns structured errors; `Program.RunPatch` rejects a failed run. Other records can continue for diagnostics, but the desktop host publishes only after a successful report and no pipeline errors. Failed runs preserve previous output files. Shared post-write validation compares non-null values with the registered semantic comparer; null/default normalization and specialized graph validation retain their own limits.

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
