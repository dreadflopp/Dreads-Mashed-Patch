using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace DreadsMashedPatch.PropertyHandlers.Npc
{
    public class FactionHandler : AbstractListPropertyHandler<IRankPlacementGetter>
    {
        public override string PropertyName => "Factions";
        public override ListSemantics Semantics => ListSemantics.SortedKeyed;

        protected override IReadOnlyList<object?> GetSortKey(IRankPlacementGetter item) => [item.Faction.FormKey];

        protected override bool IsItemIdentityEqual(IRankPlacementGetter? left, IRankPlacementGetter? right) =>
            IsFactionReferenceEqual(left, right);

        public FactionHandler()
        {
        }

        public override bool AreValuesEqual(List<IRankPlacementGetter>? value1, List<IRankPlacementGetter>? value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null || value1.Count != value2.Count) return false;

            var unmatched = value2.ToList();
            foreach (var item1 in value1)
            {
                var matchIndex = unmatched.FindIndex(item2 =>
                    IsFactionReferenceEqual(item1, item2) && IsRankEqual(item1, item2));
                if (matchIndex < 0) return false;
                unmatched.RemoveAt(matchIndex);
            }

            return true;
        }

        public string FormatFactionList(List<IRankPlacementGetter>? factions)
        {
            if (factions == null || factions.Count == 0)
                return "No factions";

            return string.Join(", ", factions.Select(f => FormatItem(f)));
        }

        protected override bool IsItemEqual(IRankPlacementGetter? item1, IRankPlacementGetter? item2)
        {
            // Only check faction reference equality, not rank
            // This prevents duplicate factions with different ranks
            return IsFactionReferenceEqual(item1, item2);
        }

        protected bool IsFactionReferenceEqual(IRankPlacementGetter? item1, IRankPlacementGetter? item2)
        {
            if (ReferenceEquals(item1, item2)) return true;
            if (item1 is null || item2 is null) return false;
            return item1.Faction.FormKey.Equals(item2.Faction.FormKey);
        }

        protected bool IsRankEqual(IRankPlacementGetter? item1, IRankPlacementGetter? item2)
        {
            if (ReferenceEquals(item1, item2)) return true;
            if (item1 is null || item2 is null) return false;
            return item1.Rank == item2.Rank;
        }

        protected override string FormatItem(IRankPlacementGetter? item)
        {
            if (item == null) return "null";
            return $"{item.Faction.FormKey}(Rank {item.Rank})";
        }

        public override void SetValue(IMajorRecord record, List<IRankPlacementGetter>? value)
        {
            if (record is INpc npc)
            {
                npc.Factions.Clear();
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        var rankPlacement = new RankPlacement
                        {
                            Faction = new FormLink<IFactionGetter>(item.Faction.FormKey),
                            Rank = item.Rank
                        };
                        npc.Factions.Add(rankPlacement);
                    }
                }
            }
        }

        public override List<IRankPlacementGetter>? GetValue(IMajorRecordGetter record)
        {
            if (record is INpcGetter npc)
            {
                return npc.Factions.ToList();
            }
            LogCollector.AddError(PropertyName, $"Error: Record is not an NPC for {PropertyName}");
            return null;
        }

        protected override void ProcessHandlerSpecificLogic(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
            ListPropertyContext<IRankPlacementGetter> listPropertyContext,
            List<IRankPlacementGetter> recordItems,
            List<ListPropertyValueContext<IRankPlacementGetter>> currentForwardItems)
        {

            var recordMod = state.LoadOrder[context.ModKey].Mod;
            if (recordMod == null) return;

            // Collect items to modify to avoid collection modification during enumeration
            var itemsToModify = new List<(ListPropertyValueContext<IRankPlacementGetter> oldItem, ListPropertyValueContext<IRankPlacementGetter> newItem)>();

            foreach (var forwardItem in currentForwardItems.Where(i => !i.IsRemoved))
            {
                var matchingRecordItem = recordItems.FirstOrDefault(r => IsFactionReferenceEqual(r, forwardItem.Value));
                if (matchingRecordItem != null && !IsRankEqual(matchingRecordItem, forwardItem.Value))
                {
                    var canModify = HasPermissionsToModify(recordMod, forwardItem.OwnerMod);
                    if (canModify)
                    {
                        var newItem = new ListPropertyValueContext<IRankPlacementGetter>(matchingRecordItem!, context.ModKey.ToString());
                        
                        itemsToModify.Add((forwardItem, newItem));
                        LogCollector.Add(PropertyName, $"[{PropertyName}] {context.ModKey}: Updating rank for faction {FormatItem(matchingRecordItem)} (new owner: {newItem.OwnerMod}) Success");
                    }
                    else
                    {
                        LogCollector.Add(PropertyName, $"[{PropertyName}] {context.ModKey}: Updating rank for faction {FormatItem(matchingRecordItem)} Permission denied. Owned by {forwardItem.OwnerMod}");
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

