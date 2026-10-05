using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Migration note:
// - Generalized: localized name and nullable form links use typed shared reflection handlers.
// - Kept specialized/atomic: Model and ObjectBounds share one geometry owner; Destructible remains
//   one generated-copy aggregate; Keywords and VMAD scripts retain their xEdit sorted/keyed handlers.
// - Flag decision: the raw record-header handler is the sole storage path and owns common Skyrim
//   plus TACT-specific flags.
// - Intentionally excluded: xEdit marks PNAM and FNAM cpIgnore, so the winning binary values are preserved.
// - Rationale: semantic TACT fields remain independently mergeable without splitting cohesive model,
//   destructible, or header state, or manufacturing meaning for ignored binary data.
public class TalkingActivatorRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(TalkingActivator.MajorFlag)) },
        // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
        { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<ITalkingActivator, ITalkingActivatorGetter>() },
        { "Name", new TranslatedStringReflectionPropertyHandler<ITalkingActivator, ITalkingActivatorGetter>("Name") },
        { "ModelAndBounds", new ModelBoundsHandler() },
        { "Destructible", new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, ITalkingActivator, ITalkingActivatorGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals) },
        { "Keywords", new KeywordListHandler() },
        { "LoopingSound", new SimpleReflectionFormLinkPropertyHandler<ISoundMarkerGetter, ITalkingActivator, ITalkingActivatorGetter>("LoopingSound") },
        { "Voice", new SimpleReflectionFormLinkPropertyHandler<IVoiceTypeGetter, ITalkingActivator, ITalkingActivatorGetter>("Voice") }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not ITalkingActivatorGetter talkingActivatorRecord)
        {
            throw new InvalidOperationException($"Expected ITalkingActivatorGetter but got {winningContext.Record.GetType()}");
        }

        return talkingActivatorRecord
            .ToLink<ITalkingActivatorGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ITalkingActivator, ITalkingActivatorGetter>(state.LinkCache)
            .ToArray();
    }
}
