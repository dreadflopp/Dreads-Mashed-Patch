using System;
using System.Drawing;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Light;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: Model now uses the shared full-model handler for IModeled records.
    // - Kept specialized: light flags, icons, bounds, destructible data, and VM data.
    // - Rationale: the shared handler writes the model's data-relative path and copies the
    //   complete model without serializing Mutagen's implied "Meshes" asset root.

    // Header migration: raw/common/Light.MajorFlag flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Optional null names remove the value; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class LightRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Light.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<ILight, ILightGetter>("Name") },
            // VMAD note: shared setter retains winner Version/ObjectFormat; script ownership/unused-data copying stays specialized because selection contains only scripts.
            { "VirtualMachineAdapter", new VirtualMachineAdapterHandler() },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Icons", new IconsHandler() },
            { "Destructible", new DestructibleHandler() },
            { "Time", new SimpleReflectionPropertyHandler<int, ILight, ILightGetter>("Time") },
            { "Radius", new SimpleReflectionPropertyHandler<uint, ILight, ILightGetter>("Radius") },
            { "Color", new SimpleReflectionPropertyHandler<Color, ILight, ILightGetter>("Color") },
            { "Flags", new FlagsHandler() },
            { "FalloffExponent", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FalloffExponent") },
            { "FOV", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FOV") },
            { "NearClip", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("NearClip") },
            { "FlickerPeriod", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FlickerPeriod") },
            { "FlickerIntensityAmplitude", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FlickerIntensityAmplitude") },
            { "FlickerMovementAmplitude", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FlickerMovementAmplitude") },
            { "Value", new ValueHandler() },
            { "Weight", new WeightHandler() },
            { "FadeValue", new SimpleReflectionPropertyHandler<float, ILight, ILightGetter>("FadeValue") },
            { "Sound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, ILight, ILightGetter>("Sound") },
            { "Lens", new SimpleReflectionFormLinkPropertyHandler<ILensFlareGetter, ILight, ILightGetter>("Lens") }
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not ILightGetter lightRecord)
            {
                throw new InvalidOperationException($"Expected ILightGetter but got {winningContext.Record.GetType()}");
            }
            var contexts = lightRecord
                .ToLink<ILightGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, ILight, ILightGetter>(state.LinkCache)
                .ToArray();

            return contexts;
        }
    }
}
