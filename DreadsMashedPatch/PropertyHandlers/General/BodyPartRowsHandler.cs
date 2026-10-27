using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.PropertyHandlers.General;

/// <summary>Keep keyed row reconciliation; limb mesh fields have an independent resource owner.</summary>
public sealed class BodyPartRowsHandler() : GeneratedCopyReflectionListPropertyHandler<IBodyPartGetter, BodyPart, IBodyPartData, IBodyPartDataGetter>(
    "Parts", ListSemantics.SortedKeyed, value => value.DeepCopy(), BodyPartMixIn.Equals, keySelector: value => value.PartNode)
{
    private static readonly BodyPart.TranslationMask MeshMask = new(false)
    { PartNode = true, LimbReplacementModel = true, LimbReplacementScale = true, TextureFilesHashes = true };
    private static readonly BodyPart.TranslationMask DataMask = new(true)
    { LimbReplacementModel = false, LimbReplacementScale = false, TextureFilesHashes = false };

    public override List<IBodyPartGetter>? GetValue(IMajorRecordGetter record) =>
        base.GetValue(record)?.Select(value => (IBodyPartGetter)value.DeepCopy(DataMask)).ToList();

    public override void SetValue(IMajorRecord record, List<IBodyPartGetter>? value) =>
        base.SetValue(record, Merge(value, base.GetValue(record), mesh: false));

    public static IPropertyHandler Meshes()
    {
        var aggregate = new GeneratedCopyReflectionListPropertyHandler<IBodyPartGetter, BodyPart, IBodyPartData, IBodyPartDataGetter>(
            "Parts", ListSemantics.SortedKeyed, value => value.DeepCopy(), BodyPartMixIn.Equals, keySelector: value => value.PartNode);
        return new NestedAssetPropertyHandler<List<IBodyPartGetter>>("Parts.LimbMeshes", aggregate, ProjectMeshes,
            (value, destination) => Merge(destination, value, mesh: true),
            (value, destination) => ProjectMeshes(Merge(destination, value, mesh: true)),
            "Parts.LimbReplacementModel", "Parts.LimbReplacementScale", "Parts.TextureFilesHashes");
    }

    private static List<IBodyPartGetter> ProjectMeshes(List<IBodyPartGetter>? value) =>
        (value ?? []).GroupBy(row => row.PartNode, StringComparer.OrdinalIgnoreCase).SelectMany(group =>
        {
            var rows = group.ToList();
            var last = rows.FindLastIndex(row => row.LimbReplacementModel.Length != 0 || row.LimbReplacementScale != 0 || row.TextureFilesHashes != null);
            return rows.Take(last + 1).Select(row => (IBodyPartGetter)row.DeepCopy(MeshMask));
        }).ToList();

    // Match duplicate PartNodes by occurrence, without dropping rows or making keys case-sensitive.
    private static List<IBodyPartGetter> Merge(List<IBodyPartGetter>? structure, List<IBodyPartGetter>? assets, bool mesh)
    {
        var byNode = (assets ?? []).GroupBy(row => row.PartNode, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(group => group.Key, group => new Queue<IBodyPartGetter>(group), StringComparer.OrdinalIgnoreCase);
        return (structure ?? []).Select(row =>
        {
            var result = row.DeepCopy(mesh ? null : DataMask);
            var resource = byNode.TryGetValue(row.PartNode, out var matches) && matches.TryDequeue(out var found) ? found : null;
            result.LimbReplacementModel = resource?.LimbReplacementModel ?? string.Empty;
            result.LimbReplacementScale = resource?.LimbReplacementScale ?? 0;
            result.TextureFilesHashes = resource?.TextureFilesHashes?.ToArray();
            return (IBodyPartGetter)result;
        }).ToList();
    }
}
