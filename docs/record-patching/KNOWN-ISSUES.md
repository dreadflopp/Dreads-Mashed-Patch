# Known issues and limits

[Patching guide](README.md) · [Record index](INDEX.md) · [Project front page](../../README.md)

Reviewed on 2026-10-05 against the current implementation with Mutagen Skyrim 0.54.4. The current version is still being tested. Some record types rarely conflict and have not had their conflict behavior tested in real modlists. Inspect all generated plugins in xEdit, make manual corrections, and report unexpected results through the repository's Issues tab.

## Coverage and comparison limits

| Area | Current boundary |
|---|---|
| PACK | Disabled by default. Even when enabled, PackageTemplate, DataInputVersion, Data, XnamMarker and ProcedureTree have no active property registration. Mutagen writes indexed data in key order; the binary regression preserves index/value associations, but gameplay impact remains unproven. Winner/priority copies still use that writer. |
| NAVM / NAVI | NAVM is disabled by default and selects whole geometry/connectivity data when enabled. There is no independent NAVI processing route or patcher-owned reconstruction. NAVM and REFR navigation-door-link forwarding do not establish navigation-map consistency. |
| DIAL / DLBR / INFO / DLVW | Disabled by default. Enabling them permits the documented property rules but can combine coupled data from different authors; validation does not establish dialogue correctness. |
| ARMA | All four model filename handlers copy associated data bytes but ignore byte-only differences. This is an explicit retained policy. Alternate textures merge separately. |
| Ordinary script-list VMAD | Scripts merge by name as complete rows. Version/ObjectFormat remain destination-owned, so metadata-only edits are not independently forwarded. INFO/PACK/PERK whole adapters and QUST/SCEN explicit child fields have different policies. |
| Null/default writes | Setters have property-specific normalization. Shared post-write validation checks non-null values only. The repaired keyword setter preserves absent versus present-empty lists, but there is no universal null-removal guarantee. |
| Omitted and opaque data | Unregistered fields are not independently reconciled. Copying and writing can change representation; complete byte preservation and gameplay semantics of every opaque payload are not established. See [unsupported records](unsupported.md) and the [inventory](INVENTORY.md). |
| Coverage audits | Passing registration and collection audits does not establish full nested equality, correct output for every combination, or gameplay safety. Audits include dormant handlers. |

The [coverage/comparison audit](COVERAGE-COMPARISON-AUDIT.md) preserves the original findings and completed keyword/PHZD/audit fixes. PACK graph enablement, NAVI reconstruction and ARMA byte-only comparison retain their existing policies; no safe new implementation is claimed by these documents.

## Verification scope

Dated [review evidence](REVIEW.md) records synthetic load-order tests, binary-overlay round trips, localized string checks, builds, and audits. The later [disk-backed transaction verification](REVIEW.md#disk-backed-transaction-verification) also records a successful real-load-order run and binary reload through a temporary native Linux harness using the desktop runner services. That run does not certify every record or verify Proton desktop patching, xEdit review, or in-game behavior. The separate Proton UI smoke test covers startup and General-tab rendering only.

A successful run means the implemented checks passed. It does not replace manual conflict review or additional compatibility patches.

## Resolved defects

These sections retain link targets used by the record tables. Full migration decisions, regressions and historical validation counts are in [REVIEW.md](REVIEW.md); they are not still-open defects.

### Record-header flags

Fixed on 2026-10-04. All 122 concrete handlers use one approved composite `MajorRecordFlagsRawHandler`; overlapping header registrations and OR-based application were removed. Selected clears cannot be reintroduced by another header view, and unowned bits remain winner-owned. ACHR/REFR UDR coordination reads the integer flag context. [Completion evidence](REVIEW.md#record-header-migration-verification).

### Container item extra data

Fixed on 2026-10-04. CONT/NPC_ share one inventory implementation retaining complete detached rows, COED presence/condition, and polymorphic owner data. Duplicate matching and permissions preserve metadata during count forwarding; CONT retains its separate count policy. [Completion evidence](REVIEW.md#container-inventory-migration-verification).

### Script-adapter metadata

Fixed on 2026-10-04. Ordinary script-list replacement preserves destination Version/ObjectFormat, including present-empty adapters. Creating an adapter without a destination uses Mutagen defaults (5/2). Metadata-only changes remain outside independent forwarding on this path. [Completion evidence](REVIEW.md#script-adapter-metadata-verification).

### Policies skipped by the initial filter

Fixed on 2026-10-05. Eligible priority snapshots and preserved baseline EditorIDs reach shared policies before ordinary vanilla/short-history exits. Typed identifier caches retain the initial at-most-three-context filter; ordinary merging still resolves full histories. Priority order, ignores, disabled types and Tamriel's specialized CELL policy retain their documented behavior. Index setup/memory costs have not been benchmarked across real modlists. [Completion evidence](REVIEW.md#filter-policy-verification).

### Optional name translations

Fixed on 2026-10-04. Translated names retain the selected target language and all available translations in detached generated copies. Equality follows Mutagen's configured language comparison. Required CLAS/EYES/FLOR/KEYM null names become empty; optional names permit removal. MATT remains a plain string. [Completion evidence](REVIEW.md#name-translation-migration-verification).

### Errors can leave partial overrides

Fixed on 2026-10-04. Every output path stages record writes and required ancestry before publication. Failed candidates are discarded, structured errors survive log flushing, and failed runs withhold primary/split file replacement. Non-null semantic write validation and record-specific validation retain their documented boundaries. [Completion evidence](REVIEW.md#partial-override-fix-verification).

### Disk-backed override transactions and diagnostic text

Fixed on 2026-10-05. Group lookup uses the registered getter type instead of a binary-overlay runtime type. Diagnostic severity requires a prefix, so embedded strings such as `<Error: Unknown lstring ID ...>` do not falsely fail a run. This does not repair missing localization strings in source plugins. [Completion evidence and real-load-order check](REVIEW.md#disk-backed-transaction-verification).

### Keyword removal and PHZD coverage

Fixed on 2026-10-05. All eighteen shared keyword-setter registrations apply selected absence, retain present-empty lists, and copy detached links. PHZD registers all eighteen inherited placed fields through existing handlers; UDR remains scoped to ACHR/REFR. The coverage audit now scans inherited record/aspect fields and classifies typed GLOB discriminators. [Completion evidence](REVIEW.md#coverage-fix-verification).

## Re-evaluation and verification

The earlier MGEF concern about additional copied subtype members was not substantiated for Mutagen 0.54.4. Archetype comparison uses Type, ActorValue and AssociationKey; concrete subtypes add a typed Association already represented by AssociationKey. No handler change is justified without a concrete counterexample. Recheck this assumption when upgrading Mutagen.

Historical pre-fix probes and counts remain in the [review evidence](REVIEW.md) and [original coverage audit](COVERAGE-COMPARISON-AUDIT.md). Use the current guides and regenerated reports to assess current registrations.
