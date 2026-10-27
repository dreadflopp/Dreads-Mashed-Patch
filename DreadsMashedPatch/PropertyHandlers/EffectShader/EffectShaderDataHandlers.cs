using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Skyrim;
using Shader = Mutagen.Bethesda.Skyrim.EffectShader;

namespace DreadsMashedPatch.PropertyHandlers.EffectShader;

/// <summary>Texture coordinates/atlas indices travel with images; effect tuning remains ordinary DATA.</summary>
public static class EffectShaderDataHandlers
{
    private static readonly string[] FillFields = ["FillTextureScaleU", "FillTextureScaleV"];
    private static readonly string[] ParticleFields = ["TextureCountU", "TextureCountV", "ParticleAnimatedStartFrame",
        "ParticleAnimatedStartFrameVariation", "ParticleAnimatedEndFrame", "ParticleAnimatedLoopStartFrame",
        "ParticleAnimatedLoopStartVariation", "ParticleAnimatedFrameCount", "ParticleAnimatedFrameCountVariation"];
    private static readonly Shader.TranslationMask FillMask = Mask(FillFields, true);
    private static readonly Shader.TranslationMask ParticleMask = Mask(ParticleFields, true);
    private static readonly Shader.TranslationMask DataMask = Mask(FillFields.Concat(ParticleFields), false);

    // TranslationMask fields are generated booleans; this does not handle or reinterpret record flags.
    private static Shader.TranslationMask Mask(IEnumerable<string> fields, bool include)
    {
        var mask = new Shader.TranslationMask(!include);
        foreach (var name in fields) typeof(Shader.TranslationMask).GetField(name)!.SetValue(mask, include);
        return mask;
    }

    public static IPropertyHandler Data() => Projection("EffectShaderData", DataMask);
    public static IPropertyHandler Fill() => Projection("FillTextureCoordinates", FillMask, FillFields);
    public static IPropertyHandler Particles() => Projection("ParticleAtlas", ParticleMask, ParticleFields);

    private static IPropertyHandler Projection(string name, Shader.TranslationMask mask, params string[] fields)
    {
        IEffectShaderGetter? Project(IEffectShaderGetter? value) => value?.DeepCopy(mask);
        IEffectShaderGetter? Merge(IEffectShaderGetter? value, IEffectShaderGetter? destination)
        {
            if (value == null || destination == null) return destination;
            var result = destination.DeepCopy();
            result.DeepCopyIn(value, out _, mask);
            return result;
        }
        var aggregate = new EffectShaderDataHandler();
        return fields.Length == 0
            ? new ProjectedPropertyHandler<IEffectShaderGetter>(name, aggregate, Project, Merge,
                typeof(IEffectShaderGetter).GetProperties().Select(property => property.Name)
                    .Except(FillFields.Concat(ParticleFields))
                    .Where(field => field is not ("StaticRegistration" or "DATADataTypeState" or "FillTexture" or "HolesTexture"
                        or "MembranePaletteTexture" or "ParticleShaderTexture" or "ParticlePaletteTexture")).ToArray())
            : new NestedAssetPropertyHandler<IEffectShaderGetter>(name, aggregate, Project, Merge,
                (value, _) => value, fields);
    }
}
