using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.Npc;

public sealed record NpcTemplateTraitsValue(IFormLinkNullableGetter<INpcSpawnGetter>? Template,
    NpcConfiguration.TemplateFlag Traits);

// Select the template pointer and its appearance-inheritance bit as one value,
// using the same ownership and reversion permissions as other atomic properties.
public sealed class TemplateAndTraitsHandler : AbstractPropertyHandler<NpcTemplateTraitsValue>, IPropertyGroupHandler
{
    private readonly SimpleReflectionFormLinkPropertyHandler<INpcSpawnGetter, INpc, INpcGetter> _template = new("Template");
    private readonly SimpleReflectionFlagPropertyHandler<NpcConfiguration.TemplateFlag, INpc, INpcGetter> _traits =
        new("Configuration.TemplateFlags", preserveUnknownBits: true, includedFlags: [NpcConfiguration.TemplateFlag.Traits]);

    public override string PropertyName => "TemplateAndTraits";
    public IReadOnlySet<string> GroupedProperties { get; } = new HashSet<string>(StringComparer.Ordinal)
        { "Template", "Configuration.TemplateFlags.Traits" };

    private static IFormLinkNullableGetter<INpcSpawnGetter>? CopyLink(IFormLinkNullableGetter<INpcSpawnGetter>? link) =>
        link is null ? null : new FormLinkNullable<INpcSpawnGetter>(link.FormKeyNullable);

    public override NpcTemplateTraitsValue GetValue(IMajorRecordGetter record) =>
        new(CopyLink(_template.GetValue(record)), _traits.GetValue(record) & NpcConfiguration.TemplateFlag.Traits);

    public override void SetValue(IMajorRecord record, NpcTemplateTraitsValue? value)
    {
        if (value is null) throw new ArgumentException("Expected an NPC template/Traits value.");
        _template.SetValue(record, value.Template);
        _traits.SetValue(record, value.Traits);
    }

    public override bool AreValuesEqual(NpcTemplateTraitsValue? left, NpcTemplateTraitsValue? right) =>
        left is null || right is null ? left is null && right is null
            : _template.AreValuesEqual(left.Template, right.Template) && _traits.AreValuesEqual(left.Traits, right.Traits);

    public override string FormatValue(object? value) => value is NpcTemplateTraitsValue template
        ? $"Template={_template.FormatValue(template.Template)}, InheritTraits={template.Traits.HasFlag(NpcConfiguration.TemplateFlag.Traits)}"
        : "null";

}
