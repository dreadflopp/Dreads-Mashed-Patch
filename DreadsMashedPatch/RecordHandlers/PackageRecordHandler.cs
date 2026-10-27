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
using DreadsMashedPatch.PropertyHandlers.Package;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// PackageTemplateGraph remains unregistered: Mutagen 0.54.4 writes data in UNAM-key order,
// changing xEdit positional rows. Procedure branches use data indexes; re-enable only with an
// order-preserving writer and the existing binary regressions. Idle selection has separate ownership.
public class PackageRecordHandler : AbstractRecordHandler
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

    public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
    {
        { "EditorID", new EditorIDHandler() },
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "VirtualMachineAdapter", new GeneratedCopyReflectionPropertyHandler<IPackageAdapterGetter, PackageAdapter, IPackage, IPackageGetter>(
            "VirtualMachineAdapter", value => value.DeepCopy(), PackageAdapterMixIn.Equals) },
        { "Flags", new SimpleReflectionFlagPropertyHandler<Package.Flag, IPackage, IPackageGetter>("Flags") },
        { "Type", new SimpleReflectionPropertyHandler<Package.Types, IPackage, IPackageGetter>("Type") },
        { "InterruptOverride", new SimpleReflectionPropertyHandler<Package.Interrupt, IPackage, IPackageGetter>("InterruptOverride") },
        { "PreferredSpeed", new SimpleReflectionPropertyHandler<Package.Speed, IPackage, IPackageGetter>("PreferredSpeed") },
        { "InterruptFlags", new SimpleReflectionFlagPropertyHandler<Package.InterruptFlag, IPackage, IPackageGetter>("InterruptFlags", preserveUnknownBits: true) },
        { "ScheduleMonth", new SimpleReflectionPropertyHandler<sbyte, IPackage, IPackageGetter>("ScheduleMonth") },
        { "ScheduleDayOfWeek", new SimpleReflectionFlagPropertyHandler<Package.DayOfWeek, IPackage, IPackageGetter>("ScheduleDayOfWeek") },
        { "ScheduleDate", new SimpleReflectionPropertyHandler<byte, IPackage, IPackageGetter>("ScheduleDate") },
        { "ScheduleHour", new SimpleReflectionPropertyHandler<sbyte, IPackage, IPackageGetter>("ScheduleHour") },
        { "ScheduleMinute", new SimpleReflectionPropertyHandler<sbyte, IPackage, IPackageGetter>("ScheduleMinute") },
        { "ScheduleDurationInMinutes", new SimpleReflectionPropertyHandler<int, IPackage, IPackageGetter>("ScheduleDurationInMinutes") },
        { "Conditions", new ConditionsHandler<IPackage, IPackageGetter>(record => record.Conditions, record => record.Conditions) },
        { "IdleAnimations", new GeneratedCopyReflectionPropertyHandler<IPackageIdlesGetter, PackageIdles, IPackage, IPackageGetter>(
            "IdleAnimations", value => value.DeepCopy(), PackageIdlesMixIn.Equals) },
        { "CombatStyle", new SimpleReflectionFormLinkPropertyHandler<ICombatStyleGetter, IPackage, IPackageGetter>("CombatStyle") },
        { "OwnerQuest", new SimpleReflectionFormLinkPropertyHandler<IQuestGetter, IPackage, IPackageGetter>("OwnerQuest") },
        { "OnBegin", new GeneratedCopyReflectionPropertyHandler<IPackageEventGetter, PackageEvent, IPackage, IPackageGetter>(
            "OnBegin", value => value.DeepCopy(), PackageEventMixIn.Equals) },
        { "OnEnd", new GeneratedCopyReflectionPropertyHandler<IPackageEventGetter, PackageEvent, IPackage, IPackageGetter>(
            "OnEnd", value => value.DeepCopy(), PackageEventMixIn.Equals) },
        { "OnChange", new GeneratedCopyReflectionPropertyHandler<IPackageEventGetter, PackageEvent, IPackage, IPackageGetter>(
            "OnChange", value => value.DeepCopy(), PackageEventMixIn.Equals) }
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IPackageGetter record)
        {
            throw new InvalidOperationException($"Expected IPackageGetter but got {winningContext.Record.GetType()}");
        }

        return record
            .ToLink<IPackageGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IPackage, IPackageGetter>(state.LinkCache)
            .ToArray();
    }
}
