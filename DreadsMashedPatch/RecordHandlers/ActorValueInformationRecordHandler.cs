using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.ActorValueInformation;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: Description and Skill use generated copying; Abbreviation remains a typed scalar.
    // - Kept specialized: PerkTree via a record-specific structural handler.
    // - Intentionally excluded: CNAM is engine-managed binary data outside the semantic conflict surface.
    // - Rationale: PerkTree is a get-only mutable collection whose nested binary and list data require Mutagen's
    //   generated deep-copy and equality semantics.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class ActorValueInformationRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IActorValueInformation, IActorValueInformationGetter>("Name") },
            { "Description", new TranslatedStringReflectionPropertyHandler<IActorValueInformation, IActorValueInformationGetter>("Description") },
            { "Abbreviation", new SimpleReflectionPropertyHandler<string, IActorValueInformation, IActorValueInformationGetter>("Abbreviation") },
            { "Skill", new GeneratedCopyReflectionPropertyHandler<IActorValueSkillGetter, ActorValueSkill, IActorValueInformation, IActorValueInformationGetter>(
                "Skill", value => value.DeepCopy(), ActorValueSkillMixIn.Equals) },
            { "PerkTree", new PerkTreeHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IActorValueInformationGetter actorValueInformation)
            {
                throw new InvalidOperationException($"Expected IActorValueInformationGetter but got {winningContext.Record.GetType()}");
            }

            return actorValueInformation
                .ToLink<IActorValueInformationGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IActorValueInformation, IActorValueInformationGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
