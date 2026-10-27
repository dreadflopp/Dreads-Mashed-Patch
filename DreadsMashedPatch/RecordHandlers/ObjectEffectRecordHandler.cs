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
    }
}
