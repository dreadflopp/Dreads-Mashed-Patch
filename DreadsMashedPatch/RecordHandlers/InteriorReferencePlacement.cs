using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.Contexts.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Context = Mutagen.Bethesda.Plugins.Cache.IModContext<Mutagen.Bethesda.Skyrim.ISkyrimMod, Mutagen.Bethesda.Skyrim.ISkyrimModGetter, Mutagen.Bethesda.Plugins.Records.IMajorRecord, Mutagen.Bethesda.Plugins.Records.IMajorRecordGetter>;
using CellContext = Mutagen.Bethesda.Plugins.Cache.IModContext<Mutagen.Bethesda.Skyrim.ISkyrimMod, Mutagen.Bethesda.Skyrim.ISkyrimModGetter, Mutagen.Bethesda.Skyrim.ICell, Mutagen.Bethesda.Skyrim.ICellGetter>;

namespace DreadsMashedPatch.RecordHandlers;

/// <summary>Interior-only placement selection; publication uses the existing record transaction.</summary>
internal static class InteriorReferencePlacement
{
    private sealed record Snapshot(Context Source, CellContext? Cell, FormKey? World)
    {
        public IPlacedGetter Record => (IPlacedGetter)Source.Record;
        public bool SameCell(Snapshot other) => Cell?.Record.FormKey == other.Cell?.Record.FormKey && World == other.World;
        public static Snapshot Read(Context source)
        {
            source.TryGetParentContext<ICell, ICellGetter>(out var cell);
            source.TryGetParentContext<IWorldspace, IWorldspaceGetter>(out var world);
            return new(source, cell, world?.Record.FormKey);
        }
    }

    // Selection-only adapter, following the existing atomic-group pattern. The
    // registered Placement handler remains the sole comparer/copier for DATA.
    private sealed class Selector(IReadOnlyDictionary<IMajorRecordGetter, Snapshot> snapshots, IPropertyHandler placement)
        : AbstractPropertyHandler<Snapshot>
    {
        public override string PropertyName => "Placement";
        public override Snapshot GetValue(IMajorRecordGetter record) => snapshots[record];
        public override void SetValue(IMajorRecord record, Snapshot? value) => throw new NotSupportedException();
        public override bool AreValuesEqual(Snapshot? left, Snapshot? right) =>
            left == null || right == null ? left == null && right == null
                : left.SameCell(right) && placement.AreValuesEqual(left.Record.Placement, right.Record.Placement);
        public override string FormatValue(object? value) => value is Snapshot s
            ? $"Cell={s.Cell?.Record.FormKey.ToString() ?? "unknown"}, {placement.FormatValue(s.Record.Placement)}" : "null";
    }

    public static PropertyForwardingCoordination Coordinate(IReadOnlyList<Context> contexts,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
        IReadOnlyDictionary<string, IPropertyHandler> handlers,
        IReadOnlyDictionary<string, IPropertyContext> properties,
        PropertyForwardingCoordination assets)
    {
        var winner = Snapshot.Read(contexts[0]);
        if (contexts.Skip(1).All(c => Snapshot.Read(c).SameCell(winner)))
        {
            // These contexts are resolved by this policy, never also by the
            // ordinary pass. Same-parent histories retain the existing algorithm.
            foreach (var context in contexts.Reverse().Skip(1))
            foreach (var key in SelectedProperties)
                handlers[key].UpdatePropertyContext(context, state, properties[key]);
            return assets;
        }

        var history = contexts.Select(Snapshot.Read).ToArray();
        var snapshots = new Dictionary<IMajorRecordGetter, Snapshot>(ReferenceEqualityComparer.Instance);
        foreach (var item in history) snapshots[item.Source.Record] = item;
        var selector = new Selector(snapshots, handlers["Placement"]);
        var selection = new SimplePropertyContext<Snapshot>();
        ((IPropertyHandler)selector).InitializeContext(contexts[^1], contexts[0], selection);
        foreach (var context in contexts.Reverse().Skip(1)) selector.UpdatePropertyContext(context, state, selection);
        var selected = selection.ForwardValueContext!.Value!;
        var location = SelectLocation(selected);
        Context? output = null;
        string? rejection = null;
        if (!selected.SameCell(winner))
        {
            if (history.Any(s => s.Cell == null || s.World != null || !s.Cell.Record.Flags.HasFlag(Cell.Flag.IsInteriorCell)))
                rejection = "Only moves between interior cells are supported";
            else if (history.Any(s => !s.Record.MultiBoundReference.IsNull
                || s.Record is IPlacedObjectGetter obj && HasSpatialDependencies(obj)))
                rejection = "Door, room, occlusion, water or attachment dependencies require a manual patch";
            else
            {
                var destination = state.LinkCache.ResolveAllContexts(selected.Cell!.Record.FormKey, typeof(ICellGetter))
                    .FirstOrDefault(c => !PatcherSettings.IsIgnoredMod(c.ModKey));
                if (destination?.Record is not ICellGetter cell || (cell.MajorRecordFlagsRaw & 0x20) != 0
                    || !cell.Flags.HasFlag(Cell.Flag.IsInteriorCell)
                    || destination.TryGetParentContext<IWorldspace, IWorldspaceGetter>(out _))
                    rejection = "The destination is not an available interior cell";
                else if (location is IFormLinkNullableGetter<ILocationGetter> { FormKeyNullable: { } key }
                    && !state.LinkCache.TryResolve<ILocationGetter>(key, out _))
                    rejection = "The destination LocationReference does not resolve to LCTN";
                else
                {
                    var cellContext = destination.AsType<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter, ICell, ICellGetter>();
                    var persistent = ((int)properties["MajorRecordFlagsRaw"].GetForwardValue()! & 0x400) != 0;
                    output = DestinationContext(contexts[0], cellContext, persistent);
                }
            }
            if (rejection != null)
            {
                selected = winner;
                location = SelectLocation(winner);
                var record = contexts[0].Record;
                var message = $"{RecordTypeCatalog.GetRecordDescription(record)} {record.FormKey} ({record.EditorID ?? "no EditorID"}): {rejection}; retaining winning placement.";
                if (history.All(s => s.Cell != null) && history.Any(s => s.World != null || !s.Cell!.Record.Flags.HasFlag(Cell.Flag.IsInteriorCell)))
                    LogCollector.AddInfo("Placement", message);
                else
                    LogCollector.AddWarning("Placement", message);
            }
        }

        var values = new Dictionary<string, object?>(assets.ForwardValues)
        {
            ["Placement"] = selected.Record.Placement,
            ["LocationReference"] = location
        };
        if (rejection != null)
            foreach (var key in SpatialDependencies)
                if (handlers.TryGetValue(key, out var handler)) values[key] = handler.GetValue(winner.Source.Record);
        var audit = $"[Placement] Winning cell={winner.Cell?.Record.FormKey.ToString() ?? "unknown"}; " +
            $"selected cell={selected.Cell?.Record.FormKey.ToString() ?? "unknown"}; " +
            $"source={(rejection == null ? selection.ForwardValueContext.OwnerMod : winner.Source.ModKey.ToString())}; " +
            $"LocationReference={handlers["LocationReference"].FormatValue(location)}";
        Console.WriteLine(audit);
        return new PropertyForwardingCoordination(values,
            string.IsNullOrEmpty(assets.AuditMessage) ? audit : assets.AuditMessage + Environment.NewLine + audit,
            "Placement", output);

        object? SelectLocation(Snapshot target)
        {
            var eligible = history.Reverse().Where(s => s.SameCell(target)).ToArray();
            var handler = handlers["LocationReference"];
            var context = (SimplePropertyContext<IFormLinkNullableGetter<ILocationGetter>>)handler.CreatePropertyContext();
            // Retain the global original for removal/reversion permission. A
            // copied null must not remove a house location as a new addition.
            handler.InitializeContext(contexts[^1], contexts[0], context);
            context.ForwardValueContext = new(
                (IFormLinkNullableGetter<ILocationGetter>?)handler.GetValue(eligible[0].Source.Record), eligible[0].Source.ModKey.ToString());
            foreach (var item in eligible.Skip(1)) handler.UpdatePropertyContext(item.Source, state, context);
            return context.GetForwardValue();
        }
    }

    // Normal patching processes each FormKey once. Supply the winning baseline
    // with destination ancestry; the existing transaction stages and publishes it.
    private static Context DestinationContext(Context winner, CellContext destination, bool persistent) =>
        new ModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(winner.ModKey, winner.Record,
            (mod, record) =>
            {
                var cell = destination.GetOrAddAsOverride(mod);
                if (!cell.Flags.HasFlag(Cell.Flag.IsInteriorCell) || (cell.MajorRecordFlagsRaw & 0x20) != 0)
                    throw new InvalidOperationException("The output destination is not an available interior cell.");
                var candidate = (IPlaced)record.DeepCopy();
                cell.Persistent.RemoveAll(r => r.FormKey == record.FormKey);
                cell.Temporary.RemoveAll(r => r.FormKey == record.FormKey);
                (persistent ? cell.Persistent : cell.Temporary).Add(candidate);
                return candidate;
            }, (_, _, _, _) => throw new NotSupportedException(), destination);

    public static readonly string[] SelectedProperties = ["Placement", "LocationReference"];
    private static bool HasSpatialDependencies(IPlacedObjectGetter obj) =>
        obj.TeleportDestination != null || obj.NavigationDoorLink != null || obj.Primitive != null || obj.BoundHalfExtents != null
        || obj.OcclusionPlane != null || obj.XORD != null || obj.Portals is { Count: > 0 } || obj.RoomPortal != null || obj.LinkedRooms.Count > 0
        || !obj.ImageSpace.IsNull || !obj.LightingTemplate.IsNull || obj.IsMultiBoundPrimitive || obj.WaterVelocity != null
        || obj.XWCN != null || obj.XWCS != null || obj.XCVL != null || obj.XCZA != null || !obj.XCZR.IsNull || !obj.XCZC.IsNull
        || obj.Reflections.Count > 0 || obj.LitWater.Count > 0 || !obj.AttachRef.IsNull;
    private static readonly string[] SpatialDependencies =
    [
        "MultiBoundReference", "TeleportDestination", "TeleportMessageBox", "NavigationDoorLink", "BoundHalfExtents", "Primitive",
        "OcclusionPlane", "XORD", "Portals", "RoomPortal", "LinkedRooms", "LightingTemplate", "ImageSpace", "IsMultiBoundPrimitive",
        "WaterVelocity", "XWCN", "XWCS", "XCVL", "XCZR", "XCZC", "XCZA", "Reflections", "LitWater", "AttachRef"
    ];
}
