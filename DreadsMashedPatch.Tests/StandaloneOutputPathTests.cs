using System.Text.Json;
using DreadsMashedPatch.App.Models;
using DreadsMashedPatch.App.Services;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class StandaloneOutputPathTests
{
    [Fact]
    public void EmptyPatchUsesCustomOutputAndLeavesInputAndItsSplitFilesUntouched()
    {
        var root = Path.Combine(Path.GetTempPath(), $"MashedPatchOutput-{Guid.NewGuid():N}");
        var input = Path.Combine(root, "Input");
        var output = Path.Combine(root, "Output");
        Directory.CreateDirectory(input);
        Directory.CreateDirectory(output);
        try
        {
            foreach (var name in new[] { "MashedPatch.esp", "MashedPatch_2.esp" })
            {
                File.WriteAllText(Path.Combine(input, name), "keep input");
                File.WriteAllText(Path.Combine(output, name), "old output");
            }
            var settings = new StandaloneSettings { DataFolderPath = input, OutputFolderPath = output };

            var result = new PatcherRunner().CreateEmptyOutput(settings);

            Assert.Equal(Path.Combine(output, "MashedPatch.esp"), result.OutputPath);
            Assert.Equal(2, result.RemovedOutputCount);
            Assert.False(File.Exists(Path.Combine(output, "MashedPatch_2.esp")));
            Assert.Equal("keep input", File.ReadAllText(Path.Combine(input, "MashedPatch.esp")));
            Assert.Equal("keep input", File.ReadAllText(Path.Combine(input, "MashedPatch_2.esp")));
            using var plugin = SkyrimMod.CreateFromBinaryOverlay(result.OutputPath, SkyrimRelease.SkyrimSE);
            Assert.Empty(plugin.ModHeader.MasterReferences);
        }
        finally
        {
            Directory.Delete(root, recursive: true);
        }
    }

    [Fact]
    public void OldSettingsRequireExplicitOutputInsteadOfReusingStagingInput()
    {
        var settings = JsonSerializer.Deserialize<StandaloneSettings>(
            "{\"SettingsVersion\":4,\"DataFolderPath\":\"manager-staging\"}")!;
        settings.Normalize();

        Assert.Equal(5, settings.SettingsVersion);
        Assert.Throws<ArgumentException>(() => PatcherRunner.GetOutputPath(settings));
    }
}
