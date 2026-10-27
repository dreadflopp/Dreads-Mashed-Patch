using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Shout;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// WordsOfPower is declaration-ordered without a safe row key; select the sequence as one value.
public class ShoutRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Shout.MajorFlag)) },
        { "Name", new TranslatedStringReflectionPropertyHandler<IShout, IShoutGetter>("Name") },
        { "MenuDisplayObject", new SimpleReflectionFormLinkPropertyHandler<IStaticGetter, IShout, IShoutGetter>("MenuDisplayObject") },
        { "Description", new TranslatedStringReflectionPropertyHandler<IShout, IShoutGetter>("Description") },
        { "WordsOfPower", new WordsOfPowerHandler() },

    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IShoutGetter shoutRecord)
        {
            throw new InvalidOperationException($"Expected IShoutGetter but got {winningContext.Record.GetType()}");
        }

        return shoutRecord
            .ToLink<IShoutGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IShout, IShoutGetter>(state.LinkCache)
            .ToArray();
    }
}
