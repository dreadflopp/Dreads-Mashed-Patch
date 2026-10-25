using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.Contexts.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Armor;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace DreadsMashedPatch.PropertyHandlers.General;

/// <summary>
/// Keeps winning bounds when the selected model filenames match the winner. Selecting different
/// filenames takes the bounds supplied with that model; bounds are never merged independently.
/// </summary>
public sealed class ModelBoundsHandler : IPropertyHandler
{
    private readonly IPropertyHandler _modelHandler;
    private readonly ObjectBoundsHandler _boundsHandler = new();
    private readonly Func<object?, object?> _filenameIdentity;

    public ModelBoundsHandler()
        : this(new ModelHandler(), GetModelFilenameIdentity)
    {
    }

    private ModelBoundsHandler(IPropertyHandler modelHandler, Func<object?, object?> filenameIdentity)
    {
        _modelHandler = modelHandler;
        _filenameIdentity = filenameIdentity;
    }

    public static ModelBoundsHandler ForArmorWorldModel() =>
        new(new WorldModelHandler(), GetArmorWorldModelFilenameIdentity);

    public string PropertyName => $"{_modelHandler.PropertyName}AndBounds";

    public object GetValue(IMajorRecordGetter record) =>
        new ModelBoundsValue(_modelHandler.GetValue(record), _boundsHandler.GetValue(record));

    public void SetValue(IMajorRecord record, object? value)
    {
        if (value is not ModelBoundsValue combined)
        {
            throw new ArgumentException($"Expected {nameof(ModelBoundsValue)}", nameof(value));
        }

        _modelHandler.SetValue(record, combined.Model);
        _boundsHandler.SetValue(record, combined.Bounds);
    }

    public bool AreValuesEqual(object? value1, object? value2)
    {
        if (value1 is not ModelBoundsValue left || value2 is not ModelBoundsValue right)
        {
            return value1 == null && value2 == null;
        }

        return _modelHandler.AreValuesEqual(left.Model, right.Model)
            && _boundsHandler.AreValuesEqual(left.Bounds, right.Bounds);
    }

    public IPropertyContext CreatePropertyContext() => new ModelBoundsPropertyContext();

    public void InitializeContext(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> originalContext,
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPropertyContext propertyContext)
    {
        var context = RequireContext(propertyContext);
        var original = (ModelBoundsValue)GetValue(originalContext.Record);
        var winning = (ModelBoundsValue)GetValue(winningContext.Record);
        context.OriginalModel = original.Model;
        context.ForwardModel = original.Model;
        context.ModelOwner = originalContext.ModKey.ToString();
        context.SelectedFilenameIdentity = _filenameIdentity(original.Model);
        context.WinningFilenameIdentity = _filenameIdentity(winning.Model);
        context.ModelBounds = original.Bounds;
        context.WinningBounds = winning.Bounds;
        context.IsResolved = false;
    }

    public void UpdatePropertyContext(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> recordContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
        IPropertyContext propertyContext)
    {
        var context = RequireContext(propertyContext);
        var current = (ModelBoundsValue)GetValue(recordContext.Record);
        var filenameIdentity = _filenameIdentity(current.Model);
        var filenameChanged = !Equals(context.SelectedFilenameIdentity, filenameIdentity);

        var modelAccepted = TryUpdateValue(
            current.Model,
            context.OriginalModel,
            context.ForwardModel,
            context.ModelOwner,
            recordContext,
            state,
            _modelHandler.AreValuesEqual,
            out var nextModel,
            out var nextModelOwner);

        if (modelAccepted)
        {
            context.ForwardModel = nextModel;
            context.ModelOwner = nextModelOwner;
        }

        if (modelAccepted && filenameChanged)
        {
            context.SelectedFilenameIdentity = filenameIdentity;
            context.ModelBounds = current.Bounds;
            LogCollector.Add(PropertyName,
                $"[{PropertyName}] {recordContext.ModKey}: Model filenames changed; source bounds follow when the selected filenames differ from the winner");
        }
    }

    public string FormatValue(object? value)
    {
        if (value is not ModelBoundsValue combined) return value?.ToString() ?? "null";
        return $"Model=({_modelHandler.FormatValue(combined.Model)}), Bounds=({_boundsHandler.FormatValue(combined.Bounds)})";
    }

    private static bool TryUpdateValue(
        object? current,
        object? original,
        object? forward,
        string forwardOwner,
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> recordContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
        Func<object?, object?, bool> equals,
        out object? next,
        out string nextOwner)
    {
        next = forward;
        nextOwner = forwardOwner;

        if (!equals(current, original) && !equals(current, forward))
        {
            next = current;
            nextOwner = recordContext.ModKey.ToString();
            return true;
        }

        if (!equals(current, original) || equals(current, forward))
        {
            return false;
        }

        var mod = state.LoadOrder[recordContext.ModKey].Mod;
        if (mod == null || !PatcherSettings.HasMasterOrVirtualMaster(mod, forwardOwner))
        {
            return false;
        }

        next = current;
        nextOwner = recordContext.ModKey.ToString();
        return true;
    }

    private static object? GetModelFilenameIdentity(object? value) => value is IModelGetter model
        ? AssetPathHelper.NormalizeForComparison(model.File.GivenPath).ToUpperInvariant()
        : null;

    private static object GetArmorWorldModelFilenameIdentity(object? value)
    {
        if (value is not IGenderedItemGetter<IArmorModelGetter?> models)
        {
            return new ValueTuple<string?, string?>(null, null);
        }

        return (
            Normalize(models.Male?.Model?.File.GivenPath),
            Normalize(models.Female?.Model?.File.GivenPath));
    }

    private static string? Normalize(string? path) => path == null
        ? null
        : AssetPathHelper.NormalizeForComparison(path).ToUpperInvariant();

    private static ModelBoundsPropertyContext RequireContext(IPropertyContext context) =>
        context as ModelBoundsPropertyContext
        ?? throw new InvalidOperationException($"Expected {nameof(ModelBoundsPropertyContext)}");
}
