using System.Linq;
using DreadsMashedPatch.Contexts.Interfaces;

namespace DreadsMashedPatch.Contexts
{
    /// <summary>
    /// Represents the context for an integer flag property, managing collections of individual flag contexts.
    /// </summary>
    public class IntFlagPropertyContext : IPropertyContext
    {
        public List<IntFlagPropertyValueContext> OriginalFlagContexts { get; set; } = new();

        public List<IntFlagPropertyValueContext> ForwardFlagContexts { get; set; } = new();

        public bool IsResolved { get; set; } = false;

        public IntFlagPropertyContext()
        {
        }

        public IntFlagPropertyValueContext? GetFlagContext(int flag, bool useForward = true)
        {
            var contexts = useForward ? ForwardFlagContexts : OriginalFlagContexts;
            return contexts.FirstOrDefault(fc => fc.Flag == flag);
        }

        public int GetCombinedOriginalValue()
        {
            int result = 0;
            foreach (var context in OriginalFlagContexts.Where(fc => fc.IsSet))
            {
                result |= context.Flag;
            }
            return result;
        }

        public object? GetForwardValue()
        {
            int result = 0;
            foreach (var context in ForwardFlagContexts)
            {
                if (context.IsSet)
                {
                    result |= context.Flag;
                }
            }
            return result;
        }
    }
}
