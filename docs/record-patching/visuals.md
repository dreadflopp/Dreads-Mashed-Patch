# Visual effects and assets

[Patching guide](README.md) · [Known issues](KNOWN-ISSUES.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="addn-addonnode"></a>

## ADDN — Addon Node

| Properties | How they are patched |
|---|---|
| `NodeIndex`, `Sound`, `MasterParticleSystemCap` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/AddonNodeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="arto-artobject"></a>

## ARTO — Art Object

| Properties | How they are patched |
|---|---|
| `Type` | Select each value separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ArtObjectRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="cams-camerashot"></a>

## CAMS — Camera Shot

| Properties | How they are patched |
|---|---|
| `Action`, `Location`, `Target`, `TimeMultiplierPlayer`, `TimeMultiplierTarget`, `TimeMultiplierGlobal`, `MaxTime`, `MinTime`, `TargetPercentBetweenActors`, `NearTargetDistance`, `ImageSpaceModifier` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Model` | Select each whole value separately. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/CameraShotRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="cpth-camerapath"></a>

## CPTH — Camera Path

| Properties | How they are patched |
|---|---|
| `Zoom`, `ZoomMustHaveCameraShots` | Select each value separately. |
| `RelatedPaths` | Select each whole collection separately. |
| `Conditions`, `Shots` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/CameraPathRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="debr-debris"></a>

## DEBR — Debris

| Properties | How they are patched |
|---|---|
| `Models` | Select each whole collection separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/DebrisRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="efsh-effectshader"></a>

## EFSH — Effect Shader

EffectShaderData keeps all DATA controls together, including Unknown, Flags, AddonModels and AmbientSound. Only the five listed texture paths are separate from that value.

| Properties | How they are patched |
|---|---|
| `FillTexture`, `ParticleShaderTexture`, `HolesTexture`, `MembranePaletteTexture`, `ParticlePaletteTexture` | Select each value separately. |
| `EffectShaderData` | Select each whole value separately. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/EffectShaderRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="imgs-imagespace"></a>

## IMGS — Image Space

| Properties | How they are patched |
|---|---|
| `ENAM`, `Hdr`, `Cinematic`, `Tint`, `DepthOfField` | Select each whole value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ImageSpaceRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="ipct-impact"></a>

## IPCT — Impact

Decal presence gates its child writes: an absent decal suppresses them. Bounds groups MinWidth, MaxWidth, MinHeight and MaxHeight; Parallax groups scale and passes. Depth, Shininess, Flags and Color remain separate. NoDecalData is independent of the presence decision.

| Properties | How they are patched |
|---|---|
| `Duration`, `Orientation`, `AngleThreshold`, `PlacementRadius`, `SoundLevel`, `NoDecalData`, `Result`, `Decal.Presence`, `Decal.Depth`, `Decal.Shininess`, `Decal.Color`, `TextureSet`, `SecondaryTextureSet`, `Sound1`, `Sound2`, `Hazard` | Select each value separately. |
| `Decal.Flags` | Merge registered flag bits separately. |
| `Model`, `Decal.Bounds`, `Decal.Parallax` | Select each whole value separately. |
| `Unknown`, `Decal.Unknown` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ImpactRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="ipds-impactdataset"></a>

## IPDS — Impact Data Set

Impacts merges mappings by material with ownership checks. Invalid, null or duplicate mappings, or an unavailable source mod, use the stored winning-list fallback.

| Properties | How they are patched |
|---|---|
| `Impacts` | Special material mapping. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ImpactDataSetRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="mato-materialobject"></a>

## MATO — Material Object

| Properties | How they are patched |
|---|---|
| `FalloffScale`, `FalloffBias`, `NoiseUvScale`, `MaterialUvScale`, `ProjectionVector`, `NormalDampener`, `SinglePassColor`, `HasSnow` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Model` | Select each whole value separately. |
| `DNAMs` | Select each whole collection separately. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/MaterialObjectRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="matt-materialtype"></a>

## MATT — Material Type

| Properties | How they are patched |
|---|---|
| `Parent`, `Name`, `HavokDisplayColor`, `Buoyancy`, `HavokImpactDataSet` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/MaterialTypeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="rfct-visualeffect"></a>

## RFCT — Visual Effect

| Properties | How they are patched |
|---|---|
| `EffectArt`, `Shader` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/VisualEffectRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="spgd-shaderparticlegeometry"></a>

## SPGD — Shader Particle Geometry

| Properties | How they are patched |
|---|---|
| `GravityVelocity`, `RotationVelocity`, `ParticleSizeX`, `ParticleSizeY`, `CenterOffsetMin`, `CenterOffsetMax`, `InitialRotationRange`, `NumSubtexturesX`, `NumSubtexturesY`, `Type`, `BoxSize`, `ParticleDensity`, `ParticleTexture` | Select each value separately. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ShaderParticleGeometryRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="txst-textureset"></a>

## TXST — Texture Set

TextureDefinition keeps all eight texture paths and nullable Flags together. Those flags are not independently merged. Decal and ObjectBounds remain separate.

| Properties | How they are patched |
|---|---|
| `ObjectBounds`, `TextureDefinition`, `Decal` | Select each whole value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/TextureSetRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).
