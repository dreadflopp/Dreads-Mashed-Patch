# How records are patched

[Project front page](../../README.md) · [Record index](INDEX.md)

Dread’s Mashed Patch combines changes to selected properties from your load order. Most properties are handled separately. Some values, groups and collections are selected together to keep their contents consistent. **Atomic means “choose the complete value from one source,” not “always use the last plugin.”**

Use the [record index](INDEX.md) to find a record by its four-letter signature and read its property rules and limitations. [Unsupported records](unsupported.md) covers the remaining record families and indirect handling.

## Which records reach the patch

1. The patcher reads enabled input plugins and processes enabled record families, including supported child records. The [front page](../../README.md#getting-started) explains the output plugin's load-order cutoff.
2. Ignored plugins are removed from each record’s override history. The **effective winner** is the last remaining version. Ordinary merging is skipped if this winner is an official source, there are at most two eligible versions, or the version immediately before the winner is official. Priority Mods, preserved official Editor IDs and Tamriel’s special persistent-cell policy can still apply.
3. A matching Priority Mods entry selects a complete source record. The last matching entry in the configured priority list wins. Otherwise, properties are processed from oldest to newest using the rules below and the record's specific rules.
4. When the selected result differs from the effective winner, the patcher copies that winner and applies the selected changes. Properties without their own patching rule remain from the copied winner or travel with another selected value.
5. The patcher writes the resulting patch, splitting it into numbered plugins if needed.

## How a value is selected

The original is the oldest eligible version. Each selected value has an owner: the plugin supplying it. A later value different from both the original and the current selection normally becomes the new selection. Returning to the original value requires the current owner to be a declared master or configured virtual master of the later plugin. A virtual-master rule supplies the same permission relationship without changing plugin files.

For GLOB and GMST records, the effective winner determines the value type. The latest eligible change to that type starts a new baseline for all registered properties; earlier types are not converted or merged back in. Ordinary short-history rules apply to the versions remaining after this boundary. Priority Mods can still select a complete record of another type. A preserved official GMST EditorID must have a prefix matching the selected value type, because that prefix determines how the game setting is read from the plugin.

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

Null and empty are not interchangeable everywhere. Some handlers track presence; others normalize to empty or a record-specific default. The shared keyword setter and NPC Perks setter write null as absence and preserve a present-empty list. The tables do not promise that null universally removes every field.

## Properties shared across records

| Property or value kind | Handling |
|---|---|
| EditorID | Independently selected, subject to settings. The default suppresses an override whose only change is EditorID. Normal forwarding and preserving the latest official baseline’s EditorID are alternatives. Official-baseline membership is configurable. |
| Record-header flags | Supported bits are merged separately, including clearing a bit. Bits without their own rule stay as in the effective winner. |
| Text | Many plain-string comparisons ignore trailing whitespace. Translated names select and copy one complete source value, retaining its target language and all available translations. Comparison follows Mutagen’s default-language/all-language setting and does not trim text. Optional null names remove the value; required null names become empty. MATT Name remains a plain string. |
| Numbers | Ordinary reflection floats use a 0.0001 comparison tolerance; the specialized Weight handler uses 0.001. Not every numeric or grouped value shares that tolerance. |
| Models and icons | Whole values, including ordered alternate textures/model data or icon paths. ARMA explicitly splits its models. Asset paths use path-aware comparison. |
| ModelAndBounds | An accepted model filename/geometry change brings bounds from that source. A bounds-only change can still forward separately. It is not an unconditional atomic model-plus-bounds decision. |
| WorldModelAndBounds | ARMO’s equivalent rule, with male/female models and icons together on the model side. |
| ObjectBounds | Complete bounds, not individual coordinates. |
| Destructible | Complete structure, including stages and their models/references. |
| Binary payloads | Complete byte values; never merged byte by byte. |
| VirtualMachineAdapter treated as keyed rows | Scripts match by name. Each script’s flags and complete properties are one authored value; individual script properties/arrays are not merged. Replacing scripts in ordinary list-style adapters preserves the destination Version/ObjectFormat; only creation without a destination uses Mutagen defaults (5/2). Metadata is not independently selected on this path. |
| Other script adapters | INFO, PACK and PERK select whole adapters. QUST and SCEN split presence and child fields explicitly; their record notes describe the gates and ownership rules. Papyrus unused bytes do not determine script equality; copying preserves destination unused values where its policy supports that. |
| Form identity and general record metadata | Not independent merge decisions. This includes version-control and runtime/registration members. Record-specific omissions appear in the tables. |

## Placed-reference coordination

ACHR and REFR coordinate safe deletion handling. A safe UDR state combines InitiallyDisabled, Z = -30000 at comparison precision, and an enable parent opposite to the player. When such a state occurs in the history, the owner of the selected InitiallyDisabled bit can also supply Placement and EnableParent. Coherent restoration is handled too. An ordinary disabled reference does not automatically trigger this rule. PHZD and the seven placed-projectile variants do not use it.

Inspect the generated patch in xEdit. These property rules do not guarantee correct gameplay behavior for every mod combination or byte-for-byte preservation of omitted fields.
