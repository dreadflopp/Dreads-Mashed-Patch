using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Binary.Parameters;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using Xunit;
using Context = Mutagen.Bethesda.Plugins.Cache.IModContext<Mutagen.Bethesda.Skyrim.ISkyrimMod, Mutagen.Bethesda.Skyrim.ISkyrimModGetter, Mutagen.Bethesda.Plugins.Records.IMajorRecord, Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter>;

namespace DreadsMashedPatch.Tests;

[Collection("CoverageFixes")]
public sealed class InteriorReferencePlacementTests : IDisposable
{
    private static readonly FormKey Key = CoverageTestHistory.RecordKey;
    private static readonly FormKey Settlement = new(CoverageTestHistory.Original, 0xA10);
    private static readonly FormKey Correction = new(CoverageTestHistory.Original, 0xA20);
    private static FormKey Cell(int index) => new(CoverageTestHistory.Original, (uint)(0x900 + index));
    private static FormKey Location(int index) => new(CoverageTestHistory.Original, (uint)(0xA00 + index));
    private sealed record Version(int Cell, float X, FormKey? Location = null, bool Persistent = false, float? Scale = 1);
    private static Version Mine => new(0, 182);
    private static Version House => new(1, 2068, Location(1));
    public void Dispose() => CoverageTestHistory.Reset();

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void GesturMoveKeepsHousePlacementAndLocationWithLaterPersistenceAndScale(bool npc)
    {
        using var h = new History(npc, [Mine, House, Mine with { Location = Location(0), Persistent = true, Scale = 1.25f }],
            edit: (i, actor) =>
            {
                if (i == 2) actor.EnableParent = new EnableParent { Reference = new FormLink<IPlacedGetter>(Constants.Player.FormKey) };
            });
        h.Run();
        var output = h.Reload();
        AssertPlacement(output, Cell(1), 2068, Location(1), true);
        var actor = Actor(output);
        Assert.Equal(1.25f, actor.Scale);
        Assert.Equal(Constants.Player.FormKey, actor.EnableParent!.Reference.FormKey);
        Assert.Equal(Settlement, npc ? ((IPlacedNpcGetter)actor).PersistentLocation.FormKey : ((IPlacedObjectGetter)actor).PersistentLocation.FormKey);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CellOnlyMoveEmitsThroughTheFullPipeline(bool npc)
    {
        using var h = new History(npc, [Mine, Mine with { Cell = 1 }, Mine]);
        h.Run(fullPipeline: true);
        AssertPlacement(h.Reload(), Cell(1), 182, null);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void HouseCellCanBeIntroducedOnlyByTheMovingPlugin(bool npc)
    {
        using var h = new History(npc, [Mine, House, Mine], introducedHouse: true);
        h.Run(fullPipeline: true);
        AssertPlacement(h.Reload(), new FormKey(CoverageTestHistory.Earlier, 0x901), 2068, Location(1));
    }

    [Theory]
    [InlineData(false, "none")]
    [InlineData(false, "declared")]
    [InlineData(false, "virtual")]
    [InlineData(true, "none")]
    [InlineData(true, "declared")]
    [InlineData(true, "virtual")]
    public void SpatialRestorationRetainsExistingPermissions(bool npc, string permission)
    {
        using var h = new History(npc, [Mine, House, Mine with { Location = Location(0) }], permission);
        h.Run();
        var denied = permission == "none";
        AssertPlacement(h.ReloadOrWinner(), Cell(denied ? 1 : 0), denied ? 2068 : 182, Location(denied ? 1 : 0));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void CopiedPlacementDoesNotTakeOwnership(bool npc)
    {
        using var h = new History(npc, [Mine, House, House, Mine], "declared");
        h.Run();
        AssertPlacement(h.ReloadOrWinner(), Cell(0), 182, null);
    }

    [Theory]
    [InlineData(false, "correction")]
    [InlineData(false, "remove-denied")]
    [InlineData(false, "remove-allowed")]
    [InlineData(true, "correction")]
    [InlineData(true, "remove-denied")]
    [InlineData(true, "remove-allowed")]
    public void SameHouseMetadataUsesGlobalOriginalReversionRules(bool npc, string change)
    {
        FormKey? value = change == "correction" ? Correction : null;
        using var h = new History(npc, [Mine, House, House with { Location = value, Scale = 1.25f }],
            change == "remove-allowed" ? "declared" : "none");
        h.Run();
        AssertPlacement(h.ReloadOrWinner(), Cell(1), 2068, change == "remove-denied" ? Location(1) : value);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EffectiveDestinationMetadataAndPatchedSiblingsSurvive(bool existingOutput)
    {
        using var h = new History(true, [Mine, House, Mine], editMod: (i, mod) =>
        {
            if (i == 2) mod.EnumerateMajorRecords<ICell>().Single(c => c.FormKey == Cell(1)).EditorID = "LatestHouse";
        });
        if (existingOutput)
        {
            var destination = h.State.LinkCache.ResolveAllContexts(Cell(1), typeof(ICellGetter)).First();
            var cell = (ICell)destination.GetOrAddAsOverride(h.State.PatchMod);
            cell.EditorID = "PatchedHouse";
            cell.Temporary.Add(new PlacedNpc(new FormKey(CoverageTestHistory.Original, 0xB01), SkyrimRelease.SkyrimSE));
        }
        h.Run();
        var output = h.Reload();
        AssertPlacement(output, Cell(1), 2068, Location(1));
        Assert.Equal(existingOutput ? "PatchedHouse" : "LatestHouse", output.EnumerateMajorRecords<ICellGetter>().Single().EditorID);
        Assert.Equal(existingOutput ? 2 : 1, output.EnumerateMajorRecords<IPlacedGetter>().Count());
    }

    [Theory]
    [InlineData("exterior", false)]
    [InlineData("exterior", true)]
    [InlineData("door", false)]
    [InlineData("deleted", false)]
    [InlineData("invalid-location", false)]
    public void UnsupportedMoveKeepsWinningPlacementAndIndependentScale(string reason, bool npc)
    {
        var moved = reason == "exterior" ? House with { Cell = 2 } : House;
        if (reason == "invalid-location") moved = moved with { Location = new FormKey(CoverageTestHistory.Original, 0xFFFF) };
        using var h = new History(npc, [Mine, moved with { Scale = 1.2f }, Mine],
            edit: (i, actor) =>
            {
                if (reason == "door" && i == 1) ((IPlacedObject)actor).TeleportDestination = new TeleportDestination
                { Door = new FormLink<IPlacedObjectGetter>(new FormKey(CoverageTestHistory.Original, 0xB00)) };
            }, editMod: (i, mod) =>
            {
                if (reason == "deleted" && i == 2) mod.EnumerateMajorRecords<ICell>().Single(c => c.FormKey == Cell(1)).MajorRecordFlagsRaw |= 0x20;
            });
        h.Run();
        var output = h.Reload();
        AssertPlacement(output, Cell(0), 182, null);
        Assert.Equal(1.2f, Actor(output).Scale);
        if (!npc) Assert.Null(((IPlacedObjectGetter)Actor(output)).TeleportDestination);
        Assert.Contains("retaining winning placement", h.Log);
        Assert.Contains(Key.ToString(), h.Log);
        Assert.Contains(reason == "exterior" ? "[Info]" : "[Warning]", h.Log);
        if (reason == "exterior") Assert.DoesNotContain("[Warning]", h.Log);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void IgnoredAndPrioritySourcesKeepExistingPrecedence(bool priority)
    {
        using var h = new History(true, [Mine, House, Mine]);
        PatcherSettings.Apply(priority
            ? new PatcherConfiguration { AlwaysWinningMods = [CoverageTestHistory.Earlier.FileName.String] }
            : new PatcherConfiguration { IgnoredMods = [CoverageTestHistory.Earlier.FileName.String] });
        h.Run();
        AssertPlacement(h.ReloadOrWinner(), Cell(priority ? 1 : 0), priority ? 2068 : 182, priority ? Location(1) : null);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DiagnosticVerbosityCannotChangeSelection(bool detailed)
    {
        using var h = new History(true, [Mine, House, Mine]);
        PatcherSettings.Apply(new PatcherConfiguration { Diagnostics = new DiagnosticsSettings
        { DebugMode = detailed, Verbosity = PatcherLogVerbosity.Detailed } });
        h.Run();
        AssertPlacement(h.Reload(), Cell(1), 2068, Location(1));
    }

    [Fact]
    public void FailedSetterDoesNotPublishDestinationChanges()
    {
        using var h = new History(true, [Mine, House with { Scale = 1.2f }, Mine]);
        var destination = h.State.LinkCache.ResolveAllContexts(Cell(1), typeof(ICellGetter)).First();
        ((ICell)destination.GetOrAddAsOverride(h.State.PatchMod)).EditorID = "AlreadyPatched";
        var before = SkyrimModMixIn.DeepCopy(h.State.PatchMod);
        var handler = new PlacedNpcRecordHandler();
        handler.PropertyHandlers["Scale"] = new FailingScale();
        h.Run(handler: handler, expectFailure: true);
        Assert.True(before.Equals(h.State.PatchMod));
        Assert.Single(h.Report!.SkippedRecords);
    }

    private sealed class FailingScale() : SimpleReflectionPropertyHandler<float?, IPlacedNpc, IPlacedNpcGetter>("Scale")
    {
        public override void SetValue(IMajorRecord record, float? value) => throw new InvalidDataException("Failed scale write");
    }

    private static IPlacedGetter Actor(ISkyrimModGetter mod) => mod.EnumerateMajorRecords<IPlacedGetter>().Single(r => r.FormKey == Key);
    private static void AssertPlacement(ISkyrimModGetter output, FormKey cellKey, float x, FormKey? location, bool persistent = false)
    {
        var actor = Actor(output);
        var cell = Assert.Single(output.EnumerateMajorRecords<ICellGetter>(), c => c.Persistent.Concat(c.Temporary).Any(r => r.FormKey == Key));
        Assert.Equal(cellKey, cell.FormKey);
        Assert.Equal(x, actor.Placement!.Position.X);
        Assert.Equal(location, actor.LocationReference.FormKeyNullable);
        Assert.Equal(persistent, (actor.MajorRecordFlagsRaw & 0x400) != 0);
        Assert.Single(persistent ? cell.Persistent : cell.Temporary, r => r.FormKey == Key);
        Assert.DoesNotContain(persistent ? cell.Temporary : cell.Persistent, r => r.FormKey == Key);
    }

    private sealed class History : IDisposable
    {
        private readonly List<MemoryStream> _streams = [];
        private readonly List<ISkyrimModDisposableGetter> _overlays = [];
        private readonly bool _npc;
        public IPatcherState<ISkyrimMod, ISkyrimModGetter> State { get; }
        public Context[] Contexts { get; }
        public PatchRunReport? Report { get; private set; }
        public string Log { get; private set; } = "";

        public History(bool npc, Version[] versions, string permission = "none", Action<int, IPlaced>? edit = null,
            Action<int, SkyrimMod>? editMod = null, bool introducedHouse = false)
        {
            _npc = npc;
            CoverageTestHistory.Configure();
            var keys = versions.Length == 3
                ? new[] { CoverageTestHistory.Original, CoverageTestHistory.Earlier, CoverageTestHistory.Winner }
                : [CoverageTestHistory.Original, CoverageTestHistory.Earlier, ModKey.FromNameAndExtension("Copied.esp"), CoverageTestHistory.Winner];
            if (permission == "virtual") PatcherSettings.Apply(new PatcherConfiguration { CompatibilityRules =
                [new VirtualMasterRule { TargetMod = keys[^1].FileName.String, VirtualMasters = [keys[1].FileName.String] }] });
            for (var index = 0; index < versions.Length; index++)
            {
                var v = versions[index];
                var mod = new SkyrimMod(keys[index], SkyrimRelease.SkyrimSE);
                if (index > 0) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = keys[0] });
                if (permission == "declared" && index == versions.Length - 1) mod.ModHeader.MasterReferences.Add(new MasterReference { Master = keys[1] });
                mod.Locations.Add(new Location(Settlement, SkyrimRelease.SkyrimSE));
                foreach (var key in new[] { Location(0), Location(1), Correction })
                {
                    var location = new Location(key, SkyrimRelease.SkyrimSE);
                    location.ParentLocation.SetTo(Settlement); mod.Locations.Add(location);
                }
                var cells = Enumerable.Range(0, 3).Select(i => new Cell(
                    i == 1 && introducedHouse ? new FormKey(keys[1], 0x901) : Cell(i), SkyrimRelease.SkyrimSE)
                {
                    Flags = i < 2 ? Mutagen.Bethesda.Skyrim.Cell.Flag.IsInteriorCell : 0,
                    Grid = i < 2 ? null : new CellGrid { Point = new P2Int(0, 0) }
                }).ToArray();
                var block = new CellBlock { BlockNumber = 0, GroupType = GroupTypeEnum.InteriorCellBlock };
                var sub = new CellSubBlock { BlockNumber = 0, GroupType = GroupTypeEnum.InteriorCellSubBlock };
                sub.Cells.Add(cells[0]);
                if (!introducedHouse || index == 1) sub.Cells.Add(cells[1]);
                block.SubBlocks.Add(sub); mod.Cells.Add(block);
                var world = new Worldspace(new FormKey(keys[0], 0x990), SkyrimRelease.SkyrimSE);
                var wb = new WorldspaceBlock { GroupType = GroupTypeEnum.ExteriorCellBlock };
                var ws = new WorldspaceSubBlock { GroupType = GroupTypeEnum.ExteriorCellSubBlock };
                ws.Items.Add(cells[2]); wb.Items.Add(ws); world.SubCells.Add(wb); mod.Worldspaces.Add(world);
                IPlaced actor = npc ? new PlacedNpc(Key, SkyrimRelease.SkyrimSE) : new PlacedObject(Key, SkyrimRelease.SkyrimSE);
                actor.Placement = new Placement { Position = new P3Float(v.X, 0, 100) };
                actor.Scale = v.Scale; actor.MajorRecordFlagsRaw = v.Persistent ? 0x400 : 0;
                if (v.Location.HasValue) actor.LocationReference.SetTo(v.Location.Value);
                if (actor is IPlacedNpc n) n.PersistentLocation.SetTo(Settlement);
                else ((IPlacedObject)actor).PersistentLocation.SetTo(Settlement);
                edit?.Invoke(index, actor);
                (v.Persistent ? cells[v.Cell].Persistent : cells[v.Cell].Temporary).Add(actor);
                editMod?.Invoke(index, mod);
                var stream = new MemoryStream(); _streams.Add(stream);
                mod.WriteToBinary(stream, new BinaryWriteParameters { MastersListContent = MastersListContentOption.NoCheck });
                stream.Position = 0;
                _overlays.Add(SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, mod.ModKey));
            }
            State = CoverageTestHistory.State(_overlays.ToArray());
            Contexts = State.LinkCache.ResolveAllContexts(Key, npc ? typeof(IPlacedNpcGetter) : typeof(IPlacedObjectGetter)).ToArray();
        }

        public void Run(bool fullPipeline = false, AbstractRecordHandler? handler = null, bool expectFailure = false)
        {
            handler ??= _npc ? new PlacedNpcRecordHandler() : new PlacedObjectRecordHandler();
            var previous = Console.Out;
            using var log = new StringWriter();
            try
            {
                Console.SetOut(log);
                Report = fullPipeline ? Program.RunPatchWithReport(State) : handler.Process(State, [Contexts[0]]);
            }
            finally { Console.SetOut(previous); Log = log.ToString(); }
            if (!expectFailure) { Assert.Empty(Report.Errors); Assert.Empty(Report.SkippedRecords); }
        }

        public SkyrimMod ReloadOrWinner() => State.PatchMod.EnumerateMajorRecords<IPlacedGetter>().Any() ? Reload() : SkyrimModMixIn.DeepCopy(_overlays[^1]);
        public SkyrimMod Reload()
        {
            using var stream = new MemoryStream(); State.PatchMod.WriteToBinary(stream); stream.Position = 0;
            using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, State.PatchMod.ModKey);
            return SkyrimModMixIn.DeepCopy(overlay);
        }
        public void Dispose()
        {
            (State as IDisposable)?.Dispose();
            foreach (var overlay in _overlays) overlay.Dispose();
            foreach (var stream in _streams) stream.Dispose();
        }
    }
}
