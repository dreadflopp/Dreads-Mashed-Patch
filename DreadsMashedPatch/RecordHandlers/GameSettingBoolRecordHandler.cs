using System.Collections.Generic;
using Mutagen.Bethesda.Skyrim;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Migration note:
// - Generalized: GMST bool variant via scalar reflection handlers.
// - Kept specialized: typed Data handling remains per concrete GMST variant.
// - Rationale: concrete Data type differs across GameSetting variants.

// History migration: shared unnarrowed group lookup; the latest eligible subtype
// change starts a new baseline. Typed Data and approved flag handlers stay specialized
// because their value surfaces differ; no cross-type conversion is performed.

// Header migration: raw/common flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
public class GameSettingBoolRecordHandler : AbstractSubtypeRecordHandler<IGameSettingBoolGetter>
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Data", new SimpleReflectionPropertyHandler<bool?, IGameSettingBool, IGameSettingBoolGetter>("Data") }
    };
}
