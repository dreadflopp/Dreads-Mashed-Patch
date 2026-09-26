using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

var repositoryRoot = args.Length > 0
    ? Path.GetFullPath(args[0])
    : Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));
var outputPath = args.Length > 1
    ? Path.GetFullPath(args[1])
    : Path.Combine(repositoryRoot, "docs", "collection-semantics-manifest.json");
var expectationsPath = args.Length > 2
    ? Path.GetFullPath(args[2])
    : Path.Combine(repositoryRoot, "docs", "xedit-collection-expectations.json");
var reportPath = args.Length > 3
    ? Path.GetFullPath(args[3])
    : Path.Combine(repositoryRoot, "docs", "collection-semantics-comparison.md");
var verify = args.Any(argument => string.Equals(argument, "--verify", StringComparison.Ordinal));
var failOnUnresolved = args.Any(argument => string.Equals(argument, "--fail-on-unresolved", StringComparison.Ordinal));

var entries = DiscoverEntries();
var options = new JsonSerializerOptions
{
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};
var json = JsonSerializer.Serialize(entries, options) + Environment.NewLine;

if (!File.Exists(expectationsPath))
{
    Console.Error.WriteLine($"xEdit collection expectations are missing: {expectationsPath}");
    return 1;
}

var expectations = JsonSerializer.Deserialize<List<XEditCollectionExpectation>>(
        File.ReadAllText(expectationsPath),
        new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
    ?? throw new InvalidOperationException("xEdit collection expectations are empty.");
var comparison = Compare(entries, expectations);
var report = BuildReport(comparison, expectations.Count, entries.Count);
var failures = comparison.Where(item => item.Result is "Mismatch" or "MissingExpectation" or "MissingRuntime" or "InvalidExpectation").ToList();
var unresolvedCount = comparison.Count(item => item.Result == "Unresolved");

if (verify)
{
    if (!File.Exists(outputPath))
    {
        Console.Error.WriteLine($"Collection semantics manifest is missing: {outputPath}");
        return 1;
    }

    var current = File.ReadAllText(outputPath);
    if (!string.Equals(NormalizeNewlines(current), NormalizeNewlines(json), StringComparison.Ordinal))
    {
        Console.Error.WriteLine("Collection semantics manifest is stale. Run scripts/Audit-CollectionSemantics.ps1 to regenerate it, then review the semantic change against xEdit.");
        return 1;
    }

    if (!File.Exists(reportPath)
        || !string.Equals(
            NormalizeNewlines(File.ReadAllText(reportPath)),
            NormalizeNewlines(report),
            StringComparison.Ordinal))
    {
        Console.Error.WriteLine("Collection semantics comparison report is stale. Run scripts/Audit-CollectionSemantics.ps1 to regenerate it.");
        return 1;
    }

    return ReportResult(entries.Count, expectations.Count, unresolvedCount, failures, failOnUnresolved);
}

Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
File.WriteAllText(outputPath, json);
Directory.CreateDirectory(Path.GetDirectoryName(reportPath)!);
File.WriteAllText(reportPath, report);
Console.WriteLine($"Collection semantics manifest: {outputPath} ({entries.Count} registrations)");
Console.WriteLine($"xEdit comparison report: {reportPath}");
return ReportResult(entries.Count, expectations.Count, unresolvedCount, failures, failOnUnresolved);

static List<CollectionRegistration> DiscoverEntries()
{
    var assembly = typeof(AbstractRecordHandler).Assembly;
    var recordHandlerTypes = assembly.GetTypes()
        .Where(type => !type.IsAbstract && typeof(AbstractRecordHandler).IsAssignableFrom(type))
        .OrderBy(type => type.Name, StringComparer.Ordinal);
    var registrations = new List<CollectionRegistration>();

    foreach (var recordHandlerType in recordHandlerTypes)
    {
        if (Activator.CreateInstance(recordHandlerType) is not AbstractRecordHandler recordHandler)
        {
            throw new InvalidOperationException($"Could not instantiate {recordHandlerType.FullName}.");
        }

        foreach (var (propertyName, propertyHandler) in recordHandler.PropertyHandlers.OrderBy(pair => pair.Key, StringComparer.Ordinal))
        {
            var valueType = GetHandledValueType(propertyHandler.GetType());
            if (valueType == null || !IsCollectionValue(valueType))
            {
                continue;
            }

            var semantics = GetListSemantics(propertyHandler);
            var mergeMode = GetMergeMode(propertyHandler, semantics);
            registrations.Add(new CollectionRegistration(
                recordHandlerType.Name,
                propertyName,
                FriendlyName(valueType),
                FriendlyName(propertyHandler.GetType()),
                mergeMode,
                IdentityPolicy(mergeMode),
                CardinalityPolicy(mergeMode),
                mergeMode switch
                {
                    "Atomic" => "Whole collection has one owner; handler-defined null/empty behavior.",
                    "Specialized" => "Custom ownership context and handler-defined null/empty behavior.",
                    _ => "Per-entry ownership; list handler tracks null versus empty when the Mutagen surface is nullable."
                },
                "Observed runtime behavior only; compare with xedit-collection-expectations.json."));
        }
    }

    return registrations
        .OrderBy(entry => entry.RecordHandler, StringComparer.Ordinal)
        .ThenBy(entry => entry.Property, StringComparer.Ordinal)
        .ToList();
}

static Type? GetHandledValueType(Type handlerType) =>
    handlerType.GetInterfaces()
        .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IPropertyHandler<>))
        .Select(type => type.GetGenericArguments()[0])
        .FirstOrDefault();

static bool IsCollectionValue(Type type)
{
    if (type == typeof(string) || type == typeof(byte[]))
    {
        return false;
    }

    if (type.IsArray)
    {
        return type.GetElementType() != typeof(byte);
    }

    if (type.Namespace == "Mutagen.Bethesda.Strings"
        || (type.Namespace == "Mutagen.Bethesda.Plugins.Records"
            && type.Name.StartsWith("IGendered", StringComparison.Ordinal))
        || (type.Namespace == "Noggog" && type.Name.Contains("MemorySlice", StringComparison.Ordinal)))
    {
        return false;
    }

    return typeof(IEnumerable).IsAssignableFrom(type)
        || type.GetInterfaces().Any(candidate => candidate == typeof(IEnumerable));
}

static ListSemantics? GetListSemantics(IPropertyHandler handler)
{
    for (var type = handler.GetType(); type != null; type = type.BaseType)
    {
        if (!type.IsGenericType || type.GetGenericTypeDefinition() != typeof(AbstractListPropertyHandler<>))
        {
            continue;
        }

        return handler.GetType().GetProperty(nameof(AbstractListPropertyHandler<object>.Semantics))?.GetValue(handler)
            as ListSemantics?;
    }

    return null;
}

static string GetMergeMode(IPropertyHandler handler, ListSemantics? semantics)
{
    if (semantics != null)
    {
        return semantics.Value.ToString();
    }

    return handler.CreatePropertyContext().GetType().Name.StartsWith("SimplePropertyContext", StringComparison.Ordinal)
        ? "Atomic"
        : "Specialized";
}

static string IdentityPolicy(string mergeMode) => mergeMode switch
{
    nameof(ListSemantics.Unordered) => "Full semantic value",
    nameof(ListSemantics.SortedKeyed) => "xEdit sort key (StructSK/RStructSK or scalar value)",
    nameof(ListSemantics.AlignedOrdered) => "Occurrence-specific aligned row key",
    nameof(ListSemantics.ExactOrdered) => "Zero-based position",
    "Atomic" => "Whole collection",
    "Specialized" => "Handler-defined",
    _ => throw new ArgumentOutOfRangeException(nameof(mergeMode), mergeMode, null)
};

static string CardinalityPolicy(string mergeMode) => mergeMode switch
{
    nameof(ListSemantics.Unordered) => "Multiset; duplicates retained",
    nameof(ListSemantics.SortedKeyed) => "Keyed rows; duplicate occurrences retained",
    nameof(ListSemantics.AlignedOrdered) => "Ordered sequence; duplicate occurrences retained",
    nameof(ListSemantics.ExactOrdered) => "Positional sequence",
    "Atomic" => "Atomic sequence",
    "Specialized" => "Handler-defined",
    _ => throw new ArgumentOutOfRangeException(nameof(mergeMode), mergeMode, null)
};

static string FriendlyName(Type type)
{
    if (!type.IsGenericType)
    {
        return type.FullName ?? type.Name;
    }

    var name = type.GetGenericTypeDefinition().FullName ?? type.Name;
    name = name[..name.IndexOf('`')];
    return $"{name}<{string.Join(", ", type.GetGenericArguments().Select(FriendlyName))}>";
}

static string NormalizeNewlines(string value) => value.Replace("\r\n", "\n", StringComparison.Ordinal);

static List<CollectionComparison> Compare(
    IReadOnlyList<CollectionRegistration> runtimeEntries,
    IReadOnlyList<XEditCollectionExpectation> expectations)
{
    var results = new List<CollectionComparison>();
    var runtimeByKey = runtimeEntries.ToDictionary(RuntimeKey, StringComparer.Ordinal);
    var duplicateExpectations = expectations.GroupBy(ExpectationKey, StringComparer.Ordinal)
        .Where(group => group.Count() > 1)
        .ToList();
    foreach (var duplicate in duplicateExpectations)
    {
        results.Add(new CollectionComparison(duplicate.Key, "InvalidExpectation", null, null, "Duplicate expectation entries."));
    }

    var expectationByKey = expectations
        .GroupBy(ExpectationKey, StringComparer.Ordinal)
        .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

    foreach (var runtime in runtimeEntries)
    {
        var key = RuntimeKey(runtime);
        if (!expectationByKey.TryGetValue(key, out var expectation))
        {
            results.Add(new CollectionComparison(key, "MissingExpectation", runtime.MergeMode, null, "Active runtime collection has no xEdit expectation."));
            continue;
        }

        if (expectation.ReviewStatus == "Unresolved")
        {
            results.Add(new CollectionComparison(key, "Unresolved", runtime.MergeMode, expectation.ExpectedRuntimeMode, expectation.Rationale));
            continue;
        }

        if (expectation.ReviewStatus is not ("Reviewed" or "IntentionalException")
            || string.IsNullOrWhiteSpace(expectation.XEditSemantics)
            || string.IsNullOrWhiteSpace(expectation.XEditConstruct)
            || string.IsNullOrWhiteSpace(expectation.IdentityKey)
            || string.IsNullOrWhiteSpace(expectation.Alignability)
            || string.IsNullOrWhiteSpace(expectation.CountOrCoupling)
            || string.IsNullOrWhiteSpace(expectation.XEditSource)
            || string.IsNullOrWhiteSpace(expectation.Rationale))
        {
            results.Add(new CollectionComparison(key, "InvalidExpectation", runtime.MergeMode, expectation.ExpectedRuntimeMode, "Reviewed expectations require complete xEdit shape, identity, coupling, source, and rationale fields."));
            continue;
        }

        var result = string.Equals(runtime.MergeMode, expectation.ExpectedRuntimeMode, StringComparison.Ordinal)
            ? "Matched"
            : "Mismatch";
        results.Add(new CollectionComparison(key, result, runtime.MergeMode, expectation.ExpectedRuntimeMode, expectation.Rationale));
    }

    foreach (var expectation in expectations.Where(item => !runtimeByKey.ContainsKey(ExpectationKey(item))))
    {
        results.Add(new CollectionComparison(ExpectationKey(expectation), "MissingRuntime", null, expectation.ExpectedRuntimeMode, "xEdit expectation has no active runtime registration."));
    }

    return results.OrderBy(item => item.Key, StringComparer.Ordinal).ToList();
}

static string BuildReport(
    IReadOnlyList<CollectionComparison> comparison,
    int expectationCount,
    int runtimeCount)
{
    var counts = comparison.GroupBy(item => item.Result).ToDictionary(group => group.Key, group => group.Count());
    var lines = new List<string>
    {
        "# Collection semantics comparison",
        "",
        "This report compares the generated runtime inventory with the independently maintained pinned-xEdit expectations.",
        "",
        $"- Runtime registrations: {runtimeCount}",
        $"- xEdit expectations: {expectationCount}",
        $"- Matched: {counts.GetValueOrDefault("Matched")}",
        $"- Unresolved: {counts.GetValueOrDefault("Unresolved")}",
        $"- Mismatched: {counts.GetValueOrDefault("Mismatch")}",
        $"- Missing/invalid: {counts.GetValueOrDefault("MissingExpectation") + counts.GetValueOrDefault("MissingRuntime") + counts.GetValueOrDefault("InvalidExpectation")}",
        "",
        "| Registration | Result | Runtime | Expected | Note |",
        "| --- | --- | --- | --- | --- |"
    };
    lines.AddRange(comparison.Select(item =>
        $"| `{Escape(item.Key)}` | {item.Result} | {item.RuntimeMode ?? "—"} | {item.ExpectedMode ?? "—"} | {Escape(item.Note)} |"));
    return string.Join(Environment.NewLine, lines) + Environment.NewLine;
}

static int ReportResult(
    int runtimeCount,
    int expectationCount,
    int unresolvedCount,
    IReadOnlyList<CollectionComparison> failures,
    bool failOnUnresolved)
{
    Console.WriteLine($"Collection comparison: {runtimeCount} runtime, {expectationCount} expected, {unresolvedCount} unresolved, {failures.Count} failures.");
    foreach (var failure in failures)
    {
        Console.Error.WriteLine($"{failure.Result}: {failure.Key} (runtime={failure.RuntimeMode ?? "missing"}, expected={failure.ExpectedMode ?? "missing"})");
    }

    if (failures.Count > 0 || (failOnUnresolved && unresolvedCount > 0))
    {
        return 1;
    }

    return 0;
}

static string RuntimeKey(CollectionRegistration entry) => $"{entry.RecordHandler}.{entry.Property}";
static string ExpectationKey(XEditCollectionExpectation entry) => $"{entry.RecordHandler}.{entry.Property}";
static string Escape(string value) => value.Replace("|", "\\|", StringComparison.Ordinal).Replace("\r", " ", StringComparison.Ordinal).Replace("\n", " ", StringComparison.Ordinal);

internal sealed record CollectionRegistration(
    string RecordHandler,
    string Property,
    string ValueType,
    string HandlerType,
    string MergeMode,
    string IdentityPolicy,
    string CardinalityPolicy,
    string PresencePolicy,
    string Evidence);

internal sealed record XEditCollectionExpectation(
    string RecordHandler,
    string Property,
    string ReviewStatus,
    string ExpectedRuntimeMode,
    string XEditSemantics,
    string XEditConstruct,
    string IdentityKey,
    string Alignability,
    string CountOrCoupling,
    string XEditSource,
    string Rationale);

internal sealed record CollectionComparison(
    string Key,
    string Result,
    string? RuntimeMode,
    string? ExpectedMode,
    string Note);
