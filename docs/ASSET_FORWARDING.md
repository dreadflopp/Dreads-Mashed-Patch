# Asset forwarding architecture and invariants

[Internal index](README.md) · [Development guide](DEVELOPMENT.md) · [Runtime ownership manifest](asset-forwarding-groups.json)

## Decision and source of truth

All resource and appearance forwarding is fixed on by
[FixedForwardingRules](../DreadsMashedPatch/AssetForwarding/ForwardingRules.cs):
Meshes, Textures, Appearance, Animations, Sounds, Music, Scripts and IconsAndImages
are public compile-time constants set to true. They are core policy, not serialized
settings or UI controls. ForwardingSettings.Allows routes those classifications to
the constants. ForwardTreeGroups is the only persisted option for these groups;
it defaults false and remains available under Smart Forwarding → Tree forwarding.

Groups, member handlers, selection permissions, bounds provenance, structural
resets, nested-reference writes and graph validation are retained. No property was
ungrouped or migrated to another handler in this change. The mutable branch-only
asset/appearance settings and asset-enabling test helper were removed; there are
no compatibility aliases. Unknown saved resource/appearance keys are ignored by
normal JSON deserialization and cannot disable the fixed policy.

The exact groups and members are declared in
[AssetGroupCatalog](../DreadsMashedPatch/AssetForwarding/AssetGroupCatalog.cs).
The checked-in JSON manifest is generated from constructible runtime handlers.
ForwardByDefault uses ForwardingSettings.Allows directly. The dedicated Trees rule
maps to ForwardTreeGroups and defaults false:

```bash
dotnet run --project tools/AssetForwardingAudit -- docs/asset-forwarding-groups.json
```

Public defaults, reasons and boundaries are in
[smart asset forwarding](record-patching/assets.md); each public record page lists
its exact group members. Keep internal implementation evidence here, not in those
pages or the root README. The manifest counts registered values, not filenames,
individual records written, deployed files, or a complete dependency graph.

The desktop displays only **Forward tree changes as a group**, unchecked by
default, with short On/Off explanations and no expandable member details. The asset and
appearance sections have been removed from MainWindow.xaml, including their
bindings. They are not merely invisible editable settings.

[AssetRuleHelpCatalog](../DreadsMashedPatch.App/Models/AssetRuleHelpCatalog.cs) retains
metadata for every runtime group so a future controls UI can reuse its exact field
expansion. It has no current view-model or XAML binding. Help tests cover the
retained metadata and tree explanation. Metadata never
selects or writes values.

## Restoring asset or appearance controls later

1. Keep AssetGroupCatalog and every grouped member intact. ForwardingRule remains
   a resource classification; AssetForwardingPolicy already has the winner branch
   needed for a restricted group. Do not introduce a parallel writer or independent
   member contexts.
2. Add an explicit settings model for the eight classifications, with defaults,
   Copy and Normalize behavior. Route ForwardingSettings.Allows through those
   instance values instead of FixedForwardingRules. Keep ForwardTreeGroups separate
   and false by default. Update the core configuration first so the desktop and
   Synthesis share one runtime policy. Current files contain no mutable asset or
   appearance keys; choose any future migration policy explicitly.
3. Restore only the desired controls in MainWindow.xaml. Use AssetRuleHelpCatalog
   and AssetRuleDetailsTemplate for membership help; do not hand-maintain a second
   list. Appearance remains a distinct broader group, not an asset-file switch.
4. Restore restriction regressions for all eight classifications: saved false
   values through JSON/Copy/PatcherSettings.Apply, effective-winner selection after
   structural resets, no asset-only output, nested parent/row removal, nullable
   flags without phantom field presence, unrelated ordinary fields, model/bounds,
   VMAD metadata and group reversion permissions. Current tests deliberately
   enforce fixed true; update those policy assertions when controls become real.
5. Update public UI guidance and group-table forwarding columns, then regenerate
   ownership/collection reports and run strict coverage, full tests and desktop
   build. Keep source provenance and scope decisions below; controls must not
   broaden groups or bypass approved flag handlers.

Current regression anchors: FixedForwardingCannotBeDisabledBySavedResourceOrAppearanceFields,
TreeOptionRoundTripsWithoutChangingFixedGroups,
FixedGroupsForwardAndTreeOptionControlsItsGroupThroughBinaryOutput,
DefaultAssetFamiliesForwardEarlierGroupChanges and TreeOptionControlsOnlyItsCompleteGroup.
Old mutually exclusive permission cases were replaced by normal-forwarding cases;
there is no test-only mutable override of the constants.

## One selection path, existing copy paths

Every participating record overrides existing initialization and final
coordination hooks. Initialization creates normal typed contexts, then
ResolveMemberContexts marks grouped member contexts resolved. Those members
therefore have **no independent forward-pass decision**. Non-members retain their
previous algorithms. Getters, equality, setters, null handling and generated
copying remain in the registered handlers; they are not duplicated by the policy.

AssetForwardingPolicy owns the sole final choice for those members. Only disabled
Trees currently reads the final eligible winner. Allowed single-member groups reuse that
member's context algorithm: complete values stay atomic, model/bounds keep their
specialized provenance, and ordinary VMAD script lists retain complete-row merging
and destination metadata. Allowed multi-member groups use a selection-only
snapshot adapter through AbstractPropertyHandler's existing addition/reversion
algorithm, then write only through member setters. This adapter is never a second
registered writer. Do not register former independent decisions in parallel.

Enabled snapshot groups react to any member change, including metadata-only edits
at an unchanged filename. Disabled groups preserve all members from the winner.
Do not add an independent metadata bypass: it would violate group coherence.
ModelBounds retains its explicitly documented bounds-only comparison exception.
The Scripts classification owns VMAD only, including QUST/SCEN metadata/attachments/fragment bindings and PERK's complete adapter. Structural reset and graph validation are existing record invariants, independent of VMAD selection. Ordinary list-style VMAD entries retain their merging and destination-metadata policies.

The shared application loop now writes ordinary containers/lists before handlers implementing INestedAssetPropertyHandler. RestrictToTarget normalizes the selected reference snapshot to surviving parents/row occurrences before applying and validating it. Parent removals take precedence and references cannot recreate entries. This is write coordination; shared addition/reversion, flag and list reconciliation algorithms are unchanged.

ProjectedPropertyHandler composes the existing typed aggregate reader/comparer/copier with explicit projection and merge functions. Its normal and resource projections preserve each other's destination fields. BodyPartRowsHandler derives the existing generated list adapter to retain keyed reconciliation. No whole-carrier gate or duplicate property context remains registered. Runtime group ownership metadata describes actual fields rather than assuming the aggregate key owns every descendant.

Final coordination selects declared asset groups, refusing overlapping keys. ACHR/REFR header flags, Transform and EnableParent retain ordinary independent contexts; no disabled-state snapshot is composed into asset decisions.
Forwarding.ForwardTreeGroups maps to ForwardingRule.Trees and defaults false. One
TreeAssets catalog group controls model/bounds, TreeData, VMAD script lists and
supported header flags. All non-tree classifications are fixed true; they do not gate TREE.
Branch-only settings use no compatibility aliases or inverted legacy readers.
Whole-record priority and specialized Tamriel CELL routes retain earlier precedence;
ordinary grouped forwarding does not post-process their explicit source snapshots.

## Structural reset and model bounds

Structural ownership resets reinitialize contexts using an intermediate override
as both original and winner. Do not use that intermediate argument as the final
winner of a restricted group. Final coordination reads the actual eligible history
and finds its latest semantic structural reset. Enabled groups start at that
baseline, while restricted groups always read the final effective winner.
QUST/SCEN/PERK VMAD packages keep scripts and bindings together. Structural reset triggers remain unchanged; they are not members of the asset-reference gate.

Multi-member geometry groups compare model payloads without bounds to decide
ownership. On an accepted filename change, capture that source's bounds. Metadata
or other group changes at the same selected filename cannot take bounds. Final
filenames matching the effective winner retain winning bounds; otherwise the
accepted filename source supplies bounds. ModelBoundsHandler exposes its existing
identity/comparison internally for this purpose; there is no competing geometry
heuristic. ARMO's filename pair and nullable bounds retain their existing policy.
Standalone/unassociated OBND remains winner-owned. Do not invent bounds for worn
ARMA, scope, skeletal, morph, stage or LOD meshes.

## Character resource boundaries and migration decisions

| Record | Generalized selection and retained specialization | Reason and ordinary fields |
|---|---|---|
| ARMO | WorldModelAndBounds uses Meshes; WornMeshes selects Armature and the complete BodyTemplate.FirstPersonFlags mask in one shared snapshot. Approved flag and atomic-link-list handlers remain the only comparers/setters; their independent contexts are inactive. Two shared-icon adapters use IconsAndImages. WorldModelHandler snapshots only models; mesh/icon setters preserve each other’s fields. | Independent bit ownership could pair an earlier slot conversion with a later addon list or combine masks no source supplied. Slot-only changes also select the complete pair. BodyTemplate.Flags, ArmorType and Race retain ordinary handlers because gameplay edits do not select worn geometry. ActsLike44 remains excluded serialization state. |
| ARMA | One mesh snapshot per gender includes both views, alternate textures and that gender’s weight-slider switch. One Textures snapshot per gender owns skin/swap references. Typed setters and detached model Data copying remain. | Weight-slider state is shared across views. BodyTemplate, Race and AdditionalRaces are eligibility/gameplay fields, not filename-indexed mesh rows; they keep approved flag/list/scalar policies. Byte-only Data differences still do not claim filename ownership. |
| HDPT | HeadPartMeshHandler replaces the appearance handler completely. Meshes owns model, TRI pairs, ExtraParts, type, texture/color overrides and IsExtraPart/UseSolidTint. | Mesh/morph/rendering data needs one definition. ValidRaces and Playable/Male/Female flags remain independent link/approved flag decisions. ExtraParts comparison ignores sorting and retains duplicates; morph order stays significant. |
| EYES | Icon uses Textures. The specialized path comparer remains; the getter accepts IEyesGetter. | xEdit labels ICON Texture, not an inventory icon. Mutable-only getter detection previously lost binary-overlay values. Name and flags remain ordinary. |
| RACE | Shared catalog selection replaces RaceAppearanceHandler. Per-gender RaceHeadDataHandler uses typed HeadData masks for Shape, Mesh, FaceTextures and Tints. HeadShape combines both Shape slices, MorphRace and clamps under Appearance. | Models use Meshes; Skin is a Meshes selector; BodyData is the EGT Textures aggregate. Face details/default texture use Textures. Both tint lists and authored NumberOfTintsInList use one Textures source. Choices merge by FormKey; flags, height, weight and gameplay retain ordinary rules. |
| NPC_ | Existing masked face snapshot stays under separate Appearance. | HairColor has no resource gate and uses the ordinary nullable FormLink handler. Height and OppositeGenderAnims retain separate scalar/animation decisions. |

RACE’s parent/gender presence follows the QUEST VMAD gating pattern: apply presence
first, suppress children of absent containers, preserve sibling fields on every
masked write. Absence and empty fields compare equally for each resource slice;
structural presence has its own context. The generated Race-level gendered copy
bypasses nested masks, so copy each HeadData directly instead. Deep-copy selected
fields before DeepCopyIn to avoid sharing mutable rows with source overlays.

Reference evidence: pinned xEdit Core/wbDefinitionsTES5.pas has TINL and clamps at
9385–9387 and independent head-data definitions at 9469–9500. RacePresets and
AvailableHairColors are wbRArrayS FormID lists. Mutagen 0.54.4 HeadDataMixIn and
HeadDataSetterTranslationCommon implement field masks; RaceBinaryWriteTranslation
writes NumberOfTintsInList as supplied, not derived. Keep its authored count with
both tint lists instead of guessing a count formula. ArmorAddonCommon exposes
weight-paired resources for both first/third-person views; WeightSliderEnabled is
one gendered parent field. BodyData.Model uses SkyrimBodyTextureAssetType (.egt),
not SkyrimModelAssetType (.nif). xEdit EYES defines ICON as Texture. These are
source/schema boundaries, not an in-game compatibility guarantee.

Obsolete broad ARMA/ARMO/RACE appearance groups and HeadPartAppearanceHandler /
RaceAppearanceHandler were removed. Single registered setters remain for every
property. Unregistered serialization/unused fields retain their prior exclusions;
no compatibility layer is needed for these unpublished branch settings.

## Animation and skeleton boundaries

The Animations rule controls references and resource selectors, not every field
related to actor motion. RACE has MaleRig and FemaleRig: each combines that gender’s
SkeletalModel and BehaviorGraph. BodyPartReference selects BodyPartData separately.
AttackRace, Attacks, MovementTypeNames and MovementTypes retain their existing
ordinary field/SortedKeyed handlers; numeric combat/speed edits cannot claim rig
ownership. The NPC OppositeGenderAnims selector remains separate from appearance.
No behavior-event, skeleton-node or template-inheritance solver is implied.

RACE reuses GenderedItemSideHandler, generalized to separate getter/mutable item
types just like GenderedItemHandler. All ARMA call sites migrated to the same four-
type adapter; their flag/link/value behavior is retained. Copy selected and opposite
sides through the typed copier, preserving nullable genders and avoiding mutable
source sharing. A fully cleared optional SkeletalModel wrapper normalizes to null;
BehaviorGraph retains its required wrapper. Old whole-gender rig registrations and
SkeletonAndBehavior are removed, not kept as parallel selection paths.

IDLE IdleReference includes only Filename and AnimationEvent. Conditions and legacy
DATA fields retain ordinary forwarding, including approved flag handling. xEdit
93cc0bc Core/wbDefinitionsTES5.pas:7701–7725 labels DATA unused and marks it cpIgnore.
Mutagen 0.54.4 still exposes it; this does not prove every engine use is absent.
Separate those fields from resource ownership without removing their handlers.

IDLM IdleSelection keeps its ordered Animations list, Flags and IdleTimer together;
its marker ModelAndBounds stays under Meshes. FlagsHandler derives from the approved
SimpleReflectionFlagPropertyHandler and only adds WaterFlagsHandler’s absent-zero
normalization. Unknown destination bits survive. The writer derives IDLC from list
count (IdleMarkerBinaryWriteTranslation.WriteBinaryAnimationCountCustom); copying
preserves declared order and nullable list state. PACK IdleSelection reuses the
existing generated PackageIdles copy of Type, TimerSetting and ordered IDLE links;
VMAD and event structures retain their own owners. No new selection/writer handler
is added for either idle package.

Reference provenance: decompiled Mutagen 0.54.4 IRace/IRaceGetter, ISimpleModelGetter,
IModelBehaviorGetter and IPackageIdlesGetter establish typed surfaces. Pinned xEdit
Race definitions at 9374–9455 separate rig paths, attacks and speed overrides;
wbAttackData at 2925 declares AttackEvent-keyed rows. wbDefinitionsCommon.pas:8626
keeps idle lists with flags/timer and a derived count. Schema boundaries justify
these forwarding scopes; they do not validate deployed HKX/NIF compatibility.

Regression entry points in AssetForwardingPolicyTests: RaceGameplayEditsCannotTakeOwnershipOfRigReferences,
RaceGameplayAndRigGroupsForwardNormally, RaceGenderRigPairsSelectIndependentlyThroughBinaryOutput,
RaceRigOverlayCopiesAreDetachedAndCanClearOneGender, IdleLegacyDataCannotSelectTheFilenameAndEvent,
OrdinaryIdleFlagsPreserveWinnerUnknownBitsThroughBinaryReload,
IdleMarkerAnimationPackageIsSeparateFromItsMarkerMesh,
PackageIdleSelectionForwardsWithItsSettings. Help tests check the exact
Animation scopes and expanded package fields. Collection expectations restore RACE
row reconciliation and retain IDLM’s atomic ordered policy inside IdleSelection.

## Scope review: resource references versus ordinary data

The boundary is a resource reference plus data required to interpret that reference. An authored visual preset alone is not enough reason to gate unrelated content. Trees and appearance are explicit broader exceptions. The following inventory covers each distinct group shape in the runtime catalog; repeated main models/icons/VMAD adapters share these decisions.

| Group shape / records | Retained dependency and scope | Ordinary or intentionally excluded fields |
|---|---|---|
| Main models / ARMO / STAT | Full model metadata and filename-indexed alternate textures; associated bounds follow filename provenance. ARMO WornMeshes pairs its addon list with its complete biped slot mask separately from dropped models. STAT LOD levels describe the same object. | Armor class/general flags and other gameplay, standalone bounds, ARMO icons and race. |
| ARMA / HDPT / RACE / NPC_ | Character resource scopes above retain their typed copies, masks and eligibility split. | Hair colour, height, choices and gameplay; Appearance remains the explicit coupled face/head-shape exception. |
| Destruction owners (all catalog registrations) | Destructible.StageModels selects ordinal models, preserving File/Data/AlternateTextures only for surviving stages. | Health, counts, stage flags, explosion/debris links and stage membership retain an ordinary atomic Destructible projection. |
| BPTD | Independent parent Model and Parts.LimbMeshes; limb model, replacement scale and model-info hashes match PartNode plus duplicate occurrence. | Damage, health, eligibility/flags, gore targeting/placement and row membership retain SortedKeyed reconciliation. PoseMatching has no verified path interpretation. |
| ANIO / ARTO / PROJ / WEAP / LSCR | NIF unload event, art interpretation type, muzzle-flash geometry/enable/time, scope model/enable/FOV, and loading-screen model framing are local resource interpretation packages. | Projectile trajectory, weapon gameplay and loading-screen conditions remain separate. |
| CAMS / IPCT / MATO / FURN / CLMT / WTHR | Complete Model/ModelFilename/Aurora references use Meshes; MATO's NIF is a model, not an image path. | Camera target/timing/flags, impact placement/duration, MATO material parameters/flags and opaque DNAM bytes stay ordinary. |
| DEBR | Complete ordered weighted model choices retain their per-model flags/selection data. | No unrelated gameplay is owned by the model list. |
| TXST / EYES / LTEX / RACE textures | Typed texture maps with texture interpretation flags, eye texture, material texture-set selector, EGT/head/tint packages as documented above. | Decal metadata in TXST and LTEX material physics/grass membership stay ordinary. |
| EFSH | MembraneTextures includes fill/holes/palette plus FillTextureScaleU/V. ParticleTextures includes particle/palette plus TextureCountU/V and ParticleAnimated frame/loop indices/variations. | Remaining EffectShaderData keeps its typed atomic semantics independently: blend/colour/timing/particle motion, AddonModels, AmbientSound, Unknown and Flags. The existing DATA writer is retained, not a new generic flag writer. |
| SPGD / LENS | ParticleTexture travels with NumSubtexturesX/Y. Ordered flare sprites retain ID, texture and per-sprite DATA. | SPGD type/motion/density/size and LENS global ColorInfluence/FadeDistanceRadiusScale are ordinary. |
| WATR | Each noise layer's path/UV scale is independent. FlowTexture owns flow-normal path, map scale and approved surface-flag mask. | Wind, amplitude, falloff, damage and remaining water behaviour. |
| CELL / WRLD / CLMT / WTHR textures | Paired water paths, HD LOD maps, sun/glare, world-map image/framing and indexed cloud textures/layer rows retain their local image interpretation. | Water record selection and unrelated world/climate/weather data remain ordinary. |
| RACE / IDLE / IDLM / PACK / NPC_ animation | Per-gender rig, body-part selector, event/file, idle choice packages and animation gender keep the documented resource-selector boundaries. | Combat, movement tuning, legacy IDLE DATA and conditions stay ordinary. |
| SNDR / MUST | SoundFiles+Type+LoopAndRumble; music track/finale, type, duration/loop/cues and ordered palette choices retain file-specific playback interpretation. | SNDR pitch/volume/routing/category/alternate/subtitle and all audio conditions/priority stay ordinary. |
| VMAD / QUST / SCEN / PERK | Script attachments/properties and adapter-specific metadata/fragment bindings only. Existing structural reset and validation remain specialized. | Quest stages/aliases/objectives/conditions, scene actors/actions/phases and perk tree/effects are outside the resource switch. |
| Icon pairs / REGN / BOOK | Only paired Icons, including inherited region Icons; REGN Sounds.Sounds rows use Sounds and Sounds.Music uses Music. | REGN grasses, objects, weather, map text, entry presence/flags/priority stay ordinary. Complete translated BookText, including embedded markup, stays ordinary: no invented parser or partial prose substitution. |
| TREE / Appearance | Dedicated broader policies documented separately; neither is folded into resource gates. | Harvesting/names and ordinary hair colour remain outside. |

RegionDataHandlers projects inherited Icons and audio away from the ordinary child definition. It retains typed generated copies, including aggregate flags; nested references cannot materialize a missing child. xEdit REGN defines RDGS as GRAS/LTEX choices, not images. Mutagen 0.54.4 exposes its second grass member as opaque Unknown; do not silently reinterpret it as a typed link. Region flags/priority and authored nested row structures remain intact.

DestructibleDataHandlers explicitly strips Model from detached stage copies: the generated parent DeepCopy does not propagate the nested stage translation mask. Restriction uses zero-based stage position and trims absent model tails. BodyPartRowsHandler retains duplicate PartNode occurrences (case-insensitive identity), strips only limb resource fields from ordinary copies and writes resource snapshots only to surviving occurrences. Typed model/Data/AlternateTextures copies remain detached.

EFSH uses root-level generated masks with its existing EffectShaderDataHandler comparison/setter. Normal DATA excludes only coordinate/atlas members, and each resource slice excludes all other DATA. Declaration-based ownership metadata makes property audits precise. The coverage tool now reads constructible registries, including factory-created projections; source matching is used only for line locations. This also exposed IDLE RelatedIdles as deliberately inactive (formerly miscounted inside a block comment); its intentional exclusion is now explicit.

Provenance: [xEdit pinned TES5 definitions](https://github.com/TES5Edit/TES5Edit/blob/93cc0bc5a1251936c3c7859eee3150eda12a62d7/Core/wbDefinitionsTES5.pas) (BPTD around 6015–6081, REGN around 9890–9969) and [Mutagen pinned Region schema](https://github.com/Mutagen-Modding/Mutagen/blob/0188012c607ce8bb283d2704400d37737f089134/Mutagen.Bethesda.Skyrim/Records/Major%20Records/Region.xml). Local reference is decompiled Mutagen 0.54.4: IRegionData/IRegionGrasses/IRegionSounds, IBodyPartGetter/BodyPart.TranslationMask, DestructibleSetterTranslationCommon, DestructionStage and EffectShaderMixIn/TranslationMask. Source surfaces establish scope/copying, not deployed-file compatibility.

Regressions: AssetReferenceScopeTests covers mixed permissions, ordinary gameplay/tuning, shader atlas separation, duplicate row identity, reversed write order and parent/row removal. Existing overlay, script graph and collection tests remain the regression boundary. Removed broad carrier ownership (BookImages, BodyGeometry, CameraPlayback, MaterialDefinition, ParticleDefinition, ShaderDefinition, ScriptStructure, NoiseAndFlow) has no compatibility aliases or active alternate selection path.

## TREE selection boundary

TreeAssets reuses the multi-member snapshot policy; TreeRecordHandler uses the same
initialization/final-coordination pattern as other participating records. Remove
record-local winner-context resets and duplicate catalog owners when adding this
single group. Name, EDID and harvesting remain outside because they do not define
tree resource compatibility. No separate forwarding gate is needed in policy,
audit tools or UI help: ForwardingSettings.Allows(Trees) supplies the default and
runtime permission in one place.

The group selects an authored model/wind/script/flag package. ModelBounds keeps
its filename/source-bounds exception; the shared script setter retains destination
VMAD Version/ObjectFormat and copies selected complete entries with the existing
Papyrus unused-data policy. Header flags keep their approved masked writer,
preserving unknown winner bits. CNAM retains its opaque eight-float block exactly.
A later wind-only or script-only edit can select that source's model path; a
bounds-only edit cannot claim group ownership. Same-path deployed replacements
remain undetectable. Priority snapshots still bypass ordinary group selection.

Regression entry points: TreeOptionControlsOnlyItsCompleteGroup covers both tree choices; TreeGroupKeepsModelWithLatestWindEdit verifies the
changed coupling; TreeRestrictionPersistsAndControlsAppearanceScriptsAndFlags
covers JSON/runtime state and complete script-list selection. Existing model/bounds
binary tests also run through TREE’s shared snapshot policy. Retained help-catalog tests require
TREE to appear exactly once with all four members and expanded CNAM fields.

## Disjoint flag ownership

Use approved masked setters/comparers. WEAP HasScope belongs only to Scope, while
remaining Data.Flags keep their ordinary handler. RACE FaceGenHead uses the ordinary approved race flag handler. NPC appearance, OppositeGenderAnims and
protection masks retain disjoint ownership.

WATR separates CausesDamage from EnableFlowmap/BlendNormals using typed adapters
around the approved masked flag handler. Rendering flags select with FlowTexture;
damage, DamagePerSecond and Spell stay separate. A zero selection retains destination
presence; nonzero bits materialize a field if necessary. If texture restrictions are restored, a rejected flow-only edit must not leave a
present-zero flags-only patch; add that off-state regression with the controls.
Unknown and other-owner bits survive either setter and either write order.

The concrete SimpleReflectionFlagPropertyHandler formerly replaced its merged
value with the raw input for nullable enums, losing preserved unowned bits. The
nullable write now retains the already merged boxed enum. This is a concrete
setter correction; abstract flag selection algorithms were not modified.

## Collections, diagnostics and audits

A list inside a multi-member group is a complete source value, even if its
registered member handler supports row reconciliation. Its independent context is
resolved and is never run. Collection discovery reports Atomic for those members.
The reviewed xEdit expectations retain the original key/order facts and mark this
intentional group-selection exception. Copying still preserves order, duplicates,
nullability and the member's generated payload copies.

Per-record diagnostics identify group name, family and either preserve-effective-
winner or forward-coherent-group. Existing property decisions display the computed
values. Do not report this as detected file replacement or verified compatibility.

Run property coverage, collection verification, the asset manifest generator and
meaningful integration tests after changing membership. PowerShell wrappers call
the tools directly; on hosts without pwsh, the equivalent commands are:

```bash
dotnet run --project tools/RecordHandlerCoverage -- . docs/record-handler-coverage.md docs/record-handler-coverage.json tools/RecordHandlerCoverage/coverage-overrides.json --fail-on-unresolved
dotnet run --project tools/CollectionSemanticsAudit -- . docs/collection-semantics-manifest.json docs/xedit-collection-expectations.json docs/collection-semantics-comparison.md --verify --fail-on-unresolved
```

## Unchanged and deliberately unresolved boundaries

Every category can be replaced without changing its filename. O=a, A=b, B=a can
mean accidental carry-forward, deliberate restoration, or replacement bytes at a.
These have indistinguishable plugin paths. Current file existence or hashes cannot
establish historical provider intent. A unique path and a master relationship are
not compatibility evidence. No resource resolver, asset copying/generation or
cross-record solver was added.

The direct typed-asset audit stops at form links; indirect routes are protected
only where explicitly listed, such as worn equipment and LSCR framing. Ordinary
placed Base and arbitrary other record identities retain their gameplay rules.
Do not claim every reference transitively selects matching resource contents.

Reviewed all seven typed asset classes in Mutagen Skyrim 0.54.4 and all eight
custom inferred/resolved containers: ArmorAddonCommon (weight counterpart),
BookCommon (image markup), CellCommon (flowmap), DialogResponsesCommon (voice),
NpcCommon (FaceGen), QuestCommon (SEQ), ScriptEntryCommon (PEX/PSC) and SkyrimModCommon
(localization). These establish additional dependencies, not independent filename
fields. PSC is source material; PEX is executable script data. Config/INI assets,
mesh-embedded paths and animation generation remain outside this policy.

ARMO RagdollConstraintTemplate and FACT rank Insignia retain existing handling;
Skyrim runtime meaning was not established sufficiently to create new filename
rules. BPTD PoseMatching is not reinterpreted as a path; it retains its ordinary body-part row decision. Arbitrary script strings, translated/GMST resource identifiers,
and other markup have no universal schema. CanopyShadow and UnusedNoisemaps remain
excluded. No blanket ownership of other RACE runtime flag bits was guessed.

REGN inherited Icons and audio selections have independent nested projections;
entry presence, flags, priority and ordinary contents remain separate. BookText
retains complete translated markup under ordinary forwarding, without an embedded
image gate. FURN XMRK stays separate from main geometry; no undocumented marker
coupling is assumed.

Reference provenance: decompiled Mutagen **0.54.4**, regenerated using the
[reference exporter](REFERENCE_SOURCES.md), is reference-only. xEdit checkout is
`93cc0bc5a1251936c3c7859eee3150eda12a62d7`; field definitions and serialized boundaries
come from Core/wbDefinitionsTES5.pas. Existing regression decisions remain relevant under the fixed forwarding policy.

## Regression entry points and validation limits

[AssetForwardingPolicyTests](../DreadsMashedPatch.Tests/AssetForwardingPolicyTests.cs)
checks fixed-policy JSON/runtime invariants, disjoint membership and active hooks,
nested/inherited typed-asset coverage, the tree exception, complete
snapshots, permission, structural resets, flags, priority/ignored mods, TREE,
translations and binary output. Existing model/bounds, appearance, script metadata,
placed reference, transaction and overlay tests retain their algorithm coverage
with normal grouped forwarding. Audit regressions compare runtime
collection modes to independently reviewed expectations.

Synthetic and binary tests establish policy and copying behavior, not deployed
resource compatibility, mod-manager provenance or in-game results. Record exact
validation results in the current regression reference with the date and scope;
do not carry old aggregate counts forward as certification.
