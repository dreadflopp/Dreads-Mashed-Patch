# Sound and music

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="aspc-acousticspace"></a>

## ASPC — Acoustic Space

| Properties | How they are patched |
|---|---|
| `AmbientSound`, `UseSoundFromRegion`, `EnvironmentType` | Select each value separately. |
| `ObjectBounds` | Keep the effective winner's bounds; no independent forwarding. |


<a id="fstp-footstep"></a>

## FSTP — Footstep

| Properties | How they are patched |
|---|---|
| `ImpactDataSet`, `Tag` | Select each value separately. |


<a id="fsts-footstepset"></a>

## FSTS — Footstep Set

| Properties | How they are patched |
|---|---|
| `WalkForwardFootsteps`, `RunForwardFootsteps`, `WalkForwardAlternateFootsteps`, `RunForwardAlternateFootsteps`, `WalkForwardAlternateFootsteps2` | Select each whole collection separately. |


<a id="musc-musictype"></a>

## MUSC — Music Type

| Properties | How they are patched |
|---|---|
| `FadeDuration` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |
| `Data` | Select each whole value separately. |
| `Tracks` | Select each whole collection separately. |


<a id="must-musictrack"></a>

## MUST — Music Track

| Properties | How they are patched |
|---|---|
| `Conditions` | Merge aligned rows in order. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `MusicPlayback` (Music) | `Type`, `Duration`, `FadeOut`, `TrackFilename`, `FinaleFilename`, `LoopData`, `CuePoints`, `Tracks` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="revb-reverbparameters"></a>

## REVB — Reverb Parameters

ReverbData groups DecayMilliseconds, HfReferenceHertz, RoomFilter, RoomHfFilter, Reflections, ReverbAmp, DecayHfRatio, ReflectDelayMS, ReverbDelayMS, DiffusionPercent, DensityPercent and Unknown.

| Properties | How they are patched |
|---|---|
| `ReverbData` | Select each whole value separately. |


<a id="snct-soundcategory"></a>

## SNCT — Sound Category

| Properties | How they are patched |
|---|---|
| `Name`, `Parent`, `StaticVolumeMultiplier`, `DefaultMenuVolume` | Select each value separately. |
| `Flags` | Merge registered flag bits separately. |


<a id="sndr-sounddescriptor"></a>

## SNDR — Sound Descriptor

SoundPlayback keeps clip selection, descriptor type and looping together. Pitch, volume, routing, category, alternate descriptor, subtitle String, priority and conditions retain their ordinary decisions.

Pitch groups PercentFrequencyShift and PercentFrequencyVariance. Volume groups Variance and StaticAttenuation. SoundFiles merges by position, without filename sorting or deduplication.

| Properties | How they are patched |
|---|---|
| `Category`, `AlternateSoundFor`, `OutputModel`, `String` | Select each value separately. |
| `Pitch`, `Volume` | Select each complete parameter pair separately. |
| `Priority` | Select each value separately. |
| `Conditions` | Merge aligned rows in order. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `SoundPlayback` (Sounds) | `Type`, `SoundFiles`, `LoopAndRumble` | Normal | Select one complete source snapshot through the shared change/reversion rules. |


<a id="sopm-soundoutputmodel"></a>

## SOPM — Sound Output Model

Data, OutputChannels and Attenuation are separate whole structures, including their arrays/bytes. Channels do not merge individually.

| Properties | How they are patched |
|---|---|
| `Type` | Select each value separately. |
| `Data`, `FNAM`, `CNAM`, `SNAM`, `OutputChannels`, `Attenuation` | Select each whole value separately. |


<a id="soun-soundmarker"></a>

## SOUN — Sound Marker

| Properties | How they are patched |
|---|---|
| `SoundDescriptor` | Select each value separately. |
| `FNAM`, `SNDD` | Select each whole value separately. |
| `ObjectBounds` | Keep the effective winner's bounds; no independent forwarding. |
