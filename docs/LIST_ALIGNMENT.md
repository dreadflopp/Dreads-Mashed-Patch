# Ordered list alignment

## Terminology

xEdit does not sort order-sensitive arrays. It aligns entries from each override
into shared display rows while preserving the order declared by every column as
far as a longest-common-subsequence diff permits. Blank cells are gaps in a
column, not values in the plugin.

`ExactOrdered` arrays are the exception: when xEdit exposes no entry sort key,
there is no sequence identity to align. Dread's Mashed Patch compares those entries
by zero-based position and treats the complete value at each position atomically.
Sound Descriptor paths and REFR linked references use this mode.

Dread's Mashed Patch uses those accumulated rows to locate and order the active
entries written to the patch. Ownership remains a separate decision: alignment
does not grant permission to remove, restore, move, or revert a value owned by a
plugin that is not one of the current plugin's masters. As with sorted keyed
entries, a later independent edit to non-key data can take ownership of the
value occupying the same row.

## Active algorithm

The active implementation is `XEditSequenceAligner`, used by every list handler
whose `Semantics` is `ListSemantics.AlignedOrdered`. This describes the aligner,
not a claim that every registration is correctly classified or has the exact
xEdit row key. Use [pinned ordering evidence](history/LIST_ORDERING_AUDIT.md) and the [current collection expectations](xedit-collection-expectations.json) for property-specific classifications.

1. Initialize one alignment row for every item in the original record.
2. Process record overrides from original to winning, using the contexts already
   resolved for the forward pass.
3. Diff the accumulated row keys against the current override's keys with a port
   of xEdit's `TDiff` Myers O(ND) longest-common-subsequence implementation.
4. Reuse rows for matches, retain unmatched old rows as gaps, and insert a new
   row for each unmatched current item.
5. Reconcile the current column against active entries by occurrence-specific
   row ID. A content change within a reused row replaces that row's value instead
   of creating a second entry.
6. For unmatched rows, retain the existing permission-aware move,
   addition/restoration, and removal rules. Full logical identity is used only
   to recognize moves and prior removals, not to distinguish values within an
   already matched row.
7. Emit active entries in accumulated row order. Removed entries remain internal
   tombstones and are not written.

The algorithm stores only the accumulated rows, row IDs, and ownership metadata.
It does not call `ResolveAllContexts` again and does not expand the context after
the forward pass. The record handler already resolves the override chain once
and visits it from original to winning, so every prior override has contributed
its alignment information by the time the winning override is processed.

The diff is O(ND), where N is the combined sequence length and D is the edit
distance. Memory is proportional to the explored diagonals plus the accumulated
rows. This is CPU and in-memory list work; it avoids the expensive extra Mutagen
context resolution that prompted the earlier incremental design.

### Conditions

Condition rows use the Skyrim xEdit sort-key fields:

- CTDA function
- CTDA parameter 1
- CTDA parameter 2
- CIS1
- CIS2

The comparison operator/value, flags, run-on fields, reference, and parameter 3
are not part of the alignment key. They are row content: changing one replaces
the value in that occurrence-specific row. Reversion to the original row value
still requires permission to modify the current value owner.

For conditions aligned in rows `x, y, z`, a value edit `x=-1` reuses row `x`;
a protected `z<200` stays in row `z`; a removed `y` is a gap. The patch writes
`x=-1, z<200`, rather than moving the protected row ahead of the edited one.

## How xEdit aligns arrays

Skyrim xEdit marks alignable arrays with record-definition sort keys. In the
record view it:

1. starts with the first non-empty plugin column;
2. builds integer IDs for each distinct display sort key;
3. progressively diffs the accumulated rows against each subsequent column;
4. shifts prior columns into the diff result, leaving blank cells for deletes;
5. places the current column into matching or added rows; and
6. carries the resulting row-key sequence into the next column.

The implementation sets `TDiff.AllowModify` to `False`, so a changed key is a
delete plus an add rather than a modified row. xEdit uses a case-sensitive key
table. Duplicate keys are aligned as separate occurrences according to the diff
algorithm's deterministic tie behavior.

The xEdit release notes illustrate columns `A,B,C`, `B,C`, and `B,A` as rows
`A,B,A,C`. Current xEdit source resolves that ambiguous final move as
`A,B,C,A`: its `TDiff` emits the unmatched old `C` before the unmatched new `A`.
Dread's Mashed Patch follows the current source implementation, not the older
illustration. This distinction has no effect when an edited item keeps the same
alignment key, as conditions do when only their operator or comparison value
changes.

References:

- xEdit array alignment: <https://github.com/TES5Edit/TES5Edit/blob/93cc0bc5a1251936c3c7859eee3150eda12a62d7/xEdit/xeMainForm.pas#L7339-L7458>
- xEdit Myers diff: <https://github.com/TES5Edit/TES5Edit/blob/93cc0bc5a1251936c3c7859eee3150eda12a62d7/External/Diff/Diff.pas#L443-L554>
- Skyrim condition keys: <https://github.com/TES5Edit/TES5Edit/blob/93cc0bc5a1251936c3c7859eee3150eda12a62d7/Core/wbDefinitionsTES5.pas#L3879-L3936>
- Original alignment release note: <https://github.com/TES5Edit/TES5Edit/blob/93cc0bc5a1251936c3c7859eee3150eda12a62d7/whatsnew.md#L2139-L2162>

## Implementation constraints

Do not replace persistent occurrence rows with neighbor-insertion heuristics,
full-value identity or adjacency/topological constraints. Earlier approaches lost
load-order-wide row identity through removals and re-additions. In particular,
using full condition equality made `z<=200`→`z<200` a removed row plus an addition;
a later independent `x=-1` could then produce `z<200, x=-1` rather than aligned
`x=-1, z<200`. The source-based aligner and row-content replacement prevent this.

- Conditions retain their reviewed CTDA key; aliases retain ID-based merging and
  response rows retain normalization/copying after shared reconciliation.
- CameraPath Shots, LinkTo, DLVW Branches, SlotParents, FLST Items, NPC Packages,
  placed LocationRefTypes and quest globals use their reviewed reference keys.
- StoryManagerQuestNode rows use Quest FormKey. Message buttons and dialog
  responses have project-specific identities because xEdit supplies no explicit
  outer key; do not claim exact key parity without investigating them.
- HDPT Parts is ordered but nested in atomic Appearance, so it does not allocate
  independent alignment rows. ARMO Armature is also one complete authored list
  to prevent independently unioned overlapping models.
- Magic-item Effects, SNDR sound slots and REFR LinkedReferences use exact
  ordinal identity. ACHR LinkedReferences is independently sorted/keyed.
- MGEF Sounds uses aligned declaration order and complete Type/Sound pair keys
  for xEdit 4.1.5f compatibility. The newer pinned source sorts by Type; see the
  [version exception](history/LIST_ORDERING_AUDIT.md#mgef-sndd-version-exception).
- Unordered/sorted-keyed values use their own identity reconciliation, not the
  aligner. Structural arrays remain atomic values rather than per-entry contexts.

Use [XEditListAlignmentTests](../DreadsMashedPatch.Tests/XEditListAlignmentTests.cs),
[ConditionHandlerTests](../DreadsMashedPatch.Tests/ConditionHandlerTests.cs),
[ListSemanticsTests](../DreadsMashedPatch.Tests/ListSemanticsTests.cs) and the
[collection verification workflow](COLLECTION_SEMANTICS.md#workflow) when changing
alignment. Keep one active implementation path; version control holds retired
implementations if a deliberate investigation needs them.
