using DreadsMashedPatch.Contexts.Interfaces;

namespace DreadsMashedPatch.Contexts
{
    public class ListPropertyContext<T> : IPropertyContext where T : class
    {
        public bool IsResolved { get; set; }
        public List<ListPropertyValueContext<T>>? OriginalValueContexts { get; set; }
        public List<ListPropertyValueContext<T>>? ForwardValueContexts { get; set; }
        public List<ListAlignmentRow<T>> AlignmentRows { get; set; } = [];
        public int NextAlignmentRowId { get; set; }
        // Presence has its own owner: removing the final row does not necessarily remove
        // an optional list subrecord, and an empty list can still be explicitly authored.
        public bool CanBeNull { get; set; }
        public bool OriginalIsNull { get; set; }
        public bool ForwardIsNull { get; set; }
        public string? ForwardPresenceOwnerMod { get; set; }

        public ListPropertyContext()
        {
            OriginalValueContexts = null;
            ForwardValueContexts = null;
        }
        public ListPropertyContext(
            List<ListPropertyValueContext<T>>? originalValueContexts,
            List<ListPropertyValueContext<T>>? forwardValueContexts,
            bool canBeNull = false,
            bool originalIsNull = false,
            bool forwardIsNull = false,
            string? forwardPresenceOwnerMod = null
        )
        {
            OriginalValueContexts = originalValueContexts;
            ForwardValueContexts = forwardValueContexts;
            CanBeNull = canBeNull;
            OriginalIsNull = originalIsNull;
            ForwardIsNull = forwardIsNull;
            ForwardPresenceOwnerMod = forwardPresenceOwnerMod;
        }

        public object? GetForwardValue()
        {
            if (ForwardValueContexts == null) return null;

            var activeItems = ForwardValueContexts
                .Where(i => !i.IsRemoved)
                .Select(i => (object)i.Value!)
                .ToList();

            if (activeItems.Count > 0)
            {
                return activeItems;
            }

            return ForwardIsNull ? null : activeItems;
        }
    }
}
