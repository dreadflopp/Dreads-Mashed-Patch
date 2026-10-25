using DreadsMashedPatch.Contexts.Interfaces;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.Contexts;

public sealed class ModelBoundsPropertyContext : IPropertyContext
{
    public bool IsResolved { get; set; }
    public object? OriginalModel { get; set; }
    public object? ForwardModel { get; set; }
    public string ModelOwner { get; set; } = string.Empty;
    public object? SelectedFilenameIdentity { get; set; }
    public object? WinningFilenameIdentity { get; set; }
    public IObjectBoundsGetter? ModelBounds { get; set; }
    public IObjectBoundsGetter? WinningBounds { get; set; }

    public object GetForwardValue() => new ModelBoundsValue(ForwardModel,
        Equals(SelectedFilenameIdentity, WinningFilenameIdentity) ? WinningBounds : ModelBounds);
}

public sealed record ModelBoundsValue(object? Model, IObjectBoundsGetter? Bounds);
