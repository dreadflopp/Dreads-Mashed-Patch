# Asset copying constraints

[Internal references](README.md) · [Handler selection](../DEVELOPMENT.md#handler-selection)

## Serialized paths

Copy Mutagen asset links from GivenPath, the text written to the plugin.
DataRelativePath is optional lookup/diagnostic information. Compare case and
slash direction without adding/removing Data, Meshes, Textures, Sound or Music
prefixes. AssetLink.ToString() and lookup-normalized paths are not setter inputs.
Binary-overlay getter links must become independent mutable links or generated
copies; generic complex reflection copying can lose nested assets.

## Record-specific boundaries

| Surface | Reuse and retained specialization | Reason |
|---|---|---|
| Independent model/behavior/texture link | SimpleReflectionAssetLinkPropertyHandler or existing typed accessor | Preserve serialized text and mutable link construction. |
| Shared Model | Generated complete model copy within ModelBoundsHandler; metadata and alternate textures stay with the model | A filename and its nested payload must survive overlays without sharing mutable source data. Bounds have [conditional ownership](MODEL_BOUNDS_MIGRATION.md). |
| ARMA | Separate male/female world/first-person filenames and alternate-texture collections | Preserve established ownership; filename copying carries Data, but byte-only differences do not claim the filename. |
| ARMO | Generated gendered world-model/icon copying; world filenames coordinate bounds | Worn armature is a different surface from dropped/world geometry. |
| STAT / WEAP | STAT LOD and WEAP ScopeModel remain separate from their main model | Do not couple independent xEdit fields merely because both contain paths. |
| WTHR / SNDR / MUSC / MUST | Indexed cloud textures and sound paths preserve slots; track/finale remain separate fields | Index/order and field boundaries are semantic, not a path-normalization concern. |
| Icons / destructible / Aurora / REGN | Generated aggregate copies retain nested assets and payloads | A matching concrete object type is not evidence that a shallow copy is safe. REGN Areas remain geometric rather than generic asset rows. |
| DEBR | Complete ordered model payload with generated item copies | ModelFilename, order and binary metadata travel together. |
| NPC / HDPT / TXST | Asset links travel inside the current atomic appearance/texture groups | Do not resurrect historical independent head-part morph or texture-slot paths. |

Regression entry points: [AssetPathHandlingTests](../../DreadsMashedPatch.Tests/AssetPathHandlingTests.cs),
[OverlayAggregateCopyTests](../../DreadsMashedPatch.Tests/OverlayAggregateCopyTests.cs),
[SoundFilesHandlerTests](../../DreadsMashedPatch.Tests/SoundFilesHandlerTests.cs).
Use mutable and binary-overlay inputs, prefix distinctions, source detachment
and output reload when changing these paths.
