using DreadsMashedPatch.AssetForwarding;
using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Cache;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.HeadPart;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Keep mesh/TRI resources, extra parts, type, texture/color and rendering bits together.
    // Eligibility flags and ValidRaces stay independent so applicability edits cannot select a mesh.
    public class HeadPartRecordHandler : AbstractRecordHandler
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

        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "Mesh", new HeadPartMeshHandler() },
            { "ValidRaces", new SimpleReflectionFormLinkPropertyHandler<IFormListGetter, IHeadPart, IHeadPartGetter>("ValidRaces") },
            { "Flags", new SimpleReflectionFlagPropertyHandler<HeadPart.Flag, IHeadPart, IHeadPartGetter>("Flags", preserveUnknownBits: true,
                includedFlags: [HeadPart.Flag.Playable, HeadPart.Flag.Male, HeadPart.Flag.Female]) },
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(HeadPart.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IHeadPart, IHeadPartGetter>("Name") },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IHeadPartGetter headPart)
            {
                throw new InvalidOperationException($"Expected IHeadPartGetter but got {winningContext.Record.GetType()}");
            }

            return headPart
                .ToLink<IHeadPartGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IHeadPart, IHeadPartGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
