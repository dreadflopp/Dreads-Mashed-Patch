# World and environment

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="cell-cell"></a>

## CELL — Cell

WaterNoiseTexture and WaterEnvironmentMap forward as a texture group; the Water record selection retains its ordinary decision.

Lighting includes its inheritance flags and Versioning. WaterHeight is not independently forwarded. Child records are handled separately. For Tamriel’s persistent cell (000D74:Skyrim.esm), default Hybrid selects the Dawnguard header only when the winning header matches Skyrim’s, excluding children and group metadata. Otherwise normal merging applies. Other modes select Skyrim, Dawnguard, the winner or normal merging. Missing Hybrid baselines use normal merging; a missing explicitly preferred source uses the winner. A matching priority-mod rule takes precedence.

| Properties | How they are patched |
|---|---|
| `Water` | Select the record reference separately. |
| `Name`, `Location`, `Owner`, `LightingTemplate`, `AcousticSpace`, `EncounterZone`, `Music`, `ImageSpace`, `SkyAndWeatherFromRegion`, `FactionRank`, `LockList` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Lighting`, `Grid`, `MaxHeightData`, `WaterVelocity`, `XWCN`, `XWCS`, `OcclusionData`, `LNAM` | Select each whole value separately. |
| `Regions` | Merge rows by key. |
| `WaterHeight` | Not independently forwarded. |
| `Landscape`, `NavigationMeshes`, `Persistent`, `PersistentTimestamp`, `PersistentUnknownGroupData`, `Temporary`, `TemporaryTimestamp`, `TemporaryUnknownGroupData`, `Timestamp`, `UnknownGroupData` | Child/group surfaces; no independent property merge. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `WaterDisplay` (Textures) | `WaterNoiseTexture`, `WaterEnvironmentMap` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="clmt-climate"></a>

## CLMT — Climate

| Properties | How they are patched |
|---|---|
| `SunriseBegin`, `SunriseEnd`, `SunsetBegin`, `SunsetEnd`, `Volatility`, `Moons`, `PhaseLength` | Select each value separately. |
| `WeatherTypes` | Merge rows by key. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `Model` (Meshes) | `Model` | Normal | Use the existing complete-value or collection policy. |
| `SunImages` (Textures) | `SunTexture`, `SunGlareTexture` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="eczn-encounterzone"></a>

## ECZN — Encounter Zone

| Properties | How they are patched |
|---|---|
| `Owner`, `Location`, `Rank`, `MinLevel`, `MaxLevel` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |


<a id="gras-grass"></a>

## GRAS — Grass

| Properties | How they are patched |
|---|---|
| `Density`, `MinSlope`, `MaxSlope`, `UnitsFromWater`, `UnitsFromWaterType`, `PositionRange`, `HeightRange`, `ColorRange`, `WavePeriod` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Unknown`, `Unknown2`, `Unknown3` | Not independently forwarded. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `ModelAndBounds` (Meshes) | `ModelAndBounds` | Normal | Select the model value; retain winning bounds for matching filenames, otherwise carry source bounds. |


<a id="lcrt-locationreferencetype"></a>

## LCRT — Location Reference Type

| Properties | How they are patched |
|---|---|
| `Color` | Select each value separately. |


<a id="lctn-location"></a>

## LCTN — Location

| Properties | How they are patched |
|---|---|
| `Name`, `ParentLocation`, `Music`, `UnreportedCrimeFaction`, `WorldLocationMarkerRef`, `WorldLocationRadius`, `HorseMarkerRef`, `Color` | Select each value separately. |
| `Keywords` | Merge rows by key. |
| `EnableParentReferencesAdded`, `EnableParentReferencesStatic`, `InitiallyDisabledReferencesAdded`, `InitiallyDisabledReferencesStatic`, `LocationRefTypeReferencesAdded`, `LocationRefTypeReferencesRemoved`, `LocationRefTypeReferencesStatic`, `PersistentActorReferencesAdded`, `PersistentActorReferencesRemoved`, `PersistentActorReferencesStatic`, `UniqueActorReferencesAdded`, `UniqueActorReferencesRemoved`, `UniqueActorReferencesStatic`, `WorldspaceCellsAdded`, `WorldspaceCellsRemoved`, `WorldspaceCellsStatic` | Not independently forwarded. |


<a id="lgtm-lightingtemplate"></a>

## LGTM — Lighting Template

| Properties | How they are patched |
|---|---|
| `AmbientColor`, `DirectionalColor`, `FogNearColor`, `FogNear`, `FogFar`, `DirectionalRotationXY`, `DirectionalRotationZ`, `DirectionalFade`, `FogClipDistance`, `FogPower`, `FogFarColor`, `FogMax`, `LightFadeStartDistance`, `LightFadeEndDistance` | Select each value separately. |
| `AmbientColors`, `DirectionalAmbientColors` | Select each whole value separately. |
| `Unknown` | Not independently forwarded. |
| `DATADataTypeState` | Serialization/unused state; no independent decision. |


<a id="ltex-landscapetexture"></a>

## LTEX — Landscape Texture

LTEX is supported independently of excluded LAND terrain geometry. The xEdit definition describes texture/material links, two HNAM scalar fields, a SNAM scalar, sorted GNAM grass references, and nullable SSE INAM snow flags. Friction and restitution remain separate authored decisions even though they share HNAM.

| Properties | How they are patched |
|---|---|
| `MaterialType`, `HavokFriction`, `HavokRestitution`, `TextureSpecularExponent` | Select each value separately. |
| `Grasses` | Merge by Grass FormID and emit in sorted order, retaining duplicate occurrences. |
| `Flags` | Use the approved nullable flag handler for IsSnow, with flag presence tracked separately from the bit value. Absent INAM and present zero are distinct. |

Mutagen exposes HNAM/SNAM scalars without nullable subrecord presence; copying/writing can normalize absent scalar subrecords. INAM serialization requires a compatible SSE FormVersion (43 or later). Support does not promise preservation of unsupported on-disk representations.

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `LandscapeMaterial` (Textures) | `TextureSet` | Normal | Use the existing complete-value or collection policy. |


<a id="navm-navigationmesh"></a>

## NAVM — Navigation Mesh

**Disabled by default; these rules apply when enabled.**

Data keeps all geometry and connectivity together; vertices and triangles do not merge independently. The separate binary payloads do not rebuild NAVI.

| Properties | How they are patched |
|---|---|
| `Data`, `ONAM`, `PNAM`, `NNAM` | Select each whole value separately. |


<a id="regn-region"></a>

## REGN — Region

RegionAreas keeps area order and each polygon together; comparison normalizes polygon direction only. Objects, Weather, Map, Land, Grasses and Sounds retain separate ordinary structural decisions, including entry presence, flags and priority. Each entry's inherited Icons have a separate grouped forwarding decision. Sounds has separate sound and music reference decisions. Grass placement and other region content stay separate from Icons; reference writes cannot restore a removed entry.

| Properties | How they are patched |
|---|---|
| `Objects`, `Weather`, `Map`, `Land`, `Grasses`, `Sounds` | Select each ordinary structure separately, excluding its separately gated Icons and sound/music references. |
| `MapColor`, `Worldspace` | Select each value separately. |
| `RegionAreas` | Select each whole collection separately. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `ObjectsIcons` (IconsAndImages) | `Objects.Icons` | Normal | Select only these references; removed region entries stay absent. |
| `WeatherIcons` (IconsAndImages) | `Weather.Icons` | Normal | Select only these references; removed region entries stay absent. |
| `MapIcons` (IconsAndImages) | `Map.Icons` | Normal | Select only these references; removed region entries stay absent. |
| `LandIcons` (IconsAndImages) | `Land.Icons` | Normal | Select only these references; removed region entries stay absent. |
| `GrassesIcons` (IconsAndImages) | `Grasses.Icons` | Normal | Select only these references; removed region entries stay absent. |
| `SoundsIcons` (IconsAndImages) | `Sounds.Icons` | Normal | Select only these references; removed region entries stay absent. |
| `RegionSounds` (Sounds) | `Sounds.Sounds` | Normal | Select only these references; removed region entries stay absent. |
| `RegionMusic` (Music) | `Sounds.Music` | Normal | Select only these references; removed region entries stay absent. |


<a id="tree-tree"></a>

## TREE — Tree

TREE remains enabled by default. Under **Smart Forwarding**, **Forward tree changes
as a group** is unchecked by default. This is the only forwarding option for these groups;
other resource and appearance groups forward normally. Use **Record Types** to enable
or disable TREE patching altogether. Priority Mods still selects a complete record.

With this rule **off**, model references, bounds, wind data, script attachments and
supported record-header flags stay with the effective winner. With it **on**, those
fields select one source group through the shared change/reversion rules. A later
wind, script or supported-flag change selects its accompanying mesh reference too.
Scripts are selected as a complete list within this group, rather than merged
independently by name. The script writer retains the destination adapter’s version
and object format.

Model/bounds retain the [shared filename rule](README.md#properties-shared-across-records):
matching the winning filename keeps winning bounds; a different selected filename
carries bounds from its accepted source. Bounds-only changes do not take ownership
of the tree group. CNAM retains all twelve wind values together: TrunkFlexibility,
BranchFlexibility, LeafAmplitude, LeafFrequency and the eight values represented by
the 32-byte Unknown block. Supported header flags use their registered mask;
unregistered bits retain the winner’s state.

| Properties | How they are patched |
|---|---|
| `Name`, `Ingredient`, `HarvestSound` | Select each value independently. |
| `Production` | Select all four seasonal production values together. |

EditorID retains its shared setting. For example, if an earlier mod changes the
mesh/wind group and a later harvesting mod carries the original group, enabling
tree forwarding can preserve the earlier group alongside the harvesting edit.
Returning the group to its original value requires permission from its current owner.

The patcher does not inspect physical tree assets. Different mods can install
different files at the same path, so selecting a coherent plugin group cannot
guarantee that it matches the files installed by your mod manager.

Forwarding choices follow the [resource and appearance rules](assets.md).

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `TreeAssets` (Trees) | `ModelAndBounds`, `TreeData`, `VirtualMachineAdapter`, `MajorRecordFlagsRaw` | Tree option; off by default | Select one complete source snapshot through the shared change/reversion rules. |

<a id="watr-water"></a>

## WATR — Water

Each noise texture stays with its own UV scale. Flow normals stay with flowmap scale and the surface flags that enable their interpretation. Wind direction/speed, amplitude, falloff and other water behaviour forward normally, separately from the texture groups.

CausesDamage is separate from the surface-rendering flag mask. Zero clears retain destination nullable field presence; nonzero bits materialize it when needed. UnusedNoisemaps remains unregistered.

| Properties | How they are patched |
|---|---|
| `NoiseLayerOneWindDirection`, `NoiseLayerTwoWindDirection`, `NoiseLayerThreeWindDirection`, `NoiseLayerOneWindSpeed`, `NoiseLayerTwoWindSpeed`, `NoiseLayerThreeWindSpeed`, `NoiseLayerOneAmplitudeScale`, `NoiseLayerTwoAmplitudeScale`, `NoiseLayerThreeAmplitudeScale`, `NoiseFalloff` | Select each value separately. |
| `Name`, `Opacity`, `Material`, `OpenSound`, `Spell`, `ImageSpace`, `DamagePerSecond`, `SpecularSunPower`, `WaterReflectivity`, `WaterFresnel`, `FogAboveWaterDistanceNearPlane`, `FogAboveWaterDistanceFarPlane`, `ShallowColor`, `DeepColor`, `ReflectionColor`, `DisplacementStartingSize`, `DisplacementFoce`, `DisplacementVelocity`, `DisplacementFalloff`, `DisplacementDampner`, `FogAboveWaterAmount`, `FogUnderWaterAmount`, `FogUnderWaterDistanceNearPlane`, `FogUnderWaterDistanceFarPlane`, `WaterRefractionMagnitude`, `SpecularPower`, `SpecularRadius`, `SpecularBrightness`, `WaterReflectionMagnitude`, `SpecularSunSparkleMagnitude`, `SpecularSunSpecularMagnitude`, `DepthReflections`, `DepthRefraction`, `DepthNormals`, `DepthSpecularLighting`, `SpecularSunSparklePower`, `LinearVelocity`, `AngularVelocity` | Select each value separately. |
| `Flags` | Select CausesDamage only; retain other and unknown bits. |
| `MNAM`, `GNAM` | Select each whole value separately. |
| `Unknown`, `Unknown2`, `Unknown3`, `Unknown4`, `Unknown5`, `Unknown6`, `Unknown7`, `UnusedNoisemaps` | Not independently forwarded. |
| `DNAMDataTypeState` | Serialization/unused state; no independent decision. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `NoiseLayerOne` (Textures) | `NoiseLayerOneTexture`, `NoiseLayerOneUvScale` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `NoiseLayerTwo` (Textures) | `NoiseLayerTwoTexture`, `NoiseLayerTwoUvScale` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `NoiseLayerThree` (Textures) | `NoiseLayerThreeTexture`, `NoiseLayerThreeUvScale` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `FlowTexture` (Textures) | `FlowNormalsNoiseTexture`, `NoiseFlowmapScale`, `Flags.Surface` | Normal | Select one complete source snapshot through the shared change/reversion rules. |

`Flags` owns CausesDamage. Zero clears retain destination nullable field presence; nonzero selected bits materialize it when needed. `Flags.Surface` owns only EnableFlowmap and BlendNormals. Their approved masked setters preserve each other and unknown bits. DamagePerSecond and Spell remain independently patched.


<a id="wrld-worldspace"></a>

## WRLD — Worldspace

WaterNoiseTexture and WaterEnvironmentMap forward as a texture group; Water selection remains independent.

LodData groups LodWater and LodWaterHeight; a water link without a height is rejected. WorldMapOffset groups scale and cell offset. Missing DistantLodMultiplier means 1.0. Child records are handled separately, not as whole worldspace collections.

**Ownership reset:** a change to `Parent`, `Climate`, `Water`, `LodData`, `LandDefaults`, `MapData`, `Flags`, `FixedDimensionsCenterCell` resets all registered properties to that override before processing later overrides. Unregistered fields still start from the final winner.

| Properties | How they are patched |
|---|---|
| `Water` | Select the record reference within the structural ownership baseline. |
| `Name`, `Location`, `Music`, `DistantLodMultiplier`, `FixedDimensionsCenterCell`, `InteriorLighting`, `EncounterZone`, `Climate` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `LodData`, `Parent`, `LandDefaults` | Select each whole value separately. |
| `CanopyShadow`, `MaxHeight`, `ObjectBoundsMax`, `ObjectBoundsMin` | Not independently forwarded. |
| `LargeReferences`, `OffsetData`, `SubCells`, `SubCellsTimestamp`, `SubCellsUnknown`, `TopCell` | Child/group surfaces; no independent property merge. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `CloudModel` (Meshes) | `CloudModel` | Normal | Use the existing complete-value or collection policy. |
| `WaterDisplay` (Textures) | `WaterNoiseTexture`, `WaterEnvironmentMap` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `DistantTextures` (Textures) | `HdLodDiffuseTexture`, `HdLodNormalTexture` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
| `MapDisplay` (Textures) | `MapImage`, `MapData`, `WorldMapOffset` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="wthr-weather"></a>

## WTHR — Weather

CloudTextures and Clouds select together in CloudLayers, preserving their indexed relationships. Each named color structure, ImageSpaces, VolumetricLighting and DirectionalAmbientLightingColors keeps its time-of-day components together. These references select identities; the referenced [VOLI](visuals.md#voli-volumetriclighting) and [LENS](visuals.md#lens-lensflare) records have separate enabled processing routes.

| Properties | How they are patched |
|---|---|
| `Precipitation`, `VisualEffect`, `FogDistanceDayNear`, `FogDistanceDayFar`, `FogDistanceNightNear`, `FogDistanceNightFar`, `FogDistanceDayPower`, `FogDistanceNightPower`, `FogDistanceDayMax`, `FogDistanceNightMax`, `WindSpeed`, `TransDelta`, `SunGlare`, `SunDamage`, `PrecipitationBeginFadeIn`, `PrecipitationEndFadeOut`, `ThunderLightningBeginFadeIn`, `ThunderLightningEndFadeOut`, `ThunderLightningFrequency`, `LightningColor`, `VisualEffectBegin`, `VisualEffectEnd`, `WindDirection`, `WindDirectionRange`, `SunGlareLensFlare` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `DNAM`, `CNAM`, `ANAM`, `BNAM`, `LNAM`, `ONAM`, `SkyUpperColor`, `FogNearColor`, `UnknownColor`, `AmbientColor`, `SunlightColor`, `SunColor`, `StarsColor`, `SkyLowerColor`, `HorizonColor`, `EffectLightingColor`, `CloudLodDiffuseColor`, `CloudLodAmbientColor`, `FogFarColor`, `SkyStaticsColor`, `WaterMultiplierColor`, `SunGlareColor`, `MoonGlareColor`, `ImageSpaces`, `VolumetricLighting`, `DirectionalAmbientLightingColors`, `NAM2`, `NAM3` | Select each whole value separately. |
| `SkyStatics` | Merge rows by key. |
| `Sounds` | Merge rows by position. |
| `Unknown` | Not independently forwarded. |
| `NAM0DataTypeState` | Serialization/unused state; no independent decision. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `Aurora` (Meshes) | `Aurora` | Normal | Use the existing complete-value or collection policy. |
| `CloudLayers` (Textures) | `CloudTextures`, `Clouds` | Normal | Select one complete source snapshot through the shared change/reversion rules. |
