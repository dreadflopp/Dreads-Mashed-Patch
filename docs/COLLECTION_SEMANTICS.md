# Collection semantics and verification

## Purpose

Every registered record-handler property whose handled value is a collection is
tracked in the generated [collection-semantics-manifest.json](collection-semantics-manifest.json). Independent
pinned-xEdit expectations live in [xedit-collection-expectations.json](xedit-collection-expectations.json).
The generated [collection-semantics-comparison.md](collection-semantics-comparison.md) compares the two. This makes
collection behavior an explicit, reviewable contract instead of relying on the
CLR collection type: a Mutagen `IReadOnlyList` can represent a sorted xEdit
array, an alignable declaration-order array, positional fields, or one atomic
structure.

The runtime manifest is deliberately observational and is not an xEdit source
of truth. The expectation file records xEdit shape, identity, alignability,
count/coupling concerns, source evidence, and rationale. The pinned source interpretation and implementation reasons are retained in
[xEdit ordering evidence](history/LIST_ORDERING_AUDIT.md) and
[collection decisions](history/LIST_ORDERING_MIGRATION.md).

The manifest includes constructible dormant handlers, including LAND. Use the
[supported/excluded routes](record-patching/unsupported.md) to determine what runs.

Use the generated comparison report for current registration counts. Each of the seven placed-projectile variants contributes five shared ReferenceRecord collection registrations. LTEX grasses use sorted Grass FormID keys. LENS sprites are nested inside `FlareDefinition`, an atomic record value, so they are documented and tested separately rather than appearing as a directly registered collection. Sprite order, duplicates and presence travel together with both flare globals.

## Modes

- `SortedKeyed`: rows are identified and emitted using the xEdit sort key.
- `AlignedOrdered`: declaration order is semantic and rows use progressive
  xEdit-style alignment.
- `ExactOrdered`: zero-based position is row identity.
- `Unordered`: complete semantic values form a multiset.
- `Atomic`: the whole collection has a single owner.
- `Specialized`: a dedicated handler owns a custom merge context that cannot be
  described by the generic modes.

Identity, cardinality, presence behavior, runtime value type, and concrete
handler type are recorded independently in the manifest. This prevents an
apparently harmless handler replacement from silently changing copy,
nullability, duplicate, or ownership behavior.

## Workflow

Run these commands from the repository root. On Linux, replace
`powershell.exe -NoProfile -ExecutionPolicy Bypass` with `pwsh -NoProfile`.
After adding, removing, or changing a collection handler, regenerate the manifest:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts/Audit-CollectionSemantics.ps1
```

Review the manifest diff against the pinned xEdit source before accepting it.
Do not regenerate merely to make a failing test pass. Confirm the xEdit array
kind, sort key, alignability, duplicate behavior, and null-versus-empty surface.

CI-style verification without rewriting the manifest is available with:

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts/Audit-CollectionSemantics.ps1 -Verify
```

To make unresolved expectations a failure (the repository currently has none):

```powershell
powershell.exe -NoProfile -ExecutionPolicy Bypass -File scripts/Audit-CollectionSemantics.ps1 -Verify -FailOnUnresolved
```

The test suite independently discovers active collection registrations and
fails if the checked-in manifest is stale. It also verifies that generic
per-entry modes use list ownership contexts, atomic modes use single-value
contexts, every registration has a complete reviewed xEdit expectation, and
only the reviewed LAND exceptions remain generically unordered.

`CollectionSemanticsBinaryRoundTripTests` additionally writes source plugins,
reads them through Mutagen binary overlays, forwards the corrected properties,
writes patch plugins, and reopens those patches. These generated fixtures cover
`BodyPartData.Parts`, `Npc.HeadParts`, `Npc.TintLayers`, and all four Armor Addon
gender/model alternate-texture surfaces. No manually authored xEdit plugins are
required for this regression coverage.

## Identity and atomic-boundary traps

- BodyPartData.Parts uses PartNode, and each ARMA world/first-person/gender
  alternate-texture collection uses (3D Name, 3D Index). These are keyed row
  collections; replacing them with atomic lists changes merge policy.
- NPC HeadParts/TintLayers and HDPT ExtraParts/Parts are nested in complete atomic
  Appearance values, not independently registered row collections. Sorted xEdit
  definitions make NPC head/tint or HDPT extra-part reordering irrelevant to
  ownership, while duplicate counts and complete contents remain significant.
  HDPT morph-file order stays significant. Copy the selected source order.
- Appearance uses ordinary whole-value ownership/reversion permissions, not an
  unconditional winning snapshot. NPC Height and OppositeGenderAnims are separate
  decisions, not nested appearance leaves.
- Structural/non-alignable arrays retain complete sequences for positional
  connections, fixed enum maps, geometry, interpolation curves or parallel data.
  A reviewed array key does not automatically make independently authored
  structures safe to union; ARMO Armature is a deliberate atomic exception.

See [appearance regressions](record-patching/REVIEW.md#appearance-boundaries) and
[pinned xEdit evidence](history/LIST_ORDERING_AUDIT.md). The manifest audits only
independent collection-value registrations; nested lists need dedicated tests.

## Intentional exclusions and specialization

Translated strings, gendered wrappers, byte arrays and memory slices may be
enumerable but are semantic scalar/binary values, not mergeable collections.
They stay in their typed handler tests. ImpactDataSet.Impacts retains a custom
merge context. Structural/dictionary/geometric values retain atomic or typed
handlers; LAND Layers/Textures remain dormant Unordered exceptions pending a
dedicated binary model.
