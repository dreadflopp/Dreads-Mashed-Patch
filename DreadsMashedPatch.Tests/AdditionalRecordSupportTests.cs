using System.Drawing;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.LensFlare;
using DreadsMashedPatch.PropertyHandlers.VolumetricLighting;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("CoverageFixes")]
public sealed class AdditionalRecordSupportTests : IDisposable
{
    public void Dispose() => CoverageTestHistory.Reset();
    private static readonly string[] LightingFields = ["Intensity", "CustomColorContribution", "ColorR", "ColorG", "ColorB",
        "DensityContribution", "DensitySize", "DensityWindSpeed", "DensityFallingSpeed", "PhaseFunctionContribution",
        "PhaseFunctionScattering", "SamplingRepartitionRangeFactor"];
    public static IEnumerable<object[]> LightingCases() => LightingFields.SelectMany(field =>
        new[] { false, true }.SelectMany(authorized => new[] { false, true }.Select(remove => new object[] { field, authorized, remove })));

    [Theory]
    [MemberData(nameof(LightingCases))]
    public void EveryLightingFieldParticipatesInAtomicSelectionAndBinaryOutput(string field, bool authorized, bool remove)
    {
        CoverageTestHistory.Configure(authorized);
        var mods = CoverageTestHistory.Mods();
        var records = mods.Select(_ => Lighting()).ToArray();
        typeof(VolumetricLighting).GetProperty(field)!.SetValue(records[1], remove ? null : 0.75f);
        var handler = new LightingPresetHandler();
        var selected = handler.GetValue(records[1]);
        Assert.False(handler.AreValuesEqual(selected, handler.GetValue(records[0])));
        for (var i = 0; i < mods.Length; i++) mods[i].VolumetricLightings.Add(records[i]);
        using var state = CoverageTestHistory.State(mods);
        Program.RunPatch(state);
        if (authorized) Assert.Empty(state.PatchMod.VolumetricLightings);
        else
        {
            var output = Assert.Single(state.PatchMod.VolumetricLightings);
            Assert.Equal(selected, handler.GetValue(output));
            using var stream = Write(state.PatchMod);
            using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
            Assert.Equal(selected, handler.GetValue(Assert.Single(overlay.VolumetricLightings)));
        }
        Assert.Equal(selected, handler.GetValue(records[1]));
    }

    [Fact]
    public void IndependentlyAuthoredLightingAndFlareEditsSelectCompletePresets()
    {
        CoverageTestHistory.Configure();
        var mods = CoverageTestHistory.Mods();
        var lights = mods.Select(_ => Lighting()).ToArray();
        var flares = mods.Select(_ => Flare()).ToArray();
        lights[1].Intensity = 0.9f;
        lights[2].ColorG = 0.8f;
        flares[1].ColorInfluence = 0.9f;
        flares[2].Sprites![0].Data!.Width = 12;
        for (var i = 0; i < mods.Length; i++)
        {
            mods[i].VolumetricLightings.Add(lights[i]);
            mods[i].LensFlares.Add(flares[i]);
        }
        using var state = CoverageTestHistory.State(mods);
        Program.RunPatch(state);
        // A new complete preset from the winner is already effective; earlier components must not leak into it.
        Assert.Empty(state.PatchMod.VolumetricLightings);
        Assert.Empty(state.PatchMod.LensFlares);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void FlareRemovalAndRestorationAreAtomicAndPermissionControlled(bool authorized, bool remove)
    {
        CoverageTestHistory.Configure(authorized);
        var mods = CoverageTestHistory.Mods();
        var records = mods.Select(_ => Flare()).ToArray();
        if (remove)
        {
            records[1].ColorInfluence = null;
            records[1].FadeDistanceRadiusScale = null;
            records[1].Sprites = null;
        }
        else
        {
            records[1].ColorInfluence = 0.8f;
            records[1].Sprites!.Reverse();
            records[1].Sprites![0].Data!.Width = 15;
        }
        var handler = new FlareDefinitionHandler();
        var selected = handler.GetValue(records[1]);
        for (var i = 0; i < mods.Length; i++) mods[i].LensFlares.Add(records[i]);
        using var state = CoverageTestHistory.State(mods);
        Program.RunPatch(state);
        if (authorized) Assert.Empty(state.PatchMod.LensFlares);
        else
        {
            Assert.True(handler.AreValuesEqual(selected, handler.GetValue(Assert.Single(state.PatchMod.LensFlares))));
            using var stream = Write(state.PatchMod);
            using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
            var reloaded = Assert.Single(overlay.LensFlares);
            Assert.Equal(selected!.ColorInfluence, reloaded.ColorInfluence);
            Assert.Equal(selected.FadeDistanceRadiusScale, reloaded.FadeDistanceRadiusScale);
            if (selected.Sprites != null)
                for (var i = 0; i < selected.Sprites.Count; i++)
                {
                    Assert.Equal(selected.Sprites[i].LensFlareSpriteId, reloaded.Sprites![i].LensFlareSpriteId);
                    Assert.Equal(selected.Sprites[i].Texture!.GivenPath, reloaded.Sprites[i].Texture!.GivenPath);
                    Assert.Equal(selected.Sprites[i].Data!.Tint.ToArgb() & 0xFFFFFF, reloaded.Sprites[i].Data!.Tint.ToArgb() & 0xFFFFFF);
                    Assert.Equal(selected.Sprites[i].Data!.Width, reloaded.Sprites[i].Data!.Width);
                }
            Assert.True(handler.AreValuesEqual(selected, handler.GetValue(reloaded)));
        }
        Assert.True(handler.AreValuesEqual(selected, handler.GetValue(records[1])));
    }

    [Theory]
    [InlineData("ColorInfluence")]
    [InlineData("FadeDistanceRadiusScale")]
    [InlineData("LensFlareSpriteId")]
    [InlineData("Texture")]
    [InlineData("Tint")]
    [InlineData("Width")]
    [InlineData("Height")]
    [InlineData("Position")]
    [InlineData("AngularFade")]
    [InlineData("Opacity")]
    [InlineData("Flags")]
    [InlineData("Order")]
    [InlineData("Duplicates")]
    public void CompleteFlareEqualityDetectsEverySemanticDifference(string field)
    {
        var original = Flare();
        var changed = original.DeepCopy();
        var handler = new FlareDefinitionHandler();
        if (field is "ColorInfluence" or "FadeDistanceRadiusScale") changed.GetType().GetProperty(field)!.SetValue(changed, 0.9f);
        else if (field == "LensFlareSpriteId") changed.Sprites![0].LensFlareSpriteId = "New";
        else if (field == "Texture") changed.Sprites![0].Texture = new AssetLink<SkyrimTextureAssetType>("Textures/Other.dds");
        else if (field == "Order") changed.Sprites!.Reverse();
        else if (field == "Duplicates") changed.Sprites!.Add(changed.Sprites[0].DeepCopy());
        else
        {
            var data = changed.Sprites![0].Data!;
            object value = field == "Tint" ? Color.FromArgb(255, 99, 22, 33)
                : field == "Flags" ? (LensFlareSpriteData.Flag)2 : 99f;
            data.GetType().GetProperty(field)!.SetValue(data, value);
        }
        Assert.False(handler.AreValuesEqual(handler.GetValue(original), handler.GetValue(changed)), field);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FlareOverlayCopyPreservesCompleteOrderedDuplicateRowsAndDetachedSources(bool empty)
    {
        var source = Flare();
        if (empty) source.Sprites!.Clear();
        else source.Sprites!.Add(source.Sprites[0].DeepCopy());
        var mod = new SkyrimMod(CoverageTestHistory.Original, SkyrimRelease.SkyrimSE);
        mod.LensFlares.Add(source);
        using var input = Write(mod);
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(input, SkyrimRelease.SkyrimSE, mod.ModKey);
        var sourceGetter = Assert.Single(overlay.LensFlares);
        var handler = new FlareDefinitionHandler();
        var snapshot = handler.GetValue(sourceGetter)!;
        var target = Flare();
        handler.SetValue(target, snapshot);
        Assert.True(handler.AreValuesEqual(snapshot, handler.GetValue(target)));
        var output = new SkyrimMod(CoverageTestHistory.Winner, SkyrimRelease.SkyrimSE);
        output.LensFlares.Add(target);
        using var binary = Write(output);
        using var reload = SkyrimMod.CreateFromBinaryOverlay(binary, SkyrimRelease.SkyrimSE, output.ModKey);
        Assert.True(handler.AreValuesEqual(snapshot, handler.GetValue(Assert.Single(reload.LensFlares))));
        if (!empty)
        {
            Assert.Equal("Data/Textures/Flare.dds", target.Sprites![0].Texture!.GivenPath);
            target.Sprites[0].Data!.Width = 99;
            target.Sprites.Clear();
            Assert.Equal(3, snapshot.Sprites!.Count);
            Assert.Equal(1f, snapshot.Sprites[0].Data!.Width);
        }
        Assert.True(handler.AreValuesEqual(snapshot, handler.GetValue(sourceGetter)));
    }

    [Fact]
    public void FlareSnapshotAndSetterDetachMutableRowsAndRetainNullablePresence()
    {
        var source = Flare();
        var handler = new FlareDefinitionHandler();
        var snapshot = handler.GetValue(source)!;
        source.Sprites![0].Data!.Width = 99;
        Assert.Equal(1f, snapshot.Sprites![0].Data!.Width);
        var absent = Flare(); absent.Sprites = null;
        var empty = Flare(); empty.Sprites!.Clear();
        Assert.False(handler.AreValuesEqual(handler.GetValue(absent), handler.GetValue(empty)));
        handler.SetValue(source, handler.GetValue(absent));
        Assert.Null(source.Sprites);
        handler.SetValue(source, handler.GetValue(empty));
        Assert.Empty(source.Sprites!);
    }

    [Theory]
    [InlineData("TextureSet")]
    [InlineData("MaterialType")]
    [InlineData("HavokFriction")]
    [InlineData("HavokRestitution")]
    [InlineData("TextureSpecularExponent")]
    [InlineData("Grasses")]
    [InlineData("Flags")]
    public void LandscapeTextureFieldsForwardThroughTheEnabledRouteAndBinaryWriter(string field)
    {
        CoverageTestHistory.Configure();
        var mods = CoverageTestHistory.Mods();
        var records = mods.Select(_ => Texture()).ToArray();
        switch (field)
        {
            case "TextureSet": records[1].TextureSet.SetTo(CoverageTestHistory.LinkKey); break;
            case "MaterialType": records[1].MaterialType.SetTo(CoverageTestHistory.LinkKey); break;
            case "HavokFriction": records[1].HavokFriction = 5; break;
            case "HavokRestitution": records[1].HavokRestitution = 7; break;
            case "TextureSpecularExponent": records[1].TextureSpecularExponent = 9; break;
            case "Grasses": records[1].Grasses.Add(new FormLink<IGrassGetter>(CoverageTestHistory.LinkKey)); break;
            case "Flags": records[1].Flags = LandscapeTexture.Flag.IsSnow; break;
        }
        var handler = new LandscapeTextureRecordHandler().PropertyHandlers[field];
        var selected = handler.GetValue(records[1]);
        for (var i = 0; i < mods.Length; i++) mods[i].LandscapeTextures.Add(records[i]);
        using var state = CoverageTestHistory.State(mods);
        Program.RunPatch(state);
        Assert.True(handler.AreValuesEqual(selected, handler.GetValue(Assert.Single(state.PatchMod.LandscapeTextures))));
        using var binary = Write(state.PatchMod);
        using var reload = SkyrimMod.CreateFromBinaryOverlay(binary, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
        Assert.True(handler.AreValuesEqual(selected, handler.GetValue(Assert.Single(reload.LandscapeTextures))));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SnowFieldAbsenceIsDistinctFromZeroAndReversionsRequirePermission(bool authorized)
    {
        CoverageTestHistory.Configure(authorized);
        var mods = CoverageTestHistory.Mods();
        var records = mods.Select(_ => Texture()).ToArray();
        records[0].Flags = records[2].Flags = 0;
        records[1].Flags = null;
        var handler = Assert.IsType<SimpleReflectionNullableFlagPropertyHandler<LandscapeTexture.Flag, ILandscapeTexture, ILandscapeTextureGetter>>(
            new LandscapeTextureRecordHandler().PropertyHandlers["Flags"]);
        Assert.False(handler.AreValuesEqual(null, 0));
        for (var i = 0; i < mods.Length; i++) mods[i].LandscapeTextures.Add(records[i]);
        using var state = CoverageTestHistory.State(mods);
        Program.RunPatch(state);
        if (authorized) Assert.Empty(state.PatchMod.LandscapeTextures);
        else
        {
            Assert.Null(Assert.Single(state.PatchMod.LandscapeTextures).Flags);
            using var binary = Write(state.PatchMod);
            using var reload = SkyrimMod.CreateFromBinaryOverlay(binary, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
            Assert.Null(Assert.Single(reload.LandscapeTextures).Flags);
        }
    }

    [Fact]
    public void FlareEqualityUsesSerializedRgbAndProjectAssetPathSemantics()
    {
        var source = Flare(); var equivalent = source.DeepCopy(); var handler = new FlareDefinitionHandler();
        equivalent.Sprites![0].Data!.Tint = Color.FromArgb(0, 11, 22, 33);
        equivalent.Sprites[0].Texture = new AssetLink<SkyrimTextureAssetType>("data\\textures\\flare.dds");
        Assert.True(handler.AreValuesEqual(handler.GetValue(source), handler.GetValue(equivalent)));
        equivalent.Sprites[0].Texture = new AssetLink<SkyrimTextureAssetType>("Textures/Flare.dds");
        Assert.False(handler.AreValuesEqual(handler.GetValue(source), handler.GetValue(equivalent)));
    }

    [Theory]
    [InlineData("VOLI", false, false)]
    [InlineData("VOLI", true, false)]
    [InlineData("VOLI", false, true)]
    [InlineData("LENS", false, false)]
    [InlineData("LENS", true, false)]
    [InlineData("LENS", false, true)]
    [InlineData("LTEX", false, false)]
    [InlineData("LTEX", true, false)]
    [InlineData("LTEX", false, true)]
    public void BinarySourcesRespectRecordSelectionAndPriorityPolicies(string signature, bool priority, bool disabled)
    {
        CoverageTestHistory.Configure();
        var mods = CoverageTestHistory.Mods();
        var getter = signature switch { "VOLI" => typeof(IVolumetricLightingGetter), "LENS" => typeof(ILensFlareGetter), _ => typeof(ILandscapeTextureGetter) };
        var config = new PatcherConfiguration { CompatibilityRules = [] };
        if (priority) config.AlwaysWinningMods.Add(CoverageTestHistory.Earlier.FileName.String);
        if (disabled) config.DisabledRecordTypes.Add(getter.FullName!);
        PatcherSettings.Apply(config);
        for (var i = 0; i < mods.Length; i++)
        {
            switch (signature)
            {
                case "VOLI":
                    var lighting = Lighting(); lighting.Intensity = i == 1 ? 0.75f : 0.25f;
                    mods[i].VolumetricLightings.Add(lighting); break;
                case "LENS":
                    var flare = Flare(); flare.Sprites![0].Data!.Opacity = i == 1 ? 0.75f : 1;
                    mods[i].LensFlares.Add(flare); break;
                default:
                    var texture = Texture(); texture.HavokFriction = i == 1 ? (byte)7 : (byte)1;
                    mods[i].LandscapeTextures.Add(texture); break;
            }
        }
        var streams = mods.Select(Write).ToArray();
        var overlays = streams.Select((stream, i) => SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mods[i].ModKey)).ToArray();
        try
        {
            using var state = CoverageTestHistory.State(overlays);
            Program.RunPatch(state);
            var records = state.PatchMod.EnumerateMajorRecords().Where(getter.IsInstanceOfType).ToArray();
            if (disabled) Assert.Empty(records);
            else
            {
                var output = Assert.Single(records);
                var expected = Assert.Single(overlays[1].EnumerateMajorRecords(), getter.IsInstanceOfType);
                var handler = signature switch
                {
                    "VOLI" => new VolumetricLightingRecordHandler().PropertyHandlers["LightingPreset"],
                    "LENS" => new LensFlareRecordHandler().PropertyHandlers["FlareDefinition"],
                    _ => new LandscapeTextureRecordHandler().PropertyHandlers["HavokFriction"]
                };
                Assert.True(handler.AreValuesEqual(handler.GetValue(expected), handler.GetValue(output)));
                using var binary = Write(state.PatchMod);
                using var reload = SkyrimMod.CreateFromBinaryOverlay(binary, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
                Assert.True(handler.AreValuesEqual(handler.GetValue(expected), handler.GetValue(
                    Assert.Single(reload.EnumerateMajorRecords(), getter.IsInstanceOfType))));
            }
        }
        finally
        {
            foreach (var overlay in overlays) overlay.Dispose();
            foreach (var stream in streams) stream.Dispose();
        }
    }

    [Fact]
    public void LandscapeGrassMembershipRetainsDuplicatesAndUsesFormIdSortOrder()
    {
        CoverageTestHistory.Configure(); var mods = CoverageTestHistory.Mods();
        var records = mods.Select(_ => Texture()).ToArray();
        var a = new FormKey(CoverageTestHistory.Original, 0x805);
        var b = new FormKey(CoverageTestHistory.Original, 0x806);
        var c = new FormKey(CoverageTestHistory.Original, 0x807);
        foreach (var record in records) record.Grasses.Add(new FormLink<IGrassGetter>(b));
        records[1].Grasses.Add(new FormLink<IGrassGetter>(a)); records[1].Grasses.Add(new FormLink<IGrassGetter>(a));
        records[2].Grasses.Add(new FormLink<IGrassGetter>(c));
        for (var i = 0; i < mods.Length; i++) mods[i].LandscapeTextures.Add(records[i]);
        using var state = CoverageTestHistory.State(mods); Program.RunPatch(state);
        Assert.Equal(new[] { a, a, b, c }, Assert.Single(state.PatchMod.LandscapeTextures).Grasses.Select(link => link.FormKey));
        using var binary = Write(state.PatchMod);
        using var reload = SkyrimMod.CreateFromBinaryOverlay(binary, SkyrimRelease.SkyrimSE, state.PatchMod.ModKey);
        Assert.Equal(new[] { a, a, b, c }, Assert.Single(reload.LandscapeTextures).Grasses.Select(link => link.FormKey));
    }

    private static VolumetricLighting Lighting()
    {
        var record = new VolumetricLighting(new FormKey(CoverageTestHistory.Original, 0x850), SkyrimRelease.SkyrimSE);
        foreach (var field in LightingFields) typeof(VolumetricLighting).GetProperty(field)!.SetValue(record, 0.25f);
        return record;
    }

    private static LensFlare Flare() => new(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE)
    {
        ColorInfluence = 0.25f, FadeDistanceRadiusScale = 0.5f,
        Sprites = [Sprite("First", 1), Sprite("Second", 2)]
    };

    private static LensFlareSprite Sprite(string id, float size) => new()
    {
        LensFlareSpriteId = id, Texture = new AssetLink<SkyrimTextureAssetType>("Data/Textures/Flare.dds"),
        Data = new LensFlareSpriteData
        {
            Tint = Color.FromArgb(255, 11, 22, 33), Width = size, Height = size + 1,
            Position = 0.5f, AngularFade = 0.75f, Opacity = 1, Flags = (LensFlareSpriteData.Flag)1
        }
    };

    private static LandscapeTexture Texture() => new(CoverageTestHistory.RecordKey, SkyrimRelease.SkyrimSE)
    {
        HavokFriction = 1, HavokRestitution = 2, TextureSpecularExponent = 3, Flags = 0
    };

    private static MemoryStream Write(ISkyrimModGetter mod)
    {
        var stream = new MemoryStream(); mod.WriteToBinary(stream); stream.Position = 0; return stream;
    }
}
