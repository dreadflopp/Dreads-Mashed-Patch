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
| ACHR / REFR relocation | Parent cell/world and child-group placement are not part of the current Transform decision. A forwarded transform can therefore be written under an incompatible winning parent. Independent XLRL selection can also retain the old location. The [field review below](#placed-reference-relocation-review-achrrefr) distinguishes required spatial consistency from unsafe broad grouping. |
| REFR water current packages | Mutagen 0.54.4 exposes XWCN/XWCS byte counts and one fixed-layout WaterVelocity/XWCU value, whereas xEdit 93cc0bc defines count/payload alternatives. XCZR/XCZC/XCZA are singular properties but xEdit defines repeated reference-or-cell target/payload entries. The XCZR registration uses ILinkedReferenceGetter while the actual surface is IPlacedGetter. Independent forwarding is retained pending binary fixtures for both count variants, multiple entries, target switches, order and presence. Check xEdit's separate XCVR surface too; do not infer it from WaterVelocity's member names. Sources: wbDefinitionsTES5.pas:9729–9761; decompiled PlacedObjectBinaryCreateTranslation, PlacedObjectBinaryOverlay, PlacedObjectBinaryWriteTranslation and WaterVelocityBinaryCreateTranslation. Grouping cannot recover unrepresented entries. |
| REFR occlusion/geometry | XORD is four positional FormIDs in xEdit (wbDefinitionsTES5.pas:9650), but Mutagen 0.54.4 reads/writes it as raw bytes. Source master indices may survive copying into a different patch master table; this is a source-based risk, not a reproduced cross-master failure. Test differing master indices and links referenced only inside XORD before considering OcclusionPlane + XORD grouping. Merely removing its registration would not establish safe winner copying. Primitive/BoundHalfExtents/Portals/multibound fields remain independent pending representative authored records establishing a shared geometry/transform boundary. RoomPortal/XPTL is marked unused/removed by CK; that alone does not authorize dropping it. |
| Policy performance | Priority/baseline identifier indexes are lazy and avoid full histories during policy checks, but their setup/memory cost has no broad real-modlist benchmark. |

## Placed-reference relocation review (ACHR/REFR)

This is an investigation of the current implementation and proposed boundaries,
not an implemented relocation policy or an in-game validation result. The field
tables cover the record-level members of Mutagen 0.54.4 `IPlacedNpcGetter` and
`IPlacedObjectGetter`, their shared placed members, and the containing structure.
Nested values retain the boundaries stated in their rows; this review does not
establish the engine meaning of every opaque byte or every script property.

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
alternative. Independent XLRL selection currently reaches the mine value for
the same reason. Adding Persist Location, flags, or unrelated fields to that
tuple introduces more ways for an unrelated edit to replace a relocation.

### Shared fields and ACHR-specific fields

Recommendations in this table describe the intended design direction. Current
registrations and existing tests continue to define executable behavior.

| Fields | Boundary and consequence for relocation |
|---|---|
| Parent CELL, parent WRLD, exterior grid, persistent/temporary child group | Structural context, absent from ordinary property comparison. Physical cell/world and position/rotation must describe the same authored placement. Exterior persistent storage uses the world's persistent cell, so storage-cell identity is not always geographic-cell identity. The final Persistent flag must agree with output storage; copying the movement source's child group blindly is insufficient. |
| `Placement`, `Scale` (`Transform`) | Current policy selects position, rotation and scale together. Extend spatial selection to include its coordinate frame and retain provenance. A same-coordinate move to another interior must still count as a change. Existing tests deliberately allow a scale-only edit to replace an earlier movement; with cell grouping it could also replace the cell. This is a policy tradeoff, not an engine requirement that scale and cell always share an owner. Do not silently change the existing scale policy as part of a structural fix. |
| `LocationReference` (XLRL) | Current independent form-link selection produces the reported mismatch. A relocation needs the location metadata associated with its selected spatial context. Later metadata-only corrections from a compatible context should remain possible; an override describing the rejected old interior must not supply that old interior's location. Compatibility must be explicit, not inferred from identical coordinates. Mutagen types this as LCTN, while xEdit accepts LCRT/LCTN/NULL; the latter variants need fixtures before a universal interpretation is implemented. |
| `PersistentLocation` (XLCN) | Separate from XLRL and from the containing cell. It can intentionally identify a broader settlement shared by multiple interiors. Retain independent persistence semantics and check compatibility when relocating; do not replace it automatically with the destination cell's most specific location. CK documents consequences for unique-actor alias filling. Gestur's StonehillsLocation is unchanged in the supplied history. |
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
| `Emittance`, `MultiBoundReference` | Remain distinct decisions, but explicit region lighting and room membership can become inappropriate after relocation. MultiBoundReference is a spatial dependency: inspect the selected room and destination, and report incompatibility. Do not assume all emission or room edits should have the movement owner. |
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

### Recommended algorithm boundary

1. Make physical placement context-aware: select the coordinate frame with the
   current Transform using existing comparison precision and reversion rules.
   Track the selected source, even for a cell-only change. Keep storage ancestry
   distinct from geographic location for persistent exterior references. The
   existing scale-only selection policy needs an explicit decision before any
   change to that ownership boundary.
2. Coordinate XLRL with the selected spatial context. For the reported interior
   move, old-mine overrides must not replace the house's location metadata.
   Permit location-only corrections associated with the selected interior rather
   than demanding identical coordinates. This requires source/context-aware
   coordination, not an ordinary larger SimplePropertyContext value. Exterior,
   persistence-only, absent-location and unusual XLRL cases need explicit
   compatibility rules and fixtures before generalization.
3. Keep XLCN, location roles, explicit encounter zone, Ownership, enable state,
   scripts and AI links under their distinct semantics. Inspect dependencies
   affected by a move. Preserve null/inherited values as authored; do not fill
   them with guessed destination defaults. Treat clear incompatibilities as
   diagnostics and unresolved specialized cases as requiring review.
4. Stage the resulting reference under its selected physical parent and final
   persistence arrangement, using the winning record for unrelated baseline
   fields and retaining already-patched parent/sibling data. Ensure the FormKey
   occurs only once. A move between roots requires atomic removal/insertion;
   failure must not publish only one side. Moving to CRF's Temporary collection
   with a later selected Persistent bit is not a valid shortcut.
5. Check related CELL/LCTN data and special REFR networks separately. The
   Location handler deliberately does not merge Added/Static/Removed reference
   tables; the patcher has no relocation-aware reconstruction of those tables.
   This is relevant to unique actors, persistence and location ref types, and
   prevents a promise that changing ACHR's cell and XLRL alone validates quests.

The narrow, established correction is Gestur's house placement plus its
associated house XLRL, preserving Stonehills Persist Location and separately
selected quest/enable/persistence edits. That is a coherent record-level target;
it has not been verified against the complete plugins' location tables,
packages, quests or an in-game run.

### Evidence and validation boundaries

- **Project implementation:** `AbstractPropertyHandler.UpdatePropertyContext`,
  `SimplePropertyContext`, `ReferenceGroupHandlers`, both placed record handlers,
  `RecordOverrideTransaction`, `MajorRecordFlagsRawHandler` and
  `LocationRecordHandler`. Parent selection is structural; flag selection is
  bitwise; no independent parent-cell merge exists.
- **Mutagen 0.54.4:** local decompiled `IPlacedGetter`, `IPlacedNpcGetter`,
  `IPlacedObjectGetter`, `ILocationGetter`, `CellCommon.EnumerateMajorRecordContexts`,
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
- **Existing regression entry points:** `LocationReferenceConflictPolicyTests`,
  `PlacedObjectGroupTests`, `SharedReferenceGroupTests`,
  `PlacedReferenceForwardingTests`, and `RecordOverrideTransactionTests`.
  The reviewed fixtures establish current same-cell selection, group ownership,
  disabled-reference policy and failure isolation. They do not establish cross-cell
  relocation or game-valid location graphs. Future validation needs cell-only
  moves; conflicting XLRL edits; allowed/denied reversions; scale-only edits;
  interior/exterior and persistent/temporary transitions; duplicate prevention;
  rollback preserving siblings; actual LCTN table inspection; and in-game actor,
  quest, door and rendering checks appropriate to the affected record kinds.

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
