using DreadsMashedPatch.App.Models;
using DreadsMashedPatch.App.Services;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("LogCollector")]
public sealed class DisabledPluginLoadOrderTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), $"DisabledPlugins-{Guid.NewGuid():N}");
    private string Data => Path.Combine(_root, "Data");

    public DisabledPluginLoadOrderTests()
    {
        Directory.CreateDirectory(Data);
        WriteMod("Skyrim.esm");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task OrdinaryDisabledPluginIsNotImportedEvenIfMissingOrMalformed(bool present)
    {
        if (present) File.WriteAllText(Path.Combine(Data, "Disabled.esp"), "invalid plugin contents");
        WriteMod("Active.esp");

        await AssertImportedAsync("Disabled.esp\n*Active.esp\n", "Skyrim.esm", "Active.esp");
    }

    [Fact]
    public async Task DisabledMasterReferencedByActivePluginIsReenabled()
    {
        var dependency = WriteMod("Dependency.esm");
        var active = new SkyrimMod(ModKey.FromNameAndExtension("Active.esp"), SkyrimRelease.SkyrimSE);
        active.Weapons.Add(new Weapon(new FormKey(dependency.ModKey, 0x800), SkyrimRelease.SkyrimSE));
        using (var stream = File.Create(Path.Combine(Data, "Active.esp"))) active.WriteToBinary(stream);
        // Verify the fixture really declares the dependency in its binary header.
        using (var plugin = SkyrimMod.CreateFromBinaryOverlay(Path.Combine(Data, "Active.esp"), SkyrimRelease.SkyrimSE))
            Assert.Contains(plugin.ModHeader.MasterReferences, master => master.Master == dependency.ModKey);

        await AssertImportedAsync("Dependency.esm\n*Active.esp\n", "Skyrim.esm", "Dependency.esm", "Active.esp");
    }

    [Fact]
    public async Task InstalledCreationClubPluginIsIncludedDespiteDisabledListingAfterOutput()
    {
        WriteMod("ccTest.esl");
        File.WriteAllText(Path.Combine(_root, "Skyrim.ccc"), "ccTest.esl\n");

        await AssertImportedAsync("MashedPatch.esp\nccTest.esl\n", "Skyrim.esm", "ccTest.esl");
    }

    [Fact]
    public async Task InstalledImplicitMastersAreIncludedDespiteDisabledListings()
    {
        WriteMod("Update.esm");

        await AssertImportedAsync("Skyrim.esm\nUpdate.esm\n", "Skyrim.esm", "Update.esm");
    }

    [Fact]
    public async Task DisabledOutputStillExcludesLaterEnabledPlugins()
    {
        WriteMod("Before.esp");
        // Neither the disabled output nor the enabled later plugin needs to exist.
        await AssertImportedAsync("*Before.esp\nMashedPatch.esp\n*After.esp\n", "Skyrim.esm", "Before.esp");
    }

    [Fact]
    public async Task FirstDuplicateListingDeterminesWhetherOrdinaryPluginIsEnabled()
    {
        File.WriteAllText(Path.Combine(Data, "Duplicate.esp"), "invalid plugin contents");

        await AssertImportedAsync("Duplicate.esp\n*Duplicate.esp\n", "Skyrim.esm");
    }

    private SkyrimMod WriteMod(string name)
    {
        var mod = new SkyrimMod(ModKey.FromNameAndExtension(name), SkyrimRelease.SkyrimSE);
        using var stream = File.Create(Path.Combine(Data, name));
        mod.WriteToBinary(stream);
        return mod;
    }

    private async Task AssertImportedAsync(string listings, params string[] expected)
    {
        var pluginsPath = Path.Combine(_root, "plugins.txt");
        await File.WriteAllTextAsync(pluginsPath, listings);
        Directory.CreateDirectory(SettingsStore.SettingsDirectory);
        var prepared = await LoadOrderPreparer.CreateAsync(new StandaloneSettings
        {
            GameFolderPath = _root,
            DataFolderPath = Data,
            LoadOrderFilePath = pluginsPath
        });
        try
        {
            string[]? imported = null;
            await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(state =>
                {
                    // Synthesis appends the new, empty patch to the imported inputs.
                    Assert.Same(state.PatchMod, state.LoadOrder.ListedOrder.Last().Mod);
                    imported = state.LoadOrder.ListedOrder.SkipLast(1)
                        .Select(listing => listing.ModKey.FileName.String).ToArray();
                })
                .Run(new RunSynthesisMutagenPatcher
                {
                    GameRelease = GameRelease.SkyrimSE,
                    DataFolderPath = Data,
                    LoadOrderFilePath = prepared.Path,
                    LoadOrderIncludesCreationClub = true,
                    ModKey = "MashedPatch.esp",
                    OutputPath = Path.Combine(_root, "Output", "MashedPatch.esp"),
                    ExtraDataFolder = _root,
                    PersistencePath = Path.Combine(_root, "Persistence"),
                    SplitIfMaxMastersExceeded = true
                });

            Assert.Equal(expected, imported);
            Assert.Equal(listings, await File.ReadAllTextAsync(pluginsPath));
        }
        finally { File.Delete(prepared.Path); }
    }

    public void Dispose() => Directory.Delete(_root, recursive: true);
}
