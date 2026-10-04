using System.Drawing;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class CollectionSemanticsBinaryRoundTripTests
{
    private static readonly ModKey SourceModKey = ModKey.FromNameAndExtension("CollectionRoundTripSource.esp");
    private static readonly ModKey PatchModKey = ModKey.FromNameAndExtension("CollectionRoundTripPatch.esp");

    [Fact]
    public void BodyPartDataPartNodeKeysAndRowDataSurviveBinaryRoundTrip()
    {
        var source = new SkyrimMod(SourceModKey, SkyrimRelease.SkyrimSE);
        var sourceRecord = new BodyPartData(new FormKey(SourceModKey, 0x800), SkyrimRelease.SkyrimSE);
        sourceRecord.Parts.Add(BodyPart("NPC Head [Head]", 1.25f, 33));
        sourceRecord.Parts.Add(BodyPart("NPC Spine [Spn0]", 2.5f, 66));
        source.BodyParts.Add(sourceRecord);

        using var sourceStream = Write(source);
        using var sourceOverlay = SkyrimMod.CreateFromBinaryOverlay(sourceStream, SkyrimRelease.SkyrimSE, SourceModKey);
        var overlayValue = new BodyPartDataRecordHandler().PropertyHandlers["Parts"]
            .GetValue(Assert.Single(sourceOverlay.BodyParts));

        var patch = new SkyrimMod(PatchModKey, SkyrimRelease.SkyrimSE);
        var target = new BodyPartData(new FormKey(PatchModKey, 0x800), SkyrimRelease.SkyrimSE);
        patch.BodyParts.Add(target);
        new BodyPartDataRecordHandler().PropertyHandlers["Parts"].SetValue(target, overlayValue);

        using var patchStream = Write(patch);
        using var patchOverlay = SkyrimMod.CreateFromBinaryOverlay(patchStream, SkyrimRelease.SkyrimSE, PatchModKey);
        var written = Assert.Single(patchOverlay.BodyParts).Parts;

        Assert.Equal(["NPC Head [Head]", "NPC Spine [Spn0]"], written.Select(part => part.PartNode));
        Assert.Equal([1.25f, 2.5f], written.Select(part => part.DamageMult));
        Assert.Equal(new byte[] { 33, 66 }, written.Select(part => part.HealthPercent));
    }

    [Fact]
    public void NpcHeadPartsAndTintLayerKeysSurviveBinaryRoundTrip()
    {
        var firstHeadPart = new FormKey(SourceModKey, 0x120);
        var secondHeadPart = new FormKey(SourceModKey, 0x121);
        var source = new SkyrimMod(SourceModKey, SkyrimRelease.SkyrimSE);
        var sourceRecord = new Npc(new FormKey(SourceModKey, 0x810), SkyrimRelease.SkyrimSE);
        sourceRecord.HeadParts.Add(new FormLink<IHeadPartGetter>(firstHeadPart));
        sourceRecord.HeadParts.Add(new FormLink<IHeadPartGetter>(secondHeadPart));
        sourceRecord.TintLayers.Add(TintLayer(2, Color.FromArgb(255, 10, 20, 30), 0.25f, 4));
        sourceRecord.TintLayers.Add(TintLayer(7, Color.FromArgb(255, 40, 50, 60), 0.75f, 9));
        source.Npcs.Add(sourceRecord);

        using var sourceStream = Write(source);
        using var sourceOverlay = SkyrimMod.CreateFromBinaryOverlay(sourceStream, SkyrimRelease.SkyrimSE, SourceModKey);
        var overlayNpc = Assert.Single(sourceOverlay.Npcs);
        var handlers = new NpcRecordHandler().PropertyHandlers;

        var patch = new SkyrimMod(PatchModKey, SkyrimRelease.SkyrimSE);
        var target = new Npc(new FormKey(PatchModKey, 0x810), SkyrimRelease.SkyrimSE);
        patch.Npcs.Add(target);
        handlers["HeadParts"].SetValue(target, handlers["HeadParts"].GetValue(overlayNpc));
        handlers["TintLayers"].SetValue(target, handlers["TintLayers"].GetValue(overlayNpc));

        using var patchStream = Write(patch);
        using var patchOverlay = SkyrimMod.CreateFromBinaryOverlay(patchStream, SkyrimRelease.SkyrimSE, PatchModKey);
        var written = Assert.Single(patchOverlay.Npcs);

        Assert.Equal([firstHeadPart, secondHeadPart], written.HeadParts.Select(link => link.FormKey));
        Assert.Equal(new ushort?[] { 2, 7 }, written.TintLayers.Select(layer => layer.Index));
        Assert.Equal([0.25f, 0.75f], written.TintLayers.Select(layer => layer.InterpolationValue));
        Assert.Equal(new short?[] { 4, 9 }, written.TintLayers.Select(layer => layer.Preset));
        Assert.Equal(
            [Color.FromArgb(255, 10, 20, 30).ToArgb(), Color.FromArgb(255, 40, 50, 60).ToArgb()],
            written.TintLayers.Select(layer => layer.Color?.ToArgb()));
    }

    [Fact]
    public void AllGenderedArmorAddonAlternateTextureSurfacesSurviveBinaryRoundTrip()
    {
        var source = new SkyrimMod(SourceModKey, SkyrimRelease.SkyrimSE);
        var sourceRecord = new ArmorAddon(new FormKey(SourceModKey, 0x820), SkyrimRelease.SkyrimSE)
        {
            FirstPersonModel = new GenderedItem<Model?>(
                Model("SourceFirstMale.nif", 1, AlternateTextures(("Body", 2, 0x201), ("Body", 7, 0x202))),
                Model("SourceFirstFemale.nif", 2, AlternateTextures(("FemaleBody", 3, 0x203)))),
            WorldModel = new GenderedItem<Model?>(
                Model("SourceWorldMale.nif", 3, AlternateTextures(("MaleWorld", 4, 0x204))),
                Model("SourceWorldFemale.nif", 4, AlternateTextures(("FemaleWorld", 5, 0x205))))
        };
        source.ArmorAddons.Add(sourceRecord);

        using var sourceStream = Write(source);
        using var sourceOverlay = SkyrimMod.CreateFromBinaryOverlay(sourceStream, SkyrimRelease.SkyrimSE, SourceModKey);
        var overlayAddon = Assert.Single(sourceOverlay.ArmorAddons);

        var patch = new SkyrimMod(PatchModKey, SkyrimRelease.SkyrimSE);
        var target = new ArmorAddon(new FormKey(PatchModKey, 0x820), SkyrimRelease.SkyrimSE)
        {
            FirstPersonModel = new GenderedItem<Model?>(
                Model("TargetFirstMale.nif", 11, null),
                Model("TargetFirstFemale.nif", 12, null)),
            WorldModel = new GenderedItem<Model?>(
                Model("TargetWorldMale.nif", 13, null),
                Model("TargetWorldFemale.nif", 14, null))
        };
        patch.ArmorAddons.Add(target);
        var handlers = new ArmorAddonRecordHandler().PropertyHandlers;
        var properties = new[]
        {
            "FirstPersonModel.Male.AlternateTextures",
            "FirstPersonModel.Female.AlternateTextures",
            "WorldModel.Male.AlternateTextures",
            "WorldModel.Female.AlternateTextures"
        };
        foreach (var property in properties)
        {
            handlers[property].SetValue(target, handlers[property].GetValue(overlayAddon));
        }

        using var patchStream = Write(patch);
        using var patchOverlay = SkyrimMod.CreateFromBinaryOverlay(patchStream, SkyrimRelease.SkyrimSE, PatchModKey);
        var written = Assert.Single(patchOverlay.ArmorAddons);

        AssertModel(written.FirstPersonModel!.Male!, Path.Combine("Meshes", "TargetFirstMale.nif"), 11, [("Body", 2, 0x201u), ("Body", 7, 0x202u)]);
        AssertModel(written.FirstPersonModel.Female!, Path.Combine("Meshes", "TargetFirstFemale.nif"), 12, [("FemaleBody", 3, 0x203u)]);
        AssertModel(written.WorldModel!.Male!, Path.Combine("Meshes", "TargetWorldMale.nif"), 13, [("MaleWorld", 4, 0x204u)]);
        AssertModel(written.WorldModel.Female!, Path.Combine("Meshes", "TargetWorldFemale.nif"), 14, [("FemaleWorld", 5, 0x205u)]);
    }

    private static BodyPart BodyPart(string partNode, float damageMult, byte healthPercent) => new()
    {
        PartNode = partNode,
        DamageMult = damageMult,
        HealthPercent = healthPercent
    };

    private static TintLayer TintLayer(ushort index, Color color, float interpolation, short preset) => new()
    {
        Index = index,
        Color = color,
        InterpolationValue = interpolation,
        Preset = preset
    };

    private static Model Model(string path, byte data, ExtendedList<AlternateTexture>? alternateTextures) => new()
    {
        File = new AssetLink<SkyrimModelAssetType>(path),
        Data = new MemorySlice<byte>([data]),
        AlternateTextures = alternateTextures
    };

    private static ExtendedList<AlternateTexture> AlternateTextures(
        params (string Name, int Index, uint TextureId)[] entries)
    {
        var result = new ExtendedList<AlternateTexture>();
        foreach (var entry in entries)
        {
            result.Add(new AlternateTexture
            {
                Name = entry.Name,
                Index = entry.Index,
                NewTexture = new FormLink<ITextureSetGetter>(new FormKey(SourceModKey, entry.TextureId))
            });
        }

        return result;
    }

    private static void AssertModel(
        IModelGetter model,
        string expectedPath,
        byte expectedData,
        IReadOnlyList<(string Name, int Index, uint TextureId)> expectedTextures)
    {
        Assert.Equal(expectedPath, model.File.DataRelativePath.ToString());
        Assert.Equal(new byte[] { expectedData }, model.Data!.Value.ToArray());
        Assert.Equal(expectedTextures.Count, model.AlternateTextures!.Count);
        for (var index = 0; index < expectedTextures.Count; index++)
        {
            var expected = expectedTextures[index];
            var actual = model.AlternateTextures[index];
            Assert.Equal(expected.Name, actual.Name);
            Assert.Equal(expected.Index, actual.Index);
            Assert.Equal(new FormKey(SourceModKey, expected.TextureId), actual.NewTexture.FormKey);
        }
    }

    private static MemoryStream Write(SkyrimMod mod)
    {
        var stream = new MemoryStream();
        mod.WriteToBinary(stream);
        stream.Position = 0;
        return stream;
    }
}
