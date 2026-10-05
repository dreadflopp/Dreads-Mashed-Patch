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

// Migration note:
// - Generalized: SHOU text, links, and metadata use shared semantic handlers.
// - Kept specialized: WordsOfPower is an atomic non-alignable sequence; header flags use the approved composite raw handler.
// - Rationale: xEdit marks the word sequence as declaration-ordered without a safe row identity.

// Header migration: raw/common/Shout.MajorFlag flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
// Name migration: translated Name uses generated copying to retain every selected language.
// Optional null names remove the value; comparison follows Mutagen's language policy.
// Other specialized fields/flags retain their policies; translations are selected as one value.
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
