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
| ARMA model data | All four filename handlers carry detached Data bytes but compare only normalized paths. Byte-only changes have no independent forwarding path. Alternate textures merge separately. This retained policy must not become a blanket claim that other models ignore data bytes. See [GenderedModelFieldHandlers](../../DreadsMashedPatch/PropertyHandlers/General/GenderedModelFieldHandlers.cs) and [ArmorAddonRecordHandlerTests](../../DreadsMashedPatch.Tests/ArmorAddonRecordHandlerTests.cs). |
| Model/bounds coupling | A later override can supply bounds for a rejected model filename. All 29 `ModelBoundsHandler` registrations share this path; TREE's default forwarding restriction bypasses it. See the reproduction below before treating the grouped registration as a geometry-consistency guarantee. |
| Ordinary script-list VMAD | Version/ObjectFormat remain destination-owned. Metadata-only edits do not trigger output; INFO/PACK/PERK whole adapters and QUST/SCEN child fields differ. |
| LTEX | HNAM/SNAM scalar interfaces do not expose absent-subrecord state; writing can normalize missing representations. Nullable snow INAM requires SSE FormVersion 43 or later. LAND remains excluded. |
| Null/default writes | Setters normalize presence per property. Shared post-write validation checks non-null values only. Keywords and NPC Perks preserve absent versus present-empty, but there is no universal null-removal guarantee. |
| Omitted/opaque data | Unregistered fields have no independent decision. Copying/writing may change representation. The meaning and safe combination of every exposed byte payload are not established. See [inventory exclusions](INVENTORY.md#exclusions-and-fields-that-are-not-independent-decisions). |
| Policy performance | Priority/baseline identifier indexes are lazy and avoid full histories during policy checks, but their setup/memory cost has no broad real-modlist benchmark. |

## Model/bounds coupling defect

`ModelBoundsHandler.UpdatePropertyContext` takes bounds from an accepted filename
change, but its subsequent bounds-only branch does not require the source model's
normalized filename identity to match the selected model. A rejected filename
reversion can therefore still supply a new, non-baseline bounds value. For ARMO,
the geometry identity is the pair of male/female world-model filenames; icons are
part of model selection but do not define that identity.

Minimal reproduction, with both plugins depending only on Skyrim.esm:

| Override order | Model filename | Bounds extent |
|---|---|---|
| Skyrim.esm | Original.nif | 1 |
| ModelMod.esp | Replacement.nif | 7 |
| BoundsMod.esp | Original.nif | 9 |
| Computed patch | **Replacement.nif** | **9** |

ModelMod's filename survives the unauthorized baseline reversion, while the new
bounds value is accepted independently. No source record contains the resulting
pair. Also, `boundsExplicitlyChanged` compares adjacent load-order overrides,
not the plugin's actual master history: an inherited bounds edit can qualify
when an unrelated preceding plugin has different bounds.

On 2026-10-24, an isolated local investigation harness reproduced the three-source
case through `AbstractRecordHandler.Process` and plugin binary write/reload for
all 29 registered record types, with TREE's restriction disabled for its probe.
Additional ARMO/STAT probes reproduced inherited bounds overwriting model-owned
bounds and checked legitimate bounds corrections for the selected filename.
A default-restricted TREE control and the six existing `ModelBoundsHandlerTests`
also passed (40 checks total). These are synthetic findings; the user's actual
armor record and plugin chain were not available. Forwarding code was not changed.

The existing tests cover filename changes taking bounds, same-filename bounds
corrections and metadata-only changes, but omit the rejected-filename/different-
bounds combination. A prospective fix should gate independent bounds selection
on matching the selected filename identity, while retaining same-filename bounds
corrections and the existing approved model/reversion policies. Same-path asset
replacements still cannot be verified from plugin records alone.

## Appearance and referenced assets

Record-only selection cannot identify the deployed winner of same-path mesh
replacers or verify FaceGen, custom skins, or compatibility between the winning
NPC, RACE, HDPT, ARMO and ARMA records. Turning off TREE's forwarding restriction
can mix asset-dependent model, wind, scripts and flags without validating them.
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
