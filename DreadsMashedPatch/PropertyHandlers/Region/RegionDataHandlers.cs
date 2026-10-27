using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace DreadsMashedPatch.PropertyHandlers.Region;

/// <summary>Region entry structure is ordinary data; inherited icons and audio selectors have separate owners.</summary>
public static class RegionDataHandlers
{
    private static IPropertyHandler Aggregate<TGetter, TMutable>(string name,
        Func<TGetter, TMutable> copy, Func<TGetter, TGetter, bool> equals)
        where TGetter : class, IRegionDataGetter where TMutable : class, TGetter, IRegionData, new() =>
        new GeneratedCopyReflectionPropertyHandler<TGetter, TMutable, IRegion, IRegionGetter>(name, copy, equals);

    public static IPropertyHandler Body<TGetter, TMutable>(string name,
        Func<TGetter, TMutable> copy, Func<TGetter, TGetter, bool> equals)
        where TGetter : class, IRegionDataGetter where TMutable : class, TGetter, IRegionData, new() =>
        new ProjectedPropertyHandler<TGetter>(name, Aggregate(name, copy, equals), value =>
        {
            if (value == null) return null;
            var result = copy(value);
            result.Icons = null;
            if (result is IRegionSounds sounds) { sounds.Music.Clear(); sounds.Sounds = null; }
            return result;
        }, (value, destination) =>
        {
            if (value == null) return null;
            var result = copy(value);
            result.Icons = destination?.Icons?.DeepCopy();
            if (result is IRegionSounds sounds && destination is IRegionSoundsGetter old)
            {
                sounds.Music.SetTo(old.Music.FormKeyNullable);
                sounds.Sounds = old.Sounds == null ? null : new ExtendedList<RegionSound>(old.Sounds.Select(item => item.DeepCopy()));
            }
            return result;
        });

    public static IPropertyHandler Icons<TGetter, TMutable>(string name,
        Func<TGetter, TMutable> copy, Func<TGetter, TGetter, bool> equals)
        where TGetter : class, IRegionDataGetter where TMutable : class, TGetter, IRegionData, new() =>
        new NestedAssetPropertyHandler<TGetter>($"{name}.Icons", Aggregate(name, copy, equals), value =>
            value?.Icons == null ? null : new TMutable { Icons = value.Icons.DeepCopy() },
            (value, destination) =>
            {
                if (destination == null) return null;
                var result = copy(destination);
                result.Icons = value?.Icons?.DeepCopy();
                return result;
            }, (value, destination) => destination == null ? null : value, $"{name}.Icons");

    public static IPropertyHandler Audio(bool music)
    {
        var aggregate = Aggregate<IRegionSoundsGetter, RegionSounds>("Sounds", value => value.DeepCopy(), (a, b) => a.Equals(b));
        var name = music ? "Sounds.Music" : "Sounds.Sounds";
        return new NestedAssetPropertyHandler<IRegionSoundsGetter>(name, aggregate, value =>
        {
            if (value == null || (music ? value.Music.IsNull : value.Sounds == null)) return null;
            var result = new RegionSounds();
            CopyAudio(result, value, music);
            return result;
        }, (value, destination) =>
        {
            if (destination == null) return null;
            var result = destination.DeepCopy();
            CopyAudio(result, value, music);
            return result;
        }, (value, destination) => destination == null ? null : value, name);
    }

    private static void CopyAudio(IRegionSounds target, IRegionSoundsGetter? source, bool music)
    {
        if (music) target.Music.SetTo(source?.Music.FormKeyNullable);
        else target.Sounds = source?.Sounds == null ? null : new ExtendedList<RegionSound>(source.Sounds.Select(item => item.DeepCopy()));
    }
}
