using System;
using System.Drawing;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Skyrim.Assets;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using Noggog;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: translated text and constraints use generated copies; links/scalars use typed handlers.
    // - Kept specialized: Conditions uses the shared polymorphic condition handler.
    // - Rationale: Condition is abstract and must be copied through generated subtype dispatch.

    // Header migration: raw/common/LoadScreen.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    public class LoadScreenRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(LoadScreen.MajorFlag)) },
            { "Icons", new SimpleReflectionIconsPropertyHandler<ILoadScreen, ILoadScreenGetter>("Icons") },
            { "Description", new TranslatedStringReflectionPropertyHandler<ILoadScreen, ILoadScreenGetter>("Description") },
            { "Conditions", new ConditionsHandler<ILoadScreen, ILoadScreenGetter>(record => record.Conditions, record => record.Conditions) },
            { "LoadingScreenNif", new SimpleReflectionFormLinkPropertyHandler<IStaticGetter, ILoadScreen, ILoadScreenGetter>("LoadingScreenNif") },
            { "InitialScale", new SimpleReflectionPropertyHandler<float?, ILoadScreen, ILoadScreenGetter>("InitialScale") },
            { "InitialRotation", new SimpleReflectionPropertyHandler<P3Int16?, ILoadScreen, ILoadScreenGetter>("InitialRotation") },
            { "RotationOffsetConstraints", new GeneratedCopyReflectionPropertyHandler<IInt16MinMaxGetter, Int16MinMax, ILoadScreen, ILoadScreenGetter>(
                "RotationOffsetConstraints", value => value.DeepCopy(), Int16MinMaxMixIn.Equals) },
            { "InitialTranslationOffset", new SimpleReflectionPropertyHandler<P3Float?, ILoadScreen, ILoadScreenGetter>("InitialTranslationOffset") },
            { "CameraPath", new SimpleReflectionAssetLinkPropertyHandler<SkyrimModelAssetType, ILoadScreen, ILoadScreenGetter>("CameraPath") },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ILoadScreenGetter loadScreen)
            {
                throw new InvalidOperationException($"Expected ILoadScreenGetter but got {winningContext.Record.GetType()}");
            }

            return loadScreen
                .ToLink<ILoadScreenGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ILoadScreen, ILoadScreenGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
