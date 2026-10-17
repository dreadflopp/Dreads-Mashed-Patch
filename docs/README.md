# Internal documentation

For the maintainer and AI agents working on the app. The [project README](../README.md) is the user guide for setup, settings, and diagnostics. Keep this index and internal documents out of public documentation navigation.

[Project front page](../README.md)

## Public documentation to maintain

| Document | Purpose |
|---|---|
| [Project README](../README.md) | Downloads, setup, concise settings guidance, mod managers, Proton, and logs. |
| [Shared patching rules](record-patching/README.md) | Value selection and terminology used by the record property tables. |
| [Record index and property tables](record-patching/INDEX.md) | Every supported record variant and how its properties are patched. |
| [Unsupported records](record-patching/unsupported.md) | Excluded and missing routes, plus indirect handling. |

VOLI, LENS, LTEX and all seven placed-projectile variants are supported and enabled by default. Their atomic groups, nullable fields and collection rules are documented in the [record tables](record-patching/INDEX.md); source review and binary test evidence are in [the completion appendix](record-patching/REVIEW.md#additional-record-support-verification).

The current release is still being tested. Review the output in xEdit and correct it manually before use; report unexpected patching through the repository's Issues tab.

NPC_, RACE and HDPT select complete atomic appearance groups through ordinary change/reversion rules. [Appearance selection evidence](record-patching/REVIEW.md#atomic-appearance-forwarding--2026-10-17) records the correction. TREE remains enabled; its **Restrict tree forwarding** checkbox defaults to preserving winning appearance, scripts and flags. [TREE migration and validation evidence](record-patching/REVIEW.md#tree-restriction-checkbox--2026-10-17) records that boundary.
The [public TREE rules](record-patching/world.md#tree-tree) explain the behavior and practical limits; [TREE policy design](DEVELOPMENT.md#tree-smart-forwarding-design) documents its implementation rationale.

## Developer documentation

| Document | Purpose |
|---|---|
| [Standalone development](STANDALONE_DEVELOPMENT.md) | Build/publish commands, desktop implementation, typography, and migration notes. |
| [Development guide](DEVELOPMENT.md) | Architecture, handler selection, copying, error handling, and validation commands. |
| [Collection semantics](COLLECTION_SEMANTICS.md) | Collection modes, reviewed xEdit expectations, and regeneration/verification workflow. |
| [Ordered list alignment](LIST_ALIGNMENT.md) | Active sequence alignment and ownership rules, with retired algorithms identified as history. |
| [External reference sources](REFERENCE_SOURCES.md) | Regenerate the ignored Mutagen and pinned xEdit references. |
| [Record/interface mapping](record-code-interface-mapping.md) | Bethesda signatures, Mutagen interfaces, and actual dispatch routes. |
| [Record inventory](record-patching/INVENTORY.md) | Complete implementation matrix and independent-decision exclusions. |
| [Property coverage report](record-handler-coverage.md) | Generated registration audit; machine-readable companion: [JSON](record-handler-coverage.json). |
| [Collection comparison report](collection-semantics-comparison.md) | Generated runtime modes versus reviewed expectations; [manifest](collection-semantics-manifest.json) and [expectations](xedit-collection-expectations.json). |

Registration coverage is evidence of implementation scope, not proof of gameplay correctness. The commented-out IDLE RelatedIdles registration is not active and is excluded by the static scanner. Dormant handlers appear in audits; the supported catalog, configuration, and actual dispatch determine which run.

## Internal evidence and history

[Windows and Proton UI styling review (2026-10-17)](UI-STYLING-REVIEW-2026-10-17.md)
records isolated rendering evidence, proposed compact path help, native-theme state issues,
and tab/editor layout findings. It proposes changes; production code was not changed by the review.

[User-reported failure investigation (2026-10-10)](USER-REPORTED-FAILURES-2026-10-10.md)
records NPC Perks and GLOB/GMST cast reproductions, implemented repairs,
default partial publication, regression results, and the limits of validation
against the users' plugins.

[Known issues and limits](record-patching/KNOWN-ISSUES.md), [review evidence](record-patching/REVIEW.md), and the [coverage/comparison audit](record-patching/COVERAGE-COMPARISON-AUDIT.md) retain internal boundaries, dated findings, fix decisions, and validation results. Earlier counts and pre-fix observations apply to their stated historical baseline. Current user-facing limitations belong in the affected record descriptions.

[Migration history](history/README.md) preserves record-specific rationale for asset paths, model/bounds coupling, and list ordering. System font fallback and rendering decisions are documented in [Standalone development](STANDALONE_DEVELOPMENT.md#typography-on-windows-and-proton). [AGENTS.md](../AGENTS.md) contains repository contribution instructions.

## Documentation maintenance

Reviewed on 2026-10-05 against the checked-in implementation and Mutagen Skyrim 0.54.4 / Synthesis 0.36.6. Keep current guides and generated audits, preserve dated decision evidence, and remove superseded guidance. The former `DreadsMashedPatch/docs/best practices.md` was replaced by the development guide: its asset-path, translation, interface, and omission examples no longer matched current policy. The generated `ilspy-help.txt` snapshot was removed; use the exporter and installed tool's `--help` instead.

When behavior changes, update shared rules, affected record tables, inventory, and internal known limits together. Update the project README only when downloads, setup, concise settings guidance, or diagnostics change; link to public record descriptions rather than adding detailed forwarding policies, property groups, or rationale there. Implementation details and migration/validation evidence belong in internal documentation. This is a standing documentation rule, also recorded in [AGENTS.md](../AGENTS.md).

Regenerate relevant audits and record validation evidence when implementation changes require them. Preserve historical test counts as historical results; do not present them as current release certification.
