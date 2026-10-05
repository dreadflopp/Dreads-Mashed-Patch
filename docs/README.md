# Documentation

[Project front page](../README.md)

## Using the patcher

| Document | Purpose |
|---|---|
| [Application guide](STANDALONE_APP.md) | Windows/Proton setup, mod managers, settings, logs, and build/publish instructions. |
| [Patching guide](record-patching/README.md) | Input filtering, ownership, selection, shared property rules, and output failures. |
| [Record index and property tables](record-patching/INDEX.md) | Every supported record variant and how its properties are patched. |
| [Unsupported records](record-patching/unsupported.md) | Excluded and missing routes, plus indirect handling. |
| [Known issues and limits](record-patching/KNOWN-ISSUES.md) | Current boundaries and links to resolved defect evidence. |

The current release is still being tested. Review the output in xEdit and correct it manually before use; report unexpected patching through the repository's Issues tab.

## Developing and verifying

| Document | Purpose |
|---|---|
| [Development guide](DEVELOPMENT.md) | Architecture, handler selection, copying, error handling, and validation commands. |
| [Collection semantics](COLLECTION_SEMANTICS.md) | Collection modes, reviewed xEdit expectations, and regeneration/verification workflow. |
| [Ordered list alignment](LIST_ALIGNMENT.md) | Active sequence alignment and ownership rules, with retired algorithms identified as history. |
| [External reference sources](REFERENCE_SOURCES.md) | Regenerate the ignored Mutagen and pinned xEdit references. |
| [Record/interface mapping](record-code-interface-mapping.md) | Bethesda signatures, Mutagen interfaces, and actual dispatch routes. |
| [Record inventory](record-patching/INVENTORY.md) | Complete implementation matrix and independent-decision exclusions. |
| [Property coverage report](record-handler-coverage.md) | Generated registration audit; machine-readable companion: [JSON](record-handler-coverage.json). |
| [Collection comparison report](collection-semantics-comparison.md) | Generated runtime modes versus reviewed expectations; [manifest](collection-semantics-manifest.json) and [expectations](xedit-collection-expectations.json). |

Registration coverage is evidence of implementation scope, not proof of gameplay correctness. The static coverage report also detects the commented-out IDLE RelatedIdles registration; it is not active. Dormant handlers appear in audits; the supported catalog, configuration, and actual dispatch determine which run.

## Evidence and history

[Review evidence](record-patching/REVIEW.md) and the [coverage/comparison audit](record-patching/COVERAGE-COMPARISON-AUDIT.md) retain dated findings, fix decisions, and validation results. Earlier counts and pre-fix observations apply to their stated historical baseline. Current behavior is described by the patching guide, inventory, and known-limits page.

[Migration history](history/README.md) preserves record-specific rationale for asset paths, model/bounds coupling, and list ordering. The [font documentation](../DreadsMashedPatch.App/Assets/Fonts/README.md) and [SIL license](../DreadsMashedPatch.App/Assets/Fonts/OFL.txt) remain alongside the bundled assets. [AGENTS.md](../AGENTS.md) contains repository contribution instructions.

## Documentation maintenance

Reviewed on 2026-10-05 against the checked-in implementation and Mutagen Skyrim 0.54.4 / Synthesis 0.36.6. Keep current guides and generated audits, preserve dated decision evidence, and remove superseded guidance. The former `DreadsMashedPatch/docs/best practices.md` was replaced by the development guide: its asset-path, translation, interface, and omission examples no longer matched current policy. The generated `ilspy-help.txt` snapshot was removed; use the exporter and installed tool's `--help` instead.

When behavior changes, update the application guide, shared rules, affected record tables, inventory, and known limits together. Regenerate relevant audits and record validation evidence. Preserve historical test counts as historical results; do not present them as current release certification.
