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
// - Generalized: NAVM binary payloads use shared handlers.
// - Kept specialized: Data uses Mutagen's generated aggregate copy because its overlay-backed geometry lists
//   require element conversion into mutable NavmeshTriangle, EdgeLink, and DoorTriangle instances.
// - Rationale: generated copy preserves the full atomic NAVM data aggregate without reflection assignments.

// Header migration: raw/common/NavigationMesh.MajorFlag flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
public class NavigationMeshRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(NavigationMesh.MajorFlag)) },
        { "Data", new GeneratedCopyReflectionPropertyHandler<INavigationMeshDataGetter, NavigationMeshData, INavigationMesh, INavigationMeshGetter>(
            "Data", value => value.DeepCopy(), NavigationMeshDataMixIn.Equals) },
        { "ONAM", new SimpleReflectionBinaryDataPropertyHandler<INavigationMesh, INavigationMeshGetter>("ONAM") },
        { "PNAM", new SimpleReflectionBinaryDataPropertyHandler<INavigationMesh, INavigationMeshGetter>("PNAM") },
        { "NNAM", new SimpleReflectionBinaryDataPropertyHandler<INavigationMesh, INavigationMeshGetter>("NNAM") },

    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not INavigationMeshGetter navigationMeshRecord)
        {
            throw new InvalidOperationException($"Expected INavigationMeshGetter but got {winningContext.Record.GetType()}");
        }

        return navigationMeshRecord
            .ToLink<INavigationMeshGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, INavigationMesh, INavigationMeshGetter>(state.LinkCache)
            .ToArray();
    }
}
