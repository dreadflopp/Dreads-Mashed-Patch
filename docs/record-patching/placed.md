# Placed records

[Record index](INDEX.md) · [Shared patching rules](README.md)

The [shared rules and table key](README.md#reading-the-property-tables) apply to every section. Each listed property is a separate decision unless the notes group it with other fields. Shared EditorID and record-header handling is not repeated. `MajorFlags` enum bits use the shared `MajorRecordFlagsRaw` handler; other flag fields keep their approved handlers.

<a id="achr-placednpc"></a>

## ACHR — Placed Npc

Transform keeps position, rotation and scale together at the project’s comparison precision, including while InitiallyDisabled is set. A later scale-only edit can replace an earlier movement edit. Transform, EnableParent and InitiallyDisabled have independent selection and reversion permissions under the [placed-reference rules](README.md#placed-reference-state-and-placement). EnableParent keeps its reference and flags together. Ownership and RagdollPose each select their paired fields together under the [shared rules](README.md#how-a-value-is-selected).

| Properties | How they are patched |
|---|---|
| `Transform` (`Placement`, `Scale`) | Select position, rotation and scale together, independently of InitiallyDisabled. |
| `Ownership` (`Owner`, `FactionRank`) | Select the owner and rank together, including removal. |
| `RagdollPose` (`RagdollData`, `RagdollBipedData`) | Select bones and biped rotation together. |
| `Base`, `EncounterZone`, `LevelModifier`, `MerchantContainer`, `Count`, `Radius`, `Health`, `PersistentLocation`, `LocationReference`, `IsIgnoredBySandbox`, `HeadTrackingWeight`, `Horse`, `FavorCost`, `Emittance`, `MultiBoundReference`, `IsIgnoredBySandbox2` | Select each value separately. |
| `Patrol`, `ActivateParents`, `LinkedReferenceColor`, `EnableParent` | Select each whole value separately. |
| `LinkedReferences` | Merge rows by key. |
| `LocationRefTypes` | Merge aligned rows in order. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |


<a id="parw-placedarrow"></a>

## PARW — Placed Arrow

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are covered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Transform selects position, rotation and scale together, using the same comparison precision as ACHR/REFR.

| Properties | How they are patched |
|---|---|
| `Transform` (`Placement`, `Scale`) | Select position, rotation and scale together. |
| `Ownership` (`Owner`, `FactionRank`) | Select the owner and rank together, including removal. |
| `Projectile`, `EncounterZone`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference` | Select each value separately. |
| `ActivateParents`, `EnableParent` | Select each whole value separately. |
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

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are covered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Transform selects position, rotation and scale together, using the same comparison precision as ACHR/REFR.

| Properties | How they are patched |
|---|---|
| `Transform` (`Placement`, `Scale`) | Select position, rotation and scale together. |
| `Ownership` (`Owner`, `FactionRank`) | Select the owner and rank together, including removal. |
| `Projectile`, `EncounterZone`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference` | Select each value separately. |
| `ActivateParents`, `EnableParent` | Select each whole value separately. |
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

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are covered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Transform selects position, rotation and scale together, using the same comparison precision as ACHR/REFR.

| Properties | How they are patched |
|---|---|
| `Transform` (`Placement`, `Scale`) | Select position, rotation and scale together. |
| `Ownership` (`Owner`, `FactionRank`) | Select the owner and rank together, including removal. |
| `Projectile`, `EncounterZone`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference` | Select each value separately. |
| `ActivateParents`, `EnableParent` | Select each whole value separately. |
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

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are covered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Transform selects position, rotation and scale together, using the same comparison precision as ACHR/REFR.

| Properties | How they are patched |
|---|---|
| `Transform` (`Placement`, `Scale`) | Select position, rotation and scale together. |
| `Ownership` (`Owner`, `FactionRank`) | Select the owner and rank together, including removal. |
| `Projectile`, `EncounterZone`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference` | Select each value separately. |
| `ActivateParents`, `EnableParent` | Select each whole value separately. |
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

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are covered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Transform selects position, rotation and scale together, using the same comparison precision as ACHR/REFR.

| Properties | How they are patched |
|---|---|
| `Transform` (`Placement`, `Scale`) | Select position, rotation and scale together. |
| `Ownership` (`Owner`, `FactionRank`) | Select the owner and rank together, including removal. |
| `Projectile`, `EncounterZone`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference` | Select each value separately. |
| `ActivateParents`, `EnableParent` | Select each whole value separately. |
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

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are covered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Transform selects position, rotation and scale together, using the same comparison precision as ACHR/REFR.

| Properties | How they are patched |
|---|---|
| `Transform` (`Placement`, `Scale`) | Select position, rotation and scale together. |
| `Ownership` (`Owner`, `FactionRank`) | Select the owner and rank together, including removal. |
| `Projectile`, `EncounterZone`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference` | Select each value separately. |
| `ActivateParents`, `EnableParent` | Select each whole value separately. |
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

Discovered through the shared placed-trap query, then narrowed to this concrete variant. All eighteen inherited placed fields are covered in addition to Projectile and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Transform selects position, rotation and scale together, using the same comparison precision as ACHR/REFR.

| Properties | How they are patched |
|---|---|
| `Transform` (`Placement`, `Scale`) | Select position, rotation and scale together. |
| `Ownership` (`Owner`, `FactionRank`) | Select the owner and rank together, including removal. |
| `Projectile`, `EncounterZone`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference` | Select each value separately. |
| `ActivateParents`, `EnableParent` | Select each whole value separately. |
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

Discovered through the placed-trap query, then narrowed to hazards. All eighteen inherited placed fields are covered in addition to Hazard and shared EditorID/header handling. `APlacedTrap.MajorFlag` bits retain the approved composite header handler. Transform selects position, rotation and scale together, using the same comparison precision as ACHR/REFR.

| Properties | How they are patched |
|---|---|
| `Transform` (`Placement`, `Scale`) | Select position, rotation and scale together. |
| `Ownership` (`Owner`, `FactionRank`) | Select the owner and rank together, including removal. |
| `Hazard`, `EncounterZone`, `HeadTrackingWeight`, `FavorCost`, `Emittance`, `MultiBoundReference`, `LocationReference` | Select each value separately. |
| `ActivateParents`, `EnableParent` | Select each whole value separately. |
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

Transform selects position, rotation and scale together at the project’s comparison precision, including while InitiallyDisabled is set. A later scale-only edit can replace an earlier movement edit. Transform, EnableParent and InitiallyDisabled have independent selection and reversion permissions under the [placed-reference rules](README.md#placed-reference-state-and-placement). EnableParent keeps its reference and flags together. Ownership keeps the owner and faction rank together; RagdollPose keeps the bones and biped rotation together. Each group uses the [shared selection rules](README.md#how-a-value-is-selected).

| Properties | How they are patched |
|---|---|
| `Base`, `LocationReference`, `ImageSpace`, `LightingTemplate`, `BoundHalfExtents`, `Radius`, `Emittance`, `TeleportMessageBox`, `MultiBoundReference`, `SpawnContainer`, `LeveledItemBaseObject`, `PersistentLocation`, `EncounterZone`, `IsMultiBoundPrimitive`, `IsIgnoredBySandbox`, `IsOpenByDefault`, `ItemCount`, `Charge`, `HeadTrackingWeight`, `FavorCost`, `CollisionLayer`, `LevelModifier`, `AttachRef`, `XCZR`, `XCZC` | Select each value separately. |
| `Transform` (`Placement`, `Scale`) | Select position, rotation and scale together, independently of InitiallyDisabled. |
| `Ownership` (`Owner`, `FactionRank`) | Select the owner and rank together, including removal. |
| `RagdollPose` (`RagdollData`, `RagdollBipedData`) | Select the complete pose, preserving each payload’s presence. |
| `Action` | Merge registered flag bits separately. |
| `Primitive`, `OcclusionPlane`, `RoomPortal`, `NavigationDoorLink`, `TeleportDestination`, `ActivateParents`, `Lock`, `LightData`, `Alpha`, `Patrol`, `MapMarker`, `EnableParent`, `WaterVelocity`, `XORD`, `XWCN`, `XWCS`, `XCVL`, `XCZA`, `DistantLodData` | Select each whole value separately. |
| `Portals` | Select each whole collection separately. |
| `LinkedRooms`, `Reflections`, `LitWater` | Merge rows by key. |
| `LocationRefTypes` | Merge aligned rows in order. |
| `LinkedReferences` | Merge rows by position. |
| `Unknown` | Not independently forwarded. |

Grouped properties follow the [shared forwarding behavior](assets.md). Each group has one decision; the other listed properties retain their separate rules.

| Property group | Properties kept together | Forwarding | Selection |
|---|---|---|---|
| `VirtualMachineAdapter` (Scripts) | `VirtualMachineAdapter` | Normal | Use the existing complete-value or collection policy. |
