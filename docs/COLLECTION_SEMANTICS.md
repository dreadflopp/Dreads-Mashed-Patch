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
count/coupling concerns, source evidence, and rationale. The authoritative
historical decisions also remain in [ordering audit](history/LIST_ORDERING_AUDIT.md) and
[ordering migration](history/LIST_ORDERING_MIGRATION.md).

The manifest includes constructible dormant handlers, including LAND. Use the
[supported/excluded routes](record-patching/unsupported.md) to determine what runs.

The current manifest has 208 registrations and 208 reviewed expectations, including five shared ReferenceRecord collections for each of the seven placed-projectile variants. LTEX grasses use sorted Grass FormID keys. LENS sprites are nested inside `FlareDefinition`, an atomic record value, so they are documented and tested separately rather than appearing as a directly registered collection. Sprite order, duplicates and presence travel together with both flare globals.

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

## Completed xEdit cross-check

All active registrations now have a reviewed expectation or an intentional
exception. The cross-check corrected these mismatches:

- `BodyPartData.Parts`: positional to sorted/keyed by `PartNode`.
- `Npc.HeadParts`: atomic to sorted/keyed by Head Part FormID.
- `Npc.TintLayers`: atomic to sorted/keyed by Tint Index.
- Armor Addon male/female first-person/world-model alternate textures: atomic
  to sorted/keyed by `(3D Name, 3D Index)`.

The remaining atomic classifications have explicit structural reasons such as
graphs with positional connections, fixed enum maps, geometric grids, opaque
subrecords, whole interpolation curves, or parallel cloud-layer arrays.

## Intentional exclusions

Translated strings, gendered value wrappers, byte arrays, and memory slices can
implement enumeration interfaces but are semantic scalar or binary values, not
mergeable collections. They are excluded from this audit and remain covered by
their typed property-handler tests.

## Migration note

- Generalized: collection registration discovery, independent xEdit
  expectations, and mode drift checks are shared across all record handlers.
- Specialized: `ImpactDataSet.Impacts` retains its dedicated merge context;
  structural, geometric, executable, fixed-grid, and dictionary-shaped values
  retain atomic or typed handlers.
- Intentionally non-migrated: `Landscape.Layers` and `Landscape.Textures`
  remain the two documented `Unordered` exceptions pending a dedicated LAND
  binary model.
- Removed dead code: the superseded NPC `HeadPartsHandler` and
  `TintLayersHandler` were removed after migration to the shared sorted/keyed
  handlers.
- Diagnostics: use the verification command and the normal solution test suite;
  the binary round-trip tests exercise all seven corrected surfaces.
