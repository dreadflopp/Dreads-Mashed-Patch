using DreadsMashedPatch.AssetForwarding;
using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Binary.Streams;
using Noggog;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.MaterialObject;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.RecordHandlers;

// DATADataTypeState controls serializer layout and is not an independently forwarded field.
public class MaterialObjectRecordHandler : AbstractRecordHandler
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
        { "DNAMs", new DNAMsHandler() },
        { "FalloffScale", new SimpleReflectionPropertyHandler<float, IMaterialObject, IMaterialObjectGetter>("FalloffScale") },
        { "FalloffBias", new SimpleReflectionPropertyHandler<float, IMaterialObject, IMaterialObjectGetter>("FalloffBias") },
        { "NoiseUvScale", new SimpleReflectionPropertyHandler<float, IMaterialObject, IMaterialObjectGetter>("NoiseUvScale") },
        { "MaterialUvScale", new SimpleReflectionPropertyHandler<float, IMaterialObject, IMaterialObjectGetter>("MaterialUvScale") },
        { "ProjectionVector", new SimpleReflectionPropertyHandler<P3Float, IMaterialObject, IMaterialObjectGetter>("ProjectionVector") },
        { "NormalDampener", new SimpleReflectionPropertyHandler<float, IMaterialObject, IMaterialObjectGetter>("NormalDampener") },
        { "SinglePassColor", new SimpleReflectionPropertyHandler<System.Drawing.Color, IMaterialObject, IMaterialObjectGetter>("SinglePassColor") },
        { "Flags", new SimpleReflectionFlagPropertyHandler<MaterialObject.Flag, IMaterialObject, IMaterialObjectGetter>("Flags", preserveUnknownBits: true) },
        { "HasSnow", new SimpleReflectionPropertyHandler<bool, IMaterialObject, IMaterialObjectGetter>("HasSnow") },
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IMaterialObjectGetter materialObjectRecord)
        {
            throw new InvalidOperationException($"Expected IMaterialObjectGetter but got {winningContext.Record.GetType()}");
        }

        return materialObjectRecord
            .ToLink<IMaterialObjectGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IMaterialObject, IMaterialObjectGetter>(state.LinkCache)
            .ToArray();
    }
}
