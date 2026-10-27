using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using DreadsMashedPatch.PropertyHandlers.Abstracts;

namespace DreadsMashedPatch.PropertyHandlers.SoundDescriptor
{
    public class ConditionsHandler : AbstractConditionsHandler<ISoundDescriptorGetter, ISoundDescriptor>
    {
        protected override IEnumerable<IConditionGetter>? GetConditions(ISoundDescriptorGetter record)
        {
            return record.Conditions;
        }

        protected override IEnumerable<IConditionGetter>? GetConditions(ISoundDescriptor record)
        {
            return record.Conditions;
        }

        protected override void UpdateConditionsCollection(ISoundDescriptor record, List<IConditionGetter> conditions)
        {
            if (record.Conditions != null)
            {
                record.Conditions.Clear();
                foreach (var condition in conditions)
                {
                    if (condition == null) continue;

                    if (condition is Condition concreteCondition)
                    {
                        record.Conditions.Add(concreteCondition);
                    }
                    else
                    {
                        var newCondition = condition.DeepCopy();
                        record.Conditions.Add(newCondition);
                    }
                }
            }
        }
    }
}