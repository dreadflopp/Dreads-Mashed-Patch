using DreadsMashedPatch.Contexts.Interfaces;

namespace DreadsMashedPatch.Contexts
{
    public class SimplePropertyValueContext<T>(T? item, string ownerMod) : IPropertyValueContext<T>
    {
        public T? Value { get; set; } = item;
        public string OwnerMod { get; set; } = ownerMod;
    }
}