using System.Text.Json;
using DreadsMashedPatch;
using DreadsMashedPatch.AssetForwarding;
using DreadsMashedPatch.RecordHandlers.Abstracts;

if (args.Length != 1)
{
    Console.Error.WriteLine("Usage: AssetForwardingAudit <output-json>");
    return 2;
}
var defaults = new PatcherConfiguration().Forwarding;
var records = typeof(AbstractRecordHandler).Assembly.GetTypes()
    .Where(type => !type.IsAbstract && typeof(AbstractRecordHandler).IsAssignableFrom(type))
    .OrderBy(type => type.Name)
    .Select(type =>
    {
        var handler = (AbstractRecordHandler)Activator.CreateInstance(type)!;
        var groups = AssetGroupCatalog.For(type);
        var owned = new HashSet<string>();
        foreach (var group in groups)
        foreach (var member in group.Members)
            if (!handler.PropertyHandlers.ContainsKey(member) || !owned.Add(member))
                throw new InvalidOperationException($"Missing or overlapping member {type.Name}.{member}");
        return new
        {
            RecordHandler = type.Name,
            Groups = groups.Select(group => new
            {
                group.Name, Family = group.Family.ToString(),
                ForwardByDefault = defaults.Allows(group.Family),
                group.Members
            }).ToArray()
        };
    }).Where(record => record.Groups.Length > 0).ToArray();
File.WriteAllText(Path.GetFullPath(args[0]), JsonSerializer.Serialize(records, new JsonSerializerOptions { WriteIndented = true }) + "\n");
Console.WriteLine($"Asset ownership: {records.Length} record handlers, {records.Sum(record => record.Groups.Length)} groups, {records.Sum(record => record.Groups.Sum(group => group.Members.Count))} registered members.");
return 0;
