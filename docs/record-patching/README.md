# How records are patched

Dread’s Mashed Patch combines changes to selected properties from your load order. Most properties are handled separately. Some values, groups and collections are selected together to keep their contents consistent. **Atomic means “choose the complete value from one source,” not “always use the last plugin.”**

Use the [record index](INDEX.md) to find a record by its four-letter signature. The guides list every supported record variant, its property rules and its omissions. [Known issues](KNOWN-ISSUES.md) describe confirmed differences between a selected change and the output. [Unsupported records](unsupported.md) covers the remaining record families and indirect handling.

| Subject | Guide |
|---|---|
| Actors, races, factions and character data | [Actors](actors.md) |
| Weapons, armor, inventory and crafting | [Items](items.md) |
| Spells, effects and projectiles | [Magic](magic.md) |
| Cells, worldspaces, weather and navigation | [World](world.md) |
| Placed actors, objects and hazards | [Placed records](placed.md) |
| Quests, scenes, dialogue, perks and packages | [Quests and AI](quests.md) |
| Sound and music | [Audio](audio.md) |
| Textures, impacts and other visual effects | [Visuals](visuals.md) |
| Activators, furniture and other objects | [Objects](objects.md) |
| Settings, globals, lists and messages | [Settings](settings.md) |

## Which records reach the patch

1. The desktop app prepares the input list, including installed plugins from Skyrim.ccc. Synthesis loads the input plugins before the output plugin, `MashedPatch.esp`; the output plugin and later plugins are outside that input cutoff. Enabled record families are queried, including supported records nested under cells, worldspaces and dialogue topics.
2. Ignored plugins are removed from each record’s override history. The **effective winner** is the last remaining version. Normally the record is skipped if this winner is an official/vanilla source, there are at most two eligible versions, or the version immediately before the winner is vanilla. Tamriel’s special persistent-cell policy can bypass this initial filter.
3. For a record that reaches its handler, a matching always-win plugin selects a source record instead of merging properties. The last matching entry in the configured priority list wins. If that source is already the effective winner, no override is needed. The initial filter can prevent priority and EditorID policies from running; see [policy limitations](KNOWN-ISSUES.md#policies-skipped-by-the-initial-filter).
4. Otherwise the properties below are processed from oldest to newest. Special record rules may reset all registered properties to a newer version or coordinate related fields. The result is compared with the effective winner.
5. If a change remains, the patcher creates an override from the effective winner, then applies the selected flags and other changed properties. NPC records use the patch’s NPC collection to obtain their override; other records use their resolved context. Parent containers are created as required. Unregistered properties are not independently merged from earlier mods.
6. Synthesis writes the patch, splitting output if necessary for master limits. The desktop app stages the files before replacing the previous output. Caught property or record errors can still leave partial changes in this output; file staging is not per-record rollback.

DIAL, DLBR, INFO, DLVW, NAVM and PACK are **supported but disabled by default**. Their guide sections describe behavior when enabled. DOBJ, LAND, LTEX and IMAD are explicitly excluded and cannot be enabled through this supported-record path.

## How a value is selected

The original is the oldest eligible version. Each selected value has an owner: the plugin supplying it. A later value different from both the original and the current selection normally becomes the new selection. Returning to the original value requires the current owner to be a declared master or configured virtual master of the later plugin. A virtual-master rule supplies the same permission relationship without changing plugin files.

For example, if a value is originally 10, one mod changes it to 20, and an unrelated later mod carries 10, the change to 20 can survive. If the later mod depends on the owner of 20, it can restore 10. A newly different value such as 30 can replace the earlier selection. Special protection, collection and ownership-reset rules can modify this behavior.

Flags use ownership per registered bit, with the current winner as their initial forward state. Collections track their own additions, removals and replacements. These are not simply a union of every version. In records with an **ownership reset**, a semantic change to a listed trigger replaces all registered property contexts with that override’s values before later versions are processed. This does not select the triggering plugin’s unregistered fields.

## Reading the property tables

Property names match the record model so that you can identify exact fields. A dotted name identifies a child field. **Multiple names in a row are separate decisions**, unless a record note explicitly groups them. Shared properties below apply even when not repeated in a record’s table.

| Table wording | Meaning |
|---|---|
| Select each value separately | Each named scalar, reference, vector or color is selected independently. Vector/color components are not individually merged. A form reference selects a target’s identity, not its contents. |
| Merge registered flag bits separately | Each supported bit has its own decision. This does not guarantee handling of every unknown bit. Flags inside an atomic value travel with that value. NPC protection is a specific exception. |
| Select each whole value separately | Each named structure is selected and copied as a unit, including nested lists and dictionaries. No recursive property merge. Model/bounds coupling is the exception described below. |
| Select each whole collection separately | One source supplies the entire named list or dictionary, including order, entries and nested data. |
| Merge rows by key | Match entries by their identity, then reconcile membership and complete row values. Duplicates can be retained. Script names, form references and record-specific keys identify rows; special keys are noted in the guides. |
| Merge aligned rows in order | Align entries across versions and merge their complete row values while retaining meaningful order. |
| Merge rows by position | Each ordinal is a separate complete row decision. No matching by identifier and no sequence alignment. |
| Not independently forwarded | No property decision for this field. It can remain from the copied winner or travel inside another selected value; this does not guarantee identical bytes after writing. |
| Serialization/unused state | No separate merge decision for layout or unused fields. Copy and writer behavior can affect them. |
| Child/group surfaces | These are not merged as property lists. Supported child records have their own processing paths. |

Null and empty are not interchangeable everywhere. Some handlers track presence; others clear a collection or decline to write null. In particular, the keyword setter does nothing for a null selection. The tables do not promise that null universally removes a field.

## Properties shared across records

| Property or value kind | Handling |
|---|---|
| EditorID | Independently selected, subject to settings. The default suppresses an override whose only change is EditorID. Normal forwarding and preserving the latest official baseline’s EditorID are alternatives. Official-baseline membership is configurable. |
| Record-header flags | Every record handler registers raw header flags. Most also register the common Skyrim view, and some a record-specific view. These overlap in storage. ARMO, PERK, STAT, TACT, TXST and WRLD use a composite raw-header path; the other path has a [confirmed flag-clearing issue](KNOWN-ISSUES.md#record-header-flags). |
| Text | Many string comparisons ignore trailing whitespace. Optional names written by the shared Name handler become English-only translated strings. Translated-string reflection handlers copy all available languages, but comparison follows Mutagen’s default-language/all-language setting. See [translation loss](KNOWN-ISSUES.md#optional-name-translations). |
| Numbers | Ordinary reflection floats use a 0.0001 comparison tolerance; the specialized Weight handler uses 0.001. Not every numeric or grouped value shares that tolerance. |
| Models and icons | Whole values, including ordered alternate textures/model data or icon paths. ARMA explicitly splits its models. Asset paths use path-aware comparison. |
| ModelAndBounds | An accepted model filename/geometry change brings bounds from that source. A bounds-only change can still forward separately. It is not an unconditional atomic model-plus-bounds decision. |
| WorldModelAndBounds | ARMO’s equivalent rule, with male/female models and icons together on the model side. |
| ObjectBounds | Complete bounds, not individual coordinates. |
| Destructible | Complete structure, including stages and their models/references. |
| Binary payloads | Complete byte values; never merged byte by byte. |
| VirtualMachineAdapter treated as keyed rows | Scripts match by name. Each script’s flags and complete properties are one authored value; individual script properties/arrays are not merged. Applying ordinary list-style adapters reconstructs them with metadata defaults, a [known issue](KNOWN-ISSUES.md#script-adapter-metadata). |
| Other script adapters | INFO, PACK and PERK select whole adapters. QUST and SCEN split presence and child fields explicitly; their record notes describe the gates and ownership rules. Papyrus unused bytes do not determine script equality; copying preserves destination unused values where its policy supports that. |
| Form identity and general record metadata | Not independent merge decisions. This includes version-control and runtime/registration members. Record-specific omissions appear in the tables. |

## Placed-reference coordination

ACHR and REFR coordinate safe deletion handling. A safe UDR state combines InitiallyDisabled, Z = -30000 at comparison precision, and an enable parent opposite to the player. When such a state occurs in the history, the owner of the selected InitiallyDisabled bit can also supply Placement and EnableParent. Coherent restoration is handled too. An ordinary disabled reference does not automatically trigger this rule. PHZD does not use it.

## Scope and evidence

These guides describe source baseline `823a8f3`, reviewed on 2026-10-04 with Mutagen Skyrim 0.54.4. The [inventory](INVENTORY.md) remains the authoritative coverage checklist. Implementation was rechecked through input preparation, queries, source resolution, selection, setters and output writing; see the [review evidence](REVIEW.md). Property support does not establish gameplay safety for every combination or byte-for-byte preservation of omitted fields.
