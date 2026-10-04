# Placed records

[Patching guide](README.md) · [Known issues](KNOWN-ISSUES.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated.

<a id="achr-placednpc"></a>

## ACHR — Placed Npc

Placement keeps position and rotation together at the project’s comparison precision. See [safe-disable coordination](README.md#placed-reference-coordination), which can select Placement and EnableParent from the InitiallyDisabled flag’s owner.

| Properties | How they are patched |
|---|---|
| `Base`, `EncounterZone`, `LevelModifier`, `MerchantContainer`, `Count`, `Radius`, `Health`, `PersistentLocation`, `LocationReference`, `IsIgnoredBySandbox`, `HeadTrackingWeight`, `Horse`, `FavorCost`, `Owner`, `FactionRank`, `Emittance`, `MultiBoundReference`, `IsIgnoredBySandbox2`, `Scale` | Select each value separately. |
| `MajorFlags` | Merge registered flag bits separately. |
| `RagdollData`, `RagdollBipedData`, `Patrol`, `ActivateParents`, `LinkedReferenceColor`, `EnableParent`, `Placement` | Select each whole value separately. |
| `LinkedReferences`, `VirtualMachineAdapter` | Merge rows by key. |
| `LocationRefTypes` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/PlacedNpcRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="phzd-placedhazard"></a>

## PHZD — Placed Hazard

Discovered through the placed-trap query, then narrowed to hazards. Only Hazard and the shared EditorID/header fields are registered. Inherited placement, scripts and reference data are not independently patched; the reason is uncertain. MajorFlags has no separate typed handler, though overlapping header bits can change. Safe-disable coordination does not run for PHZD.

| Properties | How they are patched |
|---|---|
| `Hazard` | Select each value separately. |
| `VirtualMachineAdapter`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Reflections`, `LinkedReferences`, `ActivateParents`, `EnableParent`, `Emittance`, `MultiBoundReference`, `IgnoredBySandbox`, `LocationRefTypes`, `LocationReference`, `DistantLodData`, `Scale`, `Placement` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/PlacedHazardRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="refr-placedobject"></a>

## REFR — Placed Object

Placement keeps position and rotation together at the project’s comparison precision. See [safe-disable coordination](README.md#placed-reference-coordination), which can select Placement and EnableParent from the InitiallyDisabled flag’s owner.

| Properties | How they are patched |
|---|---|
| `Base`, `Owner`, `Scale`, `LocationReference`, `ImageSpace`, `LightingTemplate`, `BoundHalfExtents`, `Radius`, `Emittance`, `TeleportMessageBox`, `MultiBoundReference`, `SpawnContainer`, `LeveledItemBaseObject`, `PersistentLocation`, `EncounterZone`, `IsMultiBoundPrimitive`, `IsIgnoredBySandbox`, `IsOpenByDefault`, `FactionRank`, `ItemCount`, `Charge`, `HeadTrackingWeight`, `FavorCost`, `CollisionLayer`, `LevelModifier`, `AttachRef`, `XCZR`, `XCZC` | Select each value separately. |
| `Action` | Merge registered flag bits separately. |
| `Primitive`, `OcclusionPlane`, `RoomPortal`, `NavigationDoorLink`, `TeleportDestination`, `ActivateParents`, `Lock`, `LightData`, `Alpha`, `Patrol`, `MapMarker`, `Placement`, `EnableParent`, `WaterVelocity`, `XORD`, `RagdollData`, `RagdollBipedData`, `XWCN`, `XWCS`, `XCVL`, `XCZA`, `DistantLodData` | Select each whole value separately. |
| `Portals` | Select each whole collection separately. |
| `LinkedRooms`, `Reflections`, `LitWater`, `VirtualMachineAdapter` | Merge rows by key. |
| `LocationRefTypes` | Merge aligned rows in order. |
| `LinkedReferences` | Merge rows by position. |
| `Unknown` | Not independently forwarded. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/PlacedObjectRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).
