using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class PatchOutputTransactionTests
{
    [Fact]
    public void CommitReplacesOldPatchAndRemovesObsoleteSplitFiles()
    {
        var folder = CreateTemporaryFolder();
        try
        {
            var primary = Path.Combine(folder, "MashedPatch.esp");
            var oldSplit = Path.Combine(folder, "MashedPatch_2.esp");
            var unrelated = Path.Combine(folder, "MashedPatch_Notes.esp");
            File.WriteAllText(primary, "old");
            File.WriteAllText(oldSplit, "old split");
            File.WriteAllText(unrelated, "keep");

            using (var transaction = new PatchOutputTransaction(primary))
            {
                File.WriteAllText(transaction.StagedOutputPath, "new");
                Assert.Equal(2, transaction.Commit());
            }

            Assert.Equal("new", File.ReadAllText(primary));
            Assert.False(File.Exists(oldSplit));
            Assert.Equal("keep", File.ReadAllText(unrelated));
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }

    [Fact]
    public void MissingPrimaryOutputLeavesPreviousPatchUntouched()
    {
        var folder = CreateTemporaryFolder();
        try
        {
            var primary = Path.Combine(folder, "MashedPatch.esp");
            var split = Path.Combine(folder, "MashedPatch_2.esp");
            File.WriteAllText(primary, "old");
            File.WriteAllText(split, "old split");

            using (var transaction = new PatchOutputTransaction(primary))
            {
                Assert.Throws<InvalidDataException>(() => transaction.Commit());
            }

            Assert.Equal("old", File.ReadAllText(primary));
            Assert.Equal("old split", File.ReadAllText(split));
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }

    [Fact]
    public void CommitPublishesNewSplitFilesTogetherWithPrimary()
    {
        var folder = CreateTemporaryFolder();
        try
        {
            var primary = Path.Combine(folder, "MashedPatch.esp");
            var split = Path.Combine(folder, "MashedPatch_2.esp");
            File.WriteAllText(primary, "old");

            using (var transaction = new PatchOutputTransaction(primary))
            {
                File.WriteAllText(transaction.StagedOutputPath, "new primary");
                File.WriteAllText(Path.Combine(
                    Path.GetDirectoryName(transaction.StagedOutputPath)!, "MashedPatch_2.esp"), "new split");
                Assert.Equal(1, transaction.Commit());
            }

            Assert.Equal("new primary", File.ReadAllText(primary));
            Assert.Equal("new split", File.ReadAllText(split));
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }

    private static string CreateTemporaryFolder()
    {
        var folder = Path.Combine(Path.GetTempPath(), $"MashedPatchTests-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        return folder;
    }
}
