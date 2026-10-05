# Coverage and comparison audit

[Patching guide](README.md) · [Known limits](KNOWN-ISSUES.md#coverage-and-comparison-limits) · [Inventory](INVENTORY.md)

Reviewed on 2026-10-05 against the current working tree based on `6fa7d0e`, including the five completed fixes recorded in [REVIEW.md](REVIEW.md). Dependencies: Mutagen Skyrim 0.54.4; local xEdit source pinned to `93cc0bc5a1251936c3c7859eee3150eda12a62d7`. The initial audit was a documentation-only investigation. Keyword removal, PHZD coverage and inherited-field auditing were subsequently fixed on the same date; completion is recorded below and in [REVIEW.md](REVIEW.md#coverage-fix-verification). The original audit evidence remains here with its historical scope.

## Fixes completed on 2026-10-05

The user selected fixes for the confirmed defects, retaining PACK, NAVI and ARMA policies.

- **Keywords:** the existing shared setter now writes selected null as absence, preserves present-empty lists and detaches every FormLink. Existing list ownership and WEAP exclusivity remain intact. Ninety new cases cover all eighteen registrations, authorized/denied restoration, source isolation and binary reload.
- **PHZD:** all eighteen formerly omitted fields now use existing handlers. Pinned xEdit ReferenceRecord confirms sorted Water and Keyword/Ref keys, ordered XLRT rows and fixed three-float XLOD ownership. XIS2 remains a nullable byte slice, with absent/present-empty tests. Ninety-three new cases cover every field, nested CELL dispatch, addition/removal/reversion permissions, generated overlay copies and binary output. UDR coordination remains scoped to ACHR/REFR.
- **Audit:** inherited record/aspect fields are now inspected; only common identity/version/runtime interfaces are filtered, with header properties retained. Three read-only GLOB discriminators have explicit classifications. A temporary fixture restoring the old PHZD registration is rejected with exactly eighteen missing candidates. Strict coverage passes for the current tree.
- **Scope at the keyword/PHZD fix:** 122 handlers, 1,618 implemented property registrations (1,619 static detections including commented-out IDLE RelatedIdles), 173 collection registrations/expectations with zero unresolved/failures. Generated reports are updated. PACK graph enablement, NAVI reconstruction, ARMA byte comparison and generalized null-write validation retain their prior policies.

The sections below describe the original findings and probes before these fixes. They are not a list of still-open keyword/PHZD/audit defects. See [completion verification](REVIEW.md#coverage-fix-verification) for current tests, build status, exclusions and cleanup.

The later [additional record support](REVIEW.md#additional-record-support-verification) expands the current catalog to 128 variants, with 131 handlers and 208 collection registrations. Counts below retain their original review baselines.

## Original findings and priorities

| Area | Established situation | Recommended next work |
|---|---|---|
| Keyword removal | A selected null is silently ignored, including through normal processing and binary output. Present-empty works. | Fix the shared nullable keyword setter and verify removal/restoration for every affected registration. This is a demonstrated output defect. |
| PHZD | Eighteen inherited placed-reference fields have no property registrations. The normal route is active; header flags and Hazard are covered. | Review and implement compatible inherited-field handlers with explicit exclusions. This is a coverage gap; its original rationale remains unknown. |
| PACK | The coupled template graph remains unregistered. Mutagen writes indexed data in key order; xEdit defines declaration-order value rows. | Resolve the writer/order policy before enabling the graph. Index/value associations survive the existing binary regression; gameplay harm from reordering has not been established. |
| ARMA | Model bytes are copied with a selected filename but ignored by filename equality, for all four model slots. | Retain the documented policy unless independent byte forwarding is explicitly requested. This is a comparison limit, not a newly demonstrated accidental omission. |
| NAVI | No query, handler or patcher-owned reconstruction exists. NAVM and navigation-door-link forwarding are separate features. | Define navigation consistency and validate real plugins before adding this feature. No safe reconstruction rule is established here. |

Passing registration audits alone did not resolve these findings; keyword and PHZD behavior now has the regression coverage described above. Coverage, comparison, setter behavior, serialization and gameplay validation are separate checks.

## Audit scope and inherited fields

At the initial review, the [property audit](../record-handler-coverage.md) passed for **122 handlers, zero missing/partial candidates and zero stale overrides**. Its [implementation](../../tools/RecordHandlerCoverage/Program.cs) then inspected directly declared getter properties and only four inherited names: EditorID, MajorRecordFlagsRaw, SkyrimMajorRecordFlags and MajorFlags. Aggregate coverage is inferred from source patterns. It does not verify active dispatch, recursive equality, successful null writes or binary preservation.

The pre-fix runtime dictionaries contained **1,601 registrations**, matching the static registration inventory. The earlier 1,748 count in the original review predates the migrations.

A supplementary reflection sweep inspected every getter's inherited instance properties, matched them to the existing report and the constructed runtime dictionaries, and classified common identity/runtime/version/header wrappers separately. Thirty record/aspect property occurrences were outside the static report:

| Surface outside the report | Occurrences | Result |
|---|---:|---|
| PHZD inherited placed fields | 18 | Unregistered; listed below. |
| SMBN / SMEN / SMQN Parent, PreviousSibling and Conditions | 9 | Already registered in all three handlers. No forwarding gap. |
| Float / integer / short GLOB TypeChar | 3 | Read-only type discriminators fixed by the concrete record class (`f`, `l`, `s`). No independent setter is needed. GLOB unknown variants have a separate declared surface. |

Other omitted inherited properties were infrastructure: FormKey, FormVersion, Version2, VersionControl, Registration, BinaryWriteTranslator, ILinkIdentifier.Type, and the IsDeleted/IsCompressed header views. These are not additional independent gameplay-field candidates. Within this inherited-property sweep, PHZD was the only unregistered gameplay-field group. This does **not** establish complete coverage of nested members or unsupported record types.

The pre-fix [collection audit](../collection-semantics-comparison.md) reported **168 runtime registrations, 168 expectations, zero unresolved entries/failures**. At that review the tracked manifest and comparison report matched regenerated temporary outputs; the post-fix count is 173. This checks registered modes against reviewed expectations; it does not exercise null setters, compare every aggregate member, or run xEdit against output. Collections hidden inside whole aggregates and the disabled PACK graph are outside its direct collection-value inventory.

## PHZD: exact omission and reuse boundary

Before the fix, [PlacedHazardRecordHandler](../../DreadsMashedPatch/RecordHandlers/PlacedHazardRecordHandler.cs) had exactly three registrations: EditorID, MajorRecordFlagsRaw and Hazard. [Program](../../DreadsMashedPatch/Program.cs) queries IAPlacedTrap contexts and narrows to IPlacedHazard. The shared header includes APlacedTrap.MajorFlag; it does not provide placed-data handling or ACHR/REFR UDR coordination.

The local decompiled IAPlacedTrapGetter and IPlacedGetter establish these eighteen formerly missing fields:

| Kind | Fields |
|---|---|
| Placement and scalar values | Placement, Scale, FactionRank, HeadTrackingWeight, FavorCost |
| Links | EncounterZone, Owner, Emittance, MultiBoundReference, LocationReference |
| Aggregates | VirtualMachineAdapter, ActivateParents, EnableParent |
| Collections | Reflections, LinkedReferences, LocationRefTypes, DistantLodData |
| Opaque payload | IgnoredBySandbox |

Winning-record copies and whole-record priority snapshots can carry these fields; ordinary property selection cannot reconcile them independently. Their absence is established, while the original design reason is not. The pre-fix migration comment acknowledged the gap; the completed migration registers these fields and explains their ownership boundaries. The earlier claim that PHZD has no aggregate/list surface is obsolete.

Reuse the [ACHR](../../DreadsMashedPatch/RecordHandlers/PlacedNpcRecordHandler.cs) and [REFR](../../DreadsMashedPatch/RecordHandlers/PlacedObjectRecordHandler.cs) patterns only after checking getter/setter compatibility. Placement has an existing generic atomic handler; compatible links/scalars and generated-copy aggregates have shared patterns. PHZD IgnoredBySandbox is a nullable byte slice, unlike ACHR/REFR's boolean IsIgnoredBySandbox. LinkedReferences must receive its own pinned-xEdit ordering/key review: ACHR and REFR already use different modes. UDR coordination is a separate policy decision, not implied by adding Placement and EnableParent. The initial audit chose no production migration or new exclusion rationale. The subsequent PHZD migration followed these patterns after checking the pinned xEdit ReferenceRecord definition.

## Keywords: selected absence is not applied

Before the fix, [KeywordListHandler](../../DreadsMashedPatch/PropertyHandlers/General/KeywordListHandler.cs) wrote only non-null values. It now applies selected absence and copies detached links. Its sorted-keyed comparison distinguishes null from an empty list. The shared [list context](../../DreadsMashedPatch/Contexts/ListPropertyContext.cs) can select null once absence is observed, including when the handler's initial CanBeNull default is false. The [application path](../../DreadsMashedPatch/RecordHandlers/Abstracts/AbstractRecordHandler.cs) deliberately validates only non-null writes, so this no-op does not fail the staged candidate or run.

Eighteen runtime registrations use this setter, including WEAP's subclass: ACTI, AMMO, ARMO, BOOK, FLOR, FURN, ALCH, INGR, KEYM, LCTN, MGEF, MISC, NPC_, SCRL, SLGM, SPEL, TACT and WEAP. Other keyword registrations use different handlers and were not assumed to have this defect.

Temporary full-run probes used three versions of the same WEAP record, Skyrim.esm → Earlier.esp → Winner.esp, with both plugins declaring Skyrim as a master and neither declaring the other:

| Keyword history | Selected result | Actual output, including binary reload |
|---|---|---|
| `[K] → null → [K]` | Absence retained from Earlier; Winner cannot restore Earlier-owned removal. | A patch override is published successfully with winner's `[K]` still present. |
| `[K] → [] → [K]` | Earlier's present-empty list. | An override with zero keyword rows. |

Direct setter/application probes also confirm that null retains the old list without an error, while empty removes rows. This is a successful-but-incorrect write, not a staging leak from a failed setter. The completed partial-override fix remains valid within its documented semantic validation boundary.

The completed fix applies selected absence on the nullable Skyrim keyword aspect while preserving the existing key/ownership and WEAP exclusivity policies. Every affected registration is tested for absent versus present-empty output, authorized/denied restoration, source isolation and binary reload. The comparer still distinguishes null and empty.

## PACK, ARMA and NAVI boundaries

**PACK:** PackageTemplate, DataInputVersion, Data, XnamMarker and ProcedureTree form the dormant [PackageTemplateGraphHandler](../../DreadsMashedPatch/PropertyHandlers/Package/PackageTemplateGraphHandler.cs). There are no active registrations for these fields. The local PackageBinaryWriteTranslation orders data values and metadata by signed key. Pinned xEdit defines Data Input Values as wbRArray without an outer sort key. The existing [binary regression](../../DreadsMashedPatch.Tests/PackageTemplateGraphHandlerTests.cs) starts with value order `[5,0,3]` and writes `[0,3,5]`, retaining the per-index values and procedure indexes. This establishes physical reordering, not broken associations or an in-game failure. Keeping the graph disabled prevents independent graph selection; copying a winner or priority snapshot can still pass its graph through the same writer. Disabled registration is therefore not a guarantee of unchanged physical row order in every emitted PACK.

**ARMA:** The [four filename handlers](../../DreadsMashedPatch/PropertyHandlers/General/GenderedModelFieldHandlers.cs) return Path plus detached Data bytes, compare only normalized case-insensitive paths, and write both together. AlternateTextures are separate keyed lists. All four byte-only equality probes pass. No separate Data registration exists. Ordinary whole-model handlers have a different comparison surface; the ARMA exception must not become a blanket statement that model bytes never participate. This audit does not establish that ignoring byte-only changes is safe for every plugin.

**NAVI:** Source searches across the core, desktop runner, record catalog, queries and dispatch found no independent handling or reconstruction. The local SkyrimModBinaryWriteTranslation writes a NavigationMeshInfoMaps group when the mod already contains entries; that serialization branch is not a reconstruction algorithm. NAVM owns complete Data plus three separate binary payloads, and REFR owns NavigationDoorLink. Neither establishes a coherent navigation information map. No real NAVM/NAVI consistency or in-game pathfinding test was run, and this audit makes no claim about every possible library writer hook.

## Verification and completion

These are the historical results of the initial documentation-only audit. Current fix verification is in [REVIEW.md](REVIEW.md#coverage-fix-verification).

Commands run from the repository root, with audit outputs outside the repository:

```bash
dotnet run --project tools/RecordHandlerCoverage --configuration Release --no-restore -- "$PWD" /tmp/dmp-coverage-review/coverage.md /tmp/dmp-coverage-review/coverage.json tools/RecordHandlerCoverage/coverage-overrides.json --fail-on-unresolved
dotnet run --project tools/CollectionSemanticsAudit --configuration Release --no-restore -- "$PWD" /tmp/dmp-coverage-review/collections.json docs/xedit-collection-expectations.json /tmp/dmp-coverage-review/collections.md --fail-on-unresolved
dotnet test DreadsMashedPatch.Tests/DreadsMashedPatch.Tests.csproj --no-restore --verbosity minimal
dotnet build DreadsMashedPatch.sln --no-restore --verbosity minimal
```

- Existing suite: **617 passed, zero failed/skipped**. It includes the PACK binary ordering regression and relevant ARMA/list/exclusion tests; the suite passes despite the demonstrated keyword defect.
- Full solution build: core, tests and Windows desktop app succeeded with **zero warnings/errors**.
- Temporary reflection and behavioral probe: **15 assertions passed**, confirming the current limitations and the empty-list control case. Two synthetic keyword patch outputs were written to memory and reloaded as binary overlays. These are investigation probes, not added regression tests.
- Tracked coverage JSON, collection manifest and collection comparison remain current; no registration change required regeneration. Documentation links and consistency were checked after editing.
- No properties were migrated, no flags were generalized, and no dead production code was removed. PHZD and keyword work remain open; PACK remains disabled; ARMA retains its policy; NAVI remains unsupported. Reasons for other unsupported routes, opaque payload semantics and complete on-disk preservation remain uncertain. No real game load order, xEdit output session or gameplay validation was performed.
