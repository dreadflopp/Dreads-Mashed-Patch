# Windows and Proton UI styling review — 2026-10-17

This is an investigation and proposed design, not an implemented UI change. Production source and configuration were left unchanged from the start of this review. Earlier uncommitted changes, including the checkbox alignment template, were preserved.

## Scope and evidence

Reviewed `DreadsMashedPatch.App/App.xaml`, all eight main-window tabs, and the installation picker. An isolated, offscreen WPF harness rendered the actual compiled windows and application resources under Proton 10.0 at the default 1040×760 and minimum 820×620 main-window sizes. It supplied fake paths, detached startup loading, and did not run patching, path detection, or settings persistence. Additional captures exercised button, text-field and combo-box visual states, active selection, and the running/disabled footer. State properties were controlled by the harness; this was not a full interactive keyboard or assistive-technology test.

Two layout concepts were applied only to the harness's in-memory visual tree: compact path-help badges and a stretched Run Log page. The harness and its build products were kept outside production source. It published successfully. SHA-256 comparison against the source/configuration baseline found no changes during the investigation. Patcher regression tests were not rerun for this documentation-only review.

The Windows Aero2 theme dictionary was also loaded into the harness to inspect native-template interactions. This is a Windows-template replay under Proton, **not a native Windows test**. Actual Windows, additional Proton versions, DPI settings, high contrast, and the user's particular prefix remain to be checked. The isolated prefix's font substitutions are not evidence of a font regression in an existing user prefix.

Local screenshots and observations are saved in the ignored directory `artifacts/ui-styling-review-2026-10-17/`. They are review artifacts, not release assets. The scratch harness used `/tmp/ui-styling-review-i_i5n9v_/`.

## Findings and recommendations

### Raised buttons and inset fields

The apparent opposite-direction drop shadows are native Classic-theme bevels. The app does not declare a drop-shadow effect. The framework's Classic button template uses a raised border; its text box uses a sunken border. These are deliberate native theme choices. The app currently combines them with custom pastel panels and a green primary button, so the overall treatment is inconsistent.

`PrimaryButton` still uses the native button template. Its custom green background/border changes the appearance of that same bevel; it is not a separate shadow-free template. Only this style supplies an explicit hover color. Ordinary buttons retain Classic's unchanged hover fill.

Recommendation: keep green as the primary-action color and introduce one shared button template for primary and secondary variants. Use a flat border with explicit normal, hover, pressed, keyboard-focus, default-action and disabled states. Give secondary buttons a modest hover change, with a separate visible focus indicator. Do not remove state feedback while removing bevels.

### Run patcher text and cross-platform state behavior

The normal enabled primary button resolves to white text on `#2E8052`, approximately 4.85:1 contrast. Its darker green hover has approximately 6.89:1 contrast. The review did not reproduce an incorrect normal-state foreground color. Apparent differences may also involve font substitution and rendering in a particular prefix.

There is a confirmed disabled-state problem in the tested Proton theme: the native template changes text to `#6A6A6A` but leaves the button green, yielding approximately 1.11:1 contrast. This is visible when the main window is placed in its running state without invoking a patch run.

The Aero2 replay exposes a different interaction: on hover the control's background property becomes dark green, but the template's inner border paints `#BEE6FD`, retaining white text. That combination has approximately 1.32:1 contrast. A color setter alone therefore does not reliably control the final painted state across native themes.

Recommendation: define the primary button's painted states in the shared template. Keep white text on green when enabled; use a neutral disabled surface with readable muted text. Treat pressed, focus and disabled as separate states. Apply the same policy to the installation picker's primary action. A native Windows smoke test is still required before shipping.

### Compact General-tab path help

Replace the four large path-description/example blocks with a small outlined information badge beside each path label. The badge has no click action. A tooltip contains that field's description and its Windows, Proton and mod-manager path examples. Keep essential instructions or validation errors visible rather than hiding them in tooltips.

Use an enabled, focusable label or equivalent information element with an accessible name; do not use a disabled button, which is a poor hover/focus help target. Supply a visible keyboard-focus cue, wrapped tooltip content and tooltip display on both hover and keyboard focus (`ToolTipService.ShowsToolTipOnKeyboardFocus`). A small `?` or information glyph should look like help, without pressed or action-button styling. Check actual screen-reader exposure and keyboard tooltip behavior during implementation.

In the isolated layout concept, moving these four blocks reduced the General page's scroll extent from 1126 to 782 device-independent pixels, a reduction of 344 pixels, or approximately 31%, at the same 509-pixel viewport. This does not eliminate scrolling or change the two top setup notices. Standardize those notices' differing border thicknesses; consolidate repeated setup prose only if essential MO2 and Stock Game guidance remains clear.

### Tab headers inadvertently control page layout

The shared `TabItem` style sets `VerticalContentAlignment="Center"`. WPF copies the selected tab item's content alignment to the selected-page presenter, so this centers whole pages as well as affecting headers.

The Run Log page measured only 78 pixels high in the default window, leaving most of the tab empty and very little space for log text. Setting the selected page's alignment to stretch in memory expanded it to 521 pixels. Priority Mods and Ignored Mods also use less than the available page height and appear vertically centered.

Recommendation: stretch tab pages and center tab headers independently in their header presentation. Confirm that each page fills its intended space and that short forms start at the top. Preserve correct behavior when the tab headers wrap at minimum width.

### Master Rules editor clips

The right-side editor combines fixed-height content, including a 230-pixel text box, with no enclosing page scrolling. Content extends beyond the panel even at the default window size; at minimum size, substantial editor content and help become inaccessible. This needs more than a palette adjustment.

Recommendation: use available-height grid rows for the editor, sensible minimum sizes, and scrolling where the form cannot fit. Revisit the fixed 260-pixel left column at narrow widths. Test with an actual rule selected so all editor controls and help are reachable at minimum size and increased text/DPI scaling.

### Installation-picker selection

The installation name inherits the selected item's white foreground, but the path is explicitly colored with `MutedTextBrush` (`#675860`). It remains dark on the active blue highlight. This mismatch was confirmed by inspecting the rendered text elements in an active selection.

Recommendation: provide foregrounds for both title and path in active and inactive selected states, using the appropriate selection colors. Allow long paths to wrap or trim with full-path help. Black margins and clipped edges in the picker review capture are an offscreen root-margin capture artifact, not a finding about that dialog's window layout.

### Remaining control consistency

| Area | Recommended treatment |
|---|---|
| Text boxes and combo boxes | Use matching flat borders, padding and heights, with distinct focus, disabled, read-only and validation states. Preserve text editing, selection, caret, IME, popup and keyboard behavior. |
| Field boundaries | Choose a darker neutral than the existing pale pink container outline. That outline has only approximately 1.41:1 contrast against the window background and should not be the sole boundary of a new flat input control. |
| Checkboxes | Retain the existing aligned template and full clickable area. Verify focus, hover, checked, unchecked, indeterminate and disabled states alongside the other controls. No further alignment replacement is proposed. |
| Tabs and lists | Apply consistent selected, hovered, focused and disabled visuals. Keep page layout independent of header appearance and preserve selection visibility when focus moves. |
| Expanders, scrollbars, tooltips and progress | Review them with the shared palette after buttons and fields. Avoid leaving these as visually unrelated Classic controls if the rest of the client area becomes flat. Keep scroll targets usable and tooltip text readable. |
| Typography and spacing | Keep shared font resources and existing fallback policy; verify resolved fonts in the intended prefixes. Normalize notice borders and local spacing. No new font dependency is justified by this review. |

Native title bars and operating-system file/folder dialogs can retain platform styling. A complete framework/theme-library migration is not needed for these changes.

## Suggested implementation order and validation

1. Fix tab-page stretching and Master Rules reachability; these affect available workspace and access to controls.
2. Add compact path-help badges with hover, focus and accessible descriptions.
3. Introduce shared button and field templates with complete state coverage; fix primary disabled text and picker selection text.
4. Bring remaining client controls into the same visual treatment, preserving the checkbox alignment fix.

Use the existing application resource dictionary and shared-style pattern. Reuse templates between button variants and avoid separate Windows/Proton visual implementations. WPF behaviors and accessibility should remain attached to the original control types. Replacing a native control template requires retaining its required parts and behavior; copying just its appearance is insufficient.

Before shipping, test the actual app on Windows and Proton at 100%, 125% and 150% scaling, at default and minimum window sizes. Cover all tabs, a populated Master Rules editor, long paths, wrapped headings, installation selection, enabled/running/disabled buttons and determinate/indeterminate progress. Exercise hover/press, Tab/Shift+Tab, Space/Enter, focus visibility, tooltip focus behavior, combo-box navigation, checkbox hit areas, text selection/copy/paste, screen-reader names, high contrast and system-color changes. Confirm that settings/path workflows still work and that merely viewing help triggers no action. These checks are proposed acceptance criteria, not completed results.

## Framework references

- [WPF Button templates](https://raw.githubusercontent.com/dotnet/wpf/main/src/Microsoft.DotNet.Wpf/src/Themes/XAML/Button.xaml): Classic raised border and Aero2 template state overrides.
- [WPF TextBox templates](https://raw.githubusercontent.com/dotnet/wpf/main/src/Microsoft.DotNet.Wpf/src/Themes/XAML/TextBox.xaml): Classic sunken border.
- [WPF TabControl implementation](https://raw.githubusercontent.com/dotnet/wpf/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/TabControl.cs): selected-page alignment copied from the selected `TabItem`.
- [WPF ToolTipService implementation](https://raw.githubusercontent.com/dotnet/wpf/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/ToolTipService.cs): keyboard-focus tooltip support.
- [Microsoft tooltip overview](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/tooltip).

Framework links track the upstream branch; the rendering measurements above describe the locally installed .NET 10 WPF runtime and Proton 10.0 used for this dated review.

## Implementation follow-up — 2026-10-17

The user subsequently authorized implementation and required preserving changes made since the investigation. Work started from clean commit `3e8a9e8`; source-hash comparison confirmed that only desktop presentation files and internal documentation changed. The newer record algorithms, settings, services and tests were preserved.

Implemented one shared client-control theme in `DreadsMashedPatch.App/Styles/`: a normal palette, a system-color contrast palette, common control templates and scrollbar templates. `App.xaml` now merges these dictionaries instead of retaining the old inline implementations. Primary and secondary buttons share a template and supply their own explicit state colors. Text boxes, combo boxes, lists, tabs, expanders, tooltips, progress and scrollbars no longer depend on the Classic/Aero2 chrome behind those controls. Control types, required editing/popup/track parts, display-name selectors, line/page scrolling commands and system scrollbar dimensions were retained. The existing checkbox geometry and full hit area remain; its mark color is independent of a selected row's label foreground.

The four General path-help badges have no action, support tooltip display on keyboard focus, and expose automation names and descriptions. Shared description strings feed the tooltip introduction and automation help; all original platform/mod-manager examples remain in the tooltip. Setup notices and verification stay visible. Selected tab pages stretch independently of centered headers. Master Rules retains a bounded list with proportional columns and scrolls its editor separately; the virtual-master input uses available height with a minimum instead of a fixed 230 pixels. Installation names and paths wrap, and both use the selected row's foreground when selected.

Startup chooses the appropriate palette and subscribes to system high-contrast changes; a change replaces the palette on the UI dispatcher. Both windows resolve brushes dynamically. Native window decorations, context menus and operating-system dialogs remain platform-managed. Font selection and patching behavior were not changed. The [standalone development guide](STANDALONE_DEVELOPMENT.md#shared-control-styling-on-windows-and-proton) describes the implementation boundary.

Validation:

- Release build/publish of the WPF app through the isolated probe: zero warnings and errors.
- Existing regression suite: **1,780 passed**, zero failed or skipped.
- Proton 10.0: captured all eight actual compiled tabs at 1040×760 and 820×620, the installation picker, running footer, control-state atlas and palette replacement. The final General extent is 790 pixels versus the original review's 1126; small spacing differences from the concept are intentional. Run Log fills its page, and the Master Rules editor remains bounded independently of rule count.
- **26 targeted assertions passed**: friendly combo display names; selected checkbox-mark contrast; four help badges with keyboard-tooltip configuration and automation descriptions; scrollbar line-down routing; expander content; Run Log height; bounded rule-list/editor sizing and editor binding updates; text selection/replacement; combo selection and popup; button automation invocation; determinate/indeterminate progress sizing; dynamic system-color foreground; and reaching the minimum-size editor's end.
- Palette key sets match, obsolete inline styles were removed, no Classic-border/drop-shadow references remain in app XAML, and `git diff --check` passes. Only UI dictionaries and the application's palette selection code were added; no dependencies or record handlers were added or replaced.

Final screenshots, observations and the throwaway probe source are retained locally in the ignored directory `artifacts/ui-styling-implementation-2026-10-17/`. The probe used fake paths and in-memory settings; it did not invoke a patch run, detection, persistence or rule-file import/export. The normal input/control classes still provide those workflows.

Limits: the control-state atlas uses controlled dependency-property states; tooltip keyboard configuration and automation properties were checked, but actual Tab navigation, screen-reader announcements, clipboard operations, native Windows rendering, OS DPI changes and live OS contrast switching were not verified. Palette replacement was exercised directly under Proton. The original native Windows/Proton acceptance matrix above remains the recommended release smoke test, rather than a claim that those checks have been completed.

## Focus and path-help follow-up — 2026-10-18

User feedback identified the oversized green focus outline and extra text-field edges as distracting. The shared focus template now uses a neutral one-pixel stroke with a one-pixel outward margin instead of a two-pixel accent stroke with a three-pixel margin. Text fields retain one border, use neutral foreground coloring when focused, and no longer contain the inner `FocusRing` or its visibility trigger. Record-list selection/focus behavior, keyboard shortcuts and help-badge keyboard access were retained. No record handlers or settings were migrated.

General-tab tooltip examples now show folder steps beginning at a recognizable Steam library, mod-list folder, MO2 instance or Amethyst game folder. Machine-specific drive letters, usernames and environment-variable notation were removed. The Windows and Proton Steam examples share one folder sequence; mod-manager-specific Stock Game and Game Root layouts remain distinct. Shared guidance explains how to follow the steps and select the destination with Browse; plugins.txt help identifies the file and hidden AppData folder.

The production palette is unchanged. Four color studies (forest and stone, Nordic blue, charcoal and gold, muted plum) were rendered using the shared control templates in an isolated review window. Primary-button text contrast ratios are approximately 6.59:1, 6.70:1, 7.59:1 and 6.55:1 respectively; this checks those color pairs only, not complete-theme accessibility.

Validation: Release app build and isolated UI-probe publish succeeded with zero warnings/errors. The Proton 10.0 probe passed 30 assertions, including four new checks for the reduced neutral outline and the single text-field border. It captured the compiled tabs, controls, path help and palette studies. A workspace search confirmed no remaining `FocusRing`, old oversized focus geometry or machine-specific example paths in app XAML; `git diff --check` passed. Artifacts and probe source are retained locally under the ignored `artifacts/ui-feedback-2026-10-18/` directory. The earlier rendering/input limitations still apply: focus states were inspected through templates and controlled dependency properties, not a physical keyboard session; native Windows and DPI coverage were not repeated. Record algorithms were unchanged, so the patcher regression suite was not rerun for this presentation-only follow-up.

## Light and dark theme implementation — 2026-10-18

The user selected the Breeze-inspired light and dark studies and requested the names **Light theme** and **Dark theme** in General. The Appearance selector reuses the existing enum-choice ComboBox binding pattern. `StandaloneSettings.Theme` uses the existing notification, normalization and settings-store serialization paths. New settings and older files without the property default to light; unknown numeric values normalize to light. Save settings and Run patcher persist the selection with other choices. The selection applies immediately and is restored when settings load.

The original palette dictionary was replaced by `LightTheme.xaml` and `DarkTheme.xaml`. Both supply the existing brush contract plus `InputBackgroundBrush`, also supplied by `HighContrast.xaml`, so inputs can use their own surface without duplicating templates. `App.SetTheme` uses the existing shared palette replacement method; OS high contrast overrides the selected palette while retaining the underlying preference. Windows, open dropdowns, lists and controls continue to resolve brushes dynamically. The existing settings-load branches now share one helper that replaces the settings subscription; theme changes do not invalidate path verification. The obsolete path-only event-handler name and original palette resource references were removed from active source. Record/property/flag handlers and patch settings retain their existing implementations; no record types were migrated.

Colors adapt the upstream [Breeze Light](https://github.com/KDE/breeze/blob/master/colors/BreezeLight.colors) and [Breeze Dark](https://github.com/KDE/breeze/blob/master/colors/BreezeDark.colors) surfaces and cyan accent to the app's shared WPF controls. Primary buttons and selected rows use dark text on cyan; muted text, borders, hover/pressed states and information panels are adapted for readability. This is a color adaptation, not a replacement with native KDE widget rendering. Catppuccin and the other studies remain preview-only artifacts.

Validation:

- Release build and self-contained publish succeeded with zero warnings/errors; the local standalone output is `artifacts/DreadsMashedPatch-win-x64/`.
- **17 targeted tests passed**, including four new theme default/compatibility/JSON tests and the existing standalone output and path-input tests.
- The isolated Proton 10.0 probe passed **50 UI assertions**, including immediate switching through the actual selector, matching light/dark labels, both existing windows recoloring, input surfaces, dropdown popup colors, active record selection, settings/path-verification preservation, real settings-store save/load, reapplying restored settings, and detaching replaced settings. The earlier control/layout assertions also passed.
- Captured all eight compiled tabs in both themes at default and minimum window sizes, selected record rows, control-state atlases and the installation picker. All three palette dictionaries expose the same **31 brush keys**. The **18 checked text/surface pairs** have contrast of at least **4.58:1**, including primary normal/hover/pressed, selection, header-muted text and disabled controls.
- App XAML parses, `git diff --check` passes, and active-source searches find no obsolete palette references or path-only event handler.

Probe source, observations and screenshots are retained locally in the ignored `artifacts/ui-theme-switch-2026-10-18/` directory. Probe settings were saved only inside the isolated Proton prefix and the previous settings file was restored afterward. No patch run, discovery or real mod-manager integration was invoked. The existing high-contrast dictionary was rendered, but live OS high-contrast toggling, physical keyboard/mouse input, native Windows and DPI coverage were not repeated. The full record-handler regression suite was not rerun for this UI/settings change.

## Proton fonts, labelled checkboxes and keyboard correction — 2026-10-18

The user reported serif fonts under Proton, dark checkbox labels in the dark theme, checkbox/label alignment, and unintended arrow-key navigation between controls. The keyword expander also needed keyboard access.

`TypographyResources` now creates the two shared font resources before the existing styles resolve them. One method enumerates preferred installed families and checks the resolved normal glyph typeface's Win32 family name against the candidate. Enumeration alone failed under Proton because the missing Segoe UI alias was listed but resolved to Times New Roman. Actual glyph validation skips that alias and selects Tahoma; monospace text selects Courier New in the isolated prefix. Windows preferences remain Segoe UI and Consolas when their actual faces are installed. The comma-separated resource definitions were removed. No platform detection, embedded font files or duplicated control styles were added.

The shared CheckBox style now supplies its foreground from the active palette, overriding the native theme's dark text. Its grid spans the full control height, with the square and content presenter independently centered; the full transparent hit area and independent checkmark color remain. Descriptive help paragraphs remain below their option labels. The existing expander header now has the WPF-recognized `HeaderSite` name and an explicit tab stop, with local header/editor tab traversal. This follows the framework's [expander template contract](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/controls/expander-styles-and-templates).

Keyboard support remains. One bubbling key handler registered for Window and ScrollViewer consumes otherwise unhandled arrows before spatial navigation occurs. Scroll viewers within text-editor and item-control templates defer to those controls, retaining native caret and selection behavior. A window-only policy was insufficient: [WPF ScrollViewer.OnKeyDown](https://github.com/dotnet/wpf/blob/main/src/Microsoft.DotNet.Wpf/src/PresentationFramework/System/Windows/Controls/ScrollViewer.cs) predicts and changes focus itself before the event reaches Window. The discarded window navigation-property approach is absent from active source. Existing Tab/Shift+Tab traversal, control activation and Ctrl+S/Ctrl+R handlers remain; no preview handler blocks dropdown/list/editor input.

Validation:

- Release build and self-contained publish succeeded with zero warnings/errors.
- **100 UI assertions passed under Proton 10.0**, retaining the preceding 50 theme/layout/settings assertions and adding glyph, checkbox and keyboard checks.
- The selected UI resource and rendered heading/checkbox glyphs report **Tahoma**; the monospace resource reports **Courier New**. Both light and dark checkbox labels, including disabled labels, match their palette brushes. All six measured Smart Forwarding square/label midpoint pairs match at **12.5 pixels** within the controls.
- WPF InputManager key events exercise Tab entry into the keyword header, Space expansion/collapse, checkbox toggling, dropdown selection and Tab exit, text-caret arrows, record-list navigation and Space toggling, the end-of-list boundary, and suppression of spatial movement from checkboxes and footer buttons. Reverse focus traversal uses WPF's Previous traversal request, exercising the Shift+Tab traversal path.
- Captured all compiled tabs at default/minimum sizes in both themes, plus corrected Smart Forwarding views and the expanded keyword editor. The installed-face check was verified against actual GlyphRun typefaces rather than only the configured font-family string.
- App XAML parses; `git diff --check` passes. Searches confirm the old comma-separated font resources and discarded directional-navigation overrides are absent from active app source. No record/property/flag handlers or patch settings changed; no record types were migrated, and the platform-neutral record suite was not rerun.

Probe source, observations and screenshots are retained locally under the ignored `artifacts/ui-input-font-fixes-2026-10-18/` directory. The probe uses a temporary Proton prefix, restores its saved-settings file after persistence checks, and does not invoke patching or mod-manager discovery. Keyboard tests drive WPF input/focus APIs; they are not physical key presses. Native Windows rendering, physical mouse/keyboard input, DPI changes and live OS contrast switching remain unverified.

## Approved plum and green palettes — 2026-10-18

The user approved the side-by-side plum and green preview, retaining the Light theme / Dark theme selector and delegating the help-badge color choice. The approved preview's 31 brush values were copied exactly into each existing light/dark dictionary. Light uses warm near-white surfaces with a deep plum header; dark uses charcoal/plum surfaces. Primary actions use green `#2E8052` with white text in both modes, green information panels replace the earlier blue panels, and focus/selection/tab accents use plum. Hover, pressed, disabled, read-only and scrolling colors follow the same palettes.

The non-action question-mark help badges now use the existing plum `PanelBrush` background and `FocusBrush` text/border. Plum fits the adjacent field labels and controls while green remains associated with the information panels and primary action. The change reuses the existing resource contract and shared badge template; it introduces no new brush keys or control implementations. High contrast still resolves those roles to matching system window/text colors.

Migration boundary: light/dark brush values and the shared badge's three brush assignments were replaced. The earlier Breeze blue/cyan color values and descriptions were removed from active app styling. High-contrast palette values remain system-managed; fonts, layouts, event handlers, saved theme preferences, record/property/flag handlers and patch settings retain their existing implementation. No record types were migrated. The previous Breeze implementation and original approval images remain historical evidence only.

Validation:

- Release build and self-contained publish succeeded with zero warnings/errors. The local executable was updated in `artifacts/DreadsMashedPatch-win-x64/`.
- **107 Proton UI assertions passed**: the preceding 100 font/layout/theme/settings/keyboard checks plus checks in both modes for green primary buttons with white text, four plum help badges and green General information panels, and one system-color help-badge check under the high-contrast dictionary.
- The light/dark brush values match the approved preview JSON exactly. All three runtime palettes retain the same **31 brush keys**. The **24 checked text/surface color pairs** have contrast of at least **4.58:1**, including badge text, primary hover/pressed text and information-panel guidance.
- Captured all compiled tabs in both modes at default/minimum sizes, both installation-picker modes, selection/control states, Smart Forwarding and the expanded keyword editor. Visual inspection confirms plum help badges and the approved green/plum combination in both General views.
- App XAML parses, `git diff --check` passes, and active-style searches find none of the superseded Breeze cyan/blue accent or information-panel values.

The isolated probe has no StartupUri and loads the production resource dictionaries directly; it invokes the compiled windows with fake paths. Its source, observations and screenshots are retained under the ignored `artifacts/ui-plum-green-implementation-2026-10-18/` directory. Settings-store checks restore the prefix's previous settings file. No patching or discovery was invoked. The earlier native Windows, physical input, DPI and live system-contrast limitations still apply; the platform-neutral record suite was not rerun for this palette-only change.


## Keyboard-only focus outlines — 2026-10-18

Shared focus adorners retain WPF's normal Tab/Shift+Tab traversal, including movement on the first Tab. The duplicate `IsKeyboardFocusWithin` border triggers were removed from ComboBox and SelectionItem; text fields keep their single editing border, and selection, validation, hover and open-dropdown cues remain independent of navigation focus.

Native-input testing found that WPF can leave its focus adorner visible when clicking the already-focused button. One application-lifetime `FocusVisualState` resource now filters the shared outline's visibility using WPF input preprocessing, including popup input. It observes keyboard and mouse-button events without handling them or moving focus; Windows' KeyboardCues accessibility preference can keep outlines visible. There is no custom Tab traversal or first-key interception.

Validation: Release publish succeeded and updated `artifacts/DreadsMashedPatch-win-x64/`; `git diff --check` passed. The isolated Proton 10.0 probe passed **28 checks** across light and dark themes using Win32 keyboard/mouse event injection through WPF's native input path. Checks cover first Tab, Shift+Tab, returning to keyboard navigation, clicking an already-focused button, single outlines, closed/open dropdown borders, text editing borders, and persistent record selection without an extra focus border. The prefix defaults KeyboardCues to true; normal automatic mode was tested with process-local WPF cache overrides only, without changing the OS preference. Probe source and observations are retained under ignored `artifacts/ui-keyboard-focus-2026-10-18/`. Native Windows and live OS accessibility-setting changes were not tested. No settings persistence, patching, discovery or record handlers changed.


### Mouse-click correction

The preceding implementation let `SystemParameters.KeyboardCues` force outlines visible after mouse input. This reproduced the user's report with Proton's unmodified `KeyboardCues=True` setting. The earlier probe disabled that setting within its process and therefore missed the shipped behavior. The shared state now follows the latest keyboard/mouse input directly; mouse clicks hide the outline regardless of KeyboardCues. Its obsolete system-parameter event subscription, callback and component-model import were removed. WPF traversal, text editing borders and selection remain unchanged.

The same native-input probe first reproduced the failure and then passed **28 checks** in both themes with the host's actual keyboard-cue setting intact, without private framework overrides. This includes clicking an already-focused button and restoring outlines with Tab/Shift+Tab. Evidence is under ignored `artifacts/ui-mouse-focus-fix-2026-10-18/`. Release publish updated the standalone executable, and `git diff --check` passed. Native Windows remains untested.
