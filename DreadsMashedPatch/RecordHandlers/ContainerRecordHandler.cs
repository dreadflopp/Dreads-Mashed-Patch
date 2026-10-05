using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Container;
using DreadsMashedPatch.PropertyHandlers.General;
using System.Collections.Generic;
using System.Linq;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: independent CONT scalar, link, model, VMAD, and header fields use shared handlers.
    // - Generalized: Items shares NPC inventory snapshots, duplicate matching, and COED reconciliation through AbstractInventoryItemsHandler.
    // - Kept specialized: CONT's keyed count policy permits new counts but gates baseline reversions; destructible data and flags retain semantic handlers.
    // - Rationale: generated copies preserve complete rows/owner variants; one metadata path prevents count forwarding from dropping or bypassing COED.

    // Header migration: raw/common/Container.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class ContainerRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "Name", new TranslatedStringReflectionPropertyHandler<IContainer, IContainerGetter>("Name") },
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Container.MajorFlag)) },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Weight", new SimpleReflectionPropertyHandler<float, IContainer, IContainerGetter>("Weight") },
            { "Items", new ItemHandler() },
            // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
            { "VirtualMachineAdapter", new SimpleReflectionVirtualMachineAdapterHandler<IContainer, IContainerGetter>() },
            { "Destructible", new DestructibleHandler() },
            { "Flags", new FlagsHandler() },
            { "OpenSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IContainer, IContainerGetter>("OpenSound") },
            { "CloseSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IContainer, IContainerGetter>("CloseSound") },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IContainerGetter containerRecord)
            {
                throw new InvalidOperationException($"Expected IContainerGetter but got {winningContext.Record.GetType()}");
            }
            return containerRecord
                .ToLink<IContainerGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IContainer, IContainerGetter>(state.LinkCache)
                .ToArray();
        }

        // ApplyForwardedProperties is now handled by the base class
        // The base class automatically handles flag property coordination
    }
}
