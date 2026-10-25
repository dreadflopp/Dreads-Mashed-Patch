using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.Contexts.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Abstracts;

namespace DreadsMashedPatch.PropertyHandlers.General
{
    public class ObjectBoundsHandler : AbstractPropertyHandler<IObjectBoundsGetter>
    {
        public override string PropertyName => "ObjectBounds";

        // Standalone bounds have no model filename selection to justify replacing the winner.
        // The shared forward pass visits the complete history from original to winning.
        public override void UpdatePropertyContext(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> recordContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
            IPropertyContext propertyContext)
        {
            if (propertyContext is not SimplePropertyContext<IObjectBoundsGetter> context)
                throw new InvalidOperationException($"Expected a simple property context for {PropertyName}");

            context.ForwardValueContext = new SimplePropertyValueContext<IObjectBoundsGetter>(
                GetValue(recordContext.Record), recordContext.ModKey.ToString());
        }

        public override void SetValue(IMajorRecord record, IObjectBoundsGetter? value)
        {
            if (record is IObjectBounded objectBoundedRecord)
            {
                if (value == null)
                {
                    // ObjectBounds cannot be null, create a default object bounds
                    var defaultObjectBounds = new ObjectBounds();
                    objectBoundedRecord.ObjectBounds = defaultObjectBounds;
                }
                else
                {
                    // Create a deep copy of the object bounds
                    var newObjectBounds = new ObjectBounds();
                    newObjectBounds.First = value.First;
                    newObjectBounds.Second = value.Second;
                    objectBoundedRecord.ObjectBounds = newObjectBounds;
                }
            }
            else if (record is IObjectBoundedOptional optionalRecord)
            {
                optionalRecord.ObjectBounds = value == null
                    ? null
                    : new ObjectBounds
                    {
                        First = value.First,
                        Second = value.Second
                    };
            }
            else
            {
                LogCollector.AddError(PropertyName, $"Error: Record does not implement IObjectBounded for {PropertyName}");
            }
        }

        public override IObjectBoundsGetter? GetValue(IMajorRecordGetter record)
        {
            if (record is IObjectBoundedGetter objectBoundedRecord)
            {
                return objectBoundedRecord.ObjectBounds;
            }
            else if (record is IObjectBoundedOptionalGetter optionalRecord)
            {
                return optionalRecord.ObjectBounds;
            }
            else
            {
                LogCollector.AddError(PropertyName, $"Error: Record does not implement IObjectBoundedGetter for {PropertyName}");
            }
            return null;
        }

        public override bool AreValuesEqual(IObjectBoundsGetter? value1, IObjectBoundsGetter? value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null) return false;

            // Compare the First and Second properties
            return value1.First.Equals(value2.First) && value1.Second.Equals(value2.Second);
        }

        public override string FormatValue(object? value)
        {
            if (value is IObjectBoundsGetter bounds)
            {
                return $"ObjectBounds(First: {bounds.First}, Second: {bounds.Second})";
            }
            return value?.ToString() ?? "ObjectBounds()";
        }
    }
}
