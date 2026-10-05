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

// Migration note:
// - Generalized: MSTT fields including flags and looping sound via existing handlers.
// - Kept specialized: shared bounds/model handlers.
// - Rationale: follows Static-style forwarding while preserving moveable-static-specific fields.

// Header migration: raw/common/MoveableStatic.MajorFlag flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
// Name migration: translated Name uses generated copying to retain every selected language.
// Optional null names remove the value; comparison follows Mutagen's language policy.
// Other specialized fields/flags retain their policies; translations are selected as one value.
public class MoveableStaticRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(MoveableStatic.MajorFlag)) },
        { "Name", new TranslatedStringReflectionPropertyHandler<IMoveableStatic, IMoveableStaticGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
        { "Destructible", new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, IMoveableStatic, IMoveableStaticGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals) },
        { "Flags", new SimpleReflectionFlagPropertyHandler<MoveableStatic.Flag, IMoveableStatic, IMoveableStaticGetter>("Flags") },
        { "LoopingSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IMoveableStatic, IMoveableStaticGetter>("LoopingSound") },

    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IMoveableStaticGetter moveableStaticRecord)
        {
            throw new InvalidOperationException($"Expected IMoveableStaticGetter but got {winningContext.Record.GetType()}");
        }

        return moveableStaticRecord
            .ToLink<IMoveableStaticGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IMoveableStatic, IMoveableStaticGetter>(state.LinkCache)
            .ToArray();
    }
}
