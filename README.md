# Dread's Mashed Patch

Dread's Mashed Patch is an automatic conflict-resolution patcher for Skyrim modlists. It combines changes to supported record properties from your load order into `MashedPatch.esp`, using property-specific rules, plugin master relationships, and configurable forwarding policies.

The desktop app runs on **Windows and through Proton on Linux**. Launch it directly or through your mod manager. Select your game and load order, choose the settings below, and generate your patch.

It may be useful to modlist creators, patch creators, and users who patch their own modlists.

## Download

Download the patcher from **Nexus Mods**: [Nexus download page — placeholder](https://www.nexusmods.com/skyrimspecialedition/mods/REPLACE_WITH_MOD_ID).

The link above will be replaced with the published mod page.

## Testing and manual review

**The current version is still being tested, and patching bugs are possible.** The patcher handles a large number of record types and properties. Some records rarely conflict and their conflict-handling behavior has not yet been tested in real modlists. Automated tests and successful patch generation do not guarantee correct results for your modlist.

**Inspect the generated patch in xEdit before using it.** Check that changes were patched as intended. An automatic patcher cannot resolve everything correctly: the end user must manually correct some of the patching and create additional compatibility patches where needed. Review every output plugin if the patch is split into multiple files.

Please report bugs through this repository's **Issues** tab. Include the patcher version, load order, relevant plugins, record FormID/FormKey and property, expected and actual results, xEdit screenshots, and the run log. See [logs and diagnostics](docs/STANDALONE_APP.md#diagnostics).

## Getting started

1. Launch `DreadsMashedPatch.exe`. With MO2, add it as an executable and launch through MO2. With Amethyst, deploy your active profile first.
2. On **General**, select your game release, game folder, Data folder, active `plugins.txt`, and patch output folder. Select **Verify paths**.
3. Review **Record Types** and the settings below. The recommended defaults are a starting point.
4. Select **Run patcher**, enable the generated plugins, and inspect them in xEdit.

For a normal install, your game folder might be `C:\Program Files (x86)\Steam\steamapps\common\Skyrim Special Edition`, with `Data` beneath it. Under Proton, an example is `Z:\home\<username>\.local\share\Steam\steamapps\common\Skyrim Special Edition`. If your list uses a separate **Stock Game** or **Game Root** copy, select that copy instead. Browse to your actual installation if it lives elsewhere.

With MO2 or Amethyst, create an empty mod for the patch in your manager and select its folder as the output. Without a mod manager, use the game's Data folder. The [setup guide](docs/STANDALONE_APP.md#selecting-folders) has complete examples for both normal and copied installations, including profile and output paths.

The output is `MashedPatch.esp`, with numbered plugins if it needs to be split. If `MashedPatch.esp` is already in your load order, only enabled plugins before it are used as inputs. Otherwise, the full enabled load order is used. Enable every generated output plugin.

## Settings

### General

| Setting or action | What it does |
| --- | --- |
| Game release | Select Special Edition Steam, Special Edition GOG, or Skyrim VR. Anniversary Edition uses the corresponding Special Edition option. |
| Skyrim game folder | The installation you actually launch; contains `SkyrimSE.exe` or `SkyrimVR.exe`. This is also where the patcher reads `Skyrim.ccc`, when present. |
| Data folder | The plugins used by your active game or modlist. MO2 must expose its virtual files by launching the patcher through MO2; Amethyst must deploy the profile first. |
| Load order file (plugins.txt) | Your active mod-manager profile's `plugins.txt`, or the game's active file when running without a manager. |
| Patch output folder | Where generated patch plugins are saved. Select your empty patch mod folder, or the game's Data folder without a manager. |
| Detect paths | Finds registered game installations. Check the result if your list uses a copied installation. |
| Verify paths | Checks plugin files against the selected load order. Expand the details to see missing files, disabled plugins, and plugins after the patch. Verify again after switching profiles or deploying mods. |
| Create empty patch output | Replaces existing patch outputs in the selected folder with one empty `MashedPatch.esp`, so you can enable and position it before the first full run. |
| Save settings | Saves your choices without running. **Run patcher** also saves them automatically. |

### Record Types

Enable the record families you want patched. Search by name or the four-character xEdit signature; press Space to toggle a selected row. Most supported types are enabled by default. New supported families are enabled automatically when you update.

Dialogue records (**DIAL, DLBR, INFO, DLVW**), navigation meshes (**NAVM**), and AI packages (**PACK**) start disabled. Dialogue and navigation data need coordinated changes; packages also have a known writer ordering issue. Read their [patching descriptions](docs/record-patching/INDEX.md) before enabling them.

### Priority Mods

Enter one full plugin filename per line, including `.esp`, `.esm`, or `.esl`. For records edited by a listed plugin, its complete record takes priority over property merging. If several listed plugins edit the same record, the one **lowest in this list** wins. For example, use this when a compatibility patch's complete result should take precedence.

### Ignored Mods

Enter one full plugin filename per line to leave its overrides out of conflict discovery. This does not disable the plugin in your game. The [patching guide](docs/record-patching/README.md) explains how ignored plugins affect forwarding and winning records.

### Smart Forwarding

“Forwarding” means carrying a mod's accepted changes into the patch.

| Setting | What it does |
| --- | --- |
| Treat Creation Club content as official baseline | Enabled by default. Treats installed Creation Club plugins as official game content. Disable it to evaluate their changes like ordinary mods. |
| Keep one vanilla weapon type keyword | Enabled by default. When an accepted change gives a weapon exactly one configured vanilla type, removes the other configured types. Other keywords and deliberately authored combinations of several types are preserved. |
| Configured vanilla weapon type keywords | The types used by the option above. Enter one FormKey per line, such as `01E711:Skyrim.esm` for swords. The defaults cover the vanilla weapon types. |
| Editor IDs (EDID) | Controls internal record names. **Forward only on an existing patch record** (default) carries renames only when another change needs a patch. **Preserve the official Editor ID** keeps the latest official name. **Forward Editor ID changes** allows renames to create overrides on their own. |
| NPC protection status | **Protect NPCs unless deliberately changed** (default) prefers Essential/Protected status but allows a downgrade by a mod authorized through ownership, declared masters, or Master Rules. **Always keep the strongest protection** keeps Essential over Protected over neither. **Forward each status change normally** uses ordinary forwarding rules. |
| Tamriel persistent cell (000D74:Skyrim.esm) | Applies to the Tamriel persistent cell's header. The default patches normally, but uses Dawnguard's header when the winner matches Skyrim's original. You can instead always use Dawnguard, use Skyrim, keep the winning override, or patch normally. This option does not select the cell's placed objects or other child records. Priority Mods takes precedence. |

### Master Rules

A plugin can normally undo another plugin's changes when it declares that plugin as a master. These rules grant the same permission for relationships missing from plugin headers.

Select or add a **Mod**, then enter its **Additional virtual masters**, one complete plugin filename per line. For example, listing `OtherMod.esp` as a virtual master for `MyPatch.esp` lets `MyPatch.esp` intentionally restore values changed by `OtherMod.esp`. Plugin files are not modified. A rule with no virtual masters has no effect.

Curated rules are included by default. **Import** replaces the current rules, **Export** saves them to a portable file, and **Restore default rules** replaces your edits with the bundled defaults.

### Diagnostics and Run Log

| Setting | What it does |
| --- | --- |
| Historical logs to keep | Number of previous run logs to retain (default 10, range 0–100). Set 0 to keep only the current log. |
| Enable debug mode | Adds detailed patching decisions to the log. Leave off for routine runs; enable when investigating a result. |
| Verbosity | In debug mode, **Summary only** limits general diagnostic detail; **Changed records** (default) includes context-change decisions. |
| Maximum value preview length | Limits how much of each value is printed in diagnostic previews (default 240 characters, range 40–10,000). |
| Deep-dive record signatures | In debug mode, logs every registered property for matching record families, such as `QUST`. Broad selections can create large logs. |
| Deep-dive FormKeys | In debug mode, targets specific records, such as `00285D:Update.esm`. Bare hexadecimal FormIDs are also accepted. |

Deep-dive fields accept one value per line, commas, or semicolons. The **Run Log** tab shows progress, warnings, and errors; the complete log is saved to `%LOCALAPPDATA%\DreadsMashedPatch\Logs\DreadsMashedPatch-current.log` (inside the prefix on Proton). Settings are saved in `%LOCALAPPDATA%\DreadsMashedPatch`.

## Supported and unsupported records

- [Supported record types and patching descriptions](docs/record-patching/INDEX.md): the complete list, with links to each record's property rules and limits, including types disabled by default.
- [Unsupported record types and reasons](docs/record-patching/unsupported.md): records left out of conflict resolution and those handled indirectly.
- [How patching works](docs/record-patching/README.md): shared rules for selecting changes, merging lists, and handling flags and other properties.
- [Known issues and limits](docs/record-patching/KNOWN-ISSUES.md): current limitations to consider when reviewing your patch.

## Further documentation

The [application setup guide](docs/STANDALONE_APP.md) covers mod managers, Proton, path examples, and output troubleshooting. Architecture, build instructions, audits, and migration notes are in the separate [developer documentation](docs/README.md#developer-documentation).
