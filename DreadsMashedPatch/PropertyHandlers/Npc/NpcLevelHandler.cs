using DreadsMashedPatch.PropertyHandlers.Abstracts;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.Npc;

/// <summary>
/// Handles the polymorphic NPC level union without reflecting through Configuration.
/// Mutagen's base-interface DeepCopy preserves fixed-level and player-level subtypes.
/// </summary>
public sealed class NpcLevelHandler : AbstractPropertyHandler<IANpcLevelGetter>
{
    public override string PropertyName => "Configuration.Level";

    public override IANpcLevelGetter? GetValue(IMajorRecordGetter record)
        => (record as INpcGetter)?.Configuration.Level;

    public override void SetValue(IMajorRecord record, IANpcLevelGetter? value)
    {
        if (record is INpc npc && value != null)
        {
            npc.Configuration.Level = value.DeepCopy();
        }
    }

    public override bool AreValuesEqual(IANpcLevelGetter? value1, IANpcLevelGetter? value2)
    {
        if (value1 == null || value2 == null)
        {
            return value1 == null && value2 == null;
        }

        return ANpcLevelMixIn.Equals(value1, value2);
    }
}
