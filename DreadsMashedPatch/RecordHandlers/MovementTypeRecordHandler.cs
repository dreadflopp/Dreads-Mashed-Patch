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

// SPEDDataTypeState controls serializer layout and is not an independently forwarded field.
public class MovementTypeRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Name", new SimpleReflectionPropertyHandler<string?, IMovementType, IMovementTypeGetter>("Name") },
        { "LeftWalk", new SimpleReflectionPropertyHandler<float, IMovementType, IMovementTypeGetter>("LeftWalk") },
        { "LeftRun", new SimpleReflectionPropertyHandler<float, IMovementType, IMovementTypeGetter>("LeftRun") },
        { "RightWalk", new SimpleReflectionPropertyHandler<float, IMovementType, IMovementTypeGetter>("RightWalk") },
        { "RightRun", new SimpleReflectionPropertyHandler<float, IMovementType, IMovementTypeGetter>("RightRun") },
        { "ForwardWalk", new SimpleReflectionPropertyHandler<float, IMovementType, IMovementTypeGetter>("ForwardWalk") },
        { "ForwardRun", new SimpleReflectionPropertyHandler<float, IMovementType, IMovementTypeGetter>("ForwardRun") },
        { "BackWalk", new SimpleReflectionPropertyHandler<float, IMovementType, IMovementTypeGetter>("BackWalk") },
        { "BackRun", new SimpleReflectionPropertyHandler<float, IMovementType, IMovementTypeGetter>("BackRun") },
        { "RotateInPlaceWalk", new SimpleReflectionPropertyHandler<float, IMovementType, IMovementTypeGetter>("RotateInPlaceWalk") },
        { "RotateInPlaceRun", new SimpleReflectionPropertyHandler<float, IMovementType, IMovementTypeGetter>("RotateInPlaceRun") },
        { "RotateWhileMovingRun", new SimpleReflectionPropertyHandler<float, IMovementType, IMovementTypeGetter>("RotateWhileMovingRun") },
        { "AnimationChangeThresholds", new GeneratedCopyReflectionPropertyHandler<IAnimationChangeThresholdsGetter, AnimationChangeThresholds, IMovementType, IMovementTypeGetter>(
            "AnimationChangeThresholds", value => value.DeepCopy(), AnimationChangeThresholdsMixIn.Equals) },
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IMovementTypeGetter movementTypeRecord)
        {
            throw new InvalidOperationException($"Expected IMovementTypeGetter but got {winningContext.Record.GetType()}");
        }

        return movementTypeRecord
            .ToLink<IMovementTypeGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IMovementType, IMovementTypeGetter>(state.LinkCache)
            .ToArray();
    }
}
