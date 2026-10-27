using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;
public class GameSettingBoolRecordHandler : AbstractSubtypeRecordHandler<IGameSettingBoolGetter>
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Data", new SimpleReflectionPropertyHandler<bool?, IGameSettingBool, IGameSettingBoolGetter>("Data") }
    };
}
