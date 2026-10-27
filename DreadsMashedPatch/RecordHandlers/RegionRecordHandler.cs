using DreadsMashedPatch.AssetForwarding;
using System;
using System.Collections.Generic;
using System.Drawing;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Region;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Areas are one ordered polygon array. Typed copies convert overlay point lists, and equality
// follows xEdit's normalization of reversed polygon directions.
public class RegionRecordHandler : AbstractRecordHandler
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
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Region.MajorFlag)) },
        { "MapColor", new SimpleReflectionPropertyHandler<Color?, IRegion, IRegionGetter>("MapColor") },
        { "Worldspace", new SimpleReflectionFormLinkPropertyHandler<IWorldspaceGetter, IRegion, IRegionGetter>("Worldspace") },
        { "RegionAreas", new RegionAreasHandler() },
        { "Objects", RegionDataHandlers.Body<IRegionObjectsGetter, RegionObjects>("Objects", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
        { "Objects.Icons", RegionDataHandlers.Icons<IRegionObjectsGetter, RegionObjects>("Objects", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
        { "Weather", RegionDataHandlers.Body<IRegionWeatherGetter, RegionWeather>("Weather", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
        { "Weather.Icons", RegionDataHandlers.Icons<IRegionWeatherGetter, RegionWeather>("Weather", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
        { "Map", RegionDataHandlers.Body<IRegionMapGetter, RegionMap>("Map", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
        { "Map.Icons", RegionDataHandlers.Icons<IRegionMapGetter, RegionMap>("Map", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
        { "Land", RegionDataHandlers.Body<IRegionLandGetter, RegionLand>("Land", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
        { "Land.Icons", RegionDataHandlers.Icons<IRegionLandGetter, RegionLand>("Land", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
        { "Grasses", RegionDataHandlers.Body<IRegionGrassesGetter, RegionGrasses>("Grasses", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
        { "Grasses.Icons", RegionDataHandlers.Icons<IRegionGrassesGetter, RegionGrasses>("Grasses", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
        { "Sounds", RegionDataHandlers.Body<IRegionSoundsGetter, RegionSounds>("Sounds", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
        { "Sounds.Music", RegionDataHandlers.Audio(music: true) },
        { "Sounds.Sounds", RegionDataHandlers.Audio(music: false) },
        { "Sounds.Icons", RegionDataHandlers.Icons<IRegionSoundsGetter, RegionSounds>("Sounds", value => value.DeepCopy(), (left, right) => left.Equals(right)) },
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IRegionGetter regionRecord)
        {
            throw new InvalidOperationException($"Expected IRegionGetter but got {winningContext.Record.GetType()}");
        }

        return regionRecord
            .ToLink<IRegionGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IRegion, IRegionGetter>(state.LinkCache)
            .ToArray();
    }
}
