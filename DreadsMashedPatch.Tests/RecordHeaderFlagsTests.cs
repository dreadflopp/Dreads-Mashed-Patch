using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

[CollectionDefinition("RecordHeaderFlags", DisableParallelization = true)]
public sealed class RecordHeaderFlagsCollection;

[Collection("RecordHeaderFlags")]
public sealed class RecordHeaderFlagsTests : IDisposable
{
    private static readonly ModKey OriginalKey = ModKey.FromNameAndExtension("Skyrim.esm");
    private static readonly ModKey EarlierKey = ModKey.FromNameAndExtension("Earlier.esp");
    private static readonly ModKey WinnerKey = ModKey.FromNameAndExtension("Winner.esp");
    private static readonly ModKey PatchKey = ModKey.FromNameAndExtension("HeaderPatch.esp");
    private const int UnknownBit = 0x00200000;
    private const int Disabled = (int)SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled;

    public void Dispose() => PatcherSettings.Apply(new PatcherConfiguration());

    [Fact]
    public void EveryRecordHandlerUsesOneHeaderPathAndPreservesUnknownWinnerBits()
    {
        var handlerTypes = typeof(AbstractRecordHandler).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(AbstractRecordHandler)))
            .ToArray();
        Assert.NotEmpty(handlerTypes);
        foreach (var type in handlerTypes)
        {
            var handler = (AbstractRecordHandler)System.Activator.CreateInstance(type)!;
            var rawHandler = Assert.IsType<MajorRecordFlagsRawHandler>(handler.PropertyHandlers["MajorRecordFlagsRaw"]);
            Assert.DoesNotContain("SkyrimMajorRecordFlags", handler.PropertyHandlers.Keys);
            Assert.DoesNotContain("MajorFlags", handler.PropertyHandlers.Keys);
            var flags = Assert.IsType<MajorRecordFlagsRawHandler>(handler.PropertyHandlers["MajorRecordFlagsRaw"]);
            Assert.True(flags.AreValuesEqual(0, UnknownBit));
            foreach (var value in Enum.GetValues<SkyrimMajorRecord.SkyrimMajorRecordFlag>())
            {
                var bit = (int)value;
                Assert.False(flags.AreValuesEqual(0, bit));
                var record = new Keyword(new FormKey(OriginalKey, 0x801), SkyrimRelease.SkyrimSE)
                {
                    MajorRecordFlagsRaw = UnknownBit | bit
                };
                rawHandler.SetValue(record, 0);
                Assert.Equal(UnknownBit, record.MajorRecordFlagsRaw);
                rawHandler.SetValue(record, bit);
                Assert.Equal(UnknownBit | bit, record.MajorRecordFlagsRaw);
            }
        }
    }

    [Theory]
    [MemberData(nameof(TypedHeaders))]
    public void CompositeOwnsEveryRecordSpecificBitIncludingUnsignedBit31(Type handlerType, Type enumType)
    {
        var handler = (AbstractRecordHandler)System.Activator.CreateInstance(handlerType)!;
        var flags = Assert.IsType<MajorRecordFlagsRawHandler>(handler.PropertyHandlers["MajorRecordFlagsRaw"]);
        foreach (var value in Enum.GetValues(enumType))
        {
            var bit = unchecked((int)Convert.ToInt64(value));
            Assert.False(flags.AreValuesEqual(0, bit));
            var record = new Keyword(new FormKey(OriginalKey, 0x801), SkyrimRelease.SkyrimSE)
            {
                MajorRecordFlagsRaw = UnknownBit | bit
            };
            handler.ApplyForwardedProperties(record, new() { ["MajorRecordFlagsRaw"] = 0 });
            Assert.Equal(UnknownBit, record.MajorRecordFlagsRaw);
            handler.ApplyForwardedProperties(record, new() { ["MajorRecordFlagsRaw"] = bit });
            Assert.Equal(UnknownBit | bit, record.MajorRecordFlagsRaw);
        }
    }

    [Theory]
    [InlineData(Disabled, 0, Disabled, 0, false)]
    [InlineData(0, Disabled, 0, Disabled, false)]
    [InlineData(Disabled, 0, Disabled, Disabled, true)]
    [InlineData(0, Disabled, 0, 0, true)]
    [InlineData(0x4000, 0, 0x4000, 0, false)]
    [InlineData(0, 0x4000, 0, 0x4000, false)]
    public void FullRunHonorsSetsClearsAndAuthorizedReversions(
        int originalFlags, int earlierFlags, int winnerFlags, int expectedFlags, bool authorized)
    {
        Configure(authorized);
        var mods = CreateMods();
        var formKey = new FormKey(OriginalKey, 0x802);
        var flags = new[] { originalFlags, earlierFlags, winnerFlags };
        for (var i = 0; i < mods.Length; i++)
            mods[i].Keywords.Add(new Keyword(formKey, SkyrimRelease.SkyrimSE) { MajorRecordFlagsRaw = flags[i] });
        using var state = CreateState(mods);

        Program.RunPatch(state);

        if (expectedFlags == winnerFlags) Assert.Empty(state.PatchMod.Keywords);
        else Assert.Equal(expectedFlags, Assert.Single(state.PatchMod.Keywords).MajorRecordFlagsRaw);
    }

    [Fact]
    public void FullRunMergesRawCommonAndTypedFlagsTogetherAndPreservesUnknownBitsInBinary()
    {
        Configure();
        var mods = CreateMods();
        var formKey = new FormKey(OriginalKey, 0x803);
        var flags = new[] { Disabled, 0x4000 | (int)Weapon.MajorFlag.NonPlayable, Disabled | UnknownBit };
        for (var i = 0; i < mods.Length; i++)
            mods[i].Weapons.Add(new Weapon(formKey, SkyrimRelease.SkyrimSE) { MajorRecordFlagsRaw = flags[i] });
        using var state = CreateState(mods);

        Program.RunPatch(state);

        const int expected = 0x4000 | (int)Weapon.MajorFlag.NonPlayable | UnknownBit;
        Assert.Equal(expected, Assert.Single(state.PatchMod.Weapons).MajorRecordFlagsRaw);
        using var stream = new MemoryStream();
        state.PatchMod.WriteToBinary(stream);
        stream.Position = 0;
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, PatchKey);
        Assert.Equal(expected, Assert.Single(overlay.Weapons).MajorRecordFlagsRaw);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void FullRunKeepsSafeUdrAndAuthorizedRestorationCoherent(bool npc, bool restore)
    {
        Configure(restore);
        var baseMods = CreateMods();
        SkyrimMod[] mods = [baseMods[0], new(ModKey.FromNameAndExtension("RawEdit.esp"), SkyrimRelease.SkyrimSE), baseMods[1], baseMods[2]];
        var formKey = new FormKey(OriginalKey, 0x804);
        var cellKey = new FormKey(OriginalKey, 0x805);
        for (var i = 0; i < mods.Length; i++)
        {
            IPlaced placed = npc
                ? new PlacedNpc(formKey, SkyrimRelease.SkyrimSE)
                : new PlacedObject(formKey, SkyrimRelease.SkyrimSE);
            placed.Placement = new Placement { Position = new P3Float(10, 20, 30) };
            if (i == 2)
            {
                placed.MajorRecordFlagsRaw = Disabled;
                placed.Placement.Position = new P3Float(10, 20, -30000);
                placed.EnableParent = new EnableParent
                {
                    Reference = new FormLink<IPlacedGetter>(Constants.Player.FormKey),
                    Flags = EnableParent.Flag.SetEnableStateToOppositeOfParent
                };
            }
            if (i == 3)
            {
                // Preserve the winner's unknown bits alongside independently selected edits.
                placed.MajorRecordFlagsRaw = UnknownBit;
            }
            if (i is 1 or 2) placed.MajorRecordFlagsRaw |= 0x4000;
            var cell = new Cell(cellKey, SkyrimRelease.SkyrimSE);
            cell.Temporary.Add(placed);
            var subBlock = new CellSubBlock();
            subBlock.Cells.Add(cell);
            var block = new CellBlock();
            block.SubBlocks.Add(subBlock);
            mods[i].Cells.Add(block);
        }
        using var state = CreateState(mods);

        Program.RunPatch(state);

        var placedOutput = Assert.Single(state.PatchMod.EnumerateMajorRecords<IPlacedGetter>());
        Assert.Equal(UnknownBit | 0x4000 | (restore ? 0 : Disabled), placedOutput.MajorRecordFlagsRaw);
        if (restore)
        {
            Assert.Equal(30, placedOutput.Placement!.Position.Z);
            Assert.Null(placedOutput.EnableParent);
        }
        else
        {
            Assert.Equal(-30000, placedOutput.Placement!.Position.Z);
            Assert.Equal(Constants.Player.FormKey, placedOutput.EnableParent!.Reference.FormKey);
            Assert.Equal(EnableParent.Flag.SetEnableStateToOppositeOfParent, placedOutput.EnableParent.Flags);
        }
    }

    private static void Configure(bool authorized = false)
    {
        PatcherSettings.Apply(new PatcherConfiguration
        {
            CompatibilityRules = authorized
                ? [new VirtualMasterRule { TargetMod = WinnerKey.FileName.String, VirtualMasters = [EarlierKey.FileName.String] }]
                : []
        });
    }

    private static SkyrimMod[] CreateMods() =>
        [new(OriginalKey, SkyrimRelease.SkyrimSE), new(EarlierKey, SkyrimRelease.SkyrimSE), new(WinnerKey, SkyrimRelease.SkyrimSE)];

#pragma warning disable CS0618
    private static SynthesisState<ISkyrimMod, ISkyrimModGetter> CreateState(params SkyrimMod[] mods)
    {
        var patchMod = new SkyrimMod(PatchKey, SkyrimRelease.SkyrimSE);
        var listings = mods.Cast<ISkyrimModGetter>().Append(patchMod).Select(mod => new ModListing<ISkyrimModGetter>(mod)).ToArray();
        var loadOrder = new LoadOrder<IModListing<ISkyrimModGetter>>(listings);
        var cache = loadOrder.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();
        var args = new RunSynthesisMutagenPatcher
        {
            OutputPath = Path.Combine(Path.GetTempPath(), "HeaderPatch.esp"),
            DataFolderPath = Path.GetTempPath(),
            LoadOrderFilePath = Path.Combine(Path.GetTempPath(), "plugins.txt"),
            GameRelease = GameRelease.SkyrimSE
        };
        return new SynthesisState<ISkyrimMod, ISkyrimModGetter>(args,
            listings.Select(listing => new LoadOrderListing(listing.ModKey, listing.Enabled)).ToArray(),
            loadOrder, cache, null!, patchMod, null, null, null, CancellationToken.None, null);
    }
#pragma warning restore CS0618

    public static IEnumerable<object[]> TypedHeaders()
    {
        yield return [typeof(ActivatorRecordHandler), typeof(Mutagen.Bethesda.Skyrim.Activator.MajorFlag)];
        yield return [typeof(AmmunitionRecordHandler), typeof(Ammunition.MajorFlag)];
        yield return [typeof(ArmorRecordHandler), typeof(Mutagen.Bethesda.Skyrim.Armor.MajorFlag)];
        yield return [typeof(CellRecordHandler), typeof(Cell.MajorFlag)];
        yield return [typeof(CombatStyleRecordHandler), typeof(CombatStyle.MajorFlag)];
        yield return [typeof(ContainerRecordHandler), typeof(Container.MajorFlag)];
        yield return [typeof(DialogResponseRecordHandler), typeof(DialogResponses.MajorFlag)];
        yield return [typeof(DoorRecordHandler), typeof(Door.MajorFlag)];
        yield return [typeof(EyesRecordHandler), typeof(Eyes.MajorFlag)];
        yield return [typeof(FurnitureRecordHandler), typeof(Furniture.MajorFlag)];
        yield return [typeof(GlobalFloatRecordHandler), typeof(Global.MajorFlag)];
        yield return [typeof(GlobalIntRecordHandler), typeof(Global.MajorFlag)];
        yield return [typeof(GlobalShortRecordHandler), typeof(Global.MajorFlag)];
        yield return [typeof(GlobalUnknownRecordHandler), typeof(Global.MajorFlag)];
        yield return [typeof(HeadPartRecordHandler), typeof(HeadPart.MajorFlag)];
        yield return [typeof(IdleMarkerRecordHandler), typeof(IdleMarker.MajorFlag)];
        yield return [typeof(IngestibleRecordHandler), typeof(Ingestible.MajorFlag)];
        yield return [typeof(KeyRecordHandler), typeof(Key.MajorFlag)];
        yield return [typeof(LightRecordHandler), typeof(Light.MajorFlag)];
        yield return [typeof(LoadScreenRecordHandler), typeof(LoadScreen.MajorFlag)];
        yield return [typeof(MiscItemRecordHandler), typeof(MiscItem.MajorFlag)];
        yield return [typeof(MoveableStaticRecordHandler), typeof(MoveableStatic.MajorFlag)];
        yield return [typeof(NavigationMeshRecordHandler), typeof(NavigationMesh.MajorFlag)];
        yield return [typeof(NpcRecordHandler), typeof(Npc.MajorFlag)];
        yield return [typeof(PerkRecordHandler), typeof(Perk.MajorFlag)];
        yield return [typeof(PlacedHazardRecordHandler), typeof(APlacedTrap.MajorFlag)];
        yield return [typeof(PlacedNpcRecordHandler), typeof(PlacedNpc.MajorFlag)];
        yield return [typeof(RaceRecordHandler), typeof(Race.MajorFlag)];
        yield return [typeof(RegionRecordHandler), typeof(Region.MajorFlag)];
        yield return [typeof(RelationshipRecordHandler), typeof(Relationship.MajorFlag)];
        yield return [typeof(ShoutRecordHandler), typeof(Shout.MajorFlag)];
        yield return [typeof(SoulGemRecordHandler), typeof(SoulGem.MajorFlag)];
        yield return [typeof(StaticRecordHandler), typeof(Mutagen.Bethesda.Skyrim.Static.MajorFlag)];
        yield return [typeof(TalkingActivatorRecordHandler), typeof(TalkingActivator.MajorFlag)];
        yield return [typeof(TreeRecordHandler), typeof(Tree.MajorFlag)];
        yield return [typeof(WeaponRecordHandler), typeof(Weapon.MajorFlag)];
        yield return [typeof(WorldspaceRecordHandler), typeof(Worldspace.MajorFlag)];
    }
}
