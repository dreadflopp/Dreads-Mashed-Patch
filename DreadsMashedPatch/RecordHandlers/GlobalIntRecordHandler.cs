using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;
public class GlobalIntRecordHandler : AbstractSubtypeRecordHandler<IGlobalIntGetter>
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Global.MajorFlag)) },
        { "Data", new SimpleReflectionPropertyHandler<int?, IGlobalInt, IGlobalIntGetter>("Data") }
    };
}
