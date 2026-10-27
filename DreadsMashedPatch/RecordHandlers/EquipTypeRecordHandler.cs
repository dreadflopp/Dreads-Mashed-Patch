using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.EquipType;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    public class EquipTypeRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "SlotParents", new SlotParentsHandler() },
            { "UseAllParents", new SimpleReflectionPropertyHandler<bool?, IEquipType, IEquipTypeGetter>("UseAllParents") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IEquipTypeGetter equipType)
            {
                throw new InvalidOperationException($"Expected IEquipTypeGetter but got {winningContext.Record.GetType()}");
            }

            return equipType
                .ToLink<IEquipTypeGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IEquipType, IEquipTypeGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
