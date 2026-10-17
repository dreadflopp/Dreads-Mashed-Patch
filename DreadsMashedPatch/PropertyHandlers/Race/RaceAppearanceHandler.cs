using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.Race;

public sealed record RaceAppearanceValue(IGenderedItemGetter<IHeadDataGetter?>? HeadData,
    FormKey? MorphRace, ushort? NumberOfTintsInList, float FacegenMainClamp, float FacegenFaceClamp);

// Follow GenderedItemHandler's typed copying. Race's generated gendered copy bypasses
// translation masks and its wrapper equality is not a deep head-data comparison.
public sealed class RaceAppearanceHandler : AbstractPropertyHandler<RaceAppearanceValue>, IPropertyGroupHandler
{
    public IReadOnlySet<string> GroupedProperties { get; } = new HashSet<string>(StringComparer.Ordinal)
        { "HeadData", "MorphRace", "NumberOfTintsInList", "FacegenMainClamp", "FacegenFaceClamp" };

    public override string PropertyName => "Appearance";

    public override RaceAppearanceValue? GetValue(IMajorRecordGetter record) => record is IRaceGetter race
        ? new(CopyHeads(race.HeadData), race.MorphRace.FormKeyNullable, race.NumberOfTintsInList,
            race.FacegenMainClamp, race.FacegenFaceClamp)
        : throw new ArgumentException("Expected a RACE record.", nameof(record));

    public override void SetValue(IMajorRecord record, RaceAppearanceValue? value)
    {
        if (record is not IRace race || value is null)
            throw new ArgumentException("Expected a RACE appearance snapshot.");
        race.HeadData = CopyHeads(value.HeadData);
        race.MorphRace.SetTo(value.MorphRace);
        race.NumberOfTintsInList = value.NumberOfTintsInList;
        race.FacegenMainClamp = value.FacegenMainClamp;
        race.FacegenFaceClamp = value.FacegenFaceClamp;
    }

    private static GenderedItem<HeadData?>? CopyHeads(IGenderedItemGetter<IHeadDataGetter?>? heads) =>
        heads is null ? null : new(heads.Male?.DeepCopy(), heads.Female?.DeepCopy());

    private static bool HeadsEqual(IHeadDataGetter? left, IHeadDataGetter? right) =>
        left is null || right is null ? left is null && right is null : HeadDataMixIn.Equals(left, right);

    public override bool AreValuesEqual(RaceAppearanceValue? left, RaceAppearanceValue? right)
    {
        if (left is null || right is null) return left is null && right is null;
        var headsEqual = left.HeadData is null || right.HeadData is null
            ? left.HeadData is null && right.HeadData is null
            : HeadsEqual(left.HeadData.Male, right.HeadData.Male) && HeadsEqual(left.HeadData.Female, right.HeadData.Female);
        return headsEqual && left.MorphRace == right.MorphRace && left.NumberOfTintsInList == right.NumberOfTintsInList
            && left.FacegenMainClamp.Equals(right.FacegenMainClamp) && left.FacegenFaceClamp.Equals(right.FacegenFaceClamp);
    }

    public override string FormatValue(object? value) => value is RaceAppearanceValue race
        ? $"MorphRace={race.MorphRace}, Tints={race.NumberOfTintsInList}, " +
          $"MaleHeadData={race.HeadData?.Male is not null}, FemaleHeadData={race.HeadData?.Female is not null}, " +
          $"MainClamp={race.FacegenMainClamp}, FaceClamp={race.FacegenFaceClamp}"
        : "null";
}
