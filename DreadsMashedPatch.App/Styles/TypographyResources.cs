using System.Windows;
using System.Windows.Media;

namespace DreadsMashedPatch.App.Styles;

public sealed class TypographyResources : ResourceDictionary
{
    public TypographyResources()
    {
        // Wine can substitute a missing family before WPF tries the next name in a font list.
        // Choose an installed family before the shared styles resolve these resources.
        var installed = Fonts.SystemFontFamilies.ToArray();
        this["UiFontFamily"] = SelectInstalledFont(installed, "Segoe UI", "Tahoma", "Arial");
        this["MonospaceFontFamily"] = SelectInstalledFont(installed,
            "Consolas", "Courier New", "Liberation Mono", "DejaVu Sans Mono");
    }

    private static FontFamily SelectInstalledFont(FontFamily[] installed, params string[] preferredNames)
    {
        foreach (var name in preferredNames)
        {
            var family = installed.FirstOrDefault(font =>
                font.FamilyNames.Values.Contains(name, StringComparer.OrdinalIgnoreCase));
            // Wine may also enumerate an alias as an installed family. Check the resolved face.
            if (family is not null
                && new Typeface(family, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal)
                    .TryGetGlyphTypeface(out var glyph)
                && glyph.Win32FamilyNames.Values.Contains(name, StringComparer.OrdinalIgnoreCase))
            {
                return family;
            }
        }

        return SystemFonts.MessageFontFamily;
    }
}
