using Xunit;

namespace DreadsMashedPatch.Tests;

[CollectionDefinition("LogCollector", DisableParallelization = true)]
public sealed class LogCollectorTestCollection;

[Collection("LogCollector")]
public sealed class LoggingWarningTests
{
    [Theory]
    [InlineData("[String] Mod.esp: Addition: [] -> <Error: Unknown lstring ID 00000011> Success")]
    [InlineData("[Name] Computed value: [Error] is part of the name")]
    [InlineData("[Name] Original value: Warning: read the sign")]
    public void DiagnosticWordsInPropertyValuesDoNotFailRunsOrReachUi(string message)
    {
        LogCollector.Clear();
        LogCollector.SetRecordLoggingContext(deepDiveRecord: false, detailedRecord: true);
        using var diagnostics = new PatchDiagnostics();
        LogCollector.Add("String", message);
        Assert.True(diagnostics.Report.Succeeded);
        Assert.Single(LogCollector.GetAll());
        var fullLog = new System.Text.StringBuilder();
        var uiLog = new System.Text.StringBuilder();
        using (var writer = new DreadsMashedPatch.App.Services.UiTextWriter(
            text => fullLog.Append(text), text => uiLog.Append(text)))
        {
            writer.WriteLine(message);
            writer.Flush();
            Assert.Equal(0, writer.ErrorCount);
            Assert.Equal(0, writer.WarningCount);
        }
        Assert.Contains(message, fullLog.ToString());
        Assert.Empty(uiLog.ToString());
        LogCollector.Clear();
    }

    [Fact]
    public void WarningBypassesSummaryVerbosityFiltering()
    {
        LogCollector.Clear();
        LogCollector.SetRecordLoggingContext(deepDiveRecord: false, detailedRecord: false);

        LogCollector.Add("Placement", "[Warning] formatter needs attention");

        Assert.Equal(1, LogCollector.GetTotalCount());
        Assert.Contains(LogCollector.GetAll(), line => line.Contains("[Warning]", StringComparison.Ordinal));
        LogCollector.Clear();
    }

    [Fact]
    public void OrdinaryDetailRemainsSuppressedInSummaryMode()
    {
        LogCollector.Clear();
        LogCollector.SetRecordLoggingContext(deepDiveRecord: false, detailedRecord: false);

        LogCollector.Add("Placement", "ordinary detailed trace");

        Assert.Equal(0, LogCollector.GetTotalCount());
        LogCollector.Clear();
    }

    [Theory]
    [InlineData("[Error] failed to copy value")]
    [InlineData("Error: failed to copy value")]
    [InlineData("Error getting property via reflection")]
    [InlineData("[Property] Error copying value")]
    [InlineData("  [Property] [Error] failed to copy value")]
    public void ErrorsBypassSummaryVerbosityFiltering(string message)
    {
        LogCollector.Clear();
        LogCollector.SetRecordLoggingContext(deepDiveRecord: false, detailedRecord: false);
        using var diagnostics = new PatchDiagnostics();

        LogCollector.Add("Placement", message);

        Assert.False(diagnostics.Report.Succeeded);
        Assert.Equal(1, LogCollector.GetTotalCount());
        Assert.Contains(LogCollector.GetAll(), line => line.Contains("Error", StringComparison.OrdinalIgnoreCase));
        using (var writer = new DreadsMashedPatch.App.Services.UiTextWriter(_ => { }, _ => { }))
        {
            writer.WriteLine(message);
            writer.Flush();
            Assert.Equal(1, writer.ErrorCount);
        }
        LogCollector.Clear();
    }

    [Fact]
    public void ExplicitDiagnosticsIncludeSeverityAndExceptionDetails()
    {
        LogCollector.Clear();
        LogCollector.SetRecordLoggingContext(deepDiveRecord: false, detailedRecord: false);

        LogCollector.AddWarning("Placement", "fallback used", new InvalidOperationException("bad value"));
        LogCollector.AddError("Placement", "copy failed", new ArgumentException("invalid data"));

        var lines = LogCollector.GetAll().ToArray();
        Assert.Contains(lines, line => line.Contains("[Warning] fallback used (InvalidOperationException: bad value)", StringComparison.Ordinal));
        Assert.Contains(lines, line => line.Contains("[Error] copy failed (ArgumentException: invalid data)", StringComparison.Ordinal));
        LogCollector.Clear();
    }

    [Fact]
    public void PrintAllAndClearEmitsPendingDiagnosticsExactlyOnce()
    {
        LogCollector.Clear();
        LogCollector.SetRecordLoggingContext(deepDiveRecord: false, detailedRecord: false);
        LogCollector.AddWarning("Placement", "fallback used");
        var originalOut = Console.Out;
        using var output = new StringWriter();

        try
        {
            Console.SetOut(output);
            LogCollector.PrintAllAndClear();
            LogCollector.PrintAllAndClear();
        }
        finally
        {
            Console.SetOut(originalOut);
            LogCollector.Clear();
        }

        Assert.Equal($"  [Warning] fallback used{Environment.NewLine}", output.ToString());
        Assert.False(LogCollector.HasLogs());
    }

    [Fact]
    public void SnapshotEnumerationIsStableWhenNewLogsArrive()
    {
        LogCollector.Clear();
        LogCollector.AddWarning("First", "original warning");
        using var snapshot = LogCollector.GetAll().GetEnumerator();

        Assert.True(snapshot.MoveNext());
        LogCollector.AddWarning("Second", "concurrent warning");

        Assert.False(snapshot.MoveNext());
        Assert.Equal(2, LogCollector.GetTotalCount());
        LogCollector.Clear();
    }

    [Fact]
    public void RecoveredExceptionDiagnosticBypassesVerbosityWithoutBecomingAWarning()
    {
        LogCollector.Clear();
        LogCollector.SetRecordLoggingContext(deepDiveRecord: false, detailedRecord: false);

        LogCollector.AddDiagnostic("Placement", "used fallback", new InvalidOperationException("probe failed"));

        var line = Assert.Single(LogCollector.GetAll());
        Assert.Contains("[Diagnostic] used fallback (InvalidOperationException: probe failed)", line, StringComparison.Ordinal);
        Assert.DoesNotContain("[Warning]", line, StringComparison.Ordinal);
        Assert.DoesNotContain("[Error]", line, StringComparison.Ordinal);
        LogCollector.Clear();
    }
}
