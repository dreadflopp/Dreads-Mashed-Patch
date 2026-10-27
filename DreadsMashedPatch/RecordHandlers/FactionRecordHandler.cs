using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Faction;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    public class FactionRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IFaction, IFactionGetter>("Name") },
            { "Relations", new GeneratedCopyReflectionListPropertyHandler<IRelationGetter, Relation, IFaction, IFactionGetter>(
                "Relations", ListSemantics.SortedKeyed, value => value.DeepCopy(), RelationMixIn.Equals,
                keySelector: relation => relation.Target.FormKey) },
            { "Ranks", new GeneratedCopyReflectionListPropertyHandler<IRankGetter, Rank, IFaction, IFactionGetter>(
                "Ranks", ListSemantics.SortedKeyed, value => value.DeepCopy(), RankMixIn.Equals,
                keySelector: rank => rank.Number) },
            { "Conditions", new ConditionsHandler() },
            { "Flags", new FlagsHandler() },
            { "ExteriorJailMarker", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IFaction, IFactionGetter>("ExteriorJailMarker") },
            { "FollowerWaitMarker", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IFaction, IFactionGetter>("FollowerWaitMarker") },
            { "StolenGoodsContainer", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IFaction, IFactionGetter>("StolenGoodsContainer") },
            { "PlayerInventoryContainer", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IFaction, IFactionGetter>("PlayerInventoryContainer") },
            { "SharedCrimeFactionList", new SimpleReflectionFormLinkPropertyHandler<IFormListGetter, IFaction, IFactionGetter>("SharedCrimeFactionList") },
            { "JailOutfit", new SimpleReflectionFormLinkPropertyHandler<IOutfitGetter, IFaction, IFactionGetter>("JailOutfit") },
            { "CrimeValues", new GeneratedCopyReflectionPropertyHandler<ICrimeValuesGetter, CrimeValues, IFaction, IFactionGetter>(
                "CrimeValues", value => value.DeepCopy(), CrimeValuesMixIn.Equals) },
            { "VendorBuySellList", new SimpleReflectionFormLinkPropertyHandler<IFormListGetter, IFaction, IFactionGetter>("VendorBuySellList") },
            { "MerchantContainer", new SimpleReflectionFormLinkPropertyHandler<IPlacedObjectGetter, IFaction, IFactionGetter>("MerchantContainer") },
            { "VendorValues", new GeneratedCopyReflectionPropertyHandler<IVendorValuesGetter, VendorValues, IFaction, IFactionGetter>(
                "VendorValues", value => value.DeepCopy(), VendorValuesMixIn.Equals) },
            { "VendorLocation", new GeneratedCopyReflectionPropertyHandler<ILocationTargetRadiusGetter, LocationTargetRadius, IFaction, IFactionGetter>(
                "VendorLocation", value => value.DeepCopy(), LocationTargetRadiusMixIn.Equals) }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IFactionGetter factionRecord)
            {
                throw new InvalidOperationException($"Expected IFactionGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = factionRecord
                .ToLink<IFactionGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IFaction, IFactionGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
