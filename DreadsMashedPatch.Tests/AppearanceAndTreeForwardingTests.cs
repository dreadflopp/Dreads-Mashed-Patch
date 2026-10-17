using System.Text.Json;
using DreadsMashedPatch.Enums;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.HeadPart;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Npc;
using DreadsMashedPatch.PropertyHandlers.Race;
using DreadsMashedPatch.PropertyHandlers.Tree;
using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("AlwaysWinningModTests")]
public sealed class AppearanceAndTreeForwardingTests : IDisposable
{
    private static readonly ModKey OriginalKey = ModKey.FromNameAndExtension("Skyrim.esm");
    private static readonly ModKey EarlierKey = ModKey.FromNameAndExtension("Earlier.esp");
    private static readonly ModKey WinnerKey = ModKey.FromNameAndExtension("Winner.esp");
    private static readonly ModKey PatchKey = ModKey.FromNameAndExtension("AppearancePatch.esp");
    private static readonly FormKey RecordKey = new(OriginalKey, 0x800);

    public AppearanceAndTreeForwardingTests() => PatcherSettings.Apply(new PatcherConfiguration());
    public void Dispose() => PatcherSettings.Apply(new PatcherConfiguration());

    [Fact]
    public void NpcKeepsWinningAppearanceAndTraitsWhileForwardingGameplayAndProtection()
    {
        var original = new Npc(RecordKey, SkyrimRelease.SkyrimSE)
        {
            FaceMorph = new NpcFaceMorph(), FaceParts = new NpcFaceParts(), Weight = 20
        };
        var earlier = original.DeepCopy();
        earlier.Class.SetTo(new FormKey(EarlierKey, 0xA00));
        earlier.HeadParts.Add(new FormLink<IHeadPartGetter>(new FormKey(EarlierKey, 0xA01)));
        earlier.TintLayers.Add(new TintLayer { Index = 1, Preset = 2 });
        earlier.FaceMorph!.NoseLongVsShort = 1;
        earlier.FaceParts!.Eyes = 2;
        earlier.Configuration.Flags = NpcConfiguration.Flag.Female | NpcConfiguration.Flag.Essential;
        earlier.Configuration.TemplateFlags = NpcConfiguration.TemplateFlag.Traits | NpcConfiguration.TemplateFlag.Stats;
        earlier.Template.SetTo(new FormKey(EarlierKey, 0xA02));
        earlier.Weight = 70;
        var winner = original.DeepCopy();
        winner.HeadParts.Add(new FormLink<IHeadPartGetter>(new FormKey(WinnerKey, 0xA01)));
        winner.HeadParts.Add(new FormLink<IHeadPartGetter>(new FormKey(WinnerKey, 0xA01)));
        winner.TintLayers.Add(new TintLayer { Index = 3, Preset = 4 });
        winner.Template.SetTo(new FormKey(WinnerKey, 0xA02));
        winner.Race.SetTo(new FormKey(WinnerKey, 0xA03));
        winner.WornArmor.SetTo(new FormKey(WinnerKey, 0xA04));
        winner.Configuration.Flags = NpcConfiguration.Flag.Invulnerable;

        var patch = Run(new NpcRecordHandler(), original, earlier, winner);
        var result = Assert.Single(patch.Npcs);
        Assert.True(new NpcAppearanceHandler().AreValuesEqual(
            new NpcAppearanceHandler().GetValue(winner), new NpcAppearanceHandler().GetValue(result)));
        Assert.Equal(earlier.Class.FormKey, result.Class.FormKey);
        Assert.True(result.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Essential));
        Assert.True(result.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Invulnerable));
        Assert.False(result.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Female));
        Assert.True(result.Configuration.TemplateFlags.HasFlag(NpcConfiguration.TemplateFlag.Stats));
        Assert.False(result.Configuration.TemplateFlags.HasFlag(NpcConfiguration.TemplateFlag.Traits));
        Assert.Equal(2, result.HeadParts.Count);
        Assert.Equal(0, result.FaceMorph!.NoseLongVsShort);
    }

    [Fact]
    public void RaceHeadDefinitionsAndTheirCountClampsAndMorphRaceStayTogether()
    {
        var original = new Race(RecordKey, SkyrimRelease.SkyrimSE)
        {
            HeadData = new GenderedItem<HeadData?>(new HeadData(), new HeadData()),
            NumberOfTintsInList = 1, FacegenFaceClamp = 1, FacegenMainClamp = 1
        };
        var earlier = original.DeepCopy();
        earlier.BaseCarryWeight = 90;
        earlier.HeadData!.Male!.TintMasks.Add(new TintAssets { Index = 2 });
        earlier.NumberOfTintsInList = 2;
        earlier.FacegenMainClamp = 9;
        earlier.MorphRace.SetTo(new FormKey(EarlierKey, 0xA00));
        var winner = original.DeepCopy();
        winner.HeadData!.Female!.TintMasks.Add(new TintAssets { Index = 3 });

        var result = Assert.Single(Run(new RaceRecordHandler(), original, earlier, winner).Races);
        var handler = new RaceAppearanceHandler();
        Assert.True(handler.AreValuesEqual(handler.GetValue(winner), handler.GetValue(result)));
        Assert.Equal(90, result.BaseCarryWeight);
        Assert.Empty(result.HeadData!.Male!.TintMasks);
        Assert.Equal((ushort?)3, Assert.Single(result.HeadData.Female!.TintMasks).Index);
    }

    [Fact]
    public void WinningTraitsInheritanceRestoresItsOriginalTemplateWithoutMasterPermission()
    {
        var original = new Npc(RecordKey, SkyrimRelease.SkyrimSE);
        original.Template.SetTo(new FormKey(OriginalKey, 0xA00));
        var earlier = original.DeepCopy();
        earlier.Template.SetTo(new FormKey(EarlierKey, 0xA00));
        earlier.Configuration.TemplateFlags = NpcConfiguration.TemplateFlag.Traits;
        earlier.Class.SetTo(new FormKey(EarlierKey, 0xA01));
        var winner = original.DeepCopy();
        winner.Configuration.TemplateFlags = NpcConfiguration.TemplateFlag.Traits;

        var result = Assert.Single(Run(new NpcRecordHandler(), original, earlier, winner).Npcs);
        Assert.Equal(original.Template.FormKey, result.Template.FormKey);
        Assert.True(result.Configuration.TemplateFlags.HasFlag(NpcConfiguration.TemplateFlag.Traits));
        Assert.Equal(earlier.Class.FormKey, result.Class.FormKey);
    }

    [Fact]
    public void StoppingTraitsInheritanceKeepsGameplayTemplateAndUsesWinningFace()
    {
        var original = new Npc(RecordKey, SkyrimRelease.SkyrimSE) { FaceMorph = new NpcFaceMorph() };
        var earlier = original.DeepCopy();
        earlier.Template.SetTo(new FormKey(EarlierKey, 0xA00));
        earlier.Configuration.TemplateFlags = NpcConfiguration.TemplateFlag.Traits | NpcConfiguration.TemplateFlag.Stats;
        earlier.FaceMorph!.NoseLongVsShort = 4;
        var winner = original.DeepCopy();

        var result = Assert.Single(Run(new NpcRecordHandler(), original, earlier, winner).Npcs);
        Assert.Equal(earlier.Template.FormKey, result.Template.FormKey);
        Assert.False(result.Configuration.TemplateFlags.HasFlag(NpcConfiguration.TemplateFlag.Traits));
        Assert.True(result.Configuration.TemplateFlags.HasFlag(NpcConfiguration.TemplateFlag.Stats));
        Assert.Equal(0, result.FaceMorph!.NoseLongVsShort);
    }

    [Fact]
    public void GameplayTemplateRestorationStillUsesVirtualMasterPermissions()
    {
        var original = new Npc(RecordKey, SkyrimRelease.SkyrimSE);
        var earlier = original.DeepCopy();
        earlier.Template.SetTo(new FormKey(EarlierKey, 0xA00));
        earlier.Configuration.TemplateFlags = NpcConfiguration.TemplateFlag.Stats;
        var winner = original.DeepCopy();
        PatcherSettings.Apply(new PatcherConfiguration
        {
            CompatibilityRules = [new VirtualMasterRule { TargetMod = WinnerKey.ToString(), VirtualMasters = [EarlierKey.ToString()] }]
        });
        Assert.Empty(Run(new NpcRecordHandler(), original, earlier, winner).Npcs);
    }

    [Fact]
    public void HeadPartKeepsWinningMeshMorphsExtraPartsAndFlagsWhileForwardingName()
    {
        var original = new HeadPart(RecordKey, SkyrimRelease.SkyrimSE) { Name = "Original" };
        var earlier = original.DeepCopy();
        earlier.Name = "Translated name";
        earlier.ExtraParts.Add(new FormLink<IHeadPartGetter>(new FormKey(EarlierKey, 0xA00)));
        earlier.Parts.Add(new Part { PartType = Part.PartTypeEnum.Tri,
            FileName = new AssetLink<SkyrimDeformedModelAssetType>("Meshes/FromA.tri") });
        earlier.Flags = HeadPart.Flag.Male;
        var winner = original.DeepCopy();
        winner.ExtraParts.Add(new FormLink<IHeadPartGetter>(new FormKey(WinnerKey, 0xA00)));
        winner.Parts.Add(new Part { PartType = Part.PartTypeEnum.ChargenMorph,
            FileName = new AssetLink<SkyrimDeformedModelAssetType>("Meshes/FromB.tri") });
        winner.Model = Model("Meshes/HeadB.nif");
        winner.Flags = HeadPart.Flag.Female;

        var result = Assert.Single(Run(new HeadPartRecordHandler(), original, earlier, winner).HeadParts);
        var handler = new HeadPartAppearanceHandler();
        Assert.True(handler.AreValuesEqual(handler.GetValue(winner), handler.GetValue(result)));
        Assert.Equal("Translated name", result.Name!.String);
        Assert.Single(result.ExtraParts);
        Assert.Single(result.Parts);
    }

    [Fact]
    public void NpcAppearanceCopyClearsAbsentSubrecordsAndDoesNotShareMutableLists()
    {
        var source = new Npc(RecordKey, SkyrimRelease.SkyrimSE);
        source.HeadParts.Add(new FormLink<IHeadPartGetter>(new FormKey(OriginalKey, 0xA00)));
        source.TintLayers.Add(new TintLayer { Index = 2, Preset = 4 });
        var handler = new NpcAppearanceHandler();
        var value = handler.GetValue(source)!;
        source.HeadParts.Clear();
        source.TintLayers[0].Preset = 8;
        var target = new Npc(new FormKey(PatchKey, 0x800), SkyrimRelease.SkyrimSE)
        {
            FaceMorph = new NpcFaceMorph { NoseLongVsShort = 4 }, FaceParts = new NpcFaceParts { Eyes = 4 }
        };
        target.Configuration.HealthOffset = 42;
        target.Configuration.Level = new NpcLevel { Level = 88 };
        target.Configuration.Flags = NpcConfiguration.Flag.Essential;
        handler.SetValue(target, value);
        Assert.Null(target.FaceMorph);
        Assert.Null(target.FaceParts);
        Assert.Single(target.HeadParts);
        Assert.Equal((short?)4, Assert.Single(target.TintLayers).Preset);
        Assert.Equal(42, target.Configuration.HealthOffset);
        Assert.Equal(88, Assert.IsType<NpcLevel>(target.Configuration.Level).Level);
        Assert.True(target.Configuration.Flags.HasFlag(NpcConfiguration.Flag.Essential));
        target.TintLayers[0].Preset = 7;
        Assert.Equal((short?)4, Assert.Single(value.Snapshot.TintLayers).Preset);
    }

    [Fact]
    public void AppearanceGroupsHaveNoParallelRegistrationsForTheirMembers()
    {
        foreach (var recordHandler in new AbstractRecordHandler[]
                 { new NpcRecordHandler(), new RaceRecordHandler(), new HeadPartRecordHandler(), new TreeRecordHandler() })
        foreach (var group in recordHandler.PropertyHandlers.Values.OfType<IPropertyGroupHandler>())
        foreach (var property in group.GroupedProperties)
            Assert.DoesNotContain(property, recordHandler.PropertyHandlers.Keys);
    }

    [Fact]
    public void RaceAppearanceOverlayCopyPreservesNullableGendersAndLeavesBodyValuesAlone()
    {
        var source = new SkyrimMod(OriginalKey, SkyrimRelease.SkyrimSE);
        var race = new Race(RecordKey, SkyrimRelease.SkyrimSE)
        {
            HeadData = new GenderedItem<HeadData?>(null, new HeadData()), NumberOfTintsInList = 2,
            FacegenMainClamp = 3, FacegenFaceClamp = 4
        };
        race.HeadData.Female!.Model = Model("Meshes/FemaleHead.nif");
        race.HeadData.Female.TintMasks.Add(new TintAssets
            { Index = 2, FileName = new AssetLink<SkyrimTextureAssetType>("Textures/FaceTint.dds") });
        race.HeadData.Female.TintMasks[0].Presets.Add(new TintPreset { Index = 5, DefaultValue = 0.5f });
        source.Races.Add(race);
        using var input = new MemoryStream();
        source.WriteToBinary(input);
        input.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(input, SkyrimRelease.SkyrimSE, OriginalKey);
        var handler = new RaceAppearanceHandler();
        var value = handler.GetValue(Assert.Single(overlay.Races))!;
        var target = new Race(new FormKey(PatchKey, 0x800), SkyrimRelease.SkyrimSE)
            { Height = new GenderedItem<float>(7, 8), BaseMass = 9 };
        handler.SetValue(target, value);
        Assert.Equal(7, target.Height.Male);
        Assert.Equal(8, target.Height.Female);
        Assert.Equal(9, target.BaseMass);
        Assert.True(handler.AreValuesEqual(value, handler.GetValue(target)));
        target.HeadData!.Female!.TintMasks[0].Presets[0].DefaultValue = 0.75f;
        Assert.Equal(0.5f, value.HeadData!.Female!.TintMasks[0].Presets[0].DefaultValue);
        handler.SetValue(target, value);
        var outputMod = new SkyrimMod(PatchKey, SkyrimRelease.SkyrimSE);
        outputMod.Races.Add(target);
        using var output = new MemoryStream();
        outputMod.WriteToBinary(output);
        output.Position = 0;
        using var reloaded = SkyrimMod.CreateFromBinaryOverlay(output, SkyrimRelease.SkyrimSE, PatchKey);
        Assert.True(handler.AreValuesEqual(value, handler.GetValue(Assert.Single(reloaded.Races))));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("1")]
    [InlineData("\"Coordinated\"")]
    [InlineData("999")]
    public void TreesStayEnabledAndPreserveWinningAppearanceWhenLoadingOldSettings(string? obsoletePolicy)
    {
        var json = obsoletePolicy is null
            ? "{}"
            : "{\"Forwarding\":{\"TreePolicy\":" + obsoletePolicy + "}}";
        var settings = JsonSerializer.Deserialize<PatcherConfiguration>(json)!.Copy();
        Assert.DoesNotContain(typeof(ITreeGetter).FullName!, settings.DisabledRecordTypes);
        Assert.True(settings.Forwarding.RestrictTreeForwarding);
        using var savedSettings = JsonDocument.Parse(JsonSerializer.Serialize(settings));
        Assert.False(savedSettings.RootElement.GetProperty("Forwarding").TryGetProperty("TreePolicy", out _));
        PatcherSettings.Apply(settings);

        var original = Tree("Meshes/Original.nif", 1);
        original.ObjectBounds.Second = new P3Int16(1, 1, 1);
        var earlier = Tree("Meshes/Replacer.nif", 9);
        earlier.Name = "Harvestable tree";
        earlier.ObjectBounds.Second = new P3Int16(7, 7, 7);
        earlier.Unknown = Enumerable.Repeat((byte)2, 32).ToArray();
        earlier.MajorRecordFlagsRaw = 1 << 15;
        earlier.VirtualMachineAdapter = new VirtualMachineAdapter();
        earlier.VirtualMachineAdapter.Scripts.Add(new ScriptEntry { Name = "ReplacerScript" });
        var winner = original.DeepCopy();
        winner.ObjectBounds.Second = new P3Int16(3, 3, 3);
        winner.TrunkFlexibility = 2;
        winner.VirtualMachineAdapter = new VirtualMachineAdapter { Version = 4, ObjectFormat = 1 };
        winner.VirtualMachineAdapter.Scripts.Add(new ScriptEntry { Name = "WinnerScript" });

        var result = Assert.Single(Run(new TreeRecordHandler(), original, earlier, winner).Trees);
        Assert.Equal("Harvestable tree", result.Name!.String);
        Assert.True(new ModelBoundsHandler().AreValuesEqual(
            new ModelBoundsHandler().GetValue(winner), new ModelBoundsHandler().GetValue(result)));
        Assert.True(new TreeDataHandler().AreValuesEqual(
            new TreeDataHandler().GetValue(winner), new TreeDataHandler().GetValue(result)));
        Assert.Equal(winner.MajorRecordFlagsRaw, result.MajorRecordFlagsRaw);
        Assert.Equal("WinnerScript", Assert.Single(result.VirtualMachineAdapter!.Scripts).Name);
        Assert.Equal(4, result.VirtualMachineAdapter.Version);
        Assert.Equal(1, result.VirtualMachineAdapter.ObjectFormat);
    }

    [Fact]
    public void RestrictedTreeForwardingOnlyForwardsMetadataAndCompleteSeasonalHarvesting()
    {
        var original = Tree("Meshes/Original.nif", 1);
        original.Name = "Original";
        var earlier = original.DeepCopy();
        earlier.Name = "Harvestable tree";
        earlier.Ingredient.SetTo(new FormKey(EarlierKey, 0xA00));
        earlier.HarvestSound.SetTo(new FormKey(EarlierKey, 0xA01));
        earlier.Production = new SeasonalIngredientProduction { Spring = 1, Summer = 2, Fall = 3, Winter = 4 };
        earlier.MajorRecordFlagsRaw = (1 << 15) | (1 << 25);
        earlier.TrunkFlexibility = 9;
        earlier.VirtualMachineAdapter = new VirtualMachineAdapter();
        earlier.VirtualMachineAdapter.Scripts.Add(new ScriptEntry { Name = "EarlierScript" });
        var winner = original.DeepCopy();
        winner.Model = Model("Meshes/Winner.nif");
        winner.ObjectBounds.Second = new P3Int16(2, 3, 4);
        winner.Unknown = Enumerable.Repeat((byte)7, 32).ToArray();
        winner.VirtualMachineAdapter = new VirtualMachineAdapter { Version = 4, ObjectFormat = 1 };
        winner.VirtualMachineAdapter.Scripts.Add(new ScriptEntry { Name = "WinnerScript" });

        var result = Assert.Single(Run(new TreeRecordHandler(), original, earlier, winner).Trees);
        Assert.Equal("Harvestable tree", result.Name!.String);
        Assert.Equal(earlier.Ingredient.FormKey, result.Ingredient.FormKey);
        Assert.Equal(earlier.HarvestSound.FormKey, result.HarvestSound.FormKey);
        Assert.True(SeasonalIngredientProductionMixIn.Equals(earlier.Production, result.Production!));
        Assert.True(new ModelBoundsHandler().AreValuesEqual(
            new ModelBoundsHandler().GetValue(winner), new ModelBoundsHandler().GetValue(result)));
        Assert.True(new TreeDataHandler().AreValuesEqual(new TreeDataHandler().GetValue(winner), new TreeDataHandler().GetValue(result)));
        Assert.Equal(winner.MajorRecordFlagsRaw, result.MajorRecordFlagsRaw);
        Assert.Equal("WinnerScript", Assert.Single(result.VirtualMachineAdapter!.Scripts).Name);
        Assert.Equal(4, result.VirtualMachineAdapter.Version);
        Assert.Equal(1, result.VirtualMachineAdapter.ObjectFormat);
    }

    [Fact]
    public void RestrictedTreeForwardingDoesNotEmitAnOverrideForAppearanceOnlyConflicts()
    {
        var original = Tree("Meshes/Original.nif", 1);
        var earlier = original.DeepCopy();
        earlier.TrunkFlexibility = 9;
        earlier.MajorRecordFlagsRaw = 1 << 15;
        var winner = original.DeepCopy();
        winner.Model = Model("Meshes/Winner.nif");
        Assert.Empty(Run(new TreeRecordHandler(), original, earlier, winner).Trees);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void TreeRestrictionPersistsAndControlsAppearanceScriptsAndFlags(bool restrict)
    {
        var settings = new PatcherConfiguration
        {
            Forwarding = new ForwardingSettings { RestrictTreeForwarding = restrict }
        };
        var reloaded = JsonSerializer.Deserialize<PatcherConfiguration>(JsonSerializer.Serialize(settings))!;
        Assert.Equal(restrict, reloaded.Forwarding.RestrictTreeForwarding);
        PatcherSettings.Apply(reloaded);

        var original = Tree("Meshes/Original.nif", 1);
        original.ObjectBounds.Second = new P3Int16(1, 1, 1);
        var earlier = Tree("Meshes/Replacer.nif", 9);
        earlier.Name = "Harvestable tree";
        earlier.ObjectBounds.Second = new P3Int16(7, 7, 7);
        earlier.Unknown = Enumerable.Repeat((byte)2, 32).ToArray();
        earlier.MajorRecordFlagsRaw = (1 << 15) | (1 << 25);
        earlier.VirtualMachineAdapter = new VirtualMachineAdapter();
        earlier.VirtualMachineAdapter.Scripts.Add(new ScriptEntry { Name = "EarlierScript" });
        var winner = original.DeepCopy();
        winner.VirtualMachineAdapter = new VirtualMachineAdapter { Version = 4, ObjectFormat = 1 };
        winner.VirtualMachineAdapter.Scripts.Add(new ScriptEntry { Name = "WinnerScript" });

        var result = Assert.Single(Run(new TreeRecordHandler(), original, earlier, winner).Trees);
        var expectedAppearance = restrict ? winner : earlier;
        Assert.Equal("Harvestable tree", result.Name!.String);
        Assert.True(new ModelBoundsHandler().AreValuesEqual(
            new ModelBoundsHandler().GetValue(expectedAppearance), new ModelBoundsHandler().GetValue(result)));
        Assert.True(new TreeDataHandler().AreValuesEqual(
            new TreeDataHandler().GetValue(expectedAppearance), new TreeDataHandler().GetValue(result)));
        Assert.Equal(expectedAppearance.MajorRecordFlagsRaw, result.MajorRecordFlagsRaw);
        Assert.Equal(restrict ? new[] { "WinnerScript" } : new[] { "EarlierScript", "WinnerScript" },
            result.VirtualMachineAdapter!.Scripts.Select(script => script.Name).Order(StringComparer.Ordinal));
        Assert.Equal(4, result.VirtualMachineAdapter.Version);
        Assert.Equal(1, result.VirtualMachineAdapter.ObjectFormat);
    }

    [Fact]
    public void UnrestrictedTreeForwardingSelectsWholeWindDataWithoutModelCoordination()
    {
        PatcherSettings.Apply(new PatcherConfiguration
        {
            Forwarding = new ForwardingSettings { RestrictTreeForwarding = false }
        });
        var original = Tree("Meshes/Original.nif", 1);
        var earlier = Tree("Meshes/Replacer.nif", 9);
        earlier.Name = "Earlier name";
        earlier.Unknown = Enumerable.Repeat((byte)2, 32).ToArray();
        var winner = original.DeepCopy();
        winner.BranchFlexibility = 3;
        winner.Unknown = Enumerable.Repeat((byte)4, 32).ToArray();

        var result = Assert.Single(Run(new TreeRecordHandler(), original, earlier, winner).Trees);
        Assert.Equal(earlier.Model!.File.GivenPath, result.Model!.File.GivenPath);
        Assert.True(new TreeDataHandler().AreValuesEqual(
            new TreeDataHandler().GetValue(winner), new TreeDataHandler().GetValue(result)));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void ExplicitPriorityModsStillSelectTheCompleteTree(bool restrict)
    {
        var original = Tree("Meshes/Original.nif", 1);
        var earlier = Tree("Meshes/Priority.nif", 9);
        var winner = original.DeepCopy();
        PatcherSettings.Apply(new PatcherConfiguration
        {
            AlwaysWinningMods = [EarlierKey.ToString()],
            Forwarding = new ForwardingSettings { RestrictTreeForwarding = restrict }
        });
        var result = Assert.Single(Run(new TreeRecordHandler(), original, earlier, winner).Trees);
        Assert.Equal(earlier.Model!.File.GivenPath, result.Model!.File.GivenPath);
        Assert.Equal(9, result.TrunkFlexibility);
    }

    [Fact]
    public void CompleteCnamSurvivesOverlayCopyAndWriteAndDoesNotShareOpaqueBytes()
    {
        var source = new SkyrimMod(OriginalKey, SkyrimRelease.SkyrimSE);
        var tree = Tree("Meshes/Original.nif", 1);
        tree.BranchFlexibility = 2;
        tree.Unknown = Enumerable.Range(3, 8).SelectMany(i => BitConverter.GetBytes((float)i)).ToArray();
        tree.LeafAmplitude = 11;
        tree.LeafFrequency = 12;
        source.Trees.Add(tree);
        using var input = new MemoryStream();
        source.WriteToBinary(input);
        input.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(input, SkyrimRelease.SkyrimSE, OriginalKey);
        var handler = new TreeDataHandler();
        var value = handler.GetValue(Assert.Single(overlay.Trees))!;
        var target = Tree("Meshes/Target.nif", 0);
        handler.SetValue(target, value);
        value.Unknown[0] ^= 0xFF;
        Assert.Equal(tree.Unknown.ToArray(), target.Unknown.ToArray());
        var outputMod = new SkyrimMod(PatchKey, SkyrimRelease.SkyrimSE);
        outputMod.Trees.Add(target);
        using var output = new MemoryStream();
        outputMod.WriteToBinary(output);
        var bytes = output.ToArray();
        var signature = System.Text.Encoding.ASCII.GetBytes("CNAM");
        var offset = Enumerable.Range(0, bytes.Length - 6).Single(i => bytes.AsSpan(i, 4).SequenceEqual(signature));
        Assert.Equal(48, BitConverter.ToUInt16(bytes, offset + 4));
        for (var i = 0; i < 12; i++) Assert.Equal(i + 1, BitConverter.ToSingle(bytes, offset + 6 + i * 4));
        output.Position = 0;
        using var reloaded = SkyrimMod.CreateFromBinaryOverlay(output, SkyrimRelease.SkyrimSE, PatchKey);
        Assert.True(handler.AreValuesEqual(handler.GetValue(tree), handler.GetValue(Assert.Single(reloaded.Trees))));
    }

    private static Model Model(string path) => new() { File = new AssetLink<SkyrimModelAssetType>(path) };
    private static Tree Tree(string path, float trunk) => new(RecordKey, SkyrimRelease.SkyrimSE)
    {
        Model = Model(path), Name = "Original", TrunkFlexibility = trunk, BranchFlexibility = 1,
        LeafAmplitude = 1, LeafFrequency = 1, Unknown = new byte[32]
    };

    private static SkyrimMod Run(AbstractRecordHandler handler, params IMajorRecord[] versions)
    {
        var keys = new[] { OriginalKey, EarlierKey, WinnerKey };
        var mods = versions.Select((record, i) =>
        {
            var mod = new SkyrimMod(keys[i], SkyrimRelease.SkyrimSE);
            if (i != 0) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = OriginalKey });
            switch (record)
            {
                case Npc npc: mod.Npcs.Add(npc); break;
                case Race race: mod.Races.Add(race); break;
                case HeadPart head: mod.HeadParts.Add(head); break;
                case Tree tree: mod.Trees.Add(tree); break;
                default: throw new ArgumentException("Unsupported test record.");
            }
            return mod;
        }).ToArray();
        var patch = new SkyrimMod(PatchKey, SkyrimRelease.SkyrimSE);
        var listings = mods.Cast<ISkyrimModGetter>().Append(patch)
            .Select(mod => new ModListing<ISkyrimModGetter>(mod)).ToArray();
        var order = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
#pragma warning disable CS0618
        using var state = new SynthesisState<ISkyrimMod, ISkyrimModGetter>(
            new RunSynthesisMutagenPatcher
            {
                OutputPath = Path.Combine(Path.GetTempPath(), "AppearancePatch.esp"),
                DataFolderPath = Path.GetTempPath(), LoadOrderFilePath = Path.Combine(Path.GetTempPath(), "unused-plugins.txt"),
                GameRelease = GameRelease.SkyrimSE
            },
            listings.Select(mod => new LoadOrderListing(mod.ModKey, mod.Enabled)).ToArray(), order,
            order.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(), null!, patch, null, null, null, CancellationToken.None, null);
#pragma warning restore CS0618
        var contexts = order.PriorityOrder.WinningContextOverrides<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(state.LinkCache).ToArray();
        var report = handler.Process(state, contexts);
        Assert.True(report.Succeeded);
        Assert.Empty(report.SkippedRecords);
        return patch;
    }
}
