# Known issues and limits

[Patching guide](README.md) · [Record index](INDEX.md)

Re-evaluated against source baseline `6fa7d0e` on 2026-10-04 with Mutagen Skyrim 0.54.4, then updated after the record-header, container-inventory, script-adapter metadata, name-translation and partial-override fixes on the same date. Those five issues are resolved; policy filtering was subsequently fixed on 2026-10-05 using targeted identifier lookups. Coverage/comparison limits were separately audited on 2026-10-05 against the current working tree; that initial documentation-only audit confirmed the keyword-removal defect and clarified the other boundaries. Keyword removal, all eighteen PHZD inherited fields and the audit’s inherited-field blind spot were fixed later on 2026-10-05; PACK, NAVI and ARMA retain their existing policies. Verification uses synthetic load orders and binary weapon/container round trips (including VMAD object formats 1 and 2) and localized BOOK string-table round trips; no real game load order or in-game result was tested.

## Record-header flags

**Fixed on 2026-10-04.** All 122 concrete handlers now use the approved composite `MajorRecordFlagsRawHandler` as their sole header implementation. It owns the existing base bits, common Skyrim bits and each applicable record-specific enum (including inherited GLOB/PHZD bits and NAVM's unsigned bit 31). Bits outside that mask remain winner-owned.

The former raw/common/type-specific registrations and OR-based application path have been removed. A selected clear therefore cannot be reintroduced by another view of the same header. ACHR/REFR's UDR coordinator now reads InitiallyDisabled ownership from the integer flag context, preserving coherent safe-disable and restoration bundles.

Evidence: [composite flags](../../DreadsMashedPatch/PropertyHandlers/General/MajorRecordFlagsRawHandler.cs), [shared application](../../DreadsMashedPatch/RecordHandlers/Abstracts/AbstractRecordHandler.cs), [UDR ownership](../../DreadsMashedPatch/RecordHandlers/PlacedReferenceUdrCoordinator.cs), [header regressions](../../DreadsMashedPatch.Tests/RecordHeaderFlagsTests.cs).

**Verification:** 49 new regressions cover all handler registrations, every configured typed bit, sets/clears, authorized and denied reversions, the original `0x800 → 0 → 0x800` full-run failure, simultaneous base/common/typed changes, unknown winner bits, binary weapon output and ACHR/REFR UDR/restoration. See [completion evidence](REVIEW.md#record-header-migration-verification). Other property handlers and the remaining issues below are outside this header fix.

## Container item extra data

**Fixed on 2026-10-04.** CONT Items now retain complete generated `ContainerEntry` copies in source snapshots, forward contexts and output rows, including nullable `Data` (COED) and polymorphic Owner. Count forwarding no longer removes unchanged ownership/condition data. Final equality compares count, COED presence, condition (0.001 tolerance) and every owner-variant field, retaining duplicate multiplicity.

CONT and NPC_ use one shared inventory implementation derived from the existing NPC reconciliation. Duplicate rows reserve unchanged matches before matching remaining occurrences by metadata-change cost, and are detached before mutation. COED addition/removal, condition edits and coherent owner replacement require the row owner's declared/virtual-master permission. CONT keeps its existing count policy: newly different counts may forward independently, while baseline count reversions require permission. Count ownership is updated after COED reconciliation, so a count edit cannot grant permission for a COED reversion in the same row.

Evidence: [shared inventory implementation](../../DreadsMashedPatch/PropertyHandlers/Abstracts/AbstractInventoryItemsHandler.cs), [CONT adapter and count policy](../../DreadsMashedPatch/PropertyHandlers/Container/ItemHandler.cs), [NPC adapter](../../DreadsMashedPatch/PropertyHandlers/Npc/ItemHandler.cs), [container regressions](../../DreadsMashedPatch.Tests/ContainerItemExtraDataTests.cs).

**Verification:** 33 new regressions pass in the full 512-test suite. Full-run regressions cover the original `1 → 2 → 1` failure, COED-only edits, authorized and denied removal/restoration, duplicate keys and count-policy preservation. Snapshot/output isolation covers both CONT and NPC_ with all three owner variants. Binary round trips check complete owners, condition, absent/default-present COED and duplicate payloads. See [completion evidence](REVIEW.md#container-inventory-migration-verification). LVLI and other property handlers retain their separate policies.

## Script-adapter metadata

**Fixed on 2026-10-04.** The shared ordinary list-style VMAD setter now preserves the destination adapter's Version/ObjectFormat when replacing scripts, including present-empty adapters. Creating an adapter without a destination still uses Mutagen defaults (5/2). The original full-run `4/1 → 5/2` reset is covered by a regression and now retains `4/1`.

Metadata remains destination/winner-owned on this path: the selected value/context contains scripts, so Version/ObjectFormat are not independently forwarded and metadata-only differences do not create an override. Script-name identity, complete-row ownership and Papyrus unused-data copying retain their existing implementation. Null/empty behavior is unchanged, including MGEF's specialized null setter that creates a default empty adapter. INFO/PACK/PERK whole adapters and QUST/SCEN explicit metadata fields retain their separate policies.

Evidence: [shared adapter setter](../../DreadsMashedPatch/PropertyHandlers/Abstracts/AbstractVirtualMachineAdapterHandler.cs), [metadata regressions](../../DreadsMashedPatch.Tests/ScriptAdapterMetadataTests.cs).

**Verification:** 28 new regressions cover all 20 ordinary adapter registrations, destination metadata, creation defaults, absent/present-empty adapters, detached script copies, unused-data copying and full-run permission-controlled restoration. Binary weapon round trips verify formats 1 and 2 with object properties/arrays, FormKeys, aliases and unused data, plus absent/present-empty VMAD. See [completion evidence](REVIEW.md#script-adapter-metadata-verification). No real game load order or in-game behavior was tested.

## Policies skipped by the initial filter

**Fixed on 2026-10-05.** Records with an eligible always-win source reach the shared policy before vanilla-winner, short-history and preceding-vanilla exits. The last matching configured priority entry remains authoritative; an already-winning source needs no override. Ignored plugins, disabled record families and absent/non-editing priority sources do not create policy overrides.

Policy checks retain the initial **at-most-three eligible contexts**. Run-scoped, typed identifier caches search only configured priority plugins and, when PreserveBaseline is enabled, eligible official plugins. Baseline resolution selects the latest official version in load order, including configured Creation Club membership, and distinguishes a present null EditorID from an absent baseline. No complete record history is expanded for these lookups. Identifier indexes are populated lazily for queried record types; they still have setup and memory costs, which have not been benchmarked on a real load order.

An otherwise skipped record with a different baseline EditorID uses the existing transaction to copy the effective winner and apply only EditorID through its registered handler. Priority handling resolves the selected plugin's context only when copying its complete record. Ordinary merging still resolves full histories for records needing merge analysis; Tamriel's specialized CELL path retains its history and child-excluding copy rules. Priority snapshots take precedence over baseline EditorID and Tamriel policies.

Evidence: [policy source lookups](../../DreadsMashedPatch/RecordPolicySources.cs), [initial filter and dispatch](../../DreadsMashedPatch/Program.cs), [shared processing](../../DreadsMashedPatch/RecordHandlers/Abstracts/AbstractRecordHandler.cs), [policy regressions](../../DreadsMashedPatch.Tests/PolicyFilterTests.cs). See [completion evidence](REVIEW.md#filter-policy-verification).

## Optional name translations

**Fixed on 2026-10-04.** All 38 translated Name registrations formerly using the string-based handler now retain an `ITranslatedStringGetter` throughout selection and use the existing generated-copy translated-string handler. Writing a selected name preserves its target language and every available translation in a detached copy. The original BOOK English/French loss is covered through full `Program.RunPatch` selection and a localized binary/overlay round trip.

Names remain complete values from one source, with ordinary permission-controlled reversions. Comparison follows `TranslatedString.DefaultLanguageComparisonOnly`: default-language mode compares the target language and its text, while all-language mode also detects translation-only edits and language additions/removals. These Mutagen comparers do not trim text. Comparison controls selection, not which languages are copied. Optional null names remove the value; required CLAS/EYES/FLOR/KEYM null names become empty using Mutagen's configured default language. MATT's plain-string Name uses the existing scalar reflection handler and retains normalized comparison/removal.

The old string-based name implementation has been removed, including all 39 registrations and its direct test usage. Existing translated-name registrations and other property/flag policies retain their implementation.

Evidence: [translated-string handler](../../DreadsMashedPatch/PropertyHandlers/General/TranslatedStringReflectionPropertyHandler.cs), [name regressions](../../DreadsMashedPatch.Tests/NameTranslationTests.cs), [completion evidence](REVIEW.md#name-translation-migration-verification).

**Verification:** 52 new regressions cover every migrated translated registration, detached multilingual output, non-English target/default languages, required/optional null behavior, MATT plain strings, both comparison modes, permission-controlled full-run forwarding/removal/restoration, and localized BOOK source/output string tables. No real game load order or in-game behavior was tested.

## Errors can leave partial overrides

**Fixed on 2026-10-04.** All output paths now use shared `CommitOverride` staging, including ordinary property forwarding, priority snapshots, NPC_ and Tamriel CELL policies. Mutagen's context insertion runs in an isolated temporary mod, retaining its nested placement and child-excluding copy rules. The target is detached; mutable ancestor headers and collection containers are copied before insertion. Publishing replaces only the completed root record or interior CELL block. Failed setters, record validation and insertion cannot leave a new override or altered ancestry in PatchMod; previously successful overrides and siblings remain intact.

Setter exceptions now stop application and fail the candidate. Caught handler errors use structured diagnostics, including reflection errors and malformed condition/response copies. Non-null selected values are checked with the registered handler's semantic comparer after writing. Existing null/default normalization and specialized QUST/SCEN/IPCT validation remain in force. Warnings and supported fallback diagnostics do not by themselves fail a run.

`Process` and `Program.RunPatchWithReport` return a `PatchRunReport` containing record keys, stages, messages and exceptions. Errors survive log flushing. Other records continue processing for diagnostics, but `Program.RunPatch` rejects the completed run if any errors occurred. The desktop runner also requires a successful structured report and zero pipeline log errors before committing primary/split files; a failed run preserves the previous output and removes its staging directory.

Evidence: [record staging](../../DreadsMashedPatch/RecordOverrideTransaction.cs), [shared processing/application](../../DreadsMashedPatch/RecordHandlers/Abstracts/AbstractRecordHandler.cs), [structured report](../../DreadsMashedPatch/PatchRunReport.cs), [runner](../../DreadsMashedPatch.App/Services/PatcherRunner.cs), [file transaction](../../DreadsMashedPatch/PatchOutputTransaction.cs), [regressions](../../DreadsMashedPatch.Tests/RecordOverrideTransactionTests.cs).

**Verification:** 25 new regression cases cover failures before/after mutation, an earlier successful setter, internally logged and flushed errors, reflection setter errors, nonfatal recovered warnings, silent non-null write failures, record validation failures, continued processing, NPC insertion, interior/top/exterior CELL and INFO ancestry, preservation of existing parent/child overrides, binary writing of successful nested output, insertion failing after parent creation, structured full-run errors, run isolation and withheld primary/split file replacement. See [completion evidence](REVIEW.md#partial-override-fix-verification). No real game load order or in-game behavior was tested.

## Disk-backed override transactions and diagnostic text

**Fixed on 2026-10-05.** The failed desktop run reported 1,903 errors: 1,901 transaction failures across thirty root record types, plus two property trace lines misclassified as errors. Transaction lookup passed binary-overlay runtime types (for example `NpcBinaryOverlay` and `WorldspaceBinaryOverlay`) to Mutagen's generated top-level group dispatch, which accepts registered record/interface types. Lookup now uses `Registration.GetterType`, following the existing policy-source and record-catalog pattern. Both staged and output groups use the same type; ancestry staging and failure rejection retain their implementation.

Severity detection now requires a diagnostic prefix, optionally after a property tag. Embedded values such as `<Error: Unknown lstring ID 00000011>` no longer create structured failures. The desktop log writer uses the same classifier, while retaining its pipeline exception/failure prefixes. Actual errors remain fatal; this change does not repair missing source localization strings.

Seven binary-overlay regressions cover WEAP, NPC_, WRLD, interior/top/exterior placed references and INFO: first and subsequent commits, preserved parent/sibling edits, rollback after a failed setter, unchanged sources and binary output reload. Three diagnostic-value cases and an expanded actual-error theory verify the collector and desktop writer. No properties, approved flag handlers or record-specific policies were migrated; no obsolete record classes were introduced or removed. See [verification](REVIEW.md#disk-backed-transaction-verification).

## Coverage and comparison limits

**Keyword removal, PHZD coverage and inherited-field auditing fixed on 2026-10-05.** The [audit report](COVERAGE-COMPARISON-AUDIT.md) preserves the original findings and records the fixes; [completion evidence](REVIEW.md#coverage-fix-verification) gives migration decisions and validation.

| Area | Current status |
|---|---|
| Keywords | **Fixed:** all eighteen shared-setter registrations write selected null as absence and retain present-empty lists. Lists and links are detached on output. Full-run permission and binary tests cover every registration, including WEAP’s specialized policy. |
| PHZD | **Fixed:** all eighteen inherited placement, script, link, scalar, aggregate, collection and XIS2 fields are registered using existing handlers. LinkedReferences and Reflections use reviewed xEdit keys; LocationRefTypes retains order; DistantLodData is one whole fixed value. Approved header handling is unchanged. UDR coordination remains scoped to ACHR/REFR. |
| Coverage audit | **Fixed:** inherited record/aspect fields are scanned, with common identity/version/runtime interfaces excluded. Typed GLOB TypeChar discriminators have explicit read-only classifications. The old PHZD registration fails strict verification with eighteen missing candidates. |
| PACK | Template graph remains deliberately unregistered. Mutagen 0.54.4 sorts indexed data by key; the existing binary regression preserves index/value associations. Gameplay impact is unproven, and winner/priority copies still use this writer. |
| ARMA | All four model filename handlers copy associated bytes but ignore byte-only differences. This explicit policy is retained. |
| NAVI | No independent query, handler or patcher-owned rebuild exists. NAVM and REFR navigation-door links do not establish navigation-map consistency. |

The expanded coverage audit passes for **122 handlers, zero missing/partial candidates or stale overrides**. The collection audit matches **173 runtime registrations and expectations**. Registration coverage still does not establish complete nested equality, successful normalization for every null setter, gameplay safety or complete on-disk preservation. Shared post-write validation remains limited to non-null values; the keyword setter now correctly implements its nullable surface.

The confirmed defects are resolved. By the user’s scope decision, PACK ordering/graph enablement and NAVI reconstruction remain separate future work, and ARMA’s model-data policy stays in force. Reasons for other unsupported routes, opaque payload semantics and complete on-disk preservation remain **uncertain**. See the [record tables](INDEX.md) and [unsupported list](unsupported.md).

## Re-evaluation and verification

The re-evaluation below predates the header, container, script-adapter metadata, name-translation and partial-override fixes. Current migration validation is recorded in [REVIEW.md](REVIEW.md#record-header-migration-verification), the [container completion evidence](REVIEW.md#container-inventory-migration-verification), the [script-adapter completion evidence](REVIEW.md#script-adapter-metadata-verification), the [name-translation completion evidence](REVIEW.md#name-translation-migration-verification) the [partial-override completion evidence](REVIEW.md#partial-override-fix-verification) and the later [coverage-fix completion evidence](REVIEW.md#coverage-fix-verification).

The previous MGEF warning about additional copied subtype members is **not substantiated for Mutagen 0.54.4**. Archetype comparison does use Type, ActorValue and AssociationKey, but the current concrete subtypes add only a typed Association, which AssociationKey already represents. The decompiled getter and implementation surfaces show no additional semantic subtype fields. No archetype handler change is proposed without a concrete counterexample; re-audit this assumption when upgrading Mutagen.

- Pre-migration test suite: **430 passed, zero failed**, using `dotnet test DreadsMashedPatch.Tests/DreadsMashedPatch.Tests.csproj --no-restore --verbosity minimal` (Debug). This builds the core and test projects, including the desktop runner services linked into the test project; it is not a full desktop UI build.
- Temporary probes outside the repository: **13 checks passed**. They re-ran the first five findings through `Program.RunPatch` or the direct Name setter, and checked null keywords, ARMA byte-only equality, PHZD/PACK registration gaps, all 11 current concrete MGEF archetypes, and injected property/record failures leaving mutated overrides. These inspect in-memory output; they do not establish binary or gameplay outcomes. Desktop commit-after-caught-errors and absent NAVI processing were confirmed by source inspection.
- Future migrations must update all call sites, remove superseded handlers/usings, confirm no obsolete references remain, and add record-specific decision notes identifying migrated properties, intentional exclusions and their reasons. Run relevant diagnostics/builds and regression tests before marking any issue fixed.
