using System.Runtime.InteropServices;

namespace DreadsMashedPatch.App.Services;

/// <summary>Handles pasted path text before it reaches filesystem APIs or shell dialogs.</summary>
public static class PathInput
{
    public static string Normalize(string? value)
    {
        var path = value?.Trim() ?? string.Empty;
        if (path.Length >= 2 && path[0] == '"' && path[^1] == '"')
            path = path[1..^1].Trim();

        if (OperatingSystem.IsWindows())
        {
            // Wine normally exposes Unix files through Z:. Only use that mapping
            // when the requested file/folder actually exists through it.
            if (path.StartsWith('/') && !path.StartsWith("//", StringComparison.Ordinal))
            {
                var winePath = "Z:" + path.Replace('/', '\\');
                if (Directory.Exists(winePath) || File.Exists(winePath)) return winePath;
            }
            return path.Replace('/', '\\');
        }

        // Pasted mixed separators are intended as separators in these UI fields.
        return path.Replace('\\', '/');
    }

    public static string GetExistingDirectory(string? value, bool isFilePath = false)
    {
        var path = Normalize(value);
        if (path.Length == 0 || path.Any(char.IsControl)) return string.Empty;
        try
        {
            var fullPath = Path.GetFullPath(path);
            var directory = isFilePath ? Path.GetDirectoryName(fullPath) : fullPath;
            return Directory.Exists(directory)
                ? Path.TrimEndingDirectorySeparator(directory!) : string.Empty;
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or IOException)
        {
            return string.Empty;
        }
    }

    public static string GetParentOrEmpty(string? value)
    {
        var path = Normalize(value);
        if (path.Length == 0 || path.Any(char.IsControl)) return string.Empty;
        try
        {
            return Directory.GetParent(path)?.FullName ?? string.Empty;
        }
        catch (Exception ex) when (ex is ArgumentException or NotSupportedException or IOException)
        {
            return string.Empty;
        }
    }

    public static T Browse<T>(string? value, bool isFilePath, Func<string, T> openDialog)
    {
        var initialDirectory = GetExistingDirectory(value, isFilePath);
        try
        {
            return openDialog(initialDirectory);
        }
        catch (Exception ex) when (initialDirectory.Length > 0
            && ex is ArgumentException or ExternalException or IOException or NotSupportedException)
        {
            // Filesystem visibility does not guarantee shell-dialog compatibility
            // under Wine. Retry once with a fresh dialog and no initial directory.
            return openDialog(string.Empty);
        }
    }
}
