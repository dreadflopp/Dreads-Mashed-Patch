using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: Parts use generated element copying and xEdit's PartNode sort key.
    // - Kept specialized: Model via shared model handler.
    // - Rationale: BPTD Body Parts is wbRArrayS/wbRStructSK([2]); PartNode is
    //   identity while the remaining body-part fields are replaceable row data.
    public class BodyPartDataRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler() },
            { "SkyrimMajorRecordFlags", new SkyrimMajorRecordFlagsHandler() },
            { "Model", new ModelHandler() },
            { "Parts", new GeneratedCopyReflectionListPropertyHandler<IBodyPartGetter, BodyPart, IBodyPartData, IBodyPartDataGetter>(
                "Parts", ListSemantics.SortedKeyed, value => value.DeepCopy(), BodyPartMixIn.Equals,
                keySelector: value => value.PartNode) }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IBodyPartDataGetter bodyPartData)
            {
                throw new InvalidOperationException($"Expected IBodyPartDataGetter but got {winningContext.Record.GetType()}");
            }

            return bodyPartData
                .ToLink<IBodyPartDataGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IBodyPartData, IBodyPartDataGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
