using Mutagen.Bethesda.Plugins.Analysis;

namespace DreadsMashedPatch;

/// <summary>Stages patch files inside the output folder so a failed build leaves the previous patch intact.</summary>
public sealed class PatchOutputTransaction : IDisposable
{
    private readonly string _outputPath;
    private readonly string _stageDirectory;
    private bool _preserveStage;
    private bool _committed;

    public PatchOutputTransaction(string outputPath)
    {
        _outputPath = Path.GetFullPath(outputPath);
        var outputDirectory = Path.GetDirectoryName(_outputPath)
            ?? throw new ArgumentException("The output path must have a directory.", nameof(outputPath));
        Directory.CreateDirectory(outputDirectory);
        _stageDirectory = Path.Combine(outputDirectory, $".MashedPatch-stage-{Guid.NewGuid():N}");
        Directory.CreateDirectory(_stageDirectory);
        StagedOutputPath = Path.Combine(_stageDirectory, Path.GetFileName(_outputPath));
    }

    public string StagedOutputPath { get; }

    /// <summary>Publishes successful records unless the report contains fatal errors.</summary>
    public int Commit(PatchRunReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        report.ThrowIfFailed();
        return Commit();
    }

    public int Commit()
    {
        if (_committed)
        {
            throw new InvalidOperationException("The patch output has already been committed.");
        }

        var stagedFiles = FindPatchFiles(_stageDirectory, Path.GetFileName(_outputPath));
        if (!stagedFiles.Any(path =>
                string.Equals(Path.GetFileName(path), Path.GetFileName(_outputPath), StringComparison.OrdinalIgnoreCase)))
        {
            throw new InvalidDataException("The patch writer did not produce the primary output plugin.");
        }

        var outputDirectory = Path.GetDirectoryName(_outputPath)!;
        var previousFiles = FindPatchFiles(outputDirectory, Path.GetFileName(_outputPath));
        var backupDirectory = Path.Combine(_stageDirectory, "previous");
        Directory.CreateDirectory(backupDirectory);
        var backedUp = new List<(string Original, string Backup)>();
        var installed = new List<string>();
        try
        {
            foreach (var previous in previousFiles)
            {
                var backup = Path.Combine(backupDirectory, Path.GetFileName(previous));
                File.Move(previous, backup);
                backedUp.Add((previous, backup));
            }

            foreach (var staged in stagedFiles)
            {
                var destination = Path.Combine(outputDirectory, Path.GetFileName(staged));
                File.Move(staged, destination);
                installed.Add(destination);
            }

            _committed = true;
            return previousFiles.Count;
        }
        catch (Exception publishError)
        {
            try
            {
                foreach (var destination in installed)
                {
                    File.Delete(destination);
                }

                foreach (var (original, backup) in backedUp)
                {
                    File.Move(backup, original);
                }
            }
            catch (Exception rollbackError)
            {
                _preserveStage = true;
                throw new AggregateException(
                    $"Patch replacement failed and recovery files remain at {_stageDirectory}.",
                    publishError,
                    rollbackError);
            }

            throw;
        }
    }

    public void Dispose()
    {
        if (!_preserveStage && Directory.Exists(_stageDirectory))
        {
            var stageDirectory = Path.GetFullPath(_stageDirectory);
            var outputDirectory = Path.GetDirectoryName(_outputPath)!;
            if (!string.Equals(Path.GetDirectoryName(stageDirectory), outputDirectory,
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("The patch staging folder is outside the output directory.");
            }

            Directory.Delete(stageDirectory, recursive: true);
        }
    }

    private static List<string> FindPatchFiles(string directory, string outputName)
    {
        var baseName = Path.GetFileNameWithoutExtension(outputName);
        var extension = Path.GetExtension(outputName);
        return Directory.EnumerateFiles(directory)
            .Where(path =>
            {
                var candidate = Path.GetFileName(path);
                return string.Equals(candidate, outputName, StringComparison.OrdinalIgnoreCase)
                    || (string.Equals(Path.GetExtension(candidate), extension, StringComparison.OrdinalIgnoreCase)
                        && MultiModFileAnalysis.IsSplitFileName(
                            Path.GetFileNameWithoutExtension(candidate), baseName, out var index)
                        && index >= 2);
            })
            .ToList();
    }
}
