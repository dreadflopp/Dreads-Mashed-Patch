using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Migration note:
// - Generalized: GLOB unknown variant via scalar reflection handlers.
// - Kept specialized: typed Data/TypeChar handling remains per concrete GLOB variant.
// - Rationale: concrete Data type and TypeChar semantics differ across Global variants.

// History migration: shared unnarrowed group lookup; the latest eligible subtype
// change starts a new baseline. Typed Data and approved flag handlers stay specialized
// because their value surfaces differ; no cross-type conversion is performed.

// Header migration: raw/common/Global.MajorFlag flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
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
