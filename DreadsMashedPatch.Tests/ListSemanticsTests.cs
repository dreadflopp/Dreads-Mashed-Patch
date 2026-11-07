using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.RecordHandlers;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class ListSemanticsTests
{
    [Fact]
    public void XEditSortKeyComparerUsesCompositeKeysAndLoadOrderFormIds()
    {
        var early = ModKey.FromNameAndExtension("Early.esm");
        var late = ModKey.FromNameAndExtension("Late.esp");
        var comparer = new XEditSortKeyComparer(new Dictionary<ModKey, int>
        {
            [early] = 0,
            [late] = 1
        });

        Assert.True(comparer.Compare([1, "a"], [1, "B"]) < 0);
        Assert.True(comparer.Compare(
            [new FormKey(early, 0xFFFFFF)],
            [new FormKey(late, 1)]) < 0);
        Assert.True(comparer.Compare([1], [1, 0]) < 0);
    }

    [Fact]
    public void GeneratedListCompositeIdentityUsesStructuralXEditKeyEquality()
    {
        Assert.True(GeneratedCopyReflectionListPropertyHandler<
            IBodyPartGetter,
            BodyPart,
            IBodyPartData,
            IBodyPartDataGetter>.AreKeysEqual(
                new object?[] { "NPC Head", 3 },
                new object?[] { "npc head", 3 }));
        Assert.False(GeneratedCopyReflectionListPropertyHandler<
            IBodyPartGetter,
            BodyPart,
            IBodyPartData,
            IBodyPartDataGetter>.AreKeysEqual(
                new object?[] { "NPC Head", 3 },
                new object?[] { "NPC Head", 4 }));
    }

    [Fact]
    public void PlacedObjectLinkedReferencesTreatReversedRowsAsDifferent()
    {
        var handler = new PlacedObjectRecordHandler().PropertyHandlers["LinkedReferences"];
        var first = LinkedReference(0x0E8500, 0x000B0D);
        var second = LinkedReference(0x0D5B87, 0x000AB6);

        Assert.True(handler.AreValuesEqual(
            new List<ILinkedReferencesGetter> { first, second },
            new List<ILinkedReferencesGetter> { first.DeepCopy(), second.DeepCopy() }));
        Assert.False(handler.AreValuesEqual(
            new List<ILinkedReferencesGetter> { first, second },
            new List<ILinkedReferencesGetter> { second.DeepCopy(), first.DeepCopy() }));
    }

    [Fact]
    public void WeatherSoundsWithTheSameTypeRetainPositionalIdentity()
    {
        var handler = new WeatherRecordHandler().PropertyHandlers["Sounds"];
        var first = CreateWeatherSound(0x100);
        var second = CreateWeatherSound(0x200);

        Assert.False(handler.AreValuesEqual(
            new List<IWeatherSoundGetter> { first, second },
            new List<IWeatherSoundGetter> { second.DeepCopy(), first.DeepCopy() }));
    }

    [Fact]
    public void MagicEffectSoundsRetainDeclarationOrderAndCompletePairIdentity()
    {
        var handler = new MagicEffectRecordHandler().PropertyHandlers["Sounds"];
        var loop = new MagicEffectSound
        {
            Type = MagicEffect.SoundType.ConcentrationCastLoop,
            Sound = new FormLink<ISoundDescriptorGetter>(new FormKey(
                ModKey.FromNameAndExtension("Skyrim.esm"), 0x03E5CB))
        };
        var sheath = loop.DeepCopy();
        sheath.Type = MagicEffect.SoundType.SheathDraw;
        var changedSound = loop.DeepCopy();
        changedSound.Sound.SetTo(new FormKey(ModKey.FromNameAndExtension("Skyrim.esm"), 0x03E5E3));

        Assert.True(handler.AreValuesEqual(
            new List<IMagicEffectSoundGetter> { loop, sheath },
            new List<IMagicEffectSoundGetter> { loop.DeepCopy(), sheath.DeepCopy() }));
        Assert.False(handler.AreValuesEqual(
            new List<IMagicEffectSoundGetter> { loop, sheath },
            new List<IMagicEffectSoundGetter> { sheath, loop }));
        Assert.False(handler.AreValuesEqual(
            new List<IMagicEffectSoundGetter> { loop },
            new List<IMagicEffectSoundGetter> { changedSound }));
    }

    [Fact]
    public void PlacedNpcLinkedReferencesRemainSortedAndOrderIndependent()
    {
        var handler = new PlacedNpcRecordHandler().PropertyHandlers["LinkedReferences"];
        var first = LinkedReference(0x0E8500, 0x000B0D);
        var second = LinkedReference(0x0D5B87, 0x000AB6);

        Assert.True(handler.AreValuesEqual(
            new List<ILinkedReferencesGetter> { first, second },
            new List<ILinkedReferencesGetter> { second.DeepCopy(), first.DeepCopy() }));
    }

    [Fact]
    public void PlacedObjectLinkedReferencesWriterPreservesDeclaredOrder()
    {
        var handler = new PlacedObjectRecordHandler().PropertyHandlers["LinkedReferences"];
        var first = LinkedReference(0x0E8500, 0x000B0D);
        var second = LinkedReference(0x0D5B87, 0x000AB6);
        var target = new PlacedObject(
            new FormKey(ModKey.FromNameAndExtension("Patch.esp"), 0x800),
            SkyrimRelease.SkyrimSE);

        handler.SetValue(
            target,
            new List<ILinkedReferencesGetter> { first, second });

        Assert.Equal(
            new[] { first.KeywordOrReference.FormKey, second.KeywordOrReference.FormKey },
            target.LinkedReferences.Select(entry => entry.KeywordOrReference.FormKey));
    }

    [Fact]
    public void BodyPartRowsAreSortedByPartNodeRatherThanPosition()
    {
        var handler = new BodyPartDataRecordHandler().PropertyHandlers["Parts"];
        var head = new BodyPart { PartNode = "NPC Head [Head]", DamageMult = 1f };
        var torso = new BodyPart { PartNode = "NPC Spine [Spn0]", DamageMult = 2f };
        var editedHead = head.DeepCopy();
        editedHead.DamageMult = 3f;

        Assert.True(handler.AreValuesEqual(
            new List<IBodyPartGetter> { head, torso },
            new List<IBodyPartGetter> { torso.DeepCopy(), head.DeepCopy() }));
        Assert.False(handler.AreValuesEqual(
            new List<IBodyPartGetter> { head },
            new List<IBodyPartGetter> { editedHead }));
    }

    private static LinkedReferences LinkedReference(uint keywordId, uint referenceId)
    {
        var skyrim = ModKey.FromNameAndExtension("Skyrim.esm");
        return new LinkedReferences
        {
            KeywordOrReference = new FormLink<IKeywordLinkedReferenceGetter>(new FormKey(skyrim, keywordId)),
            Reference = new FormLink<IPlacedGetter>(new FormKey(skyrim, referenceId))
        };
    }

    private static WeatherSound CreateWeatherSound(uint soundId)
    {
        var dawnguard = ModKey.FromNameAndExtension("Dawnguard.esm");
        return new WeatherSound
        {
            Sound = new FormLink<ISoundGetter>(new FormKey(dawnguard, soundId)),
            Type = 0
        };
    }
}
