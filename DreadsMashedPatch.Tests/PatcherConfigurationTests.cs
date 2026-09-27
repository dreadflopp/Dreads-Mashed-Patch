using DreadsMashedPatch.Enums;
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
        Assert.Equal(9, settings.Forwarding.VanillaWeaponTypeKeywords.Count);
        Assert.Contains("01E711:Skyrim.esm", settings.Forwarding.VanillaWeaponTypeKeywords);
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
        var compatibilityRules = settings.CompatibilityRules.ToDictionary(
            rule => rule.InjectedMaster,
            StringComparer.OrdinalIgnoreCase);
        var ussepRule = compatibilityRules["Unofficial Skyrim Special Edition Patch.esp"];
        Assert.DoesNotContain("Unofficial Skyrim Creation Club Content Patch.esl", ussepRule.TargetMods);

        var creationClubPatchRule =
            compatibilityRules["Unofficial Skyrim Creation Club Content Patch.esl"];
        Assert.Contains("Creation Club Rebalancing.esp", creationClubPatchRule.TargetMods);
        Assert.Contains("Masterwork - Bittercup.esp", creationClubPatchRule.TargetMods);
        Assert.Contains("Masterwork - Umbra.esp", creationClubPatchRule.TargetMods);

        var apothecaryRule = compatibilityRules["Apothecary.esp"];
        Assert.Contains("StarfrostInjuries.esp", apothecaryRule.TargetMods);

        var brumaUnofficialFixesRule = compatibilityRules["BSHeartland - Unofficial Fixes.esp"];
        Assert.Contains("BS Bruma - CC Curios Patch.esp", brumaUnofficialFixesRule.TargetMods);
    }

    [Fact]
    public void DefaultCompatibilityRulesAreIndependent()
    {
        var first = new PatcherConfiguration();
        var second = new PatcherConfiguration();

        foreach (var rule in first.CompatibilityRules)
        {
            rule.TargetMods.Clear();
        }

        Assert.All(second.CompatibilityRules, rule => Assert.NotEmpty(rule.TargetMods));
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

        Assert.Contains("Mutagen.Bethesda.Skyrim.IQuestGetter", copy.DisabledRecordTypes);
        Assert.Contains("Ignored.esp", copy.IgnoredMods);
        Assert.Contains("Priority.esp", copy.AlwaysWinningMods);
        Assert.False(copy.Forwarding.TreatCreationClubAsVanilla);
        Assert.Equal(EditorIdForwardingPolicy.ForwardOnlyWithOtherChanges, copy.Forwarding.EditorIdPolicy);
        Assert.Equal(TamrielPersistentCellPolicy.PreferSkyrim, copy.Forwarding.TamrielPersistentCellPolicy);
        Assert.Contains("000800:Test.esp", copy.Diagnostics.DeepDiveFormKeys);
        Assert.Contains("QUST", copy.Diagnostics.DeepDiveRecordSignatures);
        Assert.Equal(9, copy.Forwarding.VanillaWeaponTypeKeywords.Count);
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
