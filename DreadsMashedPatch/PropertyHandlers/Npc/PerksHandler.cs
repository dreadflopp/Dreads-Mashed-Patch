using System;
using System.Collections.Generic;
using System.Linq;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Synthesis;
using Noggog;
using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.PropertyHandlers.Npc
{
    public class PerksHandler : AbstractListPropertyHandler<IPerkPlacementGetter>
    {
        public override string PropertyName => "Perks";
        public override ListSemantics Semantics => ListSemantics.SortedKeyed;

        protected override IReadOnlyList<object?> GetSortKey(IPerkPlacementGetter item) => [item.Perk.FormKey];

        protected override bool IsItemIdentityEqual(IPerkPlacementGetter? left, IPerkPlacementGetter? right) =>
            IsPerkReferenceEqual(left, right);

        public override bool AreValuesEqual(List<IPerkPlacementGetter>? value1, List<IPerkPlacementGetter>? value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null || value1.Count != value2.Count) return false;

            var unmatched = value2.ToList();
            foreach (var item1 in value1)
            {
                var matchIndex = unmatched.FindIndex(item2 =>
                    IsPerkReferenceEqual(item1, item2) && IsRankEqual(item1, item2));
                if (matchIndex < 0) return false;
                unmatched.RemoveAt(matchIndex);
            }

            return true;
        }

        public string FormatPerkList(List<IPerkPlacementGetter>? perks)
        {
            if (perks == null || perks.Count == 0)
                return "No perks";

            return string.Join(", ", perks.Select(p => FormatItem(p)));
        }

        protected override bool IsItemEqual(IPerkPlacementGetter? item1, IPerkPlacementGetter? item2)
        {
            // Only check perk reference equality, not rank
            // This prevents duplicate perks with different ranks
            return IsPerkReferenceEqual(item1, item2);
        }

        protected bool IsPerkReferenceEqual(IPerkPlacementGetter? item1, IPerkPlacementGetter? item2)
        {
            if (ReferenceEquals(item1, item2)) return true;
            if (item1 is null || item2 is null) return false;
            return item1.Perk.FormKey.Equals(item2.Perk.FormKey);
        }

        protected bool IsRankEqual(IPerkPlacementGetter? item1, IPerkPlacementGetter? item2)
        {
            if (ReferenceEquals(item1, item2)) return true;
            if (item1 is null || item2 is null) return false;
            return item1.Rank == item2.Rank;
        }

        protected override string FormatItem(IPerkPlacementGetter? item)
        {
            if (item == null) return "null";
            return $"{item.Perk.FormKey}(Rank {item.Rank})";
        }

        public override void SetValue(IMajorRecord record, List<IPerkPlacementGetter>? value)
        {
            if (record is INpc npc)
            {
                // Match the nullable keyword-list setter: preserve presence and copy
                // complete rows before replacing the destination, including absent lists.
                npc.Perks = value == null ? null
                    : new ExtendedList<PerkPlacement>(value.Select(item => item.DeepCopy()));
            }
            else
            {
                LogCollector.AddError(PropertyName, $"Error: Record is not an NPC for {PropertyName}");
            }
        }

        public override List<IPerkPlacementGetter>? GetValue(IMajorRecordGetter record)
        {
            if (record is INpcGetter npc)
            {
                return npc.Perks?.ToList();
            }
            LogCollector.AddError(PropertyName, $"Error: Record is not an NPC for {PropertyName}");
            return null;
        }

        protected override void ProcessHandlerSpecificLogic(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
            ListPropertyContext<IPerkPlacementGetter> listPropertyContext,
            List<IPerkPlacementGetter> recordItems,
            List<ListPropertyValueContext<IPerkPlacementGetter>> currentForwardItems)
        {

            var recordMod = state.LoadOrder[context.ModKey].Mod;
            if (recordMod == null) return;

            // Collect items to modify to avoid collection modification during enumeration
            var itemsToModify = new List<(ListPropertyValueContext<IPerkPlacementGetter> oldItem, ListPropertyValueContext<IPerkPlacementGetter> newItem)>();

            foreach (var forwardItem in currentForwardItems.Where(i => !i.IsRemoved))
            {
                var matchingRecordItem = recordItems.FirstOrDefault(r => IsPerkReferenceEqual(r, forwardItem.Value));
                if (matchingRecordItem != null && !IsRankEqual(matchingRecordItem, forwardItem.Value))
                {
                    var canModify = HasPermissionsToModify(recordMod, forwardItem.OwnerMod);
                    if (canModify)
                    {
                        var newItem = new ListPropertyValueContext<IPerkPlacementGetter>(matchingRecordItem!, context.ModKey.ToString());

                        itemsToModify.Add((forwardItem, newItem));
                        LogCollector.Add(PropertyName, $"[{PropertyName}] {context.ModKey}: Updating rank for perk {FormatItem(matchingRecordItem)} (new owner: {newItem.OwnerMod}) Success");
                    }
                    else
                    {
                        LogCollector.Add(PropertyName, $"[{PropertyName}] {context.ModKey}: Updating rank for perk {FormatItem(matchingRecordItem)} Permission denied. Owned by {forwardItem.OwnerMod}");
                    }
                }
            }

            foreach (var (oldItem, newItem) in itemsToModify)
            {
                var index = currentForwardItems.IndexOf(oldItem);
                if (index != -1)
                {
                    currentForwardItems[index] = newItem;
                }
            }

        }
    }
}
