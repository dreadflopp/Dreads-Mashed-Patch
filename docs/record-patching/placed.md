# Placed records

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="achr-placednpc"></a>

## ACHR — Placed Npc

Placement keeps position and rotation together at the project’s comparison precision. See [safe-disable coordination](README.md#placed-reference-coordination), which can select Placement and EnableParent from the InitiallyDisabled flag’s owner.

| Properties | How they are patched |
|---|---|
| `Base`, `EncounterZone`, `LevelModifier`, `MerchantContainer`, `Count`, `Radius`, `Health`, `PersistentLocation`, `LocationReference`, `IsIgnoredBySandbox`, `HeadTrackingWeight`, `Horse`, `FavorCost`, `Owner`, `FactionRank`, `Emittance`, `MultiBoundReference`, `IsIgnoredBySandbox2`, `Scale` | Select each value separately. |
| `RagdollData`, `RagdollBipedData`, `Patrol`, `ActivateParents`, `LinkedReferenceColor`, `EnableParent`, `Placement` | Select each whole value separately. |
| `LinkedReferences` | Merge rows by key. |
| `LocationRefTypes` | Merge aligned rows in order. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="parw-placedarrow"></a>

## PARW — Placed Arrow

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are registered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Placement uses the same atomic position/rotation comparison as ACHR/REFR; This variant does not use their UDR coordinator.

| Properties | How they are patched |
|---|---|
| `Projectile`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference`, `Scale` | Select each value separately. |
| `Placement`, `ActivateParents`, `EnableParent` | Select each whole value separately. |
| `Reflections`, `LinkedReferences` | Merge rows by key: Water and KeywordOrReference respectively. Each selected row carries its complete payload. These linked references follow ACHR’s sorted definition, rather than REFR’s positional list. |
| `LocationRefTypes` | Merge aligned rows in declaration order; null and present-empty remain distinct. |
| `DistantLodData` | Select the whole ordered XLOD float collection, representing one fixed three-float value. |
| `IgnoredBySandbox` | Select the complete nullable XIS2 byte slice/marker. Absence and present-empty are distinct; no boolean conversion. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="pbar-placedbarrier"></a>

## PBAR — Placed Barrier

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are registered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Placement uses the same atomic position/rotation comparison as ACHR/REFR; This variant does not use their UDR coordinator.

| Properties | How they are patched |
|---|---|
| `Projectile`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference`, `Scale` | Select each value separately. |
| `Placement`, `ActivateParents`, `EnableParent` | Select each whole value separately. |
| `Reflections`, `LinkedReferences` | Merge rows by key: Water and KeywordOrReference respectively. Each selected row carries its complete payload. These linked references follow ACHR’s sorted definition, rather than REFR’s positional list. |
| `LocationRefTypes` | Merge aligned rows in declaration order; null and present-empty remain distinct. |
| `DistantLodData` | Select the whole ordered XLOD float collection, representing one fixed three-float value. |
| `IgnoredBySandbox` | Select the complete nullable XIS2 byte slice/marker. Absence and present-empty are distinct; no boolean conversion. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="pbea-placedbeam"></a>

## PBEA — Placed Beam

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are registered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Placement uses the same atomic position/rotation comparison as ACHR/REFR; This variant does not use their UDR coordinator.

| Properties | How they are patched |
|---|---|
| `Projectile`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference`, `Scale` | Select each value separately. |
| `Placement`, `ActivateParents`, `EnableParent` | Select each whole value separately. |
| `Reflections`, `LinkedReferences` | Merge rows by key: Water and KeywordOrReference respectively. Each selected row carries its complete payload. These linked references follow ACHR’s sorted definition, rather than REFR’s positional list. |
| `LocationRefTypes` | Merge aligned rows in declaration order; null and present-empty remain distinct. |
| `DistantLodData` | Select the whole ordered XLOD float collection, representing one fixed three-float value. |
| `IgnoredBySandbox` | Select the complete nullable XIS2 byte slice/marker. Absence and present-empty are distinct; no boolean conversion. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="pcon-placedcone"></a>

## PCON — Placed Cone

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are registered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Placement uses the same atomic position/rotation comparison as ACHR/REFR; This variant does not use their UDR coordinator.

| Properties | How they are patched |
|---|---|
| `Projectile`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference`, `Scale` | Select each value separately. |
| `Placement`, `ActivateParents`, `EnableParent` | Select each whole value separately. |
| `Reflections`, `LinkedReferences` | Merge rows by key: Water and KeywordOrReference respectively. Each selected row carries its complete payload. These linked references follow ACHR’s sorted definition, rather than REFR’s positional list. |
| `LocationRefTypes` | Merge aligned rows in declaration order; null and present-empty remain distinct. |
| `DistantLodData` | Select the whole ordered XLOD float collection, representing one fixed three-float value. |
| `IgnoredBySandbox` | Select the complete nullable XIS2 byte slice/marker. Absence and present-empty are distinct; no boolean conversion. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="pfla-placedflame"></a>

## PFLA — Placed Flame

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are registered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Placement uses the same atomic position/rotation comparison as ACHR/REFR; This variant does not use their UDR coordinator.

| Properties | How they are patched |
|---|---|
| `Projectile`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference`, `Scale` | Select each value separately. |
| `Placement`, `ActivateParents`, `EnableParent` | Select each whole value separately. |
| `Reflections`, `LinkedReferences` | Merge rows by key: Water and KeywordOrReference respectively. Each selected row carries its complete payload. These linked references follow ACHR’s sorted definition, rather than REFR’s positional list. |
| `LocationRefTypes` | Merge aligned rows in declaration order; null and present-empty remain distinct. |
| `DistantLodData` | Select the whole ordered XLOD float collection, representing one fixed three-float value. |
| `IgnoredBySandbox` | Select the complete nullable XIS2 byte slice/marker. Absence and present-empty are distinct; no boolean conversion. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="pgre-placedtrap"></a>

## PGRE — Placed Trap

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are registered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Placement uses the same atomic position/rotation comparison as ACHR/REFR; This variant does not use their UDR coordinator.

| Properties | How they are patched |
|---|---|
| `Projectile`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference`, `Scale` | Select each value separately. |
| `Placement`, `ActivateParents`, `EnableParent` | Select each whole value separately. |
| `Reflections`, `LinkedReferences` | Merge rows by key: Water and KeywordOrReference respectively. Each selected row carries its complete payload. These linked references follow ACHR’s sorted definition, rather than REFR’s positional list. |
| `LocationRefTypes` | Merge aligned rows in declaration order; null and present-empty remain distinct. |
| `DistantLodData` | Select the whole ordered XLOD float collection, representing one fixed three-float value. |
| `IgnoredBySandbox` | Select the complete nullable XIS2 byte slice/marker. Absence and present-empty are distinct; no boolean conversion. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="pmis-placedmissile"></a>

## PMIS — Placed Missile

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are registered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Placement uses the same atomic position/rotation comparison as ACHR/REFR; This variant does not use their UDR coordinator.

| Properties | How they are patched |
|---|---|
| `Projectile`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference`, `Scale` | Select each value separately. |
| `Placement`, `ActivateParents`, `EnableParent` | Select each whole value separately. |
| `Reflections`, `LinkedReferences` | Merge rows by key: Water and KeywordOrReference respectively. Each selected row carries its complete payload. These linked references follow ACHR’s sorted definition, rather than REFR’s positional list. |
| `LocationRefTypes` | Merge aligned rows in declaration order; null and present-empty remain distinct. |
| `DistantLodData` | Select the whole ordered XLOD float collection, representing one fixed three-float value. |
| `IgnoredBySandbox` | Select the complete nullable XIS2 byte slice/marker. Absence and present-empty are distinct; no boolean conversion. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="phzd-placedhazard"></a>

## PHZD — Placed Hazard

Discovered through the placed-trap query, then narrowed to hazards. All eighteen inherited placed fields are registered in addition to Hazard and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Placement uses the same atomic position/rotation comparison as ACHR/REFR; PHZD does not use their UDR coordinator.

| Properties | How they are patched |
|---|---|
| `Hazard`, `EncounterZone`, `Owner`, `FactionRank`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference`, `Scale` | Select each value separately. |
| `Placement`, `ActivateParents`, `EnableParent` | Select each whole value separately. |
| `Reflections`, `LinkedReferences` | Merge rows by key: Water and KeywordOrReference respectively. Each selected row carries its complete payload. PHZD linked references follow ACHR’s sorted definition, rather than REFR’s positional list. |
| `LocationRefTypes` | Merge aligned rows in declaration order; null and present-empty remain distinct. |
| `DistantLodData` | Select the whole ordered XLOD float collection, representing one fixed three-float value. |
| `IgnoredBySandbox` | Select the complete nullable XIS2 byte slice/marker. Absence and present-empty are distinct; no boolean conversion. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="refr-placedobject"></a>

## REFR — Placed Object

Placement keeps position and rotation together at the project’s comparison precision. See [safe-disable coordination](README.md#placed-reference-coordination), which can select Placement and EnableParent from the InitiallyDisabled flag’s owner.

| Properties | How they are patched |
|---|---|
| `Base`, `Owner`, `Scale`, `LocationReference`, `ImageSpace`, `LightingTemplate`, `BoundHalfExtents`, `Radius`, `Emittance`, `TeleportMessageBox`, `MultiBoundReference`, `SpawnContainer`, `LeveledItemBaseObject`, `PersistentLocation`, `EncounterZone`, `IsMultiBoundPrimitive`, `IsIgnoredBySandbox`, `IsOpenByDefault`, `FactionRank`, `ItemCount`, `Charge`, `HeadTrackingWeight`, `FavorCost`, `CollisionLayer`, `LevelModifier`, `AttachRef`, `XCZR`, `XCZC` | Select each value separately. |
| `Action` | Merge registered flag bits separately. |
| `Primitive`, `OcclusionPlane`, `RoomPortal`, `NavigationDoorLink`, `TeleportDestination`, `ActivateParents`, `Lock`, `LightData`, `Alpha`, `Patrol`, `MapMarker`, `Placement`, `EnableParent`, `WaterVelocity`, `XORD`, `RagdollData`, `RagdollBipedData`, `XWCN`, `XWCS`, `XCVL`, `XCZA`, `DistantLodData` | Select each whole value separately. |
| `Portals` | Select each whole collection separately. |
| `LinkedRooms`, `Reflections`, `LitWater` | Merge rows by key. |
| `LocationRefTypes` | Merge aligned rows in order. |
| `LinkedReferences` | Merge rows by position. |
| `Unknown` | Not independently forwarded. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
