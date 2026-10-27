using DreadsMashedPatch.App.Models;
using DreadsMashedPatch.AssetForwarding;
using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class AssetRuleHelpCatalogTests
{

    [Fact]
    public void RetainedHelpMetadataIncludesEveryRuntimeGroupAndItsExactRecordOwners()
    {
        var families = AssetRuleHelpCatalog.Families;
        Assert.Equal(Enum.GetNames<ForwardingRule>().Order(), families.Keys.Order());
        var expected = typeof(AbstractRecordHandler).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(AbstractRecordHandler).IsAssignableFrom(type))
            .SelectMany(type => AssetGroupCatalog.For(type).Select(group =>
                $"{group.Family}:{type.Name}:{group.Name}:{string.Join(',', group.Members)}"));
        var displayed = families.SelectMany(family => family.Value.SelectMany(entry =>
            entry.RecordHandlers.Select(type =>
                $"{family.Key}:{type.Name}:{entry.GroupName}:{string.Join(',', entry.Members)}")));
        Assert.Equal(expected.Order(), displayed.Order());
        Assert.All(families.Values.SelectMany(entries => entries), entry =>
        {
            Assert.NotEmpty(entry.RecordTypes);
            Assert.NotEmpty(entry.Properties);
            Assert.All(entry.Members, member => Assert.Contains(member, entry.Properties));
        });
    }

    [Fact]
    public void HelpExpandsAppearanceGeometryTextureAndSoundAggregates()
    {
        var npc = Find(ForwardingRule.Appearance, typeof(NpcRecordHandler), "Appearance");
        Assert.Contains("NPC_ — NPC", npc.RecordTypes);
        foreach (var field in new[] { "HeadParts", "FaceMorph", "TintLayers", "Race", "Weight", "Configuration.Flags.Female" })
            Assert.Contains(field, npc.Properties);
        Assert.DoesNotContain("HairColor", npc.Properties);
        Assert.DoesNotContain(AssetRuleHelpCatalog.Families.Values.SelectMany(entries => entries), entry =>
            entry.RecordHandlers.Contains(typeof(NpcRecordHandler)) && entry.Members.Contains("HairColor"));
        var head = Find(ForwardingRule.Meshes, typeof(HeadPartRecordHandler), "HeadMesh");
        Assert.Contains("ExtraParts", head.Properties);
        Assert.DoesNotContain("ValidRaces", head.Properties);
        var model = Find(ForwardingRule.Meshes, typeof(StaticRecordHandler), "Geometry");
        Assert.Contains("Model.File", model.Properties);
        Assert.Contains("Model.AlternateTextures", model.Properties);
        Assert.Contains("ObjectBounds", model.Properties);
        Assert.Contains("Lod", model.Properties);
        var textures = Find(ForwardingRule.Textures, typeof(TextureSetRecordHandler), "TextureDefinition");
        Assert.Contains("Diffuse", textures.Properties);
        Assert.Contains("BacklightMaskOrSpecular", textures.Properties);
        Assert.Contains("Flags", textures.Properties);
        var sound = Find(ForwardingRule.Sounds, typeof(SoundDescriptorRecordHandler), "SoundPlayback");
        Assert.Contains("SoundFiles", sound.Properties);
        Assert.DoesNotContain("Pitch", sound.Properties);
        Assert.DoesNotContain("Volume", sound.Properties);
    }

    [Fact]
    public void AnimationHelpSeparatesRigsFromGameplayAndIncludesIdleSelections()
    {
        var male = Find(ForwardingRule.Animations, typeof(RaceRecordHandler), "MaleRig");
        var female = Find(ForwardingRule.Animations, typeof(RaceRecordHandler), "FemaleRig");
        Assert.Equal(new[] { "SkeletalModel.Male", "BehaviorGraph.Male" }, male.Members);
        Assert.Equal(new[] { "SkeletalModel.Female", "BehaviorGraph.Female" }, female.Members);
        Assert.Contains("AlternateTextures", male.Properties);
        var raceEntries = AssetRuleHelpCatalog.Families[nameof(ForwardingRule.Animations)]
            .Where(entry => entry.RecordHandlers.Contains(typeof(RaceRecordHandler)));
        Assert.Equal(new[] { "BodyPartData", "BehaviorGraph.Female", "BehaviorGraph.Male", "SkeletalModel.Female", "SkeletalModel.Male" }.Order(),
            raceEntries.SelectMany(entry => entry.Members).Order());
        var idle = Find(ForwardingRule.Animations, typeof(IdleAnimationRecordHandler), "IdleReference");
        Assert.Equal(new[] { "Filename", "AnimationEvent" }, idle.Members);
        var marker = Find(ForwardingRule.Animations, typeof(IdleMarkerRecordHandler), "IdleSelection");
        Assert.Equal(new[] { "Animations", "Flags", "IdleTimer" }, marker.Members);
        var package = Find(ForwardingRule.Animations, typeof(PackageRecordHandler), "IdleSelection");
        foreach (var field in new[] { "Type", "TimerSetting", "Animations" }) Assert.Contains(field, package.Properties);
    }

    [Fact]
    public void AppearanceHelpContainsOnlyCoupledNpcFacesAndRaceHeadShape()
    {
        var entries = AssetRuleHelpCatalog.Families[nameof(ForwardingRule.Appearance)];
        Assert.Equal(new[] { typeof(NpcRecordHandler), typeof(RaceRecordHandler) },
            entries.SelectMany(entry => entry.RecordHandlers).OrderBy(type => type.Name));
        var race = Find(ForwardingRule.Appearance, typeof(RaceRecordHandler), "HeadShape");
        Assert.Contains("AvailableMorphs", race.Properties);
        Assert.Contains("MorphRace", race.Properties);
        Assert.DoesNotContain("TintMasks", race.Properties);
        Assert.DoesNotContain("BodyData", race.Properties);
        Assert.DoesNotContain("Skin", race.Properties);
    }

    [Fact]
    public void VmadAndTreeDetailsExplainTheirDifferentScopesAndPermissions()
    {
        var ordinary = Find(ForwardingRule.Scripts, typeof(WeaponRecordHandler), "VirtualMachineAdapter");
        var fragments = Find(ForwardingRule.Scripts, typeof(DialogResponseRecordHandler), "VirtualMachineAdapter");
        Assert.NotSame(ordinary, fragments);
        Assert.Contains("destination VMAD version", ordinary.Notes);
        Assert.Contains("complete VMAD adapter", fragments.Notes);
        var quest = Find(ForwardingRule.Scripts, typeof(QuestRecordHandler), "ScriptBindings");
        Assert.DoesNotContain("Stages", quest.Members);
        Assert.Contains("VirtualMachineAdapter.Aliases", quest.Members);
        Assert.Contains("independently", quest.Notes);

        var tree = Find(ForwardingRule.Trees, typeof(TreeRecordHandler), "TreeAssets");
        Assert.Equal(new[] { "MajorRecordFlagsRaw", "ModelAndBounds", "TreeData", "VirtualMachineAdapter" },
            tree.Members.Order());
        Assert.Equal("TREE — Tree", tree.RecordTypes);
        foreach (var field in new[] { "TrunkFlexibility", "BranchFlexibility", "LeafAmplitude", "LeafFrequency", "Unknown", "Model.File", "ObjectBounds", "VMAD script attachments" })
            Assert.Contains(field, tree.Properties);
        Assert.Contains("Other resource and appearance groups forward normally", tree.Notes);
        Assert.Contains("destination VMAD metadata", tree.Notes);
        Assert.DoesNotContain(AssetRuleHelpCatalog.Families.Where(pair => pair.Key != nameof(ForwardingRule.Trees))
            .SelectMany(pair => pair.Value), entry => entry.RecordHandlers.Contains(typeof(TreeRecordHandler)));
    }

    private static AssetRuleHelpEntry Find(ForwardingRule family, Type handler, string group) =>
        Assert.Single(AssetRuleHelpCatalog.Families[family.ToString()], entry =>
            entry.GroupName == group && entry.RecordHandlers.Contains(handler));
}
