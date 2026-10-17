using DreadsMashedPatch.Contexts;
using DreadsMashedPatch.Contexts.Interfaces;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace DreadsMashedPatch.PropertyHandlers.Npc;

public sealed record NpcTemplateTraitsValue(IFormLinkNullableGetter<INpcSpawnGetter>? Template,
    NpcConfiguration.TemplateFlag Traits);

// Like ModelBoundsHandler: reuse the ordinary accessor/ownership policy, with one
// narrow dependency rule. Appearance inheritance must use the winning template;
// templates used only by gameplay can continue through ordinary forwarding.
public sealed class TemplateAndTraitsHandler : IPropertyHandler<NpcTemplateTraitsValue>, IPropertyGroupHandler
{
    private readonly SimpleReflectionFormLinkPropertyHandler<INpcSpawnGetter, INpc, INpcGetter> _template = new("Template");
    private readonly SimpleReflectionFlagPropertyHandler<NpcConfiguration.TemplateFlag, INpc, INpcGetter> _traits =
        new("Configuration.TemplateFlags", preserveUnknownBits: true, includedFlags: [NpcConfiguration.TemplateFlag.Traits]);

    public string PropertyName => "TemplateAndTraits";
    public IReadOnlySet<string> GroupedProperties { get; } = new HashSet<string>(StringComparer.Ordinal)
        { "Template", "Configuration.TemplateFlags.Traits" };

    private static IFormLinkNullableGetter<INpcSpawnGetter>? CopyLink(IFormLinkNullableGetter<INpcSpawnGetter>? link) =>
        link is null ? null : new FormLinkNullable<INpcSpawnGetter>(link.FormKeyNullable);

    public NpcTemplateTraitsValue GetValue(IMajorRecordGetter record) =>
        new(CopyLink(_template.GetValue(record)), _traits.GetValue(record) & NpcConfiguration.TemplateFlag.Traits);

    public void SetValue(IMajorRecord record, NpcTemplateTraitsValue? value)
    {
        if (value is null) throw new ArgumentException("Expected an NPC template/Traits value.");
        _template.SetValue(record, value.Template);
        _traits.SetValue(record, value.Traits);
    }

    public bool AreValuesEqual(NpcTemplateTraitsValue? left, NpcTemplateTraitsValue? right) =>
        left is null || right is null ? left is null && right is null
            : _template.AreValuesEqual(left.Template, right.Template) && _traits.AreValuesEqual(left.Traits, right.Traits);

    public IPropertyContext CreatePropertyContext() => new TemplateContext(((IPropertyHandler)_template).CreatePropertyContext());

    public void InitializeContext(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> originalContext,
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPropertyContext propertyContext)
    {
        var context = (TemplateContext)propertyContext;
        _template.InitializeContext(originalContext, winningContext, context.Template);
        context.Traits = GetValue(originalContext.Record).Traits;
        context.IsResolved = false;
    }

    public void UpdatePropertyContext(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> recordContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
        IPropertyContext propertyContext)
    {
        var context = (TemplateContext)propertyContext;
        var current = GetValue(recordContext.Record);
        context.Traits = current.Traits;
        if (current.Traits.HasFlag(NpcConfiguration.TemplateFlag.Traits))
        {
            var templateContext = (SimplePropertyContext<IFormLinkNullableGetter<INpcSpawnGetter>>)context.Template;
            templateContext.ForwardValueContext = new(current.Template, recordContext.ModKey.ToString());
            LogCollector.Add(PropertyName, $"[{PropertyName}] {recordContext.ModKey}: Traits inherits the complete winning template.");
        }
        else
        {
            _template.UpdatePropertyContext(recordContext, state, context.Template);
        }
    }

    public string FormatValue(object? value) => value is NpcTemplateTraitsValue template
        ? $"Template={_template.FormatValue(template.Template)}, InheritTraits={template.Traits.HasFlag(NpcConfiguration.TemplateFlag.Traits)}"
        : "null";

    object? IPropertyHandler.GetValue(IMajorRecordGetter record) => GetValue(record);
    void IPropertyHandler.SetValue(IMajorRecord record, object? value) => SetValue(record, (NpcTemplateTraitsValue?)value);
    bool IPropertyHandler.AreValuesEqual(object? left, object? right) =>
        AreValuesEqual((NpcTemplateTraitsValue?)left, (NpcTemplateTraitsValue?)right);

    private sealed class TemplateContext(IPropertyContext template) : IPropertyContext
    {
        public IPropertyContext Template { get; } = template;
        public NpcConfiguration.TemplateFlag Traits { get; set; }
        public bool IsResolved { get; set; }
        public object GetForwardValue() => new NpcTemplateTraitsValue(
            CopyLink((IFormLinkNullableGetter<INpcSpawnGetter>?)Template.GetForwardValue()), Traits);
    }
}
