using DreadsMashedPatch.AssetForwarding;
using DreadsMashedPatch.PropertyHandlers.Water;
using System;
using System.Collections.Generic;
using System.Drawing;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Noggog;

namespace DreadsMashedPatch.RecordHandlers;

// Damage/rendering flags own disjoint masks and retain destination nullable presence.
// Flow texture includes flowmap interpretation; each noise texture travels with its UV parameters.
// Motion/amplitude/falloff and gameplay retain ordinary forwarding.
public class WaterRecordHandler : AbstractRecordHandler
{
    protected override void InitializePropertyContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> originalContext,
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext)
    {
        base.InitializePropertyContexts(originalContext, winningContext);
        AssetForwardingPolicy.ResolveMemberContexts(GetType(), PropertyHandlers, PropertyContexts);
    }

    protected override PropertyForwardingCoordination CoordinateForwardedProperties(
        IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state) =>
        AssetForwardingPolicy.Coordinate(GetType(), PropertyHandlers, recordContexts, state, AtomicOwnershipTriggerProperties);

    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Name", new TranslatedStringReflectionPropertyHandler<IWater, IWaterGetter>("Name") },
        { "Opacity", new SimpleReflectionPropertyHandler<byte, IWater, IWaterGetter>("Opacity") },
        { "Flags", new WaterFlagsHandler(damageFlags: true) },
        { "Flags.Surface", new WaterFlagsHandler(damageFlags: false) },
        { "MNAM", new SimpleReflectionBinaryDataPropertyHandler<IWater, IWaterGetter>("MNAM") },
        { "Material", new SimpleReflectionFormLinkPropertyHandler<IMaterialTypeGetter, IWater, IWaterGetter>("Material") },
        { "OpenSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IWater, IWaterGetter>("OpenSound") },
        { "Spell", new SimpleReflectionFormLinkPropertyHandler<ISpellGetter, IWater, IWaterGetter>("Spell") },
        { "ImageSpace", new SimpleReflectionFormLinkPropertyHandler<IImageSpaceGetter, IWater, IWaterGetter>("ImageSpace") },
        { "DamagePerSecond", new SimpleReflectionPropertyHandler<ushort?, IWater, IWaterGetter>("DamagePerSecond") },
        { "SpecularSunPower", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("SpecularSunPower") },
        { "WaterReflectivity", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("WaterReflectivity") },
        { "WaterFresnel", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("WaterFresnel") },
        { "FogAboveWaterDistanceNearPlane", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("FogAboveWaterDistanceNearPlane") },
        { "FogAboveWaterDistanceFarPlane", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("FogAboveWaterDistanceFarPlane") },
        { "ShallowColor", new SimpleReflectionPropertyHandler<Color, IWater, IWaterGetter>("ShallowColor") },
        { "DeepColor", new SimpleReflectionPropertyHandler<Color, IWater, IWaterGetter>("DeepColor") },
        { "ReflectionColor", new SimpleReflectionPropertyHandler<Color, IWater, IWaterGetter>("ReflectionColor") },
        { "DisplacementStartingSize", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("DisplacementStartingSize") },
        { "DisplacementFoce", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("DisplacementFoce") },
        { "DisplacementVelocity", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("DisplacementVelocity") },
        { "DisplacementFalloff", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("DisplacementFalloff") },
        { "DisplacementDampner", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("DisplacementDampner") },
        { "NoiseFalloff", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseFalloff") },
        { "NoiseLayerOneWindDirection", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerOneWindDirection") },
        { "NoiseLayerTwoWindDirection", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerTwoWindDirection") },
        { "NoiseLayerThreeWindDirection", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerThreeWindDirection") },
        { "NoiseLayerOneWindSpeed", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerOneWindSpeed") },
        { "NoiseLayerTwoWindSpeed", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerTwoWindSpeed") },
        { "NoiseLayerThreeWindSpeed", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerThreeWindSpeed") },
        { "FogAboveWaterAmount", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("FogAboveWaterAmount") },
        { "FogUnderWaterAmount", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("FogUnderWaterAmount") },
        { "FogUnderWaterDistanceNearPlane", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("FogUnderWaterDistanceNearPlane") },
        { "FogUnderWaterDistanceFarPlane", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("FogUnderWaterDistanceFarPlane") },
        { "WaterRefractionMagnitude", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("WaterRefractionMagnitude") },
        { "SpecularPower", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("SpecularPower") },
        { "SpecularRadius", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("SpecularRadius") },
        { "SpecularBrightness", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("SpecularBrightness") },
        { "NoiseLayerOneUvScale", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerOneUvScale") },
        { "NoiseLayerTwoUvScale", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerTwoUvScale") },
        { "NoiseLayerThreeUvScale", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerThreeUvScale") },
        { "NoiseLayerOneAmplitudeScale", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerOneAmplitudeScale") },
        { "NoiseLayerTwoAmplitudeScale", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerTwoAmplitudeScale") },
        { "NoiseLayerThreeAmplitudeScale", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseLayerThreeAmplitudeScale") },
        { "WaterReflectionMagnitude", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("WaterReflectionMagnitude") },
        { "SpecularSunSparkleMagnitude", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("SpecularSunSparkleMagnitude") },
        { "SpecularSunSpecularMagnitude", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("SpecularSunSpecularMagnitude") },
        { "DepthReflections", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("DepthReflections") },
        { "DepthRefraction", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("DepthRefraction") },
        { "DepthNormals", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("DepthNormals") },
        { "DepthSpecularLighting", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("DepthSpecularLighting") },
        { "SpecularSunSparklePower", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("SpecularSunSparklePower") },
        { "NoiseFlowmapScale", new SimpleReflectionPropertyHandler<float, IWater, IWaterGetter>("NoiseFlowmapScale") },
        { "GNAM", new SimpleReflectionBinaryDataPropertyHandler<IWater, IWaterGetter>("GNAM") },
        { "LinearVelocity", new SimpleReflectionPropertyHandler<P3Float?, IWater, IWaterGetter>("LinearVelocity") },
        { "AngularVelocity", new SimpleReflectionPropertyHandler<P3Float?, IWater, IWaterGetter>("AngularVelocity") },
        { "NoiseLayerOneTexture", new SimpleReflectionAssetLinkPropertyHandler<SkyrimTextureAssetType, IWater, IWaterGetter>("NoiseLayerOneTexture") },
        { "NoiseLayerTwoTexture", new SimpleReflectionAssetLinkPropertyHandler<SkyrimTextureAssetType, IWater, IWaterGetter>("NoiseLayerTwoTexture") },
        { "NoiseLayerThreeTexture", new SimpleReflectionAssetLinkPropertyHandler<SkyrimTextureAssetType, IWater, IWaterGetter>("NoiseLayerThreeTexture") },
        { "FlowNormalsNoiseTexture", new SimpleReflectionAssetLinkPropertyHandler<SkyrimTextureAssetType, IWater, IWaterGetter>("FlowNormalsNoiseTexture") },
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IWaterGetter waterRecord)
        {
            throw new InvalidOperationException($"Expected IWaterGetter but got {winningContext.Record.GetType()}");
        }

        return waterRecord
            .ToLink<IWaterGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IWater, IWaterGetter>(state.LinkCache)
            .ToArray();
    }
}
