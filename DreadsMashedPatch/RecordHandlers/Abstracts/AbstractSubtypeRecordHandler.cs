using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace DreadsMashedPatch.RecordHandlers.Abstracts;

/// <summary>
/// GLOB/GMST histories are read through their shared group without subtype casts.
/// The most recent eligible subtype change starts a new baseline for all properties.
/// </summary>
public abstract class AbstractSubtypeRecordHandler<TGetter> : AbstractRecordHandler
    where TGetter : class, IMajorRecordGetter
{
    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not TGetter)
            throw new InvalidOperationException($"Expected {typeof(TGetter).Name} but got {winningContext.Record.GetType()}");

        // Ignored plugins do not introduce boundaries. Do not use OfType: it would
        // rejoin an older matching subtype across a real intervening type change.
        return RecordPolicySources.GetEligibleContexts(winningContext.Record, state)
            .TakeWhile(context => context.Record is TGetter).ToArray();
    }
}
