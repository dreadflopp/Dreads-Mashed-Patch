using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Assets;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("AlwaysWinningModTests")]
public sealed class ModelBoundsForwardingTests : IDisposable
{
    private static readonly ModKey OriginalKey = ModKey.FromNameAndExtension("Skyrim.esm");
    private static readonly ModKey ModelKey = ModKey.FromNameAndExtension("ModelMod.esp");
    private static readonly ModKey BoundsKey = ModKey.FromNameAndExtension("BoundsMod.esp");
    private static readonly ModKey PatchKey = ModKey.FromNameAndExtension("ModelBoundsPatch.esp");
    private static readonly FormKey RecordKey = new(OriginalKey, 0x800);

    public ModelBoundsForwardingTests()
    {
        var configuration = new PatcherConfiguration();
        configuration.Forwarding.ForwardTreeGroups = true;
        PatcherSettings.Apply(configuration);
    }
    public void Dispose() => PatcherSettings.Apply(new PatcherConfiguration());

    [Theory]
    [MemberData(nameof(GeometryConflictCases))]
    public void RejectedFilenameReversionCannotReplaceBoundsThroughBinaryOutput(string type, bool inheritedBounds)
    {
        var handler = Handler(type);
        var original = Record(type, "Original.nif", 1);
        var modelEdit = Record(type, "Replacement.nif", inheritedBounds ? (short)1 : (short)7);
        var boundsEdit = Record(type, "Original.nif", 9);
        // In the four-source case the last plugin carries the first bounds correction;
        // it does not depend on the intervening model replacer.
        var patch = inheritedBounds
            ? Run(handler, original, Record(type, "Original.nif", 9), modelEdit, boundsEdit)
            : Run(handler, original, modelEdit, boundsEdit);
        var result = Assert.Single(patch.EnumerateMajorRecords());
        var expectedSize = inheritedBounds ? (short)1 : (short)7;
        AssertModelAndBounds(handler, result, "Replacement.nif", expectedSize);

        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, PatchKey);
        AssertModelAndBounds(handler, Assert.Single(overlay.EnumerateMajorRecords()), "Replacement.nif", expectedSize);
    }

    private static IEnumerable<AbstractRecordHandler> RecordHandlers() => typeof(ArmorRecordHandler).Assembly.GetTypes()
        .Where(type => !type.IsAbstract && typeof(AbstractRecordHandler).IsAssignableFrom(type)
            && type.GetConstructor(Type.EmptyTypes) != null)
        .Select(type => (AbstractRecordHandler)System.Activator.CreateInstance(type)!);

    public static IEnumerable<object[]> SharedRecordTypes() => RecordHandlers()
        .Where(handler => handler.PropertyHandlers.Values.OfType<ModelBoundsHandler>().Any())
        .Select(handler => new object[] { handler.GetType().Name.Replace("RecordHandler", "") });

    public static IEnumerable<object[]> StandaloneBoundsRecordTypes() => RecordHandlers()
        .Where(handler => handler.PropertyHandlers.Values.OfType<ObjectBoundsHandler>().Any())
        .Select(handler => new object[] { handler.GetType().Name.Replace("RecordHandler", "") });

    public static IEnumerable<object[]> GeometryConflictCases() => SharedRecordTypes()
        .SelectMany(row => new[] { new object[] { row[0], false }, new object[] { row[0], true } });

    [Fact]
    public void DefaultTreeRestrictionKeepsWinnerModelAndBounds()
    {
        PatcherSettings.Apply(new PatcherConfiguration());
        var patch = Run(Handler("Tree"), Record("Tree", "Original.nif", 1),
            Record("Tree", "Replacement.nif", 7), Record("Tree", "Original.nif", 9));
        Assert.Empty(patch.EnumerateMajorRecords());
    }

    [Theory]
    [MemberData(nameof(SharedRecordTypes))]
    public void BoundsOnlyCorrectionsDoNotChangeTheWinner(string type)
    {
        var handler = Handler(type);
        var patch = Run(handler, Record(type, "Original.nif", 1),
            Record(type, "Replacement.nif", 7), Record(type, "Replacement.nif", 9),
            Record(type, "Replacement.nif", 1));
        Assert.Empty(patch.EnumerateMajorRecords());
    }

    [Theory]
    [MemberData(nameof(SharedRecordTypes))]
    public void ForwardedModelMetadataKeepsWinningBounds(string type)
    {
        var handler = Handler(type);
        var patch = Run(handler, Record(type, "Original.nif", 1, [1]),
            Record(type, "Original.nif", 7, [2]), Record(type, "Original.nif", 1, [1]));
        var result = Assert.Single(patch.EnumerateMajorRecords());
        AssertModelAndBounds(handler, result, "Original.nif", 1);
        Assert.Equal(new byte[] { 2 }, ModelOf(result).Data!.Value.ToArray());
    }

    [Theory]
    [MemberData(nameof(SharedRecordTypes))]
    public void ForwardedFilenameKeepsItsSourceBoundsDespiteLaterBoundsEdits(string type)
    {
        var handler = Handler(type);
        var patch = Run(handler, Record(type, "Original.nif", 1),
            Record(type, "Replacement.nif", 7), Record(type, "Replacement.nif", 9),
            Record(type, "Original.nif", 1));
        AssertModelAndBounds(handler, Assert.Single(patch.EnumerateMajorRecords()), "Replacement.nif", 7);
    }

    [Theory]
    [MemberData(nameof(SharedRecordTypes))]
    public void ReturningToWinningFilenameUsesWinningBoundsRatherThanEarlierSourceBounds(string type)
    {
        var handler = Handler(type);
        var patch = Run(handler, Record(type, "Original.nif", 1, [1]),
            Record(type, "Replacement.nif", 7, [1]), Record(type, "Original.nif", 5, [2]),
            Record(type, "Original.nif", 9, [1]));
        var result = Assert.Single(patch.EnumerateMajorRecords());
        AssertModelAndBounds(handler, result, "Original.nif", 9);
        Assert.Equal(new byte[] { 2 }, ModelOf(result).Data!.Value.ToArray());
    }

    [Theory]
    [InlineData("Armor")]
    [InlineData("Static")]
    public void EquivalentPathCaseAndSeparatorsKeepWinningBounds(string type)
    {
        var handler = Handler(type);
        var winner = Record(type, "original.nif", 9, [1]);
        ModelOf(winner).File = new AssetLink<SkyrimModelAssetType>(@"meshes\original.nif");
        var patch = Run(handler, Record(type, "Original.nif", 1, [1]),
            Record(type, "Original.nif", 7, [2]), winner);
        AssertModelAndBounds(handler, Assert.Single(patch.EnumerateMajorRecords()), "Original.nif", 9);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void OptionalBoundsPreserveAbsenceFromTheChosenSourceThroughBinaryOutput(bool changeFilename)
    {
        var original = (SoulGem)Record("SoulGem", "Original.nif", 1, [1]);
        var earlier = (SoulGem)Record("SoulGem", changeFilename ? "Replacement.nif" : "Original.nif", 7, [2]);
        var winner = (SoulGem)Record("SoulGem", "Original.nif", 1, [1]);
        if (changeFilename) earlier.ObjectBounds = null;
        else winner.ObjectBounds = null;

        var patch = Run(new SoulGemRecordHandler(), original, earlier, winner);
        var result = Assert.Single(patch.SoulGems);
        Assert.Equal(earlier.Model!.File.GivenPath, result.Model!.File.GivenPath);
        Assert.Null(result.ObjectBounds);
        using var stream = new MemoryStream();
        patch.WriteToBinary(stream);
        stream.Position = 0;
        using var reloaded = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, PatchKey);
        Assert.Null(Assert.Single(reloaded.SoulGems).ObjectBounds);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void ArmorBoundsRequireBothGenderPathsToMatch(bool changeFemale, bool omitOtherGender)
    {
        Armor Version(string path, short bounds)
        {
            var armor = (Armor)Record("Armor", "Original.nif", bounds);
            ArmorModel ModelFor(string name) => new()
            {
                Model = new Model { File = new AssetLink<SkyrimModelAssetType>("Meshes/" + name) }
            };
            armor.WorldModel = changeFemale
                ? new GenderedItem<ArmorModel?>(omitOtherGender ? null : ModelFor("Male.nif"), ModelFor(path))
                : new GenderedItem<ArmorModel?>(ModelFor(path), omitOtherGender ? null : ModelFor("Female.nif"));
            return armor;
        }

        var patch = Run(new ArmorRecordHandler(), Version("Original.nif", 1),
            Version("Replacement.nif", 7), Version("Original.nif", 9));
        var result = Assert.Single(patch.Armors);
        Assert.EndsWith("Replacement.nif", (changeFemale
            ? result.WorldModel!.Female : result.WorldModel!.Male)!.Model!.File.GivenPath);
        Assert.Equal(new P3Int16(7, 7, 7), result.ObjectBounds.Second);
    }

    [Theory]
    [InlineData("Armor", false)]
    [InlineData("Armor", true)]
    [InlineData("Static", false)]
    [InlineData("Static", true)]
    public void AuthorizedModelReversionStillTakesItsOwnBounds(string type, bool virtualMaster)
    {
        if (virtualMaster)
        {
            PatcherSettings.Apply(new PatcherConfiguration
            {
                CompatibilityRules = [new VirtualMasterRule
                {
                    TargetMod = BoundsKey.FileName.String,
                    VirtualMasters = [ModelKey.FileName.String]
                }]
            });
        }

        var patch = Run(Handler(type), mods =>
        {
            if (!virtualMaster) mods[2].ModHeader.MasterReferences.Add(new MasterReference { Master = ModelKey });
        }, Record(type, "Original.nif", 1), Record(type, "Replacement.nif", 7), Record(type, "Original.nif", 9));
        // Selection matches the final source, including that source's bounds.
        Assert.Empty(patch.EnumerateMajorRecords());
    }

    [Theory]
    [MemberData(nameof(StandaloneBoundsRecordTypes))]
    public void StandaloneBoundsRemainWinnerOwned(string type)
    {
        var handler = Handler(type);
        IMajorRecord Version(short size)
        {
            var record = NewRecord(type);
            Assert.False(record is IModeledGetter, type);
            Assert.False(record is IArmorGetter, type);
            handler.PropertyHandlers["ObjectBounds"].SetValue(record,
                new ObjectBounds { Second = new P3Int16(size, size, size) });
            return record;
        }

        Assert.Empty(Run(handler, Version(1), Version(7), Version(1)).EnumerateMajorRecords());
    }

    private static AbstractRecordHandler Handler(string type) =>
        (AbstractRecordHandler)System.Activator.CreateInstance(typeof(ArmorRecordHandler).Assembly
            .GetType("DreadsMashedPatch.RecordHandlers." + type + "RecordHandler")!)!;

    private static IMajorRecord NewRecord(string type)
    {
        var recordType = typeof(Armor).Assembly.GetType("Mutagen.Bethesda.Skyrim." + type)!;
        return (IMajorRecord)System.Activator.CreateInstance(recordType, RecordKey, SkyrimRelease.SkyrimSE)!;
    }

    private static IMajorRecord Record(string type, string path, short bounds, byte[]? modelData = null)
    {
        var record = NewRecord(type);
        var model = new Model
        {
            File = new AssetLink<SkyrimModelAssetType>("Meshes/" + path),
            Data = modelData == null ? (MemorySlice<byte>?)null : new MemorySlice<byte>(modelData)
        };
        if (record is Armor armor)
            armor.WorldModel = new GenderedItem<ArmorModel?>(new ArmorModel { Model = model }, null);
        else
            ((IModeled)record).Model = model;
        new ObjectBoundsHandler().SetValue(record, new ObjectBounds
        {
            First = new P3Int16((short)-bounds, (short)-bounds, (short)-bounds),
            Second = new P3Int16(bounds, bounds, bounds)
        });
        return record;
    }

    private static Model ModelOf(IMajorRecord record) => record is Armor armor
        ? armor.WorldModel!.Male!.Model!
        : ((IModeled)record).Model!;

    private static void AssertModelAndBounds(AbstractRecordHandler handler, IMajorRecordGetter record, string path, short size)
    {
        var group = Assert.Single(handler.PropertyHandlers.Values.OfType<ModelBoundsHandler>());
        var value = Assert.IsType<ModelBoundsValue>(group.GetValue(record));
        var model = value.Model is IGenderedItemGetter<IArmorModelGetter?> armorModels
            ? armorModels.Male!.Model!
            : Assert.IsAssignableFrom<IModelGetter>(value.Model);
        Assert.EndsWith(path, model.File.GivenPath);
        Assert.Equal(new P3Int16(size, size, size), value.Bounds!.Second);
    }

    private static SkyrimMod Run(AbstractRecordHandler handler, params IMajorRecord[] records) =>
        Run(handler, null, records);

    private static SkyrimMod Run(AbstractRecordHandler handler, Action<SkyrimMod[]>? configureMods,
        params IMajorRecord[] records)
    {
        var keys = new[] { OriginalKey, ModelKey, BoundsKey, ModKey.FromNameAndExtension("Later.esp") };
        var mods = records.Select((record, i) =>
        {
            var mod = new SkyrimMod(keys[i], SkyrimRelease.SkyrimSE);
            if (i != 0) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = OriginalKey });
            var groupProperty = typeof(SkyrimMod).GetProperties().Single(property =>
                property.PropertyType.IsGenericType
                && property.PropertyType.GetGenericTypeDefinition() == typeof(SkyrimGroup<>)
                && property.PropertyType.GenericTypeArguments[0] == record.GetType());
            var group = groupProperty.GetValue(mod)!;
            group.GetType().GetMethod("Add", new[] { record.GetType() })!.Invoke(group, new object[] { record });
            return mod;
        }).ToArray();
        configureMods?.Invoke(mods);
        var patch = new SkyrimMod(PatchKey, SkyrimRelease.SkyrimSE);
        var listings = mods.Cast<ISkyrimModGetter>().Append(patch)
            .Select(mod => new ModListing<ISkyrimModGetter>(mod)).ToArray();
        var order = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
#pragma warning disable CS0618
        using var state = new SynthesisState<ISkyrimMod, ISkyrimModGetter>(
            new RunSynthesisMutagenPatcher
            {
                OutputPath = Path.Combine(Path.GetTempPath(), "ModelBoundsPatch.esp"),
                DataFolderPath = Path.GetTempPath(),
                LoadOrderFilePath = Path.Combine(Path.GetTempPath(), "unused-plugins.txt"),
                GameRelease = GameRelease.SkyrimSE
            }, listings.Select(mod => new LoadOrderListing(mod.ModKey, mod.Enabled)).ToArray(), order,
            order.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(), null!, patch,
            null, null, null, CancellationToken.None, null);
#pragma warning restore CS0618
        var contexts = order.PriorityOrder.WinningContextOverrides<ISkyrimMod, ISkyrimModGetter,
            IMajorRecord, IMajorRecordGetter>(state.LinkCache).ToArray();
        var report = handler.Process(state, contexts);
        Assert.True(report.Succeeded);
        Assert.Empty(report.SkippedRecords);
        return patch;
    }
}
