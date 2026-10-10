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

    // Shared GLOB/GMST groups can contain different subtypes for the same FormKey.
    // Keep their contexts unnarrowed until the effective winner and subtype era are known.
    internal static Type GetQueryGetterType(IMajorRecordGetter record) => record switch
    {
        IGlobalGetter => typeof(IGlobalGetter),
        IGameSettingGetter => typeof(IGameSettingGetter),
        _ => ((ILoquiObject)record).Registration.GetterType
    };

    internal static IEnumerable<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> GetEligibleContexts(
        IMajorRecordGetter record, IPatcherState<ISkyrimMod, ISkyrimModGetter> state) =>
        state.LinkCache.ResolveAllContexts(record.FormKey, GetQueryGetterType(record))
            .Where(context => !PatcherSettings.IsIgnoredMod(context.ModKey));

    // Filtering and ordinary merging share the same context index.
    internal static IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetInitialContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> discoveredWinner,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state) =>
        GetEligibleContexts(discoveredWinner.Record, state).Take(3).ToArray();

    internal static bool ShouldSkipOrdinaryProcessing(
        IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> contexts) =>
        contexts.Count == 0 || Utility.IsVanilla(contexts[0])
        || contexts.Count <= 2 || Utility.IsVanilla(contexts[1]);

    internal bool TryGetAlwaysWinningMod(IMajorRecordGetter record, out ModKey modKey)
    {
        var type = GetQueryGetterType(record);
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
        var type = GetQueryGetterType(record);
        if (!source.Contexts.TryResolveContext(record.FormKey, type, out var context))
        {
            throw new InvalidOperationException($"Could not resolve priority source {modKey} for {record.FormKey}.");
        }
        return context;
    }

    internal bool TryGetBaselineEditorId(IMajorRecordGetter record, out string? editorId)
    {
        editorId = null;
        if (_baseline == null || !_baseline.TryResolveIdentifier(
                record.FormKey, GetQueryGetterType(record), out editorId)) return false;

        // GMST's EDID prefix determines its binary subtype. An official name from
        // another subtype must not be applied to the selected record's Data layout.
        if (record is IGameSettingGetter setting
            && (string.IsNullOrEmpty(editorId)
                || !GameSettingUtility.TryGetGameSettingType(editorId[0], out var type)
                || !(type switch
                {
                    GameSettingType.Int => setting is IGameSettingIntGetter,
                    GameSettingType.Float => setting is IGameSettingFloatGetter,
                    GameSettingType.String => setting is IGameSettingStringGetter,
                    GameSettingType.Bool => setting is IGameSettingBoolGetter,
                    _ => false
                })))
        {
            editorId = null;
            return false;
        }
        return true;
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
