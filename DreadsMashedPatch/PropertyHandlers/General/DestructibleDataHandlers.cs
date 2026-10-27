using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.General;

/// <summary>Destruction structure/gameplay remains ordinary; only ordinal stage models use the mesh rule.</summary>
public static class DestructibleDataHandlers
{
    // The generated parent copy does not apply a nested stage mask. Project explicit
    // detached stages instead, as with RACE's nested head-data copy boundary.
    private static Destructible? ProjectData(IDestructibleGetter? value)
    {
        var result = value?.DeepCopy();
        if (result != null) foreach (var stage in result.Stages) stage.Model = null;
        return result;
    }

    public static IPropertyHandler Data(IPropertyHandler<IDestructibleGetter?> aggregate) =>
        new ProjectedPropertyHandler<IDestructibleGetter>("Destructible", aggregate,
            value => ProjectData(value), (value, destination) =>
            {
                if (value == null) return null;
                var result = ProjectData(value)!;
                for (var i = 0; i < result.Stages.Count; i++)
                    result.Stages[i].Model = i < (destination?.Stages.Count ?? 0) ? destination!.Stages[i].Model?.DeepCopy() : null;
                return result;
            });

    public static IPropertyHandler Meshes(IPropertyHandler<IDestructibleGetter?> aggregate) =>
        new NestedAssetPropertyHandler<IDestructibleGetter>("Destructible.StageModels", aggregate,
            ProjectModels, (value, destination) =>
            {
                if (destination == null) return null;
                var result = destination.DeepCopy();
                for (var i = 0; i < result.Stages.Count; i++)
                    result.Stages[i].Model = i < (value?.Stages.Count ?? 0) ? value!.Stages[i].Model?.DeepCopy() : null;
                return result;
            }, (value, destination) => ProjectModels(value, destination?.Stages.Count ?? 0), "Destructible.Stages.Model");

    private static IDestructibleGetter? ProjectModels(IDestructibleGetter? source) => ProjectModels(source, int.MaxValue);
    private static IDestructibleGetter? ProjectModels(IDestructibleGetter? source, int count)
    {
        if (source == null) return null;
        var last = Math.Min(source.Stages.Count, count) - 1;
        while (last >= 0 && source.Stages[last].Model == null) last--;
        if (last < 0) return null;
        var result = new Destructible();
        for (var i = 0; i <= last; i++) result.Stages.Add(new DestructionStage { Model = source.Stages[i].Model?.DeepCopy() });
        return result;
    }
}
