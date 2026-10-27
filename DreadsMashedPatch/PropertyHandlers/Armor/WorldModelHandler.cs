using DreadsMashedPatch.PropertyHandlers.Abstracts;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using DreadsMashedPatch.PropertyHandlers.General;

namespace DreadsMashedPatch.PropertyHandlers.Armor
{
    /// <summary>
    /// Handles ARMO's gendered ArmorModel aggregate, including meshes only; icons have independent forwarding owners.
    /// </summary>
    public sealed class WorldModelHandler : AbstractPropertyHandler<IGenderedItemGetter<IArmorModelGetter?>?>
    {
        public override string PropertyName => "WorldModel";

        public override IGenderedItemGetter<IArmorModelGetter?>? GetValue(IMajorRecordGetter record)
        {
            if (record is not IArmorGetter armor) return null;
            var male = CopyMesh(armor.WorldModel?.Male);
            var female = CopyMesh(armor.WorldModel?.Female);
            return male == null && female == null ? null : new GenderedItem<ArmorModel?>(male, female);
        }

        public override void SetValue(IMajorRecord record, IGenderedItemGetter<IArmorModelGetter?>? value)
        {
            if (record is not IArmor armor)
            {
                LogCollector.AddError(PropertyName, $"Error: Record does not implement IArmor for {PropertyName}");
                return;
            }

            var male = WithIcons(value?.Male, armor.WorldModel?.Male?.Icons);
            var female = WithIcons(value?.Female, armor.WorldModel?.Female?.Icons);
            armor.WorldModel = male == null && female == null ? null : new GenderedItem<ArmorModel?>(male, female);
        }

        public override bool AreValuesEqual(
            IGenderedItemGetter<IArmorModelGetter?>? value1,
            IGenderedItemGetter<IArmorModelGetter?>? value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null) return false;

            return ModelsEqual(value1.Male, value2.Male)
                && ModelsEqual(value1.Female, value2.Female);
        }

        public override string FormatValue(object? value)
        {
            if (value is not IGenderedItemGetter<IArmorModelGetter?> gendered)
            {
                return value?.ToString() ?? "null";
            }

            return $"Male: {FormatModel(gendered.Male)}, Female: {FormatModel(gendered.Female)}";
        }

        private static ArmorModel? CopyMesh(IArmorModelGetter? value) => value?.Model == null
            ? null : new ArmorModel { Model = value.Model.DeepCopy() };

        private static ArmorModel? WithIcons(IArmorModelGetter? value, IIconsGetter? icons) =>
            value?.Model == null && icons == null ? null
                : new ArmorModel { Model = value?.Model?.DeepCopy(), Icons = icons?.DeepCopy() };

        private static bool ModelsEqual(IArmorModelGetter? value1, IArmorModelGetter? value2)
        {
            if (value1 == null && value2 == null) return true;
            if (value1 == null || value2 == null) return false;
            return value1.Equals(value2);
        }

        private static string FormatModel(IArmorModelGetter? value)
        {
            if (value == null) return "null";

            var modelFile = AssetPathHelper.Format(value.Model?.File);
            return $"Model(File: {modelFile})";
        }
    }
}
