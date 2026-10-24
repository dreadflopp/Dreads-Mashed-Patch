# Retained implementation references

[Internal index](../README.md) · [Collection workflow](../COLLECTION_SEMANTICS.md)

These files retain decisions that help agents avoid regressions, not completed
migration task lists. The existing paths remain stable because collection
expectations cite the ordering references as provenance.

| Reference | Useful information retained |
|---|---|
| [Asset copying](ASSET_PATH_MIGRATION.md) | Serialized GivenPath, overlay copying and record-specific asset boundaries. |
| [Model and bounds](MODEL_BOUNDS_MIGRATION.md) | Conditional geometry/bounds ownership and exceptions. |
| [xEdit ordering evidence](LIST_ORDERING_AUDIT.md) | Pinned source constructs, keys and non-obvious array definitions. |
| [Collection decisions](LIST_ORDERING_MIGRATION.md) | Shared versus specialized collection choices and reasons. |

Use current [record registrations/inventory](../record-patching/INVENTORY.md),
[collection expectations](../xedit-collection-expectations.json) and
[alignment implementation](../LIST_ALIGNMENT.md) when changing a handler. The
reference evidence does not enable dormant code or authorize a policy change.
