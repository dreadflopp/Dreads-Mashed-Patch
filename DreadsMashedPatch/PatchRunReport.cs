using Mutagen.Bethesda.Plugins;

namespace DreadsMashedPatch;

public sealed record PatchRunError(FormKey? Record, string Stage, string Message, Exception? Exception);

/// <summary>Structured failures survive log flushing and prevent publishing an incomplete run.</summary>
public sealed class PatchRunReport
{
    private readonly List<PatchRunError> _errors = [];
    public IReadOnlyList<PatchRunError> Errors => _errors.AsReadOnly();
    public bool Succeeded => _errors.Count == 0;
    internal void Add(PatchRunError error) => _errors.Add(error);

    public void ThrowIfFailed()
    {
        if (!Succeeded) throw new PatchRunFailedException(this);
    }
}

public sealed class PatchRunFailedException(PatchRunReport report)
    : Exception($"Patching failed with {report.Errors.Count} error(s). Output must not be published.")
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

    public void Dispose() => Current.Value = _parent;
}
