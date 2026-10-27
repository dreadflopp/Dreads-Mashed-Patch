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
    // PerkTree needs generated copies of its nested lists and binary data; it is a get-only collection.
    // CNAM is engine-managed data and does not participate in conflict resolution.
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
