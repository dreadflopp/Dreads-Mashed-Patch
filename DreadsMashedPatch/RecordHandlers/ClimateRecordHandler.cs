using DreadsMashedPatch.AssetForwarding;
using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Climate;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    public class ClimateRecordHandler : AbstractRecordHandler
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
            { "WeatherTypes", new WeatherTypesHandler() },
            { "SunTexture", new SunTextureHandler() },
            { "SunGlareTexture", new SunGlareTextureHandler() },
            { "Model", new ModelHandler() },
            { "SunriseBegin", new SimpleReflectionPropertyHandler<TimeOnly, IClimate, IClimateGetter>("SunriseBegin") },
            { "SunriseEnd", new SimpleReflectionPropertyHandler<TimeOnly, IClimate, IClimateGetter>("SunriseEnd") },
            { "SunsetBegin", new SimpleReflectionPropertyHandler<TimeOnly, IClimate, IClimateGetter>("SunsetBegin") },
            { "SunsetEnd", new SimpleReflectionPropertyHandler<TimeOnly, IClimate, IClimateGetter>("SunsetEnd") },
            { "Volatility", new SimpleReflectionPropertyHandler<byte, IClimate, IClimateGetter>("Volatility") },
            { "Moons", new SimpleReflectionPropertyHandler<Mutagen.Bethesda.Skyrim.Climate.Moon, IClimate, IClimateGetter>("Moons") },
            { "PhaseLength", new SimpleReflectionPropertyHandler<byte, IClimate, IClimateGetter>("PhaseLength") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IClimateGetter climate)
            {
                throw new InvalidOperationException($"Expected IClimateGetter but got {winningContext.Record.GetType()}");
            }

            return climate
                .ToLink<IClimateGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IClimate, IClimateGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
