# World and environment

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="cell-cell"></a>

## CELL — Cell

Lighting includes its inheritance flags and Versioning. WaterHeight is not independently forwarded. Child records are handled separately. For Tamriel’s persistent cell (000D74:Skyrim.esm), default Hybrid selects the Dawnguard header only when the winning header matches Skyrim’s, excluding children and group metadata. Otherwise normal merging applies. Other modes select Skyrim, Dawnguard, the winner or normal merging. Missing Hybrid baselines use normal merging; a missing explicitly preferred source uses the winner. A matching priority-mod rule takes precedence.

| Properties | How they are patched |
|---|---|
| `Name`, `Location`, `Owner`, `Water`, `LightingTemplate`, `AcousticSpace`, `EncounterZone`, `Music`, `ImageSpace`, `SkyAndWeatherFromRegion`, `WaterNoiseTexture`, `FactionRank`, `LockList`, `WaterEnvironmentMap` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Lighting`, `Grid`, `MaxHeightData`, `WaterVelocity`, `XWCN`, `XWCS`, `OcclusionData`, `LNAM` | Select each whole value separately. |
| `Regions` | Merge rows by key. |
| `WaterHeight` | Not independently forwarded. |
| `Landscape`, `NavigationMeshes`, `Persistent`, `PersistentTimestamp`, `PersistentUnknownGroupData`, `Temporary`, `TemporaryTimestamp`, `TemporaryUnknownGroupData`, `Timestamp`, `UnknownGroupData` | Child/group surfaces; no independent property merge. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/CellRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="clmt-climate"></a>

## CLMT — Climate

| Properties | How they are patched |
|---|---|
| `SunTexture`, `SunGlareTexture`, `SunriseBegin`, `SunriseEnd`, `SunsetBegin`, `SunsetEnd`, `Volatility`, `Moons`, `PhaseLength` | Select each value separately. |
| `Model` | Select each whole value separately. |
| `WeatherTypes` | Merge rows by key. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ClimateRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="eczn-encounterzone"></a>

## ECZN — Encounter Zone

| Properties | How they are patched |
|---|---|
| `Owner`, `Location`, `Rank`, `MinLevel`, `MaxLevel` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/EncounterZoneRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="gras-grass"></a>

## GRAS — Grass

| Properties | How they are patched |
|---|---|
| `Density`, `MinSlope`, `MaxSlope`, `UnitsFromWater`, `UnitsFromWaterType`, `PositionRange`, `HeightRange`, `ColorRange`, `WavePeriod` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `ModelAndBounds` | Conditional model/bounds coupling; see the [shared rule](README.md#properties-shared-across-records). |
| `Unknown`, `Unknown2`, `Unknown3` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/GrassRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="lcrt-locationreferencetype"></a>

## LCRT — Location Reference Type

| Properties | How they are patched |
|---|---|
| `Color` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/LocationReferenceTypeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="lctn-location"></a>

## LCTN — Location

| Properties | How they are patched |
|---|---|
| `Name`, `ParentLocation`, `Music`, `UnreportedCrimeFaction`, `WorldLocationMarkerRef`, `WorldLocationRadius`, `HorseMarkerRef`, `Color` | Select each value separately. |
| `Keywords` | Merge rows by key. |
| `EnableParentReferencesAdded`, `EnableParentReferencesStatic`, `InitiallyDisabledReferencesAdded`, `InitiallyDisabledReferencesStatic`, `LocationRefTypeReferencesAdded`, `LocationRefTypeReferencesRemoved`, `LocationRefTypeReferencesStatic`, `PersistentActorReferencesAdded`, `PersistentActorReferencesRemoved`, `PersistentActorReferencesStatic`, `UniqueActorReferencesAdded`, `UniqueActorReferencesRemoved`, `UniqueActorReferencesStatic`, `WorldspaceCellsAdded`, `WorldspaceCellsRemoved`, `WorldspaceCellsStatic` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/LocationRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="lgtm-lightingtemplate"></a>

## LGTM — Lighting Template

| Properties | How they are patched |
|---|---|
| `AmbientColor`, `DirectionalColor`, `FogNearColor`, `FogNear`, `FogFar`, `DirectionalRotationXY`, `DirectionalRotationZ`, `DirectionalFade`, `FogClipDistance`, `FogPower`, `FogFarColor`, `FogMax`, `LightFadeStartDistance`, `LightFadeEndDistance` | Select each value separately. |
| `AmbientColors`, `DirectionalAmbientColors` | Select each whole value separately. |
| `Unknown` | Not independently forwarded. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/LightingTemplateRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="ltex-landscapetexture"></a>

## LTEX — Landscape Texture

LTEX is supported independently of excluded LAND terrain geometry. The xEdit definition describes texture/material links, two HNAM scalar fields, a SNAM scalar, sorted GNAM grass references, and nullable SSE INAM snow flags. Friction and restitution remain separate authored decisions even though they share HNAM.

| Properties | How they are patched |
|---|---|
| `TextureSet`, `MaterialType`, `HavokFriction`, `HavokRestitution`, `TextureSpecularExponent` | Select each value separately. |
| `Grasses` | Merge by Grass FormID and emit in sorted order, retaining duplicate occurrences. |
| `Flags` | Use the approved nullable flag handler for IsSnow, with flag presence tracked separately from the bit value. Absent INAM and present zero are distinct. |

Mutagen exposes HNAM/SNAM scalars without nullable subrecord presence; copying/writing can normalize absent scalar subrecords. INAM serialization requires a compatible SSE FormVersion (43 or later). Support does not promise preservation of unsupported on-disk representations.

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/LandscapeTextureRecordHandler.cs).

<a id="navm-navigationmesh"></a>

## NAVM — Navigation Mesh

**Disabled by default; these rules apply when enabled.**

Data keeps all geometry and connectivity together; vertices and triangles do not merge independently. The separate binary payloads do not rebuild NAVI.

| Properties | How they are patched |
|---|---|
| `Data`, `ONAM`, `PNAM`, `NNAM` | Select each whole value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/NavigationMeshRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="regn-region"></a>

## REGN — Region

RegionAreas keeps area order and each polygon together; comparison normalizes polygon direction only. Objects, Weather, Map, Land, Grasses and Sounds are six separate whole structures, including their nested entries.

| Properties | How they are patched |
|---|---|
| `MapColor`, `Worldspace` | Select each value separately. |
| `Objects`, `Weather`, `Map`, `Land`, `Grasses`, `Sounds` | Select each whole value separately. |
| `RegionAreas` | Select each whole collection separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/RegionRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="tree-tree"></a>

## TREE — Tree

TREE remains enabled by default. Under **Smart Forwarding**, **Restrict tree forwarding (Recommended)** is checked by default. This checkbox controls the restriction; use **Record Types** to enable or disable TREE patching altogether. **Priority Mods** takes precedence by selecting a complete record. The winning record below means the last eligible override after Ignored Mods filtering.

With the checkbox checked, the patcher does **not forward models, bounds, wind settings, scripts or record-header flags from other overrides**. It keeps those fields from the winning record. A tree's model and wind settings describe how its trunk, branches and leaves move; taking them from different mods can combine settings that neither mod authored. Independently merged bounds, scripts and flags can also change the tree's appearance or behavior.

| Excluded properties while restricted | What is preserved and why |
|---|---|
| Model and bounds | Keep the winning record's complete values so an earlier model or bounds edit cannot replace part of its appearance. |
| `TreeData` (CNAM) | Keep the winning record's complete animation configuration, including all wind values, with its model. |
| `VirtualMachineAdapter` (VMAD/scripts) | Keep the winning adapter so scripts from other overrides cannot add or change behavior. |
| Record-header flags | Keep the winning flags so independently selected bits cannot change the authored record behavior. |

**Unchecking the checkbox uses ordinary forwarding for these fields.** Model/bounds uses the shared conditional coupling rule, scripts merge by key, approved header flag bits forward normally, and complete CNAM values use ordinary value selection and master/virtual-master restoration permissions. This can combine fields from different mods. There is no model-filename compatibility check or separate model/wind coordination step.

CNAM stays atomic with either checkbox value: `TrunkFlexibility`, `BranchFlexibility`, `LeafAmplitude`, `LeafFrequency` and Mutagen's 32-byte `Unknown` block travel together. That block contains eight additional wind values and is preserved exactly; its name does not make it disposable data. It is never assembled from individual wind values across mods.

The restriction does not affect `EditorID`, `Name`, `Ingredient` (PFIG), `HarvestSound` (SNAM) or `Production`. These follow the [shared selection rules](README.md), including the separate Editor ID setting. All four seasonal Production values are selected together; the remaining fields are separate decisions.

For example, suppose mod A replaces a tree's model and wind data, then mod B changes harvesting but carries the original appearance. With the restriction enabled, the patch keeps B's winning model, bounds and wind data. It cannot infer whether B intended to restore the original appearance. An authored compatibility patch can explicitly select A's appearance and B's harvesting. Disabling the restriction instead allows ordinary forwarding, which may preserve earlier edits without establishing that the resulting fields are compatible.

The patcher does not inspect the physical mesh chosen by your mod manager. Two mods can install different meshes at the same path, so preserving the winning record cannot guarantee that external assets match it. Review the result with the assets installed by your modlist.

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/TreeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="watr-water"></a>

## WATR — Water

Flags is currently selected as one complete nullable scalar value, rather than merged per bit. UnusedNoisemaps is intentionally unregistered.

| Properties | How they are patched |
|---|---|
| `Name`, `Opacity`, `Flags`, `Material`, `OpenSound`, `Spell`, `ImageSpace`, `DamagePerSecond`, `SpecularSunPower`, `WaterReflectivity`, `WaterFresnel`, `FogAboveWaterDistanceNearPlane`, `FogAboveWaterDistanceFarPlane`, `ShallowColor`, `DeepColor`, `ReflectionColor`, `DisplacementStartingSize`, `DisplacementFoce`, `DisplacementVelocity`, `DisplacementFalloff`, `DisplacementDampner`, `NoiseFalloff`, `NoiseLayerOneWindDirection`, `NoiseLayerTwoWindDirection`, `NoiseLayerThreeWindDirection`, `NoiseLayerOneWindSpeed`, `NoiseLayerTwoWindSpeed`, `NoiseLayerThreeWindSpeed`, `FogAboveWaterAmount`, `FogUnderWaterAmount`, `FogUnderWaterDistanceNearPlane`, `FogUnderWaterDistanceFarPlane`, `WaterRefractionMagnitude`, `SpecularPower`, `SpecularRadius`, `SpecularBrightness`, `NoiseLayerOneUvScale`, `NoiseLayerTwoUvScale`, `NoiseLayerThreeUvScale`, `NoiseLayerOneAmplitudeScale`, `NoiseLayerTwoAmplitudeScale`, `NoiseLayerThreeAmplitudeScale`, `WaterReflectionMagnitude`, `SpecularSunSparkleMagnitude`, `SpecularSunSpecularMagnitude`, `DepthReflections`, `DepthRefraction`, `DepthNormals`, `DepthSpecularLighting`, `SpecularSunSparklePower`, `NoiseFlowmapScale`, `LinearVelocity`, `AngularVelocity`, `NoiseLayerOneTexture`, `NoiseLayerTwoTexture`, `NoiseLayerThreeTexture`, `FlowNormalsNoiseTexture` | Select each value separately. |
| `MNAM`, `GNAM` | Select each whole value separately. |
| `Unknown`, `Unknown2`, `Unknown3`, `Unknown4`, `Unknown5`, `Unknown6`, `Unknown7`, `UnusedNoisemaps` | Not independently forwarded. |
| `DNAMDataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/WaterRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="wrld-worldspace"></a>

## WRLD — Worldspace

LodData groups LodWater and LodWaterHeight; a water link without a height is rejected. WorldMapOffset groups scale and cell offset. Missing DistantLodMultiplier means 1.0. Child records are handled separately, not as whole worldspace collections.

**Ownership reset:** a change to `Parent`, `Climate`, `Water`, `LodData`, `LandDefaults`, `MapData`, `Flags`, `FixedDimensionsCenterCell` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Name`, `Location`, `Water`, `Music`, `MapImage`, `DistantLodMultiplier`, `FixedDimensionsCenterCell`, `InteriorLighting`, `EncounterZone`, `Climate`, `WaterNoiseTexture`, `HdLodDiffuseTexture`, `HdLodNormalTexture`, `WaterEnvironmentMap` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `LodData`, `MapData`, `CloudModel`, `WorldMapOffset`, `Parent`, `LandDefaults` | Select each whole value separately. |
| `CanopyShadow`, `MaxHeight`, `ObjectBoundsMax`, `ObjectBoundsMin` | Not independently forwarded. |
| `LargeReferences`, `OffsetData`, `SubCells`, `SubCellsTimestamp`, `SubCellsUnknown`, `TopCell` | Child/group surfaces; no independent property merge. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/WorldspaceRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="wthr-weather"></a>

## WTHR — Weather

CloudTextures and Clouds are separate whole lists. Each named color structure, ImageSpaces, VolumetricLighting and DirectionalAmbientLightingColors keeps its time-of-day components together. These references select identities; the referenced [VOLI](visuals.md#voli-volumetriclighting) and [LENS](visuals.md#lens-lensflare) records have separate enabled processing routes.

| Properties | How they are patched |
|---|---|
| `Precipitation`, `VisualEffect`, `FogDistanceDayNear`, `FogDistanceDayFar`, `FogDistanceNightNear`, `FogDistanceNightFar`, `FogDistanceDayPower`, `FogDistanceNightPower`, `FogDistanceDayMax`, `FogDistanceNightMax`, `WindSpeed`, `TransDelta`, `SunGlare`, `SunDamage`, `PrecipitationBeginFadeIn`, `PrecipitationEndFadeOut`, `ThunderLightningBeginFadeIn`, `ThunderLightningEndFadeOut`, `ThunderLightningFrequency`, `LightningColor`, `VisualEffectBegin`, `VisualEffectEnd`, `WindDirection`, `WindDirectionRange`, `SunGlareLensFlare` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `DNAM`, `CNAM`, `ANAM`, `BNAM`, `LNAM`, `ONAM`, `SkyUpperColor`, `FogNearColor`, `UnknownColor`, `AmbientColor`, `SunlightColor`, `SunColor`, `StarsColor`, `SkyLowerColor`, `HorizonColor`, `EffectLightingColor`, `CloudLodDiffuseColor`, `CloudLodAmbientColor`, `FogFarColor`, `SkyStaticsColor`, `WaterMultiplierColor`, `SunGlareColor`, `MoonGlareColor`, `ImageSpaces`, `VolumetricLighting`, `DirectionalAmbientLightingColors`, `NAM2`, `NAM3`, `Aurora` | Select each whole value separately. |
| `CloudTextures`, `Clouds` | Select each whole collection separately. |
| `SkyStatics` | Merge rows by key. |
| `Sounds` | Merge rows by position. |
| `Unknown` | Not independently forwarded. |
| `NAM0DataTypeState` | Serialization/unused state; no independent decision. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/WeatherRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).
