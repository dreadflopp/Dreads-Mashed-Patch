# Internal documentation for AI agents

Start with [AGENTS.md](../AGENTS.md) and the [development guide](DEVELOPMENT.md).
Read the affected record's registrations and tests before changing behavior.
These documents explain current invariants, reference evidence and known limits;
source code and executable tests determine actual behavior.

## Task navigation

| Task | Read |
|---|---|
| Architecture, handler choice, copying, errors, validation | [Development guide](DEVELOPMENT.md) |
| Record dispatch, supported and dormant variants | [Interface mapping](record-code-interface-mapping.md), [record inventory](record-patching/INVENTORY.md) |
| Property ownership, grouping, exclusions and exceptions | [Record inventory](record-patching/INVENTORY.md), then the linked handler |
| Find regressions and implementation decisions to preserve | [Regression reference](record-patching/REVIEW.md) |
| Unresolved behavior, writer limits and validation gaps | [Known limits](record-patching/KNOWN-ISSUES.md) |
| Collection mode, key, presence or duplicate behavior | [Collection workflow](COLLECTION_SEMANTICS.md), [alignment algorithm](LIST_ALIGNMENT.md), [source decisions](history/README.md) |
| Exact Mutagen/xEdit member and writer surfaces | [Reference exporters](REFERENCE_SOURCES.md) |
| Desktop build, publish, WPF and Proton constraints | [Standalone development](STANDALONE_DEVELOPMENT.md) |
| NPC perk-write or GLOB/GMST cast failures | [Failure diagnosis](USER-REPORTED-FAILURES-2026-10-10.md) |
| Check registration coverage | [Generated property report](record-handler-coverage.md), [JSON](record-handler-coverage.json) |
| Check collection drift | [Generated comparison](collection-semantics-comparison.md), [manifest](collection-semantics-manifest.json), [independent expectations](xedit-collection-expectations.json) |

Generated audits include constructible dormant handlers. Passing coverage does
not establish active dispatch, full nested comparison, binary preservation or
gameplay correctness. Use current reports for counts rather than copying them
into several guides.

## Public documentation to maintain

The [root README](../README.md) covers installation, setup, concise settings and
diagnostics. Its links to [shared patching rules](record-patching/README.md),
[record pages](record-patching/INDEX.md) and [unsupported records](record-patching/unsupported.md)
are the public behavior documentation. Keep internal indexes, guides, audits,
limits and regression notes out of that public navigation.

## Maintenance policy

Update current rules where agents will look for them. Keep completed migration
notes only when they explain a non-obvious invariant, a rejected approach that
would recreate a bug, exact reference provenance or a useful regression entry
point. Consolidate that information into current guides or the regression
reference; remove completed task plans, repeated checklists, superseded proposals,
old aggregate test counts and machine-specific run transcripts. A migration does
not need a permanent standalone document merely because it was completed.

Preserve concise per-record decisions required by AGENTS.md. Record validation
scope honestly: synthetic fixtures, binary round trips, real-plugin processing
and in-game checks establish different things. A dated result is evidence for
that run, not certification of later changes. Recheck dependencies and regenerate
the relevant audits when implementation changes; do not manually edit generated
reports or turn a passing audit into a gameplay claim.
