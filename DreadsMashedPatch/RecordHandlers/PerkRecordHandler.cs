using DreadsMashedPatch.AssetForwarding;
using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Perk;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Ranks, entry points, conditions and predecessor links cross-reference one another.
// A gameplay-tree change therefore selects a complete PERK rather than independently merged fields.
public class PerkRecordHandler : AbstractRecordHandler
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

    private static readonly IReadOnlySet<string> CoupledPropertyNames = new HashSet<string>(StringComparer.Ordinal)
    {
        "VirtualMachineAdapter",
        "Conditions",
        "Trait",
        "Level",
        "NumRanks",
        "Playable",
        "Hidden",
        "NextPerk",
        "Effects"
    };

    protected override IReadOnlySet<string> AtomicOwnershipTriggerProperties =>
        CoupledPropertyNames;

    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Perk.MajorFlag)) },
        { "VirtualMachineAdapter", new GeneratedCopyReflectionPropertyHandler<IPerkAdapterGetter, PerkAdapter, IPerk, IPerkGetter>(
            "VirtualMachineAdapter", value => value.DeepCopy(), PerkAdapterMixIn.Equals) },
        { "Name", new TranslatedStringReflectionPropertyHandler<IPerk, IPerkGetter>("Name") },
        { "Description", new TranslatedStringReflectionPropertyHandler<IPerk, IPerkGetter>("Description") },
        { "Icons", new SimpleReflectionIconsPropertyHandler<IPerk, IPerkGetter>("Icons") },
        { "Conditions", new ConditionsHandler<IPerk, IPerkGetter>(record => record.Conditions, record => record.Conditions) },
        { "Trait", new SimpleReflectionPropertyHandler<bool, IPerk, IPerkGetter>("Trait") },
        { "Level", new SimpleReflectionPropertyHandler<byte, IPerk, IPerkGetter>("Level") },
        { "NumRanks", new SimpleReflectionPropertyHandler<byte, IPerk, IPerkGetter>("NumRanks") },
        { "Playable", new SimpleReflectionPropertyHandler<bool, IPerk, IPerkGetter>("Playable") },
        { "Hidden", new SimpleReflectionPropertyHandler<bool, IPerk, IPerkGetter>("Hidden") },
        { "NextPerk", new SimpleReflectionFormLinkPropertyHandler<IPerkGetter, IPerk, IPerkGetter>("NextPerk") },
        { "Effects", new EffectsHandler() }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IPerkGetter perkRecord)
        {
            throw new InvalidOperationException($"Expected IPerkGetter but got {winningContext.Record.GetType()}");
        }

        return perkRecord
            .ToLink<IPerkGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IPerk, IPerkGetter>(state.LinkCache)
            .ToArray();
    }
}
