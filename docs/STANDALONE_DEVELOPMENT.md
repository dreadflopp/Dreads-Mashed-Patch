# Standalone application development

[Developer documentation](README.md#developer-documentation) · [Development guide](DEVELOPMENT.md) · [User setup guide](STANDALONE_APP.md)

Build, implementation, and migration details for contributors. User-facing settings are explained in the [project README](../README.md#settings).

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

### Typography on Windows and Proton

Both application windows use embedded, static TrueType Noto Sans fonts at 14 device-independent pixels for body text. Logs and multiline plugin/keyword fields use Noto Sans Mono at 13 pixels; record signatures and elapsed time use the same embedded monospace family. Regular, semibold, bold, and italic UI faces and regular/semibold monospace faces are included, so these styles do not require installing fonts in Windows or a Proton prefix. Glyphs outside the bundled families' coverage still use WPF font fallback.

The shared window style uses display text formatting, grayscale antialiasing, layout rounding, and device-pixel snapping. Grayscale avoids dependence on the prefix's ClearType/subpixel configuration. Buttons, fields, tabs, headings, and help text share spacing rules. System file dialogs, message boxes, and window decorations retain platform-provided typography because WPF application styles do not control those surfaces.

Typography migration note: both windows share one typography/rendering style; all former Consolas assignments use the bundled monospace resource or shared textbox style. The installation picker reuses the existing section-heading style. System dialogs remain platform-managed; patch logic and record-specific handlers retain their existing implementation. Font sources and the redistribution license are in `DreadsMashedPatch.App/Assets/Fonts`; the license is also embedded and copied alongside published output.

Browse migration note: game, input, output, and plugins.txt browsing share one path-input and dialog-fallback implementation; the separate raw `InitialDirectory` assignments were removed. Master-rule import/export dialogs retain their existing behavior because they do not accept pasted starting-directory fields. Game-folder inference uses the same safe path handling. Record handlers and flag policies are unchanged.

Path/output migration note: full runs and empty-output creation now share one explicit destination setting and the existing output transaction. The Data folder setting remains separate; Synthesis still handles imports, deduplication, load-order trimming, and output splitting. Creation Club file parsing is shared by preparation and verification. No record handlers or flag policies changed. The old output destination derived from the Data folder and obsolete Creation Club parsing loop were removed.

Migration note: EDID policy selection is generalized in the shared record-processing path for every supported record type. Record-specific property handlers and all non-EDID fields remain specialized as before; only the final EDID decision is governed centrally so there is one implementation path.

Proton validation evidence: startup and General-tab rendering were smoke-tested under Proton 10.0 in a fresh prefix at the default window size; this does not verify patching or mod-manager integration.
