using DreadsMashedPatch.AssetForwarding;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Ammunition;
using DreadsMashedPatch.RecordHandlers.Abstracts;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using System;

namespace DreadsMashedPatch.RecordHandlers
{
    // DATADataTypeState controls serializer layout and is not an independently forwarded field.
    public class AmmunitionRecordHandler : AbstractRecordHandler
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
            { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag), typeof(Ammunition.MajorFlag)) },
            { "Name", new TranslatedStringReflectionPropertyHandler<IAmmunition, IAmmunitionGetter>("Name") },
            { "ModelAndBounds", new ModelBoundsHandler() },
            { "Icons", new IconsHandler() },
            { "Destructible", DestructibleDataHandlers.Data(new DestructibleHandler()) },
            { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new DestructibleHandler()) },
            { "PickUpSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IAmmunition, IAmmunitionGetter>("PickUpSound") },
            { "PutDownSound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IAmmunition, IAmmunitionGetter>("PutDownSound") },
            { "Description", new TranslatedStringReflectionPropertyHandler<IAmmunition, IAmmunitionGetter>("Description") },
            { "Keywords", new KeywordListHandler() },
            { "Projectile", new SimpleReflectionFormLinkPropertyHandler<IProjectileGetter, IAmmunition, IAmmunitionGetter>("Projectile") },
            { "Flags", new SimpleReflectionFlagPropertyHandler<Mutagen.Bethesda.Skyrim.Ammunition.Flag, IAmmunition, IAmmunitionGetter>("Flags") },
            { "Damage", new SimpleReflectionPropertyHandler<float, IAmmunition, IAmmunitionGetter>("Damage") },
            { "Value", new ValueHandler() },
            { "Weight", new WeightHandler() },
            { "ShortName", new SimpleReflectionPropertyHandler<string, IAmmunition, IAmmunitionGetter>("ShortName") },

        };

        public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
            IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
            IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            if (winningContext.Record is not IAmmunitionGetter ammunitionRecord)
            {
                throw new InvalidOperationException($"Expected IAmmunitionGetter but got {winningContext.Record.GetType()}");
            }

            return ammunitionRecord
                .ToLink<IAmmunitionGetter>()
                .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IAmmunition, IAmmunitionGetter>(state.LinkCache)
                .ToArray();
        }
    }
}
