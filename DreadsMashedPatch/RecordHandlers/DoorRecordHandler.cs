using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Door;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: VM adapter, bounds, name, model, sounds, and major flags via shared handlers.
    // - Kept specialized: Destructible via dedicated destructible handler.
    // - Rationale: preserve destructible deep-copy semantics while reusing stable scalar/link handlers.

    // Header migration: raw/common/Door.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class DoorRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Door.MajorFlag)) },
            // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IDoor, IDoorGetter>() },
            { "Name", new TranslatedStringReflectionPropertyHandler<IDoor, IDoorGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Destructible", new DestructibleHandler() },
            { "OpenSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IDoor, IDoorGetter>("OpenSound") },
            { "CloseSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IDoor, IDoorGetter>("CloseSound") },
            { "LoopSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IDoor, IDoorGetter>("LoopSound") },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.Door.Flag, IDoor, IDoorGetter>("Flags") },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IDoorGetter door)
            {
                throw new InvalidOperationException($"Expected IDoorGetter but got {winningContext.Record.GetType()}");
            }

            return door
                .ToLink<IDoorGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IDoor, IDoorGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
