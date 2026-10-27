using DreadsMashedPatch.AssetForwarding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.DialogResponse;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // The complete VMAD adapter is copied together to retain its subtype, version and fragments.
    // PreviousDialog is structural linkage; opaque UnknownData and Papyrus Unused bytes do not drive conflicts.
    public class DialogResponseRecordHandler : AbstractRecordHandler
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
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(DialogResponses.MajorFlag)) },
            { "DATA", new SimpleReflectionBinaryDataPropertyHandler<IDialogResponses, IDialogResponsesGetter>("DATA") },
            { "VirtualMachineAdapter", new VirtualMachineAdapterHandler() },
            { "Flags", new FlagsHandler() },

            { "ResetHours", new SimpleReflectionPropertyHandler<float, IDialogResponses, IDialogResponsesGetter>("Flags.ResetHours") },
            { "Topic", new SimpleReflectionFormLinkPropertyHandler<IDialogTopicGetter, IDialogResponses, IDialogResponsesGetter>("Topic") },
            { "FavorLevel", new SimpleReflectionPropertyHandler<FavorLevel?, IDialogResponses, IDialogResponsesGetter>("FavorLevel") },
            { "LinkTo", new SimpleReflectionListPropertyHandler<IFormLinkGetter<IDialogTopicGetter>, IDialogResponses, IDialogResponsesGetter>("LinkTo", ListSemantics.AlignedOrdered) },
            { "ResponseData", new SimpleReflectionFormLinkPropertyHandler<IDialogResponsesGetter, IDialogResponses, IDialogResponsesGetter>("ResponseData") },
            { "Responses", new ResponsesHandler(normalizeTrailingWhitespace: true) },
            { "Conditions", new ConditionsHandler() },

            { "Prompt", new TranslatedStringReflectionPropertyHandler<IDialogResponses, IDialogResponsesGetter>("Prompt") },
            { "Speaker", new SimpleReflectionFormLinkPropertyHandler<INpcGetter, IDialogResponses, IDialogResponsesGetter>("Speaker") },
            { "WalkAwayTopic", new SimpleReflectionFormLinkPropertyHandler<IDialogTopicGetter, IDialogResponses, IDialogResponsesGetter>("WalkAwayTopic") },
            { "AudioOutputOverride", new SimpleReflectionFormLinkPropertyHandler<ISoundOutputModelGetter, IDialogResponses, IDialogResponsesGetter>("AudioOutputOverride") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IDialogResponsesGetter dialogResponseRecord)
            {
                throw new InvalidOperationException($"Expected IDialogResponsesGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = dialogResponseRecord
                .ToLink<IDialogResponsesGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IDialogResponses, IDialogResponsesGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
