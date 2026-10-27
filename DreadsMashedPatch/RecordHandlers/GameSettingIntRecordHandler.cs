using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;
public class GameSettingIntRecordHandler : AbstractSubtypeRecordHandler<IGameSettingIntGetter>
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Data", new SimpleReflectionPropertyHandler<int?, IGameSettingInt, IGameSettingIntGetter>("Data") }
    };
}
