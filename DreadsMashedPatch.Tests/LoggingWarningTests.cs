using Xunit;

namespace DreadsMashedPatch.Tests;

[CollectionDefinition("LogCollector", DisableParallelization = true)]
public sealed class LogCollectorTestCollection;

[Collection("LogCollector")]
public sealed class LoggingWarningTests
{
    [Theory]
    [InlineData("Checking output patch for errors...")]
    [InlineData("Output validation completed: 42 override(s) retained; 0 omitted.")]
    [InlineData("Partial patch: skipped 1 record(s). Successful records will be published if output writing succeeds.")]
    [InlineData("Skipped 1 record(s). Fatal errors prevent publication.")]
    [InlineData("Skipped 2 malformed Weapon context(s) during load.")]
    public void ProgressAndSummariesReachRunLogWithoutIncreasingWarningCounts(string line)
    {
        var fullLog = new System.Text.StringBuilder();
        var runLog = new System.Text.StringBuilder();
        using var writer = new DreadsMashedPatch.App.Services.UiTextWriter(
            text => fullLog.Append(text), text => runLog.Append(text));
        writer.WriteLine(line);
        writer.Flush();
        Assert.Contains(line, fullLog.ToString());
        Assert.Contains(line, runLog.ToString());
        Assert.Equal(0, writer.WarningCount);
        Assert.Equal(0, writer.ErrorCount);
    }

    [Fact]
    public void ScopedWarningsIdentifyTheRecordAndRemainWarningsInTheUi()
    {
        LogCollector.Clear();
        var key = new Mutagen.Bethesda.Plugins.FormKey(Mutagen.Bethesda.Plugins.ModKey.FromNameAndExtension("Source.esp"), 0x800);
        using var diagnostics = new PatchDiagnostics(key);
        LogCollector.AddWarning("Property", "An actionable record warning");
        var line = Assert.Single(LogCollector.GetAll());
        Assert.Contains(key.ToString(), line);
        using var writer = new DreadsMashedPatch.App.Services.UiTextWriter(_ => { }, _ => { });
        writer.WriteLine(line);
        writer.Flush();
        Assert.Equal(1, writer.WarningCount);
        Assert.Equal(0, writer.ErrorCount);
        Assert.True(diagnostics.Report.Succeeded);
        LogCollector.Clear();
    }

    [Theory]
    [InlineData("[Warning] fallback used")]
    [InlineData("  [Warning] fallback used")]
    [InlineData("Warning: No handler implemented for TEST")]
    [InlineData("WARNING: fallback used")]
    [InlineData("Warning fallback used")]
    [InlineData("Warning")]
    [InlineData("[Property] [Warning] fallback used")]
    [InlineData("  [Property] Warning: fallback used")]
    [InlineData("[Warning] Error: recovered by fallback")]
    [InlineData("[Warning] [Error] recovered by fallback")]
    public void WarningFormatsBypassVerbosityFilteringAndCountOnce(string message)
    {
        LogCollector.Clear();
        LogCollector.SetRecordLoggingContext(deepDiveRecord: false, detailedRecord: false);
        using var diagnostics = new PatchDiagnostics();
        LogCollector.Add("Property", message);
        var line = Assert.Single(LogCollector.GetAll());
        using var writer = new DreadsMashedPatch.App.Services.UiTextWriter(_ => { }, _ => { });
        writer.WriteLine(line);
        writer.Flush();
        writer.Flush();
        Assert.Equal(1, writer.WarningCount);
        Assert.Equal(0, writer.ErrorCount);
        Assert.True(diagnostics.Report.Succeeded);
        LogCollector.Clear();
    }

    [Fact]
    public void BufferedWarningsAreCountedOncePerEntryAcrossFlushAndDispose()
    {
        const string message = "  [Property] [Warning] fallback used";
        var runLog = new System.Text.StringBuilder();
        var writer = new DreadsMashedPatch.App.Services.UiTextWriter(_ => { }, text => runLog.Append(text));
        using (writer)
        {
            writer.Write("  [Property] [War");
            writer.Flush();
            Assert.Equal(0, writer.WarningCount);
            writer.Write("ning] fallback used\r");
            writer.Flush();
            Assert.Equal(0, writer.WarningCount);
            writer.Write('\n');
            writer.Flush();
            Assert.Equal(1, writer.WarningCount);
            // Identical messages can describe separate occurrences and must both count.
            writer.Write(message);
            writer.Flush();
            Assert.Equal(1, writer.WarningCount);
        }
        Assert.Equal(2, writer.WarningCount);
        Assert.Equal(0, writer.ErrorCount);
        Assert.Equal($"{message}{Environment.NewLine}{message}{Environment.NewLine}", runLog.ToString());
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    public void MalformedContextsCountEachWarningWithoutCountingTheLoadSummary(int count)
    {
        PatcherSettings.Apply(new PatcherConfiguration());
        var contexts = Enumerable.Range(0, count)
            .Select<int, Mutagen.Bethesda.Plugins.Cache.IModContext<
                Mutagen.Bethesda.Skyrim.ISkyrimMod, Mutagen.Bethesda.Skyrim.ISkyrimModGetter,
                Mutagen.Bethesda.Skyrim.IWeapon, Mutagen.Bethesda.Skyrim.IWeaponGetter>>(
                _ => throw new InvalidDataException("Malformed context"));
        var loader = typeof(Program).GetMethod("LoadContextsSafely",
            System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic)!
            .MakeGenericMethod(typeof(Mutagen.Bethesda.Skyrim.IWeapon), typeof(Mutagen.Bethesda.Skyrim.IWeaponGetter));
        var runLog = new System.Text.StringBuilder();
        var writer = new DreadsMashedPatch.App.Services.UiTextWriter(_ => { }, text => runLog.Append(text));
        var previous = Console.Out;
        try
        {
            Console.SetOut(writer);
            var loaded = Assert.IsAssignableFrom<Array>(loader.Invoke(null, [contexts, "Weapon"]));
            Assert.Empty(loaded);
        }
        finally
        {
            Console.SetOut(previous);
            writer.Dispose();
            PatcherSettings.Apply(new PatcherConfiguration());
        }
        Assert.Contains($"Skipped {count} malformed Weapon context(s) during load.", runLog.ToString());
        Assert.Equal(count, writer.WarningCount);
        Assert.Equal(0, writer.ErrorCount);
    }

    [Theory]
    [InlineData("[String] Mod.esp: Addition: [] -> <Error: Unknown lstring ID 00000011> Success")]
    [InlineData("[Name] Computed value: [Error] is part of the name")]
    [InlineData("[Name] Original value: Warning: read the sign")]
    [InlineData("[Name] Computed value: [Warning] is part of the name")]
    [InlineData("[Info] Expected exclusion; Warning: appears in the description")]
    [InlineData("[Info] Warning: expected exclusion")]
    [InlineData("[Diagnostic] [Error] recovered by fallback")]
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
    [InlineData("[Error] Warning: copy failed")]
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
            Assert.Equal(0, writer.WarningCount);
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
