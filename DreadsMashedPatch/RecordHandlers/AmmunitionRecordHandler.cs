using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Ammunition;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: AMMO scalar/form-link fields use typed reflection; translated text uses generated copying.
    // - Kept specialized: ObjectBounds/Model/Icons/Destructible/Keywords/Value/Weight and Skyrim flag handlers.
    // - Intentionally excluded: DATADataTypeState is Mutagen serialization state, not an xEdit field.
    // - Rationale: semantic fields are forwarded while the winning record retains its binary DATA layout.

    // Header migration: raw/common/Ammunition.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class AmmunitionRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Ammunition.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IAmmunition, IAmmunitionGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Icons", new IconsHandler() },
            { "Destructible", new DestructibleHandler() },
            { "PickUpSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IAmmunition, IAmmunitionGetter>("PickUpSound") },
            { "PutDownSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IAmmunition, IAmmunitionGetter>("PutDownSound") },
            { "Description", new TranslatedStringReflectionPropertyHandler<IAmmunition, IAmmunitionGetter>("Description") },
            { "Keywords", new KeywordListHandler() },
            { "Projectile", new SimpleReflectionFormLinkPropertyHandler<IProjectileGetter, IAmmunition, IAmmunitionGetter>("Projectile") },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.Ammunition.Flag, IAmmunition, IAmmunitionGetter>("Flags") },
            { "Damage", new SimpleReflectionPropertyHandler<float, IAmmunition, IAmmunitionGetter>("Damage") },
            { "Value", new ValueHandler() },
            { "Weight", new WeightHandler() },
            { "ShortName", new SimpleReflectionPropertyHandler<string, IAmmunition, IAmmunitionGetter>("ShortName") },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IAmmunitionGetter ammunitionRecord)
            {
                throw new InvalidOperationException($"Expected IAmmunitionGetter but got {winningContext.Record.GetType()}");
            }

            return ammunitionRecord
                .ToLink<IAmmunitionGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
