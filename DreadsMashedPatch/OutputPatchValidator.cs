using System.Collections;
using Loqui;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace DreadsMashedPatch;

/// <summary>Validates output overrides, including the scoped COED rank import workaround; never changes source records.</summary>
internal sealed class OutputPatchValidator
{
    private readonly IPatcherState<ISkyrimMod, ISkyrimModGetter> _state;
    private readonly Dictionary<FormKey, IMajorRecordGetter> _output;
    private readonly Dictionary<FormKey, IMajorRecordGetter> _input = [];
    private sealed record Failure(string Reason, FormLinkInformation? Link = null, HashSet<FormKey>? Cycle = null);

    private OutputPatchValidator(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        _state = state;
        _output = state.PatchMod.EnumerateMajorRecords().ToDictionary(r => r.FormKey, r => (IMajorRecordGetter)r);
        var required = _output.Values.Where(r => !r.IsDeleted).SelectMany(r => r.EnumerateFormLinks(false))
            .Where(l => !l.IsNull && !IsEnginePlayerReference(l)).Select(l => l.FormKey).ToHashSet();
        var lists = _output.Values.Any(r => !r.IsDeleted && ListType(r) != null);
        if (required.Count == 0 && !lists) return;
        var inputs = state.LoadOrder.PriorityOrder
            .Where(l => l.Enabled && l.Mod != null && l.ModKey != state.PatchMod.ModKey).ToArray();
        // Include every input list key before selecting winners. An incorrectly
        // typed later override must also shadow an older list during traversal.
        if (lists)
            foreach (var listing in inputs)
            {
                required.UnionWith(listing.Mod!.LeveledItems.Select(r => r.FormKey));
                required.UnionWith(listing.Mod.LeveledNpcs.Select(r => r.FormKey));
                required.UnionWith(listing.Mod.LeveledSpells.Select(r => r.FormKey));
            }
        // Resolve only the references emitted by the patch, with all leveled lists
        // available for graph traversal. One priority-order pass avoids creating a
        // separate Mutagen context/type index for every broad link interface.
        foreach (var listing in inputs)
        {
            foreach (var record in listing.Mod!.EnumerateMajorRecords())
                if (required.Contains(record.FormKey))
                    _input.TryAdd(record.FormKey, record);
        }
    }

    public static void Validate(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        Console.WriteLine("Checking output patch for errors...");
        Console.Out.Flush();
        var validator = new OutputPatchValidator(state);
        var count = validator._output.Count;
        validator.Run();
        LogCollector.PrintAllAndClear();
        Console.WriteLine($"Output validation completed: {validator._output.Count} override(s) retained; {count - validator._output.Count} omitted.");
        Console.Out.Flush();
    }

    private void Run()
    {
        while (_output.Count > 0)
        {
            var rejected = new Dictionary<FormKey, Failure>();
            foreach (var record in _output.Values)
            {
                // Deleted records do not use their residual payload at runtime.
                if (record.IsDeleted) continue;
                var failure = ValidateRecord(record);
                if (failure != null) rejected.Add(record.FormKey, failure);
            }
            if (rejected.Count == 0) FindLeveledCycles(rejected);
            if (rejected.Count == 0) return;

            // Removing a containing override also removes its nested output records.
            // Report those omissions too; never publish an orphan or mark a record Deleted.
            foreach (var key in rejected.Keys.ToArray())
                foreach (var child in _output[key].EnumerateMajorRecords())
                    rejected.TryAdd(child.FormKey, new Failure($"containing override {key} was rejected by output validation"));

            var messages = rejected.Select(pair =>
            {
                var record = _output[pair.Key];
                return (Record: record, Reason: Describe(record, pair.Value));
            }).ToArray();
            foreach (var group in messages.GroupBy(m => ((ILoquiObject)m.Record).Registration.GetterType))
                _state.PatchMod.Remove(group.Select(m => m.Record.FormKey).ToHashSet(), group.Key);
            foreach (var (record, reason) in messages)
            {
                _output.Remove(record.FormKey);
                PatchDiagnostics.OmitInvalidOutput(record.FormKey, RecordTypeCatalog.GetRecordDescription(record), reason);
                LogCollector.AddWarning("Output validation",
                    $"Omitted output override {RecordTypeCatalog.GetRecordDescription(record)} {record.FormKey} " +
                    $"({record.EditorID ?? "no EditorID"}): {reason}");
            }
            // A removed override exposes the input winner. Recheck remaining output
            // against that effective result; the input-winner snapshot stays valid.
        }
    }

    private Failure? ValidateRecord(IMajorRecordGetter record)
    {
        var unusedVariables = new Dictionary<FormLinkInformation, int>();
        var normalizedRanks = 0;
        foreach (var data in OwnershipData(record))
        {
            if (data.Owner is not IUntypedOwnerGetter owner) continue;
            var variable = FormLinkInformation.Factory(owner.VariableData);
            if (owner.OwnerData.IsNull)
                unusedVariables[variable] = unusedVariables.GetValueOrDefault(variable) + 1;
            else if (data is IExtraData mutable &&
                     Resolve(new FormLinkInformation(owner.OwnerData.FormKey, typeof(IFactionGetter))) != null)
            {
                // WORKAROUND for Synthesis/Mutagen's untyped COED import. Assume
                // the signed rank with the smallest absolute value compatible
                // with FormKey.ID's remaining 24 bits (positive on the midpoint
                // tie). Values above the midpoint get an FF upper byte, including
                // -1 without a special case; arbitrary 32-bit ranks cannot be recovered.
                // See docs/DEVELOPMENT.md "COED faction-rank import workaround"
                // for the scope and removal conditions. Do not mask light IDs to FFF.
                var id = owner.VariableData.FormKey.ID;
                mutable.Owner = new FactionOwner
                {
                    Faction = new FormLink<IFactionGetter>(owner.OwnerData.FormKey),
                    RequiredRank = id <= 0x800000 ? (int)id : (int)id - 0x1000000
                };
                normalizedRanks++;
            }
        }
        if (normalizedRanks > 0)
            LogCollector.AddInfo("Output validation",
                $"Normalized {normalizedRanks} untyped COED faction rank(s) in {record.FormKey}.");
        // Skyrim LLCT is uint8 in xEdit wbDefinitionsTES5.pas. Mutagen 0.54.4
        // Leveled{Item,Npc,Spell}BinaryWriteTranslation writes a one-byte counter.
        var count = record switch
        {
            ILeveledItemGetter item => item.Entries?.Count ?? 0,
            ILeveledNpcGetter npc => npc.Entries?.Count ?? 0,
            ILeveledSpellGetter spell => spell.Entries?.Count ?? 0,
            _ => 0
        };
        if (count > byte.MaxValue)
            return new Failure($"Entries contains {count} entries; the LLCT counter permits at most {byte.MaxValue}. The list was not truncated");

        foreach (var link in record.EnumerateFormLinks(iterateNestedRecords: false).Where(l => !l.IsNull))
        {
            var info = FormLinkInformation.Factory(link);
            if (unusedVariables.TryGetValue(info, out var unused) && unused > 0)
            {
                unusedVariables[info] = unused - 1;
                continue;
            }
            // PlayerRef is supplied by the engine, not an ordinary Skyrim.esm
            // record. xEdit's checked FormID logic accepts PLYR before lookup.
            // It is an actor reference (ACHR), never an NPC_ base or a package.
            if (IsEnginePlayerReference(info)) continue;
            var target = Resolve(info);
            if (target == null)
                return new Failure($"reference {link.FormKey} does not resolve to the permitted target type {link.Type.Name}", info);
            // Do not apply a blanket deleted-target rule to historical/metadata links.
            // Packages and leveled entries specifically require usable runtime targets.
            if (target.IsDeleted && RequiresLiveTarget(record, info))
                return new Failure($"reference {link.FormKey} targets a deleted {link.Type.Name} record", info);
        }
        return null;
    }

    private static bool RequiresLiveTarget(IMajorRecordGetter record, FormLinkInformation link) => record switch
    {
        INpcGetter npc => npc.Packages.Any(p => p.FormKey == link.FormKey && link.Type == typeof(IPackageGetter)),
        ILeveledItemGetter or ILeveledNpcGetter or ILeveledSpellGetter =>
            ListLinks(record).Any(l => l.FormKey == link.FormKey && l.Type == link.Type),
        _ => false
    };

    private IMajorRecordGetter? Resolve(FormLinkInformation link)
    {
        if (_output.TryGetValue(link.FormKey, out var patched))
            return link.Type.IsInstanceOfType(patched) ? patched : null;
        return _input.TryGetValue(link.FormKey, out var input) && link.Type.IsInstanceOfType(input) ? input : null;
    }

    private static bool IsEnginePlayerReference(IFormLinkIdentifier link) =>
        link.FormKey == Constants.Player.FormKey && link.Type.IsAssignableFrom(typeof(IPlacedNpcGetter));

    // xEdit COED chooses Global for NPC ownership, signed rank for faction
    // ownership, and unused bytes for no owner. Generated UntypedOwner links
    // alone cannot express that distinction or retain every raw rank bit.
    private static IEnumerable<IExtraDataGetter> OwnershipData(IMajorRecordGetter record) => record switch
    {
        INpcGetter npc => npc.Items?.Select(i => i.Data).OfType<IExtraDataGetter>() ?? [],
        IContainerGetter container => container.Items?.Select(i => i.Data).OfType<IExtraDataGetter>() ?? [],
        ILeveledItemGetter item => item.Entries?.Select(i => i.ExtraData).OfType<IExtraDataGetter>() ?? [],
        ILeveledNpcGetter npc => npc.Entries?.Select(i => i.ExtraData).OfType<IExtraDataGetter>() ?? [],
        _ => []
    };

    private static IEnumerable<FormLinkInformation> ListLinks(IMajorRecordGetter record) => record switch
    {
        ILeveledItemGetter item => item.Entries?.Where(e => e.Data != null).Select(e => FormLinkInformation.Factory(e.Data!.Reference)) ?? [],
        ILeveledNpcGetter npc => npc.Entries?.Where(e => e.Data != null).Select(e => FormLinkInformation.Factory(e.Data!.Reference)) ?? [],
        ILeveledSpellGetter spell => spell.Entries?.Where(e => e.Data != null).Select(e => FormLinkInformation.Factory(e.Data!.Reference)) ?? [],
        _ => []
    };

    private static Type? ListType(IMajorRecordGetter record) => record switch
    {
        ILeveledItemGetter => typeof(ILeveledItemGetter),
        ILeveledNpcGetter => typeof(ILeveledNpcGetter),
        ILeveledSpellGetter => typeof(ILeveledSpellGetter),
        _ => null
    };

    private void FindLeveledCycles(Dictionary<FormKey, Failure> rejected)
    {
        var graph = new Dictionary<FormKey, FormKey[]>();
        var pending = new Stack<IMajorRecordGetter>(_output.Values.Where(r => !r.IsDeleted && ListType(r) != null));
        while (pending.TryPop(out var record))
        {
            if (graph.ContainsKey(record.FormKey)) continue;
            var type = ListType(record)!;
            var children = ListLinks(record).Where(l => !l.IsNull)
                .Select(Resolve).Where(r => r != null && !r.IsDeleted && type.IsInstanceOfType(r))
                .Cast<IMajorRecordGetter>().DistinctBy(r => r.FormKey).ToArray();
            graph.Add(record.FormKey, children.Select(r => r.FormKey).ToArray());
            foreach (var child in children) pending.Push(child);
        }

        // Iterative strongly-connected components avoid recursion limits on long lists.
        var visited = new HashSet<FormKey>();
        var finished = new List<FormKey>();
        foreach (var root in graph.Keys)
        {
            var stack = new Stack<(FormKey Key, bool Finish)>();
            stack.Push((root, false));
            while (stack.TryPop(out var frame))
            {
                if (frame.Finish) { finished.Add(frame.Key); continue; }
                if (!visited.Add(frame.Key)) continue;
                stack.Push((frame.Key, true));
                foreach (var child in graph[frame.Key]) stack.Push((child, false));
            }
        }
        var reverse = graph.Keys.ToDictionary(k => k, _ => new List<FormKey>());
        foreach (var (parent, children) in graph)
            foreach (var child in children) reverse[child].Add(parent);
        visited.Clear();
        foreach (var root in finished.AsEnumerable().Reverse())
        {
            var component = new HashSet<FormKey>();
            var stack = new Stack<FormKey>();
            stack.Push(root);
            while (stack.TryPop(out var key))
            {
                if (!visited.Add(key)) continue;
                component.Add(key);
                foreach (var parent in reverse[key]) stack.Push(parent);
            }
            if (component.Count < 2 && !graph[root].Contains(root)) continue;
            foreach (var key in component.Where(_output.ContainsKey))
                rejected.TryAdd(key, new Failure(
                    $"Entries participates in a circular leveled-list component: {string.Join(", ", component.OrderBy(k => k.ToString(), StringComparer.Ordinal))}", Cycle: component));
        }
    }

    private string Describe(IMajorRecordGetter record, Failure failure)
    {
        var getterType = ((ILoquiObject)record).Registration.GetterType;
        var history = _state.LinkCache.ResolveAllSimpleContexts(record.FormKey, getterType)
            .Where(c => c.ModKey != _state.PatchMod.ModKey).ToArray();
        var candidates = history.Where(c => failure.Link != null
            ? c.Record.EnumerateFormLinks(false).Any(l => l.FormKey == failure.Link.FormKey && l.Type == failure.Link.Type)
            : failure.Cycle != null ? ListLinks(c.Record).Any(l => failure.Cycle.Contains(l.FormKey)) : true)
            .Select(c => c.ModKey).Distinct().ToArray();
        if (failure.Cycle != null)
        {
            var listType = ListType(record)!;
            candidates = failure.Cycle.SelectMany(key => _state.LinkCache.ResolveAllSimpleContexts(key, listType))
                .Where(c => c.ModKey != _state.PatchMod.ModKey && ListLinks(c.Record).Any(l => failure.Cycle.Contains(l.FormKey)))
                .Select(c => c.ModKey).Distinct().ToArray();
        }
        var sources = candidates.Length > 0 ? candidates : history.Select(c => c.ModKey).Distinct().ToArray();
        string? path = null;
        if (failure.Link != null)
        {
            try { path = FindLinkPath(record, failure.Link, "", new(ReferenceEqualityComparer.Instance)); }
            catch (Exception) { /* A diagnostic path must not prevent rejection of a known-invalid override. */ }
        }
        var sourceText = sources.Length == 0 ? "the input plugins for this record"
            : string.Join(", ", sources.Select(k => k.FileName.String));
        return $"{(path == null ? "" : path + ": ")}{failure.Reason}. " +
            $"Check {sourceText} in xEdit. " +
            (failure.Cycle == null ? "" : "The cycle may result from combining otherwise valid source records. ") +
            "Repair or replace the record, or remove the plugin.";
    }

    // Paths are diagnostic-only and discovered only for rejected links. Validation
    // itself uses Mutagen's generated link enumeration, not reflection over fields.
    private static string? FindLinkPath(object? value, FormLinkInformation link, string path, HashSet<object> visited)
    {
        if (value is IFormLinkGetter formLink)
            return formLink.FormKey == link.FormKey && formLink.Type == link.Type ? path : null;
        if (value == null || value is string || value.GetType().IsValueType || !visited.Add(value)) return null;
        if (path.Length > 0 && value is IMajorRecordGetter) return null;
        if (value is IEnumerable items)
        {
            var index = 0;
            foreach (var item in items)
            {
                var found = FindLinkPath(item, link, $"{path}[{index++}]", visited);
                if (found != null) return found;
            }
        }
        else if (value is ILoquiObject loqui)
        {
            foreach (var property in loqui.Registration.GetterType.GetInterfaces().Append(loqui.Registration.GetterType)
                .SelectMany(t => t.GetProperties()).Where(p => p.GetIndexParameters().Length == 0).DistinctBy(p => p.Name))
            {
                var child = property.GetValue(value);
                if (child is not IFormLinkGetter && child is not IFormLinkContainerGetter && child is not IEnumerable) continue;
                var found = FindLinkPath(child, link, path.Length == 0 ? property.Name : path + "." + property.Name, visited);
                if (found != null) return found;
            }
        }
        return null;
    }
}
