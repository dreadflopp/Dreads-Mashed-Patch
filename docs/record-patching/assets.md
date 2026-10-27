# Resource and appearance forwarding

[Shared patching rules](README.md) · [Record index](INDEX.md)

An asset is a file a plugin points to, such as a 3D model, texture, animation,
sound or script. Except for the broader appearance and tree groups, each group contains asset references and only the data needed to interpret them together. Unrelated gameplay, text and tuning keep their normal decisions. Grouped forwarding selects plugin references and related settings; it
does not copy or modify the referenced files.

Mods can change which file a plugin uses, or replace the file without changing
its name. The patcher cannot detect file replacements, so a later plugin may
look unchanged while using an updated file. Forwarding an earlier group may
undo that update or apply settings that no longer match.

## Normal grouped forwarding

Resource references and appearance groups always use their normal forwarding rules.
There are no asset-family or appearance switches in the UI or saved settings.
A group is selected and copied together, using the same change/reversion permissions
as other complete values. This does not mean that every earlier edit is accepted:
Priority Mods, ignored sources, structural ownership and record-specific rules still apply.

**Forward tree changes as a group** is the only option for these groups, under
**Smart Forwarding → Tree forwarding**. It is unchecked by default. Off keeps the
winning TREE resource group; on allows that group to forward. Other TREE fields
retain their ordinary decisions. Expand **Record types and grouped properties**
beneath this option to see its members. Record Types controls whether each record
family is processed.

Appearance remains grouped: NPC face fields, race, sex, weight and skin select
together, and RACE head-part/morph definitions select with their morph race and
clamps. HairColor and Height retain their independent ordinary decisions. Armor,
head-part and eye resources retain their narrower groups.

A mesh can need its alternate textures or texture override to render correctly,
so those fields stay with that mesh. The classifications in the record tables
(Meshes, Textures, Animations and so on) describe the reference type; they are
not configurable forwarding rules. The patcher does not identify historical file
bytes or the provider of installed resources.

## What stays together

The individual record pages enumerate every group's exact properties and default.
New multi-property groups select a complete source snapshot under the
[shared change/reversion rules](README.md#how-a-value-is-selected). Already atomic
values retain their copying/comparison rules. Ordinary single-list script adapters
retain complete script-entry merging including destination metadata.
An asset group has one owner; its member fields do not also merge independently.

Changes to required non-filename members also count as changes to a group: a music-duration edit, texture-atlas change or shader UV-scale edit can select the group without renaming a file. Ordinary volume, colour, motion or gameplay edits do not claim resource ownership. The model-bounds exception below still applies.

For nested references, parent and row membership comes first: forwarding cannot restore a deleted region entry, destruction stage or body-part row.

Script forwarding selects plugin attachments, properties and fragment bindings; it does not patch compiled PEX files. Ordinary list adapters retain script-entry merging and destination metadata. QUST and SCEN keep their VMAD metadata, scripts and bindings together; PERK selects its complete VMAD adapter. Quest, scene and perk content still follows its existing structural ownership and validation rules, independently of VMAD selection.

### Repeated original values and unrelated changes

Suppose mod 1 has the original group A, mod 2 changes it to B, and mods 3 and 4
both carry A. B survives unless a later override can
revert its current owner through a declared or configured virtual master. Two
copies of A provide no additional reversion permission.

Independent changes outside the group do not grant or remove its forwarding
permission. Mod 2's separate edit X and mod 3's separate edit Y can both survive
under their own selection rules, alongside B. This example assumes distinct independent properties,
ordinary processing, and no record-wide structural ownership reset. Priority Mods
and record-specific policies retain their documented precedence.

Forwarding B preserves a useful earlier edit when later plugins carried stale
values. Plugin records do not identify intent: a plugin with otherwise original record data can still ship
replacement files at the original paths. Unrelated record edits do not resolve
that file-content ambiguity.

| Resource | Grouping |
|---|---|
| Main object mesh | Complete model, ordered alternate textures, metadata and associated conditional bounds. |
| STAT geometry | Main model/bounds and all four LOD models together. |
| Armor | ARMO dropped models/bounds and Armature references use Meshes; male/female icons use IconsAndImages. ARMA selects each gender’s first/third-person models, alternate textures and shared weight-slider flag together under Meshes; skin/swap references use Textures. Body-template fields and supported races retain ordinary forwarding. |
| Appearance | NPC face group and RACE head shape forward as complete groups. RACE head meshes and Skin use Meshes; face textures, tint definitions/count and body EGT resources use Textures. Presets, hair-color choices and race flags retain ordinary forwarding. |
| Special meshes | Destruction stage models and limb models are separate from health, damage and row structure. Limb scale/model hashes, complete debris choices, muzzle-flash interpretation, animated-model events, art type, scope configuration and loading-screen framing have explicit groups. Camera and impact tuning remain independent. Furniture's XMRK resource remains separate. |
| Material model | MATO's NIF model uses Meshes. Its material parameters, flags and opaque DNAM data remain ordinary. |
| Texture sets | All eight TXST texture maps and its flags; Decal retains its separate rule. |
| Effect/particle textures | EFSH membrane textures with UV scale, and particle textures with atlas indices. Other shader DATA retains its ordinary group. SPGD keeps its texture with the subtexture grid; particle motion and density remain independent. |
| Water | Each noise texture and its UV scale; flow-normal texture with flowmap scale and surface flags. Motion, amplitude, falloff and gameplay remain independent. Zero flag clears retain destination field presence. |
| Weather clouds | Complete indexed cloud texture and layer configuration together. |
| Sky/world images | Paired sun/glare and HD LOD maps; world map image/framing; local water texture paths. Water record selection remains ordinary. These are separate groups. |
| Lens flares | Ordered sprite textures and per-sprite settings. Global colour influence and fade-distance scale remain independent. |
| Animation | Each RACE gender’s skeleton/behavior pair; a separate BodyPartData reference; IDLE filename/event pair; IDLM ordered idles/flags/timer; complete PACK idle selection; NPC OppositeGenderAnims. RACE combat/movement tuning and IDLE legacy DATA retain ordinary forwarding. |
| Sound | Complete clip sequence with descriptor type and looping. Pitch, volume, routing, category, alternate descriptor, subtitle String, priority and conditions remain independent. REGN sound references and each row's selection flags/chance use Sounds; music selection uses Music. |
| Music | Track type, track/finale, timing, loops, cues and palette tracks. Conditions remain independent. |
| Scripts | Existing complete entry/adapter rules and VMAD-only bindings. Record structure retains its existing ownership boundaries and validation. |
| UI images | Complete icon pairs, including only the inherited Icons inside region entries. Region grass placement, weather, map text and other content remain ordinary. BookText forwards normally as complete translated text, including image markup; embedded paths are not rewritten or independently gated. |

## Model bounds and trees

Bounds are associated with the main model only where that association exists.
When the final selected model filename matches the winner, keep winning bounds.
When it differs, carry the bounds from the accepted filename source. Bounds-only
or metadata-only edits do not independently take bounds. Optional bounds retain
the chosen source's presence. ARMO has one bounds value for its selected filename
pair. No bounds are invented for skeletons, morphs, individual destruction stages,
LOD levels, scope models or worn ARMA meshes.

**Forward tree changes as a group** is unchecked by default. Off preserves
the winning TREE model/bounds, wind data, script attachments and supported header
flags. On selects those values as one group. Other property groups forward normally. Model/bounds keep the filename rule above, and script copying
retains destination adapter metadata. Name, ingredient, harvest sound and seasonal
production retain their own rules. See [TREE](world.md#tree-tree).

## Selection precedence and limits

Priority Mods still select complete records before ordinary merging. Existing
special Tamriel CELL selection also keeps its earlier policy route. Ignored
plugins remain ineligible. Within ordinary merging, the disabled tree option uses the
final effective winner, including after a structural ownership reset. Shared protection and placed-reference coordination retain their roles.

Forwarding can bypass a later replacement at the original path. Preserving the
TREE winner can sacrifice earlier improvements if a later plugin carried old data. A unique filename, declared master, existing file or
matching path does not resolve that ambiguity. Inspect the patch and use an
authored compatibility patch or explicit source choice when necessary.

Some dependencies have no directly forwarded filename: armor weight-paired meshes,
NPC FaceGen, dialogue voice files, CELL flowmaps, generated LOD, quest SEQ files,
localization files and resources referenced inside meshes or compiled scripts.
Grouped forwarding does not generate or copy those files, inspect their contents,
resolve cross-record compatibility, or validate NPC template inheritance.
RACE body-texture resources are EGT files, HDPT morphs are TRI files, and behavior
resources are HKX files; they are not interchangeable texture/model categories.

Arbitrary script strings, resource-valued settings and other markup cannot all
be identified as filenames. Existing handling remains for unresolved/legacy
ragdoll, faction insignia and pose-matching strings. Unused CanopyShadow and
UnusedNoisemaps remain excluded. These limits are not a claim that those resources
are safe or that every dependency has been detected.
