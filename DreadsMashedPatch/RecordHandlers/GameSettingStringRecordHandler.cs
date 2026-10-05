using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Migration note:
// - Generalized: GMST translated-string data uses Mutagen's generated localized copy.
// - Kept specialized: typed Data handling remains per concrete GMST variant.
// - Rationale: concrete Data type differs across GameSetting variants.

// Header migration: raw/common flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
public class GameSettingStringRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Data", new TranslatedStringReflectionPropertyHandler<IGameSettingString, IGameSettingStringGetter>("Data") }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IGameSettingStringGetter record)
        {
            throw new InvalidOperationException($"Expected IGameSettingStringGetter but got {winningContext.Record.GetType()}");
        }

        return record
            .ToLink<IGameSettingStringGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IGameSettingString, IGameSettingStringGetter>(state.LinkCache)
            .ToArray();
    }
}
