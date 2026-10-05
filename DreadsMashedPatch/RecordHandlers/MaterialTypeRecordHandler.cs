using System;
using System.Collections.Generic;
using System.Drawing;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.RecordHandlers;

// Migration note:
// - Generalized: MATT links, plain-string name, color, buoyancy, and metadata use shared semantic handlers.
// - Kept specialized: none; Flags remains on the project-approved flag handler path.
// - Rationale: every semantic field is independently writable without aggregate coupling.

// Header migration: raw/common flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
// Name migration: plain-string Name uses typed scalar reflection; nullable removal and
// normalized comparison stay intact. Other specialized fields/flags retain their policies.
public class MaterialTypeRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Parent", new SimpleReflectionFormLinkPropertyHandler<IMaterialTypeGetter, IMaterialType, IMaterialTypeGetter>("Parent") },
        { "Name", new SimpleReflectionPropertyHandler<string, IMaterialType, IMaterialTypeGetter>("Name") },
        { "HavokDisplayColor", new SimpleReflectionPropertyHandler<Color?, IMaterialType, IMaterialTypeGetter>("HavokDisplayColor") },
        { "Buoyancy", new SimpleReflectionPropertyHandler<float?, IMaterialType, IMaterialTypeGetter>("Buoyancy") },
        { "Flags", new SimpleReflectionFlagPropertyHandler<MaterialType.Flag, IMaterialType, IMaterialTypeGetter>("Flags") },
        { "HavokImpactDataSet", new SimpleReflectionFormLinkPropertyHandler<IImpactDataSetGetter, IMaterialType, IMaterialTypeGetter>("HavokImpactDataSet") }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IMaterialTypeGetter materialTypeRecord)
        {
            throw new InvalidOperationException($"Expected IMaterialTypeGetter but got {winningContext.Record.GetType()}");
        }

        return materialTypeRecord
            .ToLink<IMaterialTypeGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IMaterialType, IMaterialTypeGetter>(state.LinkCache)
            .ToArray();
    }
}
