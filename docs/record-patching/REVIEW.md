# Implementation decisions and regression reference

[Internal index](../README.md) · [Development guide](../DEVELOPMENT.md) · [Inventory](INVENTORY.md) · [Known limits](KNOWN-ISSUES.md)

Use this map to find the tests and invariants relevant to a change. Completed
migration plans and sequential build logs are not maintained here. Registrations
and per-record decision notes live with the handlers; current field lists live
in the inventory and public record pages.

Asset-bearing values are now controlled by [asset family policies](../ASSET_FORWARDING.md).
Resource and Appearance classifications are fixed true; only ForwardTreeGroups is configurable and defaults off.
Reference rules own only references and required interpretation data. Existing forwarding regressions retain their permission/copying invariants.
The [runtime group manifest](../asset-forwarding-groups.json) is authoritative for
membership and the collection reports account for multi-member atomic selection.

## Regression map

| Area | Preserve this behavior and rationale | Tests |
|---|---|---|
| Record headers | One composite MajorRecordFlagsRaw handler owns the supported raw/common/typed bits. Independent header aliases or OR-based reapplication can resurrect selected clears. Unknown bits stay winner-owned. InitiallyDisabled owns only its flag decision and cannot authorize property reversions or transfer their ownership. | [RecordHeaderFlagsTests](../../DreadsMashedPatch.Tests/RecordHeaderFlagsTests.cs), [PlacedReferenceForwardingTests](../../DreadsMashedPatch.Tests/PlacedReferenceForwardingTests.cs) |
| ACHR/REFR interior placement | Cell + position/rotation share selection; Scale and enable-state decisions remain independent. XLRL uses same-cell history and global-original removal permissions. Structural-only moves emit through the existing transaction with winning baseline and effective destination metadata. Final persistence chooses the collection for supported moves. Unsupported exterior/specialized moves use winning spatial fallback. Binary fixtures cover Gestur, cell-only moves, metadata corrections/removal, master/virtual-master restoration, copied ownership, patched siblings, fallback and failed setters. | [Interior regressions](../../DreadsMashedPatch.Tests/InteriorReferencePlacementTests.cs), [enable state](../../DreadsMashedPatch.Tests/PlacedReferenceForwardingTests.cs) |
| CONT/NPC inventory | Complete generated row copies retain duplicate occurrences, nullable COED, condition and all polymorphic owners. Reserve unchanged duplicates first. NPC count permission and COED permission precede ownership transfer; a new count cannot authorize a same-row COED reversion. CONT retains its count policy. | [ContainerItemExtraDataTests](../../DreadsMashedPatch.Tests/ContainerItemExtraDataTests.cs), [NpcItemExtraDataTests](../../DreadsMashedPatch.Tests/NpcItemExtraDataTests.cs) |
| Script-list VMAD | Replace scripts without resetting destination Version/ObjectFormat; defaults 5/2 apply only when creating an adapter. Script rows remain complete values. INFO/PACK/PERK whole adapters and QUST/SCEN child registrations have different boundaries. | [ScriptAdapterMetadataTests](../../DreadsMashedPatch.Tests/ScriptAdapterMetadataTests.cs), [VirtualMachineAdapterHandlerTests](../../DreadsMashedPatch.Tests/VirtualMachineAdapterHandlerTests.cs) |
| MGEF sound order | Reuse the generated-copy list handler with shared ordered alignment and full Type/Sound identity. Released xEdit 4.1.5f uses an alignable plain SNDD array; the newer pinned definition sorts by Type. Preserve declaration order, duplicate occurrences and permission-owned removals through binary output. Archetype, VMAD and approved flag handlers stay specialized; no replacement class or parallel path is needed. Exact reference provenance and writer evidence are in the [version exception](../history/LIST_ORDERING_AUDIT.md#mgef-sndd-version-exception). | [Frostbite pipeline and pair-alignment regressions](../../DreadsMashedPatch.Tests/UserReportedFailureRegressionTests.cs), [ordered equality](../../DreadsMashedPatch.Tests/ListSemanticsTests.cs), [overlay copying](../../DreadsMashedPatch.Tests/OverlayAggregateCopyTests.cs) |
| Translated names | Retain target language and all translations in detached generated copies. Respect Mutagen's language-comparison mode. Required CLAS/EYES/FLOR/KEYM names normalize null to empty; optional names can be absent; MATT Name is a plain string. | [NameTranslationTests](../../DreadsMashedPatch.Tests/NameTranslationTests.cs) |
| Output transactions | Stage context insertion, setters and ancestry before publication. Roll back failed candidates without losing prior parent/sibling edits. All output paths, including priority and NPC, share the transaction. | [RecordOverrideTransactionTests](../../DreadsMashedPatch.Tests/RecordOverrideTransactionTests.cs), [PatchOutputTransactionTests](../../DreadsMashedPatch.Tests/PatchOutputTransactionTests.cs) |
| Binary-overlay groups | Group lookup uses the registered getter interface; an overlay's runtime class is not a supported group key. Do not replace it with root.GetType(). | [RecordOverrideTransactionTests](../../DreadsMashedPatch.Tests/RecordOverrideTransactionTests.cs) |
| Partial publication and diagnostics | Record-local errors discard and report that candidate while successful records publish. Fatal import/type/serialization/publication failures still block files. Structured errors survive flushing. Severity must be a prefix; embedded `<Error: Unknown lstring ID ...>` is ordinary source text. | [PartialPublicationTests](../../DreadsMashedPatch.Tests/PartialPublicationTests.cs), [PatchOutputTransactionTests](../../DreadsMashedPatch.Tests/PatchOutputTransactionTests.cs), [LoggingWarningTests](../../DreadsMashedPatch.Tests/LoggingWarningTests.cs) |
| Initial policies | Priority sources, PreserveBaseline EditorID and Tamriel CELL handling run before ordinary short-history/vanilla exits. Ignored/disabled sources remain ineligible. Targeted identifier indexes avoid expanding every history merely to check policy. | [PolicyFilterTests](../../DreadsMashedPatch.Tests/PolicyFilterTests.cs), [AlwaysWinningModTests](../../DreadsMashedPatch.Tests/AlwaysWinningModTests.cs), [TamrielPersistentCellPolicyTests](../../DreadsMashedPatch.Tests/TamrielPersistentCellPolicyTests.cs) |
| Keyword presence | Selected null removes the list, present-empty remains distinct, and written links are detached. WEAP exclusivity is an additional policy, not a replacement setter. | [KeywordRemovalTests](../../DreadsMashedPatch.Tests/KeywordRemovalTests.cs), [WeaponKeywordListHandlerTests](../../DreadsMashedPatch.Tests/WeaponKeywordListHandlerTests.cs) |
| PHZD and placed projectiles | Review inherited getter/aspect fields, not only declared fields. Reuse compatible PHZD handlers with concrete context narrowing. Their IgnoredBySandbox is a nullable byte slice, not ACHR/REFR's boolean. | [PlacedHazardRecordHandlerTests](../../DreadsMashedPatch.Tests/PlacedHazardRecordHandlerTests.cs), [PlacedProjectileRecordHandlerTests](../../DreadsMashedPatch.Tests/PlacedProjectileRecordHandlerTests.cs) |
| Shared reference groups | ACHR/REFR select Ownership and RagdollPose through shared generic handlers, with separate Placement/Scale; PHZD/seven projectile variants select Transform + Ownership; CELL selects Ownership; ECZN uses the same Ownership handler with byte Rank. Hazards/projectiles retain whole-Transform selection. Spatial selection remains independent of InitiallyDisabled and the complete EnableParent aggregate. Complete competing values and authorized/unauthorized restorations run from source overlays through patch output for every migrated record. ECZN generated Rank setters expand short DATA; unchanged rank writes are skipped to retain old layouts. Fixtures cover zero/5/255, short-source-to-full destination, tail preservation and real legacy-winner publication. Flags retain approved handling. Exact source provenance and per-record boundaries are in the inventory. | [SharedReferenceGroupTests](../../DreadsMashedPatch.Tests/SharedReferenceGroupTests.cs), [REFR ownership/presence/mutation regressions](../../DreadsMashedPatch.Tests/PlacedObjectGroupTests.cs), [precision](../../DreadsMashedPatch.Tests/PlacedObjectPlacementHandlerTests.cs), [owner-null output](../../DreadsMashedPatch.Tests/FormLinkNullSemanticsTests.cs) |
| VOLI | All twelve nullable lighting parameters form one authored preset; independent leaves would mix presets. | [AdditionalRecordSupportTests](../../DreadsMashedPatch.Tests/AdditionalRecordSupportTests.cs) |
| LENS | Only the ordered nullable sprite sequence forms the texture-controlled value; global colour/fade scalars remain ordinary. Retain order, duplicates, nullability and detached nested data. Compare serialized RGB separately from Opacity; do not compare non-serialized CLR alpha. LFSP counts are generated. | [AdditionalRecordSupportTests](../../DreadsMashedPatch.Tests/AdditionalRecordSupportTests.cs) |
| LTEX | Scalar friction/restitution remain independent; snow uses approved nullable flag handling. Absent INAM differs from present zero. Grass membership is sorted by FormID. | [AdditionalRecordSupportTests](../../DreadsMashedPatch.Tests/AdditionalRecordSupportTests.cs) |
| RACE equipment flags | Reuse the approved nullable flag handler with winner-owned unnamed bits and per-named-bit reversion permissions. The reported `-6401` (`0xFFFFE6FF`) and `1791` (`0x000006FF`) have the same named mask (`0x06FF`); upper-bit differences must not trigger forwarding or hide names in initial/context/computed diagnostics. Preserve destination unnamed bits even when another property causes publication. Reference: decompiled Mutagen 0.54.4 `EquipTypeFlag` (bits 0–12), `RaceBinaryOverlay.EquipmentFlags` (four-byte nullable VNAM), and `RaceBinaryWriteTranslation.Write` (four-byte nullable enum); generated via `scripts/Export-MutagenDecompiled.ps1`. Synthetic pipeline and binary-reload tests establish named-bit merging and output preservation, not engine behavior or the intent of USMP’s cleared upper bits. Other RACE specialization remains unchanged; no new handler or obsolete class is introduced. | [RaceEquipmentFlagsTests](../../DreadsMashedPatch.Tests/RaceEquipmentFlagsTests.cs) |
| NPC Perks and GLOB/GMST | Perks must populate an absent destination and preserve selected presence. Subtype histories resolve through the shared group getter and stop at the latest eligible subtype boundary; no older subtype reconnection or scalar conversion. | [UserReportedFailureRegressionTests](../../DreadsMashedPatch.Tests/UserReportedFailureRegressionTests.cs), [diagnostic details](../USER-REPORTED-FAILURES-2026-10-10.md) |
| Asset paths and model bounds | Preserve GivenPath. Final filenames matching the winner retain winning bounds; selecting different filenames carries their source bounds. Bounds are never independently merged, including standalone registrations. See the [model/bounds decisions](#model-and-bounds). ARMA model metadata has a separate comparison policy. | [AssetPathHandlingTests](../../DreadsMashedPatch.Tests/AssetPathHandlingTests.cs), [ModelBoundsHandlerTests](../../DreadsMashedPatch.Tests/ModelBoundsHandlerTests.cs), [ModelBoundsForwardingTests](../../DreadsMashedPatch.Tests/ModelBoundsForwardingTests.cs), [ModelHandlerRegressionTests](../../DreadsMashedPatch.Tests/ModelHandlerRegressionTests.cs) |
| Collection identity | Sorted keys, aligned occurrence IDs, exact positions and whole sequences are distinct policies. Do not use full row equality as a substitute for the reviewed identity key. | [XEditListAlignmentTests](../../DreadsMashedPatch.Tests/XEditListAlignmentTests.cs), [ListSemanticsTests](../../DreadsMashedPatch.Tests/ListSemanticsTests.cs), [ExactOrderedEffectsTests](../../DreadsMashedPatch.Tests/ExactOrderedEffectsTests.cs), [CollectionSemanticsBinaryRoundTripTests](../../DreadsMashedPatch.Tests/CollectionSemanticsBinaryRoundTripTests.cs) |
| ARMO worn meshes | Armature and the complete BodyTemplate.FirstPersonFlags mask select one source through WornMeshes, including slot-only edits, unnamed bits, empty lists and owner-authorized reversions. Approved member handlers remain the sole comparers/setters; ArmorType and legacy general flags keep ordinary forwarding. These synthetic pipeline/binary-output cases establish ARMO selection, not compatibility with independently overridden ARMA or deployed NIF partitions. Reference: pinned xEdit 93cc0bc, Core/wbDefinitionsTES5.pas:2331–2353 and 4073–4084; decompiled Mutagen 0.54.4 BodyTemplateBinaryCreateTranslation/BodyTemplateBinaryWriteTranslation distinguish BOD2 slots/armor type from legacy BODT flags and ActsLike44 encoding state. | [Armor forwarding tests](../../DreadsMashedPatch.Tests/AssetForwardingPolicyTests.Armor.cs), [member handler tests](../../DreadsMashedPatch.Tests/ArmorRecordHandlerTests.cs) |
| OTFT outfit package | Items uses the shared atomic form-link adapter: select an entire source package with ordinary reversion permissions, compare as a multiset and copy detached links in source order. Header flags and EditorID stay independent. The Vigilant conflict fixture uses the reported item FormKeys and proves the winner's three-item package survives instead of the former five-item hybrid. Nullable absence emits no INAM; present-empty emits an empty INAM, retaining presence through source/patch overlays. Reference: xEdit 93cc0bc, Core/wbDefinitionsTES5.pas:7441–7444 (wbArrayS with ARMO/LVLI links); decompiled Mutagen 0.54.4 IOutfit/IOutfitGetter, OutfitBinaryWriteTranslation/OutfitBinaryOverlay and ListBinaryTranslation.Write(IReadOnlyList, RecordType, BinaryMasterWriteDelegate). No slot-aware merge or resolution of referenced LVLI contents is attempted. | [Outfit forwarding tests](../../DreadsMashedPatch.Tests/AssetForwardingPolicyTests.Outfit.cs), [member and source/patch overlay tests](../../DreadsMashedPatch.Tests/OutfitRecordHandlerTests.cs), [ordered atomic regression](../../DreadsMashedPatch.Tests/FootstepSetRecordHandlerTests.cs) |

## Model and bounds

All record types registered with `ModelAndBounds` and ARMO's `WorldModelAndBounds`
share `ModelBoundsHandler`. When model forwarding is enabled, model values keep ordinary selection and declared/
virtual-master reversion permissions. The final selected normalized filenames
determine bounds: matching the effective winner retains its bounds; otherwise
the bounds come from the source of the accepted filename change. The context
retains the winner and filename-source bounds separately so metadata-only edits
and returning to the winner's filenames cannot replace winning bounds. ARMO
compares both male/female paths, including absent sides. Case and slash direction
do not distinguish filenames. Metadata/icons remain in model selection but do
not define filename identity. Optional bounds retain the chosen source's presence.

Independent bounds forwarding was deliberately removed, with no opt-in setting.
A correction such as USSEP's nonzero circlet bounds may be useful, but a later
same-path mesh replacement may intentionally restore original bounds. Plugin
records cannot establish the deployed mesh's geometry. Preserving a baseline
conflict is preferred to introducing a new one while guessing at asset ownership.
Removing only the bounds-only branch is insufficient: a filename accepted earlier
in the history may already match the final winner, whose bounds must then stay.

The [inventory](INVENTORY.md#record-and-property-matrix) gives each record's shared
registration and specialized exceptions. ASPC, DUAL, ENCH, LVLI, LVSP, NPC_, SOUN,
SPEL and TXST have no corresponding main model filename and retain winning bounds.
Their `ObjectBoundsHandler` follows the last eligible override in the shared
chronological pass rather than applying addition/reversion permissions. Its
getter/setter/equality still serve model/bounds copying; no parallel handler is
registered. NPC appearance and referenced models remain specialized. TREE retains
its winner-owned group by default; enabled forwarding uses one dedicated Trees selection independently of the fixed resource classifications. ARMO WornMeshes selects the armature/slot pair separately from dropped-model bounds; approved flag and armature writers retain their copying behavior.

Removed state: independent original/previous bounds and bounds ownership, the
bounds-only selection branch, and the misleading geometry-identity naming. Filename
identity is a path comparison, not evidence that a same-path mesh is unchanged.

`ModelBoundsForwardingTests` discovers every shared registration and covers both
rejected-filename scenarios through patch processing and binary write/reload,
plus winner preservation, ignored bounds corrections, forwarded metadata, and
returning to winning filenames. It also covers both armor genders, absent
counterparts, path spelling, declared/virtual-master reversions, optional bounds
binary presence, restricted TREE, and standalone bounds. These synthetic tests do not inspect
the deployed mesh for a same-path replacement or establish gameplay compatibility.

## Appearance boundaries

NPC HairColor is excluded from both the appearance copy and equality masks, and from
`GroupedProperties`. It has no asset-group membership: ordinary forwarding applies
separately from the appearance group. HCLF directly references CLFM, whereas tint
indices are interpreted using race and sex. An accepted color-only change must not
select the whole face or transfer its owner. No obsolete handler remains: the
existing form-link handler is the only HairColor setter. The appearance regression
tests cover both load orders, separate ownership, declared/virtual-master restoration,
normal grouped forwarding and binary output; help tests keep the displayed groups disjoint.
These tests do not establish visible recoloring of custom hair or FaceGen assets.

Race and Female remain together with the face as a conservative compatibility policy,
not a serialization requirement or a guarantee against blackface. Exact evidence:
[xEdit 93cc0bc, wbTintLayerToStr](https://github.com/TES5Edit/TES5Edit/blob/93cc0bc5a1251936c3c7859eee3150eda12a62d7/Core/wbDefinitionsTES5.pas#L1922-L2015)
resolves tint indices from RNAM and ACBS Female; its bundled
[Reuse faces script](https://github.com/TES5Edit/TES5Edit/blob/93cc0bc5a1251936c3c7859eee3150eda12a62d7/Build/Edit%20Scripts/Skyrim%20-%20Reuse%20faces.pas#L86-L105)
selects donors by race/sex. Mutagen 0.54.4
[Npc.xml](https://github.com/Mutagen-Modding/Mutagen/blob/0188012c607ce8bb283d2704400d37737f089134/Mutagen.Bethesda.Skyrim/Records/Major%20Records/Npc.xml)
and [Race.xml](https://github.com/Mutagen-Modding/Mutagen/blob/0188012c607ce8bb283d2704400d37737f089134/Mutagen.Bethesda.Skyrim/Records/Major%20Records/Race.xml)
expose NPC HairColor and race AvailableHairColors as color-record references, not
race-indexed selections. Weight/WornArmor retain their existing grouping; this
investigation did not justify a change to them.


| Record | Shared selection | Specialized behavior and reason |
|---|---|---|
| NPC_ | Configurable asset policy; opted-in atomic ownership and declared/virtual-master restoration. HairColor uses ordinary forwarding through the shared nullable form-link handler, without an asset gate; Height uses the existing scalar handler; OppositeGenderAnims has an animation-family policy and its own approved masked flag handler. | Face fields, Race, Female, Weight and WornArmor retain generated masked copying as one coherent value. Height/hair-color/animation edits do not replace or take ownership of that face. Appearance, animation and protection writers own disjoint flag masks. |
| RACE | HeadShape selects both genders’ head parts/morph definitions, MorphRace and clamps under the separate Appearance rule. Meshes and Textures own narrower resource slices; preset/hair-color lists and flags use ordinary forwarding. | Male/female rig pairs and BodyPartData use separate animation-scope decisions; combat and movement rows retain ordinary reconciliation. Typed masked HeadData copying preserves sibling decisions. Parent/gender presence is applied first and gates child writes; tint lists retain their authored shared count. See [resource decisions](../ASSET_FORWARDING.md#character-resource-boundaries-and-migration-decisions). |
| HDPT | Meshes controls model, type, extra parts, morph files, texture/color and rendering flags. Eligibility flags and ValidRaces remain ordinary. | Keep model and morph resources together. ExtraParts equality ignores sorting; morph-file order remains significant. |
| TREE | ForwardTreeGroups defaults false and resolves model/bounds, TreeData, scripts and header flags from the effective winner. Enabling it selects one TreeAssets group independently of the fixed resource classifications; name and harvesting retain ordinary policies. | Preserve full CNAM, including eight floats represented as 32 Unknown bytes. There is no model-filename/geometry-owner heuristic to infer compatibility with wind data or same-path mesh replacers. Priority snapshots still take precedence. |

NPC head/tint sorting and HDPT extra-part sorting alone do not claim ownership;
complete entry contents and duplicate counts still matter. Copies retain source
order. Apply configurable winner preservation in the asset policy, rather than hard-coding
it in appearance handlers. Do not reintroduce independent head/tint row merging. TemplateAndTraits remains a separate atomic pointer/Traits
value. Effective template inheritance and deployed FaceGen compatibility are not
resolved by selecting local fields.

Tests: [AppearanceAndTreeForwardingTests](../../DreadsMashedPatch.Tests/AppearanceAndTreeForwardingTests.cs),
[HeadPartRecordHandlerTests](../../DreadsMashedPatch.Tests/HeadPartRecordHandlerTests.cs),
[ProtectionFlagsHandlerTests](../../DreadsMashedPatch.Tests/ProtectionFlagsHandlerTests.cs),
[DiagnosticValueFormatterTests](../../DreadsMashedPatch.Tests/DiagnosticValueFormatterTests.cs).

## Reference evidence

The reviewed xEdit revision is `93cc0bc5a1251936c3c7859eee3150eda12a62d7`.
In `Core/wbDefinitionsTES5.pas`, LTEX is at lines 8000–8020 and VOLI/LENS at
10206–10243; placed-projectile registrations at 4184–4191 use the shared
ReferenceRecord definition. Collection evidence is retained in the
[ordering references](../history/README.md). Regenerate ignored Mutagen and
xEdit trees with the [reference exporters](../REFERENCE_SOURCES.md) after version
changes. They are reference-only inputs, not project code.

## Asset policy regression scope

[AssetForwardingPolicyTests](../../DreadsMashedPatch.Tests/AssetForwardingPolicyTests.cs)
covers family defaults and opt-in, all typed nested/inherited resource owners,
complete snapshots through binary reload, no protected-only output, structural
reset precedence, permission, priority/ignored sources, TREE, translated book
markup and disjoint nullable/scope masks and ordinary race flags. The concrete nullable flag setter
keeps its merged bits; zero water clears retain destination presence. Multi-member
list exceptions retain independent xEdit order/key provenance in the reviewed
expectations. Shared selection/flag/list algorithms are unchanged. The common application loop orders structural writes before nested resource writes and restricts references to surviving rows/parents.

## Validation scope

The interior-only relocation change was checked on 2026-11-05: all 2,324 tests
passed, Release built without warnings/errors, and strict coverage reported 131
handlers with no missing/partial candidates. These are structural/selection checks,
not validation of the installed Gestur modlist or gameplay.

A temporary harness compared actual ACHR `Process` calls against `e3a518f` on
Linux x64/Ryzen 5 7600, SDK 10.0.401, Release/default runtime. Each fresh process
used binary overlays, 10,000 references, 100 interior cells and three histories;
figures are five-sample medians after one warm-up. Setup, initial reference
indexing and serialization are excluded; selection and publication are included.

| Scenario | Before → after (ms) | Runtime change |
|---|---:|---:|
| Unchanged, no output | 580.4 → 598.1 | +3.0% |
| Scale edits, 10,000 overrides | 857.8 → 895.0 | +4.3% |
| Scale edits with 10% interior moves | 837.1 → 872.9 | +4.3% |

Cumulative current-thread allocation changed by −0.5% to +0.8%. Process peak
working set, including setup, stayed within 1%. Relocation now fixes ancestry,
so that scenario is not an equivalent-output speed comparison. These synthetic
figures do not establish whole-modlist timing or crowded-cell scaling.

On 2026-10-27, the solution (core, tests and WPF desktop) built with zero warnings
or errors, and the full suite passed **2,077 tests**. Strict property coverage
reported 131 handlers with zero missing/partial candidates. Collection verification
covered 209 registrations/expectations with zero unresolved entries or failures.
The generated catalog contains 72 record handlers, 161 groups and 235 registered
member values. All 161 public group/forwarding rows matched it; local public links and
anchors resolved. settings.md has one shared eligibility link and no source-file links.

Mixed-setting regressions cover ARMO mesh/icon independence and clear order, ARMA
gender mesh packages versus skin/gameplay, HDPT resources versus eligibility, EYES
binary-overlay texture access, and RACE shape/mesh/texture/choice separation with
absent-parent gating. TREE tests cover its sole group, independence from all mesh/script
toggle combinations, complete script-list selection, latest wind/model coupling,
retained bounds provenance and expanded UI help. Animation regressions cover per-gender
rig selection and detached overlay copies, independent combat/movement and legacy
IDLE fields, IDLM/PACK ordered idle packages, and nullable/unknown marker flags. Settings tests verify that saved resource/appearance keys cannot disable normal forwarding, serialization omits those keys, and the separate tree setting survives JSON/Copy/runtime snapshots. All property groups retain their existing selection/copying boundaries; no compatibility aliases remain. Future control restoration notes are in the architecture guide.

Reference-scope regressions also cover REGN grass content and independent icon/audio gates, destruction gameplay versus stage meshes, keyed BPTD limb resources, EFSH atlas fields versus ordinary effects, ordinary camera/impact/sound/particle/water/material tuning, reversed write order and parent/row removal. Overlay/reload checks verify detached stage-model and limb-model data. BookText remains ordinary translated markup. The resource scope inventory and retained dependencies are consolidated in the architecture guide.

These checks establish selection, copying, serialization and documentation
consistency. They do not verify deployed assets, in-game rendering or desktop
visual layout.

A 2026-10-05 disk-backed check used the desktop runner services through a temporary
native Linux harness with the maintainer's real load order. Output reloaded with
2,838 unique major records and 131 masters. This established that run's input,
transaction and binary publication path, not universal field preservation,
Proton desktop integration, xEdit conflict review or gameplay correctness.
UI rendering evidence and platform gaps are summarized in
[Standalone development](../STANDALONE_DEVELOPMENT.md#validation-scope).
