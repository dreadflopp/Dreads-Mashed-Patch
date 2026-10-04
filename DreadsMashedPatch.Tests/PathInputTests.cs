using System.Runtime.InteropServices;
using DreadsMashedPatch.App.Models;
using DreadsMashedPatch.App.Services;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class PathInputTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), $"MashedPatchBrowse-{Guid.NewGuid():N}");

    public PathInputTests() => Directory.CreateDirectory(_folder);

    [Fact]
    public void MixedSeparatorsQuotesAndTrailingSeparatorResolveToExistingFolder()
    {
        var profile = Path.Combine(_folder, "profiles", "HexedRim");
        Directory.CreateDirectory(profile);
        var pasted = Path.Combine(_folder, "profiles") + "\\HexedRim/";

        Assert.Equal(profile, PathInput.GetExistingDirectory($"  \"{pasted}\"  "));
    }

    [Fact]
    public void FileBrowserUsesParentEvenBeforePluginsFileExists()
    {
        Assert.Equal(_folder, PathInput.GetExistingDirectory(Path.Combine(_folder, "plugins.txt"), isFilePath: true));
    }

    [Theory]
    [InlineData("")]
    [InlineData("\0invalid")]
    [InlineData("invalid\npath")]
    public void InvalidInputDoesNotPreventOpeningBrowser(string input)
    {
        var calls = new List<string>();

        var result = PathInput.Browse(input, false, initial =>
        {
            calls.Add(initial);
            return "chosen";
        });

        Assert.Equal("chosen", result);
        Assert.Equal(new[] { string.Empty }, calls);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void RejectedShellDirectoryRetriesWithoutInitialPath(bool comError)
    {
        var calls = new List<string>();

        var result = PathInput.Browse(_folder, false, initial =>
        {
            calls.Add(initial);
            if (calls.Count == 1)
            {
                if (comError) throw new COMException("The parameter is incorrect", unchecked((int)0x80070057));
                throw new ArgumentException("The parameter is incorrect");
            }
            return "chosen";
        });

        Assert.Equal("chosen", result);
        Assert.Equal(new[] { _folder, string.Empty }, calls);
    }

    [Fact]
    public void CancelDoesNotTriggerRetry()
    {
        var calls = 0;
        Assert.Null(PathInput.Browse<string?>(_folder, false, _ => { calls++; return null; }));
        Assert.Equal(1, calls);
    }

    [Fact]
    public void PersistentFailureReturnsToCallerAfterOneRetry()
    {
        var calls = 0;
        Assert.Throws<ArgumentException>(() => PathInput.Browse<string>(_folder, false, _ =>
        {
            calls++;
            throw new ArgumentException("dialog unavailable");
        }));
        Assert.Equal(2, calls);
    }

    [Fact]
    public void InvalidDataPathDoesNotThrowWhileInferringGameFolder()
    {
        var settings = new StandaloneSettings { DataFolderPath = "\0invalid" };
        settings.Normalize();
        Assert.Equal(string.Empty, settings.GameFolderPath);
    }

    [Fact]
    public void NormalizePreservesWindowsDriveAndUncPathsOnWindows()
    {
        if (!OperatingSystem.IsWindows()) return;
        Assert.Equal(@"C:\Games\Skyrim\Data", PathInput.Normalize(@"C:/Games\Skyrim/Data"));
        Assert.Equal(@"\\server\share\Data", PathInput.Normalize(@"//server/share\Data"));
    }

    public void Dispose() => Directory.Delete(_folder, recursive: true);
}
