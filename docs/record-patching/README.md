# How records are patched

[Project front page](../../README.md) · [Record index](INDEX.md)

Dread’s Mashed Patch combines changes to selected properties from your load order. Most properties are handled separately. Some values, groups and collections are selected together to keep their contents consistent. **Atomic means “choose the complete value from one source,” not “always use the last plugin.”**

Use the [record index](INDEX.md) to find a record by its four-letter signature and read its property rules and limitations. [Unsupported records](unsupported.md) covers the remaining record families and indirect handling.

## Which records reach the patch

1. The patcher reads enabled input plugins and processes enabled record families, including supported child records. The [front page](../../README.md#getting-started) explains the output plugin's load-order cutoff.
2. Ignored plugins are removed from each record’s override history. The **effective winner** is the last remaining version. Ordinary merging is skipped if this winner is an official source, there are at most two eligible versions, or the version immediately before the winner is official. Priority Mods, preserved official Editor IDs and Tamriel’s special persistent-cell policy can still apply.
3. A matching Priority Mods entry selects a complete source record, bypassing ordinary merging and official EditorID preservation. The last matching entry in the configured priority list wins. Otherwise, properties are processed from oldest to newest using the rules below and the record's specific rules.
4. For ordinary property merging, when the selected result differs from the effective winner, the patcher copies that winner and applies the selected changes. Properties without their own patching rule remain from the copied winner or travel with another selected value. Priority Mods and Tamriel’s special persistent-cell policy can instead copy their selected source directly.
5. Before writing, the patcher validates its completed output in memory. An override with an unresolved or incorrectly typed non-null record reference is omitted in full. The engine's built-in Player reference is valid in actor-reference fields even without a plugin record. NPC package links and leveled-list entries must not target deleted records. Leveled lists are checked for their 255-entry counter limit and circular nesting through the effective load order. For faction-owned items affected by an import limitation, the patcher reconstructs the required rank by choosing the compatible value closest to zero. Already correctly imported ranks are unchanged. Validation does not prune entries or change source plugins. Warnings name the affected records and input plugins to inspect; a cycle may result from combining individually valid source records.
6. The patcher writes the remaining patch, splitting it into numbered plugins if needed. A record that fails processing or output validation is skipped and reported; successful records can still be published as a partial patch. Fatal run errors or output-writing failures prevent publication. Omitting an override exposes the existing load-order winner and does not repair it.

## How a value is selected

The original is the oldest eligible version. Each selected value has an owner: the plugin supplying it. A later value different from both the original and the current selection normally becomes the new selection. Returning to the original value requires the current owner to be a declared master or configured virtual master of the later plugin. A virtual-master rule supplies the same permission relationship without changing plugin files.

For GLOB and GMST records, the effective winner determines the value type. The latest eligible change to that type starts a new baseline for all registered properties; earlier types are not converted or merged back in. Ordinary short-history rules apply to the versions remaining after this boundary. Priority Mods can still select a complete record of another type. A preserved official GMST EditorID must have a prefix matching the selected value type, because that prefix determines how the game setting is read from the plugin.

For example, if a value is originally 10, one mod changes it to 20, and an unrelated later mod carries 10, the change to 20 can survive. If the later mod depends on the owner of 20, it can restore 10. A newly different value such as 30 can replace the earlier selection. Special protection, collection and ownership-reset rules can modify this behavior.

Resource and appearance [property groups](assets.md) always use normal forwarding. Their members select together using the shared selection rules and each record's documented exceptions. The UI has no asset or appearance switches. **Forward tree changes as a group** is the only option for these groups and defaults off. Grouping does not verify deployed resources.

NPC Height, HairColor and protection remain independent of its appearance group. HairColor uses ordinary change/reversion rules; OppositeGenderAnims has its own animation decision. The [NPC rules](actors.md#npc_-npc) describe which appearance fields are selected together.

Flags outside asset groups use ownership per registered bit, with the effective winner as their initial forward state unless a record’s ownership-reset rule starts them from its baseline. Collections track their own additions, removals and replacements. These are not simply a union of every version. In records with an **ownership reset**, a semantic change to a listed trigger replaces all registered property contexts with that override’s values before later versions are processed. This does not select the triggering plugin’s unregistered fields.

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
| Models and icons | Use normal [grouped forwarding](assets.md). Whole values, including ordered alternate textures/model data or icon paths. ARMA keeps each gender’s first/third-person meshes with alternate textures and its weight-slider flag. Asset paths use path-aware comparison. |
| ModelAndBounds | Keep the effective winner's bounds when the selected model filename matches the winner, ignoring path case and slash direction. When selecting a different filename, carry bounds from the source that supplied that filename. Bounds-only edits are not forwarded. |
| WorldModelAndBounds | ARMO’s equivalent rule, with male/female mesh models together on the model side. Icons have independent forwarding rules. Keep winning bounds when both selected filenames match the winner; otherwise carry bounds with the selected filename pair. |
| ObjectBounds | Standalone bounds stay with the effective winner. They are not independently forwarded. |
| Destructible | Health, stage structure, flags and gameplay references remain one ordinary value. Stage models use their separate mesh-reference decision and cannot restore removed stages. |
| Binary payloads | Complete byte values; never merged byte by byte. |
| VirtualMachineAdapter treated as keyed rows | Ordinary list-style scripts match by name. Each script’s flags and complete properties are one authored value; individual script properties/arrays are not merged. Replacing scripts in ordinary list-style adapters preserves the destination Version/ObjectFormat; only creation without a destination uses Mutagen defaults (5/2). Metadata is not independently selected on this path. |
| Other script adapters | Forward normally as grouped values. INFO, PACK and PERK select whole adapters. QUST and SCEN expose presence and child fields separately; their VMAD-only groups keep metadata, scripts and bindings together. Record structure retains its own ownership rules. Their record notes describe the gates and ownership rules. Papyrus unused bytes do not determine script equality; copying preserves destination unused values where its policy supports that. |
| Form identity and general record metadata | Not independent merge decisions. This includes version-control and runtime/registration members. Record-specific omissions appear in the tables. |

## Placed-reference state and placement

ACHR and REFR support moves between interior cells: the containing cell, position and rotation are selected together under normal addition/reversion permissions. Scale is independent, so a size-only edit cannot undo an accepted move. Cell-only moves count even when the coordinates match.

LocationReference comes from overrides describing the accepted cell. Compatible location-only corrections remain possible, with the normal permission checks for removal and restoration. PersistentLocation, InitiallyDisabled and EnableParent keep their separate owners; EnableParent retains its reference and flags together. A supported interior move uses the final Persistent flag to choose its destination collection.

Cross-cell moves involving exterior cells or specialized door/navigation, room/multibound, occlusion, water or attachment data keep winning placement and dependent spatial payloads. Expected exterior exclusions are informational; other rejected moves produce record-specific warnings. Other valid edits still merge. Same-parent histories retain their existing forwarding rules. This change does not implement general exterior or persistence-only storage correction, location-table rebuilding, script repair or LOD generation.

InitiallyDisabled is a starting-state flag; references may become enabled during play. Safe undeleted-and-disabled patterns receive the same independent state decisions. Hazards and placed projectiles retain their existing Transform group (position, rotation and scale).

Inspect the generated patch in xEdit. These property rules do not guarantee correct gameplay behavior for every mod combination or byte-for-byte preservation of omitted fields.
