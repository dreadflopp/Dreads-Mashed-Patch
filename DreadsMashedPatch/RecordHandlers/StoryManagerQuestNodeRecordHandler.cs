using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Topology, conditions, flags and limits establish complete-node ownership. Quest rows use
// FormID keys and can merge only while that configuration is stable.
public class StoryManagerQuestNodeRecordHandler : AbstractRecordHandler
{
    private static readonly IReadOnlySet<string> ConfigurationPropertyNames = new HashSet<string>(StringComparer.Ordinal)
    {
        "Parent",
        "PreviousSibling",
        "Conditions",
        "Flags",
        "QuestFlags",
        "MaxConcurrentQuests",
        "MaxNumQuestsToRun"
    };

    protected override IReadOnlySet<string> AtomicOwnershipTriggerProperties =>
        ConfigurationPropertyNames;

    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Parent", new SimpleReflectionFormLinkPropertyHandler<IAStoryManagerNodeGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>("Parent") },
        { "PreviousSibling", new SimpleReflectionFormLinkPropertyHandler<IAStoryManagerNodeGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>("PreviousSibling") },
        { "Conditions", new ConditionsHandler<IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(record => record.Conditions, record => record.Conditions) },
        { "Flags", new SimpleReflectionFlagPropertyHandler<AStoryManagerNode.Flag, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>("Flags") },
        { "QuestFlags", new SimpleReflectionFlagPropertyHandler<StoryManagerQuestNode.QuestFlag, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>("QuestFlags") },
        { "MaxConcurrentQuests", new SimpleReflectionPropertyHandler<uint?, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>("MaxConcurrentQuests") },
        { "MaxNumQuestsToRun", new SimpleReflectionPropertyHandler<uint?, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>("MaxNumQuestsToRun") },
        { "Quests", new GeneratedCopyReflectionListPropertyHandler<IStoryManagerQuestGetter, StoryManagerQuest, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(
            "Quests", ListSemantics.AlignedOrdered, value => value.DeepCopy(), StoryManagerQuestMixIn.Equals,
            keySelector: quest => quest.Quest.FormKey) }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IStoryManagerQuestNodeGetter record)
        {
            throw new InvalidOperationException($"Expected IStoryManagerQuestNodeGetter but got {winningContext.Record.GetType()}");
        }

        return record
            .ToLink<IStoryManagerQuestNodeGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerQuestNode, IStoryManagerQuestNodeGetter>(state.LinkCache)
            .ToArray();
    }
}
