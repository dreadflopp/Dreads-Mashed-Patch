using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using DreadsMashedPatch.PropertyHandlers.Abstracts;

namespace DreadsMashedPatch.PropertyHandlers.Quest
{
    public class DialogConditionsHandler : AbstractConditionsHandler<IQuestGetter, IQuest>
    {
        protected override IEnumerable<IConditionGetter>? GetConditions(IQuestGetter record)
        {
            return record.DialogConditions;
        }

        protected override IEnumerable<IConditionGetter>? GetConditions(IQuest record)
        {
            return record.DialogConditions;
        }

        protected override void UpdateConditionsCollection(IQuest record, List<IConditionGetter> conditions)
        {
            if (record.DialogConditions != null)
            {
                record.DialogConditions.Clear();
                foreach (var condition in conditions)
                {
                    if (condition == null) continue;

                    if (condition is Condition concreteCondition)
                    {
                        record.DialogConditions.Add(concreteCondition);
                    }
                    else
                    {
                        var newCondition = condition.DeepCopy();
                        record.DialogConditions.Add(newCondition);
                    }
                }
            }
        }
    }
}
