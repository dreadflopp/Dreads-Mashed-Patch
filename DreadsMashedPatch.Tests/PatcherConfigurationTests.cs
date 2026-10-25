using DreadsMashedPatch.Enums;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class PatcherConfigurationTests
{
    [Fact]
    public void DefaultsPreserveForwardingPoliciesAndDisableStructurallyRiskyRecordTypes()
    {
        var settings = new PatcherConfiguration();

        Assert.Equal(
            new[]
            {
                typeof(IDialogBranchGetter).FullName!,
                typeof(IDialogResponsesGetter).FullName!,
                typeof(IDialogTopicGetter).FullName!,
                typeof(IDialogViewGetter).FullName!,
                typeof(INavigationMeshGetter).FullName!,
                typeof(IPackageGetter).FullName!
            },
            settings.DisabledRecordTypes.Order(StringComparer.Ordinal));
        Assert.Empty(settings.IgnoredMods);
        Assert.Empty(settings.AlwaysWinningMods);
        Assert.True(settings.Forwarding.TreatCreationClubAsVanilla);
        Assert.True(settings.Forwarding.EnforceSingleVanillaWeaponTypeKeyword);
        Assert.True(settings.Forwarding.RestrictTreeForwarding);
        Assert.NotEmpty(settings.Forwarding.VanillaWeaponTypeKeywords);
        Assert.Equal(
            EditorIdForwardingPolicy.ForwardOnlyWithOtherChanges,
            settings.Forwarding.EditorIdPolicy);
        Assert.Equal(
            ProtectionForwardingPolicy.PreferHigherWithAuthorizedDowngrades,
            settings.Forwarding.ProtectionPolicy);
        Assert.Equal(
            TamrielPersistentCellPolicy.Hybrid,
            settings.Forwarding.TamrielPersistentCellPolicy);
        Assert.False(settings.Diagnostics.DebugMode);
        Assert.Equal(PatcherLogVerbosity.ContextChanges, settings.Diagnostics.Verbosity);
    }

    [Fact]
    public void DefaultCompatibilityRulesHaveValidUniquePluginNames()
    {
        var rules = PatcherConfiguration.CreateDefaultCompatibilityRules();
        var targets = new HashSet<ModKey>();
        Assert.NotEmpty(rules);

        foreach (var rule in rules)
        {
            Assert.True(ModKey.TryFromFileName(rule.TargetMod, out var target),
                $"Invalid default rule target: {rule.TargetMod}");
            Assert.True(targets.Add(target), $"Duplicate default rule target: {rule.TargetMod}");
            foreach (var master in rule.VirtualMasters)
            {
                Assert.True(ModKey.TryFromFileName(master, out _),
                    $"Invalid virtual master for {rule.TargetMod}: {master}");
            }
        }
    }

    [Fact]
    public void DefaultCompatibilityRulesAreIndependent()
    {
        var first = new PatcherConfiguration();
        var second = new PatcherConfiguration();

        foreach (var rule in first.CompatibilityRules)
        {
            rule.VirtualMasters.Clear();
        }

        Assert.All(second.CompatibilityRules, rule => Assert.NotEmpty(rule.VirtualMasters));
    }

    [Fact]
    public void NormalizeCombinesDuplicateTargetRules()
    {
        var settings = new PatcherConfiguration
        {
            CompatibilityRules =
            [
                new VirtualMasterRule
                {
                    TargetMod = " Target.esp ",
                    VirtualMasters = ["First.esp"]
                },
                new VirtualMasterRule
                {
                    TargetMod = "target.esp",
                    VirtualMasters = ["Second.esp", "FIRST.esp"]
                }
            ]
        };

        settings.Normalize();

        var rule = Assert.Single(settings.CompatibilityRules);
        Assert.Equal("Target.esp", rule.TargetMod);
        Assert.Equal(2, rule.VirtualMasters.Count);
        Assert.Contains("First.esp", rule.VirtualMasters);
        Assert.Contains("Second.esp", rule.VirtualMasters);
    }

    [Fact]
    public void CopyCreatesAnIndependentRuntimeSnapshot()
    {
        var settings = new PatcherConfiguration
        {
            DisabledRecordTypes = ["Mutagen.Bethesda.Skyrim.IQuestGetter"],
            IgnoredMods = ["True Light.esp", "Ignored.esp"],
            AlwaysWinningMods = ["Priority.esp"],
            Forwarding = new ForwardingSettings
            {
                TreatCreationClubAsVanilla = false,
                VanillaWeaponTypeKeywords = ["01E711:Skyrim.esm", "01E712:Skyrim.esm"],
                RestrictTreeForwarding = false,
                EditorIdPolicy = EditorIdForwardingPolicy.ForwardOnlyWithOtherChanges,
                TamrielPersistentCellPolicy = TamrielPersistentCellPolicy.PreferSkyrim
            },
            Diagnostics = new DiagnosticsSettings
            {
                DebugMode = true,
                DeepDiveFormKeys = ["000800:Test.esp"],
                DeepDiveRecordSignatures = ["QUST"]
            }
        };

        var copy = settings.Copy();
        settings.DisabledRecordTypes.Clear();
        settings.IgnoredMods.Clear();
        settings.AlwaysWinningMods.Clear();
        settings.Diagnostics.DeepDiveFormKeys.Clear();
        settings.Forwarding.VanillaWeaponTypeKeywords.Clear();
        settings.Forwarding.RestrictTreeForwarding = true;

        Assert.Contains("Mutagen.Bethesda.Skyrim.IQuestGetter", copy.DisabledRecordTypes);
        Assert.Contains("Ignored.esp", copy.IgnoredMods);
        Assert.Contains("Priority.esp", copy.AlwaysWinningMods);
        Assert.False(copy.Forwarding.TreatCreationClubAsVanilla);
        Assert.False(copy.Forwarding.RestrictTreeForwarding);
        Assert.Equal(EditorIdForwardingPolicy.ForwardOnlyWithOtherChanges, copy.Forwarding.EditorIdPolicy);
        Assert.Equal(TamrielPersistentCellPolicy.PreferSkyrim, copy.Forwarding.TamrielPersistentCellPolicy);
        Assert.Contains("000800:Test.esp", copy.Diagnostics.DeepDiveFormKeys);
        Assert.Contains("QUST", copy.Diagnostics.DeepDiveRecordSignatures);
        Assert.Equal(
            ["01E711:Skyrim.esm", "01E712:Skyrim.esm"],
            copy.Forwarding.VanillaWeaponTypeKeywords.Order(StringComparer.Ordinal));
    }

    [Fact]
    public void AlwaysWinningModsPreserveOrderAndLastDuplicatePosition()
    {
        var settings = new PatcherConfiguration
        {
            AlwaysWinningMods = [" First.esp ", "Second.esp", "FIRST.esp"]
        };

        settings.Normalize();

        Assert.Equal(["Second.esp", "FIRST.esp"], settings.AlwaysWinningMods);
    }
}
