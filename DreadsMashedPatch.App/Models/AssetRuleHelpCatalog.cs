using DreadsMashedPatch.AssetForwarding;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.TextureSet;
using DreadsMashedPatch.RecordHandlers;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.App.Models;

public sealed record AssetRuleHelpEntry(
    string GroupName,
    string RecordTypes,
    string Properties,
    string Notes,
    IReadOnlyList<Type> RecordHandlers,
    IReadOnlyList<string> Members);

/// <summary>Tree help and retained metadata for future controls; follows runtime ownership without selecting values.</summary>
public static class AssetRuleHelpCatalog
{
    public static IReadOnlyDictionary<string, IReadOnlyList<AssetRuleHelpEntry>> Families { get; } = Build();

    private static IReadOnlyDictionary<string, IReadOnlyList<AssetRuleHelpEntry>> Build()
    {
        var entries = new List<(ForwardingRule Family, AssetRuleHelpEntry Entry)>();
        foreach (var type in typeof(AbstractRecordHandler).Assembly.GetTypes()
                     .Where(type => !type.IsAbstract && typeof(AbstractRecordHandler).IsAssignableFrom(type)))
        {
            var groups = AssetGroupCatalog.For(type);
            if (groups.Count == 0) continue;
            var handler = (AbstractRecordHandler)System.Activator.CreateInstance(type)!;
            foreach (var group in groups)
            {
                var notes = group.Members.Contains("ModelAndBounds") || group.Members.Contains("WorldModelAndBounds")
                    ? "Matching model filenames keep the last plugin's bounds. A different filename carries bounds from the selected model's source."
                    : string.Empty;
                if (group.Family == ForwardingRule.Scripts)
                    notes = group.Members.Count > 1
                        ? "VMAD scripts, metadata and fragment bindings stay together. Quest and scene structure use their existing ownership and validation rules independently of this switch."
                        : handler.PropertyHandlers[group.Members[0]] is AbstractScriptListPropertyHandler
                            ? "Merge complete script entries by name; retain the destination VMAD version and object format."
                            : "Select the complete VMAD adapter, including its metadata and fragment data where present.";
                if (group.Family == ForwardingRule.Animations)
                    notes = group.Name switch
                    {
                        "MaleRig" or "FemaleRig" => "Select one gender's skeleton and behavior graph together. The other gender, attacks and movement tuning have their own decisions.",
                        "BodyPartReference" => "Select the body-part definition reference independently of rig paths. Its node names still need a compatible skeleton.",
                        "IdleReference" => "Select the behavior filename and animation event together. Conditions and legacy DATA fields have separate decisions.",
                        "IdleSelection" when type == typeof(IdleMarkerRecordHandler) => "Select ordered idle references, flags and timer together. The marker mesh uses the mesh-reference rule.",
                        "IdleSelection" => "Select the complete idle package, including selection type, timer and ordered idle references. Script attachments use their own rule.",
                        "AnimationChoice" => "Select animation gender independently of the appearance group.",
                        _ => notes
                    };
                if (group.Family == ForwardingRule.Trees)
                    notes += " One source supplies mesh references, wind data, script attachments and supported record flags. Other resource and appearance groups forward normally. Script copying retains destination VMAD metadata; name and harvesting keep their own rules.";
                if (type == typeof(RegionRecordHandler))
                    notes = "Only the listed references are controlled here. Region entry presence, flags, priority, grass placement and other content follow their normal rules. Removed entries are never restored by a nested reference.";
                entries.Add((group.Family, new AssetRuleHelpEntry(group.Name, RecordLabel(type),
                    DescribeProperties(handler, group.Members), notes.Trim(), [type], group.Members)));
            }
        }

        return Enum.GetValues<ForwardingRule>().ToDictionary(family => family.ToString(), family =>
            (IReadOnlyList<AssetRuleHelpEntry>)entries.Where(entry => entry.Family == family)
                // Identical groups share their record list; different payloads and policies remain separate.
                .GroupBy(entry => (entry.Entry.GroupName, Members: string.Join("\n", entry.Entry.Members),
                    entry.Entry.Properties, entry.Entry.Notes))
                .Select(group => new AssetRuleHelpEntry(group.Key.GroupName,
                    string.Join(", ", group.Select(entry => entry.Entry.RecordTypes).Order(StringComparer.Ordinal)),
                    group.Key.Properties, group.Key.Notes,
                    group.SelectMany(entry => entry.Entry.RecordHandlers).ToArray(), group.First().Entry.Members))
                .OrderBy(entry => entry.GroupName, StringComparer.Ordinal)
                .ThenBy(entry => entry.RecordTypes, StringComparer.Ordinal).ToArray(), StringComparer.Ordinal);
    }

    private static string RecordLabel(Type handlerType)
    {
        var name = handlerType.Name.Replace("RecordHandler", string.Empty, StringComparison.Ordinal);
        // The record handler is singular; Mutagen's INFO getter is plural.
        if (name == "DialogResponse") name = "DialogResponses";
        var getter = typeof(INpcGetter).Assembly.GetType($"Mutagen.Bethesda.Skyrim.I{name}Getter")
            ?? throw new InvalidOperationException($"No record getter for asset help: {handlerType.Name}");
        return $"{RecordTypeCatalog.GetSignature(getter)} — {RecordTypeCatalog.GetDisplayName(getter)}";
    }

    private static string DescribeProperties(AbstractRecordHandler handler, IReadOnlyList<string> members) =>
        string.Join("; ", members.Select(member => DescribeMember(member, handler.PropertyHandlers[member])));

    private static string DescribeMember(string member, IPropertyHandler handler)
    {
        if (member == "FlareSprites")
            return "FlareSprites (Sprites: LensFlareSpriteId, Texture, Data: Tint RGB, Width, Height, Position, AngularFade, Opacity, Flags)";
        if (handler is IPropertyGroupHandler grouped)
            return $"{member} ({string.Join(", ", grouped.GroupedProperties.Order(StringComparer.Ordinal))})";
        return member switch
        {
            "ModelAndBounds" => "ModelAndBounds (Model.File, Model.AlternateTextures, Model.Data, ObjectBounds)",
            "WorldModelAndBounds" => "WorldModelAndBounds (male/female WorldModel meshes and alternate textures, ObjectBounds)",
            "Icons" => "Icons (LargeIconFilename, SmallIconFilename)",
            "SkeletalModel.Male" or "SkeletalModel.Female" => $"{member} (File, Data)",
            "BehaviorGraph.Male" or "BehaviorGraph.Female" => $"{member} (File, Data, AlternateTextures)",
            "IdleAnimations" => "IdleAnimations (Type, TimerSetting, Animations: ordered IDLE references)",
            "Animations" when handler is AtomicReflectionListPropertyHandler<Mutagen.Bethesda.Plugins.IFormLinkGetter<IIdleAnimationGetter>, IIdleMarker, IIdleMarkerGetter> => "Animations (ordered IDLE references)",
            "TextureDefinition" => $"TextureDefinition ({string.Join(", ", typeof(TextureDefinition).GetProperties().Select(property => property.Name))})",
            "VirtualMachineAdapter" when handler is AbstractScriptListPropertyHandler => "VirtualMachineAdapter (VMAD script attachments and complete script properties)",
            "VirtualMachineAdapter" => "VirtualMachineAdapter (complete VMAD adapter, scripts, properties and any fragments)",
            _ => member
        };
    }
}
