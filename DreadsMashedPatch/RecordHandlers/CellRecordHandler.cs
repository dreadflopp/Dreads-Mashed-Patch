using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Cell;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: semantic CELL fields use shared scalar, flag, link, and list handlers; obsolete one-field
    //   CELL handlers replaced by these registrations were removed.
    // - Kept specialized: lighting, ownership, encounter-zone, and occlusion structures retain typed handlers.
    // - Smart rule: Tamriel's persistent CELL (000D74:Skyrim.esm) can retain the exact Dawnguard,
    //   Skyrim, or winning header, or use a hybrid that substitutes Dawnguard only for a winning
    //   Skyrim-equivalent header. Mutagen's CELL override mask keeps child references, landscape,
    //   navigation meshes, and group metadata outside this header policy and comparison.
    // - Intentionally excluded: WaterHeight and Landscape are runtime-managed; NavigationMeshes is navigation data.
    // - Rationale: excluded fields remain exactly as authored by the winning override.

    // Header migration: raw/common/Cell.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    // Output migration: all Tamriel policy copies use shared staging, including parent creation.
    // CELL source selection and Mutagen child-excluding copy rules remain specialized.
    // Filter-policy migration: priority snapshots now run in the shared path before this policy.
    // Ordinary EDID-only corrections use baseline identifiers; Tamriel still needs its CELL history.
    public class CellRecordHandler : AbstractRecordHandler
    {
        internal static readonly ModKey SkyrimModKey = ModKey.FromNameAndExtension("Skyrim.esm");
        internal static readonly ModKey DawnguardModKey = ModKey.FromNameAndExtension("Dawnguard.esm");
        internal static readonly FormKey TamrielPersistentCellFormKey = new(SkyrimModKey, 0x000D74);
        private static readonly Cell.TranslationMask CellHeaderComparisonMask = new(defaultOn: true)
        {
            Persistent = false,
            Temporary = false,
            Landscape = false,
            NavigationMeshes = false,
            Timestamp = false,
            PersistentTimestamp = false,
            TemporaryTimestamp = false,
            UnknownGroupData = false,
            PersistentUnknownGroupData = false,
            TemporaryUnknownGroupData = false
        };

        internal static bool RequiresSmartPolicyProcessing(FormKey formKey) =>
            formKey == TamrielPersistentCellFormKey
            && PatcherSettings.TamrielPersistentCellPolicy != Enums.TamrielPersistentCellPolicy.StandardForwarding;

        private readonly Dictionary<string, IPropertyHandler> _propertyHandlers;

        public CellRecordHandler()
        {
            // Initialize property handlers for Cell records. Uses reflection-based handlers where applicable (ICell / ICellGetter).
            _propertyHandlers = new Dictionary<string, IPropertyHandler>
            {
                { "EditorID", new EditorIDHandler() },
                { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Cell.MajorFlag)) },
                { "Name", new TranslatedStringReflectionPropertyHandler<ICell, ICellGetter>("Name") },
                { "Flags", new SimpleReflectionFlagPropertyHandler<Cell.Flag, ICell, ICellGetter>("Flags") },

                { "Regions", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IRegionGetter>, ICell, ICellGetter>("Regions", ListSemantics.SortedKeyed) },
                { "Location", new SimpleReflectionFormLinkPropertyHandler<ILocationGetter, ICell, ICellGetter>("Location") },
                { "Owner", new SimpleReflectionFormLinkPropertyHandler<IOwnerGetter, ICell, ICellGetter>("Owner") },
                { "Water", new SimpleReflectionFormLinkPropertyHandler<IWaterGetter, ICell, ICellGetter>("Water") },
                { "Lighting", new LightingHandler() },
                { "LightingTemplate", new SimpleReflectionFormLinkPropertyHandler<ILightingTemplateGetter, ICell, ICellGetter>("LightingTemplate") },
                { "AcousticSpace", new SimpleReflectionFormLinkPropertyHandler<IAcousticSpaceGetter, ICell, ICellGetter>("AcousticSpace") },
                { "EncounterZone", new SimpleReflectionFormLinkPropertyHandler<IEncounterZoneGetter, ICell, ICellGetter>("EncounterZone") },
                { "Music", new SimpleReflectionFormLinkPropertyHandler<IMusicTypeGetter, ICell, ICellGetter>("Music") },
                { "ImageSpace", new SimpleReflectionFormLinkPropertyHandler<IImageSpaceGetter, ICell, ICellGetter>("ImageSpace") },
                { "SkyAndWeatherFromRegion", new SimpleReflectionFormLinkPropertyHandler<IRegionGetter, ICell, ICellGetter>("SkyAndWeatherFromRegion") },
                { "Grid", new GridHandler() },
                { "MaxHeightData", new MaxHeightDataHandler() },
                { "WaterNoiseTexture", new WaterNoiseTextureHandler() },
                { "WaterVelocity", new WaterVelocityHandler() },
                { "XWCN", new WaterCurrentCountHandler() },
                { "XWCS", new WaterCurrentCountOldHandler() },
                { "OcclusionData", new OcclusionDataHandler() },
                { "LNAM", new LNAMHandler() },
                { "FactionRank", new SimpleReflectionPropertyHandler<int?, ICell, ICellGetter>("FactionRank") },
                { "LockList", new SimpleReflectionFormLinkPropertyHandler<ILockListGetter, ICell, ICellGetter>("LockList") },
                { "WaterEnvironmentMap", new WaterEnvironmentMapHandler() },

            };
        }

        public override Dictionary<string, IPropertyHandler> PropertyHandlers => _propertyHandlers;

        protected override bool TryApplyRecordPolicy(
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state,
            IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts)
        {
            var winningContext = recordContexts[0];
            if (winningContext.Record.FormKey != TamrielPersistentCellFormKey)
            {
                return false;
            }

            var policy = PatcherSettings.TamrielPersistentCellPolicy;
            if (policy == Enums.TamrielPersistentCellPolicy.StandardForwarding)
            {
                Console.WriteLine("Tamriel persistent CELL policy: using normal property forwarding");
                return false;
            }

            if (policy == Enums.TamrielPersistentCellPolicy.KeepWinning)
            {
                Console.WriteLine(
                    $"Tamriel persistent CELL policy: copying winning header from {winningContext.ModKey}");
                CommitOverride(winningContext, state);
                return true;
            }

            if (policy == Enums.TamrielPersistentCellPolicy.Hybrid)
            {
                var skyrimContext = recordContexts.FirstOrDefault(context => context.ModKey == SkyrimModKey);
                var dawnguardContext = recordContexts.FirstOrDefault(context => context.ModKey == DawnguardModKey);
                if (skyrimContext?.Record is not ICellGetter skyrimCell
                    || dawnguardContext == null)
                {
                    LogCollector.AddWarning(
                        "TamrielPersistentCell",
                        $"Hybrid policy could not find both Skyrim.esm and Dawnguard.esm in the override " +
                        $"chain for {TamrielPersistentCellFormKey}; using normal property forwarding");
                    return false;
                }

                var winningCell = (ICellGetter)winningContext.Record;
                if (!winningCell.Equals(skyrimCell, CellHeaderComparisonMask))
                {
                    Console.WriteLine(
                        "Tamriel persistent CELL hybrid policy: winning header differs from Skyrim.esm; " +
                        "using normal property forwarding");
                    return false;
                }

                Console.WriteLine(
                    "Tamriel persistent CELL hybrid policy: winning header matches Skyrim.esm; " +
                    "copying Dawnguard.esm header");
                CommitOverride(dawnguardContext, state);
                return true;
            }

            var preferredMod = policy == Enums.TamrielPersistentCellPolicy.PreferDawnguard
                ? DawnguardModKey
                : SkyrimModKey;
            var preferredContext = recordContexts.FirstOrDefault(context => context.ModKey == preferredMod);
            if (preferredContext == null)
            {
                LogCollector.AddWarning(
                    "TamrielPersistentCell",
                    $"Could not find {preferredMod} in the override chain for {TamrielPersistentCellFormKey}; " +
                    $"copying the winning header from {winningContext.ModKey}");
                CommitOverride(winningContext, state);
                return true;
            }

            if (preferredContext.ModKey == winningContext.ModKey)
            {
                Console.WriteLine(
                    $"Tamriel persistent CELL policy: preferred {preferredMod} header already wins");
            }
            else
            {
                Console.WriteLine(
                    $"Tamriel persistent CELL policy: copying complete header from {preferredMod} " +
                    $"over {winningContext.ModKey}");
            }

            CommitOverride(preferredContext, state);
            return true;
        }

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ICellGetter cellRecord)
            {
                throw new InvalidOperationException($"Expected ICellGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = cellRecord
                .ToLink<ICellGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ICell, ICellGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }

        // CommitOverride and ApplyForwardedProperties are now handled by the base class
        // The base class automatically handles flag property coordination
    }
}
