using System.Text.Json;
using System.Text.Json.Serialization;
using DreadsMashedPatch.App.Models;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class StandaloneThemeTests
{
    [Fact]
    public void ExistingSettingsWithoutThemeUseLightAndRetainTheirOtherChoices()
    {
        var settings = JsonSerializer.Deserialize<StandaloneSettings>(
            """{"SettingsVersion":5,"HistoricalLogsToKeep":23,"OutputFolderPath":"patch-output"}""")!;

        settings.Normalize();

        Assert.Equal(AppTheme.Light, settings.Theme);
        Assert.Equal(23, settings.HistoricalLogsToKeep);
        Assert.Equal("patch-output", settings.OutputFolderPath);
    }

    [Theory]
    [InlineData(AppTheme.Light)]
    [InlineData(AppTheme.Dark)]
    public void ThemeSurvivesSettingsJsonRoundTrip(AppTheme theme)
    {
        var options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
        var settings = new StandaloneSettings { Theme = theme, HistoricalLogsToKeep = 23 };

        var json = JsonSerializer.Serialize(settings, options);
        var restored = JsonSerializer.Deserialize<StandaloneSettings>(json, options)!;
        restored.Normalize();

        Assert.Equal(theme, restored.Theme);
        Assert.Equal(23, restored.HistoricalLogsToKeep);
    }

    [Fact]
    public void UnknownNumericThemeFallsBackToLight()
    {
        var settings = JsonSerializer.Deserialize<StandaloneSettings>("""{"Theme":99}""")!;

        settings.Normalize();

        Assert.Equal(AppTheme.Light, settings.Theme);
    }
}
