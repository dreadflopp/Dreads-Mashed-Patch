using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace DreadsMashedPatch.Tests;

[CollectionDefinition("LoggingSettingsTests", DisableParallelization = true)]
public sealed class LoggingSettingsTestCollection;

[Collection("LoggingSettingsTests")]
public sealed class LoggingSettingsTests : IDisposable
{
    private static readonly ModKey TestModKey = ModKey.FromNameAndExtension("DeepDiveTest.esp");

    public LoggingSettingsTests()
    {
        LoggingSettings.Apply(new DiagnosticsSettings());
        LogCollector.Clear();
        LogCollector.SetRecordLoggingContext(deepDiveRecord: false, detailedRecord: false);
    }

    public void Dispose()
    {
        LoggingSettings.Apply(new DiagnosticsSettings());
        LogCollector.Clear();
        LogCollector.SetRecordLoggingContext(deepDiveRecord: false, detailedRecord: false);
    }

    [Fact]
    public void SupportedRecordSignatureEnablesDeepDive()
    {
        LoggingSettings.Apply(new DiagnosticsSettings
        {
            DebugMode = true,
            DeepDiveRecordSignatures = ["qust"]
        });
        var context = CreateContext(new Quest(new FormKey(TestModKey, 0x800), SkyrimRelease.SkyrimSE));

        Assert.True(LoggingSettings.IsDeepDiveRecord(context));
    }

    [Fact]
    public void ExactFormKeyEnablesDeepDive()
    {
        var formKey = new FormKey(TestModKey, 0x801);
        LoggingSettings.Apply(new DiagnosticsSettings
        {
            DebugMode = true,
            DeepDiveFormKeys = [formKey.ToString()]
        });
        var context = CreateContext(new Quest(formKey, SkyrimRelease.SkyrimSE));

        Assert.True(LoggingSettings.IsDeepDiveRecord(context));
    }

    [Fact]
    public void DebugModeRemainsRequiredForDeepDive()
    {
        LoggingSettings.Apply(new DiagnosticsSettings
        {
            DebugMode = false,
            DeepDiveRecordSignatures = ["QUST"]
        });
        var context = CreateContext(new Quest(new FormKey(TestModKey, 0x802), SkyrimRelease.SkyrimSE));

        Assert.False(LoggingSettings.IsDeepDiveRecord(context));
    }

    [Fact]
    public void DeepDiveLoggingEmitsEveryPropertyIdentifier()
    {
        LogCollector.SetRecordLoggingContext(deepDiveRecord: true, detailedRecord: true);

        LogCollector.Add("EditorID", "EDID diagnostic");
        LogCollector.Add("VirtualMachineAdapter.Scripts", "VMAD diagnostic");

        var logs = LogCollector.GetAll().ToArray();
        Assert.Contains(logs, line => line.Contains("EDID diagnostic", StringComparison.Ordinal));
        Assert.Contains(logs, line => line.Contains("VMAD diagnostic", StringComparison.Ordinal));
    }

    private static IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> CreateContext(
        IMajorRecord record) =>
        new ModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(
            TestModKey,
            record,
            (_, _) => throw new NotSupportedException(),
            (_, _, _, _) => throw new NotSupportedException());
}
