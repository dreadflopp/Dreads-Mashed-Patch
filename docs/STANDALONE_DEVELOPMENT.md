# Standalone application development

[Internal index](README.md) · [Development guide](DEVELOPMENT.md) · [User setup guide](../README.md#getting-started)

Build instructions and implementation constraints for AI agents. User-facing settings are explained in the [project README](../README.md#settings).

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

## Host boundary

The desktop host configures and invokes the core patcher. Record/property handlers
remain the only forwarding implementation; do not reproduce selection logic in
WPF services. For errors and publication use the
[shared output contract](DEVELOPMENT.md#output-and-diagnostics).

## Typography on Windows and Proton

Both windows use `Styles/TypographyResources.cs` before shared styles resolve.
It enumerates installed font families and verifies the resolved normal glyph
face matches the requested family; Wine can enumerate a missing Segoe UI alias
that actually resolves to Times New Roman. A comma-separated fallback list or
name enumeration alone reintroduces this bug.

UI preference is Segoe UI, Tahoma, Arial at 14 device-independent pixels;
monospace preference is Consolas, Courier New, Liberation Mono, DejaVu Sans Mono
at 13. If none is installed, use the system message font. The app distributes no
fonts and needs no platform-detection branch. An isolated Proton 10 prefix
selected Tahoma/Courier New. Missing glyphs can still use WPF fallback fonts.

Shared window styles use display formatting, grayscale antialiasing, layout
rounding and pixel snapping. System dialogs and window decorations remain
platform-managed. Keep one typography implementation for both windows.

## Control and theme constraints

`App.xaml` merges one set of client-area templates in `Styles/Controls.xaml` and
`Scrolling.xaml`. Preserve framework contracts: TextBox PART_ContentHost,
ComboBox selection/editable/popup parts, scrollbar tracks/thumbs/commands/system
metrics, progress value sizing and indeterminate animation. Explicit hover,
pressed, default-action, disabled and keyboard-focus states are needed under
Proton's Classic theme as well as Windows.

- LightTheme/DarkTheme supply the approved plum surfaces/accents and green primary
  actions/information panels; primary actions have white text. Help badges use
  plum panel/focus brushes. Do not restore the retired Breeze or preview palettes.
- Light is the default. StandaloneSettings.Theme uses the existing normalization,
  notification, JSON persistence and settings-load subscription lifecycle.
  Changing theme recolors both windows without invalidating path verification.
- All palettes have matching brush keys. OS high contrast overrides the selected
  palette through system surface/text pairs on the UI dispatcher, while retaining
  the saved light/dark preference. Keep a separate input background role.
- CheckBox square and content center independently across the full height; a
  transparent outer Border preserves the full hit area. Palette foreground must
  override the native theme. The glyph has independent color so a selected row
  cannot make it white on white. Preserve indeterminate/disabled states, access
  keys, automation and Space activation. Native BulletDecorator centering alone
  reduced the hit area and is not an equivalent implementation.
- Expanders expose the WPF-recognized HeaderSite ToggleButton as a tab stop and
  group header/editor traversal locally. Tab-page stretching is separate from
  centered tab-header content. Master Rules needs a bounded list and scrollable
  editor; picker paths wrap and inherit the selected row's text brush.
- General path help uses four non-action, focusable badges. Each description is
  shared between tooltip and automation help. Use named starting folders and
  arrows between subfolders; essential setup notices stay visible.
- Record rows show names inline without duplicate name tooltips. A user reported
  Win32Exception 1412 in WPF's PopupControlService tooltip teardown while scrolling
  this list under GE-Proton 11.7 via Amethyst. Removing these redundant popups is
  a targeted workaround; the exact failure has not been reproduced locally.
  Keep useful help tooltips and avoid broadly swallowing popup exceptions.

## Keyboard and focus constraints

One shared bubbling-key handler for Window and ScrollViewer consumes only
otherwise unhandled arrows. Template scrollers for text/item controls defer to
those controls so caret, list and dropdown input still works. A window-only
navigation setting is insufficient because ScrollViewer moves focus first.
Existing preview shortcuts have a different role; do not block all arrows in a
preview handler or replace WPF Tab/Shift+Tab traversal.

The shared focus outline is one neutral pixel outside the control; text fields
use their existing editing border. Selection, validation, hover and dropdown-open
cues are separate from navigation focus. `Styles/FocusVisualState.cs` observes
keyboard/mouse preprocessing, including popup input, without handling events or
moving focus. Mouse clicks hide the outline even when KeyboardCues is true;
keyboard input restores it. Do not override WPF caches in a probe to hide the
host setting: that missed a shipped Proton mouse-focus failure. Verify first Tab,
Shift+Tab, clicking an already-focused button, popup input and persistent record
selection without duplicate outlines.

Source references: [WPF expander contract](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/expander-styles-and-templates),
[ScrollViewer key handling](https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/ScrollViewer.cs).

## Validation scope

The platform-neutral tests cover [theme persistence](../DreadsMashedPatch.Tests/StandaloneThemeTests.cs),
[path inputs](../DreadsMashedPatch.Tests/PathInputTests.cs) and
[output paths](../DreadsMashedPatch.Tests/StandaloneOutputPathTests.cs), not WPF rendering.
For UI changes, build/publish and exercise actual resources and compiled windows
at default/minimum sizes in both themes, including disabled/selected states,
keyboard input, popup behavior and saved-settings restoration. Use glyph output
and rendered geometry rather than trusting configured font names alone.

Isolated Proton 10 probes on 2026-10-18 checked compiled tabs, picker, themes,
fonts, checkbox geometry/hit areas and settings persistence. The final mouse-focus
correction passed 28 native-input checks in both themes with the host's actual
KeyboardCues setting; it supersedes earlier probes using private cache overrides.
Relevant ignored local artifacts, if still available, are
`artifacts/ui-input-font-fixes-2026-10-18/`,
`artifacts/ui-plum-green-implementation-2026-10-18/` and
`artifacts/ui-mouse-focus-fix-2026-10-18/`.
These are optional local evidence, not portable build/test dependencies.

The probes used temporary prefixes/fake paths, restored settings and did not run
patching or mod-manager discovery. Native Windows rendering/input, varied DPI,
live OS high-contrast switching and real mod-manager integration remain unverified.
The maintainer confirmed system-font fallback works; that does not establish
patching correctness. Current build/test commands are above and in the
[development guide](DEVELOPMENT.md#validation).
