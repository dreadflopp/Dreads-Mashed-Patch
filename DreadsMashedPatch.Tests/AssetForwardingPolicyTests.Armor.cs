using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed partial class AssetForwardingPolicyTests
{
    [Theory]
    [InlineData(BipedObjectFlag.Head)]
    [InlineData(BipedObjectFlag.Hands)]
    [InlineData(null)]
    public void ArmorWornMeshesKeepSlotsWithTheirArmatureThroughBinaryOutput(BipedObjectFlag? winningSlots)
    {
        var original = ArmorWithWornMeshes(BipedObjectFlag.Head, new FormKey(Original, 0xA10));
        var earlier = ArmorWithWornMeshes(BipedObjectFlag.Hair, new FormKey(Earlier, 0xA11));
        earlier.BodyTemplate!.ArmorType = ArmorType.HeavyArmor;
        earlier.BodyTemplate.Flags = BodyTemplate.Flag.NonPlayable;
        var winningAddon = new FormKey(Winner, 0xA12);
        var winner = ArmorWithWornMeshes(winningSlots ?? default, winningAddon);
        if (winningSlots == null) winner.BodyTemplate = null;

        var patch = Run(new ArmorRecordHandler(), new PatcherConfiguration(), original, earlier, winner);
        var saved = Assert.Single(patch.Armors);
        Assert.Equal(winningAddon, Assert.Single(saved.Armature).FormKey);
        Assert.Equal(winningSlots ?? default, saved.BodyTemplate!.FirstPersonFlags);
        Assert.Equal(ArmorType.HeavyArmor, saved.BodyTemplate.ArmorType);
        Assert.Equal(BodyTemplate.Flag.NonPlayable, saved.BodyTemplate.Flags);

        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, Patch);
        var reloaded = Assert.Single(overlay.Armors);
        Assert.Equal(winningAddon, Assert.Single(reloaded.Armature).FormKey);
        Assert.Equal(winningSlots ?? default, reloaded.BodyTemplate!.FirstPersonFlags);
        Assert.Equal(ArmorType.HeavyArmor, reloaded.BodyTemplate.ArmorType);
    }

    [Theory]
    [InlineData(BipedObjectFlag.Hands)]
    [InlineData((BipedObjectFlag)(1u | (1u << (44 - 30))))]
    public void ArmorSlotOnlyChangesSelectTheirArmatureAndWholeMaskIncludingUnnamedBits(BipedObjectFlag winningSlots)
    {
        var addon = new FormKey(Original, 0xA10);
        var original = ArmorWithWornMeshes(BipedObjectFlag.Head, addon);
        var earlier = ArmorWithWornMeshes(BipedObjectFlag.Hair, new FormKey(Earlier, 0xA11));
        earlier.Value = 75;
        var winner = ArmorWithWornMeshes(winningSlots, addon);

        var saved = Assert.Single(Run(new ArmorRecordHandler(), new PatcherConfiguration(),
            original, earlier, winner).Armors);

        Assert.Equal(addon, Assert.Single(saved.Armature).FormKey);
        Assert.Equal(winningSlots, saved.BodyTemplate!.FirstPersonFlags);
        Assert.Equal(75u, saved.Value);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ArmorWornMeshReversionRequiresPermissionForTheCompletePair(bool authorized)
    {
        var originalAddon = new FormKey(Original, 0xA10);
        var earlierAddon = new FormKey(Earlier, 0xA11);
        var original = ArmorWithWornMeshes(BipedObjectFlag.Head, originalAddon);
        var earlier = ArmorWithWornMeshes(BipedObjectFlag.Hair, earlierAddon);
        var winner = original.DeepCopy();
        winner.Value = 75;
        var config = new PatcherConfiguration();
        if (authorized) config.CompatibilityRules = [new VirtualMasterRule
            { TargetMod = Winner.FileName.String, VirtualMasters = [Earlier.FileName.String] }];

        var patch = Run(new ArmorRecordHandler(), config, original, earlier, winner);
        if (authorized)
        {
            Assert.Empty(patch.Armors);
            return;
        }
        var saved = Assert.Single(patch.Armors);

        Assert.Equal(earlierAddon, Assert.Single(saved.Armature).FormKey);
        Assert.Equal(BipedObjectFlag.Hair, saved.BodyTemplate!.FirstPersonFlags);
        Assert.Equal(75u, saved.Value);
    }

    [Fact]
    public void ArmorWornMeshesCanSelectAnEmptyArmatureWithItsSlotMask()
    {
        var original = ArmorWithWornMeshes(BipedObjectFlag.Head, new FormKey(Original, 0xA10));
        var earlier = ArmorWithWornMeshes(BipedObjectFlag.Hair, new FormKey(Earlier, 0xA11));
        earlier.Value = 75;
        var winner = ArmorWithWornMeshes(BipedObjectFlag.Body);

        var saved = Assert.Single(Run(new ArmorRecordHandler(), new PatcherConfiguration(),
            original, earlier, winner).Armors);

        Assert.Empty(saved.Armature);
        Assert.Equal(BipedObjectFlag.Body, saved.BodyTemplate!.FirstPersonFlags);
        Assert.Equal(75u, saved.Value);
    }

    private static Armor ArmorWithWornMeshes(BipedObjectFlag slots, params FormKey[] addons)
    {
        var armor = new Armor(Key, SkyrimRelease.SkyrimSE)
        {
            BodyTemplate = new BodyTemplate { FirstPersonFlags = slots, ArmorType = ArmorType.LightArmor }
        };
        foreach (var addon in addons) armor.Armature.Add(new FormLink<IArmorAddonGetter>(addon));
        return armor;
    }
}
