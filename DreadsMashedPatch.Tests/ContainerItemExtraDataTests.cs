using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Order;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Synthesis.CLI;
using Noggog;
using Xunit;
using ContainerItemHandler = DreadsMashedPatch.PropertyHandlers.Container.ItemHandler;
using NpcItemHandler = DreadsMashedPatch.PropertyHandlers.Npc.ItemHandler;

namespace DreadsMashedPatch.Tests;

[CollectionDefinition("ContainerItemExtraData", DisableParallelization = true)]
public sealed class ContainerItemExtraDataCollection;

[Collection("ContainerItemExtraData")]
public sealed class ContainerItemExtraDataTests : IDisposable
{
    private static readonly ModKey OriginalKey = ModKey.FromNameAndExtension("Skyrim.esm");
    private static readonly ModKey EarlierKey = ModKey.FromNameAndExtension("Earlier.esp");
    private static readonly ModKey WinnerKey = ModKey.FromNameAndExtension("Winner.esp");
    private static readonly ModKey PatchKey = ModKey.FromNameAndExtension("ContainerPatch.esp");
    private static readonly FormKey ContainerKey = new(OriginalKey, 0x800);
    private static readonly FormKey ItemKey = new(OriginalKey, 0x801);
    private static readonly ContainerItemHandler Handler = new();

    public void Dispose() => PatcherSettings.Apply(new PatcherConfiguration());

    [Theory]
    [InlineData("untyped")]
    [InlineData("npc")]
    [InlineData("faction")]
    public void FullRunCountMergeRetainsUnchangedCoed(string ownerKind)
    {
        Configure();
        var mods = CreateMods();
        AddContainers(mods, [Entry(1, Data(ownerKind)), Entry(2, Data(ownerKind)), Entry(1, Data(ownerKind))]);
        using var state = CreateState(mods);

        Program.RunPatch(state);

        var row = Assert.Single(Assert.Single(state.PatchMod.Containers).Items!);
        Assert.Equal(2, row.Item.Count);
        AssertDataEqual(Data(ownerKind), row.Data);
        for (var i = 0; i < mods.Length; i++)
        {
            var input = Assert.Single(Assert.Single(mods[i].Containers).Items!);
            Assert.Equal(i == 1 ? 2 : 1, input.Item.Count);
            AssertDataEqual(Data(ownerKind), input.Data);
        }

        if (ownerKind == "untyped")
        {
            RoundTrip(state.PatchMod, reloaded =>
            {
                var saved = Assert.Single(Assert.Single(reloaded.Containers).Items!);
                Assert.Equal(ItemKey, saved.Item.Item.FormKey);
                Assert.Equal(2, saved.Item.Count);
                AssertDataEqual(Data(ownerKind), saved.Data);
            });
        }
    }

    [Theory]
    [InlineData(1, false, 2)]
    [InlineData(1, true, 1)]
    [InlineData(3, false, 3)]
    public void FullRunPreservesExistingContainerCountSelection(int winnerCount, bool authorized, int expectedCount)
    {
        Configure(authorized);
        var mods = CreateMods();
        AddContainers(mods, [Entry(1, Data("untyped")), Entry(2, Data("untyped")), Entry(winnerCount, Data("untyped"))]);
        using var state = CreateState(mods);

        Program.RunPatch(state);

        if (expectedCount == winnerCount)
            Assert.Empty(state.PatchMod.Containers);
        else
        {
            var row = Assert.Single(Assert.Single(state.PatchMod.Containers).Items!);
            Assert.Equal(expectedCount, row.Item.Count);
            AssertDataEqual(Data("untyped"), row.Data);
        }
    }

    [Theory]
    [InlineData("condition")]
    [InlineData("untypedVariable")]
    [InlineData("untypedOwner")]
    [InlineData("npcGlobal")]
    [InlineData("npcOwner")]
    [InlineData("factionRank")]
    [InlineData("factionOwner")]
    [InlineData("ownerVariant")]
    public void FullRunForwardsCoedOnlyEdits(string edit)
    {
        Configure();
        var original = Data(edit.StartsWith("npc") ? "npc" : edit.StartsWith("faction") ? "faction" : "untyped");
        var changed = original.DeepCopy();
        switch (edit)
        {
            case "condition": changed.ItemCondition = 0.875f; break;
            case "untypedOwner": ((UntypedOwner)changed.Owner).OwnerData.SetTo(new FormKey(OriginalKey, 0x902)); break;
            case "untypedVariable": ((UntypedOwner)changed.Owner).VariableData.SetTo(new FormKey(OriginalKey, 0x902)); break;
            case "npcOwner": ((NpcOwner)changed.Owner).Npc.SetTo(new FormKey(OriginalKey, 0x902)); break;
            case "npcGlobal": ((NpcOwner)changed.Owner).Global.SetTo(new FormKey(OriginalKey, 0x902)); break;
            case "factionOwner": ((FactionOwner)changed.Owner).Faction.SetTo(new FormKey(OriginalKey, 0x902)); break;
            case "factionRank": ((FactionOwner)changed.Owner).RequiredRank = 7; break;
            case "ownerVariant": changed.Owner = Data("faction").Owner; break;
        }
        var mods = CreateMods();
        AddContainers(mods, [Entry(1, original), Entry(1, changed), Entry(1, original.DeepCopy())]);
        using var state = CreateState(mods);

        Program.RunPatch(state);

        AssertDataEqual(changed, Assert.Single(Assert.Single(state.PatchMod.Containers).Items!).Data);
    }

    [Theory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void FullRunCoedRemovalAndRestorationRespectOwnership(bool baselinePresent, bool authorized)
    {
        Configure(authorized);
        var original = baselinePresent ? Data("untyped") : null;
        // Present default COED also matters: absence must never equal a default group.
        var changed = baselinePresent ? null : new ExtraData();
        var mods = CreateMods();
        AddContainers(mods, [Entry(1, original), Entry(1, changed), Entry(1, original?.DeepCopy())]);
        using var state = CreateState(mods);

        Program.RunPatch(state);

        if (authorized)
            Assert.Empty(state.PatchMod.Containers);
        else
        {
            AssertDataEqual(changed, Assert.Single(Assert.Single(state.PatchMod.Containers).Items!).Data);
            RoundTrip(state.PatchMod, reloaded =>
                AssertDataEqual(changed, Assert.Single(Assert.Single(reloaded.Containers).Items!).Data));
        }
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FullRunDuplicateKeysKeepDistinctCoedWhenSourceRowsAreReordered(bool newlyAdded)
    {
        Configure();
        var mods = CreateMods();
        var first = Entry(1, Data("untyped"));
        var second = Entry(4, Data("faction"));
        var changed = first.DeepCopy();
        changed.Item.Count = 2;
        mods[0].Containers.Add(new Container(ContainerKey, SkyrimRelease.SkyrimSE)
        {
            Items = newlyAdded ? null : new ExtendedList<ContainerEntry> { first.DeepCopy(), second.DeepCopy() }
        });
        mods[1].Containers.Add(new Container(ContainerKey, SkyrimRelease.SkyrimSE)
        {
            Items = new ExtendedList<ContainerEntry> { second.DeepCopy(), changed.DeepCopy() }
        });
        mods[2].Containers.Add(new Container(ContainerKey, SkyrimRelease.SkyrimSE)
        {
            Items = newlyAdded ? null : new ExtendedList<ContainerEntry> { first.DeepCopy(), second.DeepCopy() }
        });
        using var state = CreateState(mods);

        Program.RunPatch(state);

        var rows = Assert.Single(state.PatchMod.Containers).Items!;
        Assert.Equal(2, rows.Count);
        Assert.True(Handler.AreValuesEqual([changed, second], rows.ToList()));
        Assert.NotSame(rows[0], rows[1]);
        Assert.NotSame(rows[0].Data, rows[1].Data);
    }

    [Fact]
    public void DuplicateMatchingReservesUnchangedRowsBeforeApplyingEdits()
    {
        Configure();
        var mods = CreateMods();
        var first = Entry(1, Data("untyped"));
        var second = Entry(2, Data("untyped"));
        var changed = Entry(2, Data("faction"));
        mods[0].Containers.Add(new Container(ContainerKey, SkyrimRelease.SkyrimSE)
        {
            Items = new ExtendedList<ContainerEntry> { first, second }
        });
        mods[1].Containers.Add(new Container(ContainerKey, SkyrimRelease.SkyrimSE)
        {
            Items = new ExtendedList<ContainerEntry> { second.DeepCopy(), changed }
        });
        mods[2].Containers.Add(new Container(ContainerKey, SkyrimRelease.SkyrimSE)
        {
            Items = new ExtendedList<ContainerEntry> { first.DeepCopy(), second.DeepCopy() }
        });
        using var state = CreateState(mods);

        Program.RunPatch(state);

        Assert.True(Handler.AreValuesEqual([changed, second], Assert.Single(state.PatchMod.Containers).Items!.ToList()));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CountEditCannotGrantPermissionToRevertCoedInTheSameRow(bool declaredMaster)
    {
        Configure();
        var mods = CreateMods();
        if (declaredMaster) mods[2].ModHeader.MasterReferences.Add(new MasterReference { Master = EarlierKey });
        var changed = Data("untyped");
        changed.ItemCondition = 0.875f;
        AddContainers(mods, [Entry(1, Data("untyped")), Entry(2, changed), Entry(3, Data("untyped"))]);
        using var state = CreateState(mods);

        Program.RunPatch(state);

        if (declaredMaster)
            Assert.Empty(state.PatchMod.Containers);
        else
        {
            var row = Assert.Single(Assert.Single(state.PatchMod.Containers).Items!);
            Assert.Equal(3, row.Item.Count);
            AssertDataEqual(changed, row.Data);
        }
    }

    [Theory]
    [InlineData(false, "untyped")]
    [InlineData(false, "npc")]
    [InlineData(false, "faction")]
    [InlineData(true, "untyped")]
    [InlineData(true, "npc")]
    [InlineData(true, "faction")]
    public void SnapshotsAndDestinationOwnIndependentCompleteRows(bool npc, string ownerKind)
    {
        IPropertyHandler handler = npc ? new NpcItemHandler() : new ContainerItemHandler();
        var sourceRow = Entry(1, Data(ownerKind));
        IMajorRecord source = npc
            ? new Npc(ContainerKey, SkyrimRelease.SkyrimSE) { Items = new ExtendedList<ContainerEntry> { sourceRow } }
            : new Container(ContainerKey, SkyrimRelease.SkyrimSE) { Items = new ExtendedList<ContainerEntry> { sourceRow } };
        var context = new ModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(
            OriginalKey, source, (_, _) => throw new NotSupportedException(),
            (_, _, _, _) => throw new NotSupportedException());
        var propertyContext = Assert.IsType<ListPropertyContext<ContainerEntry>>(handler.CreatePropertyContext());
        handler.InitializeContext(context, context, propertyContext);
        var original = Assert.Single(propertyContext.OriginalValueContexts!).Value;
        var forward = Assert.Single(propertyContext.ForwardValueContexts!).Value;
        IMajorRecord destination = npc ? new Npc(ContainerKey, SkyrimRelease.SkyrimSE) : new Container(ContainerKey, SkyrimRelease.SkyrimSE);
        handler.SetValue(destination, new List<ContainerEntry> { forward });
        var saved = npc ? ((INpc)destination).Items![0] : ((IContainer)destination).Items![0];

        Assert.NotSame(sourceRow, original);
        Assert.NotSame(original, forward);
        Assert.NotSame(forward, saved);
        Assert.NotSame(sourceRow.Data, original.Data);
        Assert.NotSame(original.Data, forward.Data);
        Assert.NotSame(forward.Data, saved.Data);
        Assert.NotSame(sourceRow.Data!.Owner, original.Data!.Owner);
        Assert.NotSame(original.Data.Owner, forward.Data!.Owner);
        Assert.NotSame(forward.Data.Owner, saved.Data!.Owner);
        forward.Item.Count = 9;
        forward.Data.ItemCondition = 0.25f;
        forward.Data.Owner = new UntypedOwner();
        Assert.Equal(1, sourceRow.Item.Count);
        Assert.Equal(1, original.Item.Count);
        Assert.Equal(1, saved.Item.Count);
        AssertDataEqual(Data(ownerKind), sourceRow.Data);
        AssertDataEqual(Data(ownerKind), original.Data);
        AssertDataEqual(Data(ownerKind), saved.Data);
    }

    [Fact]
    public void EqualityIncludesCoedPresenceAndAllMetadataWhileRetainingDuplicateMultiplicity()
    {
        var row = Entry(1, Data("untyped"));
        var differentCondition = row.DeepCopy();
        differentCondition.Data!.ItemCondition = 0.9f;
        var differentOwner = Entry(1, Data("npc"));
        Assert.False(Handler.AreValuesEqual([row], [Entry(2, Data("untyped"))]));
        Assert.False(Handler.AreValuesEqual([row], [differentCondition]));
        Assert.False(Handler.AreValuesEqual([row], [differentOwner]));
        Assert.False(Handler.AreValuesEqual([Entry(1, null)], [Entry(1, new ExtraData())]));
        Assert.True(Handler.AreValuesEqual([row, differentOwner], [differentOwner, row]));
        Assert.False(Handler.AreValuesEqual([row, differentOwner], [row, row]));
    }

    [Theory]
    [InlineData("untyped")]
    [InlineData("npc")]
    [InlineData("faction")]
    public void BinaryRoundTripPreservesOwnerVariantsAndDuplicateRows(string ownerKind)
    {
        var mod = new SkyrimMod(PatchKey, SkyrimRelease.SkyrimSE);
        var ownerKey = new FormKey(PatchKey, 0x900);
        var variableKey = new FormKey(PatchKey, 0x901);
        if (ownerKind == "faction") mod.Factions.Add(new Faction(ownerKey, SkyrimRelease.SkyrimSE));
        else if (ownerKind == "npc") mod.Npcs.Add(new Npc(ownerKey, SkyrimRelease.SkyrimSE));
        else mod.Keywords.Add(new Keyword(ownerKey, SkyrimRelease.SkyrimSE));
        mod.Globals.Add(new GlobalFloat(variableKey, SkyrimRelease.SkyrimSE));
        var data = Data(ownerKind, PatchKey);
        var rows = new List<ContainerEntry> { Entry(2, data), Entry(3, null), Entry(4, new ExtraData()) };
        var container = mod.Containers.AddNew();
        Handler.SetValue(container, rows);

        RoundTrip(mod, reloaded =>
        {
            var saved = Assert.Single(reloaded.Containers).Items!;
            Assert.Equal(3, saved.Count);
            Assert.Equal(new[] { 2, 3, 4 }, saved.Select(row => row.Item.Count));
            Assert.All(saved, row => Assert.Equal(ItemKey, row.Item.Item.FormKey));
            AssertDataEqual(data, saved[0].Data);
            Assert.Null(saved[1].Data);
            AssertDataEqual(new ExtraData(), saved[2].Data);
            Assert.True(Handler.AreValuesEqual(rows, Handler.GetValue(Assert.Single(reloaded.Containers))));
        });
    }

    private static void RoundTrip(ISkyrimModGetter mod, Action<ISkyrimModGetter> verify)
    {
        // The path-based reader builds the record-type cache needed to decode
        // COED owner variants; the stream-overlay overload does not initialize it.
        var directory = Path.Combine(Path.GetTempPath(), $"ContainerCoed-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        try
        {
            var path = Path.Combine(directory, PatchKey.FileName.String);
            mod.WriteToBinary(path);
            using var reloaded = SkyrimMod.CreateFromBinaryOverlay(new ModPath(PatchKey, path), SkyrimRelease.SkyrimSE);
            verify(reloaded);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static ExtraData Data(string ownerKind, ModKey? ownerMod = null)
    {
        var owner = new FormKey(ownerMod ?? OriginalKey, 0x900);
        var variable = new FormKey(ownerMod ?? OriginalKey, 0x901);
        return new ExtraData
        {
            ItemCondition = 0.625f,
            Owner = ownerKind switch
            {
                "untyped" => new UntypedOwner { OwnerData = new FormLink<ISkyrimMajorRecordGetter>(owner), VariableData = new FormLink<ISkyrimMajorRecordGetter>(variable) },
                "npc" => new NpcOwner { Npc = new FormLink<INpcGetter>(owner), Global = new FormLink<IGlobalGetter>(variable) },
                "faction" => new FactionOwner { Faction = new FormLink<IFactionGetter>(owner), RequiredRank = -2 },
                _ => throw new ArgumentException(ownerKind)
            }
        };
    }

    private static ContainerEntry Entry(int count, ExtraData? data) => new()
    {
        Item = new ContainerItem { Item = new FormLink<IItemGetter>(ItemKey), Count = count },
        Data = data
    };

    private static void AssertDataEqual(IExtraDataGetter? expected, IExtraDataGetter? actual)
    {
        if (expected == null) { Assert.Null(actual); return; }
        Assert.NotNull(actual);
        Assert.Equal(expected.ItemCondition, actual.ItemCondition);
        Assert.True(OwnerTargetUtility.AreEqual(expected.Owner, actual.Owner));
    }

    private static void Configure(bool authorized = false) => PatcherSettings.Apply(new PatcherConfiguration
    {
        CompatibilityRules = authorized
            ? [new VirtualMasterRule { TargetMod = WinnerKey.FileName.String, VirtualMasters = [EarlierKey.FileName.String] }]
            : []
    });

    private static SkyrimMod[] CreateMods()
    {
        SkyrimMod[] mods = [new(OriginalKey, SkyrimRelease.SkyrimSE), new(EarlierKey, SkyrimRelease.SkyrimSE), new(WinnerKey, SkyrimRelease.SkyrimSE)];
        foreach (var mod in mods.Skip(1)) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = OriginalKey });
        return mods;
    }

    private static void AddContainers(SkyrimMod[] mods, ContainerEntry[] rows)
    {
        for (var i = 0; i < mods.Length; i++)
            mods[i].Containers.Add(new Container(ContainerKey, SkyrimRelease.SkyrimSE) { Items = new ExtendedList<ContainerEntry> { rows[i] } });
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
            OutputPath = Path.Combine(Path.GetTempPath(), "ContainerPatch.esp"), DataFolderPath = Path.GetTempPath(),
            LoadOrderFilePath = Path.Combine(Path.GetTempPath(), "plugins.txt"), GameRelease = GameRelease.SkyrimSE
        };
        return new SynthesisState<ISkyrimMod, ISkyrimModGetter>(args,
            listings.Select(listing => new LoadOrderListing(listing.ModKey, listing.Enabled)).ToArray(),
            loadOrder, cache, null!, patchMod, null, null, null, CancellationToken.None, null);
    }
#pragma warning restore CS0618
}
