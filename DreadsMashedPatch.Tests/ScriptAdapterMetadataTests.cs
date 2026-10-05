using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using Xunit;

namespace DreadsMashedPatch.Tests;

[CollectionDefinition("ScriptAdapterMetadata", DisableParallelization = true)]
public sealed class ScriptAdapterMetadataCollection;

[Collection("ScriptAdapterMetadata")]
public sealed class ScriptAdapterMetadataTests : IDisposable
{
    private static readonly ModKey OriginalKey = ModKey.FromNameAndExtension("Skyrim.esm");
    private static readonly ModKey EarlierKey = ModKey.FromNameAndExtension("Earlier.esp");
    private static readonly ModKey WinnerKey = ModKey.FromNameAndExtension("Winner.esp");
    private static readonly ModKey PatchKey = ModKey.FromNameAndExtension("ScriptPatch.esp");
    private static readonly FormKey RecordKey = new(OriginalKey, 0x800);
    private static readonly FormKey ObjectKey = new(OriginalKey, 0x801);

    public void Dispose() => PatcherSettings.Apply(new PatcherConfiguration());

    public static IEnumerable<object[]> ListAdapterHandlers()
    {
        foreach (var type in typeof(AbstractRecordHandler).Assembly.GetTypes()
                     .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(AbstractRecordHandler))))
        {
            var handler = (AbstractRecordHandler)System.Activator.CreateInstance(type)!;
            if (handler.PropertyHandlers.TryGetValue("VirtualMachineAdapter", out var property) &&
                property is AbstractScriptListPropertyHandler)
                yield return [type];
        }
    }

    [Theory]
    [MemberData(nameof(ListAdapterHandlers))]
    public void EveryListAdapterPreservesDestinationMetadataAndCopiesScripts(Type handlerType)
    {
        var handler = (AbstractRecordHandler)System.Activator.CreateInstance(handlerType)!;
        var scripts = Assert.IsAssignableFrom<AbstractScriptListPropertyHandler>(handler.PropertyHandlers["VirtualMachineAdapter"]);
        var recordType = typeof(SkyrimMod).Assembly.GetType(
            "Mutagen.Bethesda.Skyrim." + handlerType.Name.Replace("RecordHandler", ""))!;
        var record = (IMajorRecord)System.Activator.CreateInstance(recordType, RecordKey, SkyrimRelease.SkyrimSE)!;
        var adapterProperty = recordType.GetProperty("VirtualMachineAdapter")!;
        var destination = Adapter(4, 1, 1, 7);
        adapterProperty.SetValue(record, destination);
        var selected = Script(2, 99);

        scripts.SetValue(record, [selected]);

        var result = Assert.IsType<VirtualMachineAdapter>(adapterProperty.GetValue(record));
        AssertMetadata(result, 4, 1);
        var saved = Assert.Single(result.Scripts);
        Assert.Equal(2, Assert.IsType<ScriptIntProperty>(saved.Properties[0]).Data);
        Assert.Equal((ushort)7, Assert.IsType<ScriptObjectProperty>(saved.Properties[1]).Unused);
        Assert.NotSame(selected, saved);
        Assert.NotSame(selected.Properties[0], saved.Properties[0]);
        Assert.Equal(1, Assert.IsType<ScriptIntProperty>(Assert.Single(destination.Scripts).Properties[0]).Data);
        Assert.Equal((ushort)99, Assert.IsType<ScriptObjectProperty>(selected.Properties[1]).Unused);

        scripts.SetValue(record, []);
        result = Assert.IsType<VirtualMachineAdapter>(adapterProperty.GetValue(record));
        AssertMetadata(result, 4, 1);
        Assert.Empty(result.Scripts);
        Assert.NotNull(scripts.GetValue(record));

        adapterProperty.SetValue(record, null);
        Assert.Null(scripts.GetValue(record));
        scripts.SetValue(record, []);
        result = Assert.IsType<VirtualMachineAdapter>(adapterProperty.GetValue(record));
        AssertMetadata(result, VirtualMachineAdapter.VersionDefault, VirtualMachineAdapter.ObjectFormatDefault);
        Assert.Empty(result.Scripts);
        scripts.SetValue(record, null);
        // MGEF's existing specialized null setter materializes an empty adapter.
        if (record is MagicEffect) Assert.Empty(Assert.IsType<VirtualMachineAdapter>(adapterProperty.GetValue(record)).Scripts);
        else Assert.Null(adapterProperty.GetValue(record));

        adapterProperty.SetValue(record, null);
        scripts.SetValue(record, [selected]);
        result = Assert.IsType<VirtualMachineAdapter>(adapterProperty.GetValue(record));
        AssertMetadata(result, VirtualMachineAdapter.VersionDefault, VirtualMachineAdapter.ObjectFormatDefault);
        Assert.Equal((ushort)0, Assert.IsType<ScriptObjectProperty>(Assert.Single(result.Scripts).Properties[1]).Unused);
    }

    [Theory]
    [InlineData(4, 1, 4, 1)]
    [InlineData(4, 1, 5, 2)]
    [InlineData(5, 2, 4, 1)]
    public void FullRunScriptReversionPreservesWinnerMetadataAndBinaryObjectLayout(
        short earlierVersion, ushort earlierFormat, short winnerVersion, ushort winnerFormat)
    {
        PatcherSettings.Apply(new PatcherConfiguration());
        var mods = CreateMods();
        AddWeapons(mods, [Adapter(4, 1, 1), Adapter(earlierVersion, earlierFormat, 2), Adapter(winnerVersion, winnerFormat, 1)]);
        using var state = CreateState(mods);

        Program.RunPatch(state);

        var weapon = Assert.Single(state.PatchMod.Weapons);
        AssertMetadata(weapon.VirtualMachineAdapter!, winnerVersion, winnerFormat);
        Assert.Equal(2, Assert.IsType<ScriptIntProperty>(Assert.Single(weapon.VirtualMachineAdapter!.Scripts).Properties[0]).Data);
        RoundTrip(state.PatchMod, mod =>
        {
            var adapter = Assert.Single(mod.Weapons).VirtualMachineAdapter!;
            AssertMetadata(adapter, winnerVersion, winnerFormat);
            var script = Assert.Single(adapter.Scripts);
            Assert.Equal(2, Assert.IsAssignableFrom<IScriptIntPropertyGetter>(script.Properties[0]).Data);
            var obj = Assert.IsAssignableFrom<IScriptObjectPropertyGetter>(script.Properties[1]);
            Assert.Equal(ObjectKey, obj.Object.FormKey);
            Assert.Equal((short)12, obj.Alias);
            Assert.Equal((ushort)7, obj.Unused);
            var objects = Assert.IsAssignableFrom<IScriptObjectListPropertyGetter>(script.Properties[2]);
            Assert.Equal(ObjectKey, Assert.Single(objects.Objects).Object.FormKey);
            Assert.Equal((short)13, objects.Objects[0].Alias);
            Assert.Equal((ushort)8, objects.Objects[0].Unused);
        });
    }

    [Fact]
    public void MetadataOnlyDifferencesDoNotCreateAnIndependentMergeDecision()
    {
        PatcherSettings.Apply(new PatcherConfiguration());
        var mods = CreateMods();
        AddWeapons(mods, [Adapter(4, 1, 1), Adapter(5, 2, 1), Adapter(4, 1, 1)]);
        using var state = CreateState(mods);
        Program.RunPatch(state);
        Assert.Empty(state.PatchMod.Weapons);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(true, false)]
    [InlineData(false, true)]
    [InlineData(true, true)]
    public void FullRunRestoresAbsentOrPresentEmptyAdapterAccordingToPermission(bool presentEmpty, bool authorized)
    {
        PatcherSettings.Apply(new PatcherConfiguration
        {
            CompatibilityRules = authorized
                ? [new VirtualMasterRule { TargetMod = WinnerKey.FileName.String, VirtualMasters = [EarlierKey.FileName.String] }]
                : []
        });
        var mods = CreateMods();
        AddWeapons(mods, [Adapter(4, 1, 1), presentEmpty ? new VirtualMachineAdapter { Version = 4, ObjectFormat = 1 } : null, Adapter(4, 1, 1)]);
        using var state = CreateState(mods);
        Program.RunPatch(state);
        if (authorized)
        {
            Assert.Empty(state.PatchMod.Weapons);
            AssertMetadata(mods[2].Weapons.Single().VirtualMachineAdapter!, 4, 1);
            return;
        }
        var adapter = Assert.Single(state.PatchMod.Weapons).VirtualMachineAdapter;
        if (!presentEmpty) Assert.Null(adapter);
        else
        {
            Assert.NotNull(adapter);
            AssertMetadata(adapter, 4, 1);
            Assert.Empty(adapter.Scripts);
        }
        RoundTrip(state.PatchMod, mod =>
        {
            var saved = Assert.Single(mod.Weapons).VirtualMachineAdapter;
            Assert.Equal(adapter == null, saved == null);
            if (saved != null)
            {
                AssertMetadata(saved, 4, 1);
                Assert.Equal(adapter!.Scripts.Count, saved.Scripts.Count);
            }
        });
    }

    private static void AssertMetadata(IAVirtualMachineAdapterGetter adapter, short version, ushort format)
    {
        Assert.Equal(version, adapter.Version);
        Assert.Equal(format, adapter.ObjectFormat);
    }

    private static ScriptEntry Script(int value, ushort unused = 7)
    {
        var script = new ScriptEntry { Name = "TestScript", Flags = ScriptEntry.Flag.Local };
        script.Properties.Add(new ScriptIntProperty { Name = "Value", Data = value });
        script.Properties.Add(new ScriptObjectProperty
        {
            Name = "Target", Object = new FormLink<ISkyrimMajorRecordGetter>(ObjectKey), Alias = 12, Unused = unused
        });
        var objects = new ScriptObjectListProperty { Name = "Targets" };
        objects.Objects.Add(new ScriptObjectProperty
        {
            Object = new FormLink<ISkyrimMajorRecordGetter>(ObjectKey), Alias = 13, Unused = (ushort)(unused + 1)
        });
        script.Properties.Add(objects);
        return script;
    }

    private static VirtualMachineAdapter Adapter(short version, ushort format, int value, ushort unused = 7)
    {
        var adapter = new VirtualMachineAdapter { Version = version, ObjectFormat = format };
        adapter.Scripts.Add(Script(value, unused));
        return adapter;
    }

    private static SkyrimMod[] CreateMods()
    {
        SkyrimMod[] mods = [new(OriginalKey, SkyrimRelease.SkyrimSE), new(EarlierKey, SkyrimRelease.SkyrimSE), new(WinnerKey, SkyrimRelease.SkyrimSE)];
        foreach (var mod in mods.Skip(1)) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = OriginalKey });
        return mods;
    }

    private static void AddWeapons(SkyrimMod[] mods, VirtualMachineAdapter?[] adapters)
    {
        for (var i = 0; i < mods.Length; i++)
            mods[i].Weapons.Add(new Weapon(RecordKey, SkyrimRelease.SkyrimSE)
            {
                VirtualMachineAdapter = adapters[i], BasicStats = new WeaponBasicStats()
            });
    }

    private static void RoundTrip(ISkyrimModGetter mod, Action<ISkyrimModGetter> verify)
    {
        var directory = Path.Combine(Path.GetTempPath(), $"ScriptMetadata-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            var path = Path.Combine(directory, PatchKey.FileName.String);
            mod.WriteToBinary(path);
            using var reloaded = SkyrimMod.CreateFromBinaryOverlay(new ModPath(PatchKey, path), SkyrimRelease.SkyrimSE);
            verify(reloaded);
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

#pragma warning disable CS0618
    private static SynthesisState<ISkyrimMod, ISkyrimModGetter> CreateState(params SkyrimMod[] mods)
    {
        var patchMod = new SkyrimMod(PatchKey, SkyrimRelease.SkyrimSE);
        var listings = mods.Cast<ISkyrimModGetter>().Append(patchMod).Select(mod => new ModListing<ISkyrimModGetter>(mod)).ToArray();
        var loadOrder = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
        var cache = loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        var args = new RunSynthesisMutagenPatcher
        {
            OutputPath = Path.Combine(Path.GetTempPath(), "ScriptPatch.esp"), DataFolderPath = Path.GetTempPath(),
            LoadOrderFilePath = Path.Combine(Path.GetTempPath(), "plugins.txt"), GameRelease = GameRelease.SkyrimSE
        };
        return new SynthesisState<ISkyrimMod, ISkyrimModGetter>(args,
            listings.Select(listing => new LoadOrderListing(listing.ModKey, listing.Enabled)).ToArray(),
            loadOrder, cache, null!, patchMod, null, null, null, CancellationToken.None, null);
    }
#pragma warning restore CS0618
}
