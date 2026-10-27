using DreadsMashedPatch.AssetForwarding;
using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.LensFlare;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;
public class LensFlareRecordHandler : AbstractRecordHandler
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
        { "ColorInfluence", new SimpleReflectionPropertyHandler<float?, ILensFlare, ILensFlareGetter>("ColorInfluence") },
        { "FadeDistanceRadiusScale", new SimpleReflectionPropertyHandler<float?, ILensFlare, ILensFlareGetter>("FadeDistanceRadiusScale") },
        { "FlareSprites", new NestedAssetPropertyHandler<FlareDefinitionValue>("FlareSprites", new FlareDefinitionHandler(),
            value => value == null ? null : value with { ColorInfluence = null, FadeDistanceRadiusScale = null },
            (value, destination) => value == null ? destination : value with
            { ColorInfluence = destination?.ColorInfluence, FadeDistanceRadiusScale = destination?.FadeDistanceRadiusScale },
            (value, _) => value, "Sprites") },
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not ILensFlareGetter lensFlareRecord)
        {
            throw new InvalidOperationException($"Expected ILensFlareGetter but got {winningContext.Record.GetType()}");
        }

        return lensFlareRecord
            .ToLink<ILensFlareGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ILensFlare, ILensFlareGetter>(state.LinkCache)
            .ToArray();
    }
}
