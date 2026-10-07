# Unsupported records and indirect handling

[Project front page](../../README.md) · [Record index](INDEX.md) · [Shared patching rules](README.md)

A reference to another record does not patch that record’s contents. Likewise, a handler present in the source does not mean the normal run calls it.

| Record | Current handling |
|---|---|
| DOBJ — Default objects | Explicitly excluded. The source cites runtime merging of default-object mappings. The existing handler is dormant. |
| LAND — Landscape | Explicitly excluded, even beneath a processed CELL. Height data, normals, colors and texture layers require coordinated terrain handling. Its handler is dormant. |
| IMAD — Image-space adapter | Explicitly excluded because the current writer does not preserve absent DNAM data. Its handler is dormant. |
| HAIR — Hair | No independent processing route. RACE merges hair references only; HAIR does not provide a modern hair-appearance implementation. |
| NAVI — Navigation mesh information map | No independent processing or reconstruction. NAVM and REFR navigation-door links do not implement a NAVI merge or establish navigation-map consistency. |
| TES4 / GRUP | Plugin header and structural groups are managed by output/copy machinery, not independent record conflict handlers. |
| NOTE | No separate NOTE major-record type in the inspected Skyrim model. Notes represented as BOOK follow the book rules. |

The DOBJ exclusion reflects the code's stated rationale; engine behavior has not been independently verified. Referencing a HAIR or NAVI record does not add an independent patching route.

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
