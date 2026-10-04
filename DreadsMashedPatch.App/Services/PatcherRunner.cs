using DreadsMashedPatch.App.Models;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;

namespace DreadsMashedPatch.App.Services;

public sealed class PatcherRunner
{
    internal const string OutputPluginName = "MashedPatch.esp";

    public static string GetOutputPath(StandaloneSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.OutputFolderPath))
        {
            throw new ArgumentException("Select a patch output folder.", nameof(settings));
        }

        return Path.Combine(settings.OutputFolderPath, OutputPluginName);
    }

    public EmptyOutputResult CreateEmptyOutput(StandaloneSettings settings)
    {
        ArgumentNullException.ThrowIfNull(settings);
        settings.Normalize();

        var outputPath = GetOutputPath(settings);
        using var stagedOutput = new PatchOutputTransaction(outputPath);
        var outputModKey = ModKey.FromNameAndExtension(OutputPluginName.AsSpan());
        var release = settings.GameRelease == GameRelease.SkyrimVR
            ? SkyrimRelease.SkyrimVR
            : SkyrimRelease.SkyrimSE;
        var emptyMod = new SkyrimMod(outputModKey, release);
        using (var stream = File.Create(stagedOutput.StagedOutputPath))
        {
            emptyMod.WriteToBinary(stream);
        }

        var removedOutputs = stagedOutput.Commit();
        return new EmptyOutputResult(outputPath, removedOutputs);
    }

    public async Task<PatcherRunResult> RunAsync(
        StandaloneSettings settings,
        Action<string> writeLog,
        Action<string> writeUiLog)
    {
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(writeLog);
        ArgumentNullException.ThrowIfNull(writeUiLog);

        settings.Normalize();

        var outputPath = GetOutputPath(settings);
        var outputModKey = ModKey.FromNameAndExtension(OutputPluginName.AsSpan());
        var preparedLoadOrder = await LoadOrderPreparer.CreateAsync(settings);
        try
        {
            PatcherSettings.Apply(settings.Patcher, preparedLoadOrder.CreationClubPlugins);

            writeLog($"Game release: {settings.GameRelease}{Environment.NewLine}");
            writeLog($"Game folder: {settings.GameFolderPath}{Environment.NewLine}");
            writeLog($"Data folder: {settings.DataFolderPath}{Environment.NewLine}");
            writeLog($"Load order: {settings.LoadOrderFilePath}{Environment.NewLine}");
            writeLog($"Patch output: {outputPath}{Environment.NewLine}");
            if (File.Exists(preparedLoadOrder.CreationClubPath))
            {
                writeLog($"Creation Club list: {preparedLoadOrder.CreationClubPath} "
                    + $"({preparedLoadOrder.CreationClubPluginCount} installed plugins){Environment.NewLine}");
            }
            else
            {
                writeLog($"Creation Club list not found at {preparedLoadOrder.CreationClubPath}; "
                    + $"continuing with plugins.txt entries.{Environment.NewLine}");
            }

            writeLog(preparedLoadOrder.OutputPluginFound
                ? $"{OutputPluginName} was found in the load order; "
                    + $"{preparedLoadOrder.ListingsAfterOutput} later listings will not be read.{Environment.NewLine}"
                : $"{OutputPluginName} was not found in the load order; the complete enabled load order will be read.{Environment.NewLine}");

            using var stagedOutput = new PatchOutputTransaction(outputPath);
            var arguments = new RunSynthesisMutagenPatcher
            {
                OutputPath = stagedOutput.StagedOutputPath,
                GameRelease = settings.GameRelease,
                DataFolderPath = settings.DataFolderPath,
                LoadOrderFilePath = preparedLoadOrder.Path,
                ExtraDataFolder = SettingsStore.SettingsDirectory,
                PersistencePath = Path.Combine(SettingsStore.SettingsDirectory, "Persistence"),
                PatcherName = "Mashed Patch",
                // Synthesis uses this primary ModKey as the load-order cutoff, removing
                // it and every later listing before importing any input plugins. Keep
                // this as the unsuffixed key even when automatic output splitting is on.
                ModKey = outputModKey.FileName.String,
                // The temporary load order already contains the Creation Club entries
                // from the explicitly selected game folder.
                LoadOrderIncludesCreationClub = true,
                SplitIfMaxMastersExceeded = true
            };

            var result = await Task.Run(async () =>
            {
                var originalOut = Console.Out;
                var originalError = Console.Error;
                var writer = new UiTextWriter(writeLog, writeUiLog);
                Console.SetOut(writer);
                Console.SetError(writer);

                try
                {
                    var pipeline = SynthesisPipeline.Instance
                        .AddPatch<ISkyrimMod, ISkyrimModGetter>(Program.RunPatch);
                    await pipeline.Run(arguments);
                }
                finally
                {
                    Console.SetOut(originalOut);
                    Console.SetError(originalError);
                    writer.Dispose();
                }

                return new PatcherRunResult(writer.WarningCount, writer.ErrorCount);
            });
            var replacedOutputs = stagedOutput.Commit();
            writeLog($"Replaced {replacedOutputs} previous patch output file(s).{Environment.NewLine}");
            return result;
        }
        finally
        {
            File.Delete(preparedLoadOrder.Path);
        }
    }
}

public sealed record PatcherRunResult(int WarningCount, int ErrorCount);

public sealed record EmptyOutputResult(string OutputPath, int RemovedOutputCount);
