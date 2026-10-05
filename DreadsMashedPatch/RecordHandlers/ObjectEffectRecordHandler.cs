using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins;
using DreadsMashedPatch.PropertyHandlers.ObjectEffect;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Effects migration note: generalized reconciliation to the shared exact-position
    // atomic handler; record access and flag handling stay specialized because xEdit's
    // outer Effects array has no row key and flags require approved flag handlers.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class ObjectEffectRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IObjectEffect, IObjectEffectGetter>("Name") },
            { "ObjectBounds", new ObjectBoundsHandler() },
            { "EnchantmentCost", new EnchantmentCostHandler() },
            { "CastType", new SimpleReflectionPropertyHandler<CastType, IObjectEffect, IObjectEffectGetter>("CastType") },
            { "EnchantmentAmount", new EnchantmentAmountHandler() },
            { "TargetType", new SimpleReflectionPropertyHandler<TargetType, IObjectEffect, IObjectEffectGetter>("TargetType") },
            { "EnchantType", new SimpleReflectionPropertyHandler<ObjectEffect.EnchantTypeEnum, IObjectEffect, IObjectEffectGetter>("EnchantType") },
            { "ChargeTime", new ChargeTimeHandler() },
            { "BaseEnchantment", new SimpleReflectionFormLinkPropertyHandler<IObjectEffectGetter, IObjectEffect, IObjectEffectGetter>("BaseEnchantment") },
            { "WornRestrictions", new SimpleReflectionFormLinkPropertyHandler<IFormListGetter, IObjectEffect, IObjectEffectGetter>("WornRestrictions") },
            { "Effects", new EffectsHandler() },
            { "Flags", new FlagsHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IObjectEffectGetter objectEffectRecord)
            {
                throw new InvalidOperationException($"Expected IObjectEffectGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = objectEffectRecord
                .ToLink<IObjectEffectGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IObjectEffect, IObjectEffectGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }

        // CommitOverride and ApplyForwardedProperties are now handled by the base class
        // The base class automatically handles flag property coordination
    }
}
