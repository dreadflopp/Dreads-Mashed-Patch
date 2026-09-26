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
public class NavigationMeshRecordHandler : AbstractRecordHandler
{
    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler() },
        { "SkyrimMajorRecordFlags", new SkyrimMajorRecordFlagsHandler() },
        { "Data", new GeneratedCopyReflectionPropertyHandler<INavigationMeshDataGetter, NavigationMeshData, INavigationMesh, INavigationMeshGetter>(
            "Data", value => value.DeepCopy(), NavigationMeshDataMixIn.Equals) },
        { "ONAM", new SimpleReflectionBinaryDataPropertyHandler<INavigationMesh, INavigationMeshGetter>("ONAM") },
        { "PNAM", new SimpleReflectionBinaryDataPropertyHandler<INavigationMesh, INavigationMeshGetter>("PNAM") },
        { "NNAM", new SimpleReflectionBinaryDataPropertyHandler<INavigationMesh, INavigationMeshGetter>("NNAM") },
        { "MajorFlags", new SimpleReflectionFlagPropertyHandler<NavigationMesh.MajorFlag, INavigationMesh, INavigationMeshGetter>("MajorFlags") }
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
