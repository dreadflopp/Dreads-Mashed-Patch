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

// Migration note:
// - Generalized: PACK scalar, flag, list, binary, link, and nested properties, including the corrected InterruptFlags path.
// - Temporarily disabled: PackageTemplateGraph would atomically own the template reference, version, indexed data,
//   marker, and ordered procedure tree. Mutagen 0.54.4 sorts PACK data values by UNAM key while writing, which changes
//   their physical xEdit row order. Keep the registration commented out until an order-preserving writer is available.
// - Kept specialized: Conditions uses polymorphic copying; VMAD/events and IdleAnimations use Mutagen-generated
//   aggregate copies so overlay-backed nested values become mutable safely.
// - Intentionally excluded: Unknown* fields are outside the semantic conflict surface.
// - Rationale: procedure branches refer to package data indexes, so splitting the graph can invent invalid combinations;
//   abstract Condition values still need typed copying, and InterruptFlags is a normal flag field.

// Header migration: raw/common flags share one masked integer handler.
// Unknown winner bits stay intact; other fields retain their existing handlers and policies.
// Removed overlapping header registrations so selected clears cannot be reintroduced.
public class PackageRecordHandler : AbstractRecordHandler
{
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
        // TEMPORARILY DISABLED: Mutagen's PACK writer reorders package data by UNAM key.
        // { "PackageTemplateGraph", new PackageTemplateGraphHandler() },
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
