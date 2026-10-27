using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;
public class GlobalUnknownRecordHandler : AbstractSubtypeRecordHandler<IGlobalUnknownGetter>
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Global.MajorFlag)) },
        { "TypeChar", new SimpleReflectionPropertyHandler<char, IGlobalUnknown, IGlobalUnknownGetter>("TypeChar") },

        { "Data", new SimpleReflectionPropertyHandler<float?, IGlobalUnknown, IGlobalUnknownGetter>("Data") }
    };
}
