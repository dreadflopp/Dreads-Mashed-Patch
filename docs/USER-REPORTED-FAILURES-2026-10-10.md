# User-reported patch failures: investigation and repair, 2026-10-10

[Internal documentation index](README.md)

Status: production repairs are implemented for NPC Perks and all eight GLOB/GMST
variants. The regression suite exercises the production handlers and full-run
pipeline; the exploratory test-only implementations have been removed. The
findings below describe the defects as they existed before this repair.

## Findings and confidence

| Report | Finding | Confidence |
|---|---|---|
| NPC `Property Perks did not retain the selected value` | The setter cannot populate an absent destination list. The ordinary original → perk-adding override → later override without perks chain reproduces the exact logged failure. | Confirmed defect and reproduction, including binary overlays. Actual reported mod files were not available. |
| GLOB `InvalidCastException` | Typed history resolution can wrap an older global of a different subtype in the winner's subtype context. Reading that context's `Record` throws. | Confirmed mechanism in mutable and binary fixtures; consistent with the logs. The exact users' subtype transitions remain unverified without their plugins or fuller stack traces. |
| GMST `InvalidCastException` in the full log | The four GMST handlers use the same typed history pattern. A change of setting subtype reproduces the failure too. | Confirmed mechanism in fixtures. It remains a hypothesis for the four reported string settings. |
| Failure after record processing | The previous final `PatchRunFailedException` rejected accumulated record errors. | Confirmed from the supplied log and the previous output/report implementation. The default publication policy below replaces this behavior. |

The NPC evidence identifies a patcher defect, rather than establishing that
Enhanced NPC Perks or Odin is intrinsically incompatible. Mods that add perks
followed by overrides lacking a perk list expose this path. The exact records in
those mod versions have not been inspected.

## Full linked log

The [supplied log](https://drive.google.com/file/d/1IExLeonO58uV4cXp0XrF5CG50y8hsqvZ/view?usp=sharing)
was successfully downloaded and read. It records a run dated 2026-10-09 using
Mutagen 0.54.4 and Synthesis 0.36.6, matching the repository's dependencies.
It contains 63 skipped records and 118 error entries:

| Failure | Skipped records | Error entries |
|---|---:|---:|
| NPC Perks write validation | 55 | 110 |
| GLOB cast | 4 | 4 |
| GMST cast | 4 | 4 |
| Total | 63 | 118 |

Each NPC failure logs the property error and then the record skip. Consequently
the final error count is not a count of independently failed records.

The cast failures are:

| FormKey | EditorID | Logged type |
|---|---|---|
| `0F728B:Skyrim.esm` | HPWhiterun | Global Int |
| `0F728D:Skyrim.esm` | HPRiften | Global Int |
| `D0D0A4:Update.esm` | rr_Code_Origin_GV | Global Short |
| `D0D0A6:Update.esm` | rr_Code_Class_GV | Global Float |
| `0D4F40:Skyrim.esm` | sCantEquipGeneric | Game Setting String |
| `0D4F47:Skyrim.esm` | sMapMarkerAdded | Game Setting String |
| `0D4BE0:Skyrim.esm` | sDiscoveredText | Game Setting String |
| `10E5F3:Skyrim.esm` | sLackRequiredPerkToImproveMagical | Game Setting String |

The NPC failures have winners from 13 different plugins; 30 are witch templates
whose winning context is `PAN_Nameless - Witches.esp`. The winning plugin is not
necessarily the plugin that added perks. Similarly, the origin plugin in a
FormKey does not identify the override causing a GLOB type mismatch. The
`Update.esm` suffix and D0/D1/etc. ID prefixes alone do not diagnose corruption.

The downloaded log is intentionally not copied into the repository: it contains
the user's installation paths and roughly 8 MB of run output.

## NPC perk setter

Before repair, `PerksHandler.SetValue` called `npc.Perks?.Clear()` and only added selected rows if
`npc.Perks != null`. Mutagen declares `INpc.Perks` as a writable nullable
`ExtendedList<PerkPlacement>`; a new or copied NPC without perks has null here.
The selected non-null list therefore never reaches the candidate output record.
`AbstractRecordHandler.ApplyForwardedProperties` correctly detects the mismatch,
and the transaction discards that record.

Two additional presence defects follow from the same code:

- A selected present-empty list is dropped when the destination is absent.
- A selected null clears a present list without removing it. Shared post-write
  validation does not compare null selections, so this can pass silently.

The repaired production setter follows `KeywordListHandler`'s existing replacement
pattern: assign null for selected absence, otherwise assign a fresh list of
generated `PerkPlacement.DeepCopy()` rows. This preserves selected presence,
rank, and detached links. The existing keyed merge/rank ownership rules remain
in `PerksHandler`; the repair changes its setter only.

## GLOB and GMST typed histories

Before repair, all four GLOB and all four GMST record handlers called typed `ResolveAllContexts`.
The local decompiled reference shows that Mutagen's group selection uses the
shared Globals/GameSettings group for these subtype queries. Its generic cache
method wraps resolved contexts using `ModContextCaster`; that wrapper casts its
underlying record when `Record` is read. A subtype query is therefore not a
guarantee that every entry in an override history implements that subtype.

For example, a float original followed by short overrides returns three typed
short contexts. Reading the original context throws while casting
`GlobalFloatBinaryOverlay` to `IGlobalShortGetter`. All unchanged-subtype
fixtures pass. GMST fixtures demonstrate the analogous cast; their binary
variants use appropriate setting EditorID prefixes so reloading retains each
authored type.

The eight handlers now inherit `AbstractSubtypeRecordHandler<TGetter>`, which
reuses the unnarrowed history lookup in `RecordPolicySources` through the shared
GLOB/GMST group getter. It retains only the contiguous eligible history of the
winner's subtype. The latest subtype transition establishes a new merge
baseline for every registered property, including headers; older scalar values
are not converted. A short → float → short history cannot reconnect to the old
short version. Ordinary short-history exits apply after this boundary.

Ignored plugins are removed before establishing the boundary. `Program` resolves
an ignored winner before narrowing the group for dispatch, and enabled-type
checks use the effective winner. Priority identifiers and complete source copies
use the shared group getter so a priority record can select another subtype.
Official GMST name preservation is allowed only when its type prefix matches
the selected record; priority snapshots retain their own complete names and data.
The unchanged-subtype path continues using the existing scalar, translated-text,
flag, ownership and transaction handlers.

## Implementation scope and decision notes

Property registrations are unchanged. NPC Perks writing was corrected; history
resolution was generalized for all eight GLOB/GMST variants. Their duplicated
typed GetRecordContexts overrides and obsolete usings were removed. The shared
base owns the sole active history implementation; test-only candidates were
removed. No flag handler was changed. Other properties retain their handlers
because their existing value/copy/ownership rules are unaffected.

| Record type | Implemented change | Kept specialized and why |
|---|---|---|
| NPC | Nullable Perks list replacement using the existing detached-copy pattern. | Perk identity, rank ownership, and sorted-keyed semantics; writing alone required repair. All other properties retain their handlers. |
| Global Short | Unnarrowed history lookup with a subtype boundary. | Nullable short Data and approved header handler; the scalar surface is subtype-specific. |
| Global Float | Same shared history policy. | Nullable float Data and approved header handler. |
| Global Int | Same shared history policy. | Nullable int Data and approved header handler. |
| Global Unknown | Same shared history policy. | TypeChar, nullable float Data, and approved header handler. |
| Game Setting String | Same shared history policy. | Generated translated-string copying and approved header handler. |
| Game Setting Int | Same shared history policy. | Nullable int Data and approved header handler. |
| Game Setting Float | Same shared history policy. | Nullable float Data and approved header handler. |
| Game Setting Bool | Same shared history policy. | Nullable bool Data and approved header handler. |

To confirm the user's cast failures specifically, inspect all override columns
for the eight listed records: GLOB FNAM type, GMST EditorID/type, and the first
plugin changing that type. A stack trace including the underlying cast types
would also distinguish this mechanism from another InvalidCastException.

## Default partial publication

The requested default now publishes successfully processed records even when
other records fail. No setting or toggle is added. The existing detached record
transaction discards failed candidates, including their staged ancestry. Only
the record-processing catch recovers that record scope's errors. Errors outside
that boundary, including errors for the same FormKey in another scope, remain
fatal. Read failures with no selected changes and short-history EDID checks also
reach the record skip boundary rather than failing the whole run.

`PatchRunReport` keeps errors and separate skipped records (FormKey, type, reason).
`Succeeded` indicates no fatal errors; `IsPartial` indicates skipped records.
The final run log identifies partial output and lists skipped records. The
desktop status and completion dialog identify a partial patch and report its
skipped-record count. Record error lines remain errors, tagged `[Error] [Record]`
for buffered desktop logging; the old gate rejecting all error lines is removed.

Both the Synthesis entry point and desktop publication path accept partial
reports. Fatal load/import, record-type processing, serialization, missing-primary
output, and file publication failures still block publication. Previous primary
and split output files remain intact when the desktop pipeline fails. Skipping
a record omits its proposed merged changes and does not repair malformed inputs
or prove gameplay compatibility with other merged records.

The public root README has been restored. Investigation history, diagnostics
implementation details, and validation evidence are kept in internal developer
documentation.

## Validation

`DreadsMashedPatch.Tests/UserReportedFailureRegressionTests.cs` contains **78
regression cases** against the production implementation: unchanged subtype
controls, subtype-boundary selection, full-run patch/reload for all eight variants,
no reconnection across type changes, ignored winners and intermediate overrides,
effective subtype enablement, complete priority snapshots, compatible/incompatible
official GMST names on both short-history and ordinary merge paths, NPC additions
into absent lists, null/empty selections, detached rows/links, and rank ownership.
Binary input fixtures retain deliberately declared master relationships using
Mutagen's NoCheck writer option; the normal writer trims unused synthetic masters.

`PartialPublicationTests` adds 10 cases for scope-specific recovery, retained
fatal errors, desktop publication rejection, and fragmented/asynchronous logs.
The transaction regressions now verify default partial primary/split publication,
record discard after read/write/validation failures, and a full production run
through both hosts' publication paths with binary reload. Two added cases cover
logged reads with no forward decisions in ordinary and short-history paths.

- Complete test suite: **1,735 passed, 0 failed, 0 skipped**.
- Solution build: **succeeded, 0 warnings, 0 errors**.
- Coverage audit: **131 handlers, 0 missing candidates, 0 partial candidates**;
  the eight generalized handlers retain their previous classifications.
- Collection audit: **208 runtime / 208 expected, 0 unresolved, 0 failures**;
  the verified manifest and expectations are unchanged.
- Workspace search: no retired candidate classes or typed GLOB/GMST history
  calls remain. Approved property/flag registrations are unchanged.

The coverage parser now recognizes the generic subtype base instead of relying
solely on record-specific casts or ToLink calls. Generated coverage reports were
regenerated; JSON changes are registration source-line locations, and Markdown
changes are its generation timestamp. PowerShell is unavailable in this Linux
workspace, so the audit scripts' underlying .NET tools were invoked directly with
the same arguments and failure switches.

Commands: `dotnet test DreadsMashedPatch.Tests/DreadsMashedPatch.Tests.csproj --no-restore`
and `dotnet build DreadsMashedPatch.sln --no-restore --verbosity minimal`.
The initial investigation, before production edits, passed 45 characterization
and candidate cases / 1,690 total tests. Those historical results were superseded
by the successful-behavior regressions above; expected defect assertions and
candidate handler subclasses were removed.

The initial bug fixes passed 1,723 tests before the partial-publication policy was
added. The final solution build had no warnings or errors; concurrently running
the final test build triggered one transient MSB3026 copy retry before succeeding.

These results cover synthetic fixtures, not a rerun of the users' full load orders
or certification of gameplay behavior. The source mod versions in the bug reports
were not available. No desktop UI or in-game validation was performed.
