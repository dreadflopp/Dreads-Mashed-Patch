# Known issues and limits

[Patching guide](README.md) · [Record index](INDEX.md)

These describe current behavior, not fixes. The first five findings were reproduced using small synthetic records; the review did not run a real game load order or verify the resulting plugin in Skyrim.

## Record-header flags

On the legacy path with both raw and common Skyrim header views, applying the common view can OR previously set raw bits back into the result. A selected bit clear can therefore be lost. An end-to-end keyword example with raw flags `0x800 → 0 → 0x800` emitted `0x800` despite selecting the clear. ARMO, PERK, STAT, TACT, TXST and WRLD use a different composite path.

Evidence: [shared application](../../DreadsMashedPatch/RecordHandlers/Abstracts/AbstractRecordHandler.cs), [raw flags](../../DreadsMashedPatch/PropertyHandlers/General/MajorRecordFlagsRawHandler.cs).

## Container item extra data

CONT Items snapshots retain only the item reference and count. When the list is rewritten, ownership/condition extra data (COED) is lost, including data present in every input version. A full run with counts `1 → 2 → 1` forwarded count 2 but removed the extra data. NPC inventory and LVLI entries have different copying rules.

Evidence: [container item reader and writer](../../DreadsMashedPatch/PropertyHandlers/Container/ItemHandler.cs), [shared row replacement](../../DreadsMashedPatch/PropertyHandlers/Abstracts/AbstractListPropertyHandler.cs).

## Script-adapter metadata

Ordinary list-style VMAD setters construct a new adapter rather than preserving Version/ObjectFormat. A full run merging a changed script from adapters all marked `4/1` produced metadata `5/2`. This affects the adapter-list path, not the whole adapters in INFO/PACK/PERK or explicit metadata fields in QUST/SCEN.

Evidence: [adapter setter](../../DreadsMashedPatch/PropertyHandlers/Abstracts/AbstractVirtualMachineAdapterHandler.cs).

## Policies skipped by the initial filter

Always-win plugin and preserve-baseline EditorID policies are evaluated after the normal early filter. They cannot affect records already skipped for short histories, a vanilla winner or an immediately preceding vanilla version. Two-version full-run examples produced no override for either a differing priority source or a differing baseline EditorID. Tamriel’s special CELL path can bypass the initial filter.

Evidence: [ShouldBreakEarly and dispatch](../../DreadsMashedPatch/Program.cs), [record policies](../../DreadsMashedPatch/RecordHandlers/Abstracts/AbstractRecordHandler.cs).

## Optional name translations

The shared Name setter rebuilds optional translated names from a string as English. When a name is forwarded, other languages can be lost. A direct setter check on a book with English and French names removed French. This is specific to that setter; translated-string reflection handlers copy available languages.

Evidence: [NameHandler](../../DreadsMashedPatch/PropertyHandlers/General/NameHandler.cs).

## Errors can leave partial overrides

The ordinary output override is created before its properties are applied. Property failures are logged and other setters may continue. Record-level exceptions are caught, but there is no general rollback of an already-created override. Thus a “skipping record” message does not establish that the record is absent from the output. The desktop file transaction can commit a run containing such caught errors.

Evidence: [record processing](../../DreadsMashedPatch/RecordHandlers/Abstracts/AbstractRecordHandler.cs), [runner](../../DreadsMashedPatch.App/Services/PatcherRunner.cs), [file transaction](../../DreadsMashedPatch/PatchOutputTransaction.cs).

## Coverage and comparison limits

- PHZD omits inherited placement, scripts and reference fields. The omission is established; its reason is uncertain.
- PACK’s template graph is not registered, even when package processing is enabled.
- MGEF archetypes compare only Type, ActorValue and AssociationKey; differences in other copied subtype members alone do not trigger forwarding.
- ARMA model data bytes travel with filename selection but do not independently trigger it.
- Null is not universally written as removal; the keyword setter ignores null.
- No patcher-owned NAVI merge or rebuild was found. Forwarding NAVM data or navigation-door links does not establish one.

The [record tables](INDEX.md) and [unsupported list](unsupported.md) show exact scope. Reasons for unsupported types without explicit exclusion entries, the gameplay meaning of opaque payloads, and complete on-disk preservation of serialization/unused fields remain **uncertain**. No intended behavior is inferred from comments or handler names alone.
