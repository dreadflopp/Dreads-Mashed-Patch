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
