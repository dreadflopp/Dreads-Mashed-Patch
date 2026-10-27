using DreadsMashedPatch.AssetForwarding;
using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.LeveledNpc;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Noggog;

namespace DreadsMashedPatch.RecordHandlers
{
    // Entries are an unordered multiset: identical values may occur more than once.
    // OwnerTarget generated copies preserve all owner variants.
    public class LeveledNpcRecordHandler : AbstractRecordHandler
    {
        protected override void InitializePropertyContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> originalContext,
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext)
        {
            base.InitializePropertyContexts(originalContext, winningContext);
            AssetForwardingPolicy.ResolveMemberContexts(GetType(), PropertyHandlers, PropertyContexts);
        }

        protected override PropertyForwardingCoordination CoordinateForwardedProperties(
            IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state) =>
            AssetForwardingPolicy.Coordinate(GetType(), PropertyHandlers, recordContexts, state, AtomicOwnershipTriggerProperties);

        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "ChanceNone", new SimpleReflectionPropertyHandler<Percent, ILeveledNpc, ILeveledNpcGetter>("ChanceNone") },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.LeveledNpc.Flag, ILeveledNpc, ILeveledNpcGetter>("Flags") },
            { "Global", new SimpleReflectionFormLinkPropertyHandler<IGlobalGetter, ILeveledNpc, ILeveledNpcGetter>("Global") },
            { "Entries", new EntriesHandler() },
            { "ModelAndBounds", new ModelBoundsHandler() }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ILeveledNpcGetter leveledNpc)
            {
                throw new InvalidOperationException($"Expected ILeveledNpcGetter but got {winningContext.Record.GetType()}");
            }

            return leveledNpc
                .ToLink<ILeveledNpcGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ILeveledNpc, ILeveledNpcGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
