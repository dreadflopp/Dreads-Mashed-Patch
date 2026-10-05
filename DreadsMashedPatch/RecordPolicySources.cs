using DreadsMashedPatch.Enums;
using DreadsMashedPatch.PropertyHandlers.General;
using Loqui;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;

namespace DreadsMashedPatch;

/// <summary>
/// Run-scoped, typed policy lookups. Identifier caches index only official/priority
/// plugins; a full source context is resolved only when copying a priority record.
/// </summary>
public sealed class RecordPolicySources : IDisposable
{
    private static readonly EditorIDHandler EditorIdHandler = new();
    private readonly IIdentifierLinkCache? _baseline;
    private readonly PrioritySource[] _prioritySources;

    public RecordPolicySources(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        var eligibleMods = state.LoadOrder.ListedOrder
            .Where(listing => listing.Enabled && listing.Mod != null
                && listing.ModKey != state.PatchMod.ModKey
                && !PatcherSettings.IsIgnoredMod(listing.ModKey))
            .Select(listing => listing.Mod!)
            .ToArray();

        if (PatcherSettings.EditorIdPolicy == EditorIdForwardingPolicy.PreserveBaseline)
        {
            _baseline = eligibleMods.Where(mod => Utility.IsVanilla(mod.ModKey))
                .ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(LinkCachePreferences.OnlyIdentifiers());
        }

        _prioritySources = eligibleMods.Where(mod => PatcherSettings.IsAlwaysWinningMod(mod.ModKey))
            .OrderByDescending(mod => PatcherSettings.GetAlwaysWinningPriority(mod.ModKey))
            .Select(mod => new PrioritySource(mod))
            .ToArray();
    }

    internal static IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetInitialContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> discoveredWinner,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state) =>
        discoveredWinner.Record.ToLink()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>(state.LinkCache)
            .Where(context => !PatcherSettings.IsIgnoredMod(context.ModKey))
            .Take(3)
            .ToArray();

    internal static bool ShouldSkipOrdinaryProcessing(
        IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> contexts) =>
        contexts.Count == 0 || Utility.IsVanilla(contexts[0])
        || contexts.Count <= 2 || Utility.IsVanilla(contexts[1]);

    internal bool TryGetAlwaysWinningMod(IMajorRecordGetter record, out ModKey modKey)
    {
        var type = ((ILoquiObject)record).Registration.GetterType;
        foreach (var source in _prioritySources)
        {
            if (source.Identifiers.TryResolveIdentifier(record.FormKey, type, out _))
            {
                modKey = source.Mod.ModKey;
                return true;
            }
        }

        modKey = default;
        return false;
    }

    internal IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> GetPriorityContext(
        IMajorRecordGetter record, ModKey modKey)
    {
        var source = _prioritySources.Single(source => source.Mod.ModKey == modKey);
        var type = ((ILoquiObject)record).Registration.GetterType;
        if (!source.Contexts.TryResolveContext(record.FormKey, type, out var context))
        {
            throw new InvalidOperationException($"Could not resolve priority source {modKey} for {record.FormKey}.");
        }
        return context;
    }

    internal bool TryGetBaselineEditorId(IMajorRecordGetter record, out string? editorId)
    {
        editorId = null;
        return _baseline != null && _baseline.TryResolveIdentifier(
            record.FormKey, ((ILoquiObject)record).Registration.GetterType, out editorId);
    }

    internal bool RequiresBaselineEditorIdOverride(IMajorRecordGetter record) =>
        TryGetBaselineEditorId(record, out var editorId)
        && !EditorIdHandler.AreValuesEqual(editorId, record.EditorID);

    public void Dispose()
    {
        _baseline?.Dispose();
        foreach (var source in _prioritySources) source.Dispose();
    }

    private sealed class PrioritySource(ISkyrimModGetter mod) : IDisposable
    {
        private IIdentifierLinkCache? _identifiers;
        private ILinkCache<ISkyrimMod, ISkyrimModGetter>? _contexts;
        public ISkyrimModGetter Mod { get; } = mod;
        public IIdentifierLinkCache Identifiers => _identifiers ??=
            Mod.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>(LinkCachePreferences.OnlyIdentifiers());
        public ILinkCache<ISkyrimMod, ISkyrimModGetter> Contexts => _contexts ??=
            Mod.ToImmutableLinkCache<ISkyrimMod, ISkyrimModGetter>();

        public void Dispose()
        {
            _identifiers?.Dispose();
            _contexts?.Dispose();
        }
    }
}
