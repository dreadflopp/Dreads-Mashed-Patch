using System;
using System.Linq;
using DreadsMashedPatch.Contexts.Interfaces;

namespace DreadsMashedPatch.Contexts
{
    /// <summary>
    /// Represents the context for a flag property, managing collections of individual flag contexts.
    /// </summary>
    /// <typeparam name="TFlag">The flag enum type</typeparam>
    public class FlagPropertyContext<TFlag> : IPropertyContext where TFlag : struct, Enum
    {
        public List<FlagPropertyValueContext<TFlag>> OriginalFlagContexts { get; set; } = new();

        public List<FlagPropertyValueContext<TFlag>> ForwardFlagContexts { get; set; } = new();

        public bool IsResolved { get; set; } = false;

        public FlagPropertyContext()
        {
        }

        public FlagPropertyValueContext<TFlag>? GetFlagContext(TFlag flag, bool useForward = true)
        {
            var contexts = useForward ? ForwardFlagContexts : OriginalFlagContexts;
            return contexts.FirstOrDefault(fc => fc.Flag.Equals(flag));
        }

        public void SetFlagContext(TFlag flag, bool isSet, string ownerMod)
        {
            var existing = GetFlagContext(flag, true);
            if (existing != null)
            {
                existing.IsSet = isSet;
                existing.OwnerMod = ownerMod;
            }
            else
            {
                ForwardFlagContexts.Add(new FlagPropertyValueContext<TFlag>(flag, isSet, ownerMod));
            }
        }

        public void RemoveFlagContext(TFlag flag)
        {
            var existing = GetFlagContext(flag, true);
            if (existing != null)
            {
                ForwardFlagContexts.Remove(existing);
            }
        }

        public TFlag GetCombinedOriginalValue()
        {
            TFlag result = default;
            foreach (var context in OriginalFlagContexts.Where(fc => fc.IsSet))
            {
                result = CombineFlags(result, context.Flag);
            }
            return result;
        }

        private static TFlag CombineFlags(TFlag flags1, TFlag flags2)
        {
            // Use dynamic to handle the bitwise OR operation on enum types
            dynamic d1 = flags1;
            dynamic d2 = flags2;
            return (TFlag)(d1 | d2);
        }

        public object? GetForwardValue()
        {
            TFlag result = default;
            foreach (var context in ForwardFlagContexts)
            {
                if (context.IsSet)
                {
                    result = CombineFlags(result, context.Flag);
                }
            }
            return result;
        }
    }
}