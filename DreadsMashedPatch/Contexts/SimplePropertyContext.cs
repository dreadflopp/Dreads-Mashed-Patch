using DreadsMashedPatch.Contexts.Interfaces;

namespace DreadsMashedPatch.Contexts
{
    public class SimplePropertyContext<T> : IPropertyContext
    {
        public bool IsResolved { get; set; }
        public SimplePropertyValueContext<T>? OriginalValueContext { get; set; }
        public SimplePropertyValueContext<T>? ForwardValueContext { get; set; }

        public SimplePropertyContext()
        {
            OriginalValueContext = null;
            ForwardValueContext = null;
        }

        public SimplePropertyContext(SimplePropertyValueContext<T>? originalValueContext, SimplePropertyValueContext<T>? forwardValueContext)
        {
            OriginalValueContext = originalValueContext;
            ForwardValueContext = forwardValueContext;
        }

        public object? GetForwardValue()
        {
            return ForwardValueContext != null ? (object?)ForwardValueContext.Value : null;
        }
    }
}