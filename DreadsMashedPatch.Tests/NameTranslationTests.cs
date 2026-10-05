using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using Xunit;

namespace DreadsMashedPatch.Tests;

[CollectionDefinition("NameTranslations", DisableParallelization = true)]
public sealed class NameTranslationsCollection;

[Collection("NameTranslations")]
public sealed class NameTranslationTests : IDisposable
{
    private static readonly ModKey OriginalKey = ModKey.FromNameAndExtension("Skyrim.esm");
    private static readonly ModKey EarlierKey = ModKey.FromNameAndExtension("Earlier.esp");
    private static readonly ModKey WinnerKey = ModKey.FromNameAndExtension("Winner.esp");
    private static readonly ModKey PatchKey = ModKey.FromNameAndExtension("NamePatch.esp");
    private static readonly FormKey RecordKey = new(OriginalKey, 0x800);
    private readonly bool _comparison = TranslatedString.DefaultLanguageComparisonOnly;
    private readonly Language _language = TranslatedString.DefaultLanguage;

    public void Dispose()
    {
        TranslatedString.DefaultLanguageComparisonOnly = _comparison;
        TranslatedString.DefaultLanguage = _language;
        PatcherSettings.Apply(new PatcherConfiguration());
    }

    public static IEnumerable<object[]> MigratedNames()
    {
        // Explicit migration inventory, including required names and ColorRecord's interface alias.
        yield return [typeof(ActivatorRecordHandler), typeof(Mutagen.Bethesda.Skyrim.Activator), false];
        yield return [typeof(ActorValueInformationRecordHandler), typeof(ActorValueInformation), false];
        yield return [typeof(AlchemicalApparatusRecordHandler), typeof(AlchemicalApparatus), false];
        yield return [typeof(AmmunitionRecordHandler), typeof(Ammunition), false];
        yield return [typeof(ArmorRecordHandler), typeof(Armor), false];
        yield return [typeof(BookRecordHandler), typeof(Book), false];
        yield return [typeof(CellRecordHandler), typeof(Cell), false];
        yield return [typeof(ClassRecordHandler), typeof(Mutagen.Bethesda.Skyrim.Class), true];
        yield return [typeof(ColorRecordHandler), typeof(ColorRecord), false];
        yield return [typeof(ContainerRecordHandler), typeof(Container), false];
        yield return [typeof(DialogTopicRecordHandler), typeof(DialogTopic), false];
        yield return [typeof(DoorRecordHandler), typeof(Door), false];
        yield return [typeof(ExplosionRecordHandler), typeof(Explosion), false];
        yield return [typeof(EyesRecordHandler), typeof(Eyes), true];
        yield return [typeof(FactionRecordHandler), typeof(Faction), false];
        yield return [typeof(FloraRecordHandler), typeof(Flora), true];
        yield return [typeof(FurnitureRecordHandler), typeof(Furniture), false];
        yield return [typeof(HazardRecordHandler), typeof(Hazard), false];
        yield return [typeof(HeadPartRecordHandler), typeof(HeadPart), false];
        yield return [typeof(IngestibleRecordHandler), typeof(Ingestible), false];
        yield return [typeof(IngredientRecordHandler), typeof(Ingredient), false];
        yield return [typeof(KeyRecordHandler), typeof(Key), true];
        yield return [typeof(LightRecordHandler), typeof(Light), false];
        yield return [typeof(LocationRecordHandler), typeof(Location), false];
        yield return [typeof(MagicEffectRecordHandler), typeof(MagicEffect), false];
        yield return [typeof(MiscItemRecordHandler), typeof(MiscItem), false];
        yield return [typeof(MoveableStaticRecordHandler), typeof(MoveableStatic), false];
        yield return [typeof(NpcRecordHandler), typeof(Npc), false];
        yield return [typeof(ObjectEffectRecordHandler), typeof(ObjectEffect), false];
        yield return [typeof(ProjectileRecordHandler), typeof(Projectile), false];
        yield return [typeof(ScrollRecordHandler), typeof(Scroll), false];
        yield return [typeof(ShoutRecordHandler), typeof(Shout), false];
        yield return [typeof(SoulGemRecordHandler), typeof(SoulGem), false];
        yield return [typeof(SpellRecordHandler), typeof(Spell), false];
        yield return [typeof(TreeRecordHandler), typeof(Tree), false];
        yield return [typeof(WaterRecordHandler), typeof(Water), false];
        yield return [typeof(WeaponRecordHandler), typeof(Weapon), false];
        yield return [typeof(WorldspaceRecordHandler), typeof(Worldspace), false];
    }

    [Theory]
    [MemberData(nameof(MigratedNames))]
    public void EveryMigratedTranslatedNameRetainsLanguagesAndNullPolicy(
        Type handlerType, Type recordType, bool required)
    {
        var recordHandler = (AbstractRecordHandler)System.Activator.CreateInstance(handlerType)!;
        var handler = recordHandler.PropertyHandlers["Name"];
        Assert.Equal(typeof(TranslatedStringReflectionPropertyHandler<,>), handler.GetType().GetGenericTypeDefinition());
        var source = (IMajorRecord)System.Activator.CreateInstance(recordType, RecordKey, SkyrimRelease.SkyrimSE)!;
        var target = (IMajorRecord)System.Activator.CreateInstance(recordType, RecordKey, SkyrimRelease.SkyrimSE)!;
        var property = recordType.GetProperty("Name")!;
        var name = Name("Selected", "Selection", Language.French);
        property.SetValue(source, name);

        var selected = Assert.IsAssignableFrom<ITranslatedStringGetter>(handler.GetValue(source));
        AssertName(selected, "Selected", "Selection", Language.French);
        recordHandler.ApplyForwardedProperties(target, new Dictionary<string, object?> { ["Name"] = selected });
        var result = Assert.IsType<TranslatedString>(property.GetValue(target));
        Assert.NotSame(name, result);
        AssertName(result, "Selected", "Selection", Language.French);
        result.Set(Language.French, "Output edit");
        Assert.Equal("Selection", name.Lookup(Language.French));
        name.Set(Language.English, "Source edit");
        Assert.Equal("Selected", result.Lookup(Language.English));

        TranslatedString.DefaultLanguageComparisonOnly = true;
        Assert.True(handler.AreValuesEqual(Name("Same", "Un"), Name("Same", "Deux")));
        TranslatedString.DefaultLanguageComparisonOnly = false;
        Assert.False(handler.AreValuesEqual(Name("Same", "Un"), Name("Same", "Deux")));
        Assert.False(handler.AreValuesEqual(Name("Same", "Un"), new TranslatedString(Language.English, "Same")));
        Assert.True(handler.AreValuesEqual(Name("Same", "Un"), Name("Same", "Un")));
        Assert.True(handler.AreValuesEqual(null, null));
        Assert.False(handler.AreValuesEqual(null, name));

        TranslatedString.DefaultLanguage = Language.German;
        handler.SetValue(target, null);
        if (required)
        {
            var empty = Assert.IsType<TranslatedString>(property.GetValue(target));
            Assert.Equal(string.Empty, empty.String);
            Assert.Equal(Language.German, empty.TargetLanguage);
            Assert.NotNull(handler.GetValue(target));
        }
        else
        {
            Assert.Null(property.GetValue(target));
            Assert.Null(handler.GetValue(target));
        }
    }

    [Fact]
    public void MaterialTypeKeepsPlainStringComparisonAndRemoval()
    {
        var handler = Assert.IsType<SimpleReflectionPropertyHandler<string, IMaterialType, IMaterialTypeGetter>>(
            new MaterialTypeRecordHandler().PropertyHandlers["Name"]);
        var record = new MaterialType(RecordKey, SkyrimRelease.SkyrimSE);
        handler.SetValue(record, "Stone");
        Assert.Equal("Stone", handler.GetValue(record));
        Assert.True(handler.AreValuesEqual("Stone", "Stone  "));
        handler.SetValue(record, null);
        Assert.Null(handler.GetValue(record));
    }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(true, false, true)]
    [InlineData(false, false, false)]
    [InlineData(false, false, true)]
    [InlineData(true, true, false)]
    [InlineData(true, true, true)]
    [InlineData(false, true, false)]
    [InlineData(false, true, true)]
    public void FullRunForwardsCompleteNameAccordingToComparisonAndReversionPermission(
        bool defaultOnly, bool translationOnly, bool authorized)
    {
        TranslatedString.DefaultLanguageComparisonOnly = defaultOnly;
        Configure(authorized);
        var original = Name("Original", "Original FR");
        var earlier = Name(translationOnly ? "Original" : "Selected", "Selection FR");
        var mods = CreateMods(original, earlier, original.DeepCopy());
        using var state = CreateState(mods);

        Program.RunPatch(state);

        if (authorized || (defaultOnly && translationOnly)) Assert.Empty(state.PatchMod.Books);
        else
        {
            var result = Assert.Single(state.PatchMod.Books).Name!;
            AssertName(result, earlier.String!, "Selection FR");
            Assert.NotSame(earlier, result);
            AssertName(mods[2].Books.Single().Name!, "Original", "Original FR");
        }
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void FullRunPreservesNameRemovalAndRestoration(bool originalAbsent, bool authorized)
    {
        Configure(authorized);
        var original = originalAbsent ? null : Name("Original", "Original FR");
        var earlier = originalAbsent ? Name("Selected", "Selection FR") : null;
        using var state = CreateState(CreateMods(original, earlier, original?.DeepCopy()));
        Program.RunPatch(state);
        if (authorized) Assert.Empty(state.PatchMod.Books);
        else
        {
            var result = Assert.Single(state.PatchMod.Books).Name;
            if (originalAbsent) AssertName(result!, "Selected", "Selection FR");
            else Assert.Null(result);
        }
    }

    private static TranslatedString Name(string english, string french, Language target = Language.English)
        => new(target, new Dictionary<Language, string>
        {
            [Language.English] = english, [Language.French] = french
        });

    [Fact]
    public void LocalizedOverlayNameSurvivesSelectionCopyAndBinaryRoundTrip()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"NameTranslations-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            var source = new SkyrimMod(EarlierKey, SkyrimRelease.SkyrimSE);
            source.ModHeader.Flags |= SkyrimModHeader.HeaderFlag.Localized;
            source.Books.Add(new Book(RecordKey, SkyrimRelease.SkyrimSE)
            {
                Name = Name("Selected", "Selection FR", Language.French)
            });
            var sourcePath = Path.Combine(directory, EarlierKey.FileName.String);
            source.WriteToBinary(sourcePath);
            var parameters = new BinaryReadParameters
            {
                StringsParam = new StringsReadParameters
                {
                    StringsFolderOverride = Path.Combine(directory, "Strings"), TargetLanguage = Language.French
                }
            };
            var patch = new SkyrimMod(PatchKey, SkyrimRelease.SkyrimSE);
            patch.ModHeader.Flags |= SkyrimModHeader.HeaderFlag.Localized;
            var target = new Book(RecordKey, SkyrimRelease.SkyrimSE);
            var handler = new BookRecordHandler().PropertyHandlers["Name"];
            using (var overlay = SkyrimMod.CreateFromBinaryOverlay(new ModPath(EarlierKey, sourcePath),
                       SkyrimRelease.SkyrimSE, parameters))
            {
                var selected = Assert.IsAssignableFrom<ITranslatedStringGetter>(handler.GetValue(Assert.Single(overlay.Books)));
                handler.SetValue(target, selected);
                Assert.NotSame(selected, target.Name);
            }
            // The generated copy remains usable after the source overlay is disposed.
            AssertName(target.Name!, "Selected", "Selection FR", Language.French);
            patch.Books.Add(target);
            var patchPath = Path.Combine(directory, PatchKey.FileName.String);
            patch.WriteToBinary(patchPath);
            using var reloaded = SkyrimMod.CreateFromBinaryOverlay(new ModPath(PatchKey, patchPath),
                SkyrimRelease.SkyrimSE, parameters);
            AssertName(Assert.Single(reloaded.Books).Name!, "Selected", "Selection FR", Language.French);
        }
        finally { Directory.Delete(directory, recursive: true); }
    }

    private static void AssertName(ITranslatedStringGetter name, string english, string french,
        Language target = Language.English)
    {
        Assert.Equal(target, name.TargetLanguage);
        Assert.Equal(2, name.NumLanguages);
        Assert.Equal(english, name.Lookup(Language.English));
        Assert.Equal(french, name.Lookup(Language.French));
    }

    private static void Configure(bool authorized) => PatcherSettings.Apply(new PatcherConfiguration
    {
        CompatibilityRules = authorized
            ? [new VirtualMasterRule { TargetMod = WinnerKey.FileName.String, VirtualMasters = [EarlierKey.FileName.String] }]
            : []
    });

    private static SkyrimMod[] CreateMods(params TranslatedString?[] names)
    {
        SkyrimMod[] mods = [new(OriginalKey, SkyrimRelease.SkyrimSE), new(EarlierKey, SkyrimRelease.SkyrimSE), new(WinnerKey, SkyrimRelease.SkyrimSE)];
        for (var i = 0; i < mods.Length; i++)
        {
            if (i > 0) mods[i].ModHeader.MasterReferences.Add(new MasterReference { Master = OriginalKey });
            mods[i].Books.Add(new Book(RecordKey, SkyrimRelease.SkyrimSE) { Name = names[i] });
        }
        return mods;
    }

#pragma warning disable CS0618
    private static SynthesisState<ISkyrimMod, ISkyrimModGetter> CreateState(params SkyrimMod[] mods)
    {
        var patch = new SkyrimMod(PatchKey, SkyrimRelease.SkyrimSE);
        var listings = mods.Cast<ISkyrimModGetter>().Append(patch).Select(mod => new ModListing<ISkyrimModGetter>(mod)).ToArray();
        var loadOrder = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
        var cache = loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        var args = new RunSynthesisMutagenPatcher
        {
            OutputPath = Path.Combine(Path.GetTempPath(), PatchKey.FileName.String), DataFolderPath = Path.GetTempPath(),
            LoadOrderFilePath = Path.Combine(Path.GetTempPath(), "plugins.txt"), GameRelease = GameRelease.SkyrimSE
        };
        return new SynthesisState<ISkyrimMod, ISkyrimModGetter>(args,
            listings.Select(listing => new LoadOrderListing(listing.ModKey, listing.Enabled)).ToArray(),
            loadOrder, cache, null!, patch, null, null, null, CancellationToken.None, null);
    }
#pragma warning restore CS0618
}
