using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.PropertyHandlers.General;
using Noggog;

namespace DreadsMashedPatch.PropertyHandlers.Abstracts
{
    // CONT and NPC_ share complete inventory snapshots and permission-controlled
    // COED reconciliation. Record adapters expose Items and any count-policy exception.
    public abstract class AbstractInventoryItemsHandler<TRecordGetter, TRecord> : AbstractListPropertyHandler<ContainerEntry>
        where TRecordGetter : class, IMajorRecordGetter
        where TRecord : class, IMajorRecord
    {
        public override string PropertyName => "Items";
        public override ListSemantics Semantics => ListSemantics.SortedKeyed;

        protected override IReadOnlyList<object?> GetSortKey(ContainerEntry item) => [item.Item.Item.FormKey];

        protected override ContainerEntry CopyItemForForwardContext(ContainerEntry item) => item.DeepCopy();

        protected abstract IReadOnlyList<IContainerEntryGetter>? GetItems(TRecordGetter record);
        protected abstract void SetItems(TRecord record, ExtendedList<ContainerEntry>? items);

        protected virtual bool CanUpdateCount(
            ISkyrimModGetter recordMod,
            ListPropertyContext<ContainerEntry> listPropertyContext,
            ListPropertyValueContext<ContainerEntry> forwardItem,
            ContainerEntry recordItem) => HasPermissionsToModify(recordMod, forwardItem.OwnerMod);

        public override void SetValue(IMajorRecord record, List<ContainerEntry>? value)
        {
            if (record is TRecord typedRecord)
            {
                SetItems(typedRecord, value == null
                    ? null
                    : new ExtendedList<ContainerEntry>(value.Select(item => item.DeepCopy())));
                return;
            }

            LogCollector.Add(PropertyName, $"Error: Record does not implement {typeof(TRecord).Name} for {PropertyName}");
        }

        public override List<ContainerEntry>? GetValue(IMajorRecordGetter record)
        {
            if (record is TRecordGetter typedRecord)
            {
                return GetItems(typedRecord)?.Select(item => item.DeepCopy()).ToList();
            }

            LogCollector.Add(PropertyName, $"Error: Record does not implement {typeof(TRecordGetter).Name} for {PropertyName}");
            return null;
        }

        // Final equality includes metadata. Key-only equality below keeps the shared
        // row replacement step from bypassing inventory reconciliation permissions.
        public override bool AreValuesEqual(List<ContainerEntry>? value1, List<ContainerEntry>? value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null || value1.Count != value2.Count) return false;

            // Inventory order is not semantic, but duplicate FormKeys are valid. Match
            // each complete entry once instead of grouping by FormKey and losing duplicates.
            var unmatched = value2.ToList();
            foreach (var item1 in value1)
            {
                var matchIndex = unmatched.FindIndex(item2 => AreItemContentsEqual(item1, item2));
                if (matchIndex < 0) return false;
                unmatched.RemoveAt(matchIndex);
            }

            return true;
        }

        private static bool AreItemContentsEqual(ContainerEntry? item1, ContainerEntry? item2)
        {
            if (item1 == null && item2 == null) return true;
            if (item1 == null || item2 == null) return false;
            if (item1.Item.Item.FormKey != item2.Item.Item.FormKey) return false;
            if (item1.Item.Count != item2.Item.Count) return false;

            var data1 = item1.Data;
            var data2 = item2.Data;
            if (data1 == null || data2 == null)
            {
                return data1 == null && data2 == null;
            }

            return Math.Abs(data1.ItemCondition - data2.ItemCondition) <= 0.001f
                   && OwnerTargetUtility.AreEqual(data1.Owner, data2.Owner);
        }

        protected override bool IsItemEqual(ContainerEntry? item1, ContainerEntry? item2)
        {
            if (item1 == null && item2 == null) return true;
            if (item1 == null || item2 == null) return false;

            // Membership and shared replacements compare only FormKey; count and COED
            // changes are handled once by ProcessHandlerSpecificLogic.
            return item1.Item.Item.FormKey == item2.Item.Item.FormKey;
        }

        protected override string FormatItem(ContainerEntry? item)
        {
            if (item == null) return "null";
            var result = $"{item.Item.Item.FormKey} (Count: {item.Item.Count}";
            if (item.Data != null)
            {
                result += $", Condition: {item.Data.ItemCondition}";
                if (item.Data.Owner != null)
                {
                    result += $", Owner: {FormatOwner(item.Data.Owner)}";
                }
            }
            result += ")";
            return result;
        }

        protected override void ProcessHandlerSpecificLogic(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
            ListPropertyContext<ContainerEntry> listPropertyContext,
            List<ContainerEntry> recordItems,
            List<ListPropertyValueContext<ContainerEntry>> currentForwardItems)
        {
            var recordMod = state.LoadOrder[context.ModKey].Mod;
            if (recordMod == null) return;

            // Group items by FormKey for occurrence-aware matching
            var forwardItemsByFormKey = currentForwardItems
                .Where(i => !i.IsRemoved)
                .GroupBy(i => i.Value.Item.Item.FormKey)
                .ToDictionary(g => g.Key, g => g.ToList());

            var recordItemsByFormKey = recordItems
                .Select((item, index) => (Item: item, Index: index))
                .GroupBy(x => x.Item.Item.Item.FormKey)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Process each FormKey group
            foreach (var formKeyGroup in forwardItemsByFormKey)
            {
                var formKey = formKeyGroup.Key;
                var forwardItems = formKeyGroup.Value;

                if (!recordItemsByFormKey.TryGetValue(formKey, out var recordItemsForFormKey))
                {
                    continue;
                }

                // Match each source occurrence at most once, preferring unchanged rows.
                var matches = FindMatches(forwardItems, recordItemsForFormKey);

                // Apply the matched metadata
                foreach (var match in matches)
                {
                    var forwardItem = match.ForwardItem;
                    var recordItem = match.RecordItem;

                    // The shared additions path can seed duplicate identities from
                    // the same group exemplar. Detach before mutating metadata so
                    // each occurrence retains its own complete payload.
                    forwardItem.Value = forwardItem.Value.DeepCopy();

                    bool hasChanges = false;
                    var changes = new List<string>();

                    // Update count if it's different and we have permissions
                    if (recordItem.Item.Count != forwardItem.Value.Item.Count)
                    {
                        if (CanUpdateCount(recordMod, listPropertyContext, forwardItem, recordItem))
                        {
                            var oldCount = forwardItem.Value.Item.Count;
                            forwardItem.Value.Item.Count = recordItem.Item.Count;
                            changes.Add($"count {oldCount} -> {recordItem.Item.Count}");
                            hasChanges = true;
                        }
                        else
                        {
                            LogCollector.Add(PropertyName, $"[{PropertyName}] {context.ModKey}: Cannot update count for {forwardItem.Value.Item.Item.FormKey} - no permission (owned by {forwardItem.OwnerMod})");
                        }
                    }

                    hasChanges |= ReconcileExtraData(
                        context.ModKey.ToString(),
                        recordMod,
                        forwardItem,
                        recordItem.Data,
                        changes);

                    // Log changes if any were made
                    if (hasChanges)
                    {
                        var oldOwner = forwardItem.OwnerMod;
                        forwardItem.OwnerMod = context.ModKey.ToString();
                        LogCollector.Add(PropertyName, $"[{PropertyName}] {context.ModKey}: Updated {string.Join(", ", changes)} for {forwardItem.Value.Item.Item.FormKey} (was owned by {oldOwner}) Success");
                    }
                }
            }
        }

        internal bool ReconcileExtraData(
            string modName,
            ISkyrimModGetter recordMod,
            ListPropertyValueContext<ContainerEntry> forwardItem,
            IExtraDataGetter? recordData,
            List<string> changes)
        {
            var forwardData = forwardItem.Value.Data;

            // COED presence is meaningful. Do not collapse an absent group into
            // a present group containing Mutagen's default UntypedOwner and zero
            // condition values.
            if (recordData == null || forwardData == null)
            {
                if (recordData == null && forwardData == null)
                {
                    return false;
                }

                if (!HasPermissionsToModify(recordMod, forwardItem.OwnerMod))
                {
                    LogCollector.Add(PropertyName, $"[{PropertyName}] {modName}: Cannot update extra data for {forwardItem.Value.Item.Item.FormKey} - no permission (owned by {forwardItem.OwnerMod})");
                    return false;
                }

                forwardItem.Value.Data = recordData == null
                    ? null
                    : recordData.DeepCopy();
                changes.Add(recordData == null
                    ? "extra data present -> null"
                    : "extra data null -> present");
                return true;
            }

            var hasChanges = false;

            if (Math.Abs(recordData.ItemCondition - forwardData.ItemCondition) > 0.001f)
            {
                if (HasPermissionsToModify(recordMod, forwardItem.OwnerMod))
                {
                    var oldCondition = forwardData.ItemCondition;
                    forwardData.ItemCondition = recordData.ItemCondition;
                    changes.Add($"condition {oldCondition} -> {recordData.ItemCondition}");
                    hasChanges = true;
                }
                else
                {
                    LogCollector.Add(PropertyName, $"[{PropertyName}] {modName}: Cannot update condition for {forwardItem.Value.Item.Item.FormKey} - no permission (owned by {forwardItem.OwnerMod})");
                }
            }

            if (!OwnerTargetUtility.AreEqual(recordData.Owner, forwardData.Owner))
            {
                if (HasPermissionsToModify(recordMod, forwardItem.OwnerMod))
                {
                    var oldOwner = forwardData.Owner;
                    forwardData.Owner = OwnerTargetUtility.DeepCopy(recordData.Owner);
                    changes.Add($"owner {FormatOwner(oldOwner)} -> {FormatOwner(recordData.Owner)}");
                    hasChanges = true;
                }
                else
                {
                    LogCollector.Add(PropertyName, $"[{PropertyName}] {modName}: Cannot update owner for {forwardItem.Value.Item.Item.FormKey} - no permission (owned by {forwardItem.OwnerMod})");
                }
            }

            return hasChanges;
        }

        private List<(ListPropertyValueContext<ContainerEntry> ForwardItem, ContainerEntry RecordItem)> FindMatches(
            List<ListPropertyValueContext<ContainerEntry>> forwardItems,
            List<(ContainerEntry Item, int Index)> recordItemsForFormKey)
        {
            var matches = new List<(ListPropertyValueContext<ContainerEntry>, ContainerEntry)>();
            var usedRecordIndices = new HashSet<int>();

            // Reserve exact matches first: a changed duplicate must not consume
            // the source row belonging to an unchanged occurrence.
            var remainingForwardItems = new List<ListPropertyValueContext<ContainerEntry>>();
            foreach (var forwardItem in forwardItems)
            {
                var exactIndex = recordItemsForFormKey.FindIndex(candidate =>
                    !usedRecordIndices.Contains(candidate.Index)
                    && AreItemContentsEqual(forwardItem.Value, candidate.Item));
                if (exactIndex < 0)
                {
                    remainingForwardItems.Add(forwardItem);
                    continue;
                }

                var exact = recordItemsForFormKey[exactIndex];
                matches.Add((forwardItem, exact.Item));
                usedRecordIndices.Add(exact.Index);
            }

            // For each remaining occurrence, choose the available source row
            // requiring the fewest metadata changes (stable source-order ties).
            foreach (var forwardItem in remainingForwardItems)
            {
                var bestMatch = recordItemsForFormKey
                    .Where(x => !usedRecordIndices.Contains(x.Index))
                    .Select(x => new
                    {
                        RecordItem = x.Item,
                        Index = x.Index,
                        Cost = CalculateChangeCost(forwardItem.Value, x.Item)
                    })
                    .OrderBy(x => x.Cost)
                    .FirstOrDefault();

                if (bestMatch != null)
                {
                    matches.Add((forwardItem, bestMatch.RecordItem));
                    usedRecordIndices.Add(bestMatch.Index);
                }
            }

            return matches;
        }

        private static int CalculateChangeCost(ContainerEntry forwardItem, ContainerEntry recordItem)
        {
            int cost = 0;

            // Count changes needed
            if (forwardItem.Item.Count != recordItem.Item.Count) cost += 1;

            if (forwardItem.Data == null || recordItem.Data == null)
            {
                if (forwardItem.Data != null || recordItem.Data != null) cost += 1;
                return cost;
            }

            if (Math.Abs(forwardItem.Data.ItemCondition - recordItem.Data.ItemCondition) > 0.001f) cost += 1;

            if (!OwnerTargetUtility.AreEqual(forwardItem.Data.Owner, recordItem.Data.Owner)) cost += 1;

            return cost;
        }

        private static string FormatOwner(IOwnerTargetGetter? owner)
        {
            if (owner == null) return "null";

            switch (owner)
            {
                case IUntypedOwnerGetter untypedOwner:
                    return $"UntypedOwner(OwnerData:{untypedOwner.OwnerData.FormKey}, VariableData:{untypedOwner.VariableData.FormKey})";

                case IFactionOwnerGetter factionOwner:
                    return $"FactionOwner(Faction:{factionOwner.Faction.FormKey}, Rank:{factionOwner.RequiredRank})";

                case INpcOwnerGetter npcOwner:
                    return $"NpcOwner(NPC:{npcOwner.Npc.FormKey}, Global:{npcOwner.Global.FormKey})";

                default:
                    return $"{owner.GetType().Name}({owner.GetHashCode():X8})";
            }
        }

    }
}
