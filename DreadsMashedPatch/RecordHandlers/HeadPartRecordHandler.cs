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
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: scalar, flag, link, model, and ExtraParts fields use shared handlers.
    // - Kept specialized: Parts preserves ordered atomic NAM0/NAM1 pairs with Mutagen-generated deep copying.
    // - Rationale: binary-overlay filenames are getter asset links and cannot be assigned to mutable asset links by reflection.

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
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(HeadPart.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IHeadPart, IHeadPartGetter>("Name") },
            { "Model", new ModelHandler() },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.HeadPart.Flag, IHeadPart, IHeadPartGetter>("Flags") },
            { "Type", new SimpleReflectionPropertyHandler<Mutagen.Bethesda.Skyrim.HeadPart.TypeEnum?, IHeadPart, IHeadPartGetter>("Type") },
            { "ExtraParts", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IHeadPartGetter>, IHeadPart, IHeadPartGetter>("ExtraParts", ListSemantics.SortedKeyed) },
            { "Parts", new PartsHandler() },
            { "TextureSet", new SimpleReflectionFormLinkPropertyHandler<ITextureSetGetter, IHeadPart, IHeadPartGetter>("TextureSet") },
            { "Color", new SimpleReflectionFormLinkPropertyHandler<IColorRecordGetter, IHeadPart, IHeadPartGetter>("Color") },
            { "ValidRaces", new SimpleReflectionFormLinkPropertyHandler<IFormListGetter, IHeadPart, IHeadPartGetter>("ValidRaces") },

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
