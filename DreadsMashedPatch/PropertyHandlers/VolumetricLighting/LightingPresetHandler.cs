using DreadsMashedPatch.PropertyHandlers.Abstracts;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.VolumetricLighting;

// Following ReverbDataHandler: one immutable snapshot for the complete authored preset.
public sealed record LightingPresetValue(
    float? Intensity, float? CustomColorContribution, float? ColorR, float? ColorG, float? ColorB,
    float? DensityContribution, float? DensitySize, float? DensityWindSpeed, float? DensityFallingSpeed,
    float? PhaseFunctionContribution, float? PhaseFunctionScattering, float? SamplingRepartitionRangeFactor);

public sealed class LightingPresetHandler : AbstractPropertyHandler<LightingPresetValue>
{
    public override string PropertyName => "LightingPreset";

    public override LightingPresetValue? GetValue(IMajorRecordGetter record) =>
        record is IVolumetricLightingGetter lighting
            ? new(lighting.Intensity, lighting.CustomColorContribution, lighting.ColorR, lighting.ColorG,
                lighting.ColorB, lighting.DensityContribution, lighting.DensitySize, lighting.DensityWindSpeed,
                lighting.DensityFallingSpeed, lighting.PhaseFunctionContribution, lighting.PhaseFunctionScattering,
                lighting.SamplingRepartitionRangeFactor)
            : null;

    public override void SetValue(IMajorRecord record, LightingPresetValue? value)
    {
        if (record is not IVolumetricLighting lighting || value is null) return;
        lighting.Intensity = value.Intensity;
        lighting.CustomColorContribution = value.CustomColorContribution;
        lighting.ColorR = value.ColorR;
        lighting.ColorG = value.ColorG;
        lighting.ColorB = value.ColorB;
        lighting.DensityContribution = value.DensityContribution;
        lighting.DensitySize = value.DensitySize;
        lighting.DensityWindSpeed = value.DensityWindSpeed;
        lighting.DensityFallingSpeed = value.DensityFallingSpeed;
        lighting.PhaseFunctionContribution = value.PhaseFunctionContribution;
        lighting.PhaseFunctionScattering = value.PhaseFunctionScattering;
        lighting.SamplingRepartitionRangeFactor = value.SamplingRepartitionRangeFactor;
    }
}
