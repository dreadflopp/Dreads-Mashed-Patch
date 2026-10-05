using System;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Class;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers
{
    // Migration note:
    // - Generalized: Class scalar fields via reflection handlers.
    // - Kept specialized: fixed-key weight dictionaries.
    // - Intentionally excluded: Unknown* fields are outside the semantic conflict surface.
    // - Rationale: dictionary properties are mutable collections without setters and require typed copy/equality.

    // Header migration: raw/common flags share one masked integer handler.
    // Unknown winner bits stay intact; other fields retain their existing handlers and policies.
    // Removed overlapping header registrations so selected clears cannot be reintroduced.
    // Name migration: translated Name uses generated copying to retain every selected language.
    // Required null names become empty; comparison follows Mutagen's language policy.
    // Other specialized fields/flags retain their policies; translations are selected as one value.
    public class ClassRecordHandler : AbstractRecordHandler
    {
        public override Dictionary<string, IPropertyHandler> PropertyHandlers { get; } = new()
        {
            { "EditorID", new EditorIDHandler() },
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IClass, IClassGetter>("Name", required: true) },
            { "Description", new SimpleReflectionPropertyHandler<string, IClass, IClassGetter>("Description") },
            { "Icon", new SimpleReflectionPropertyHandler<string, IClass, IClassGetter>("Icon") },
            { "Teaches", new SimpleReflectionPropertyHandler<Skill?, IClass, IClassGetter>("Teaches") },
            { "MaxTrainingLevel", new SimpleReflectionPropertyHandler<byte, IClass, IClassGetter>("MaxTrainingLevel") },
            { "SkillWeights", new ClassWeightsHandler<Skill>("SkillWeights", record => record.SkillWeights, record => record.SkillWeights) },
            { "BleedoutDefault", new SimpleReflectionPropertyHandler<float, IClass, IClassGetter>("BleedoutDefault") },
            { "VoicePoints", new SimpleReflectionPropertyHandler<uint, IClass, IClassGetter>("VoicePoints") },
            { "StatWeights", new ClassWeightsHandler<BasicStat>("StatWeights", record => record.StatWeights, record => record.StatWeights) },
        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IClassGetter classRecord)
            {
                throw new InvalidOperationException($"Expected IClassGetter but got {winningContext.Record.GetType()}");
            }

            return classRecord
                .ToLink<IClassGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IClass, IClassGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
