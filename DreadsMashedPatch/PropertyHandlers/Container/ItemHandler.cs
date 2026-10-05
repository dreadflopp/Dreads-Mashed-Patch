using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace DreadsMashedPatch.PropertyHandlers.Container
{
    public class ItemHandler : AbstractInventoryItemsHandler<IContainerGetter, IContainer>
    {
        protected override IReadOnlyList<IContainerEntryGetter>? GetItems(IContainerGetter record) => record.Items;

        protected override void SetItems(IContainer record, ExtendedList<ContainerEntry>? items) => record.Items = items;

        protected override bool CanUpdateCount(
            ISkyrimModGetter recordMod,
            ListPropertyContext<ContainerEntry> listPropertyContext,
            ListPropertyValueContext<ContainerEntry> forwardItem,
            ContainerEntry recordItem)
        {
            // Preserve CONT's existing keyed count policy: a newly different count
            // can forward independently; returning to the baseline needs permission.
            // Reconcile it here once so shared row replacement cannot overwrite COED.
            var originalItem = listPropertyContext.OriginalValueContexts?.FirstOrDefault(item =>
                item.Value.Item.Item.FormKey == recordItem.Item.Item.FormKey);
            return originalItem == null
                || originalItem.Value.Item.Count != recordItem.Item.Count
                || HasPermissionsToModify(recordMod, forwardItem.OwnerMod);
        }
    }
}
