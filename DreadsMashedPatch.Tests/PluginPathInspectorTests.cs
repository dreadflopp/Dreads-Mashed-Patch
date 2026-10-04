using Mutagen.Bethesda;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class PluginPathInspectorTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"MashedPatchPaths-{Guid.NewGuid():N}");
    private string Game => Path.Combine(_root, "Stock Game");
    private string Data => Path.Combine(Game, "Data");
    private string Output => Path.Combine(_root, "Patch Output");
    private string Plugins => Path.Combine(_root, "plugins.txt");

    public PluginPathInspectorTests()
    {
        Directory.CreateDirectory(Data);
        Directory.CreateDirectory(Output);
        File.WriteAllText(Path.Combine(Game, "SkyrimSE.exe"), "");
        AddPlugin("Skyrim.esm");
        File.WriteAllText(Plugins, "");
    }

    [Fact]
    public void ImplicitAndCreationClubPluginsDoNotNeedExplicitListings()
    {
        AddPlugin("Update.esm");
        AddPlugin("ccTest.esl");
        AddPlugin("Enabled.esp");
        File.WriteAllText(Path.Combine(Game, "Skyrim.ccc"), "ccTest.esl\nccNotInstalled.esl\n");
        File.WriteAllText(Plugins, "# profile\n*Enabled.esp # comment\nDisabled.esp\n");

        var result = Inspect();

        Assert.Equal(0, result.ErrorCount);
        Assert.Equal(0, result.WarningCount);
        Assert.Contains("Installed Creation Club plugins: 1", result.Report);
        Assert.Contains("Disabled.esp [disabled; missing", result.Report);
        Assert.Contains("Update.esm [implicit]", result.Report);
    }

    [Fact]
    public void MissingEnabledPluginReportsDeploymentAndStockGameGuidance()
    {
        File.WriteAllText(Plugins, "*Missing.esp\n");

        var result = Inspect();

        Assert.Equal(1, result.ErrorCount);
        Assert.Contains("Enabled plugin missing from input folder: Missing.esp", result.Report);
        Assert.Contains("Stock Game", result.Report);
    }

    [Fact]
    public void MissingOutputAndLaterListingsAreExcludedFromPatchInput()
    {
        AddPlugin("Before.esp");
        File.WriteAllText(Plugins, "*Before.esp\n*MashedPatch.esp\n*After.esp\n");

        var result = Inspect();

        Assert.Equal(0, result.ErrorCount);
        Assert.Equal(0, result.WarningCount);
        Assert.Contains("After.esp [output or later listing; excluded", result.Report);
    }

    [Fact]
    public void UnlistedPluginsWarnButDisabledAndSplitOutputPluginsDoNot()
    {
        AddPlugin("Unlisted.esp");
        AddPlugin("Disabled.esp");
        AddPlugin("MashedPatch_2.esp");
        File.WriteAllText(Plugins, "Disabled.esp\n");

        var result = Inspect();

        Assert.Equal(0, result.ErrorCount);
        Assert.Equal(1, result.WarningCount);
        Assert.Contains("absent from plugins.txt: Unlisted.esp", result.Report);
    }

    [Fact]
    public void CaseDifferencesMatchPluginIdentityButCheckActualFilesystemAccessibility()
    {
        AddPlugin("Example.ESP");
        File.WriteAllText(Plugins, "*example.esp\n");

        var result = Inspect();

        Assert.DoesNotContain("Enabled plugin missing", result.Report);
        Assert.Equal(0, result.WarningCount);
        if (File.Exists(Path.Combine(Data, "example.esp")))
            Assert.Equal(0, result.ErrorCount);
        else
        {
            Assert.Equal(1, result.ErrorCount);
            Assert.Contains("different filename casing", result.Report);
        }
    }

    [Fact]
    public void MalformedPluginsFileUsesMutagenLineDiagnostics()
    {
        File.WriteAllText(Plugins, "# profile\n*not-a-plugin\n");

        var error = Assert.Throws<InvalidDataException>(() => Inspect());

        Assert.Contains("line 2", error.Message);
    }

    [Fact]
    public void MissingInputAndOutputPathsProduceActionableReport()
    {
        var result = PluginPathInspector.Inspect(GameRelease.SkyrimSE, Game, Path.Combine(_root, "missing"),
            Plugins, "", "MashedPatch.esp");

        Assert.True(result.ErrorCount >= 2);
        Assert.Contains("Select an existing plugin input folder", result.Report);
        Assert.Contains("Select an existing patch output folder", result.Report);
    }

    [Fact]
    public void DuplicateListingWarnsAndUsesFirstEntryForEnabledStatus()
    {
        File.WriteAllText(Plugins, "Missing.esp\n*Missing.esp\n");

        var result = Inspect();

        Assert.Equal(0, result.ErrorCount);
        Assert.Equal(1, result.WarningCount);
        Assert.Contains("Duplicate plugins.txt entries", result.Report);
        Assert.Contains("duplicate; first listing takes precedence", result.Report);
    }

    [Fact]
    public void CreationClubListedAfterOutputStillAppearsAsAutomaticallyIncluded()
    {
        AddPlugin("ccTest.esl");
        File.WriteAllText(Path.Combine(Game, "Skyrim.ccc"), "ccTest.esl\n");
        File.WriteAllText(Plugins, "*MashedPatch.esp\nccTest.esl\n");

        var result = Inspect();

        Assert.Equal(0, result.ErrorCount);
        Assert.Equal(0, result.WarningCount);
        Assert.Contains("ccTest.esl [included implicitly or via Creation Club; found]", result.Report);
    }

    [Fact]
    public void AutomaticallyIncludedPluginsUseCanonicalPathsInsteadOfRawListingCasing()
    {
        AddPlugin("ccTest.esl");
        File.WriteAllText(Path.Combine(Game, "Skyrim.ccc"), "ccTest.esl\n");
        File.WriteAllText(Plugins, "skyrim.esm\nCCTEST.ESL\n");

        var result = Inspect();

        Assert.Equal(0, result.ErrorCount);
        Assert.Equal(0, result.WarningCount);
    }

    [Fact]
    public void CaseDuplicateFilesAreReportedAsAmbiguousOnCaseSensitiveFilesystems()
    {
        AddPlugin("Example.esp");
        AddPlugin("EXAMPLE.ESP");
        File.WriteAllText(Plugins, "*Example.esp\n");

        var result = Inspect();

        if (Directory.GetFiles(Data).Length == 3)
        {
            Assert.Equal(1, result.ErrorCount);
            Assert.Contains("Multiple files represent", result.Report);
        }
        else Assert.Equal(0, result.ErrorCount);
    }

    [Fact]
    public void VerificationDoesNotChangeInputOrOutputFiles()
    {
        AddPlugin("Before.esp");
        File.WriteAllText(Plugins, "*Before.esp\n");
        var oldOutput = Path.Combine(Output, "MashedPatch.esp");
        File.WriteAllText(oldOutput, "previous output");
        var before = Directory.GetFiles(_root, "*", SearchOption.AllDirectories)
            .ToDictionary(path => path, File.ReadAllBytes);

        Inspect();

        Assert.Equal(before.Count, Directory.GetFiles(_root, "*", SearchOption.AllDirectories).Length);
        foreach (var (path, bytes) in before) Assert.Equal(bytes, File.ReadAllBytes(path));
    }

    private PluginPathInspection Inspect() => PluginPathInspector.Inspect(
        GameRelease.SkyrimSE, Game, Data, Plugins, Output, "MashedPatch.esp");

    private void AddPlugin(string name) => File.WriteAllText(Path.Combine(Data, name), "plugin placeholder");

    public void Dispose() => Directory.Delete(_root, recursive: true);
}
