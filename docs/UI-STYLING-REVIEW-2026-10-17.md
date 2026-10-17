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
