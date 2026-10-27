using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.Contexts.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace DreadsMashedPatch.AssetForwarding;

/// <summary>
/// One policy decision per explicitly declared group. Registered member handlers retain
/// comparison and writing, but their independent forwarding contexts are never processed.
/// No generic property/list/flag algorithm is changed for other properties.
/// </summary>
public static class AssetForwardingPolicy
{
    public static void ResolveMemberContexts(Type recordHandlerType,
        IReadOnlyDictionary<string, IPropertyHandler> handlers,
        IDictionary<string, IPropertyContext> contexts)
    {
        foreach (var group in AssetGroupCatalog.For(recordHandlerType))
        foreach (var member in group.Members)
        {
            if (!handlers.ContainsKey(member) || !contexts.TryGetValue(member, out var context))
                throw new InvalidOperationException($"{recordHandlerType.Name}: asset group {group.Name} has no member {member}.");
            context.IsResolved = true;
        }
    }

    public static PropertyForwardingCoordination Coordinate(Type recordHandlerType,
        IReadOnlyDictionary<string, IPropertyHandler> handlers,
        IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> contexts,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
        IReadOnlySet<string> structuralTriggers,
        PropertyForwardingCoordination? prior = null)
    {
        var groups = AssetGroupCatalog.For(recordHandlerType);
        if (groups.Count == 0) return prior ?? PropertyForwardingCoordination.None;
        var winner = contexts[0];
        var history = contexts.Reverse().ToArray();
        // Match the existing record-wide ownership reset. Never use an intermediate reset
        // context as the effective winner of a restricted asset group.
        var start = 0;
        for (var i = 1; i < history.Length; i++)
            if (structuralTriggers.Any(key => !handlers[key].AreValuesEqual(
                handlers[key].GetValue(history[i - 1].Record), handlers[key].GetValue(history[i].Record))))
                start = i;
        var eligible = history[start..];
        var values = prior == null ? new Dictionary<string, object?>()
            : new Dictionary<string, object?>(prior.ForwardValues);
        var audit = new List<string>();
        if (prior?.AuditMessage != null) audit.Add(prior.AuditMessage);
        foreach (var group in groups)
        {
            var allow = PatcherSettings.AllowsForwarding(group.Family);
            IReadOnlyDictionary<string, object?> selected;
            if (!allow)
                selected = group.Members.ToDictionary(key => key, key => handlers[key].GetValue(winner.Record));
            else if (group.Members.Count == 1)
            {
                // A pre-existing complete value keeps its specialized selection, e.g.
                // model/source bounds or ordinary keyed script-entry merging.
                var key = group.Members[0];
                var handler = handlers[key];
                var context = handler.CreatePropertyContext();
                handler.InitializeContext(eligible[0], winner, context);
                foreach (var current in eligible.Skip(1))
                    if (!context.IsResolved) handler.UpdatePropertyContext(current, state, context);
                selected = new Dictionary<string, object?> { [key] = context.GetForwardValue() };
            }
            else
                selected = SelectSnapshot(group, handlers, eligible, winner, state);
            foreach (var member in selected)
            {
                if (!values.TryAdd(member.Key, member.Value))
                    throw new InvalidOperationException($"Overlapping coordination ownership for {recordHandlerType.Name}.{member.Key}.");
            }
            audit.Add($"{group.Name} [{group.Family}]: {(allow ? "forward coherent group" : "preserve effective winner")}");
        }
        return new PropertyForwardingCoordination(values, string.Join("; ", audit), "Assets");
    }

    private static IReadOnlyDictionary<string, object?> SelectSnapshot(AssetGroup group,
        IReadOnlyDictionary<string, IPropertyHandler> handlers,
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] history,
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winner,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        var selector = new SnapshotSelectionHandler(group, handlers);
        var context = new SimplePropertyContext<Snapshot>();
        ((IPropertyHandler)selector).InitializeContext(history[0], winner, context);
        var bounds = group.Members.Where(key => handlers[key] is ModelBoundsHandler)
            .ToDictionary(key => key, key => ((ModelBoundsValue)handlers[key].GetValue(history[0].Record)!).Bounds);
        foreach (var current in history.Skip(1))
        {
            var before = context.ForwardValueContext!.Value!;
            selector.UpdatePropertyContext(current, state, context);
            var after = context.ForwardValueContext!.Value!;
            if (ReferenceEquals(before, after)) continue;
            foreach (var key in bounds.Keys.ToArray())
            {
                var modelHandler = (ModelBoundsHandler)handlers[key];
                var left = (ModelBoundsValue)before.Values[key]!;
                var right = (ModelBoundsValue)after.Values[key]!;
                if (!Equals(modelHandler.FilenameIdentity(left), modelHandler.FilenameIdentity(right)))
                    bounds[key] = right.Bounds;
            }
        }
        var result = new Dictionary<string, object?>(context.ForwardValueContext!.Value!.Values);
        foreach (var (key, sourceBounds) in bounds)
        {
            var handler = (ModelBoundsHandler)handlers[key];
            var selected = (ModelBoundsValue)result[key]!;
            var winning = (ModelBoundsValue)handler.GetValue(winner.Record);
            result[key] = selected with
            {
                Bounds = Equals(handler.FilenameIdentity(selected), handler.FilenameIdentity(winning))
                    ? winning.Bounds : sourceBounds
            };
        }
        return result;
    }

    private sealed record Snapshot(IReadOnlyDictionary<string, object?> Values);

    // Selection-only adapter reuses ordinary addition/reversion permissions. It is never
    // registered as a writer: each member still has exactly one registered setter.
    private sealed class SnapshotSelectionHandler(AssetGroup group,
        IReadOnlyDictionary<string, IPropertyHandler> handlers) : AbstractPropertyHandler<Snapshot>
    {
        public override string PropertyName => group.Name;
        public override Snapshot GetValue(IMajorRecordGetter record) => new(
            group.Members.ToDictionary(key => key, key => handlers[key].GetValue(record)));
        public override void SetValue(IMajorRecord record, Snapshot? value) =>
            throw new InvalidOperationException("Asset snapshots are written through their registered member handlers.");
        public override bool AreValuesEqual(Snapshot? left, Snapshot? right) =>
            left == null || right == null ? left == null && right == null : group.Members.All(key =>
                handlers[key] is ModelBoundsHandler models
                    ? models.AreModelPayloadsEqual((ModelBoundsValue)left.Values[key]!, (ModelBoundsValue)right.Values[key]!)
                    : handlers[key].AreValuesEqual(left.Values[key], right.Values[key]));
        public override string FormatValue(object? value) => value is Snapshot snapshot
            ? string.Join(", ", group.Members.Select(key => $"{key}={handlers[key].FormatValue(snapshot.Values[key])}")) : "null";
    }
}
