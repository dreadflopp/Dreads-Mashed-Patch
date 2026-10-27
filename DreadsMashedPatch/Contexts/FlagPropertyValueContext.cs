using DreadsMashedPatch.Contexts.Interfaces;

namespace DreadsMashedPatch.Contexts
{
    public class FlagPropertyValueContext<TFlag> : IPropertyValueContext<TFlag>
    {
        public TFlag Flag { get; set; }

        public bool IsSet { get; set; }

        public string OwnerMod { get; set; }

        public TFlag Value => IsSet ? Flag : (TFlag)(object)0;

        public FlagPropertyValueContext(TFlag flag, bool isSet, string ownerMod)
        {
            Flag = flag;
            IsSet = isSet;
            OwnerMod = ownerMod;
        }

        public override string ToString()
        {
            return $"{Flag} = {IsSet} (owned by {OwnerMod})";
        }
    }
}