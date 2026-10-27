using DreadsMashedPatch.AssetForwarding;
using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Impact;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Decal presence gates child writes; bounds and parallax each retain their coupled values.
    // Other decal fields can merge independently without a partial reversion replacing all of DODT.
    public class ImpactRecordHandler : AbstractRecordHandler
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
            { "Model", new ModelHandler() },
            { "Duration", new SimpleReflectionPropertyHandler<float, IImpact, IImpactGetter>("Duration") },
            { "Orientation", new SimpleReflectionPropertyHandler<Mutagen.Bethesda.Skyrim.Impact.OrientationType, IImpact, IImpactGetter>("Orientation") },
            { "AngleThreshold", new SimpleReflectionPropertyHandler<float, IImpact, IImpactGetter>("AngleThreshold") },
            { "PlacementRadius", new SimpleReflectionPropertyHandler<float, IImpact, IImpactGetter>("PlacementRadius") },
            { "SoundLevel", new SimpleReflectionPropertyHandler<SoundLevel, IImpact, IImpactGetter>("SoundLevel") },
            { "NoDecalData", new SimpleReflectionPropertyHandler<bool, IImpact, IImpactGetter>("NoDecalData") },
            { "Result", new SimpleReflectionPropertyHandler<Mutagen.Bethesda.Skyrim.Impact.ResultType, IImpact, IImpactGetter>("Result") },
            { "Decal.Presence", new ImpactDecalPresenceHandler() },
            { "Decal.Bounds", new ImpactDecalBoundsHandler() },
            { "Decal.Depth", new SimpleReflectionPropertyHandler<float, IImpact, IImpactGetter>("Decal.Depth") },
            { "Decal.Shininess", new SimpleReflectionPropertyHandler<float, IImpact, IImpactGetter>("Decal.Shininess") },
            { "Decal.Parallax", new ImpactDecalParallaxHandler() },
            { "Decal.Flags", new SimpleReflectionFlagPropertyHandler<Decal.Flag, IImpact, IImpactGetter>("Decal.Flags", preserveUnknownBits: true) },
            { "Decal.Color", new SimpleReflectionPropertyHandler<System.Drawing.Color, IImpact, IImpactGetter>("Decal.Color") },
            { "TextureSet", new SimpleReflectionFormLinkPropertyHandler<ITextureSetGetter, IImpact, IImpactGetter>("TextureSet") },
            { "SecondaryTextureSet", new SimpleReflectionFormLinkPropertyHandler<ITextureSetGetter, IImpact, IImpactGetter>("SecondaryTextureSet") },
            { "Sound1", new SimpleReflectionFormLinkPropertyHandler<ISoundGetter, IImpact, IImpactGetter>("Sound1") },
            { "Sound2", new SimpleReflectionFormLinkPropertyHandler<ISoundGetter, IImpact, IImpactGetter>("Sound2") },
            { "Hazard", new SimpleReflectionFormLinkPropertyHandler<IHazardGetter, IImpact, IImpactGetter>("Hazard") }
        };

        public override void ApplyForwardedProperties(
            IMajorRecord record,
            Dictionary<string, object?> propertiesToForward)
        {
            if (record is not IImpact impact)
            {
                throw new InvalidOperationException($"Expected IImpact but got {record.GetType()}");
            }

            var hasPresenceDecision = propertiesToForward.TryGetValue(
                "Decal.Presence",
                out var presenceValue);
            var shouldBePresent = hasPresenceDecision
                ? presenceValue is bool present
                    ? present
                    : throw new InvalidOperationException(
                        $"Decal.Presence expected a Boolean value but got {presenceValue?.GetType().Name ?? "null"}")
                : impact.Decal != null;

            var coordinatedProperties = new Dictionary<string, object?>();
            if (hasPresenceDecision)
            {
                coordinatedProperties["Decal.Presence"] = shouldBePresent;
            }

            foreach (var (propertyName, value) in propertiesToForward)
            {
                if (propertyName == "Decal.Presence") continue;
                if (!shouldBePresent && propertyName.StartsWith("Decal.", StringComparison.Ordinal))
                {
                    continue;
                }

                coordinatedProperties[propertyName] = value;
            }

            base.ApplyForwardedProperties(record, coordinatedProperties);
        }

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IImpactGetter impact)
            {
                throw new InvalidOperationException($"Expected IImpactGetter but got {winningContext.Record.GetType()}");
            }

            return impact
                .ToLink<IImpactGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IImpact, IImpactGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
