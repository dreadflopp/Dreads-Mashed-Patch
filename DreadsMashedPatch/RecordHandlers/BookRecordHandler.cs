using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Book;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: BOOK text, links, flags, and scalar fields use shared semantic handlers.
    // - Kept specialized: Teaches and Icons retain their typed aggregate handlers.
    // - Intentionally excluded: Unused is non-semantic storage; the winning value is preserved.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class BookRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IBook, IBookGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Value", new ValueHandler() },
            { "Weight", new WeightHandler() },
            { "Description", new TranslatedStringReflectionPropertyHandler<IBook, IBookGetter>("Description") },
            { "PickUpSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IBook, IBookGetter>("PickUpSound") },
            { "PutDownSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IBook, IBookGetter>("PutDownSound") },
            { "Keywords", new KeywordListHandler() },
            { "BookText", new TranslatedStringReflectionPropertyHandler<IBook, IBookGetter>("BookText") },
            { "Destructible", new DestructibleHandler() },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.Book.Flag, IBook, IBookGetter>("Flags") },
            { "Type", new SimpleReflectionPropertyHandler<Mutagen.Bethesda.Skyrim.Book.BookType, IBook, IBookGetter>("Type") },
            { "Teaches", new TeachesHandler() },
            { "InventoryArt", new SimpleReflectionFormLinkPropertyHandler<IStaticGetter, IBook, IBookGetter>("InventoryArt") },
            // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IBook, IBookGetter>() },
            { "Icons", new IconsHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IBookGetter bookRecord)
            {
                throw new InvalidOperationException($"Expected IBookGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = bookRecord
                .ToLink<IBookGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IBook, IBookGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }

        // CommitOverride and ApplyForwardedProperties are now handled by the base class
        // The base class automatically handles flag property coordination
    }
}
