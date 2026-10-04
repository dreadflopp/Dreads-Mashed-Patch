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
| NAVI — Navigation mesh information map | No independent route or patcher-owned reconstruction found. NAVM and REFR navigation-door links do not implement a NAVI merge. Implicit library-writer behavior is not established. |
| PARW, PBAR, PBEA, PCON, PFLA, PGRE, PMIS — Placed arrows, barriers, beams, cones, flames, traps and missiles | The placed-trap query can encounter these variants, but narrowing retains only PHZD. No independent property forwarding for these seven types. |
| TES4 / GRUP | Plugin header and structural groups are managed by output/copy machinery, not independent record conflict handlers. |
| NOTE | No separate NOTE major-record type in the inspected Skyrim model. Notes represented as BOOK follow the book rules. |

The exclusion reasons above report what the code says; they are not independent verification of engine or writer behavior. The reason for the missing routes for HAIR, LENS, NAVI, VOLI and the seven placed variants is **uncertain**.

Some supported records also use indirect routes:

| Record/group | Route and boundary |
|---|---|
| GLOB | Global query narrowed to Float, Int, Short or Unknown. Each has its own Data handling. |
| GMST | Game-setting query narrowed to Bool, Float, Int or String. |
| PHZD | Placed-trap query narrowed to hazards. Only Hazard and shared EditorID/header handling are registered; inherited placed data is omitted. |
| FLST | Routed to FormIdRecordHandler. |
| INFO | Routed to singular-named DialogResponseRecordHandler, separately from DIAL child lists. |
| ACHR / REFR | Placed NPC/object handlers, independently queried even beneath cells. |
| CELL / WRLD children | Supported child major records are queried separately; child containers are not atomic property lists. Enclosing groups can be created by override copying. |

DIAL, DLBR, INFO, DLVW, NAVM and PACK are **disabled by default**, not unsupported. Their behavior when enabled is in the [index](INDEX.md).

Evidence: [supported/excluded catalog, queries, narrowing and dispatch](../../DreadsMashedPatch/Program.cs), [default configuration](../../DreadsMashedPatch/PatcherConfiguration.cs), [authoritative inventory](INVENTORY.md).
