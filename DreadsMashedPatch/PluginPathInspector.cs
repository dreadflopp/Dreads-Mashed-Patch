using System.Text;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;

namespace DreadsMashedPatch;

/// <summary>Checks plugin filenames and listings without importing records or changing any files.</summary>
public static class PluginPathInspector
{
    public static IReadOnlyList<ModKey> ReadCreationClubPlugins(string gameFolder)
    {
        var path = Path.Combine(gameFolder, "Skyrim.ccc");
        return File.Exists(path)
            ? File.ReadLines(path).Select(line => line.Trim()).Where(line => line.Length > 0)
                .Select(line => ModKey.FromNameAndExtension(line.AsSpan())).Distinct().ToArray()
            : [];
    }

    public static PluginPathInspection Inspect(
        GameRelease release, string gameFolder, string dataFolder, string pluginsFile,
        string outputFolder, string outputPluginName)
    {
        var report = new StringBuilder();
        var warnings = new List<string>();
        var errors = new List<string>();
        report.AppendLine($"Game folder: {gameFolder}");
        report.AppendLine($"Data folder: {dataFolder}");
        report.AppendLine($"Load order: {pluginsFile}");
        report.AppendLine($"Patch output folder: {outputFolder}");
        report.AppendLine();

        if (!Directory.Exists(gameFolder))
            errors.Add("Select an existing game folder (the list's Stock Game folder for Wabbajack).");
        else if (!File.Exists(Path.Combine(gameFolder, release == GameRelease.SkyrimVR ? "SkyrimVR.exe" : "SkyrimSE.exe")))
            errors.Add("The selected game folder does not contain the executable for the selected game release.");
        if (!Directory.Exists(dataFolder))
            errors.Add("Select an existing Data folder.");
        if (!File.Exists(pluginsFile))
            errors.Add("Select the active profile's plugins.txt file.");
        if (!Directory.Exists(outputFolder))
            errors.Add("Select an existing patch output folder. The staging root is not an output mod.");
        if (Directory.Exists(gameFolder) && Directory.Exists(dataFolder)
            && !string.Equals(Path.TrimEndingDirectorySeparator(Path.GetFullPath(dataFolder)),
                Path.Combine(Path.GetFullPath(gameFolder), "Data"),
                OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal))
            warnings.Add("The selected Data folder is outside the selected game's Data folder. Confirm both paths belong to the game installation your list uses.");

        var diskFiles = Directory.Exists(dataFolder)
            ? Directory.EnumerateFiles(dataFolder)
                .Where(path => ModKey.TryFromFileName(Path.GetFileName(path), out _))
                .OrderBy(Path.GetFileName, StringComparer.OrdinalIgnoreCase).ToArray()
            : [];
        var installed = diskFiles.GroupBy(path => ModKey.FromNameAndExtension(Path.GetFileName(path).AsSpan()))
            .ToDictionary(group => group.Key, group => group.ToArray());
        var implicitKeys = Implicits.Get(release).Listings.ToHashSet();
        var ccc = Directory.Exists(gameFolder) ? ReadCreationClubPlugins(gameFolder) : [];
        // Match the preparation used by the runner: only exact, accessible CCC paths are added.
        var installedCcc = ccc.Where(key => File.Exists(Path.Combine(dataFolder, key.FileName.String))).ToHashSet();
        var listings = File.Exists(pluginsFile)
            ? PluginListings.RawLoadOrderListingsFromPath(pluginsFile, release).ToArray()
            : [];
        var outputKey = ModKey.FromNameAndExtension(outputPluginName.AsSpan());
        var cutoff = Array.FindIndex(listings, listing => listing.ModKey == outputKey);
        var listedKeys = listings.Select(listing => listing.ModKey).ToHashSet();

        var skyrimKey = ModKey.FromNameAndExtension("Skyrim.esm".AsSpan());
        if (!installed.ContainsKey(skyrimKey))
            errors.Add("Skyrim.esm was not found in the selected Data folder. Select the deployed or virtual Data folder.");

        foreach (var (key, paths) in installed.Where(pair => pair.Value.Length > 1))
            errors.Add($"Multiple files represent {key.FileName.String}: {string.Join(", ", paths.Select(Path.GetFileName))}. Resolve this ambiguity.");
        foreach (var group in listings.GroupBy(listing => listing.ModKey).Where(group => group.Count() > 1))
            warnings.Add($"Duplicate plugins.txt entries: {group.Key.FileName.String}. Synthesis uses the first entry.");

        foreach (var key in implicitKeys.Where(installed.ContainsKey))
            CheckAccessible(key.FileName.String, "Implicit plugin");
        foreach (var key in ccc.Where(installed.ContainsKey).Except(installedCcc))
            CheckAccessible(key.FileName.String, "Creation Club plugin");

        foreach (var file in diskFiles)
        {
            var key = ModKey.FromNameAndExtension(Path.GetFileName(file).AsSpan());
            if (!listedKeys.Contains(key) && !implicitKeys.Contains(key) && !installedCcc.Contains(key)
                && key != outputKey && !Mutagen.Bethesda.Plugins.Analysis.MultiModFileAnalysis.IsSplitModSibling(key, outputKey))
                warnings.Add($"Present in the input folder but absent from plugins.txt: {Path.GetFileName(file)}. This may be an intentionally inactive plugin.");
        }

        var disabledPlugins = new List<string>();
        var laterPlugins = new List<string>();
        var outputPlugins = installed.Keys.Concat(listedKeys).Distinct()
            .Where(key => key == outputKey || Mutagen.Bethesda.Plugins.Analysis.MultiModFileAnalysis.IsSplitModSibling(key, outputKey))
            .OrderBy(key => key.FileName.String, StringComparer.OrdinalIgnoreCase).ToArray();
        var seenListings = new HashSet<ModKey>();
        for (var i = 0; i < listings.Length; i++)
        {
            var listing = listings[i];
            var duplicate = !seenListings.Add(listing.ModKey);
            var afterCutoff = cutoff >= 0 && i >= cutoff;
            var implicitPlugin = implicitKeys.Contains(listing.ModKey) && installed.ContainsKey(listing.ModKey);
            var automaticallyIncluded = implicitPlugin || installedCcc.Contains(listing.ModKey);
            var enabled = listing.Enabled || automaticallyIncluded;
            var excluded = afterCutoff && !automaticallyIncluded;
            var found = installed.ContainsKey(listing.ModKey);
            var outputPlugin = outputPlugins.Contains(listing.ModKey);
            var entry = $"{listing.FileName} [{(enabled ? "enabled" : "disabled")}; {(found ? "found" : "missing from input folder")}]";
            if (!duplicate && !outputPlugin)
            {
                if (!enabled) disabledPlugins.Add(entry);
                if (excluded) laterPlugins.Add(entry);
            }
            if (enabled && !excluded && !duplicate && !found)
                errors.Add($"Enabled plugin missing from input folder: {listing.ModKey.FileName.String}. Check the Stock Game path, active profile, deployment, or launch through MO2.");
            // Implicit and CCC entries precede plugins.txt and supply their own filenames.
            // Their accessibility was checked above; a differently cased raw listing is ignored.
            else if (enabled && !excluded && !duplicate && !automaticallyIncluded)
                CheckAccessible(listing.FileName, "Enabled plugin");
        }

        report.AppendLine();
        report.AppendLine($"Discovered plugins: {diskFiles.Length}");
        report.AppendLine($"Installed implicit plugins: {implicitKeys.Count(installed.ContainsKey)}");
        report.AppendLine($"Installed Creation Club plugins: {installedCcc.Count}");
        report.AppendLine(cutoff >= 0
            ? $"The patch reads enabled plugins before {outputPluginName}; the output and later entries are excluded."
            : $"{outputPluginName} is absent from plugins.txt; the patch reads the complete enabled load order.");
        if (!File.Exists(Path.Combine(gameFolder, "Skyrim.ccc")))
            report.AppendLine("Skyrim.ccc was not found; only implicit plugins and plugins.txt will be used.");
        report.AppendLine();
        report.AppendLine($"Patch output plugins ({outputPlugins.Length}; not counted as disabled):");
        foreach (var key in outputPlugins) report.AppendLine($"  {key.FileName.String}");
        report.AppendLine($"Disabled plugins ({disabledPlugins.Count}):");
        foreach (var entry in disabledPlugins) report.AppendLine($"  {entry}");
        report.AppendLine($"Plugins after the patch ({laterPlugins.Count}; excluded from patch input):");
        foreach (var entry in laterPlugins) report.AppendLine($"  {entry}");
        report.AppendLine();
        report.AppendLine("This checks filenames and listings. It does not validate plugin contents, masters, or output write permissions.");
        report.AppendLine("The stock-game path cannot be confirmed automatically: it must match the game launched by your mod manager.");

        var distinctErrors = errors.Distinct().ToArray();
        var distinctWarnings = warnings.Distinct().ToArray();
        var summary = $"{diskFiles.Length} discovered plugins; {disabledPlugins.Count} disabled; "
            + $"{laterPlugins.Count} after the patch; {distinctErrors.Length} errors; {distinctWarnings.Length} warnings.";
        var issues = new StringBuilder(summary).AppendLine().AppendLine();
        foreach (var error in distinctErrors) issues.AppendLine($"ERROR: {error}");
        foreach (var warning in distinctWarnings) issues.AppendLine($"WARNING: {warning}");
        issues.AppendLine().Append(report);
        return new PluginPathInspection(summary, issues.ToString(), distinctErrors.Length, distinctWarnings.Length);

        void CheckAccessible(string fileName, string source)
        {
            if (File.Exists(Path.Combine(dataFolder, fileName))) return;
            var message = $"{source} was not found at the expected filename {fileName}. "
                + "A matching plugin has different filename casing or a ghost suffix. Correct the filename/listing or check Wine/MO2 visibility.";
            errors.Add(message);
        }
    }
}

public sealed record PluginPathInspection(string Summary, string Report, int ErrorCount, int WarningCount);
