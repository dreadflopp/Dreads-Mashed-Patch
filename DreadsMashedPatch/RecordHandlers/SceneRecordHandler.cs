using DreadsMashedPatch.AssetForwarding;
using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Scene;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Actions, actors, phases, quest aliases, last-action index and VMAD fragments cross-reference
// one another. Indexed graph changes therefore select a complete SCEN ownership boundary.
public class SceneRecordHandler : AbstractRecordHandler
{
    protected override void InitializePropertyContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> originalContext,
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext)
    {
        base.InitializePropertyContexts(originalContext, winningContext);
        AssetForwardingPolicy.ResolveMemberContexts(GetType(), PropertyHandlers, PropertyContexts);
    }

    protected override PropertyForwardingCoordination CoordinateForwardedProperties(
        IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state) =>
        AssetForwardingPolicy.Coordinate(GetType(), PropertyHandlers, recordContexts, state, AtomicOwnershipTriggerProperties);

    private const string VirtualMachineAdapterPrefix = "VirtualMachineAdapter.";
    private const string VirtualMachineAdapterPresence = "VirtualMachineAdapter.Presence";

    private static readonly IReadOnlySet<string> StructuralPropertyNames = new HashSet<string>(StringComparer.Ordinal)
    {
        VirtualMachineAdapterPresence,
        "VirtualMachineAdapter.ScriptFragments",
        "Phases",
        "Actors",
        "Actions",
        "Quest",
        "LastActionIndex"
    };

    protected override IReadOnlySet<string> AtomicOwnershipTriggerProperties => StructuralPropertyNames;

    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { VirtualMachineAdapterPresence, new SceneVirtualMachineAdapterPresenceHandler() },
        { "VirtualMachineAdapter.Version", new SimpleReflectionPropertyHandler<short, IScene, ISceneGetter>("VirtualMachineAdapter.Version") },
        { "VirtualMachineAdapter.ObjectFormat", new SimpleReflectionPropertyHandler<ushort, IScene, ISceneGetter>("VirtualMachineAdapter.ObjectFormat") },
        { "VirtualMachineAdapter.Scripts", new SceneScriptsHandler() },
        { "VirtualMachineAdapter.ScriptFragments", new SceneScriptFragmentsHandler() },
        { "Flags", new SimpleReflectionFlagPropertyHandler<Scene.Flag, IScene, ISceneGetter>("Flags", preserveUnknownBits: true) },
        { "Phases", new ScenePhasesHandler() },
        { "Actors", new SceneActorsHandler() },
        { "Actions", new SceneActionsHandler() },
        { "Quest", new SimpleReflectionFormLinkPropertyHandler<IQuestGetter, IScene, ISceneGetter>("Quest") },
        { "LastActionIndex", new SimpleReflectionPropertyHandler<uint?, IScene, ISceneGetter>("LastActionIndex") },
        { "VNAM", new SimpleReflectionBinaryDataPropertyHandler<IScene, ISceneGetter>("VNAM") },
        { "Conditions", new ConditionsHandler<IScene, ISceneGetter>(record => record.Conditions, record => record.Conditions) }
    };

    public override void ApplyForwardedProperties(
        IMajorRecord record,
        Dictionary<string, object?> propertiesToForward)
    {
        if (record is not IScene scene)
        {
            throw new InvalidOperationException($"Expected IScene but got {record.GetType()}");
        }

        var hasPresenceDecision = propertiesToForward.TryGetValue(
            VirtualMachineAdapterPresence,
            out var presenceValue);
        var shouldBePresent = hasPresenceDecision
            ? presenceValue is bool present
                ? present
                : throw new InvalidOperationException(
                    $"{VirtualMachineAdapterPresence} expected a Boolean value but got {presenceValue?.GetType().Name ?? "null"}")
            : scene.VirtualMachineAdapter != null;

        var coordinatedProperties = new Dictionary<string, object?>();
        if (hasPresenceDecision)
        {
            coordinatedProperties[VirtualMachineAdapterPresence] = shouldBePresent;
        }

        foreach (var (propertyName, value) in propertiesToForward)
        {
            if (propertyName == VirtualMachineAdapterPresence) continue;
            if (!shouldBePresent && propertyName.StartsWith(VirtualMachineAdapterPrefix, StringComparison.Ordinal))
            {
                continue;
            }

            coordinatedProperties[propertyName] = value;
        }

        base.ApplyForwardedProperties(record, coordinatedProperties);
    }

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not ISceneGetter record)
        {
            throw new InvalidOperationException($"Expected ISceneGetter but got {winningContext.Record.GetType()}");
        }

        return record
            .ToLink<ISceneGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IScene, ISceneGetter>(state.LinkCache)
            .ToArray();
    }
}
