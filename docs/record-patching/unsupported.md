# Unsupported records and indirect handling

[Patching guide](README.md) · [Record index](INDEX.md)

A reference to another record does not patch that record’s contents. Likewise, a handler present in the source does not mean the normal run calls it.

| Record | Current handling |
|---|---|
| DOBJ — Default objects | Explicitly excluded. The source cites runtime merging of default-object mappings. The existing handler is dormant. |
| LAND — Landscape | Explicitly excluded under the project’s runtime-field policy, even beneath a processed CELL. Its handler is dormant. |
| IMAD — Image-space adapter | Explicitly excluded; the source cites Mutagen handling of absent DNAM. Its handler is dormant. |
| HAIR — Hair | No independent processing route. RACE merges hair references only. |
| NAVI — Navigation mesh information map | No independent route or patcher-owned reconstruction found. NAVM and REFR navigation-door links do not implement a NAVI merge. The local writer serializes populated NAVI groups; that is not a rebuild. No real navigation consistency test was run; broader writer-hook behavior remains unestablished. See the [NAVI audit](COVERAGE-COMPARISON-AUDIT.md#pack-arma-and-navi-boundaries). |
| TES4 / GRUP | Plugin header and structural groups are managed by output/copy machinery, not independent record conflict handlers. |
| NOTE | No separate NOTE major-record type in the inspected Skyrim model. Notes represented as BOOK follow the book rules. |

The exclusion reasons above report what the code says; they are not independent verification of engine or writer behavior. HAIR and NAVI remain without independent routes for the reasons reviewed below.

## Should these remain unsupported?

Assessment on 2026-10-05 using the dispatch code, Mutagen 0.54.4 decompiled getter/writer surfaces, and the pinned xEdit definitions. The remaining exclusions are assessed below. VOLI, LENS, LTEX and the seven placed-projectile variants are now supported and enabled by default; see their [record tables](INDEX.md) and [implementation verification](REVIEW.md#additional-record-support-verification).

| Records | Recommendation and evidence |
|---|---|
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
| PHZD | Placed-trap query narrowed to hazards. Hazard, all eighteen inherited placed fields and shared metadata/header handling are registered. All seven placed-projectile siblings have their own concrete narrowing and handlers. |
| PARW / PBAR / PBEA / PCON / PFLA / PGRE / PMIS | Shared placed-trap query narrowed separately to each concrete variant. Projectile, all eighteen inherited fields and shared metadata/header handling are registered. No ACHR/REFR UDR coordination. |
| FLST | Routed to FormIdRecordHandler. |
| INFO | Routed to singular-named DialogResponseRecordHandler, separately from DIAL child lists. |
| ACHR / REFR | Placed NPC/object handlers, independently queried even beneath cells. |
| CELL / WRLD children | Supported child major records are queried separately; child containers are not atomic property lists. Enclosing groups can be created by override copying. |

DIAL, DLBR, INFO, DLVW, NAVM and PACK are **disabled by default**, not unsupported. Their behavior when enabled is in the [index](INDEX.md).

Evidence: [supported/excluded catalog, queries, narrowing and dispatch](../../DreadsMashedPatch/Program.cs), [default configuration](../../DreadsMashedPatch/PatcherConfiguration.cs), [authoritative inventory](INVENTORY.md).
