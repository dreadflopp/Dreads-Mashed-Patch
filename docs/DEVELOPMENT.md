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

All seven placed-projectile handlers reuse PHZD’s inherited-field patterns, including shared Transform and Ownership groups, and concrete narrowing from the shared `IAPlacedTrap` query. ACHR/REFR use InteriorReferencePlacement for interior cell + DATA selection and same-cell XLRL, with independent Scale. Placement/XLRL contexts are resolved only by that policy, using ordinary handlers for same-parent histories. Supported moves supply winning baseline data and effective destination ancestry to the existing transaction; normal processing emits each FormKey once. Exterior/specialized moves use winning fallback. Other placed routes retain Transform; enable-state decisions remain independent. LTEX uses the approved nullable snow flag handler; nullable presence and zero are distinct. Its xEdit HNAM friction/restitution fields remain separate scalar decisions, and GNAM grasses use sorted FormID keys. See [record-specific completion evidence](record-patching/REVIEW.md#regression-map).

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

The desktop warning count measures emitted warning entries, including repeated occurrences with identical text. Partial-patch, fatal skipped-record and malformed-context load summaries are progress lines, explicitly admitted by `UiTextWriter`; they must not carry warning severity because their underlying diagnostics have already been counted. Keep genuine warnings at the point of failure. An explicit `[Warning]`, `[Error]`, `[Info]` or `[Diagnostic]` prefix owns the entry's severity; the shared classifier may skip a property tag, but must not reinterpret a severity tag as a property tag and promote message text to a different severity. `LoggingWarningTests` covers severity formats, embedded diagnostic words, buffered writes and malformed-context counts; `OutputPatchValidatorTests` and `RecordOverrideTransactionTests` cover warning-only omissions and record-error skips through full runs.

`Program.RunPatchWithReport` returns errors and skipped-record metadata. A record-local failure discards the whole candidate; only the record-processing catch marks that scope's errors as recovered and adds the FormKey, record type, and reason to `SkippedRecords`. Other records continue. This publishes a partial patch by default in both hosts, with no setting or opt-in. `Succeeded` means no fatal errors remain; `IsPartial` distinguishes a run with skipped records from a clean run. Error and skipped-record counts are separate because one failed record can produce several errors. Reports survive log flushing.

`Program.RunPatch` rejects fatal reports. The desktop host also requires the patch callback to have run, no pipeline errors, and the primary staged output before publishing primary/split files together. Load/import, record-type processing, serialization, and publication failures remain blocking and preserve previous output files. Record diagnostics have an explicit `[Error] [Record]` prefix so asynchronous log flushing cannot turn them into pipeline failures; this prefix alone does not recover a structured error. The desktop completion status identifies partial patches, and the run log lists every skipped record. Shared post-write validation compares non-null values with the registered semantic comparer; null/default normalization and specialized graph validation retain their own limits.

`OutputPatchValidator` runs once merging has completed successfully, before Synthesis writes/splits files, for both hosts. Its start and retained/omitted completion messages are explicitly admitted by `UiTextWriter` and flushed so the desktop Run Log shows the validation phase before lookup/enumeration work. It examines emitted records only; source-selection and permission rules are unchanged. Mutagen's generated `EnumerateFormLinks(false)` checks each record's own nested values without attributing child-record errors to its parent. Optional/null links and unknown flag bits remain permitted. Non-null links must resolve to the generated permitted target type against output-first/input-winner resolution, with the exact engine PlayerRef exception below. Only NPC Packages and leveled Entries impose a deleted-target restriction; do not generalize it to historical/metadata links without confirmed field rules. Deleted output records retain their unused residual payload.

`Constants.Player` (`000014:Skyrim.esm`) is an engine-provided actor reference, not the Player's NPC_ base (`000007:Skyrim.esm`), and does not require a physical plugin record. Accept it only when the expected getter can represent `IPlacedNpcGetter`; it cannot satisfy NPC_ base, package or item links. This is not an exemption for every low FormID or for `000014` owned by another plugin. Provenance: pinned xEdit `Core/wbInterface.pas:19395–19399` checks `IsPlayer` as PLYR before ordinary lookup; `Core/wbDefinitionsTES5.pas:3075–3076` permits PLYR for XESP; decompiled Mutagen 0.54.4 `Constants.cs` identifies the FormKey. `OutputPatchValidatorTests` exercises an absent Player plugin record and retained XESP through binary output, broad script links and negative key/type cases. Forwarding fixtures must not fabricate a Player ACHR to satisfy the cache: doing so hides this engine-specific lookup exception.

### COED faction-rank import workaround

This is a compatibility workaround for the current import pipeline, not a Skyrim rank-format rule. COED's second ownership slot is contextual: NPC ownership uses a Global link, faction ownership uses a signed 32-bit rank, and no owner makes the slot unused. Mutagen 0.54.4 falls back to `UntypedOwner` when it cannot identify an external owner during import. Synthesis 0.36.6 does not supply the external type lookup. Generated link enumeration and writing then treat both slots as FormLinks, losing the original scalar's high-byte representation and potentially remapping it on write.

`OutputPatchValidator.ValidateRecord` normalizes only emitted, non-deleted records whose COED owner is still `UntypedOwner` and resolves to `IFactionGetter` against the effective output/input winners. The approved decoding rule is `id = VariableData.FormKey.ID; rank = id <= 0x800000 ? (int)id : (int)id - 0x1000000`. Choose the signed integer with the smallest absolute value compatible with those remaining 24 bits. IDs at or below `800000` get an upper byte of `00`, and IDs above it get `FF`. At the equal-magnitude midpoint `800000`, choose positive 8388608 rather than -8388608. This single rule replaces the former special case mapping `FFFFFF` to -1; -1 follows naturally from the same rule. It supports modest positive and negative ranks consistently without imposing the unrelated NPC membership minimum of -128. Replace the owner union with `FactionOwner` containing the same faction link and that integer rank. Do not merely ignore the bogus link: the typed union is what makes Mutagen's writer emit an integer. Generated link enumeration then validates the faction reference and no longer treats its rank as a reference. The COED condition, row count/order/duplicates, other properties, source plugins and merge-selection rules stay intact. One `[Info]` message per affected record gives the conversion count and FormKey; the former unsupported-rank rejection and its serialization-limitation warning have been removed.

The explicit assumption is that original ranks lie in the reconstructed range -8388607 through 8388608. All ranks from -10000 through 10000 are reconstructed correctly; for example `FFD8F0` becomes -10000, `FFFF7F` becomes -129, `FFFF80` becomes -128, `FFFFFE` becomes -2, and `FFFFFF` becomes -1. This is a reconstruction heuristic, not a COED format constraint or an 8-bit NPC faction-membership rule: COED ranks are signed 32-bit integers. It cannot preserve arbitrary original integers: positive 16767216 also leaves `FFD8F0` and becomes -10000; negative -8388608 becomes positive 8388608 at the midpoint; `01000006` becomes 6. The original upper byte cannot always be recovered from the FormKey plugin identity: out-of-range source-local master indices collapse to the source plugin, and its current runtime load-order index is unrelated. Retaining original bytes or correctly typed import is required for lossless recovery. Already typed `FactionOwner` values are never normalized, so their complete signed 32-bit ranks, including other negative/large values, remain intact. NPC owners and other reference fields are excluded; never mask runtime light-plugin IDs down to `FFF`, reinterpret NPC-owned Global links, or exempt a matching genuine broken reference elsewhere. Unowned COED's second slot is ignored with occurrence counting, preserving detection of identical links in other fields.

NPC/CONT inventory and LVLI/LVLN entry COED share the existing ownership-data traversal and one normalization path. Record-specific property selection and approved flag handlers remain specialized and unchanged. No source adapter, extra import pass, competing property handler or general mod-repair path was introduced. Exact scalar preservation requires retaining original bytes or resolving owner types before converting rank bytes into FormKeys.

Removal conditions: when replacing Synthesis or otherwise changing imports, verify that the replacement reader supplies external owner-type information and produces typed ownership before source snapshots are compared/copied. Removing Synthesis alone is insufficient if the replacement still invokes Mutagen without that information. Verify NPC/CONT/LVLI/LVLN ownership from local and master-defined factions, ordinary merging and Priority Mods, normal/light plugins, unchanged source data, missing-owner recovery, reader lifetimes and binary integer preservation. Existing/prior `PatchMod` and split inputs also need coverage when supported. Once those cases decode correctly without this workaround, remove this output normalization and its information log, retire the assumption-specific fixtures, and retain/update end-to-end import/write regressions. Keep ordinary reference, capacity and cycle validation. This workaround must not become a permanent general-purpose rank mask in a future importer.

COED provenance: pinned xEdit `Core/wbDefinitionsTES5.pas:2205–2212` and `Core/wbDefinitionsCommon.pas:5451–5470` (`wbCOEDOwnerDecider`); decompiled Mutagen 0.54.4 `ExtraDataBinaryCreateTranslation.GetBinaryOwner`, `UntypedOwnerCommon.EnumerateFormLinks`, `ExtraDataBinaryWriteTranslation.WriteBinaryOwnerCustom`, `RecordTypeInfoCacheReader`, and `BinaryReadParameters.LinkCache`. Synthesis 0.36.6 `States/DI/PatcherStateFactory` imports without supplying that LinkCache. In the installed Zoldyg replacer, Inigo `000D62:Inigo.esp` inventory rows 0/1/7/8/9 have raw COED owner `00000DB1` (PlayerFaction) and rank `FFFFFFFF` (-1). An unnormalized detached copy changed that rank to `02FFFFFF` (50331647); the source plugin itself has no missing record at that location. `OutputPatchValidatorTests.ExternalFactionRankSurvivesTypedImportAndOutputWorkaroundFixesUntypedCopy` verifies external-faction source binaries with negative ranks down to -8388607, zero and positive ranks up to 8388608 through type-aware reading and through the output workaround; an original -8388608 explicitly becomes positive 8388608 in the workaround while type-aware reading preserves it. `UntypedFactionRanksAreNormalizedAndSurviveBinaryOutput` covers all four COED-bearing types, normal/light masters, zero, positive/negative ranks including -10000 and -129, the sign boundary and the positive midpoint tie through binary output. Other fixtures cover unchanged typed negative/large ranks, genuine NPC-owned Global references, duplicate row counts/conditions, idempotent logging and an identical genuine broken script reference. Use path-based COED readers; the stream-overlay overload does not initialize its record-type cache.

Type-aware import investigation (2026-11-08, not an activated runtime path): opening the installed replacer with `BinaryReadParameters { LinkCache = bootstrapCache }`, copying Inigo and writing/reloading a temporary plugin preserved all five ranks at -1. A separate synthetic matrix preserved -1, 0, 1, 16777215, int.MinValue and int.MaxValue for each of NPC/CONT/LVLI/LVLN on both SkyrimSE and SkyrimVR (48 ownership cases). A public `SynthesisState` read-view prototype passed typed source overlays and a new context cache into the existing `RunPatchWithReport` with the same output object: both normal merging and Priority Mods selected an earlier -1 rank over a later 1, without omissions. Original source getters remained untyped and source files were unchanged. Detached output still wrote/reloaded after disposing the typed views. A missing external faction in a priority source remained a recovered record-local failure: the healthy actor survived, one record was skipped, and the run had no fatal error. This does not certify a complete modlist, desktop VFS configuration, localization, split inputs or gameplay.

The [read-only installed-plugin probe](../scripts/benchmarks/TypeAwareImport/Program.cs) repeats the real Inigo read/write and measures cold/warm owner lookup. Run `dotnet run -c Release --project scripts/benchmarks/TypeAwareImport -- "<Skyrim Data directory>" "<Inigo.esp path>" "<Zoldyg_inigo_replacer.esp path>"`. With five vanilla masters plus those two mods, opening the original overlays' headers took 35–53ms and about 0.25 MiB allocated; this excludes full record decoding. The first typed read/copy took 4.9–5.1s and about 2487 MiB allocated; subsequent fresh typed views sharing the warmed cache took 1.7–1.9ms and about 0.07 MiB. These are local Release samples and transient allocation totals. `RecordTypeInfoCacheReader.IsOfRecordType<INpcGetter>` first fails for the faction owner, then calls the generic record-existence resolver before allowing `GetBinaryOwner` to test the faction case. That generic call builds a broad input-record index. Merely rereading fewer files does not eliminate this cache cost. An owner-type lookup improvement upstream would need to avoid that broad fallback for ordinary valid faction ownership, while retaining missing-target handling.

Broader type-aware integration remains unimplemented: `SynthesisPipeline.Run` constructs `PatcherStateFactory` internally and has no read-parameter/import-factory hook in the pinned version. Existing overlays' parse packages are internal; assigning a later state cache cannot retroactively update the `RecordTypeInfoCacheReader`'s captured cache. Replacing `LoadOrder` listings after creating its immutable context cache also leaves old sources in that cache. A future cached typed-source adapter would need to cover both `RecordPolicySources.GetEligibleContexts` and `GetPriorityContext` before comparisons/copies, preserving source identity and full values without competing property handlers. A full typed read-view replacement was feasible experimentally but requires broader state/ownership bookkeeping. Original overlays/cache remain host-owned; additional read views must be separately disposed after copying values. Language/embedded encoding, filesystem/VFS, master remapping, policies and cutoff must remain intact; these import parameters are not all exposed by `IPatcherState`. Existing mutable `PatchMod` imported from `SourcePath` and split-file remapping also need consideration. The desktop starts with a fresh patch. This broader design is outside the approved output-only COED rank workaround.

Upstream reference checked on the investigation date: Synthesis development commit [`14cf3fa`](https://github.com/Mutagen-Modding/Synthesis/blob/14cf3fa045e72da68430fbcf7470e9552b0ea8ad/Mutagen.Bethesda.Synthesis/States/DI/PatcherStateFactory.cs) still imports without supplying the cache; Mutagen development commit [`b55de37`](https://github.com/Mutagen-Modding/Mutagen/blob/b55de3715229bca562f99baa3dcae5c001f812d7/Mutagen.Bethesda.Core/Plugins/Utility/RecordTypeInfoCacheReader.cs) still performs the generic existence fallback. This is source inspection, not evidence that any particular released upgrade fixes the issue. The local decompiled references remain read-only.

### Remaining output validation

LVLI/LVLN/LVSP counters are limited to 255 entries. Provenance: pinned xEdit `Core/wbDefinitionsTES5.pas:2196,8026–8073` defines LLCT as `itU8`; decompiled Mutagen 0.54.4 `LeveledItemBinaryWriteTranslation`, `LeveledNpcBinaryWriteTranslation`, and `LeveledSpellBinaryWriteTranslation` use `WriteWithCounter(..., 1)`. Iterative strongly-connected components detect cycles reachable from emitted leveled lists through unchanged input lists. Only output overrides participating in a cycle are omitted; existing input-only cycles are not repaired. Complete containing-record rejection also removes and reports its emitted descendants. Removal uses Mutagen's typed record-removal dispatch, not the Deleted flag. Remaining output is rechecked after omissions because input fallbacks can expose another invalid reference or cycle. Input winners are snapshotted once; output resolutions always use the current output dictionary.

Input lookup collects emitted reference keys and, when output contains leveled lists, every input leveled-list key needed for traversal. One priority-order scan of enabled input records retains only those winners, including incorrectly typed winners that must shadow older compatible versions. Ignored-for-merging plugins still participate in runtime reference resolution. Do not restore per-link `TryResolveSimpleContext` lookups: Mutagen builds separate lazy context indexes for broad getter interfaces, repeatedly enumerating and allocating overlay objects. Ordinary `TryResolve` also builds type-specific indexes. Diagnostics may still request record histories for rejected overrides only. xEdit's `xEdit/xeMainForm.pas:3374–3403` walks elements and calls `Check`; `Core/wbInterface.pas:19361+` checks contextual FormIDs and uses `RecordByFormID`; `Core/wbImplementation.pas:3604+,4881+` searches already indexed, sorted record arrays and follows master/override relationships. Its interactive checking time excludes building those indexes during plugin loading.

The opt-in [output-validation benchmark](../scripts/benchmarks/OutputValidation/Program.cs) uses five installed vanilla masters and 500 NPCs, 500 weapons and 500 leveled-item overrides. Run `dotnet run -c Release --project scripts/benchmarks/OutputValidation -- "<Skyrim Data directory>"`. It compares the former cold/warm resolver alone with the complete current validator using a fresh cache; imports, patch construction, merging and binary writing are outside timing. Local Release runs on 2026-11-08 resolved all 3069 unique links in 5.54–5.71s with the former cold context lookup (about 2548 MiB allocated); full current validation retained all 1500 overrides in 1.95–2.30s (about 2269 MiB allocated). These are transient allocation totals, not peak memory. This is a scoped real-plugin investigation, not certification of a complete modlist or a promise of xEdit-equivalent run times. Whole-input enumeration and rejected-record diagnostics can still cost time on large load orders.

Validation omissions use warnings and `PatchDiagnostics.OmitInvalidOutput`, adding `SkippedRecords` without manufactured processing errors. Each warning names the record, first detected failure, offending target/count/cycle and source records to inspect. Keep messages concise: identity, field, reason, source and necessary action. Do not repeat the omission or input-repair caveat in every footer; the public diagnostics documentation explains it. Record diagnostics are emitted at rejection; the final run summary reports the total without repeating each warning. Paths are best-effort diagnostics found only on rejection; generated enumeration determines validity. Source plugins containing the reference are evidence to investigate, not proof that every listed plugin authored the defect. Cycle warnings include contributing input plugins across the component and explain that otherwise valid sources can combine into a cycle. `LogCollector.AddWarning` attaches the active record FormKey when its caller has not supplied it; formatter warnings use that same path. Successful COED normalization and expected exterior-placement exclusions use `LogCollector.AddInfo`; unavailable interior destinations and specialized dependencies keep record-specific warnings. Regression coverage lives in `OutputPatchValidatorTests` and `InteriorReferencePlacementTests`. Forwarding fixtures explicitly define their ordinary reference targets rather than bypassing production validation.

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
