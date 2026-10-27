using DreadsMashedPatch.Enums;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed partial class AssetForwardingPolicyTests
{
    [Fact]
    public void VigilantOutfitConflictKeepsOneCompletePackageAndIndependentEditorIdThroughBinaryOutput()
    {
        var creation = ModKey.FromNameAndExtension("ccmtysse002-ve.esl");
        var original = OutfitWithItems(
            new FormKey(Original, 0x0CC844), new FormKey(Original, 0x01395B),
            new FormKey(Original, 0x01395D), new FormKey(Original, 0x0BACF3));
        var earlier = OutfitWithItems(
            new FormKey(Original, 0x0CC844), new FormKey(Original, 0x10C45F),
            new FormKey(creation, 0x000D7A), new FormKey(creation, 0x000D7C));
        var expected = new[]
        {
            new FormKey(Original, 0x0BACF3), new FormKey(Original, 0x0CC848),
            new FormKey(Original, 0x0BACD7)
        };
        var winner = OutfitWithItems(expected);
        original.EditorID = winner.EditorID = "VigilantOfStendarrOutfitNoHood";
        earlier.EditorID = "IndependentOutfitName";
        var config = new PatcherConfiguration();
        config.Forwarding.EditorIdPolicy = EditorIdForwardingPolicy.StandardForwarding;

        var patch = Run(new OutfitRecordHandler(), config, original, earlier, winner);
        var saved = Assert.Single(patch.Outfits);
        Assert.Equal(expected, saved.Items!.Select(link => link.FormKey));
        Assert.Equal("IndependentOutfitName", saved.EditorID);
        Assert.Equal(4, earlier.Items!.Count);

        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        Assert.Equal(expected, Assert.Single(overlay.Outfits).Items!.Select(link => link.FormKey));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OutfitReversionRequiresPermissionForTheWholePackage(bool authorized)
    {
        var original = OutfitWithItems(new FormKey(Original, 0xA10), new FormKey(Original, 0xA11));
        var earlier = OutfitWithItems(new FormKey(Earlier, 0xA12), new FormKey(Earlier, 0xA13));
        var winner = original.DeepCopy();
        var config = new PatcherConfiguration();
        if (authorized) config.CompatibilityRules = [new VirtualMasterRule
            { TargetMod = Winner.FileName.String, VirtualMasters = [Earlier.FileName.String] }];

        var patch = Run(new OutfitRecordHandler(), config, original, earlier, winner);
        if (authorized) Assert.Empty(patch.Outfits);
        else Assert.Equal(earlier.Items!.Select(link => link.FormKey),
            Assert.Single(patch.Outfits).Items!.Select(link => link.FormKey));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OutfitCanForwardACompleteAbsentOrEmptyPackage(bool presentEmpty)
    {
        var original = OutfitWithItems(new FormKey(Original, 0xA10));
        var earlier = OutfitWithItems();
        if (!presentEmpty) earlier.Items = null;
        var winner = original.DeepCopy();

        var saved = Assert.Single(Run(new OutfitRecordHandler(), new PatcherConfiguration(),
            original, earlier, winner).Outfits);

        if (presentEmpty) Assert.Empty(Assert.IsType<ExtendedList<IFormLinkGetter<IOutfitTargetGetter>>>(saved.Items));
        else Assert.Null(saved.Items);
    }

    [Fact]
    public void OutfitReorderingAloneDoesNotCreateAnOverride()
    {
        var first = new FormKey(Original, 0xA10);
        var second = new FormKey(Original, 0xA11);
        var original = OutfitWithItems(first, second, first);
        var earlier = OutfitWithItems(first, first, second);
        var winner = original.DeepCopy();

        Assert.Empty(Run(new OutfitRecordHandler(), new PatcherConfiguration(), original, earlier, winner).Outfits);
    }

    private static Outfit OutfitWithItems(params FormKey[] items) => new(new FormKey(Original, 0x0C7891), SkyrimRelease.SkyrimSE)
    {
        Items = new ExtendedList<IFormLinkGetter<IOutfitTargetGetter>>(
            items.Select(key => new FormLink<IOutfitTargetGetter>(key)))
    };
}
