using System.Windows;
using System.Windows.Media;

namespace DreadsMashedPatch.App.Services;

public static class FontDiagnostics
{
    public static string Describe(FontFamily uiFontFamily, FontFamily monospaceFontFamily) =>
        string.Join(Environment.NewLine,
        [
            DescribeFace("UI", uiFontFamily, FontWeights.Normal, FontStyles.Normal),
            DescribeFace("UI", uiFontFamily, FontWeights.SemiBold, FontStyles.Normal),
            DescribeFace("UI", uiFontFamily, FontWeights.Bold, FontStyles.Normal),
            DescribeFace("UI", uiFontFamily, FontWeights.Normal, FontStyles.Italic),
            DescribeFace("Monospace", monospaceFontFamily, FontWeights.Normal, FontStyles.Normal),
            DescribeFace("Monospace", monospaceFontFamily, FontWeights.SemiBold, FontStyles.Normal)
        ]) + Environment.NewLine;

    private static string DescribeFace(string label, FontFamily family, FontWeight weight, FontStyle style)
    {
        var prefix = $"[Fonts] {label} ({weight}/{style}), requested: {family.Source}";
        try
        {
            var typeface = new Typeface(family, style, weight, FontStretches.Normal);
            if (!typeface.TryGetGlyphTypeface(out var glyph))
            {
                return $"{prefix}; resolved: composite font or no physical face available.";
            }

            var familyName = glyph.FamilyNames.Values.FirstOrDefault() ?? "unknown";
            return $"{prefix}; resolved: {familyName} ({glyph.Weight}/{glyph.Style}/{glyph.Stretch}); "
                + $"simulations: {glyph.StyleSimulations}; file: {glyph.FontUri}";
        }
        catch (Exception ex)
        {
            // Optional diagnostics must not prevent startup on an unusual Wine font setup.
            return $"{prefix}; font diagnostics unavailable: {ex.Message}";
        }
    }
}
