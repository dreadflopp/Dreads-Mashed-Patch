# Documentation review evidence

[Patching guide](README.md) · [Authoritative inventory](INVENTORY.md)

Reviewed against source baseline `823a8f3` on 2026-10-04, using Mutagen Skyrim 0.54.4. This is a source-review appendix; the user-facing behavior is in the guide and property tables. No production source or tests were changed.

## Complete processing path

Every supported record section links its actual registration and source-context lookup. Those lookups resolve the record’s complete override history using the link cache; the common path below then selects and writes properties. This includes handlers reached through different interface names, not just classes named after signatures.

| Stage | Inspected implementation | Result used in the guides |
|---|---|---|
| Input preparation | [LoadOrderPreparer](../../DreadsMashedPatch.App/Services/LoadOrderPreparer.cs), [PatcherRunner](../../DreadsMashedPatch.App/Services/PatcherRunner.cs) | Installed CCC plugins and plugin list feed Synthesis; output-plugin cutoff determines inputs. Settings are applied before processing. |
| Query and dispatch | [Program](../../DreadsMashedPatch/Program.cs), [configuration](../../DreadsMashedPatch/PatcherConfiguration.cs) | Supported/enabled type queries, narrowing, malformed-context handling, early filtering, concrete handler dispatch. GLOB/GMST narrow by variant; placed-trap discovery narrows to PHZD. |
| Per-record source lookup | Each linked [record handler](../../DreadsMashedPatch/RecordHandlers) | ResolveAllContexts for the appropriate getter type, then shared ignore filtering and winner selection. All 118 supported handler routes were reconciled with the inventory and live registrations. |
| Policy and selection | [AbstractRecordHandler](../../DreadsMashedPatch/RecordHandlers/Abstracts/AbstractRecordHandler.cs), [AbstractPropertyHandler](../../DreadsMashedPatch/PropertyHandlers/Abstracts/AbstractPropertyHandler.cs) | Priority copying, oldest-to-newest processing, ownership resets, permission-controlled reversions, comparison with effective winner, EditorID policy. |
| Collection selection | [AbstractListPropertyHandler](../../DreadsMashedPatch/PropertyHandlers/Abstracts/AbstractListPropertyHandler.cs) and registered specializations | Keyed/aligned/positional decisions, deletions/restorations, row replacements after specialized updates. The final replacement step matters for CONT counts. Whole-list handlers use a whole-value context instead. |
| Final mutation | [AbstractRecordHandler](../../DreadsMashedPatch/RecordHandlers/Abstracts/AbstractRecordHandler.cs), registered property setters, [NPC override](../../DreadsMashedPatch/RecordHandlers/NpcRecordHandler.cs) | Create/get winner override, coordinate header writes, apply changed properties. NPC uses the patch NPC collection. Exceptions can leave partial mutations. |
| Output files | [PatcherRunner](../../DreadsMashedPatch.App/Services/PatcherRunner.cs), [PatchOutputTransaction](../../DreadsMashedPatch/PatchOutputTransaction.cs) | Synthesis serialization/splitting and staged file replacement; no rollback of individual records for caught errors. |

## Record-specific paths checked after shared selection

| Records | Additional path to final mutation |
|---|---|
| CELL | Policy can bypass the initial filter; selected Tamriel header uses a child-excluding copy mask. Normal cells use registered properties. |
| ACHR / REFR | UDR coordinator can replace selected placement/enable parent using the selected disable-bit owner before property application. PHZD has no coordinator. |
| PERK / QUST / SCEN / SMBN / SMEN / SMQN / WRLD | Adjacent semantic trigger changes reset all registered contexts. Final output still starts from the effective winner, so unregistered fields do not follow the trigger source. |
| IPCT | Decal presence suppresses child writes when absent; bounds and parallax setters copy their grouped fields. |
| QUST | Adapter presence gates child writes; proposed structural changes are validated before application. This does not replace shared error/override behavior. |
| SCEN | Adapter presence gates writes; structural copies omit/clear nested unused data. |
| ARMA / ARMO and model-bearing records | Followed filename/alternate-texture and model/bounds setters, including carried-but-not-compared model data. |
| NPC_ / RACE / CONT / leveled lists | Followed protection, ranks, duplicate inventory, extra-data, skill-slot and entry copying through the final collection setters. |
| PROJ / TXST / EFSH / REVB / SNDR / WRLD | Followed grouped snapshot equality and setters to the physical member groups described in each guide. |
| INFO / PACK / script-bearing records | Distinguished whole adapters from script-list reconstruction and explicit QUST/SCEN child fields. Confirmed inactive PACK graph registration. |
| IPDS / REGN / NAVM / SOPM / WTHR | Followed fallback or whole-aggregate/list copying, including nested collections and independently selected blobs. |
| Remaining records | Followed their registered scalar, link, flag, binary, generated-copy, asset and collection handlers through the shared output path. No extra record-specific override policy was inferred from names or comments. |

## Verification

- Refreshed runtime reflection: 133 concrete record variants, 122 constructible handlers. Reconciled 118 supported variants, four explicitly excluded variants and eleven without independent routes against the inventory.
- Re-ran the coverage tool in Release: 122 handlers, zero reported missing/partial candidates. Its inherited-property limitations remain documented; this result does not establish complete PHZD coverage.
- Compared the public property tables with the inventory and current handler registrations. Checked relative links and index coverage.
- Ran the existing Release test suite: **430 passed, zero failed**.
- Used temporary synthetic probes outside the repository to exercise full `Program.RunPatch` for header clearing, CONT extra-data loss, script-adapter metadata, short-history priority and baseline-EditorID behavior. A direct Name setter probe checked translation loss. Results are described in [known issues](KNOWN-ISSUES.md).

The synthetic probes inspect the in-memory output patch; they do not establish binary serialization or gameplay outcomes. No real load-order patch was generated for this review. Decompiled Mutagen sources were consulted as reference only. Exact preservation of every unused field, opaque payload semantics and implicit writer handling remain uncertain where not established by the inspected code.
