using System.Collections;
using System.Text.Json;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Xunit;

namespace DreadsMashedPatch.Tests;

public sealed class CollectionSemanticsManifestTests
{
    [Fact]
    public void EveryActiveCollectionRegistrationMatchesTheReviewedManifest()
    {
        var expected = LoadManifest()
            .ToDictionary(entry => Key(entry.RecordHandler, entry.Property), StringComparer.Ordinal);
        var actual = DiscoverCollectionRegistrations()
            .ToDictionary(entry => Key(entry.RecordHandler, entry.Property), StringComparer.Ordinal);

        Assert.Equal(expected.Keys.Order(StringComparer.Ordinal), actual.Keys.Order(StringComparer.Ordinal));
        foreach (var (key, registration) in actual)
        {
            var manifestEntry = expected[key];
            Assert.Equal(manifestEntry.MergeMode, registration.MergeMode);
            Assert.Equal(manifestEntry.HandlerType, registration.HandlerType);
            Assert.Equal(manifestEntry.ValueType, registration.ValueType);
        }
    }

    [Fact]
    public void EveryRuntimeCollectionMatchesACompleteReviewedXEditExpectation()
    {
        var actual = DiscoverCollectionRegistrations()
            .ToDictionary(entry => Key(entry.RecordHandler, entry.Property), StringComparer.Ordinal);
        var expected = LoadXEditExpectations()
            .ToDictionary(entry => Key(entry.RecordHandler, entry.Property), StringComparer.Ordinal);

        Assert.Equal(expected.Keys.Order(StringComparer.Ordinal), actual.Keys.Order(StringComparer.Ordinal));
        foreach (var (key, runtime) in actual)
        {
            var expectation = expected[key];
            Assert.Contains(expectation.ReviewStatus, new[] { "Reviewed", "IntentionalException" });
            Assert.Equal(expectation.ExpectedRuntimeMode, runtime.MergeMode);
            Assert.False(string.IsNullOrWhiteSpace(expectation.XEditSemantics));
            Assert.False(string.IsNullOrWhiteSpace(expectation.XEditConstruct));
            Assert.False(string.IsNullOrWhiteSpace(expectation.IdentityKey));
            Assert.False(string.IsNullOrWhiteSpace(expectation.Alignability));
            Assert.False(string.IsNullOrWhiteSpace(expectation.CountOrCoupling));
            Assert.False(string.IsNullOrWhiteSpace(expectation.XEditSource));
            Assert.False(string.IsNullOrWhiteSpace(expectation.Rationale));
        }
    }

    [Fact]
    public void AtomicCollectionsUseSingleValueOwnershipContexts()
    {
        foreach (var registration in DiscoverCollectionRegistrations().Where(item => item.MergeMode == "Atomic"))
        {
            Assert.StartsWith(
                "SimplePropertyContext",
                registration.Handler.CreatePropertyContext().GetType().Name);
        }
    }

    [Fact]
    public void PerEntryCollectionsUseListOwnershipContexts()
    {
        foreach (var registration in DiscoverCollectionRegistrations().Where(item =>
                     item.MergeMode is not ("Atomic" or "Specialized")))
        {
            Assert.StartsWith(
                "ListPropertyContext",
                registration.Handler.CreatePropertyContext().GetType().Name);
        }
    }

    [Fact]
    public void OnlyReviewedXEditExceptionsRemainUnordered()
    {
        var unordered = DiscoverCollectionRegistrations()
            .Where(item => item.MergeMode == nameof(ListSemantics.Unordered))
            .Select(item => Key(item.RecordHandler, item.Property))
            .Order(StringComparer.Ordinal);

        Assert.Equal(
            new[]
            {
                Key("LandscapeRecordHandler", "Layers"),
                Key("LandscapeRecordHandler", "Textures")
            },
            unordered);
    }

    private static IReadOnlyList<ManifestEntry> LoadManifest()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "collection-semantics-manifest.json");
        Assert.True(File.Exists(path), $"Collection semantics manifest was not copied to the test output: {path}");
        return JsonSerializer.Deserialize<List<ManifestEntry>>(File.ReadAllText(path))
            ?? throw new InvalidOperationException("Collection semantics manifest is empty.");
    }

    private static IReadOnlyList<XEditExpectation> LoadXEditExpectations()
    {
        var path = Path.Combine(AppContext.BaseDirectory, "xedit-collection-expectations.json");
        Assert.True(File.Exists(path), $"xEdit collection expectations were not copied to the test output: {path}");
        return JsonSerializer.Deserialize<List<XEditExpectation>>(File.ReadAllText(path))
            ?? throw new InvalidOperationException("xEdit collection expectations are empty.");
    }

    private static IReadOnlyList<DiscoveredRegistration> DiscoverCollectionRegistrations()
    {
        var registrations = new List<DiscoveredRegistration>();
        var recordHandlerTypes = typeof(AbstractRecordHandler).Assembly.GetTypes()
            .Where(type => !type.IsAbstract && typeof(AbstractRecordHandler).IsAssignableFrom(type))
            .OrderBy(type => type.Name, StringComparer.Ordinal);

        foreach (var recordHandlerType in recordHandlerTypes)
        {
            var recordHandler = Assert.IsAssignableFrom<AbstractRecordHandler>(Activator.CreateInstance(recordHandlerType));
            foreach (var (propertyName, handler) in recordHandler.PropertyHandlers)
            {
                var valueType = GetHandledValueType(handler.GetType());
                if (valueType == null || !IsCollectionValue(valueType))
                {
                    continue;
                }

                registrations.Add(new DiscoveredRegistration(
                    recordHandlerType.Name,
                    propertyName,
                    FriendlyName(valueType),
                    FriendlyName(handler.GetType()),
                    GetMergeMode(handler),
                    handler));
            }
        }

        return registrations;
    }

    private static Type? GetHandledValueType(Type handlerType) =>
        handlerType.GetInterfaces()
            .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IPropertyHandler<>))
            .Select(type => type.GetGenericArguments()[0])
            .FirstOrDefault();

    private static bool IsCollectionValue(Type type)
    {
        if (type == typeof(string) || type == typeof(byte[])) return false;
        if (type.IsArray) return type.GetElementType() != typeof(byte);
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

    private static ListSemantics? GetListSemantics(IPropertyHandler handler)
    {
        for (var type = handler.GetType(); type != null; type = type.BaseType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(AbstractListPropertyHandler<>))
            {
                return handler.GetType().GetProperty(nameof(AbstractListPropertyHandler<object>.Semantics))
                    ?.GetValue(handler) as ListSemantics?;
            }
        }

        return null;
    }

    private static string GetMergeMode(IPropertyHandler handler)
    {
        var semantics = GetListSemantics(handler);
        if (semantics != null) return semantics.Value.ToString();
        return handler.CreatePropertyContext().GetType().Name.StartsWith("SimplePropertyContext", StringComparison.Ordinal)
            ? "Atomic"
            : "Specialized";
    }

    private static string FriendlyName(Type type)
    {
        if (!type.IsGenericType) return type.FullName ?? type.Name;
        var name = type.GetGenericTypeDefinition().FullName ?? type.Name;
        name = name[..name.IndexOf('`')];
        return $"{name}<{string.Join(", ", type.GetGenericArguments().Select(FriendlyName))}>";
    }

    private static string Key(string recordHandler, string property) => $"{recordHandler}.{property}";

    private sealed record ManifestEntry(
        string RecordHandler,
        string Property,
        string ValueType,
        string HandlerType,
        string MergeMode);

    private sealed record XEditExpectation(
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

    private sealed record DiscoveredRegistration(
        string RecordHandler,
        string Property,
        string ValueType,
        string HandlerType,
        string MergeMode,
        IPropertyHandler Handler);
}
