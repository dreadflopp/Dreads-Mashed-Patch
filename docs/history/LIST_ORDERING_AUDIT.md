# Pinned xEdit ordering evidence

[Internal references](README.md) · [Collection workflow](../COLLECTION_SEMANTICS.md) · [Alignment](../LIST_ALIGNMENT.md)

This reference supports the provenance strings in xedit-collection-expectations.json.
It retains source definitions and interpretation traps; completed migration steps
and earlier handler classifications have been removed. Current registrations and
explicit exceptions are in the [collection decisions](LIST_ORDERING_MIGRATION.md)
and [inventory](../record-patching/INVENTORY.md).

## Source

TES5Edit dev-4.1.6, commit `93cc0bc5a1251936c3c7859eee3150eda12a62d7`.
Regenerate ignored XEditSource with the [reference exporter](../REFERENCE_SOURCES.md).
Do not edit the checkout or assume these line numbers apply to another revision.

- Core/wbInterface.pas:8443 describes sorted arrays.
- Core/wbDefinitionsTES5.pas:1157 disables FormList sorting.
- Core/wbDefinitionsCommon.pas:8896 defines Magic Effect sounds.
- Other Skyrim definitions below are in Core/wbDefinitionsTES5.pas.
- [Alignment/diff source locations](../LIST_ALIGNMENT.md#how-xedit-aligns-arrays)
  identify the progressive row and Myers implementations.

## Interpreting array definitions

- wbArrayS/wbRArrayS sort by scalar or StructSK/RStructSK keys; declaration order
  does not establish row identity.
- wbArray/wbRArray preserve declaration order. Alignable definitions progressively
  align plugin columns by element key and add display gaps.
- dfNotAlignable disables row alignment; it does not make sequence order irrelevant.
- A fixed wbStruct projected as IReadOnlyList is positional structure, not an
  independently mergeable list.
- Definition callbacks can override apparent shape. FLST.LNAM uses wbRArrayS,
  but wbFLSTLNAMIsSorted always returns false.
- A nested key is not necessarily the outer array's row key. Magic-item Effects
  have nested EFIT sort data but no outer effect-row key.
- Full row equality must not substitute for a defined key; otherwise a data edit
  is misclassified as removal plus addition.

## Definitions requiring care

| Property | Definition or identity evidence | Consequence |
|---|---|---|
| Conditions | CTDA function, parameters 1/2, CIS1, CIS2 | Align by those keys; operator/value/run-on/reference/parameter 3 remain row content. |
| FLST Items | Effective unsorted LNAM despite ArrayS constructor | Preserve aligned declaration order. |
| NPC Packages / placed LocationRefTypes | wbRArray/wbArray with FormID identity | Align ordered occurrences. |
| ACHR LinkedReferences | wbRArrayS with wbStructSK([0]), Keyword/Ref key | Sorted/keyed; referenced target is row data. |
| REFR LinkedReferences | wbRArray with plain wbStruct(XLKR), no StructSK | Exact ordinal identity. It intentionally differs from ACHR. |
| Magic-item Effects | Outer wbRArray/wbRStruct lacks key | Exact position; each generated Effect value is atomic. |
| SNDR SoundFiles | wbRArray of ANAM strings without key | Exact numbered slots; preserve GivenPath. |
| CameraPath Shots / DialogResponse LinkTo / DialogView Branches / EquipType SlotParents / Quest TextDisplayGlobals | Declaration-order rows with referenced FormID identity | Aligned rows with occurrence tracking. |
| Quest Aliases / stages / objectives | Alias ID / stage index / objective index | Keep reviewed specialized/keyed semantics; do not infer them from CLR list type. |
| StoryManagerQuestNode Quests | wbRArray, wbRStructSK([0]), Quest FormID | Ordered alignment by Quest, with complete quest-row data. |
| DialogResponse Responses / Message MenuButtons | No explicit outer StructSK | Project-specific normalized response/button identities remain deliberate exceptions, not claims of exact xEdit keys. |
| HDPT Parts / ExtraParts | Ordered morph-file pairs / sorted extra-part FormIDs | Both are nested in atomic Appearance. Pair order matters; extra-part sorting alone does not. |
| NPC HeadParts / TintLayers | Sorted HeadPart FormID / Tint Index definitions | Nested atomic Appearance contents compare without sorting claiming ownership; retain duplicates and full tint payload. |
| ARMA alternate textures | Composite (3D Name, 3D Index) | Keyed replacement; preserve other row data. |
| BPTD Parts | PartNode key | Sorted/keyed rather than ordinal identity. |
| MGEF Sounds | Pinned dev-4.1.6 uses wbArrayS/StructSK([0]); released 4.1.5f uses plain wbArray/wbStruct | Runtime deliberately preserves aligned declaration order for 4.1.5f; see version exception below. |
| WTHR Sounds | Sound Type | Type is key, selected sound is data. |
| CLMT WeatherTypes | Weather FormID | Chance/global remain data. |
| CONT/COBJ/NPC Items | Item FormID | Count and COED are payload; specialized duplicate/ownership rules still apply. |
| DOBJ Objects | Use/type field | Object FormID is data; DOBJ route remains dormant. |
| FACT Relations / Ranks | Target Faction / rank number | Keyed rows; Conditions remain aligned. |
| Leveled entries / PERK Effects | Composite xEdit entry/effect keys | Consult exact handler and expectation, not whole-row equality. |
| NPC/RACE Attacks / RACE MovementTypes | Attack Event / Movement Type | Other values are row payload. |
| LCTN arrays | Definition-specific Ref, Actor Ref, Worldspace or parent-reference key | Nested coordinates are data inside the keyed entry. |
| CameraPath RelatedPaths / IDLE RelatedIdles | Fixed Parent/Previous structure | Atomic fixed structure; IDLE RelatedIdles is not actively registered. |
| SHOU WordsOfPower | Don't sort and dfNotAlignable | One exact authored sequence, no per-entry alignment. |
| REGN Areas / structural grids / executable graphs | Geometric or positional connections | Do not infer safe union/alignment without a structural model. |
| LAND Layers/Textures | Union/context-dependent projection | Remain dormant generic Unordered exceptions pending a dedicated model. |

DLVW Topics has no collection member on the referenced writable Mutagen interface;
no independent registration is justified by its ordered xEdit definition alone.
WATR UnusedNoisemaps is excluded; an earlier unordered classification is not an
active policy. Recheck exact generated/binary surfaces before changing either.

## MGEF SNDD version exception

xEdit 4.1.5f, tag `xedit-4.1.5f`, commit
`f5c00f3fa3ee39511185515802647246c807f759`, defines `wbMagicEffectSounds`
as `wbArray(SNDD, 'Sounds', wbStruct(...))` in
`Core/wbDefinitionsCommon.pas:663–681`. Its plain struct uses both Type and Sound
as the display alignment key (`Core/wbInterface.pas:14419–14496`);
`TwbSubRecord.GetAlignable` permits this variable-length unsorted array
(`Core/wbImplementation.pas:14726–14747`). `SetSummaryKey` controls the summary,
not sorting. Inspect this version without replacing the pinned reference:

```bash
git -C XEditSource fetch origin tag xedit-4.1.5f --depth 1
git -C XEditSource show xedit-4.1.5f:Core/wbDefinitionsCommon.pas
```

The newer pinned definition sorts by numeric Type. Applying that policy to
FrostDamageConcAimed reverses its authored Concentration/Sheath order after
Release is removed, and 4.1.5f displays the moved entry on another aligned row.
Runtime therefore uses shared `AlignedOrdered` reconciliation and generated
full-pair equality, with no Type-only key. This intentionally differs from the
newer reference; it remains valid declaration-order plugin data.

Mutagen 0.54.4 `MagicEffectBinaryOverlay.FillRecordType` reads SNDD as 8-byte
Type/Sound pairs in stream order. `MagicEffectBinaryWriteTranslation.Write`
passes the list unchanged to `ListBinaryTranslation`, and
`MagicEffectSoundBinaryWriteTranslation.WriteEmbedded` writes the 4-byte Type
followed by the Sound FormID. No sorted-container setting is serialized.
`UserReportedFailureRegressionTests.FrostbiteSoundRemovalPreservesDeclaredOrderThroughPatchOutput`
covers mutable and binary-overlay input, retained Release removal, duplicate
occurrences and binary patch reload; this does not establish an xEdit GUI run.
