using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Eyes;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: Name and flags via shared handlers.
    // - Kept specialized: Icon via dedicated texture-path-normalizing handler.
    // - Rationale: icon asset path normalization follows project texture handling policy.

    // Header migration: raw/common/Eyes.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Required null names become empty; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class EyesRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Eyes.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IEyes, IEyesGetter>("Name", required: true) },
            { "Icon", new IconHandler() },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.Eyes.Flag, IEyes, IEyesGetter>("Flags") },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IEyesGetter eyes)
            {
                throw new InvalidOperationException($"Expected IEyesGetter but got {winningContext.Record.GetType()}");
            }

            return eyes
                .ToLink<IEyesGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IEyes, IEyesGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
