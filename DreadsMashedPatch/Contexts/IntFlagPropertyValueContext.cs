namespace DreadsMashedPatch.Contexts
{
    public class IntFlagPropertyValueContext
    {
        public int Flag { get; set; }

        public string FlagName { get; set; }

        public bool IsSet { get; set; }

        public string OwnerMod { get; set; }

        public IntFlagPropertyValueContext(int flag, string flagName, bool isSet, string ownerMod)
        {
            Flag = flag;
            FlagName = flagName;
            IsSet = isSet;
            OwnerMod = ownerMod;
        }

        public override string ToString()
        {
            return $"{FlagName} (0x{Flag:X}) = {IsSet} (owned by {OwnerMod})";
        }
    }
}
