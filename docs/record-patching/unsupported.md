# Unsupported records and indirect handling

[Patching guide](README.md) · [Record index](INDEX.md)

A reference to another record does not patch that record’s contents. Likewise, a handler present in the source does not mean the normal run calls it.

| Record | Current handling |
|---|---|
| DOBJ — Default objects | Explicitly excluded. The source cites runtime merging of default-object mappings. The existing handler is dormant. |
| LAND — Landscape | Explicitly excluded under the project’s runtime-field policy, even beneath a processed CELL. Its handler is dormant. |
| LTEX — Landscape texture | Explicitly excluded under the same policy. Its handler is dormant. |
| IMAD — Image-space adapter | Explicitly excluded; the source cites Mutagen handling of absent DNAM. Its handler is dormant. |
| HAIR — Hair | No independent processing route. RACE merges hair references only. |
| LENS — Lens flare | No independent processing route. LIGH Lens and WTHR SunGlareLensFlare select references, not flare sprites/parameters. |
| VOLI — Volumetric lighting | No independent processing route. WTHR selects a complete time-of-day reference structure, not the referenced lighting records. |
| NAVI — Navigation mesh information map | No independent route or patcher-owned reconstruction found. NAVM and REFR navigation-door links do not implement a NAVI merge. The local writer serializes populated NAVI groups; that is not a rebuild. No real navigation consistency test was run; broader writer-hook behavior remains unestablished. See the [NAVI audit](COVERAGE-COMPARISON-AUDIT.md#pack-arma-and-navi-boundaries). |
| PARW, PBAR, PBEA, PCON, PFLA, PGRE, PMIS — Placed arrows, barriers, beams, cones, flames, traps and missiles | The placed-trap query can encounter these variants, but narrowing retains only PHZD. No independent property forwarding for these seven types. |
| TES4 / GRUP | Plugin header and structural groups are managed by output/copy machinery, not independent record conflict handlers. |
| NOTE | No separate NOTE major-record type in the inspected Skyrim model. Notes represented as BOOK follow the book rules. |

The exclusion reasons above report what the code says; they are not independent verification of engine or writer behavior. The original omission rationale for HAIR, LENS, NAVI, VOLI and the seven placed variants is **uncertain**. Current lack of support does not mean that adding support would be inappropriate.

## Should these remain unsupported?

Assessment on 2026-10-05 using the dispatch code, Mutagen 0.54.4 decompiled getter/writer surfaces, and the pinned xEdit definitions. These are recommendations for future work; the processing routes above remain unchanged.

| Records | Recommendation and evidence |
|---|---|
| VOLI | Candidate for support. Mutagen exposes twelve nullable lighting parameters, and xEdit defines them as authored fields. Review whether related color/density parameters should select together, then reuse scalar or aggregate handlers and verify binary output. No serialization blocker was found in this interface review. |
| LENS | Candidate for support. Two nullable parameters and a sprite collection are exposed. xEdit defines an ordered, count-coupled sprite array; selecting the complete array is a reasonable starting policy, subject to copy/count/presence verification. Sprite fields and flags would travel inside their complete values. |
| PARW, PBAR, PBEA, PCON, PFLA, PGRE, PMIS | Candidates for support. Each exposes a Projectile link plus the same inherited placed fields already handled for PHZD. xEdit uses its shared ReferenceRecord definition for these signatures. Reuse the existing PHZD field patterns after verifying each concrete setter, query/narrowing route, nested output and header flags. This does not imply extending ACHR/REFR UDR policy. |
| LTEX | Reconsider the blanket exclusion. The dormant handler covers texture/material links, friction/restitution, specular exponent, grasses and snow flags. The existing reason names a runtime policy but does not establish why these authored properties must be excluded. Validate field coupling and serialization before enabling its route. |
| LAND | Keep excluded from generic property merging for now. Height data, normals, colors and texture layers require coordinated terrain handling. The dormant handler still has unordered Layers/Textures exceptions; its existence does not establish a safe terrain merge. Whole-record selection could be investigated separately. |
| NAVI | Keep unsupported until a navigation-consistency policy exists. Its map entries include navmesh links, edge/door connections and optional island data. Independent list merging or copying does not establish consistency with the selected NAVM data. |
| IMAD | Keep excluded until the writer issue is resolved or safely bounded. The current decompiled writer emits DNAM unconditionally, so absence is not preserved. That is a serialization limitation, not proof that image-space adapters can never be supported. |
| DOBJ | Retain the current exclusion pending a focused engine-behavior review. The source asserts runtime merging; the available handler and tests do not independently verify that assertion or whether additional patching is useful. |
| HAIR | Low priority. The current Mutagen getter adds no gameplay fields beyond shared metadata, and pinned xEdit lists HAIR among unused records with only EDID. It should not be treated as a missing modern hair-appearance implementation. |
| TES4 / GRUP / NOTE | Keep their existing structural/indirect treatment. These do not require additional independent record conflict handlers in the current model. |

The authored surfaces and array definitions above can be checked in the pinned [xEdit Skyrim definitions](https://github.com/TES5Edit/TES5Edit/blob/93cc0bc5a1251936c3c7859eee3150eda12a62d7/Core/wbDefinitionsTES5.pas). Exact Mutagen surfaces are available through the [local reference exporter](../REFERENCE_SOURCES.md). Interface availability establishes implementation feasibility, not safe gameplay behavior. Adding support requires the full registration/query/dispatch path, reviewed property rules and output verification.

## Supported records with indirect routes

Some supported records also use indirect routes:

| Record/group | Route and boundary |
|---|---|
| GLOB | Global query narrowed to Float, Int, Short or Unknown. Each has its own Data handling. |
| GMST | Game-setting query narrowed to Bool, Float, Int or String. |
| PHZD | Placed-trap query narrowed to hazards. Hazard, all eighteen inherited placed fields and shared metadata/header handling are registered. The sibling variants are still narrowed away. |
| FLST | Routed to FormIdRecordHandler. |
| INFO | Routed to singular-named DialogResponseRecordHandler, separately from DIAL child lists. |
| ACHR / REFR | Placed NPC/object handlers, independently queried even beneath cells. |
| CELL / WRLD children | Supported child major records are queried separately; child containers are not atomic property lists. Enclosing groups can be created by override copying. |

DIAL, DLBR, INFO, DLVW, NAVM and PACK are **disabled by default**, not unsupported. Their behavior when enabled is in the [index](INDEX.md).

Evidence: [supported/excluded catalog, queries, narrowing and dispatch](../../DreadsMashedPatch/Program.cs), [default configuration](../../DreadsMashedPatch/PatcherConfiguration.cs), [authoritative inventory](INVENTORY.md).
