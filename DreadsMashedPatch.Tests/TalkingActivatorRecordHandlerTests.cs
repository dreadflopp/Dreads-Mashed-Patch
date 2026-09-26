using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Strings;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class TalkingActivatorRecordHandlerTests
{
    private static readonly ModKey TestModKey = ModKey.FromNameAndExtension("TalkingActivatorTests.esp");

    [Fact]
    public void UsesReviewedOwnershipAndCollectionSemantics()
    {
        var handlers = new TalkingActivatorRecordHandler().PropertyHandlers;

        Assert.IsType<MajorRecordFlagsRawHandler>(handlers["MajorRecordFlagsRaw"]);
        Assert.DoesNotContain("SkyrimMajorRecordFlags", handlers.Keys);
        Assert.DoesNotContain("MajorFlags", handlers.Keys);
        Assert.DoesNotContain("PNAM", handlers.Keys);
        Assert.DoesNotContain("FNAM", handlers.Keys);

        Assert.IsType<TranslatedStringReflectionPropertyHandler<ITalkingActivator, ITalkingActivatorGetter>>(
            handlers["Name"]);
        Assert.IsType<ModelBoundsHandler>(handlers["ModelAndBounds"]);
        Assert.IsType<GeneratedCopyReflectionPropertyHandler<
            IDestructibleGetter,
            Destructible,
            ITalkingActivator,
            ITalkingActivatorGetter>>(handlers["Destructible"]);

        var keywords = Assert.IsType<KeywordListHandler>(handlers["Keywords"]);
        Assert.Equal(ListSemantics.SortedKeyed, keywords.Semantics);

        var scripts = Assert.IsType<SimpleReflectionVirtualMachineAdapterHandler<
            ITalkingActivator,
            ITalkingActivatorGetter>>(handlers["VirtualMachineAdapter"]);
        Assert.Equal(ListSemantics.SortedKeyed, scripts.Semantics);

        Assert.IsType<SimpleReflectionFormLinkPropertyHandler<
            ISoundMarkerGetter,
            ITalkingActivator,
            ITalkingActivatorGetter>>(handlers["LoopingSound"]);
        Assert.IsType<SimpleReflectionFormLinkPropertyHandler<
            IVoiceTypeGetter,
            ITalkingActivator,
            ITalkingActivatorGetter>>(handlers["Voice"]);
    }

    [Fact]
    public void CompositeHeaderHandlerOwnsCommonAndTalkingActivatorFlagsOnlyOnce()
    {
        var handler = new TalkingActivatorRecordHandler();
        const int unownedBit = 0x01000000;
        var record = new TalkingActivator(new FormKey(TestModKey, 0x800), SkyrimRelease.SkyrimSE)
        {
            MajorRecordFlagsRaw = unownedBit
                | (int)TalkingActivator.MajorFlag.RadioStation
                | (int)SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled
        };

        handler.ApplyForwardedProperties(record, new Dictionary<string, object?>
        {
            ["MajorRecordFlagsRaw"] = (int)SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled
        });

        Assert.Equal(
            unownedBit | (int)SkyrimMajorRecord.SkyrimMajorRecordFlag.InitiallyDisabled,
            record.MajorRecordFlagsRaw);
        Assert.False(record.MajorFlags.HasFlag(TalkingActivator.MajorFlag.RadioStation));
    }

    [Fact]
    public void LocalizedNameAndDestructibleAggregateAreDeepCopied()
    {
        var handlers = new TalkingActivatorRecordHandler().PropertyHandlers;
        var record = new TalkingActivator(new FormKey(TestModKey, 0x801), SkyrimRelease.SkyrimSE);
        var name = new TranslatedString(
            Language.English,
            new Dictionary<Language, string>
            {
                [Language.English] = "Talking statue",
                [Language.German] = "Sprechende Statue"
            });
        var destructible = new Destructible
        {
            Data = new DestructableData
            {
                Health = 125,
                DESTCount = 1
            }
        };

        handlers["Name"].SetValue(record, name);
        handlers["Destructible"].SetValue(record, destructible);

        Assert.NotSame(name, record.Name);
        Assert.True(record.Name!.TryLookup(Language.English, out var english));
        Assert.True(record.Name.TryLookup(Language.German, out var german));
        Assert.Equal("Talking statue", english);
        Assert.Equal("Sprechende Statue", german);
        Assert.NotSame(destructible, record.Destructible);
        Assert.Equal(125, record.Destructible!.Data!.Health);
        Assert.Equal((byte)1, record.Destructible.Data.DESTCount);
    }
}
