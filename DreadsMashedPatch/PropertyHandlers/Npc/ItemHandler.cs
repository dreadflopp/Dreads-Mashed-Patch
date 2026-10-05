using DreadsMashedPatch.PropertyHandlers.Abstracts;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace DreadsMashedPatch.PropertyHandlers.Npc
{
    public class ItemHandler : AbstractInventoryItemsHandler<INpcGetter, INpc>
    {
        protected override IReadOnlyList<IContainerEntryGetter>? GetItems(INpcGetter record) => record.Items;

        protected override void SetItems(INpc record, ExtendedList<ContainerEntry>? items) => record.Items = items;
    }
}
