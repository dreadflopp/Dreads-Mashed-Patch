# Collection semantics comparison

This report compares the generated runtime inventory with the independently maintained pinned-xEdit expectations.

- Runtime registrations: 168
- xEdit expectations: 168
- Matched: 168
- Unresolved: 0
- Mismatched: 0
- Missing/invalid: 0

| Registration | Result | Runtime | Expected | Note |
| --- | --- | --- | --- | --- |
| `ActivatorRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ActivatorRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ActorValueInformationRecordHandler.PerkTree` | Matched | Atomic | Atomic | Per-entry ownership could combine nodes and positional connection indexes from different source graphs. |
| `AlchemicalApparatusRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `AmmunitionRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ArmorAddonRecordHandler.AdditionalRaces` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ArmorAddonRecordHandler.FirstPersonModel.Female.AlternateTextures` | Matched | SortedKeyed | SortedKeyed | xEdit keys alternate textures by 3D Name and 3D Index; New Texture is replaceable row data. |
| `ArmorAddonRecordHandler.FirstPersonModel.Male.AlternateTextures` | Matched | SortedKeyed | SortedKeyed | xEdit keys alternate textures by 3D Name and 3D Index; New Texture is replaceable row data. |
| `ArmorAddonRecordHandler.WorldModel.Female.AlternateTextures` | Matched | SortedKeyed | SortedKeyed | xEdit keys alternate textures by 3D Name and 3D Index; New Texture is replaceable row data. |
| `ArmorAddonRecordHandler.WorldModel.Male.AlternateTextures` | Matched | SortedKeyed | SortedKeyed | xEdit keys alternate textures by 3D Name and 3D Index; New Texture is replaceable row data. |
| `ArmorRecordHandler.Armature` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `ArmorRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ArmorRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `BodyPartDataRecordHandler.Parts` | Matched | SortedKeyed | SortedKeyed | xEdit sorts body-part rows by PartNode; changes to other fields replace data under that key. |
| `BookRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `BookRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `CameraPathRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `CameraPathRecordHandler.RelatedPaths` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `CameraPathRecordHandler.Shots` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `CellRecordHandler.Regions` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ClassRecordHandler.SkillWeights` | Matched | Atomic | Atomic | Mutagen exposes named dictionary views over fixed positional fields in one DATA structure. |
| `ClassRecordHandler.StatWeights` | Matched | Atomic | Atomic | Mutagen exposes named dictionary views over fixed positional fields in one DATA structure. |
| `ClimateRecordHandler.WeatherTypes` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `CollisionLayerRecordHandler.CollidesWith` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ConstructibleObjectRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ConstructibleObjectRecordHandler.Items` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ContainerRecordHandler.Items` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ContainerRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `DebrisRecordHandler.Models` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `DefaultObjectManagerRecordHandler.Objects` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `DialogResponseRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `DialogResponseRecordHandler.LinkTo` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `DialogResponseRecordHandler.Responses` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `DialogViewRecordHandler.Branches` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `DialogViewRecordHandler.TNAMs` | Matched | Atomic | Atomic | Without typed topic FormIDs, raw payloads cannot be safely aligned or independently owned. |
| `DoorRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `EquipTypeRecordHandler.SlotParents` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ExplosionRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `FactionRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `FactionRecordHandler.Ranks` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `FactionRecordHandler.Relations` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `FloraRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `FloraRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `FootstepSetRecordHandler.RunForwardAlternateFootsteps` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `FootstepSetRecordHandler.RunForwardFootsteps` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `FootstepSetRecordHandler.WalkForwardAlternateFootsteps` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `FootstepSetRecordHandler.WalkForwardAlternateFootsteps2` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `FootstepSetRecordHandler.WalkForwardFootsteps` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `FormIdRecordHandler.Items` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `FurnitureRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `FurnitureRecordHandler.Markers` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `FurnitureRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `HeadPartRecordHandler.ExtraParts` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `HeadPartRecordHandler.Parts` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `IdleAnimationRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `IdleMarkerRecordHandler.Animations` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `ImageSpaceAdapterRecordHandler.BlurRadius` | Matched | Atomic | Atomic | Whole-curve ownership avoids synthesizing an interpolation curve that no source plugin authored. |
| `ImageSpaceAdapterRecordHandler.DoubleVisionStrength` | Matched | Atomic | Atomic | Whole-curve ownership avoids synthesizing an interpolation curve that no source plugin authored. |
| `ImageSpaceAdapterRecordHandler.FadeColor` | Matched | Atomic | Atomic | Whole-curve ownership avoids synthesizing an interpolation curve that no source plugin authored. |
| `ImageSpaceAdapterRecordHandler.MotionBlurStrength` | Matched | Atomic | Atomic | Whole-curve ownership avoids synthesizing an interpolation curve that no source plugin authored. |
| `ImageSpaceAdapterRecordHandler.TintColor` | Matched | Atomic | Atomic | Whole-curve ownership avoids synthesizing an interpolation curve that no source plugin authored. |
| `ImpactDataSetRecordHandler.Impacts` | Matched | Specialized | Specialized | The dedicated handler merges the xEdit material-keyed mapping and falls back safely for invalid duplicate/null keys. |
| `IngestibleRecordHandler.Effects` | Matched | ExactOrdered | ExactOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `IngestibleRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `IngredientRecordHandler.Effects` | Matched | ExactOrdered | ExactOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `IngredientRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `IngredientRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `KeyRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `KeyRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `LandscapeRecordHandler.Layers` | Matched | Unordered | Unordered | Known LAND exception: xEdit identity is union/context dependent and is intentionally not generalized. |
| `LandscapeRecordHandler.Textures` | Matched | Unordered | Unordered | Known LAND exception: xEdit identity is union/context dependent and is intentionally not generalized. |
| `LandscapeRecordHandler.VertexColors` | Matched | Atomic | Atomic | Independent vertex ownership can create incoherent terrain geometry or shading. |
| `LandscapeRecordHandler.VertexNormals` | Matched | Atomic | Atomic | Independent vertex ownership can create incoherent terrain geometry or shading. |
| `LandscapeTextureRecordHandler.Grasses` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `LeveledItemRecordHandler.Entries` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `LeveledNpcRecordHandler.Entries` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `LeveledSpellRecordHandler.Entries` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `LightRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `LoadScreenRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `LocationRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `MagicEffectRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `MagicEffectRecordHandler.CounterEffects` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `MagicEffectRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `MagicEffectRecordHandler.Sounds` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `MagicEffectRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `MaterialObjectRecordHandler.DNAMs` | Matched | Atomic | Atomic | xEdit deliberately hides and ignores the opaque payload; it has no safe semantic row identity. |
| `MessageRecordHandler.MenuButtons` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `MiscItemRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `MiscItemRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `MusicTrackRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `MusicTrackRecordHandler.CuePoints` | Matched | Atomic | Atomic | Direct review confirms a fixed, positional, opaque, geometric, graph, curve, or count-coupled structure; whole-value ownership prevents invalid synthetic combinations. |
| `MusicTrackRecordHandler.Tracks` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `MusicTypeRecordHandler.Tracks` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `NpcRecordHandler.ActorEffect` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `NpcRecordHandler.Attacks` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `NpcRecordHandler.Factions` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `NpcRecordHandler.HeadParts` | Matched | SortedKeyed | SortedKeyed | NPC head parts are an xEdit sorted scalar FormID collection. |
| `NpcRecordHandler.Items` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `NpcRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `NpcRecordHandler.Packages` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `NpcRecordHandler.Perks` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `NpcRecordHandler.PlayerSkills.SkillOffsets` | Matched | Atomic | Atomic | Mutagen dictionaries project fixed skill slots rather than independently declared collection rows. |
| `NpcRecordHandler.PlayerSkills.SkillValues` | Matched | Atomic | Atomic | Mutagen dictionaries project fixed skill slots rather than independently declared collection rows. |
| `NpcRecordHandler.TintLayers` | Matched | SortedKeyed | SortedKeyed | Tint Index identifies the row; color, interpolation, and preset are replaceable data. |
| `NpcRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ObjectEffectRecordHandler.Effects` | Matched | ExactOrdered | ExactOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `OutfitRecordHandler.Items` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PackageRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PerkRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PerkRecordHandler.Effects` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PlacedNpcRecordHandler.LinkedReferences` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PlacedNpcRecordHandler.LocationRefTypes` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PlacedNpcRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PlacedObjectRecordHandler.LinkedReferences` | Matched | ExactOrdered | ExactOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PlacedObjectRecordHandler.LinkedRooms` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PlacedObjectRecordHandler.LitWater` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PlacedObjectRecordHandler.LocationRefTypes` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PlacedObjectRecordHandler.Portals` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `PlacedObjectRecordHandler.Reflections` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `PlacedObjectRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `QuestRecordHandler.Aliases` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `QuestRecordHandler.DialogConditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `QuestRecordHandler.EventConditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `QuestRecordHandler.Objectives` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `QuestRecordHandler.Stages` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `QuestRecordHandler.TextDisplayGlobals` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `QuestRecordHandler.VirtualMachineAdapter.Aliases` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `QuestRecordHandler.VirtualMachineAdapter.Fragments` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `QuestRecordHandler.VirtualMachineAdapter.Scripts` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `RaceRecordHandler.ActorEffect` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `RaceRecordHandler.Attacks` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `RaceRecordHandler.BipedObjectNames` | Matched | Atomic | Atomic | Mutagen dictionary/list projections represent fixed enum or positional fields, not independently keyed rows. |
| `RaceRecordHandler.EquipmentSlots` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `RaceRecordHandler.Eyes` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `RaceRecordHandler.Hairs` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `RaceRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `RaceRecordHandler.MovementTypeNames` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `RaceRecordHandler.MovementTypes` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `RaceRecordHandler.Regen` | Matched | Atomic | Atomic | Mutagen dictionary/list projections represent fixed enum or positional fields, not independently keyed rows. |
| `RaceRecordHandler.SkillBoosts` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `RaceRecordHandler.Starting` | Matched | Atomic | Atomic | Mutagen dictionary/list projections represent fixed enum or positional fields, not independently keyed rows. |
| `RegionRecordHandler.RegionAreas` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `SceneRecordHandler.Actions` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `SceneRecordHandler.Actors` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `SceneRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `SceneRecordHandler.Phases` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `SceneRecordHandler.VirtualMachineAdapter.Scripts` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ScrollRecordHandler.Effects` | Matched | ExactOrdered | ExactOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ScrollRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `ShoutRecordHandler.WordsOfPower` | Matched | Atomic | Atomic | The pinned list audit requires whole-value ownership because independently merging rows can produce invalid structural or behavioral combinations. |
| `SoulGemRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `SoundDescriptorRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `SoundDescriptorRecordHandler.SoundFiles` | Matched | ExactOrdered | ExactOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `SpellRecordHandler.Effects` | Matched | ExactOrdered | ExactOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `SpellRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `StoryManagerBranchNodeRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `StoryManagerEventNodeRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `StoryManagerQuestNodeRecordHandler.Conditions` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `StoryManagerQuestNodeRecordHandler.Quests` | Matched | AlignedOrdered | AlignedOrdered | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `TalkingActivatorRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `TalkingActivatorRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `TreeRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `WeaponRecordHandler.Keywords` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `WeaponRecordHandler.VirtualMachineAdapter` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `WeatherRecordHandler.CloudTextures` | Matched | Atomic | Atomic | Cloud data is reconstructed from several fixed parallel arrays; whole-value ownership prevents cross-layer desynchronization. |
| `WeatherRecordHandler.Clouds` | Matched | Atomic | Atomic | Cloud data is reconstructed from several fixed parallel arrays; whole-value ownership prevents cross-layer desynchronization. |
| `WeatherRecordHandler.SkyStatics` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
| `WeatherRecordHandler.Sounds` | Matched | SortedKeyed | SortedKeyed | Expected mode is the reviewed result recorded by the pinned xEdit list-ordering audit and migration. |
