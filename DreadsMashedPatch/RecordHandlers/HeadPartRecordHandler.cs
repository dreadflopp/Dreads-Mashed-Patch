using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.HeadPart;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Appearance migration: model, type, extra parts, complete morph-file pairs, texture, color and valid races
    //   select one generated-copy asset snapshot through ordinary ownership/reversion forwarding.
    //   Appearance flags use the approved flag setter; sorted extra parts ignore reordering in equality.
    // - Kept specialized: record header flags, translated name and EDID retain their existing policies.
    // - Removed: the independent ordered Parts handler and list/link/model registrations.
    // - Rationale: mixing mesh and morph sources can invalidate exported NPC faces.

    // Header migration: raw/common/HeadPart.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class HeadPartRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "Appearance", new HeadPartAppearanceHandler() },
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(HeadPart.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IHeadPart, IHeadPartGetter>("Name") },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IHeadPartGetter headPart)
            {
                throw new InvalidOperationException($"Expected IHeadPartGetter but got {winningContext.Record.GetType()}");
            }

            return headPart
                .ToLink<IHeadPartGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
