# Mashed Patch standalone application

Mashed Patch is a Windows desktop patcher. It is not intended to be added to or launched by the Synthesis application. The desktop host uses Mutagen and the Synthesis pipeline libraries internally to construct the load order and write the output plugin, while retaining one patch implementation in `DreadsMashedPatch.Program.RunPatch`.

## Running

1. Launch `DreadsMashedPatch.exe`.
2. Confirm the Skyrim game folder, plugin input (Data) folder, active `plugins.txt`, and patch output folder on the **General** tab. Select **Verify paths** and review the report.
3. Choose record families and forwarding policies.
4. Select **Run patcher**. Settings are saved automatically before the run.

Settings are stored in the Windows user-local application data folder (`%LOCALAPPDATA%\DreadsMashedPatch`). In Proton, this is inside the prefix. The **Save settings** button writes settings without running the patcher.

Run output is stored in `%LOCALAPPDATA%\DreadsMashedPatch\Logs`. `DreadsMashedPatch-current.log` contains the latest run. At the start of the next run it becomes a timestamped historical log. The configurable retention limit defaults to ten historical logs, and the oldest logs are removed automatically.

### Mod Organizer 2

Add `DreadsMashedPatch.exe` to MO2 as an executable and launch it through MO2. For Wabbajack lists, select the list's Stock Game folder and its Data folder, then select the active MO2 profile's `plugins.txt`. Select the virtual Data folder as the output folder to use MO2's configured output mod or **Overwrite**, or explicitly select a dedicated output mod folder.

### Wabbajack and Stock Game paths

Some lists launch a copied game installation, often named **Stock Game**, rather than the official Steam/GOG installation. Both the game folder and input Data folder must refer to the installation your list actually launches. **Detect paths** locates registered installations and may select the official installation instead. The highlighted notice on the General tab explains this; **Verify paths** can identify missing plugins but cannot prove which installation your manager launches.

### Vortex and deployed installations

Run the executable normally after deployment. Confirm that the game folder, Data folder, and `plugins.txt` belong to the active deployment/profile.
Choose a separate patch output folder, then install its generated plugins together as a dedicated output mod. The mod manager's staging root is not an output mod folder.

### Proton and Amethyst

Launch the Windows executable in a Proton prefix that can access the selected game and profile paths, either directly or through Amethyst's external applications feature. Deploy the active profile first. Confirm the actual game folder (including a list's Stock Game copy), deployed Data folder, and active `plugins.txt` manually. Choose a separate output folder and install its generated plugins together as an output mod in Amethyst; do not write directly into the staging root. WPF behavior under Proton has not yet been verified.

### Output and path verification

The output folder is independent of the plugin input folder and is required for both a full run and **Create empty patch output**. Existing settings from before this option was introduced require an explicit selection; the app does not silently reuse the input or staging folder. Both the run log and completion message identify the chosen destination. Only previous patch outputs in that destination are replaced; copies in other directories are untouched.

**Verify paths** produces a read-only report on the General tab showing the discovered `.esm`, `.esp`, and `.esl` files and each `plugins.txt` entry's enabled/disabled and found/missing status. Missing enabled inputs are errors; unlisted files are warnings because they may be intentionally inactive. Installed implicit masters and Creation Club plugins are accounted for, and output/later listings are marked as excluded by the patch's load-order cutoff. Duplicate listings and filenames are reported. Changing any selected path or game release invalidates the displayed report. Verify again after changing the manager's deployment or profile, even if the path strings remain the same.

Listings are parsed with Mutagen, and plugin identities are compared using `ModKey`. Filesystem checks are separate: a case-insensitive name match does not guarantee that the expected path exists on a case-sensitive filesystem. The report identifies such discrepancies without renaming files. This check does not import records, inspect master dependencies, validate output write permissions, or confirm the manager's launch target.

Pasted path text can include surrounding double quotes or mixed `/` and `\` separators. The app normalizes these for its runtime. Under Wine/Proton, existing Unix absolute paths are mapped through `Z:` when that mapped file or folder is accessible. All four path Browse buttons share the same normalization and use an absolute existing starting directory. If the shell dialog rejects it, Browse retries once without a starting directory; a persistent failure produces a handled error message. Clearing a path field also opens Browse without a starting directory.

Browse migration note: game, input, output, and plugins.txt browsing share one path-input and dialog-fallback implementation; the separate raw `InitialDirectory` assignments were removed. Master-rule import/export dialogs retain their existing behavior because they do not accept pasted starting-directory fields. Game-folder inference uses the same safe path handling. Record handlers and flag policies are unchanged.

## Record selection

Every supported record family is enabled by default. The UI groups them by the four-character signatures used by xEdit, such as `QUST` for quests. Settings store only disabled families, so record families added in future versions are automatically enabled. The list supports search, arrow-key navigation, and Space to toggle the selected row. `Ctrl+F` focuses the search box.

## Diagnostics

The Run Log tab shows bounded progress, warnings, and errors while the complete output is written live to `DreadsMashedPatch-current.log`. Debug mode enables context-change logging and optional deep dives by xEdit record signature or FormKey without placing the full diagnostic stream in the UI. A matched deep-dive record logs every registered property. Broad Detailed logging remains available as a developer-only configuration because enabling it for every record can produce extremely large log files. Deep-dive selectors accept commas, semicolons, or one value per line.

The game release is selected explicitly and passed to Mutagen/Synthesis. Anniversary Edition uses the corresponding Special Edition Steam or GOG selection. Mutagen requires this value for implicit masters, load-order parsing, and binary defaults. Creation Club listings are read explicitly from `Skyrim.ccc` in the selected game folder, merged with `plugins.txt`, and deduplicated by the Synthesis pipeline.

The primary output name is fixed as `MashedPatch.esp`. If the patch needs more than 254 masters, Synthesis automatically splits it into additional numbered plugins such as `MashedPatch_2.esp`. If the primary output already appears in the selected load order, Synthesis reads only enabled plugins placed before it. If it is absent, Synthesis reads the complete enabled load order. Each run writes new output into a temporary folder inside the selected output folder, then replaces the previous primary and numbered outputs after writing succeeds. A failed patch build leaves the previous output files in place.

Use **Create empty patch output** on the General tab before the first full run when the plugin must be positioned in a mod manager. The action removes the primary and recognized split outputs, then writes one empty, masterless plugin at the stable primary filename.

Path/output migration note: full runs and empty-output creation now share one explicit destination setting and the existing output transaction. Plugin input remains separate; Synthesis still handles imports, deduplication, load-order trimming, and output splitting. Creation Club file parsing is shared by preparation and verification. No record handlers or flag policies changed. The old input-derived output destination and obsolete Creation Club parsing loop were removed.

Plugins on the **Priority Mods** tab win at record scope. If another plugin overwrites one of their records, Mashed Patch copies the complete record snapshot from the matching priority mod occurring last in the configured list instead of merging individual properties. No patch record is needed when that selected source is already the winning override.

At the start of every run, the patcher builds its official baseline from the base game, DLC, `SkyrimVR.esm`, and optionally the installed entries from `Skyrim.ccc`. It then intersects that set with the final Synthesis load order, so missing plugins are ignored. The **Treat Creation Club content as official baseline** policy is enabled by default to preserve the original behaviour; disabling it makes Creation Club conflicts eligible for forwarding like ordinary mods.

The **Editor IDs (EDID)** policy offers three behaviors. **Preserve the official Editor ID** (the default) restores the EDID from the latest plugin in the configured official baseline. This is a defensive compatibility measure because some game behavior can unexpectedly resolve an EDID rather than only a FormID; renamed EDIDs have caused dialogue and NPC speech failures in released mods, as documented in the [Scion changelog](https://www.nexusmods.com/skyrimspecialedition/mods/41639?tab=logs). **Forward Editor ID changes** retains normal field forwarding. **Forward only on an existing patch record** forwards an EDID when some other property also needs an override, but never creates an EDID-only patch record.

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
