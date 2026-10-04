using DreadsMashedPatch.Enums;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch;

public sealed class PatcherConfiguration
{
    /// <summary>
    /// Full interface names for record families that should not be processed.
    /// Structurally coupled dialogue, navigation, and package record families default
    /// to disabled because independently forwarded fields can create combinations that
    /// no source plugin authored or, in the case of packages, cannot currently be
    /// written without reordering indexed data.
    /// Storing exclusions keeps newly added record families enabled by default after
    /// an application update.
    /// </summary>
    public HashSet<string> DisabledRecordTypes { get; set; } = new(StringComparer.Ordinal)
    {
        typeof(IDialogTopicGetter).FullName!,
        typeof(IDialogBranchGetter).FullName!,
        typeof(IDialogResponsesGetter).FullName!,
        typeof(IDialogViewGetter).FullName!,
        typeof(INavigationMeshGetter).FullName!,
        typeof(IPackageGetter).FullName!
    };

    public ForwardingSettings Forwarding { get; set; } = new();

    public DiagnosticsSettings Diagnostics { get; set; } = new();

    /// <summary>
    /// Plugins whose overrides are omitted while conflict chains are evaluated.
    /// </summary>
    public HashSet<string> IgnoredMods { get; set; } = new(StringComparer.OrdinalIgnoreCase)
    {
    };

    /// <summary>
    /// Plugins whose complete record snapshots take precedence whenever another
    /// plugin overwrites them. If several configured plugins contain the same
    /// record, the plugin occurring last in this list takes precedence.
    /// </summary>
    public List<string> AlwaysWinningMods { get; set; } = [];

    public List<VirtualMasterRule> CompatibilityRules { get; set; } = CreateDefaultCompatibilityRules();

    public static List<VirtualMasterRule> CreateDefaultCompatibilityRules() =>
    [
        Rule("imp_helm_legend.esp", "Unofficial Skyrim Special Edition Patch.esp"),
        Rule("Navigator-NavFixes.esl", "Unofficial Skyrim Special Edition Patch.esp"),
        Rule("SurvivalModeImproved.esp", "Unofficial Skyrim Special Edition Patch.esp"),
        Rule("King-Priest.esp", "Unofficial Skyrim Special Edition Patch.esp"),
        Rule("Window Shadows Ultimate.esp", "Unofficial Skyrim Special Edition Patch.esp"),
        Rule("Dawnguard HQ.esp", "Unofficial Skyrim Special Edition Patch.esp"),
        Rule("DawnguardArsenal.esp", "Unofficial Skyrim Special Edition Patch.esp"),
        Rule("Lux.esp",
            "Unofficial Skyrim Special Edition Patch.esp",
            "Embers XD.esp",
            "NAT-ENB.esp"),
        Rule("Lux Orbis.esp",
            "Unofficial Skyrim Special Edition Patch.esp",
            "Lux.esp"),
        Rule("Lux - Great Village of Shor's Stone patch.esp", "Unofficial Skyrim Special Edition Patch.esp"),
        Rule("CS Light.esp", "Unofficial Skyrim Special Edition Patch.esp"),
        Rule("Gourmet.esp",
            "Unofficial Skyrim Special Edition Patch.esp",
            "ccQDRSSE001-SurvivalMode.esl",
            "SurvivalModeImproved.esp"),
        Rule("Reliquary of Myth.esp", "Unofficial Skyrim Special Edition Patch.esp"),
        Rule("CraftingRevamped.esp", "Unofficial Skyrim Special Edition Patch.esp"),
        Rule("BBNoKillmoves.esp",
            "Unofficial Skyrim Special Edition Patch.esp",
            "BladeAndBlunt.esp"),
        Rule("Simple Better Civil War Soldiers.esp",
            "Unofficial Skyrim Special Edition Patch.esp",
            "cutting room floor.esp"),
        Rule("AI Overhaul.esp", "Unofficial Skyrim Special Edition Patch.esp"),

        Rule("Creation Club Rebalancing.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Bittercup.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Chrysamere.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Civil War Champions.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Dawnfang.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Dead Man's Dread.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Fishing.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Forgotten Seasons.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Gallows Hall - Tweaks and Enhancements.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Gallows Hall.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Ghosts of the Tribunal - Reduced Cut.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Ghosts of the Tribunal.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Goldbrand.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Ruin's Edge.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Saints and Seducers.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Shadowrend.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Spell Knight Armor.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Stendarr's Hammer.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Sunder and Wraithguard.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Arms of Chaos.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Boots of Blinding Speed.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Bow of Shadows.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Cause.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Contest.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Crusader's Relics - Knight of the North.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Crusader's Relics.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Dragonbone Mail.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Gray Cowl.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Headman's Cleaver.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Lord's Mail.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Staff of Hasedoki.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Staff of Sheogorath - ECSS.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - The Staff of Sheogorath.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Masterwork - Umbra.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),
        Rule("Starfrost.esp", "Unofficial Skyrim Creation Club Content Patch.esl"),

        Rule("MadMen.esp", "cutting room floor.esp", "BladeAndBlunt.esp"),
        Rule("StarfrostInjuries.esp", "Apothecary.esp"),
        Rule("BS Bruma - CC Curios Patch.esp", "BSHeartland - Unofficial Fixes.esp"),
        Rule("Civil War Overhaul.esp", "Simple Better Civil War Soldiers.esp"),
        Rule("Immersive Sounds - Compendium.esp", "Audio Overhaul Skyrim.esp"),
        Rule("Aspens Ablaze.esp", "Nature of the Wild Lands.esp")
    ];

    private static VirtualMasterRule Rule(string targetMod, params string[] virtualMasters) => new()
    {
        TargetMod = targetMod,
        VirtualMasters = new HashSet<string>(virtualMasters, StringComparer.OrdinalIgnoreCase)
    };

    public void Normalize()
    {
        Forwarding ??= new ForwardingSettings();
        Forwarding.Normalize();
        Diagnostics ??= new DiagnosticsSettings();
        Diagnostics.Normalize();
        CompatibilityRules = NormalizeCompatibilityRules(CompatibilityRules);

        DisabledRecordTypes = new HashSet<string>(
            (DisabledRecordTypes ?? []).Where(x => !string.IsNullOrWhiteSpace(x)),
            StringComparer.Ordinal);
        IgnoredMods = new HashSet<string>(
            (IgnoredMods ?? []).Select(name => name.Trim()).Where(name => name.Length > 0),
            StringComparer.OrdinalIgnoreCase);
        AlwaysWinningMods = (AlwaysWinningMods ?? [])
            .Select(name => name.Trim())
            .Where(name => name.Length > 0)
            .Reverse()
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Reverse()
            .ToList();
    }

    public PatcherConfiguration Copy()
    {
        Normalize();
        return new PatcherConfiguration
        {
            DisabledRecordTypes = new HashSet<string>(DisabledRecordTypes, StringComparer.Ordinal),
            IgnoredMods = new HashSet<string>(IgnoredMods, StringComparer.OrdinalIgnoreCase),
            AlwaysWinningMods = [.. AlwaysWinningMods],
            Forwarding = new ForwardingSettings
            {
                TreatCreationClubAsVanilla = Forwarding.TreatCreationClubAsVanilla,
                EnforceSingleVanillaWeaponTypeKeyword = Forwarding.EnforceSingleVanillaWeaponTypeKeyword,
                VanillaWeaponTypeKeywords = new HashSet<string>(
                    Forwarding.VanillaWeaponTypeKeywords,
                    StringComparer.OrdinalIgnoreCase),
                EditorIdPolicy = Forwarding.EditorIdPolicy,
                ProtectionPolicy = Forwarding.ProtectionPolicy,
                TamrielPersistentCellPolicy = Forwarding.TamrielPersistentCellPolicy
            },
            Diagnostics = Diagnostics.Copy(),
            CompatibilityRules = CompatibilityRules.Select(rule => rule.Copy()).ToList()
        };
    }

    private static List<VirtualMasterRule> NormalizeCompatibilityRules(
        IEnumerable<VirtualMasterRule>? rules)
    {
        var normalized = new List<VirtualMasterRule>();
        var ruleIndexes = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var rule in rules ?? [])
        {
            if (rule is null)
            {
                continue;
            }

            rule.Normalize();
            if (ruleIndexes.TryGetValue(rule.TargetMod, out var existingIndex))
            {
                normalized[existingIndex].VirtualMasters.UnionWith(rule.VirtualMasters);
                continue;
            }

            ruleIndexes[rule.TargetMod] = normalized.Count;
            normalized.Add(rule);
        }

        return normalized;
    }
}

public sealed class VirtualMasterRule
{
    public string TargetMod { get; set; } = string.Empty;

    public HashSet<string> VirtualMasters { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public VirtualMasterRule Normalize()
    {
        TargetMod = TargetMod?.Trim() ?? string.Empty;
        VirtualMasters = new HashSet<string>(
            (VirtualMasters ?? []).Select(name => name.Trim()).Where(name => name.Length > 0),
            StringComparer.OrdinalIgnoreCase);
        return this;
    }

    public VirtualMasterRule Copy() => new()
    {
        TargetMod = TargetMod,
        VirtualMasters = new HashSet<string>(VirtualMasters, StringComparer.OrdinalIgnoreCase)
    };
}

public sealed class ForwardingSettings
{
    private static readonly string[] DefaultVanillaWeaponTypeKeywordValues =
    [
        "06D932:Skyrim.esm", // WeapTypeBattleaxe
        "01E715:Skyrim.esm", // WeapTypeBow
        "01E713:Skyrim.esm", // WeapTypeDagger
        "06D931:Skyrim.esm", // WeapTypeGreatsword
        "01E714:Skyrim.esm", // WeapTypeMace
        "01E716:Skyrim.esm", // WeapTypeStaff
        "01E711:Skyrim.esm", // WeapTypeSword
        "01E712:Skyrim.esm", // WeapTypeWarAxe
        "06D930:Skyrim.esm"  // WeapTypeWarhammer
    ];

    public bool TreatCreationClubAsVanilla { get; set; } = true;

    /// <summary>
    /// When an override successfully introduces exactly one configured weapon type keyword,
    /// that override also owns removal of the other configured types. Overrides which
    /// explicitly contain multiple configured types are preserved as authored.
    /// </summary>
    public bool EnforceSingleVanillaWeaponTypeKeyword { get; set; } = true;

    public HashSet<string> VanillaWeaponTypeKeywords { get; set; } =
        new(DefaultVanillaWeaponTypeKeywordValues, StringComparer.OrdinalIgnoreCase);

    public EditorIdForwardingPolicy EditorIdPolicy { get; set; } =
        EditorIdForwardingPolicy.ForwardOnlyWithOtherChanges;

    public ProtectionForwardingPolicy ProtectionPolicy { get; set; } =
        ProtectionForwardingPolicy.PreferHigherWithAuthorizedDowngrades;

    public TamrielPersistentCellPolicy TamrielPersistentCellPolicy { get; set; } =
        TamrielPersistentCellPolicy.Hybrid;

    public void Normalize()
    {
        VanillaWeaponTypeKeywords = new HashSet<string>(
            (VanillaWeaponTypeKeywords ?? [])
                .Select(value => value.Trim())
                .Where(value => value.Length > 0),
            StringComparer.OrdinalIgnoreCase);
    }
}

public sealed class DiagnosticsSettings
{
    public bool DebugMode { get; set; }

    public PatcherLogVerbosity Verbosity { get; set; } = PatcherLogVerbosity.ContextChanges;

    public bool IncludeNoChangeDecisionsInDetailed { get; set; } = true;

    public int MaxValuePreviewLength { get; set; } = 240;

    public HashSet<string> DeepDiveRecordSignatures { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public HashSet<string> DeepDiveFormKeys { get; set; } = new(StringComparer.OrdinalIgnoreCase);

    public void Normalize()
    {
        MaxValuePreviewLength = Math.Clamp(MaxValuePreviewLength, 40, 10_000);
        DeepDiveRecordSignatures = NormalizeSet(DeepDiveRecordSignatures);
        DeepDiveFormKeys = NormalizeSet(DeepDiveFormKeys);
    }

    public DiagnosticsSettings Copy()
    {
        Normalize();
        return new DiagnosticsSettings
        {
            DebugMode = DebugMode,
            Verbosity = Verbosity,
            IncludeNoChangeDecisionsInDetailed = IncludeNoChangeDecisionsInDetailed,
            MaxValuePreviewLength = MaxValuePreviewLength,
            DeepDiveRecordSignatures = new HashSet<string>(DeepDiveRecordSignatures, StringComparer.OrdinalIgnoreCase),
            DeepDiveFormKeys = new HashSet<string>(DeepDiveFormKeys, StringComparer.OrdinalIgnoreCase)
        };
    }

    private static HashSet<string> NormalizeSet(IEnumerable<string>? values)
    {
        return new HashSet<string>(
            (values ?? []).Select(x => x.Trim()).Where(x => x.Length > 0),
            StringComparer.OrdinalIgnoreCase);
    }
}
