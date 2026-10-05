using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Key;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: KEYM-specific VM and major flags via reflection handlers.
    // - Kept specialized: shared item handlers for bounds/model/icons/destructible/sounds/keywords/value/weight.
    // - Rationale: mirrors existing MiscItem pattern while honoring KEYM interface surface.

    // Header migration: raw/common/Key.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Required null names become empty; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class KeyRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Key.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IKey, IKeyGetter>("Name", required: true) },
            // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IKey, IKeyGetter>() },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Icons", new IconsHandler() },
            { "Destructible", new DestructibleHandler() },
            { "PickUpSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IKey, IKeyGetter>("PickUpSound") },
            { "PutDownSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IKey, IKeyGetter>("PutDownSound") },
            { "Keywords", new KeywordListHandler() },
            { "Value", new ValueHandler() },
            { "Weight", new WeightHandler() },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IKeyGetter keyRecord)
            {
                throw new InvalidOperationException($"Expected IKeyGetter but got {winningContext.Record.GetType()}");
            }

            return keyRecord
                .ToLink<IKeyGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IKey, IKeyGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
