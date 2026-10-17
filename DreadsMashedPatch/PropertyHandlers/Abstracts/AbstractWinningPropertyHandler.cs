using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.Contexts.Interfaces;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace DreadsMashedPatch.PropertyHandlers.Abstracts;

/// <summary>
/// Keeps each complete authored snapshot, including a return to the original value.
/// Used for appearance data which must follow the effective winning override.
/// </summary>
public abstract class AbstractWinningPropertyHandler<T> : AbstractPropertyHandler<T>
{
    public override void UpdatePropertyContext(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> context,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
        IPropertyContext propertyContext)
    {
        if (propertyContext is not SimplePropertyContext<T> valueContext)
            throw new InvalidOperationException($"Expected a simple context for {PropertyName}.");

        valueContext.ForwardValueContext = new(GetValue(context.Record), context.ModKey.ToString());
        LogCollector.Add(PropertyName, $"[{PropertyName}] Complete appearance snapshot selected from {context.ModKey}");
    }
}
