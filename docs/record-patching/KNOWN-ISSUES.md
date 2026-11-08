# Known limits and investigation boundaries

[Internal index](../README.md) · [Regression reference](REVIEW.md) · [Inventory](INVENTORY.md)

These are current limitations or policies that need explicit investigation before
being generalized. Resolved defects belong in the regression reference and tests,
not an open-issues list. Public limitations remain on the affected record pages.

## Implementation limits

| Area | Boundary and evidence to preserve |
|---|---|
| PACK graph | PACK is disabled by default. PackageTemplate, DataInputVersion, Data, XnamMarker and ProcedureTree have no active property registration. Mutagen sorts indexed values/metadata by signed key; xEdit declares value rows in declaration order. [PackageTemplateGraphHandlerTests](../../DreadsMashedPatch.Tests/PackageTemplateGraphHandlerTests.cs) writes `[5,0,3]` as `[0,3,5]` while retaining index/value and procedure associations. This establishes physical reordering, not gameplay harm. Winner/priority copies still use that writer; leaving the graph unregistered does not guarantee identical physical order. |
| NAVM / NAVI | NAVM is disabled by default and selects whole geometry/connectivity data when enabled. No independent NAVI route or patcher-owned reconstruction exists. A Mutagen writer branch for existing NavigationMeshInfoMaps is not a reconstruction algorithm. NAVM and REFR navigation-door-link forwarding do not establish navigation-map consistency. |
| DIAL / DLBR / INFO / DLVW | Disabled by default. Enabling documented property selection can combine coupled dialogue data; tests do not establish dialogue correctness. |
| ARMA model data | All four filename handlers carry detached Data bytes but compare only normalized paths. Byte-only changes have no independent forwarding path. Alternate textures now travel inside the per-gender first/third-person mesh snapshot; their member row updater is inactive. This retained policy must not become a blanket claim that other models ignore data bytes. See [GenderedModelFieldHandlers](../../DreadsMashedPatch/PropertyHandlers/General/GenderedModelFieldHandlers.cs) and [ArmorAddonRecordHandlerTests](../../DreadsMashedPatch.Tests/ArmorAddonRecordHandlerTests.cs). |
| Ordinary script-list VMAD | Version/ObjectFormat remain destination-owned. Metadata-only edits do not trigger output; INFO/PACK/PERK whole adapters and QUST/SCEN child fields differ. |
| LTEX | HNAM/SNAM scalar interfaces do not expose absent-subrecord state; writing can normalize missing representations. Nullable snow INAM requires SSE FormVersion 43 or later. LAND remains excluded. |
| Null/default writes | Setters normalize presence per property. Shared post-write validation checks non-null values only. Keywords and NPC Perks preserve absent versus present-empty, but there is no universal null-removal guarantee. |
| Omitted/opaque data | Unregistered fields have no independent decision. Copying/writing may change representation. The meaning and safe combination of every exposed byte payload are not established. See [inventory exclusions](INVENTORY.md#exclusions-and-fields-that-are-not-independent-decisions). |
| ACHR / REFR relocation | Interior-only cell + DATA and same-cell XLRL selection correct the reported mixed-parent output. Exterior and specialized spatial moves fall back to winning placement; general persistence-only storage correction and LCTN/script/network reconstruction remain outside scope. See the [field review](#placed-reference-relocation-review-achrrefr). |
| REFR water current packages | Mutagen 0.54.4 exposes XWCN/XWCS byte counts and one fixed-layout WaterVelocity/XWCU value, whereas xEdit 93cc0bc defines count/payload alternatives. XCZR/XCZC/XCZA are singular properties but xEdit defines repeated reference-or-cell target/payload entries. The XCZR registration uses ILinkedReferenceGetter while the actual surface is IPlacedGetter. Independent forwarding is retained pending binary fixtures for both count variants, multiple entries, target switches, order and presence. Check xEdit's separate XCVR surface too; do not infer it from WaterVelocity's member names. Sources: wbDefinitionsTES5.pas:9729–9761; decompiled PlacedObjectBinaryCreateTranslation, PlacedObjectBinaryOverlay, PlacedObjectBinaryWriteTranslation and WaterVelocityBinaryCreateTranslation. Grouping cannot recover unrepresented entries. |
| REFR occlusion/geometry | XORD is four positional FormIDs in xEdit (wbDefinitionsTES5.pas:9650), but Mutagen 0.54.4 reads/writes it as raw bytes. Source master indices may survive copying into a different patch master table; this is a source-based risk, not a reproduced cross-master failure. Test differing master indices and links referenced only inside XORD before considering OcclusionPlane + XORD grouping. Merely removing its registration would not establish safe winner copying. Primitive/BoundHalfExtents/Portals/multibound fields remain independent pending representative authored records establishing a shared geometry/transform boundary. RoomPortal/XPTL is marked unused/removed by CK; that alone does not authorize dropping it. |
| Policy performance | Priority/baseline identifier indexes are lazy and avoid full histories during policy checks, but their setup/memory cost has no broad real-modlist benchmark. |

## Placed-reference relocation review (ACHR/REFR)

InteriorReferencePlacement implements the narrow interior-to-interior correction.
The field tables retain the Mutagen 0.54.4 surface review and dependency limits;
they are not a claim that every listed compatibility check is implemented or
that game behavior has been validated. Nested values retain their stated boundaries.

### Established failure and the grouping trap

The reported GesturRockbreakerREF (`01AA55:Skyrim.esm`) history has these values:

| Source | Containing cell | Coordinates | LocationReference / XLRL |
|---|---|---|---|
| Skyrim.esm | StonehillsMine | Original mine position | Absent |
| cutting room floor.esp | CRFStonehillsGesturHouse | House position | House location |
| Andrealphus Jobs Overhaul - MASTER.esp; GodsAndWorship.esp | StonehillsMine | Original mine position | Mine location |
| Reported MashedPatch output | StonehillsMine | House position | Mine location |

The pasted decision log establishes that the mine-coordinate restorations were
denied and CRF's transform was forwarded. The xEdit screenshot establishes the
different containing cells and XLRL values. No in-game symptom was supplied.

Simply adding XLRL to an atomic `(cell, transform, location)` value does not solve
the selection problem. In `AbstractPropertyHandler.UpdatePropertyContext`, the
later `(mine, original position, MineLocation)` tuple differs from the original
`(mine, original position, null)` tuple. It is consequently an addition, not a
permission-controlled restoration, and can replace CRF's entire move. This is a
branch trace of the existing algorithm, not a binary fixture of an implemented
alternative. The former independent XLRL selection reached the mine value for
the same reason. Adding Persist Location, flags, or unrelated fields to that
tuple introduces more ways for an unrelated edit to replace a relocation.

### Shared fields and ACHR-specific fields

Current interior selection is described explicitly below; broader dependency
checks remain review boundaries, not implicit implementation promises.

| Fields | Boundary and consequence for relocation |
|---|---|
| Parent CELL, parent WRLD, exterior grid, persistent/temporary child group | Supported interior moves select CELL with position/rotation and use final Persistent to choose destination storage. The existing transaction publishes one winning-baseline reference per FormKey during normal processing. Exterior/cross-world moves are rejected; exterior geographic indexing and general persistence-only storage correction are not implemented. |
| `Placement`, `Scale` | ACHR/REFR select interior CELL + position/rotation, retaining source ownership. Cell-only moves count. Scale is deliberately separate so a size edit cannot replace an earlier move. Hazards/projectiles retain Transform pending separate review. |
| `LocationReference` (XLRL) | Select from same-parent-cell history while preserving the global original for removal/reversion permissions. Metadata does not participate in movement equality. A forwarded interior move requires its non-null link to resolve as LCTN; xEdit also accepts LCRT/NULL variants, whose universal interpretation is not implemented. |
| `PersistentLocation` (XLCN) | Separate from XLRL and the containing cell; it can name a broader settlement. Gestur retains StonehillsLocation. The interior fix does not traverse LCTN ancestry or establish compatibility with unique-actor aliases; review those relationships in actual plugins. |
| `LocationRefTypes` (XLRT) | Semantic roles such as Boss, used by location-based systems. They are not coordinates and should not be reset by a movement or scale edit. Relocation requires checking the related LCTN reference tables; selecting the list on ACHR alone cannot establish that a location's boss/alias bookkeeping agrees. Preserve existing aligned-list semantics unless separately changed with evidence. |
| `EncounterZone`, `LevelModifier` | Keep explicit encounter/level decisions separate. An absent encounter zone inherits from the cell, so a move can change effective behavior even without a field edit. Audit explicit zone choices against the destination rather than automatically copying or deriving them. Grouping them with placement could undo leveling fixes or moves. |
| `Owner`, `FactionRank` (`Ownership`) | Keep the existing pair. Do not tie it to placement. For relevant REFR types, an absent owner inherits from the containing cell, so moving can change effective ownership without changing this pair. Grouping cannot preserve both an intentional inheritance change and an unrelated ownership correction automatically. |
| `MajorRecordFlagsRaw`, ACHR `MajorFlags` view | Keep the approved composite, bitwise handler as the sole flag owner. Persistent needs coordination with structural insertion. InitiallyDisabled, StartsDead, Deleted, NoAiAcquire, LOD and physics bits are not all relocation metadata. Making the whole header follow a move can undo quest enable state or other independent fixes. REFR flag meanings can also depend on Base type. |
| `EnableParent` | Keep reference and embedded flags together as currently implemented, with selection independent of movement and InitiallyDisabled. Folding this into placement could restore disabled content or discard quest gating. A custom enable parent can be intentional across cells; validate the reference, not mere co-location. Existing disabled-reference regressions must continue to be respected. |
| `Base` | Separate identity/behavior decision. Changes can affect skeleton, scripts, usable reference payloads and, for REFR, header-bit meanings. Blind independent merging is not proof of compatibility, but bundling every Base-dependent field with movement would discard unrelated corrections. Flag type-changing or skeleton-changing combinations for targeted review. |
| `RagdollData`, `RagdollBipedData` (`RagdollPose`) | Retain the existing complete pose pair and presence semantics. Compatibility with the selected actor/object skeleton matters. No reviewed evidence establishes that all payload coordinates can be transformed or that pose must always have the placement owner; do not add that coupling by assumption. |
| `VirtualMachineAdapter` | Keep current script/property merging and metadata rules. Scripts can contain arbitrary cell/location/marker references, so no generic movement group can discover or repair all dependencies. A large atomic group would instead discard independent script fixes. |
| `LinkedReferences`, ACHR `LinkedReferenceColor` | Preserve link-pair payloads and current collection policy: ACHR keyed, REFR positional. Link targets can be distant and intentional; do not force them to match the placement source. Patrol chains, work markers and script links still need reference-level validation after a move. Color remains a separate aggregate. |
| `ActivateParents` | Keep the complete activation configuration together, separately from placement. Grouping it with a move could lose activation-only restrictions or delays; requiring its references to share a cell would reject legitimate setups. |
| `Patrol` | Keep its idle/time/topic payload together. Patrol behavior can depend on linked markers and NPC packages outside this record, so the aggregate does not validate the route. A move should not automatically replace patrol configuration. |
| `Emittance`, `MultiBoundReference` | Remain distinct decisions. Explicit multibound membership gates automatic cross-cell movement, retaining winning spatial data. No general region-lighting compatibility rule is implemented. |
| `IsIgnoredBySandbox`, ACHR `IsIgnoredBySandbox2` | Preserve the distinct markers and current semantics. They affect AI use independently of movement. Combining them or moving their ownership into placement lacks supporting evidence. |
| ACHR `MerchantContainer`, `Horse` | Keep independent links. Merchant storage and the actor's horse can intentionally be elsewhere; copying movement-source values can discard valid vendor or riding fixes. Existence and expected target type are useful checks, not a same-cell rule. |
| ACHR `Count`, `Radius`, `Health`; shared `HeadTrackingWeight`, `FavorCost` | Keep separate values. Several CK descriptions are legacy/unused, and their exact engine effect is not uniformly established. That does not authorize deleting them, interpreting them as relocation metadata, or claiming harmlessness. REFR Radius also has a map-marker use. |
| `EditorID`, FormKey and record-header serialization/version fields | EditorID retains its configured policy. Preserve record identity through a move; do not duplicate as a new FormKey. Version/compression/control data are serialization concerns, not values to merge into spatial ownership. |

### Additional REFR fields

REFR covers unrelated object kinds. One relocation group for every exposed field
would mix door connectivity, room geometry, water data and ordinary item tuning.

| Fields | Boundary and consequence for grouping |
|---|---|
| `BoundHalfExtents`, `Primitive`, `IsMultiBoundPrimitive` | Geometry/type marker data can depend on Base and transform. Bounds/color/type already travel together inside Primitive. There is no verified universal rule joining all geometry to placement; a broad group could drop a bound-only correction or a move. Require representative room/trigger/multibound fixtures. |
| `OcclusionPlane`, `XORD` | Plane geometry and linked occluders have plausible spatial coupling, but XORD contains positional FormIDs represented as raw bytes by Mutagen. Grouping cannot remap hidden master indices. See the existing occlusion limit above before introducing an aggregate. |
| `Portals`, `RoomPortal` | Portals already select their entire endpoint collection. Endpoint rooms and geometric placement must agree across references; one-record grouping cannot repair the room graph. Incorrect combinations can cause missing rendering. xEdit calls XPTL/RoomPortal unused and CK-removed; this alone is not grounds to discard it. |
| `LinkedRooms`, `LightingTemplate`, `ImageSpace`, `Unknown` | These form the XRMR/LNAM/INAM/XLRM bound-data surface. Mutagen's custom writer derives count/presence flags from the selected links, so a shared serialized header does not itself require one selection owner. Room graph consistency is a separate issue. Unknown is preserved from the baseline, not independently forwarded. |
| `LightData`, `Alpha` | Keep each existing complete value. Lighting offsets and alpha tuning can legitimately be edited without moving the object. Their compatibility can depend on Base/assets; merging them into placement would discard unrelated fixes. |
| `TeleportDestination`, `TeleportMessageBox`, `NavigationDoorLink` | Keep each current complete payload, but evaluate doors as a connected system. XTEL's arrival coordinates describe the destination side, not this door's own DATA position. Destination door, arrival marker, opposite door link and NAVM/triangle must agree. Grouping all three locally or grouping them with this door's transform cannot guarantee that. Do not translate XTEL by this door's movement delta. |
| `Reflections`, `LitWater` | Keep current keyed policies, including the whole reflection/refraction payload per water key. Explicit water links may become inappropriate after a move, but broad grouping would discard independent lighting/water corrections. Validate targets and spatial relevance where possible. |
| `XWCN`, `XWCS`, `WaterVelocity`, `XCVL`, `XCZR`, `XCZC`, `XCZA` | Count/payload and reference-or-cell target relationships need dedicated handling. Current Mutagen surfaces do not fully represent xEdit's alternatives or repetitions, and XCVR has no matching exposed record member. The existing water-current limit above blocks claims that grouping alone makes this safe. XCZC is water-current data, not the containing cell. |
| `SpawnContainer`, `LeveledItemBaseObject`, `ItemCount`, `Charge` | Independent item/spawn decisions, subject to Base-type applicability. A movement should not revert a count, charge or spawn fix. Links need target validation, not automatic movement-source ownership. |
| `CollisionLayer` | Base/physics applicability matters. It is not a cell reference, and copying it with every move can undo a collision fix. Keep a separate decision pending any specific demonstrated coupling. |
| `Lock`, `IsOpenByDefault` | Preserve the complete lock payload and independent default-open marker. Ownership, door flags and scripts can change effective accessibility, but grouping those all with movement would lose valid security/quest edits. |
| `Action` | Keep the approved flag handler. Do not move flag selection into a reflection aggregate or transfer its owner on a movement-only edit. |
| `MapMarker`, `Radius` | MapMarker already holds its flags/name/type together; discovery radius is separate. Coordinates and radius jointly affect discovery, but a label, travel flag or radius edit should not automatically undo a relocation. Validate the resulting marker in context. |
| `AttachRef` | Depends on the selected target and an appropriate animation/NIF attachment node. Physical co-location and a common movement owner do not establish that compatibility. Keep separate and report obvious target/type problems. |
| `DistantLodData` | Keep complete payload/presence behavior. LOD also depends on placement, Base, flags and externally generated assets; grouping cannot regenerate LOD or prove that deployed LOD matches. |

### Implemented boundary

Placement and LocationReference have one selection path in InteriorReferencePlacement;
the registered member handlers remain their only writers. Same-parent histories
use the existing ordinary algorithm. Different-parent histories use a selection-only
cell/DATA adapter and same-cell XLRL history, retaining global-original permissions.
Scale and approved flags stay independent.

Only available interior destinations are supported. Exterior histories, unresolved
LCTN targets and special door/navigation, room/multibound, occlusion, water or
attachment payloads gate automatic moves. Rejection keeps winning placement and
dependent spatial payloads, selects compatible XLRL and logs the reason. Unrelated
valid edits continue.

A destination context supplies winning reference data and effective destination
CELL ancestry to the existing RecordOverrideTransaction. Its copy masks retain
patched parents/siblings, and failed setters discard the staged candidate. Normal
processing handles each FormKey once; no repeated-relocation API, output-location
index or multi-root transaction was added. A supported interior move inserts into
the collection selected by the final Persistent bit.

No exterior cell lookup, persistence-only storage normalization, LCTN ancestry/table
reconstruction, arbitrary script repair or network/LOD rebuild is implemented.
Gestur's coherent record-level output has binary regression coverage, but its
complete plugins' location tables, packages, quests and gameplay remain unverified.

### Evidence and validation boundaries

- **Project implementation:** `AbstractPropertyHandler.UpdatePropertyContext`,
  `SimplePropertyContext`, `ReferenceGroupHandlers`, both placed record handlers,
  `RecordOverrideTransaction`, `MajorRecordFlagsRawHandler` and
  `LocationRecordHandler`. Parent selection is structural; flag selection is
  bitwise; parent CELL is structural rather than a reflected field.
- **Mutagen 0.54.4:** local decompiled `IPlacedGetter`, `IPlacedNpcGetter`,
  `IPlacedObjectGetter`, `ILocationGetter`, `CellCommon.EnumerateMajorRecordContexts`,
  `ModContextExt.EnumerateMajorRecordContexts/CellCopyMask`,
  `CellBinaryWriteTranslation`, `WorldspaceBinaryWriteTranslation`, and both
  placed records' binary create/write translations. The context inserter chooses
  Persistent/Temporary from source collection membership; the writer emits the
  chosen collections rather than inferring them from newly merged flags.
- **xEdit 93cc0bc5a1251936c3c7859eee3150eda12a62d7:**
  `Core/wbDefinitionsTES5.pas`, ACHR definition near 3107, LCTN near 6308,
  REFR near 9500, and `Build/Edit Scripts/Put worldspace references in the right cells.pas`.
  The latter explicitly excludes persistent references from ordinary geographic
  cell reassignment. xEdit also declares ACHR XLOD, which the reviewed Mutagen
  getter/writer do not expose; exposed-property coverage is not complete binary
  format coverage.
- **CK documentation consulted 2026-11-05:** [Reference](https://ck.uesp.net/wiki/Reference)
  for inheritance, persistence, enable/activation, door and attachment behavior;
  [Location Ref Type](https://ck.uesp.net/wiki/Location_Ref_Type) and
  [Quest Alias Tab](https://ck.uesp.net/wiki/Quest_Alias_Tab) for location-based
  reference discovery;
  [Cell Reset](https://ck.uesp.net/wiki/Cell_Reset) for boss/cleared relationships;
  [Room Bounds and Portal Basics](https://ck.uesp.net/wiki/Room_Bounds_and_Portal_Basics)
  for rendering dependencies. Page text was read via MediaWiki's
  `w/api.php?action=parse&page=PAGE&prop=wikitext&format=json` endpoint.
  These document concepts; they do not establish every XLRL binary variant's
  engine precedence or a relocation-table reconstruction algorithm.
- **Regression entry points:** `InteriorReferencePlacementTests` verifies source
  overlays through output reload for Gestur, cell-only changes, permissions,
  compatible metadata, independent Scale/EnableParent, effective destination
  metadata, existing siblings, unsupported moves and failed setters.
  `LocationReferenceConflictPolicyTests`, `PlacedObjectGroupTests`,
  `SharedReferenceGroupTests`, `PlacedReferenceForwardingTests` and
  `RecordOverrideTransactionTests` retain ordinary selection/copying invariants.
  These fixtures do not establish game-valid LCTN tables, quests or generated assets.

## Appearance and referenced assets

Record-only selection cannot identify the deployed winner of same-path mesh
replacers or verify FaceGen, custom skins, or compatibility between the winning
NPC, RACE, HDPT, ARMO and ARMA records. Enabling tree-group forwarding
can mix asset-dependent model, wind, scripts and flags without validating them.
The [asset forwarding guide](../ASSET_FORWARDING.md) defines active group ownership and defaults, and the remaining resource/dependency limits. Restoring an earlier path can bypass a later same-path replacement; grouping does not verify deployed files.
The implementation constraints and tests are in the
[appearance regression reference](REVIEW.md#appearance-boundaries).

OppositeGenderAnims remains relative to the selected sex/race behavior graph;
independent forwarding does not convert it into an absolute masculine/feminine
animation choice. TemplateAndTraits selects a pointer and Traits bit without
resolving effective inheritance. Local height/appearance values can therefore
be superseded by engine template behavior.

## Audit and diagnostic limits

Output validation checks generated, non-null FormLinks, the LLCT capacity, and LVLI/LVLN/LVSP cycles containing output overrides. The engine's exact PlayerRef (`000014:Skyrim.esm`) is accepted for actor-reference-compatible getters without a plugin-record lookup; other low FormIDs are not broadly exempted. It is not a full xEdit check or a source-plugin audit. Raw bytes with hidden FormIDs (such as XORD), required-null rules, context-dependent constraints beyond the explicit exceptions, other graph types, deployed assets, script compatibility and effective NPC template behavior remain outside its coverage. Deleted targets are rejected only for NPC packages and leveled entries. Input-only cycles and existing errors exposed by omitting an override are left untouched. Warnings list input records to investigate; output-only validation cannot always identify the unique author of a bad reference or a merge-created conflict. Lookup performance has a scoped five-vanilla-master benchmark, not a complete target-modlist measurement; see [lookup design and benchmark boundaries](../DEVELOPMENT.md#output-and-diagnostics).

Valid COED faction ownership can be imported as `UntypedOwner` when external type information is unavailable. The output-only compatibility workaround uses the 24 bits in `VariableData.FormKey.ID` to choose the compatible signed rank with the smallest absolute value (positive at the midpoint tie). This replaces the former special case for -1. It applies only to confirmed faction owners, replaces the union with a typed `FactionOwner`, and logs information instead of rejecting positive untyped ranks. This assumes original ranks from -8388607 through 8388608; it reconstructs negative ranks such as -2, -129 and -10000, but cannot preserve arbitrary 32-bit ranks (for example positive 16767216 becomes -10000). Already typed ranks and NPC-owned references are untouched. Source plugins and merging rules remain unchanged. Removing Synthesis alone does not remove the need for correct external-owner decoding; retire this workaround only after the replacement import path demonstrates typed ownership and full integer preservation. Exact provenance, regression coverage, removal conditions and the unimplemented broader import investigation are in the [development guide](../DEVELOPMENT.md#coed-faction-rank-import-workaround).

Coverage audits include dormant handlers, infer some aggregate ownership and do
not execute every setter or inspect every nested comparison. Collection audits
cover independently registered collection values; nested atomic lists need their
own tests. Check actual query/filter/dispatch and the source comparer before
claiming full support or byte preservation.

A skipped record receives no newly merged override; other successful records
publish under the default partial-output policy. Fatal failures block file
publication. Correct discard behavior does not make an incomplete patch
compatible with every modlist. The [development guide](../DEVELOPMENT.md#output-and-diagnostics)
defines the error contract.

Embedded missing-localization text is not a diagnostic severity marker; this
does not repair the source plugin's missing strings. For the reported NPC Perks
and GLOB/GMST cast failures, mechanisms are reproduced in fixtures, but the exact
users' plugins were not verified; see [failure diagnosis](../USER-REPORTED-FAILURES-2026-10-10.md).

With Mutagen 0.54.4, MGEF Archetype Type, ActorValue and AssociationKey cover the
semantic subtype surface inspected; a typed Association is represented by
AssociationKey. No additional missing archetype-field defect was established.
Recheck generated interfaces on a dependency upgrade rather than adding handlers
without a concrete counterexample.

[Validation scope](REVIEW.md#validation-scope) distinguishes synthetic fixtures,
binary reloads and a single real-load-order check. Native Windows UI, real
mod-manager integration, broad modlist compatibility and in-game outcomes have
not been established by those checks.
