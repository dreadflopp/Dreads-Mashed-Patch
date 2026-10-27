using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.Race;

public enum RaceHeadDataPart { Shape, Mesh, FaceTextures, Tints }

/// <summary>Copies one gender's head-data fields through Mutagen's typed mask,
/// preserving fields selected by other forwarding rules.</summary>
public sealed class RaceHeadDataHandler : AbstractPropertyHandler<IHeadDataGetter>, IPropertyGroupHandler
{
    private readonly MaleFemaleGender _gender;
    private readonly HeadData.TranslationMask _mask;
    public override string PropertyName { get; }
    public IReadOnlySet<string> GroupedProperties { get; }

    public RaceHeadDataHandler(MaleFemaleGender gender, RaceHeadDataPart part)
    {
        _gender = gender;
        PropertyName = $"HeadData.{gender}.{part}";
        _mask = new(false);
        string[] fields;
        switch (part)
        {
            case RaceHeadDataPart.Shape:
                _mask.HeadParts = true;
                _mask.AvailableMorphs = true;
                fields = ["HeadParts", "AvailableMorphs"];
                break;
            case RaceHeadDataPart.Mesh:
                _mask.Model = true;
                fields = ["Model"];
                break;
            case RaceHeadDataPart.FaceTextures:
                _mask.FaceDetails = true;
                _mask.DefaultFaceTexture = true;
                fields = ["FaceDetails", "DefaultFaceTexture"];
                break;
            case RaceHeadDataPart.Tints:
                _mask.TintMasks = true;
                fields = ["TintMasks"];
                break;
            default: throw new ArgumentOutOfRangeException(nameof(part));
        }
        GroupedProperties = fields.Select(field => $"HeadData.{gender}.{field}").ToHashSet(StringComparer.Ordinal);
    }

    // Presence is tracked separately. Empty fields compare equally whether their
    // optional parent exists or not, so parent edits do not take resource ownership.
    public override IHeadDataGetter GetValue(IMajorRecordGetter record) =>
        record is IRaceGetter race
            ? race.HeadData?[_gender]?.DeepCopy(_mask) ?? new HeadData()
            : throw new ArgumentException("Expected a RACE record.", nameof(record));

    public override void SetValue(IMajorRecord record, IHeadDataGetter? value)
    {
        if (record is not IRace race) throw new ArgumentException("Expected a RACE record.", nameof(record));
        var selected = value ?? new HeadData();
        if (race.HeadData?[_gender] is null && AreValuesEqual(selected, new HeadData())) return;
        var target = RaceHeadDataPresenceHandler.EnsureHead(race, _gender);
        // DeepCopyIn alone can share nested objects; materialize a detached snapshot first.
        target.DeepCopyIn(selected.DeepCopy(_mask), _mask);
    }

    public override bool AreValuesEqual(IHeadDataGetter? left, IHeadDataGetter? right) =>
        HeadDataMixIn.Equals(left ?? new HeadData(), right ?? new HeadData(), _mask);
}

public sealed record RaceHeadDataPresence(bool Present, bool Male, bool Female);

/// <summary>Like QUEST VMAD presence, the parent decision gates all child writes.</summary>
public sealed class RaceHeadDataPresenceHandler : AbstractPropertyHandler<RaceHeadDataPresence>
{
    public override string PropertyName => "HeadData.Presence";
    public override RaceHeadDataPresence GetValue(IMajorRecordGetter record) => record is IRaceGetter race
        ? new(race.HeadData is not null, race.HeadData?.Male is not null, race.HeadData?.Female is not null)
        : throw new ArgumentException("Expected a RACE record.", nameof(record));

    public override void SetValue(IMajorRecord record, RaceHeadDataPresence? value)
    {
        if (record is not IRace race || value is null) throw new ArgumentException("Expected RACE head presence.");
        if (!value.Present) { race.HeadData = null; return; }
        race.HeadData ??= new GenderedItem<HeadData?>(null, null);
        race.HeadData.Male = value.Male ? race.HeadData.Male ?? new HeadData() : null;
        race.HeadData.Female = value.Female ? race.HeadData.Female ?? new HeadData() : null;
    }

    internal static HeadData EnsureHead(IRace race, MaleFemaleGender gender)
    {
        race.HeadData ??= new GenderedItem<HeadData?>(null, null);
        return race.HeadData[gender] ??= new HeadData();
    }
}
