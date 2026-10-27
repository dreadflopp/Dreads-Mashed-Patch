using DreadsMashedPatch.PropertyHandlers.Abstracts;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Noggog;

namespace DreadsMashedPatch.PropertyHandlers.Race;

/// <summary>Uses the ordinary sorted FormLink-list policy for race head choices.</summary>
public sealed class RaceHeadChoicesHandler<TTarget>(MaleFemaleGender gender, string fieldName,
    Func<IHeadDataGetter, IReadOnlyList<IFormLinkGetter<TTarget>>> getter,
    Func<HeadData, ExtendedList<IFormLinkGetter<TTarget>>> setter)
    : AbstractListPropertyHandler<IFormLinkGetter<TTarget>> where TTarget : class, IMajorRecordGetter
{
    public override string PropertyName => $"HeadData.{gender}.{fieldName}";
    public override ListSemantics Semantics => ListSemantics.SortedKeyed;
    public override List<IFormLinkGetter<TTarget>> GetValue(IMajorRecordGetter record) =>
        record is IRaceGetter race && race.HeadData?[gender] is { } head ? getter(head).ToList() : [];

    public override void SetValue(IMajorRecord record, List<IFormLinkGetter<TTarget>>? value)
    {
        if (record is not IRace race) throw new ArgumentException("Expected a RACE record.", nameof(record));
        if (race.HeadData?[gender] is null && (value is null || value.Count == 0)) return;
        var list = setter(RaceHeadDataPresenceHandler.EnsureHead(race, gender));
        list.Clear();
        if (value is not null) list.AddRange(value.Select(link => new FormLink<TTarget>(link.FormKey)));
    }

    protected override IReadOnlyList<object?> GetSortKey(IFormLinkGetter<TTarget> item) => [item.FormKey];
    protected override bool IsItemEqual(IFormLinkGetter<TTarget>? left, IFormLinkGetter<TTarget>? right) =>
        left?.FormKey == right?.FormKey;
}
