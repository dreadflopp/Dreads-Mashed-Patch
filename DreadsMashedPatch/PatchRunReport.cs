using Mutagen.Bethesda.Plugins;

namespace DreadsMashedPatch;

public sealed record PatchRunError(FormKey? Record, string Stage, string Message, Exception? Exception)
{
    // Set only by the record boundary after its failed candidate has been discarded.
    public bool IsSkippedRecordError { get; internal set; }
}

public sealed record SkippedPatchRecord(FormKey Record, string RecordType, string Reason);

/// <summary>Retains record skips and fatal errors independently of log flushing.</summary>
public sealed class PatchRunReport
{
    private readonly List<PatchRunError> _errors = [];
    private readonly List<SkippedPatchRecord> _skippedRecords = [];
    public IReadOnlyList<PatchRunError> Errors => _errors.AsReadOnly();
    public IReadOnlyList<SkippedPatchRecord> SkippedRecords => _skippedRecords.AsReadOnly();
    public int FatalErrorCount => _errors.Count(error => !error.IsSkippedRecordError);
    public bool Succeeded => FatalErrorCount == 0;
    public bool IsPartial => _skippedRecords.Count > 0;
    internal void Add(PatchRunError error) => _errors.Add(error);
    internal void Add(SkippedPatchRecord record) => _skippedRecords.Add(record);

    public void ThrowIfFailed()
    {
        if (!Succeeded) throw new PatchRunFailedException(this);
    }
}

public sealed class PatchRunFailedException(PatchRunReport report)
    : Exception($"Patching scope failed with {report.FatalErrorCount} unrecovered error(s).")
{
    public PatchRunReport Report { get; } = report;
}

internal sealed class PatchDiagnostics : IDisposable
{
    private static readonly AsyncLocal<PatchDiagnostics?> Current = new();
    private readonly PatchDiagnostics? _parent;
    private readonly FormKey? _record;
    public PatchRunReport Report { get; } = new();

    public PatchDiagnostics(FormKey? record = null)
    {
        _parent = Current.Value;
        _record = record;
        Current.Value = this;
    }

    public static void Error(string stage, string message, Exception? exception = null)
    {
        var scope = Current.Value;
        var error = new PatchRunError(scope?._record, stage, message, exception);
        for (; scope != null; scope = scope._parent) scope.Report.Add(error);
    }

    public static void ThrowIfFailed() => Current.Value?.Report.ThrowIfFailed();

    public static bool IsRecordScope => Current.Value?._record != null;

    // A FormKey alone does not make an error recoverable. Only the record loop
    // can accept a skip, after transactional application has returned or unwound.
    public void SkipRecord(string recordType, Exception exception)
    {
        if (_record == null) throw new InvalidOperationException("Only a record scope can recover a skipped record.");
        var failure = Report.Errors.FirstOrDefault();
        Error("Record", "Record was skipped", exception);
        foreach (var error in Report.Errors) error.IsSkippedRecordError = true;
        var cause = exception is PatchRunFailedException && failure?.Exception != null
            ? failure.Exception.GetBaseException() : exception.GetBaseException();
        var reason = exception is PatchRunFailedException && failure?.Exception == null && failure != null
            ? $"{failure.Stage}: {failure.Message}" : $"{cause.GetType().Name}: {cause.Message}";
        var skipped = new SkippedPatchRecord(_record.Value, recordType, reason);
        for (var scope = this; scope != null; scope = scope._parent) scope.Report.Add(skipped);
    }

    public void Dispose() => Current.Value = _parent;
}
