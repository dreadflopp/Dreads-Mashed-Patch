# Visual effects and assets

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="addn-addonnode"></a>

## ADDN — Addon Node

| Properties | How they are patched |
|---|---|
| `NodeIndex`, `Sound`, `MasterParticleSystemCap` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="arto-artobject"></a>

## ARTO — Art Object

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `ArtModel` (Meshes) | `ModelAndBounds`, `Type` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="cams-camerashot"></a>

## CAMS — Camera Shot

Action, location, target, timing, distances and flags follow their ordinary scalar or flag rules independently of the mesh reference.

| Properties | How they are patched |
|---|---|
| `Action`, `Location`, `Target`, `TimeMultiplierPlayer`, `TimeMultiplierTarget`, `TimeMultiplierGlobal`, `MaxTime`, `MinTime`, `TargetPercentBetweenActors`, `NearTargetDistance` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ImageSpaceModifier` | Select each value separately. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `CameraModel` (Meshes) | `Model` | Normal | Use the existing complete-value or collection policy. |


<a id="cpth-camerapath"></a>

## CPTH — Camera Path

| Properties | How they are patched |
|---|---|
| `Zoom`, `ZoomMustHaveCameraShots` | Select each value separately. |
| `RelatedPaths` | Select each whole collection separately. |
| `Conditions`, `Shots` | Merge aligned rows in order. |


<a id="debr-debris"></a>

## DEBR — Debris

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `Models` (Meshes) | `Models` | Normal | Use the existing complete-value or collection policy. |


<a id="efsh-effectshader"></a>

## EFSH — Effect Shader

EffectShaderData keeps the remaining DATA controls together, including Unknown, Flags, AddonModels, AmbientSound, colours, blending and timing. It forwards separately from the texture groups. FillTextureCoordinates contains FillTextureScaleU/V. ParticleAtlas contains TextureCountU/V and the ParticleAnimated start/end/loop/frame-count values and their variations, so atlas indices match the selected particle image.

| Properties | How they are patched |
|---|---|
| `EffectShaderData` | Select the remaining effect DATA as one value, excluding texture coordinates and atlas indices. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `MembraneTextures` (Textures) | `FillTexture`, `HolesTexture`, `MembranePaletteTexture`, `FillTextureCoordinates` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `ParticleTextures` (Textures) | `ParticleShaderTexture`, `ParticlePaletteTexture`, `ParticleAtlas` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="imgs-imagespace"></a>

## IMGS — Image Space

| Properties | How they are patched |
|---|---|
| `ENAM`, `Hdr`, `Cinematic`, `Tint`, `DepthOfField` | Select each whole value separately. |


<a id="ipct-impact"></a>

## IPCT — Impact

Duration, orientation, angle threshold and placement radius forward independently of the mesh.

Decal presence gates its child writes: an absent decal suppresses them. Bounds groups MinWidth, MaxWidth, MinHeight and MaxHeight; Parallax groups scale and passes. Depth, Shininess, Flags and Color remain separate. NoDecalData is independent of the presence decision.

| Properties | How they are patched |
|---|---|
| `Duration`, `Orientation`, `AngleThreshold`, `PlacementRadius` | Select each value separately. |
| `SoundLevel`, `NoDecalData`, `Result`, `Decal.Presence`, `Decal.Depth`, `Decal.Shininess`, `Decal.Color`, `TextureSet`, `SecondaryTextureSet`, `Sound1`, `Sound2`, `Hazard` | Select each value separately. |
| `Decal.Flags` | Merge registered flag bits separately. |
| `Decal.Bounds`, `Decal.Parallax` | Select each whole value separately. |
| `Unknown`, `Decal.Unknown` | Not independently forwarded. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `ImpactModel` (Meshes) | `Model` | Normal | Use the existing complete-value or collection policy. |


<a id="ipds-impactdataset"></a>

## IPDS — Impact Data Set

Impacts merges mappings by material with ownership checks. Invalid, null or duplicate mappings, or an unavailable source mod, use the stored winning-list fallback.

| Properties | How they are patched |
|---|---|
| `Impacts` | Special material mapping. |


<a id="lens-lensflare"></a>

## LENS — Lens Flare

The ordered sprite sequence keeps textures with their per-sprite data. Global ColorInfluence and FadeDistanceRadiusScale forward separately and are separate from the texture group. Shared EditorID and header flags remain independent.

| Properties | How they are patched |
|---|---|
| `ColorInfluence`, `FadeDistanceRadiusScale` | Select each value separately. |
| `Sprites` contents | Each copied row retains its ID, texture and complete nullable Data: tint RGB, Width, Height, Position, AngularFade, Opacity and Flags. These fields and embedded flags have no independent merge path. |

Sprite equality uses serialized RGB (alpha is not stored in LFSD), all other Data fields, IDs, order and duplicates. Textures use the project's case/slash-aware GivenPath comparison; selected copies retain the authored path. Mutagen writes the derived LFSP sprite count. There is no per-row alignment or per-bit sprite flag merge.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `FlareSprites` (Textures) | `FlareSprites` | Normal | Use the existing complete-value or collection policy. |


<a id="mato-materialobject"></a>

## MATO — Material Object

The NIF model forwards as its own mesh group. DNAMs, material falloff, UV/projection parameters, normal dampening, colour, HasSnow and approved Flags forwarding retain their ordinary decisions. Opaque DNAM bytes are not interpreted as texture references.

| Properties | How they are patched |
|---|---|
| `FalloffScale`, `FalloffBias`, `NoiseUvScale`, `MaterialUvScale`, `ProjectionVector`, `NormalDampener`, `SinglePassColor`, `HasSnow` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `DNAMs` | Select the complete opaque byte sequence. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `MaterialModel` (Meshes) | `Model` | Normal | Use the existing complete-value or collection policy. |


<a id="matt-materialtype"></a>

## MATT — Material Type

| Properties | How they are patched |
|---|---|
| `Parent`, `Name`, `HavokDisplayColor`, `Buoyancy`, `HavokImpactDataSet` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |


<a id="rfct-visualeffect"></a>

## RFCT — Visual Effect

| Properties | How they are patched |
|---|---|
| `EffectArt`, `Shader` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |


<a id="spgd-shaderparticlegeometry"></a>

## SPGD — Shader Particle Geometry

Only the particle texture and its subtexture grid dimensions stay together. Gravity, rotation, size, offsets, density, box size and particle Type forward normally.

| Properties | How they are patched |
|---|---|
| `GravityVelocity`, `RotationVelocity`, `ParticleSizeX`, `ParticleSizeY`, `CenterOffsetMin`, `CenterOffsetMax`, `InitialRotationRange`, `Type`, `BoxSize`, `ParticleDensity` | Select each value separately. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `ParticleTexture` (Textures) | `NumSubtexturesX`, `NumSubtexturesY`, `ParticleTexture` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="txst-textureset"></a>

## TXST — Texture Set

TextureDefinition keeps all eight texture paths and nullable Flags together. Those flags are not independently merged. Decal remains separate; ObjectBounds stays with the effective winner.

| Properties | How they are patched |
|---|---|
| `Decal` | Select each whole value separately. |
| `ObjectBounds` | Keep the effective winner's bounds; no independent forwarding. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `TextureDefinition` (Textures) | `TextureDefinition` | Normal | Use the existing complete-value or collection policy. |


<a id="voli-volumetriclighting"></a>

## VOLI — Volumetric Lighting

The twelve nullable lighting parameters form one complete preset. Color, density, phase and sampling controls select together so that an earlier component cannot leak into a later authored preset. Shared EditorID and header flags remain independent.

| Properties | How they are patched |
|---|---|
| `LightingPreset` | Select all twelve parameters together: `Intensity`, `CustomColorContribution`, `ColorR`, `ColorG`, `ColorB`, `DensityContribution`, `DensitySize`, `DensityWindSpeed`, `DensityFallingSpeed`, `PhaseFunctionContribution`, `PhaseFunctionScattering`, `SamplingRepartitionRangeFactor`. Presence and exact float values participate in comparison. |
