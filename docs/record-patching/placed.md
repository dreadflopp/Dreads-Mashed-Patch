# Placed records

[Patching guide](README.md) · [Known issues](KNOWN-ISSUES.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="achr-placednpc"></a>

## ACHR — Placed Npc

Placement keeps position and rotation together at the project’s comparison precision. See [safe-disable coordination](README.md#placed-reference-coordination), which can select Placement and EnableParent from the InitiallyDisabled flag’s owner.

| Properties | How they are patched |
|---|---|
| `Base`, `EncounterZone`, `LevelModifier`, `MerchantContainer`, `Count`, `Radius`, `Health`, `PersistentLocation`, `LocationReference`, `IsIgnoredBySandbox`, `HeadTrackingWeight`, `Horse`, `FavorCost`, `Owner`, `FactionRank`, `Emittance`, `MultiBoundReference`, `IsIgnoredBySandbox2`, `Scale` | Select each value separately. |

| `RagdollData`, `RagdollBipedData`, `Patrol`, `ActivateParents`, `LinkedReferenceColor`, `EnableParent`, `Placement` | Select each whole value separately. |
| `LinkedReferences`, `VirtualMachineAdapter` | Merge rows by key. |
| `LocationRefTypes` | Merge aligned rows in order. |

[Record registration and source-record lookup](../../DreadsMashedPatch/RecordHandlers/PlacedNpcRecordHandler.cs); selection and final output follow the [shared processing path](README.md#which-records-reach-the-patch).

<a id="phzd-placedhazard"></a>

## PHZD — Placed Hazard

Discovered through the placed-trap query, then narrowed to hazards. All eighteen inherited placed fields are registered in addition to Hazard and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Placement uses the same atomic position/rotation comparison as ACHR/REFR; PHZD does not use their UDR coordinator. See the [migration evidence](REVIEW.md#coverage-fix-verification).

| Properties | How they are patched |
|---|---|
| `Hazard`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference`, `Scale` | Select each value separately. |
| `Placement`, `ActivateParents`, `EnableParent` | Select each whole value separately. |
| `VirtualMachineAdapter` | Merge scripts by name; retain destination Version/ObjectFormat using the shared ordinary VMAD policy. |
| `Reflections`, `LinkedReferences` | Merge rows by key: Water and KeywordOrReference respectively. Each selected row carries its complete payload. PHZD linked references follow ACHR’s sorted definition, rather than REFR’s positional list. |
| `LocationRefTypes` | Merge aligned rows in declaration order; null and present-empty remain distinct. |
| `DistantLodData` | Select the whole ordered XLOD float collection, representing one fixed three-float value. |
| `IgnoredBySandbox` | Select the complete nullable XIS2 byte slice/marker. Absence and present-empty are distinct; no boolean conversion. |

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
