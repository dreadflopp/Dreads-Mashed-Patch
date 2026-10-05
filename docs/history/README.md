# Migration history

[Documentation index](../README.md)

These notes preserve decisions and cleanup evidence. Current registrations and exclusions are in the [record tables](../record-patching/INDEX.md) and [inventory](../record-patching/INVENTORY.md). Historical recommendations do not enable dormant handlers or override current project policy.

| Note | Why it is retained |
|---|---|
| [Asset paths](ASSET_PATH_MIGRATION.md) | Record-specific copying decisions and preservation of serialized `GivenPath`. |
| [Model and bounds](MODEL_BOUNDS_MIGRATION.md) | Which model-bearing records share geometry/bounds ownership and which fields remain independent. |
| [Ordering audit](LIST_ORDERING_AUDIT.md) | Pinned xEdit evidence behind collection classifications. |
| [Ordering migration](LIST_ORDERING_MIGRATION.md) | Resulting shared modes, specialized exceptions, and removed implementations. |

The active [collection workflow](../COLLECTION_SEMANTICS.md) and [alignment algorithm](../LIST_ALIGNMENT.md) are maintained separately. Later fixes and their dated checks remain in [review evidence](../record-patching/REVIEW.md).
