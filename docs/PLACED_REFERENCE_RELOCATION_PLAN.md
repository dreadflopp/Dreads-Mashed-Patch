# ACHR / REFR relocation implementation plan

[Internal index](README.md) · [Field review and evidence](record-patching/KNOWN-ISSUES.md#placed-reference-relocation-review-achrrefr)

Status: proposed implementation, with a measured baseline of the current writer.
No relocation behavior is implemented by this document or its benchmark. Remove
this plan after delivery, retaining current invariants and useful regression and
performance evidence in the maintained guides.

## Outcome and scope

For GesturRockbreakerREF, forwarding CRF's relocation must select its house cell,
house position/rotation and associated house XLRL. Independent persistence,
quest/enable, ownership and script edits must retain their own decisions. A
later mine-location addition must not bypass movement reversion permissions.

Implement the shared machinery first, enable the first verified interior ACHR
case, then extend it to ordinary REFR with representative fixtures. Exterior
and persistence transitions require explicit support before automatic forwarding;
special door/room/occlusion/water cases require separate dependency rules. Do not
enable unverified cases merely because they implement IPlacedGetter.

Proposed ownership changes for ACHR/REFR:

- Select physical cell/world and complete Placement (position/rotation) together,
  retaining the selected source and existing precision/reversion permissions.
- Split Scale from the existing Transform group for ACHR/REFR. This is a deliberate
  behavior change: a size-only edit must not select the old cell/position. The
  shared Transform implementation remains active for hazards/projectiles until
  those routes are separately reviewed; it is not dead code to delete.
- Coordinate XLRL selection with the selected spatial context, retaining compatible
  location-only corrections. Do not add it to the placement equality tuple.
- Keep XLCN, location roles, explicit encounter zones, ownership, enable state,
  links and scripts distinct. Check relevant dependencies rather than taking all
  fields from the movement source. Preserve approved flag handlers.
- Compute persistent/temporary output storage from final persistence and the
  supported physical placement. A world's persistent storage cell is not an
  ordinary geographic cell.

On a specifically detected unsupported or inconsistent relocation, retain the
winning physical placement and its placement-associated metadata together,
report the rejected move, and continue valid unrelated merges. If even that
baseline is structurally invalid or publication fails, use existing record-skip
diagnostics. A setter/insertion failure must not publish a partial move.

## Implementation sequence

### 1. Establish the decision contract in regression fixtures

Use `PlacedReferenceForwardingTests`, `LocationReferenceConflictPolicyTests` and
the existing binary-overlay helpers as patterns. Add real containing cells to
the test history rather than testing only detached records with changed floats.

Cover Gestur's original-null/CRF-house/later-mine XLRL sequence; a cell-only move
with identical coordinates; permitted and denied declared/virtual-master
restorations; copied overrides that must not steal ownership; and scale-only
edits that leave placement intact. Include an assertion demonstrating why the
larger `(cell, transform, XLRL)` scalar group is the wrong algorithm.

Define compatible context initially by the supported interior identity, not exact
coordinate equality. A later house-location correction may be valid even if that
override has another position in the same house. Extend the definition explicitly
for exterior geography and persistence-only storage changes. Do not infer a move
from the Persistent flag or equate geographic position with the top storage cell.

### 2. Add context-aware selection using existing coordination patterns

Reuse the record-local initialization/final-coordination pattern demonstrated by
`AssetForwardingPolicy`; keep relocation outside asset family settings. Introduce
one shared placed-reference coordinator/projection for ACHR and REFR rather than
two record-specific algorithms.

Project each already-resolved history entry into compact spatial identities and
numeric values once per processed record. Retain the authored source/owner as
selection proceeds; do not guess it afterward from the first equal coordinates.
Reuse existing Placement equality, Scale scalar selection and master/virtual-master
permission semantics. Avoid copying the full record or cloning Placement for
every ancestry check.

Replace the ACHR/REFR Transform registrations completely with the intended
Placement/Scale paths. Reuse registered setters for ordinary properties.
Mark coordinator-owned decisions resolved in the normal property pass, as the
asset pattern already does, so XLRL and Placement never have two active selection
owners. Keep Scale on its one ordinary path. Preserve script/asset coordination
when composing relocation decisions.

`PropertyForwardingCoordination` currently carries only values and audit text.
Extend it with an optional structural placement result, or add a narrowly scoped
equivalent record-level result. The default for all other records must preserve
current behavior. `AbstractRecordHandler.Process` must request an override when
structure changes even if no ordinary field differs from the winning record.
Do not introduce a fictitious reflection Cell property.

### 3. Separate the record baseline from its output destination

Extend `CommitOverride` / `RecordOverrideTransaction` so the winning source record
provides unrelated baseline data while a separate selection determines ancestry.
Do not simply call the CRF context's GetOrAddAsOverride and inherit all CRF fields,
or insert through the winning context and retain its wrong parent.

Prepare the detached candidate, apply selected values with existing handlers,
validate the final persistence arrangement, and stage the necessary destination
ancestry. Reuse current copy masks and preserve already-patched CELL/WRLD headers
and siblings. Retain the existing path when no structural relocation is required.

Ensure a FormKey occurs once in output. If a reference is already present under
another output parent, stage removal and insertion together, including moves
between different top-level roots. Publish both sides only after validation.
Do not deep-copy an entire populated world or scan the entire output to find
each reference. If a run-scoped output-location index is used, update it only
after successful publication and keep it coherent with every parent-replacement
path, including priority policies. Test rollback and index consistency together.

### 4. Validate metadata and gate unsupported dependencies

For the selected interior move, start XLRL from the selected placement context;
process eligible same-context corrections with their own removal/reversion
semantics. Old-mine values must not leak into the retained house placement.
Preserve null/present distinctions and retain a clear diagnostic provenance.

Validate XLCN without requiring equality to the cell's most specific location:
the broader Stonehills location can remain valid. Check target links and the
relevant CELL/LCTN relationships lazily. Existing LCTN Added/Static/Removed tables
are not reconstructed by the patcher. Inspect those tables in representative
plugins before claiming support for quest/persistence/location-role coherence.
A required unresolved table conflict must reject the relocation, not trigger a
guessed generic rebuild.

Unusual XLRL LCRT/NULL variants, incompatible explicit room membership, door/navmesh
networks and incompletely represented water/occlusion data remain explicit limits
until their source surfaces and fixtures establish a rule. Reuse the same
fallback/diagnostics contract for those cases. Grouping opaque payloads does not
resolve their representation or FormID-remapping issues.

### 5. Extend supported cases and diagnostics

Exercise interior-to-interior, interior/exterior, worldspace changes, geographic
exterior cell boundaries and persistent/temporary transitions before enabling
each case. A persistence-only change must not look like movement. Preserve
record identity, ignored-mod filtering, Always-win snapshots, short-history
skips, baseline EditorID policy and independent enable/disable semantics.

Reuse the coordinator for ordinary REFR, then evaluate special REFR categories
individually. The scope does not include generic NAVM/NAVI reconstruction, arbitrary
script rewriting, LOD generation, a global LCTN rebuild or a new collection merge
algorithm. Those would materially enlarge the project.

Log original/winning/selected physical parent and world, storage arrangement,
position/rotation, source owner, associated XLRL and any fallback reason. Detailed
mode may show each context; ordinary logging should avoid extra large history
dumps and repeated formatting/resolution work.

### 6. Validate, measure and update documentation

Run focused selection/transaction tests, binary reloads and the appropriate core
suite. Include structural-only output; all supported persistence transitions;
duplicate prevention; failure after removal but before insertion; preservation of
existing parent/sibling edits; and unchanged behavior in unrelated record types.
Re-run coverage/reference searches to remove obsolete ACHR/REFR registrations,
update affected tests, and confirm a single active owner per field.

Update internal inventory/regression/limits documentation and public placed-record
behavior. Regenerate affected coverage/group reports through existing workflows;
do not manually edit generated artifacts. Keep implementation evidence out of
the root README and public record pages.

Then inspect the actual Gestur source/output records, their relevant LCTN data
and actor dependencies. In-game validation must check actor placement, suitable
quest aliases and persistence/enable behavior on an appropriate test save. Binary
round trips alone cannot provide that evidence.

## Performance assessment

### What the source establishes

The ordinary path already has the affected record's override history. Its
property work is roughly proportional to processed records × history depth ×
property work. Adding one parent walk per history entry adds approximately
`O(N × H × D)`, where D is the bounded ancestry depth. Keep only `O(H)` new
projection state for the current record, plus lazily populated identity caches.
Do not retain new snapshots for every record in the load order.

Use existing typed link caches. Cache relevant location ancestry/cell lookups by
FormKey; use a lazily built per-world geographic cell index if a supported
persistent-exterior case needs one. Include index construction in final cold-run
measurements. Do not construct a new broad IMajorRecord context index, resolve
the same history once per field, walk every location graph per reference or
scan all exterior cells for each relocation. Preserve current filter/priority
shortcuts and avoid new work for disabled record types.

The current output transaction already copies collection containers along a
mutable ancestry path and performs linear child lookups. Repeated output into a
crowded cell can therefore grow faster than linearly with emitted references.
A move may touch both old and destination output paths. This is the principal
performance risk; a blanket full-world clone or global reference search per move
would make it substantially worse. Keep ordinary commits on the current path;
touch only the required ancestry for moved records. Broader batching/transaction
optimization is separate work unless measurements show it is required.

### Measured current baseline, 2026-11-05

The opt-in [baseline benchmark](../scripts/benchmarks/PlacedReferenceBaseline/Program.cs)
measures the current writer and an isolated parent-traversal loop. It does not
implement or time the proposed coordinator, relocation insertion, LCTN validation,
disk loading, serialization, logging or new indexes.

Environment: Linux x64, Ryzen 5 7600, SDK 10.0.401, Release build,
`DOTNET_TieredCompilation=0`. Source is the reviewed working tree with Mutagen
0.54.4; `RecordOverrideTransaction.cs` SHA-256:
`8d3d9772a772900b790a497899cbd959eb971942b06a1106d1942000e68022dd`.
Source objects are minimal in-memory NPC references, with context/index setup
and warm-up outside measurement. Exterior cases exercise ancestry, not a
game-valid geographic layout. These figures establish scope-specific costs,
not expected whole-patcher runtime on a real modlist.

| Measurement | Interior | Exterior |
|---|---:|---:|
| Extra ancestry traversal per 10,000 references × 3 override contexts | 1.30 ms | 2.18 ms |
| Extra allocation in that traversal loop | 0 bytes | 0 bytes |
| Current writer: 2,000 overrides, 1 cell | 93.92 ms | 97.89 ms |
| Current writer: 2,000 overrides, 50 cells | 20.87 ms | 26.04 ms |
| Current writer: cumulative allocation, 2,000 overrides, 1 cell | 102.5 MiB | 117.2 MiB |
| Current writer: cumulative allocation, 2,000 overrides, 50 cells | 63.8 MiB | 78.4 MiB |

Traversal figures are differences of medians from seven alternating-order
samples, each making forty passes; values above divide by forty. Writer figures
are medians of five complete fresh-output runs after warm-up. Allocations are
cumulative current-thread allocations, **not peak or retained memory**. The final
implementation will allocate state and may perform metadata work beyond this
allocation-free traversal loop.

Reproduce from the repository root:

```bash
DOTNET_TieredCompilation=0 dotnet run --project scripts/benchmarks/PlacedReferenceBaseline/PlacedReferenceBaseline.csproj -c Release
```

### Before/after performance gate

Extend the benchmark with the actual implementation before judging its overhead.
Measure selection, validation, publication and end-to-end patching separately.
Use binary-overlay sources as well as mutable fixtures, identical logging/output
settings, cold and warm caches, and both the normal production runtime settings
and controlled runs. Include startup/index construction, output serialization,
cumulative allocation, peak working set and retained cache sizes.

Select representative cases rather than the full Cartesian product: 10k and
100k references; histories of 3/10/30 contexts; 0/1/10/100 percent relocations;
crowded source/destination cells and distributed cells; same-world and cross-root
moves; persistence transitions; summary and detailed logging. Compare publication
against equivalent already-correct destination output, since fixing selection
can legitimately change how many records are emitted.

Development targets, not measured promises:

- For no-relocation cases, investigate a repeatable median runtime regression
  above 5% or peak-memory increase above 10% against the same working-tree
  baseline. Rerun long enough to distinguish small regressions from noise.
- For relocation-heavy cases, account for additional touched ancestry and emitted
  records. Reject new scans proportional to total load-order/output size per
  move, or cache growth unrelated to the distinct records actually consulted.
- Double input counts in dense-cell cases to distinguish existing writer scaling
  from newly introduced scaling. Track lookup/index-build counts as well as time.
- Keep timing benchmarks opt-in; ordinary correctness tests should assert output
  and bounded operation counts rather than fragile millisecond thresholds.

Conclusion: there is no inherent expensive selection step in the proposed design;
parent traversal alone is small in this baseline. Overall overhead remains
unmeasured until the coordinator and transactional relocation exist. Publication
and dependency validation need the performance gate before enabling broad REFR
support.

## Change size and delivery boundaries

This is a medium-to-large correctness change within the current architecture.
It crosses selection, structural publication, diagnostics and documentation; it
is larger than replacing one property handler. It does not require a new core
merge algorithm, dependency update or desktop UI redesign.

Planning range for the supported ACHR plus ordinary-REFR work: roughly 6–10
production files added/changed, 6–10 regression files added/updated, and the
affected maintained/generated documentation. Expect several hundred production
lines and substantial regression coverage; exact counts depend on how much of
the existing transaction can be reused. The opt-in benchmark is separate from
production behavior. A complete generalized location-index rebuild or special
REFR network repair is a substantially larger follow-up, not included in this
estimate.

Keep delivery reviewable:

1. Shared decision/output-result plumbing and regression fixtures, preserving
   unrelated record behavior.
2. Verified ACHR interior relocation, XLRL coordination, independent Scale and
   transactional publication with diagnostic fallback.
3. Additional proven spatial/persistence cases and ordinary REFR support, with
   measured overhead and explicit special-record exclusions.

The hardest work is preserving correct behavior when sources disagree, not
reading a cell identifier. Do not call the work complete while supported cases
still publish mismatched ancestry/metadata or while required validation remains
unperformed; narrow documented support is preferable to unsupported guarantees.
