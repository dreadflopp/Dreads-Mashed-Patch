using DreadsMashedPatch.PropertyHandlers.Abstracts;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;

namespace DreadsMashedPatch.PropertyHandlers.Armor;

// Uses the shared icon copy/path comparer; only the gendered surface is record-specific.
public sealed class ArmorIconsHandler(MaleFemaleGender gender) : AbstractIconsHandler<IArmorGetter, IArmor>
{
    public override string PropertyName => $"WorldModel.{gender}.Icons";
    protected override IIconsGetter? GetIcons(IArmorGetter record) => record.WorldModel?[gender]?.Icons;
    protected override void SetIcons(IArmor record, Icons? value)
    {
        if (record.WorldModel?[gender] == null && value == null) return;
        record.WorldModel ??= new GenderedItem<ArmorModel?>(null, null);
        record.WorldModel[gender] ??= new ArmorModel();
        record.WorldModel[gender]!.Icons = value;
        if (value is null && record.WorldModel[gender]!.Model is null)
            record.WorldModel[gender] = null;
        if (record.WorldModel.Male is null && record.WorldModel.Female is null)
            record.WorldModel = null;
    }
}
