using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.RecordHandlers.Interfaces
{
    public interface IRecordHandler
    {
        PatchRunReport Process(IPatcherState<ISkyrimMod, ISkyrimModGetter> state, IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] filteredWinningContexts, RecordPolicySources? policySources = null);

        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state);

        void CommitOverride(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
            Dictionary<string, object?>? propertiesToForward = null);

        void ApplyForwardedProperties(IMajorRecord record, Dictionary<string, object?> propertiesToForward);

        // Property that all record handlers must implement
        Dictionary<string, IPropertyHandler> PropertyHandlers { get; }
    }
}
