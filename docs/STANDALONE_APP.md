# Mashed Patch standalone application

[Project front page](../README.md) · [Documentation index](README.md)

The current version is still being tested. Inspect every generated output plugin in xEdit, correct patching that does not match your intended modlist behavior, and report bugs through the repository's Issues tab. Download the executable from the Nexus placeholder linked on the project front page.

Mashed Patch is a Windows desktop patcher. It is not intended to be added to or launched by the Synthesis application. The desktop host uses Mutagen and the Synthesis pipeline libraries internally to construct the load order and write the output plugin, while retaining one patch implementation in `DreadsMashedPatch.Program.RunPatch`.

## Running

1. Launch `DreadsMashedPatch.exe`.
2. Confirm the Skyrim game folder, Data folder, active `plugins.txt`, and patch output folder on the **General** tab. Select **Verify paths** and review the report.
3. Choose record families and forwarding policies.
4. Select **Run patcher**. Settings are saved automatically before the run.

Settings are stored in the Windows user-local application data folder (`%LOCALAPPDATA%\DreadsMashedPatch`). In Proton, this is inside the prefix. The **Save settings** button writes settings without running the patcher.

Run output is stored in `%LOCALAPPDATA%\DreadsMashedPatch\Logs`. `DreadsMashedPatch-current.log` contains the latest run. At the start of the next run it becomes a timestamped historical log. The configurable retention limit defaults to ten historical logs, and the oldest logs are removed automatically.

### Mod Organizer 2

Add `DreadsMashedPatch.exe` to MO2 as an executable and launch it through MO2. For Wabbajack lists, select the list's Stock Game folder and its Data folder, then select the active MO2 profile's `plugins.txt`. Create an empty mod for the patch using MO2's interface, then select that mod's folder as the patch output folder.

### Wabbajack and Stock Game paths

Some lists launch a copied game installation, often named **Stock Game** or **Game Root**, rather than the official Steam/GOG installation. Both the game folder and Data folder must refer to the installation your list actually launches. **Detect paths** locates registered installations and may select the official installation instead. The highlighted notice on the General tab explains this; **Verify paths** can identify missing plugins but cannot prove which installation your manager launches.

### Proton and Amethyst

Launch the Windows executable in a Proton prefix that can access the selected game and profile paths, either directly or through Amethyst's external applications feature. Deploy the active profile first. Confirm the actual game folder (including a list's Stock Game copy), deployed Data folder, and active `plugins.txt` manually. An example Amethyst load-order path is `Z:\home\<username>\Games\Amethyst\Skyrim Special Edition\profiles\<profile name>\plugins.txt`; adjust it to match your installation and active profile. Create an empty mod for the patch using Amethyst's interface, then select that mod's folder as the patch output folder. Startup and General-tab rendering have been smoke-tested under Proton 10.0 in a fresh prefix at the default window size; this does not verify patching or mod-manager integration.

### Without a mod manager

Select your Skyrim installation's Data folder in both the **Data folder** and **Patch output folder** fields. Select the game's active `plugins.txt`, usually in `%LOCALAPPDATA%\Skyrim Special Edition` or the corresponding GOG or VR folder. Use **Create empty patch output** if you need to position the primary plugin first. Enable it and set its load-order position before running the full patcher; enable any numbered output plugins after a successful run.

### Selecting folders

- **Skyrim game folder:** Contains `SkyrimSE.exe` or `SkyrimVR.exe` and, when present, `Skyrim.ccc`.
- **Data folder:** Contains the plugins used by your game or mod list.
- **Patch output folder:** Stores the generated patch plugins. With MO2 or Amethyst, create an empty mod for the patch using your mod manager's interface, then select that mod's folder. Without a mod manager, select your Skyrim installation's Data folder.

The General tab has a shared **Mod manager setup** notice: launch the patcher through MO2 to access your active profile's virtual files, or deploy your active Amethyst profile before running the patcher. The paths below are examples. Replace the placeholders and adjust the folders to match your installation and active profile. The Amethyst game path follows the `.wj/<list name>/root/Game Root` layout.

| Field | MO2 example | Amethyst example (Proton) |
| --- | --- | --- |
| Skyrim game folder | `C:\Modlists\<list name>\Stock Game` | `Z:\home\<username>\Games\Amethyst\Skyrim Special Edition\.wj\<list name>\root\Game Root` |
| Data folder | `C:\Modlists\<list name>\Stock Game\Data` | `Z:\home\<username>\Games\Amethyst\Skyrim Special Edition\.wj\<list name>\root\Game Root\Data` |
| Load order file (plugins.txt) | `C:\Modlists\<list name>\profiles\<profile name>\plugins.txt` | `Z:\home\<username>\Games\Amethyst\Skyrim Special Edition\profiles\<profile name>\plugins.txt` |
| Patch output folder | `C:\Modlists\<list name>\mods\Mashed Patch` | `Z:\home\<username>\Games\Amethyst\Skyrim Special Edition\mods\Mashed Patch` |

### Typography on Windows and Proton

Both application windows use embedded, static TrueType Noto Sans fonts at 14 device-independent pixels for body text. Logs and multiline plugin/keyword fields use Noto Sans Mono at 13 pixels; record signatures and elapsed time use the same embedded monospace family. Regular, semibold, bold, and italic UI faces and regular/semibold monospace faces are included, so these styles do not require installing fonts in Windows or a Proton prefix. Glyphs outside the bundled families' coverage still use WPF font fallback.

The shared window style uses display text formatting, grayscale antialiasing, layout rounding, and device-pixel snapping. Grayscale avoids dependence on the prefix's ClearType/subpixel configuration. Buttons, fields, tabs, headings, and help text share spacing rules. System file dialogs, message boxes, and window decorations retain platform-provided typography because WPF application styles do not control those surfaces.

Typography migration note: both windows share one typography/rendering style; all former Consolas assignments use the bundled monospace resource or shared textbox style. The installation picker reuses the existing section-heading style. System dialogs remain platform-managed; patch logic and record-specific handlers retain their existing implementation. Font sources and the redistribution license are in `DreadsMashedPatch.App/Assets/Fonts`; the license is also embedded and copied alongside published output.

### Output and path verification

The patch output folder is selected separately from the Data folder and is required for both a full run and **Create empty patch output**. Existing settings from before this option was introduced require an explicit selection; the app does not silently reuse the Data folder. Both the run log and completion message identify the chosen destination. Only previous patch outputs in that destination are replaced; copies in other directories are untouched.

**Verify paths** shows counts of discovered files, disabled plugins, plugins after the patch, errors, and warnings on the General tab. Expand **Path verification details** for path information and the names of disabled plugins, patch outputs, and plugins excluded by the patch's load-order cutoff. Details stay collapsed when there are no issues and open automatically for errors or warnings. Patch outputs are listed separately and do not count as disabled. Disabled plugins after the patch appear in both relevant lists. Missing enabled inputs are errors; unlisted files are warnings because they may be intentionally inactive. Installed implicit masters and Creation Club plugins are accounted for without listing every enabled plugin. Duplicate listings and filenames are reported. Changing any selected path or game release invalidates the displayed report. Verify again after changing the manager's deployment or profile, even if the path strings remain the same.

Listings are parsed with Mutagen, and plugin identities are compared using `ModKey`. Filesystem checks are separate: a case-insensitive name match does not guarantee that the expected path exists on a case-sensitive filesystem. The report identifies such discrepancies without renaming files. This check does not import records, inspect master dependencies, validate output write permissions, or confirm the manager's launch target.

Pasted path text can include surrounding double quotes or mixed `/` and `\` separators. The app normalizes these for its runtime. Under Wine/Proton, existing Unix absolute paths are mapped through `Z:` when that mapped file or folder is accessible. All four path Browse buttons share the same normalization and use an absolute existing starting directory. If the shell dialog rejects it, Browse retries once without a starting directory; a persistent failure produces a handled error message. Clearing a path field also opens Browse without a starting directory.

Browse migration note: game, input, output, and plugins.txt browsing share one path-input and dialog-fallback implementation; the separate raw `InitialDirectory` assignments were removed. Master-rule import/export dialogs retain their existing behavior because they do not accept pasted starting-directory fields. Game-folder inference uses the same safe path handling. Record handlers and flag policies are unchanged.

## Record selection

Most supported record families are enabled by default. DIAL, DLBR, INFO, DLVW, NAVM and PACK start disabled in new configurations because their coupled data needs extra care. Their behavior when enabled and remaining limits are in the [record tables](record-patching/INDEX.md). The UI groups them by the four-character signatures used by xEdit, such as `QUST` for quests. Settings store only disabled families, so record families added in future versions are automatically enabled. The list supports search, arrow-key navigation, and Space to toggle the selected row. `Ctrl+F` focuses the search box.

## Diagnostics

The Run Log tab shows bounded progress, warnings, and errors while the complete output is written live to `DreadsMashedPatch-current.log`. Debug mode enables context-change logging and optional deep dives by xEdit record signature or FormKey without placing the full diagnostic stream in the UI. A matched deep-dive record logs every registered property. Broad Detailed logging remains available as a developer-only configuration because enabling it for every record can produce extremely large log files. Deep-dive selectors accept commas, semicolons, or one value per line.

The game release is selected explicitly and passed to Mutagen/Synthesis. Anniversary Edition uses the corresponding Special Edition Steam or GOG selection. Mutagen requires this value for implicit masters, load-order parsing, and binary defaults. Creation Club listings are read explicitly from `Skyrim.ccc` in the selected game folder, merged with `plugins.txt`, and deduplicated by the Synthesis pipeline.

The primary output name is fixed as `MashedPatch.esp`. If the patch needs more than 254 masters, Synthesis automatically splits it into additional numbered plugins such as `MashedPatch_2.esp`. If the primary output already appears in the selected load order, Synthesis reads only enabled plugins placed before it. If it is absent, Synthesis reads the complete enabled load order. Each run writes new output into a temporary folder inside the selected output folder, then replaces the previous primary and numbered outputs after writing succeeds. A failed patch build leaves the previous output files in place. Record/property errors discard the failed record candidate and reject the run after diagnostics finish, even when the handler caught the original exception. The runner requires a successful structured run report and no pipeline errors before publishing the primary or split outputs; warnings alone do not block publication.

Use **Create empty patch output** on the General tab before the first full run when the plugin must be positioned in a mod manager. The action removes the primary and recognized split outputs, then writes one empty, masterless plugin at the stable primary filename.

Path/output migration note: full runs and empty-output creation now share one explicit destination setting and the existing output transaction. The Data folder setting remains separate; Synthesis still handles imports, deduplication, load-order trimming, and output splitting. Creation Club file parsing is shared by preparation and verification. No record handlers or flag policies changed. The old output destination derived from the Data folder and obsolete Creation Club parsing loop were removed.

Plugins on the **Priority Mods** tab win at record scope. If another plugin overwrites one of their records, Mashed Patch copies the complete record snapshot from the matching priority mod occurring last in the configured list instead of merging individual properties. No patch record is needed when that selected source is already the winning override.

At the start of every run, the patcher builds its official baseline from the base game, DLC, `SkyrimVR.esm`, and optionally the installed entries from `Skyrim.ccc`. It then intersects that set with the final Synthesis load order, so missing plugins are ignored. The **Treat Creation Club content as official baseline** policy is enabled by default to preserve the original behaviour; disabling it makes Creation Club conflicts eligible for forwarding like ordinary mods.

The **Editor IDs (EDID)** policy offers three behaviors. **Preserve the official Editor ID** restores the EDID from the latest plugin in the configured official baseline. This keeps official identifiers stable when mods rename them. **Forward Editor ID changes** retains normal field forwarding. **Forward only on an existing patch record** (the default for new configurations) forwards an EDID when some other property also needs an override, but never creates an EDID-only patch record.

Migration note: EDID policy selection is generalized in the shared record-processing path for every supported record type. Record-specific property handlers and all non-EDID fields remain specialized as before; only the final EDID decision is governed centrally so there is one implementation path.

The **Tamriel persistent cell** policy applies only to `000D74:Skyrim.esm`, the persistent CELL for the Tamriel worldspace. This record is frequently included wholesale by otherwise unrelated mods, which can repeatedly restore older CELL states. The recommended hybrid option uses normal property forwarding unless the winning CELL header is equivalent to Skyrim.esm; in that case it copies Dawnguard.esm's header instead. The other options always copy Dawnguard.esm, copy Skyrim.esm, copy the winning override, or always use normal property forwarding. Header equivalence and copying deliberately exclude persistent and temporary references, landscape, navigation meshes, and group metadata. This specific rule takes precedence over ordinary early exits, while an explicit Priority Mods rule remains authoritative; all other CELL records are unaffected.

The **Master Rules** tab supports intentional overwrite relationships that plugin headers do not declare. Each rule names one plugin to treat as a master and the target mods that receive that authority. New configurations include curated rules for USSEP-aware mods and Creation Club overhauls that rely on `Unofficial Skyrim Creation Club Content Patch.esl`. Empty rules have no effect and can be populated later. The **Restore default rules** button replaces the edited list with a fresh copy of that default set. Rules are compiled once per run into a target-to-virtual-masters lookup; imported plugin headers are never modified. If the relationship is already present in a target plugin's real masters list, the rule is harmless and does not add or duplicate header entries.

## Build and publish

For regenerating the ignored Mutagen and xEdit references on Windows or Linux,
see [Rebuilding external reference sources](REFERENCE_SOURCES.md).

On Windows, run the existing launcher from the repository root for a clean Release build, test run, and self-contained publish:

```bat
.\Build-Standalone.cmd
```

The launcher permits this repository's PowerShell script to run for that process only; it does not change the system execution policy. The script removes only the known generated `bin`/`obj` directories and the canonical `artifacts\DreadsMashedPatch-win-x64` publish directory. It then restores packages, builds the solution, runs the tests, and publishes the current standalone executable.

On Linux, install the .NET 10 SDK and run:

```bash
bash ./Build-Standalone.sh
```

The Linux script requires .NET 10. It uses the SDK on `PATH` if compatible, otherwise it checks the repository's `.tools/dotnet` installation. It prints the selected SDK and reports a clear error if neither is compatible. This avoids selecting a system .NET 9 SDK when a local .NET 10 SDK is available.

The Linux script restores, builds, tests, and publishes the same `win-x64` Windows executable to `artifacts/DreadsMashedPatch-win-x64`. It does not clean `bin` or `obj`. `EnableWindowsTargeting` lets the SDK obtain Windows targeting packs on Linux; it does not make WPF a native Linux UI. Run the published executable through Wine or Proton to test the interface.

Build and run tests on Windows:

```powershell
dotnet build "DreadsMashedPatch.sln"
dotnet test "DreadsMashedPatch.Tests/DreadsMashedPatch.Tests.csproj"
```

On Linux, `dotnet test DreadsMashedPatch.Tests/DreadsMashedPatch.Tests.csproj` runs the platform-neutral patcher tests without building the WPF app. `dotnet build DreadsMashedPatch.sln` also compiles the Windows-targeted WPF app when its targeting packs are available.

Publish a self-contained 64-bit Windows executable:

```powershell
dotnet publish "DreadsMashedPatch.App/DreadsMashedPatch.App.csproj" -c Release -o "artifacts/DreadsMashedPatch-win-x64"
```

Trimming is deliberately disabled because the patcher and Mutagen use reflection. Native WPF dependencies are bundled for extraction by the self-contained executable.

## Implementation note

- Generalized: runtime settings, record-family selection, diagnostics configuration, standalone state creation, and output writing are exposed through the desktop host.
- Specialized: every existing record and property handler remains the only implementation for its record-specific forwarding behavior.
- Reason: the UI configures and invokes the established patch path; it does not duplicate forwarding logic.
