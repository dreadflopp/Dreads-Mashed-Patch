using System;
using System.Drawing;
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
    // - Generalized: Color and Playable via reflection handlers.
    // - Generalized: Name via generated translated-string copying.
    // - Rationale: very small record surface and fully reflection-safe fields.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class ColorRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IColorRecord, IColorRecordGetter>("Name") },
            { "Color", new SimpleReflectionPropertyHandler<Color, IColorRecord, IColorRecordGetter>("Color") },
            { "Playable", new SimpleReflectionPropertyHandler<bool, IColorRecord, IColorRecordGetter>("Playable") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IColorRecordGetter colorRecord)
            {
                throw new InvalidOperationException($"Expected IColorRecordGetter but got {winningContext.Record.GetType()}");
            }

            return colorRecord
                .ToLink<IColorRecordGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IColorRecord, IColorRecordGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
