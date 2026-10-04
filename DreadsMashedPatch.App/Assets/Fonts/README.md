# Bundled fonts

These unmodified, hinted, static TrueType fonts come from the official
[Noto fonts repository](https://github.com/notofonts/noto-fonts/tree/ffebf8c1ee449e544955a7e813c54f9b73848eac),
pinned to commit `ffebf8c1ee449e544955a7e813c54f9b73848eac`.

- `NotoSans-{Regular,SemiBold,Bold,Italic}.ttf`: `hinted/ttf/NotoSans/`.
- `NotoSansMono-{Regular,SemiBold}.ttf`: `hinted/ttf/NotoSansMono/`.
- `OFL.txt`: the upstream repository's `LICENSE`, preserved verbatim.

All fonts are licensed under the SIL Open Font License 1.1. The font files and
license are WPF resources embedded in the application. The license is also copied
to `Assets/Fonts/OFL.txt` in build and publish output, outside the executable bundle.

Static faces avoid relying on variable-font support in WPF or Proton. UI text uses
Noto Sans; logs, record signatures, elapsed time, and multiline plugin/keyword
fields use Noto Sans Mono. No font installation or download is needed at runtime.
