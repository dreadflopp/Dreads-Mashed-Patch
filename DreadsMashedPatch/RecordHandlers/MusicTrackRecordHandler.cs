using DreadsMashedPatch.AssetForwarding;
using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Skyrim.Assets;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.MusicTrack;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.RecordHandlers;

// Tracks is one ordered value: xEdit gives the sequence no stable row key.
public class MusicTrackRecordHandler : AbstractRecordHandler
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
        { "Type", new SimpleReflectionPropertyHandler<MusicTrack.TypeEnum, IMusicTrack, IMusicTrackGetter>("Type") },
        { "Duration", new SimpleReflectionPropertyHandler<float?, IMusicTrack, IMusicTrackGetter>("Duration") },
        { "FadeOut", new SimpleReflectionPropertyHandler<float?, IMusicTrack, IMusicTrackGetter>("FadeOut") },
        { "TrackFilename", new TrackAssetLinkHandler("TrackFilename") },
        { "FinaleFilename", new TrackAssetLinkHandler("FinaleFilename") },
        { "LoopData", new GeneratedCopyReflectionPropertyHandler<IMusicTrackLoopDataGetter, MusicTrackLoopData, IMusicTrack, IMusicTrackGetter>(
            "LoopData", value => value.DeepCopy(), MusicTrackLoopDataMixIn.Equals) },
        { "CuePoints", new CuePointsHandler() },
        { "Conditions", new ConditionsHandler() },
        { "Tracks", new AtomicReflectionListPropertyHandler<IFormLinkGetter<IMusicTrackGetter>, IMusicTrack, IMusicTrackGetter>("Tracks") }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IMusicTrackGetter musicTrackRecord)
        {
            throw new InvalidOperationException($"Expected IMusicTrackGetter but got {winningContext.Record.GetType()}");
        }

        return musicTrackRecord
            .ToLink<IMusicTrackGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IMusicTrack, IMusicTrackGetter>(state.LinkCache)
            .ToArray();
    }
}
