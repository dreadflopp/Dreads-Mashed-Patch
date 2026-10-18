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

Both application windows use the shared `UiFontFamily` resource at 14 device-independent pixels for body text. `Styles/TypographyResources.cs` enumerates installed font-family names and selects the first candidate from Segoe UI, Tahoma and Arial whose resolved glyph typeface also matches the requested name. Logs and multiline plugin/keyword fields use the same selection method with Consolas, Courier New, Liberation Mono and DejaVu Sans Mono at 13 pixels; record signatures and elapsed time share the monospace resource. The resources are created before shared styles resolve them. Windows normally supplies Segoe UI and Consolas; the isolated Proton 10 prefix selects Tahoma and Courier New. If none of a resource's preferred families is installed, it uses the system message font. The application does not redistribute font files. Missing glyphs can still use WPF's additional fallback fonts.

Proton font correction (2026-10-18): passing a comma-separated list beginning with a missing Segoe UI allowed Wine to substitute Times New Roman before the intended Tahoma fallback. Wine also enumerates some aliases as available families, so enumerating names alone is insufficient; the resolved glyph typeface must match. The shared resource provider replaces those lists with validated installed-family selection, preserving Windows preferences while avoiding missing-family substitution under Proton. There is one font-selection implementation for both UI and monospace resources; no platform-detection branch, bundled font or parallel control style was added.

The shared window style uses display text formatting, grayscale antialiasing, layout rounding, and device-pixel snapping. Grayscale avoids dependence on the prefix's ClearType/subpixel configuration. Buttons, fields, tabs, headings, and help text share spacing rules. System file dialogs, message boxes, and window decorations retain platform-provided typography because WPF application styles do not control those surfaces.

Checkbox alignment (2026-10-17): one shared CheckBox template centers the square and label in a common Grid. Proton 10's Classic template otherwise positions its glyph against the first content line while the content is centered in a taller control; an empty Record Types checkbox has no content line and its square starts at the top of the 30-pixel row. A transparent outer Border retains the full clickable area. Centering only the native BulletDecorator was rejected because it reduced that area. The template follows WPF's existing Grid-based checkbox pattern, uses shared palette brushes (system colors in high contrast) and vector marks for checked/unchecked/indeterminate/disabled states, and retains the CheckBox control's keyboard/access-key/automation behavior with the shared keyboard focus visual. The glyph has its own foreground so selecting a record row cannot turn the mark white on a white square. Fonts, record handlers and flag policies are unchanged.

Alignment validation: an isolated WPF rendering probe loaded the application resources under Proton 10.0 in a fresh prefix. It reproduced squares about 4 pixels too high beside settings labels and 8 pixels too high in Record Types rows. After the fix, all eight sample checkbox glyphs align within 0.5 device-independent pixels of their labels or empty control centers. Checked, unchecked, indeterminate and disabled states were inspected visually; hit tests at the top, center and bottom of enabled labeled controls succeed, while disabled controls reject them. The WPF app builds with zero warnings/errors. This is a targeted rendering/input-layout check, not a full interactive Windows/Proton or mod-manager test.

Typography migration note: both windows retain one typography/rendering style and shared UI/monospace resources. Font selection uses installed system families, replacing the former comma-separated lists and earlier embedded font references. All bundled font files, their licenses, and font-resource/license-copy project entries were removed. Rendering and spacing stay shared; the installation picker reuses the existing section-heading style, and system dialogs remain platform-managed. Temporary startup and run font diagnostics and their helper were removed after font behavior was confirmed working. No record handlers or flag policies changed.

### Shared control styling on Windows and Proton

The [2026-10-17 styling review](UI-STYLING-REVIEW-2026-10-17.md) led to one shared set of WPF client-area templates in `DreadsMashedPatch.App/Styles/Controls.xaml` and `Scrolling.xaml`. Buttons retain their original control behavior and share a template between neutral and accented primary variants. Hover, pressed, keyboard focus, default-action and disabled states are explicit; disabled primary actions use a neutral surface and readable muted text. Text fields retain `PART_ContentHost`, combo boxes retain selection templates/selectors, editable text and popup parts, and scrollbars retain tracks, thumbs, line/page commands and system size metrics. The progress indicator retains WPF value sizing and pulses when indeterminate. Lists, tabs, expanders and tooltips share the same palette.

`App.xaml` merges the palette and controls once. The former inline styles were removed. Both windows use dynamic brush references. General offers **Light theme** and **Dark theme**, using plum surfaces and accents with green primary actions and information panels in `LightTheme.xaml` and `DarkTheme.xaml`. These replace the earlier Breeze color values within the same two dictionaries. Primary actions use white text on green in both modes; help badges reuse the plum panel/focus brushes to fit their field labels and keep green for actions and information panels. No additional palette or template path was added. Light is the default for new and older settings. The `StandaloneSettings.Theme` enum uses the existing normalization, property notification and JSON save/load paths; Save settings and Run patcher persist it with the other choices. Loading settings applies the saved palette, and changing the selector updates the current windows immediately without reloading settings or invalidating path verification. The shared settings-load helper removes the previous settings subscription before attaching the current one.

`App.SetTheme` and the existing palette-switching method provide one implementation for applying the selected theme. OS high contrast takes priority through `HighContrast.xaml`; the selected light/dark preference remains stored, so leaving high contrast restores it. A system high-contrast change swaps only the palette on the UI dispatcher. All palettes provide matching brush keys, including a separate input background for light/dark text fields. The contrast palette supplies matching system surface/text pairs. Native title bars, context menus and operating-system dialogs retain platform styling. Existing fonts and the centered checkbox layout remain shared.

General-tab path examples now appear in four non-action help badges' tooltips, with keyboard-focus display and accessible names/descriptions. Each description is a window resource shared by its tooltip and automation help, avoiding duplicate text. Examples use named starting folders and arrows between subfolders instead of machine-specific absolute paths; shared guidance explains how to follow the folder steps. Essential setup notices and path verification remain visible. Tab pages stretch while headers center independently. Master Rules uses a bounded, proportional rule list and a separately scrollable editor; its virtual-master field grows with available space and retains a usable minimum. Picker paths wrap and inherit the selected row's foreground when selected.

Focus appearance (2026-10-18): the shared keyboard indicator is a neutral one-pixel outline immediately outside the control. Text fields use their existing one-pixel border for focus; the extra inner `FocusRing` and its trigger were removed. Keyboard navigation, help-badge focus and the Record Types list's selection and Space handling remain available. Other palette studies remain review artifacts; the only runtime choices are Light theme and Dark theme, plus the OS high-contrast override.

Checkbox and navigation correction (2026-10-18): the shared CheckBox style explicitly resolves its foreground through the active palette, overriding the native theme's dark text. The square and content presenter center independently within the full control height; the transparent hit area and independent checkmark color remain. One shared bubbling-key handler is registered for Window and ScrollViewer; it consumes only otherwise unhandled arrows, before page scrollers or WPF can use them for spatial focus movement. Scrollers belonging to text-editor and item-control templates defer to those input controls, retaining their own editing/selection keys. Tab/Shift+Tab focus traversal and existing shortcuts remain. A window-only navigation property or handler is insufficient because page ScrollViewers also move focus; the property approach was removed instead of retained as a second policy. The existing main-window shortcut handler remains in the preview phase because its purpose differs from handling otherwise unhandled arrows. Expanders expose WPF's named `HeaderSite` ToggleButton as a tab stop and group their header/editor traversal locally, so Space expands or collapses the focused header.

Migration boundary: presentation templates, colors and layout were generalized; the original UI event handlers, settings models, services and record/property/flag handlers were intentionally retained. Native templates behind the changed client controls and the obsolete inline primary-button hover rule were replaced, rather than kept as parallel implementations. No record types were migrated. Validation and remaining platform checks are recorded in the styling review's implementation follow-up.

Browse migration note: game, input, output, and plugins.txt browsing share one path-input and dialog-fallback implementation; the separate raw `InitialDirectory` assignments were removed. Master-rule import/export dialogs retain their existing behavior because they do not accept pasted starting-directory fields. Game-folder inference uses the same safe path handling. Record handlers and flag policies are unchanged.

Path/output migration note: full runs and empty-output creation now share one explicit destination setting and the existing output transaction. The Data folder setting remains separate; Synthesis still handles imports, deduplication, load-order trimming, and output splitting. Creation Club file parsing is shared by preparation and verification. No record handlers or flag policies changed. The old output destination derived from the Data folder and obsolete Creation Club parsing loop were removed.

Partial-publication decision (2026-10-10): full runs publish successfully processed records by default after discarding failed candidates. There is no toggle. Both hosts share structured fatal/record-error reporting; the desktop publication gate rejects pipeline errors and missing outputs, while record diagnostics remain visible without rejecting partial output. The completion status, dialog, and log report skipped records. The previous desktop gate that rejected every error line was replaced rather than kept alongside the new gate. Record transactions, primary/split replacement, and approved flag handlers remain unchanged.

Migration note: EDID policy selection is generalized in the shared record-processing path for every supported record type. Record-specific property handlers and all non-EDID fields remain specialized as before; only the final EDID decision is governed centrally so there is one implementation path.

Proton validation evidence: the earlier bundled-font version's startup and General-tab rendering were smoke-tested under Proton 10.0 in a fresh prefix at the default window size. The maintainer subsequently confirmed the current system-font fallback version's fonts are working; this does not verify patching or mod-manager integration.
