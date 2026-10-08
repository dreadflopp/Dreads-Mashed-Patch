# Standalone application development

[Developer documentation](README.md#developer-documentation) · [Development guide](DEVELOPMENT.md) · [User setup guide](../README.md#getting-started)

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

Both application windows use the shared `UiFontFamily` fallback sequence `Segoe UI, Tahoma, Arial` at 14 device-independent pixels for body text. Logs and multiline plugin/keyword fields use `Consolas, Courier New, Liberation Mono, DejaVu Sans Mono` at 13 pixels; record signatures and elapsed time use the same monospace resource. WPF chooses available fonts from Windows or the Wine/Proton environment. Windows normally supplies Segoe UI and Consolas; Wine supplies Tahoma, and Proton 10 includes Liberation-based Arial and Courier New replacements. The application does not redistribute font files. Missing glyphs can still use WPF's additional fallback fonts.

The shared window style uses display text formatting, grayscale antialiasing, layout rounding, and device-pixel snapping. Grayscale avoids dependence on the prefix's ClearType/subpixel configuration. Buttons, fields, tabs, headings, and help text share spacing rules. System file dialogs, message boxes, and window decorations retain platform-provided typography because WPF application styles do not control those surfaces.

Typography migration note: both windows retain one typography/rendering style and shared UI/monospace resources. Font selection now uses explicit system fallback lists, replacing embedded font references. All bundled font files, their licenses, and font-resource/license-copy project entries were removed. Rendering and spacing stay shared; the installation picker reuses the existing section-heading style, and system dialogs remain platform-managed. Temporary startup and run font diagnostics and their helper were removed after font behavior was confirmed working. No record handlers or flag policies changed.

Browse migration note: game, input, output, and plugins.txt browsing share one path-input and dialog-fallback implementation; the separate raw `InitialDirectory` assignments were removed. Master-rule import/export dialogs retain their existing behavior because they do not accept pasted starting-directory fields. Game-folder inference uses the same safe path handling. Record handlers and flag policies are unchanged.

Path/output migration note: full runs and empty-output creation now share one explicit destination setting and the existing output transaction. The Data folder setting remains separate; Synthesis still handles imports, deduplication, load-order trimming, and output splitting. Creation Club file parsing is shared by preparation and verification. No record handlers or flag policies changed. The old output destination derived from the Data folder and obsolete Creation Club parsing loop were removed.

Migration note: EDID policy selection is generalized in the shared record-processing path for every supported record type. Record-specific property handlers and all non-EDID fields remain specialized as before; only the final EDID decision is governed centrally so there is one implementation path.

Proton validation evidence: the earlier bundled-font version's startup and General-tab rendering were smoke-tested under Proton 10.0 in a fresh prefix at the default window size. The maintainer subsequently confirmed the current system-font fallback version's fonts are working; this does not verify patching or mod-manager integration.
