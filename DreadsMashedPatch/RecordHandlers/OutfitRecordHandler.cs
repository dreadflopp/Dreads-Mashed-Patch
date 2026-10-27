using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.RecordHandlers;
public class OutfitRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        // One authored outfit package; xEdit's sorted FormID array does not imply safe item-by-item merging.
        { "Items", new AtomicFormLinkListPropertyHandler<IOutfitTargetGetter, IOutfit, IOutfitGetter>("Items", orderMatters: false) }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IOutfitGetter outfitRecord)
        {
            throw new InvalidOperationException($"Expected IOutfitGetter but got {winningContext.Record.GetType()}");
        }

        return outfitRecord
            .ToLink<IOutfitGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IOutfit, IOutfitGetter>(state.LinkCache)
            .ToArray();
    }
}
