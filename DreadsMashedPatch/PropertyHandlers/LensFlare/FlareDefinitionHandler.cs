using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace DreadsMashedPatch.PropertyHandlers.LensFlare;

public sealed record FlareDefinitionValue(
    float? ColorInfluence, float? FadeDistanceRadiusScale, IReadOnlyList<ILensFlareSpriteGetter>? Sprites);

// Complete definition ownership follows the project's grouped snapshot pattern.
// Generated copies retain sprite IDs, GivenPath, complete LFSD (including its flags), order and duplicates.
public sealed class FlareDefinitionHandler : AbstractPropertyHandler<FlareDefinitionValue>
{
    public override string PropertyName => "FlareDefinition";

    public override FlareDefinitionValue? GetValue(IMajorRecordGetter record) =>
        record is ILensFlareGetter flare
            ? new(flare.ColorInfluence, flare.FadeDistanceRadiusScale,
                flare.Sprites?.Select(sprite => (ILensFlareSpriteGetter)sprite.DeepCopy()).ToArray())
            : null;

    public override void SetValue(IMajorRecord record, FlareDefinitionValue? value)
    {
        if (record is not ILensFlare flare || value is null) return;
        flare.ColorInfluence = value.ColorInfluence;
        flare.FadeDistanceRadiusScale = value.FadeDistanceRadiusScale;
        flare.Sprites = value.Sprites is null ? null : new ExtendedList<LensFlareSprite>(
            value.Sprites.Select(sprite => sprite.DeepCopy()));
    }

    public override bool AreValuesEqual(FlareDefinitionValue? left, FlareDefinitionValue? right)
    {
        if (left is null || right is null) return left is null && right is null;
        if (left.ColorInfluence != right.ColorInfluence || left.FadeDistanceRadiusScale != right.FadeDistanceRadiusScale)
            return false;
        if (left.Sprites is null || right.Sprites is null) return left.Sprites is null && right.Sprites is null;
        return left.Sprites.Count == right.Sprites.Count && left.Sprites.Zip(right.Sprites)
            .All(pair => SpritesEqual(pair.First, pair.Second));
    }
    private static readonly LensFlareSpriteData.TranslationMask NonTintMask = new(true) { Tint = false };

    private static bool SpritesEqual(ILensFlareSpriteGetter left, ILensFlareSpriteGetter right)
    {
        if (left.LensFlareSpriteId != right.LensFlareSpriteId || !AssetPathHelper.AreEqual(left.Texture, right.Texture))
            return false;
        if (left.Data is null || right.Data is null) return left.Data is null && right.Data is null;
        // LFSD stores NoAlphaFloat RGB; Opacity is the independent authored opacity field.
        return left.Data.Tint.R == right.Data.Tint.R && left.Data.Tint.G == right.Data.Tint.G
            && left.Data.Tint.B == right.Data.Tint.B
            && LensFlareSpriteDataMixIn.Equals(left.Data, right.Data, NonTintMask);
    }
}
