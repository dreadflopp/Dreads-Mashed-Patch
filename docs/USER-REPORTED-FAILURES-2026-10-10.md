# NPC Perks and GLOB/GMST failure diagnosis

[Internal index](README.md) · [Regression map](record-patching/REVIEW.md#regression-map)

These defects were repaired on 2026-10-10. Keep the signatures, reproduction
mechanisms and uncertainty below for matching future reports; they are not open
migration tasks. The exact reported plugins and in-game outcomes were not verified.

## Report evidence

The [supplied log](https://drive.google.com/file/d/1IExLeonO58uV4cXp0XrF5CG50y8hsqvZ/view?usp=sharing)
records a 2026-10-09 run with Mutagen 0.54.4 and Synthesis 0.36.6: 55 NPC skips,
four GLOB skips and four GMST skips. NPC property errors were logged again as
record skips, producing 118 error entries for 63 skipped records. Do not equate
error-line count with failed-record count. The raw log is not copied into Git
because it contains installation paths and roughly 8 MB of output.

| Cast failure FormKey | EditorID | Logged variant |
|---|---|---|
| 0F728B:Skyrim.esm | HPWhiterun | Global Int |
| 0F728D:Skyrim.esm | HPRiften | Global Int |
| D0D0A4:Update.esm | rr_Code_Origin_GV | Global Short |
| D0D0A6:Update.esm | rr_Code_Class_GV | Global Float |
| 0D4F40:Skyrim.esm | sCantEquipGeneric | Game Setting String |
| 0D4F47:Skyrim.esm | sMapMarkerAdded | Game Setting String |
| 0D4BE0:Skyrim.esm | sDiscoveredText | Game Setting String |
| 10E5F3:Skyrim.esm | sLackRequiredPerkToImproveMagical | Game Setting String |

The winning plugin need not be the perk-adding plugin. A FormKey's origin plugin
or D0/D1/etc. prefix does not identify the override causing a subtype mismatch
or establish corruption. The reproduced mechanisms do not establish that Odin,
Enhanced NPC Perks or a particular NPC overhaul is intrinsically incompatible.

## Perks write validation

`Property Perks did not retain the selected value` occurred when an earlier mod
added perks but the winning NPC had no list. The old setter used null-conditional
Clear and added rows only if the destination list already existed, losing the
selected non-null list. It also lost present-empty selections and turned selected
null into an empty list. Shared non-null post-write validation caught the first
case; it could not catch the selected-null error.

The production setter now follows the shared keyword replacement pattern:
selected null writes absence, otherwise assign a fresh list of generated
PerkPlacement copies. Preserve rank ownership, duplicate semantics and detached
links; reverting to in-place mutation recreates the absent-destination bug.

## Mixed-subtype history casts

Mutagen group lookup can return records of different GLOB/GMST subtypes under
typed ResolveAllContexts wrappers. ModContextCaster casts only when Record is
read; having a typed context array does not prove each record has that subtype.
For example, a float original followed by short overrides can throw when reading
the original as IGlobalShortGetter. Binary fixtures reproduce all eight GLOB/GMST
variants. The analogous mechanism is consistent with the reported string GMST
errors, but their exact subtype transitions remain unconfirmed.

All eight production handlers use AbstractSubtypeRecordHandler and shared group
getters through RecordPolicySources. Keep only the contiguous eligible history of
the winner's subtype; the latest transition is the baseline. A short→float→short
history must not reconnect to the older short value. Remove ignored plugins
before finding that boundary and resolve an ignored winner before dispatch/type
enablement. Priority snapshots can select another subtype; official GMST EditorID
preservation requires its type prefix to match. Do not coerce older scalars into
the winner's type.

Tests: [UserReportedFailureRegressionTests](../DreadsMashedPatch.Tests/UserReportedFailureRegressionTests.cs).
Synthetic binary fixtures use Mutagen's NoCheck master option where declared
permission relationships must survive writing; the normal writer trims unused
synthetic masters. Production handling must not depend on that fixture option.

For record skips and fatal failures use the current
[publication contract](DEVELOPMENT.md#output-and-diagnostics) and
[PartialPublicationTests](../DreadsMashedPatch.Tests/PartialPublicationTests.cs).
A terminal PatchRunFailedException from an old log does not describe today's
record-skip publication policy.
