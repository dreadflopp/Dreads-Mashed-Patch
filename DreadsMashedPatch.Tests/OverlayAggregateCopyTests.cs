using System.Drawing;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

[Collection("LogCollector")]
public sealed class OverlayAggregateCopyTests
{
    private static readonly ModKey SourceModKey = ModKey.FromNameAndExtension("AggregateSource.esp");
    private static readonly ModKey PatchModKey = ModKey.FromNameAndExtension("AggregatePatch.esp");

    [Fact]
    public void PlacedNpcActivateParentsCopiesOverlayRowsWithoutWarnings()
    {
        using var stream = CreatePlacedNpcPlugin();
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, SourceModKey);
        var source = Assert.Single(overlay.EnumerateMajorRecords<IPlacedNpcGetter>());
        var target = new PlacedNpc(new FormKey(PatchModKey, 0xA03), SkyrimRelease.SkyrimSE);
        var handler = Assert.IsType<GeneratedCopyReflectionPropertyHandler<
            IActivateParentsGetter,
            ActivateParents,
            IPlacedNpc,
            IPlacedNpcGetter>>(new PlacedNpcRecordHandler().PropertyHandlers["ActivateParents"]);
        LogCollector.Clear();

        handler.SetValue(target, handler.GetValue(source));

        var copied = Assert.Single(target.ActivateParents!.Parents);
        Assert.Equal(new FormKey(SourceModKey, 0xA02), copied.Reference.FormKey);
        Assert.Equal(1.25f, copied.Delay);
        Assert.Empty(LogCollector.GetAll());
        LogCollector.Clear();
    }

    [Fact]
    public void WeatherColorCopiesOverlayWithoutReflectingOverTimeOfDayIndexer()
    {
        using var stream = CreateWeatherPlugin();
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, SourceModKey);
        var source = Assert.Single(overlay.Weathers);
        var target = new Weather(new FormKey(PatchModKey, 0xA11), SkyrimRelease.SkyrimSE);
        var handler = Assert.IsType<GeneratedCopyReflectionPropertyHandler<
            IWeatherColorGetter,
            WeatherColor,
            IWeather,
            IWeatherGetter>>(new WeatherRecordHandler().PropertyHandlers["SkyUpperColor"]);
        LogCollector.Clear();

        handler.SetValue(target, handler.GetValue(source));

        Assert.Equal(Color.FromArgb(255, 10, 20, 30), target.SkyUpperColor.Sunrise);
        Assert.Equal(Color.FromArgb(255, 40, 50, 60), target.SkyUpperColor.Day);
        Assert.Empty(LogCollector.GetAll());
        LogCollector.Clear();
    }

    [Fact]
    public void PackageIdleAnimationsCopiesOverlayListWithoutWarnings()
    {
        using var stream = CreatePackagePlugin();
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, SourceModKey);
        var source = Assert.Single(overlay.Packages);
        var target = new Package(new FormKey(PatchModKey, 0xA22), SkyrimRelease.SkyrimSE);
        var handler = Assert.IsType<GeneratedCopyReflectionPropertyHandler<
            IPackageIdlesGetter,
            PackageIdles,
            IPackage,
            IPackageGetter>>(new PackageRecordHandler().PropertyHandlers["IdleAnimations"]);
        LogCollector.Clear();

        handler.SetValue(target, handler.GetValue(source));

        Assert.Equal(PackageIdles.Types.Random, target.IdleAnimations!.Type);
        Assert.Equal(2.5f, target.IdleAnimations.TimerSetting);
        Assert.Equal(new FormKey(SourceModKey, 0xA21), Assert.Single(target.IdleAnimations.Animations).FormKey);
        Assert.Empty(LogCollector.GetAll());
        LogCollector.Clear();
    }

    [Fact]
    public void MagicEffectForwardValuesFromOverlayApplyWithoutInvalidCast()
    {
        using var stream = CreateMagicEffectPlugin();
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, SourceModKey);
        var source = Assert.Single(overlay.MagicEffects);
        var target = new MagicEffect(new FormKey(PatchModKey, 0xA32), SkyrimRelease.SkyrimSE);
        var recordHandler = new MagicEffectRecordHandler();
        var forwardedNames = new[] { "Description", "CastingSoundLevel", "MenuDisplayObject", "Archetype", "Sounds" };
        var forwarded = forwardedNames.ToDictionary(
            name => name,
            name => recordHandler.PropertyHandlers[name].GetValue(source));
        LogCollector.Clear();

        recordHandler.ApplyForwardedProperties(target, forwarded);

        Assert.Equal("Overlay description", target.Description?.String);
        Assert.Equal(SoundLevel.Silent, target.CastingSoundLevel);
        Assert.Equal(new FormKey(SourceModKey, 0xA31), target.MenuDisplayObject.FormKey);
        Assert.IsType<MagicEffectPeakValueModArchetype>(target.Archetype);
        Assert.Equal(ActorValue.HeavyArmorModifier, target.Archetype.ActorValue);
        var copiedSound = Assert.Single(Assert.IsAssignableFrom<IEnumerable<MagicEffectSound>>(target.Sounds));
        Assert.IsType<MagicEffectSound>(copiedSound);
        Assert.Equal(MagicEffect.SoundType.Release, copiedSound.Type);
        Assert.Equal(new FormKey(SourceModKey, 0xA33), copiedSound.Sound.FormKey);
        Assert.Empty(LogCollector.GetAll());
        LogCollector.Clear();
    }

    [Fact]
    public void NavigationMeshDataCopiesOverlayGeometryListsWithoutWarnings()
    {
        using var stream = CreateNavigationMeshPlugin();
        using var overlay = SkyrimMod.CreateFromBinaryOverlay(stream, SkyrimRelease.SkyrimSE, SourceModKey);
        var source = Assert.Single(overlay.EnumerateMajorRecords<INavigationMeshGetter>());
        var target = new NavigationMesh(new FormKey(PatchModKey, 0xA43), SkyrimRelease.SkyrimSE);
        var handler = Assert.IsType<GeneratedCopyReflectionPropertyHandler<
            INavigationMeshDataGetter,
            NavigationMeshData,
            INavigationMesh,
            INavigationMeshGetter>>(new NavigationMeshRecordHandler().PropertyHandlers["Data"]);
        LogCollector.Clear();

        handler.SetValue(target, handler.GetValue(source));

        Assert.Equal(3, target.Data!.Vertices.Count);
        Assert.Single(target.Data.Triangles);
        Assert.Single(target.Data.EdgeLinks);
        Assert.Single(target.Data.DoorTriangles);
        Assert.Empty(LogCollector.GetAll());
        LogCollector.Clear();
    }

    [Fact]
    public void TranslatedStringHandlerPreservesAllLanguages()
    {
        var source = new TranslatedString(
            Language.English,
            new Dictionary<Language, string>
            {
                [Language.English] = "English text",
                [Language.German] = "Deutscher Text"
            });
        var target = new MagicEffect(new FormKey(PatchModKey, 0xA50), SkyrimRelease.SkyrimSE);
        var handler = new TranslatedStringReflectionPropertyHandler<IMagicEffect, IMagicEffectGetter>("Description");

        handler.SetValue(target, source);

        Assert.NotSame(source, target.Description);
        Assert.True(target.Description!.TryLookup(Language.English, out var english));
        Assert.True(target.Description.TryLookup(Language.German, out var german));
        Assert.Equal("English text", english);
        Assert.Equal("Deutscher Text", german);
    }

    private static MemoryStream CreatePlacedNpcPlugin()
    {
        var source = new SkyrimMod(SourceModKey, SkyrimRelease.SkyrimSE);
        var placed = new PlacedNpc(new FormKey(SourceModKey, 0xA01), SkyrimRelease.SkyrimSE)
        {
            ActivateParents = new ActivateParents
            {
                Flags = ActivateParents.Flag.ParentActivateOnly
            }
        };
        placed.ActivateParents.Parents.Add(new ActivateParent
        {
            Reference = new FormLink<IPlacedGetter>(new FormKey(SourceModKey, 0xA02)),
            Delay = 1.25f
        });
        var cell = new Cell(new FormKey(SourceModKey, 0xA00), SkyrimRelease.SkyrimSE);
        cell.Temporary.Add(placed);
        var subBlock = new CellSubBlock();
        subBlock.Cells.Add(cell);
        var block = new CellBlock();
        block.SubBlocks.Add(subBlock);
        source.Cells.Records.Add(block);

        var stream = new MemoryStream();
        source.WriteToBinary(stream);
        stream.Position = 0;
        return stream;
    }

    private static MemoryStream CreateWeatherPlugin()
    {
        var source = new SkyrimMod(SourceModKey, SkyrimRelease.SkyrimSE);
        var weather = new Weather(new FormKey(SourceModKey, 0xA10), SkyrimRelease.SkyrimSE);
        weather.SkyUpperColor.Sunrise = Color.FromArgb(255, 10, 20, 30);
        weather.SkyUpperColor.Day = Color.FromArgb(255, 40, 50, 60);
        source.Weathers.Add(weather);

        var stream = new MemoryStream();
        source.WriteToBinary(stream);
        stream.Position = 0;
        return stream;
    }

    private static MemoryStream CreatePackagePlugin()
    {
        var source = new SkyrimMod(SourceModKey, SkyrimRelease.SkyrimSE);
        var package = new Package(new FormKey(SourceModKey, 0xA20), SkyrimRelease.SkyrimSE)
        {
            IdleAnimations = new PackageIdles
            {
                Type = PackageIdles.Types.Random,
                TimerSetting = 2.5f
            }
        };
        package.IdleAnimations.Animations.Add(
            new FormLink<IIdleAnimationGetter>(new FormKey(SourceModKey, 0xA21)));
        source.Packages.Add(package);

        var stream = new MemoryStream();
        source.WriteToBinary(stream);
        stream.Position = 0;
        return stream;
    }

    private static MemoryStream CreateMagicEffectPlugin()
    {
        var source = new SkyrimMod(SourceModKey, SkyrimRelease.SkyrimSE);
        var effect = new MagicEffect(new FormKey(SourceModKey, 0xA30), SkyrimRelease.SkyrimSE)
        {
            Description = new TranslatedString(Language.English) { String = "Overlay description" },
            CastingSoundLevel = SoundLevel.Silent,
            Archetype = new MagicEffectPeakValueModArchetype { ActorValue = ActorValue.HeavyArmorModifier }
        };
        effect.MenuDisplayObject.SetTo(new FormKey(SourceModKey, 0xA31));
        effect.Sounds =
        [
            new MagicEffectSound
            {
                Type = MagicEffect.SoundType.Release,
                Sound = new FormLink<ISoundDescriptorGetter>(new FormKey(SourceModKey, 0xA33))
            }
        ];
        source.MagicEffects.Add(effect);

        var stream = new MemoryStream();
        source.WriteToBinary(stream);
        stream.Position = 0;
        return stream;
    }

    private static MemoryStream CreateNavigationMeshPlugin()
    {
        var source = new SkyrimMod(SourceModKey, SkyrimRelease.SkyrimSE);
        var cellKey = new FormKey(SourceModKey, 0xA40);
        var meshKey = new FormKey(SourceModKey, 0xA41);
        var cell = new Cell(cellKey, SkyrimRelease.SkyrimSE);
        var mesh = new NavigationMesh(meshKey, SkyrimRelease.SkyrimSE)
        {
            Data = new NavigationMeshData
            {
                NavmeshVersion = 12,
                CrcHash = 1234,
                NavmeshGridDivisor = 1
            }
        };
        Assert.IsType<CellNavmeshParent>(mesh.Data.Parent).Parent.SetTo(cellKey);
        mesh.Data.Vertices.Add(new P3Float(0, 0, 0));
        mesh.Data.Vertices.Add(new P3Float(1, 0, 0));
        mesh.Data.Vertices.Add(new P3Float(0, 1, 0));
        mesh.Data.Triangles.Add(new NavmeshTriangle { Vertices = new P3Int16(0, 1, 2) });
        var edgeLink = new EdgeLink { TriangleIndex = 0 };
        edgeLink.Mesh.SetTo(meshKey);
        mesh.Data.EdgeLinks.Add(edgeLink);
        var doorTriangle = new DoorTriangle { TriangleBeforeDoor = 0 };
        doorTriangle.Door.SetTo(new FormKey(SourceModKey, 0xA42));
        mesh.Data.DoorTriangles.Add(doorTriangle);
        cell.NavigationMeshes.Add(mesh);

        var subBlock = new CellSubBlock();
        subBlock.Cells.Add(cell);
        var block = new CellBlock();
        block.SubBlocks.Add(subBlock);
        source.Cells.Records.Add(block);

        var stream = new MemoryStream();
        source.WriteToBinary(stream);
        stream.Position = 0;
        return stream;
    }
}
