using DreadsMashedPatch.App.Services;
using Mutagen.Bethesda.Plugins;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("LogCollector")]
public sealed class PartialPublicationTests
{
    private static readonly FormKey Record = new(ModKey.FromNameAndExtension("Skyrim.esm"), 0x800);

    [Fact]
    public void SkippingRecordRecoversOnlyItsOwnErrorsAfterLogsHaveBeenFlushed()
    {
        LogCollector.Clear();
        using var run = new PatchDiagnostics();
        PatchDiagnostics.Error("Load", "Fatal import failure");
        using (var record = new PatchDiagnostics(Record))
        {
            LogCollector.AddError("Perks", "Bad row", new InvalidDataException("Bad payload"));
            LogCollector.Clear();
            Assert.Throws<PatchRunFailedException>(PatchDiagnostics.ThrowIfFailed);
            record.SkipRecord("NPC_ - NPC", new InvalidDataException("Bad payload"));
            record.Report.ThrowIfFailed();
        }

        Assert.True(run.Report.IsPartial);
        Assert.False(run.Report.Succeeded);
        Assert.Equal(1, run.Report.FatalErrorCount);
        Assert.Equal(Record, Assert.Single(run.Report.SkippedRecords).Record);
        Assert.False(run.Report.Errors[0].IsSkippedRecordError);
        Assert.All(run.Report.Errors.Skip(1), error => Assert.True(error.IsSkippedRecordError));
        Assert.Throws<PatchRunFailedException>(run.Report.ThrowIfFailed);
    }

    [Fact]
    public void RecordErrorRemainsFatalUntilTheRecordBoundaryAcceptsTheSkip()
    {
        using var run = new PatchDiagnostics();
        using (var record = new PatchDiagnostics(Record))
        {
            PatchDiagnostics.Error("Write", "Unrecovered record failure");
        }
        using (var record = new PatchDiagnostics(Record))
        {
            record.SkipRecord("NPC_ - NPC", new InvalidDataException("Separate attempt"));
        }
        Assert.Equal(1, run.Report.FatalErrorCount);
        Assert.Throws<PatchRunFailedException>(run.Report.ThrowIfFailed);
    }

    [Theory]
    [InlineData("fatal-report")]
    [InlineData("pipeline-error")]
    [InlineData("no-patcher")]
    [InlineData("no-primary")]
    public void DesktopHostKeepsPreviousPrimaryAndSplitFilesOnFatalFailure(string failure)
    {
        using var run = new PatchDiagnostics();
        using (var record = new PatchDiagnostics(Record))
            record.SkipRecord("NPC_ - NPC", new InvalidDataException("Bad row"));
        if (failure == "fatal-report") PatchDiagnostics.Error("Write", "Serialization failed");
        var folder = Path.Combine(Path.GetTempPath(), $"PartialPublication-{Guid.NewGuid():N}");
        Directory.CreateDirectory(folder);
        try
        {
            var output = Path.Combine(folder, "MashedPatch.esp");
            var split = Path.Combine(folder, "MashedPatch_2.esp");
            File.WriteAllText(output, "old primary");
            File.WriteAllText(split, "old split");
            using (var transaction = new PatchOutputTransaction(output))
            {
                if (failure != "no-primary") File.WriteAllText(transaction.StagedOutputPath, "new primary");
                File.WriteAllText(Path.Combine(Path.GetDirectoryName(transaction.StagedOutputPath)!, "MashedPatch_2.esp"), "new split");
                var result = new PatcherRunResult(1, 1, 1, failure == "pipeline-error" ? 1 : 0);
                void Publish() => PatcherRunner.PublishOutput(
                    transaction, failure == "no-patcher" ? null : run.Report, result);
                if (failure == "fatal-report") Assert.Throws<PatchRunFailedException>(Publish);
                else if (failure == "no-primary") Assert.Throws<InvalidDataException>(Publish);
                else Assert.Throws<InvalidOperationException>(Publish);
            }
            Assert.Equal("old primary", File.ReadAllText(output));
            Assert.Equal("old split", File.ReadAllText(split));
            Assert.Equal(2, Directory.GetFileSystemEntries(folder).Length);
        }
        finally { Directory.Delete(folder, true); }
    }

    [Theory]
    [InlineData("[Error] Write failed")]
    [InlineData("Exception: could not serialize")]
    [InlineData("Unhandled exception. InvalidDataException")]
    [InlineData("FAILED after 00:01")]
    public void DesktopStillCountsPipelineErrorsFollowingRecordErrors(string fatalLine)
    {
        var uiLog = new System.Text.StringBuilder();
        var writer = new UiTextWriter(_ => { }, text => uiLog.Append(text));
        // Exercise a diagnostic split across asynchronous flushes, including a final
        // pipeline line without a newline. Scope timing must not change severity.
        writer.Write("  [Error] [Rec");
        writer.Flush();
        writer.WriteLine("ord] Skipping record 000800:Skyrim.esm: InvalidDataException: Bad row");
        writer.Flush();
        writer.Write(fatalLine);
        writer.Dispose();
        Assert.Equal(2, writer.ErrorCount);
        Assert.Equal(1, writer.PipelineErrorCount);
        Assert.Contains("Skipping record", uiLog.ToString());
        Assert.Contains(fatalLine, uiLog.ToString());
    }
}
