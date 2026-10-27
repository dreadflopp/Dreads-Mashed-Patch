using DreadsMashedPatch.AssetForwarding;
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
    // Tamriel's persistent CELL policy selects headers only; Mutagen's override mask excludes
    // child references, landscape, navigation meshes and group metadata from these copies.
    public class CellRecordHandler : AbstractRecordHandler
    {
        protected override void InitializePropertyContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> originalContext,
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext)
        {
            base.InitializePropertyContexts(originalContext, winningContext);
            AssetForwardingPolicy.ResolveMemberContexts(GetType(), PropertyHandlers, PropertyContexts);
        }

        protected override PropertyForwardingCoordination CoordinateForwardedProperties(
            IReadOnlyList<IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>> recordContexts,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state) =>
            AssetForwardingPolicy.Coordinate(GetType(), PropertyHandlers, recordContexts, state, AtomicOwnershipTriggerProperties);

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
    }
}
