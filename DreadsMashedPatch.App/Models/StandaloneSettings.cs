using Mutagen.Bethesda;
using DreadsMashedPatch.App.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DreadsMashedPatch.App.Models;

public sealed class StandaloneSettings : INotifyPropertyChanged
{
    private GameRelease _gameRelease = GameRelease.SkyrimSE;
    private string _gameFolderPath = string.Empty;
    private string _dataFolderPath = string.Empty;
    private string _loadOrderFilePath = string.Empty;
    private string _outputFolderPath = string.Empty;
    private int _historicalLogsToKeep = 10;

    public event PropertyChangedEventHandler? PropertyChanged;

    public int SettingsVersion { get; set; } = 5;

    public GameRelease GameRelease
    {
        get => _gameRelease;
        set => SetProperty(ref _gameRelease, value);
    }

    public string GameFolderPath
    {
        get => _gameFolderPath;
        set => SetProperty(ref _gameFolderPath, value ?? string.Empty);
    }

    public string DataFolderPath
    {
        get => _dataFolderPath;
        set => SetProperty(ref _dataFolderPath, value ?? string.Empty);
    }

    public string LoadOrderFilePath
    {
        get => _loadOrderFilePath;
        set => SetProperty(ref _loadOrderFilePath, value ?? string.Empty);
    }

    public int HistoricalLogsToKeep
    {
        get => _historicalLogsToKeep;
        set => SetProperty(ref _historicalLogsToKeep, value);
    }

    public string OutputFolderPath
    {
        get => _outputFolderPath;
        set => SetProperty(ref _outputFolderPath, value ?? string.Empty);
    }

    public PatcherConfiguration Patcher { get; set; } = new();

    public void Normalize()
    {
        SettingsVersion = 5;
        if (GameRelease is not (GameRelease.SkyrimSE or GameRelease.SkyrimSEGog or GameRelease.SkyrimVR))
        {
            GameRelease = GameRelease.SkyrimSE;
        }

        GameFolderPath = PathInput.Normalize(GameFolderPath);
        DataFolderPath = PathInput.Normalize(DataFolderPath);
        LoadOrderFilePath = PathInput.Normalize(LoadOrderFilePath);
        // Require an explicit destination, including for older saved settings.
        // The input directory may be a manager-owned staging directory.
        OutputFolderPath = PathInput.Normalize(OutputFolderPath);
        if (string.IsNullOrWhiteSpace(GameFolderPath) && !string.IsNullOrWhiteSpace(DataFolderPath))
        {
            GameFolderPath = PathInput.GetParentOrEmpty(DataFolderPath);
        }
        HistoricalLogsToKeep = Math.Clamp(HistoricalLogsToKeep, 0, 100);
        Patcher ??= new PatcherConfiguration();
        Patcher.Normalize();
    }

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
