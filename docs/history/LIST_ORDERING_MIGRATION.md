# Collection implementation decisions

[Internal references](README.md) · [Pinned xEdit evidence](LIST_ORDERING_AUDIT.md) · [Current expectations](../xedit-collection-expectations.json)

The filename is retained for expectation provenance. This is the current decision
summary, not a pending migration. Generic list handlers require an explicit
ListSemantics value; CLR collection interfaces do not select behavior.

## Shared machinery and exceptions

| Surface | Shared behavior | Specialized boundary and reason |
|---|---|---|
| Sorted rows | XEditSortKeyComparer orders reviewed scalar/composite keys; FormIDs compare in load-order space then local ID | Row content is selected as a complete value. Duplicates remain occurrence-sensitive. |
| Aligned rows | XEditSequenceAligner progressively aligns reviewed keys and reconciles persistent occurrence row IDs | Permissions still govern move/removal/restoration/reversion. Conditions, quest aliases and response rows retain their reviewed identity/copy adapters. |
| ExactOrdered | Zero-based positional identity; each complete entry is atomic | Magic-item Effects, SNDR sound slots and REFR linked references have no outer alignment key. |
| Atomic sequences | One owner retains complete authored order and payload | Structural/non-alignable arrays must not get generic per-entry ownership. |
| ARMO Armature | Whole ordered Armor Addon list | Unioning independently authored rows can equip overlapping models that no source declared. |
| OTFT Items | Whole outfit package under ordinary ownership/reversion permissions | xEdit sorts INAM FormIDs, but unioning competing wardrobes can introduce overlapping equipment. Equality ignores order and retains duplicate counts; detached copies retain source order. Nullable absence emits no INAM; present-empty emits an empty INAM, preserving presence on reload. |
| NPC Packages / quest globals / CameraPath Shots / LinkTo / Branches / SlotParents / FLST Items / placed LocationRefTypes | Reviewed aligned references | Preserve source access/nullability behavior; FLST sorting is disabled by a callback. |
| NPC/RACE gameplay rows, inventory, leveled entries and PERK effects | Reviewed keyed identities | Count/rank permissions, fixed skill-slot storage, COED and typed polymorphic copying stay specialized. |
| NPC/HDPT Appearance | Atomic snapshot contains head/tint/extra-part/morph collections | Sorted equality ignores reordering for NPC heads/tints and HDPT extra parts without independent row merging. HDPT morph order remains significant. |
| ARMA alternate textures / BPTD Parts | Key by (Name, Index) / PartNode | These are independent collection registrations and must not become atomic merely because NPC appearance is atomic. |
| FURN Markers / IDLM Animations / MUSC Tracks / MUST Tracks / DEBR Models | Whole sequences | Marker structure, Run In Sequence, Maintain Track Order and payload ordering make union or arbitrary sorting unsafe. |
| Scene/quest adapters, fragments and graphs | Record-specific contexts and validated structure | Use their actual registrations/presence gates; no blanket list classification describes every child. |
| LAND Layers/Textures | Unordered | Dormant exceptions pending a dedicated binary model. Do not activate or assign generic keys as part of a handler cleanup. |

Unordered values use full semantic multiset equality without alignment rows.
Do not resurrect the retired ListOrdering.None/PreserveModOrder model: it conflated
sorted keys, unordered values and declaration-order/structural arrays.

Intentional exclusions: WATR UnusedNoisemaps has no active forwarding registration;
DLVW Topics is absent from the referenced writable interface. SHOU WordsOfPower
and REGN area geometry retain complete-sequence handlers. Fixed dictionaries and
gendered aggregates are semantic values rather than generic row merges.

For a change, check the pinned xEdit definition, writable Mutagen surface,
independent expectation, setter copying and binary round trip. Passing a mode
comparison alone does not establish correct identity or gameplay compatibility.
