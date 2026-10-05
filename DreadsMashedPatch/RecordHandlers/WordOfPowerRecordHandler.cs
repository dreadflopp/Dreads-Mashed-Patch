using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Migration note:
// - Generalized: WOOP translated text fields use generated localized-string copying.
// - Kept specialized: none.
// - Rationale: IWordOfPower only exposes Name/Translation translated strings plus shared major-record fields.

// Header migration: raw/common flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
public class WordOfPowerRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Name", new TranslatedStringReflectionPropertyHandler<IWordOfPower, IWordOfPowerGetter>("Name") },
        { "Translation", new TranslatedStringReflectionPropertyHandler<IWordOfPower, IWordOfPowerGetter>("Translation") }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IWordOfPowerGetter wordOfPowerRecord)
        {
            throw new InvalidOperationException($"Expected IWordOfPowerGetter but got {winningContext.Record.GetType()}");
        }

        return wordOfPowerRecord
            .ToLink<IWordOfPowerGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IWordOfPower, IWordOfPowerGetter>(state.LinkCache)
            .ToArray();
    }
}
