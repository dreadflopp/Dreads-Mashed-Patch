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

// Topology, conditions, flags, concurrency and event type jointly define the event branch;
// a change to any of them establishes complete-node ownership.
public class StoryManagerEventNodeRecordHandler : AbstractRecordHandler
{
    private static readonly IReadOnlySet<string> ConfigurationPropertyNames = new HashSet<string>(StringComparer.Ordinal)
    {
        "Parent",
        "PreviousSibling",
        "Conditions",
        "Flags",
        "MaxConcurrentQuests",
        "Type"
    };

    protected override IReadOnlySet<string> AtomicOwnershipTriggerProperties =>
        ConfigurationPropertyNames;

    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Parent", new SimpleReflectionFormLinkPropertyHandler<IAStoryManagerNodeGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter>("Parent") },
        { "PreviousSibling", new SimpleReflectionFormLinkPropertyHandler<IAStoryManagerNodeGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter>("PreviousSibling") },
        { "Conditions", new ConditionsHandler<IStoryManagerEventNode, IStoryManagerEventNodeGetter>(record => record.Conditions, record => record.Conditions) },
        { "Flags", new SimpleReflectionNullableFlagPropertyHandler<AStoryManagerNode.Flag, IStoryManagerEventNode, IStoryManagerEventNodeGetter>("Flags", preserveUnknownBits: true) },
        { "MaxConcurrentQuests", new SimpleReflectionPropertyHandler<uint?, IStoryManagerEventNode, IStoryManagerEventNodeGetter>("MaxConcurrentQuests") },
        { "Type", new SimpleReflectionPropertyHandler<StoryManagerEventNode.Types?, IStoryManagerEventNode, IStoryManagerEventNodeGetter>("Type") }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IStoryManagerEventNodeGetter record)
        {
            throw new InvalidOperationException($"Expected IStoryManagerEventNodeGetter but got {winningContext.Record.GetType()}");
        }

        return record
            .ToLink<IStoryManagerEventNodeGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IStoryManagerEventNode, IStoryManagerEventNodeGetter>(state.LinkCache)
            .ToArray();
    }
}
