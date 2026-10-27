using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Noggog;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class OutfitRecordHandlerTests
{
    private static readonly ModKey Source = ModKey.FromNameAndExtension("OutfitSource.esp");
    private static readonly FormKey First = new(Source, 0x801);
    private static readonly FormKey Second = new(Source, 0x802);

    [Fact]
    public void ItemsUseOneAtomicValueWithUnorderedMultiplicitySensitiveEquality()
    {
        var handler = Assert.IsType<AtomicFormLinkListPropertyHandler<IOutfitTargetGetter, IOutfit, IOutfitGetter>>(
            new OutfitRecordHandler().PropertyHandlers["Items"]);

        Assert.True(handler.AreValuesEqual([First, Second, First], [First, First, Second]));
        Assert.False(handler.AreValuesEqual([First, Second, First], [First, Second, Second]));
        Assert.False(handler.AreValuesEqual([First, Second, First], [First, Second]));
        Assert.False(handler.AreValuesEqual(null, []));
    }

    [Fact]
    public void SetterCreatesAbsentListAndCopiesLinksInSelectedSourceOrder()
    {
        var handler = new OutfitRecordHandler().PropertyHandlers["Items"];
        var source = new Outfit(new FormKey(Source, 0x810), SkyrimRelease.SkyrimSE)
        {
            Items = new ExtendedList<IFormLinkGetter<IOutfitTargetGetter>>
            {
                new FormLink<IOutfitTargetGetter>(Second),
                new FormLink<IOutfitTargetGetter>(First),
                new FormLink<IOutfitTargetGetter>(Second)
            }
        };
        var target = new Outfit(new FormKey(Source, 0x811), SkyrimRelease.SkyrimSE);
        Assert.Null(target.Items);

        var value = handler.GetValue(source);
        handler.SetValue(target, value);

        var copied = target.Items!;
        var sourceItems = source.Items!;
        Assert.Equal([Second, First, Second], copied.Select(link => link.FormKey));
        Assert.NotSame(sourceItems, copied);
        Assert.NotSame(sourceItems[0], copied[0]);
        sourceItems.Clear();
        Assert.Equal([Second, First, Second], Assert.IsType<List<FormKey>>(value));
        Assert.Equal([Second, First, Second], copied.Select(link => link.FormKey));
    }

    [Fact]
    public void SetterPreservesSelectedNullAndPresentEmptyValuesInMemory()
    {
        var handler = new OutfitRecordHandler().PropertyHandlers["Items"];
        var target = new Outfit(new FormKey(Source, 0x810), SkyrimRelease.SkyrimSE);

        handler.SetValue(target, new List<FormKey>());
        Assert.NotNull(target.Items);
        Assert.Empty(target.Items);
        Assert.Empty(Assert.IsType<List<FormKey>>(handler.GetValue(target)));
        handler.SetValue(target, new List<FormKey> { First });
        handler.SetValue(target, null);
        Assert.Null(target.Items);
        Assert.Null(handler.GetValue(target));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void SourceOverlaysAndPatchOutputRetainCompletePackageAndInamPresence(int kind)
    {
        var handler = new OutfitRecordHandler().PropertyHandlers["Items"];
        var source = new SkyrimMod(Source, SkyrimRelease.SkyrimSE);
        var sourceRecord = new Outfit(new FormKey(Source, 0x810), SkyrimRelease.SkyrimSE);
        handler.SetValue(sourceRecord, kind switch
        {
            0 => null,
            1 => new List<FormKey>(),
            _ => new List<FormKey> { Second, First, Second }
        });
        source.Outfits.Add(sourceRecord);
        using var sourceStream = new MemoryStream();
        source.WriteToBinary(sourceStream);
        sourceStream.Position = 0;
        using var sourceOverlay = SkyrimMod.CreateFromBinaryOverlay(sourceStream, SkyrimRelease.SkyrimSE, Source);
        var overlayRecord = Assert.Single(sourceOverlay.Outfits);

        var patchKey = ModKey.FromNameAndExtension("OutfitPatch.esp");
        var patch = new SkyrimMod(patchKey, SkyrimRelease.SkyrimSE);
        var target = new Outfit(new FormKey(patchKey, 0x810), SkyrimRelease.SkyrimSE);
        handler.SetValue(target, new List<FormKey> { First });
        handler.SetValue(target, handler.GetValue(overlayRecord));
        patch.Outfits.Add(target);
        using var patchStream = new MemoryStream();
        patch.WriteToBinary(patchStream);
        patchStream.Position = 0;
        using var patchOverlay = SkyrimMod.CreateFromBinaryOverlay(patchStream, SkyrimRelease.SkyrimSE, patchKey);
        var written = Assert.Single(patchOverlay.Outfits);

        if (kind == 0)
        {
            Assert.Null(overlayRecord.Items);
            Assert.Null(written.Items);
        }
        else if (kind == 1)
        {
            Assert.NotNull(overlayRecord.Items);
            Assert.Empty(overlayRecord.Items);
            Assert.NotNull(written.Items);
            Assert.Empty(written.Items);
        }
        else
        {
            Assert.Equal([Second, First, Second], written.Items!.Select(link => link.FormKey));
        }
    }
}
