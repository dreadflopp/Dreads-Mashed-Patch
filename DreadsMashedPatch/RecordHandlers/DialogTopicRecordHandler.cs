using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.DialogTopic;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: DIAL links, enums, priority, name, and record metadata use shared semantic handlers.
    // - Kept specialized: TopicFlags retains its approved flag handler.
    // - Intentionally excluded: Unknown is child-group navigation metadata rather than a normal DIAL field.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class DialogTopicRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IDialogTopic, IDialogTopicGetter>("Name") },
            { "Priority", new SimpleReflectionPropertyHandler<float, IDialogTopic, IDialogTopicGetter>("Priority", 0.001f) },
            { "Branch", new SimpleReflectionFormLinkPropertyHandler<IDialogBranchGetter, IDialogTopic, IDialogTopicGetter>("Branch") },
            { "Quest", new SimpleReflectionFormLinkPropertyHandler<IQuestGetter, IDialogTopic, IDialogTopicGetter>("Quest") },
            { "TopicFlags", new TopicFlagsHandler() },
            { "Category", new SimpleReflectionPropertyHandler<DialogTopic.CategoryEnum, IDialogTopic, IDialogTopicGetter>("Category") },
            { "Subtype", new SimpleReflectionPropertyHandler<DialogTopic.SubtypeEnum, IDialogTopic, IDialogTopicGetter>("Subtype") },
            { "SubtypeName", new SimpleReflectionPropertyHandler<RecordType, IDialogTopic, IDialogTopicGetter>("SubtypeName") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IDialogTopicGetter dialogTopicRecord)
            {
                throw new InvalidOperationException($"Expected IDialogTopicGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = dialogTopicRecord
                .ToLink<IDialogTopicGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IDialogTopic, IDialogTopicGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }

        // CommitOverride and ApplyForwardedProperties are now handled by the base class
        // The base class automatically handles flag property coordination
    }
}
