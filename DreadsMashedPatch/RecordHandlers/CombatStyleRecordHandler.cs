using System;
using Noggog;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: scalar fields use typed reflection; object aggregates use Mutagen-generated copy/equality.
    // - Kept specialized: none.
    // - Intentionally excluded: CSGDDataTypeState is Mutagen serialization state, not an xEdit field.
    // - Rationale: semantic fields are forwarded while the winning record retains its binary CSGD layout.
    public class CombatStyleRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler() },
            { "SkyrimMajorRecordFlags", new SkyrimMajorRecordFlagsHandler() },
            { "OffensiveMult", new SimpleReflectionPropertyHandler<float, ICombatStyle, ICombatStyleGetter>("OffensiveMult", 0.0001f) },
            { "DefensiveMult", new SimpleReflectionPropertyHandler<float, ICombatStyle, ICombatStyleGetter>("DefensiveMult", 0.0001f) },
            { "GroupOffensiveMult", new SimpleReflectionPropertyHandler<float, ICombatStyle, ICombatStyleGetter>("GroupOffensiveMult", 0.0001f) },
            { "EquipmentScoreMultMelee", new SimpleReflectionPropertyHandler<float, ICombatStyle, ICombatStyleGetter>("EquipmentScoreMultMelee", 0.0001f) },
            { "EquipmentScoreMultMagic", new SimpleReflectionPropertyHandler<float, ICombatStyle, ICombatStyleGetter>("EquipmentScoreMultMagic", 0.0001f) },
            { "EquipmentScoreMultRanged", new SimpleReflectionPropertyHandler<float, ICombatStyle, ICombatStyleGetter>("EquipmentScoreMultRanged", 0.0001f) },
            { "EquipmentScoreMultShout", new SimpleReflectionPropertyHandler<float, ICombatStyle, ICombatStyleGetter>("EquipmentScoreMultShout", 0.0001f) },
            { "EquipmentScoreMultUnarmed", new SimpleReflectionPropertyHandler<float, ICombatStyle, ICombatStyleGetter>("EquipmentScoreMultUnarmed", 0.0001f) },
            { "EquipmentScoreMultStaff", new SimpleReflectionPropertyHandler<float, ICombatStyle, ICombatStyleGetter>("EquipmentScoreMultStaff", 0.0001f) },
            { "AvoidThreatChance", new SimpleReflectionPropertyHandler<float, ICombatStyle, ICombatStyleGetter>("AvoidThreatChance", 0.0001f) },
            { "CSMD", new SimpleReflectionPropertyHandler<ReadOnlyMemorySlice<byte>?, ICombatStyle, ICombatStyleGetter>("CSMD") },
            { "Melee", new GeneratedCopyReflectionPropertyHandler<ICombatStyleMeleeGetter, CombatStyleMelee, ICombatStyle, ICombatStyleGetter>(
                "Melee", value => value.DeepCopy(), CombatStyleMeleeMixIn.Equals) },
            { "CloseRange", new GeneratedCopyReflectionPropertyHandler<ICombatStyleCloseRangeGetter, CombatStyleCloseRange, ICombatStyle, ICombatStyleGetter>(
                "CloseRange", value => value.DeepCopy(), CombatStyleCloseRangeMixIn.Equals) },
            { "LongRangeStrafeMult", new SimpleReflectionPropertyHandler<float?, ICombatStyle, ICombatStyleGetter>("LongRangeStrafeMult", 0.0001f) },
            { "Flight", new GeneratedCopyReflectionPropertyHandler<ICombatStyleFlightGetter, CombatStyleFlight, ICombatStyle, ICombatStyleGetter>(
                "Flight", value => value.DeepCopy(), CombatStyleFlightMixIn.Equals) },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.CombatStyle.Flag, ICombatStyle, ICombatStyleGetter>("Flags") },
            { "MajorFlags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.CombatStyle.MajorFlag, ICombatStyle, ICombatStyleGetter>("MajorFlags") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ICombatStyleGetter combatStyle)
            {
                throw new InvalidOperationException($"Expected ICombatStyleGetter but got {winningContext.Record.GetType()}");
            }

            return combatStyle
                .ToLink<ICombatStyleGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ICombatStyle, ICombatStyleGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
