# Sound and music

[Patching guide](README.md) · [Known issues](KNOWN-ISSUES.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated.

<a id="aspc-acousticspace"></a>

## ASPC — Acoustic Space

| Properties | How they are patched |
|---|---|
| `AmbientSound`, `UseSoundFromRegion`, `EnvironmentType` | Select each value separately. |
| `ObjectBounds` | Select each whole value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/AcousticSpaceRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="fstp-footstep"></a>

## FSTP — Footstep

| Properties | How they are patched |
|---|---|
| `ImpactDataSet`, `Tag` | Select each value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/FootstepRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="fsts-footstepset"></a>

## FSTS — Footstep Set

| Properties | How they are patched |
|---|---|
| `WalkForwardFootsteps`, `RunForwardFootsteps`, `WalkForwardAlternateFootsteps`, `RunForwardAlternateFootsteps`, `WalkForwardAlternateFootsteps2` | Select each whole collection separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/FootstepSetRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="musc-musictype"></a>

## MUSC — Music Type

| Properties | How they are patched |
|---|---|
| `FadeDuration` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Data` | Select each whole value separately. |
| `Tracks` | Select each whole collection separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/MusicTypeRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="must-musictrack"></a>

## MUST — Music Track

| Properties | How they are patched |
|---|---|
| `Type`, `Duration`, `FadeOut`, `TrackFilename`, `FinaleFilename` | Select each value separately. |
| `LoopData` | Select each whole value separately. |
| `CuePoints`, `Tracks` | Select each whole collection separately. |
| `Conditions` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/MusicTrackRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="revb-reverbparameters"></a>

## REVB — Reverb Parameters

ReverbData groups DecayMilliseconds, HfReferenceHertz, RoomFilter, RoomHfFilter, Reflections, ReverbAmp, DecayHfRatio, ReflectDelayMS, ReverbDelayMS, DiffusionPercent, DensityPercent and Unknown.

| Properties | How they are patched |
|---|---|
| `ReverbData` | Select each whole value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/ReverbParametersRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="snct-soundcategory"></a>

## SNCT — Sound Category

| Properties | How they are patched |
|---|---|
| `Name`, `Parent`, `StaticVolumeMultiplier`, `DefaultMenuVolume` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/SoundCategoryRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="sndr-sounddescriptor"></a>

## SNDR — Sound Descriptor

Pitch groups PercentFrequencyShift and PercentFrequencyVariance. Volume groups Variance and StaticAttenuation. SoundFiles merges by position, without filename sorting or deduplication.

| Properties | How they are patched |
|---|---|
| `Type`, `Category`, `AlternateSoundFor`, `OutputModel`, `String`, `Priority` | Select each value separately. |
| `LoopAndRumble`, `Pitch`, `Volume` | Select each whole value separately. |
| `Conditions` | Merge aligned rows in order. |
| `SoundFiles` | Merge rows by position. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/SoundDescriptorRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="sopm-soundoutputmodel"></a>

## SOPM — Sound Output Model

Data, OutputChannels and Attenuation are separate whole structures, including their arrays/bytes. Channels do not merge individually.

| Properties | How they are patched |
|---|---|
| `Type` | Select each value separately. |
| `Data`, `FNAM`, `CNAM`, `SNAM`, `OutputChannels`, `Attenuation` | Select each whole value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/SoundOutputModelRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="soun-soundmarker"></a>

## SOUN — Sound Marker

| Properties | How they are patched |
|---|---|
| `SoundDescriptor` | Select each value separately. |
| `ObjectBounds`, `FNAM`, `SNDD` | Select each whole value separately. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/SoundMarkerRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).
