using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: ENAM uses the binary handler; nested image-space sections use generated copy/equality.
    // - Kept specialized: none.
    // - Rationale: the record is a small composition of binary data plus nested value objects.
    public class ImageSpaceRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler() },
            { "SkyrimMajorRecordFlags", new SkyrimMajorRecordFlagsHandler() },
            { "ENAM", new SimpleReflectionBinaryDataPropertyHandler<IImageSpace, IImageSpaceGetter>("ENAM") },
            { "Hdr", new GeneratedCopyReflectionPropertyHandler<IImageSpaceHdrGetter, ImageSpaceHdr, IImageSpace, IImageSpaceGetter>(
                "Hdr", value => value.DeepCopy(), ImageSpaceHdrMixIn.Equals) },
            { "Cinematic", new GeneratedCopyReflectionPropertyHandler<IImageSpaceCinematicGetter, ImageSpaceCinematic, IImageSpace, IImageSpaceGetter>(
                "Cinematic", value => value.DeepCopy(), ImageSpaceCinematicMixIn.Equals) },
            { "Tint", new GeneratedCopyReflectionPropertyHandler<IImageSpaceTintGetter, ImageSpaceTint, IImageSpace, IImageSpaceGetter>(
                "Tint", value => value.DeepCopy(), ImageSpaceTintMixIn.Equals) },
            { "DepthOfField", new GeneratedCopyReflectionPropertyHandler<IImageSpaceDepthOfFieldGetter, ImageSpaceDepthOfField, IImageSpace, IImageSpaceGetter>(
                "DepthOfField", value => value.DeepCopy(), ImageSpaceDepthOfFieldMixIn.Equals) }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IImageSpaceGetter imageSpace)
            {
                throw new InvalidOperationException($"Expected IImageSpaceGetter but got {winningContext.Record.GetType()}");
            }

            return imageSpace
                .ToLink<IImageSpaceGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IImageSpace, IImageSpaceGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
