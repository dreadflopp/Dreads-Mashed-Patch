using DreadsMashedPatch.AssetForwarding;
using System;
using System.Collections.Generic;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Skyrim;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Plugins.Records;
using Mutagen.Bethesda.Plugins.Cache;
using DreadsMashedPatch.PropertyHandlers.General;
using DreadsMashedPatch.PropertyHandlers.Interfaces;
using DreadsMashedPatch.PropertyHandlers.Projectile;
using DreadsMashedPatch.RecordHandlers.Abstracts;

namespace DreadsMashedPatch.RecordHandlers;

// Trajectory, explosion, muzzle-flash, pickup, disable and collision fields select as coupled
// groups; independently mixing their members could invent behavior no source plugin authored.
public class ProjectileRecordHandler : AbstractRecordHandler
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
        { "MajorRecordFlagsRaw", new MajorRecordFlagsRawHandler(typeof(SkyrimMajorRecord.SkyrimMajorRecordFlag)) },
        { "Name", new TranslatedStringReflectionPropertyHandler<IProjectile, IProjectileGetter>("Name") },
        { "ModelAndBounds", new ModelBoundsHandler() },
        { "Destructible", DestructibleDataHandlers.Data(new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, IProjectile, IProjectileGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals)) },
        { "Destructible.StageModels", DestructibleDataHandlers.Meshes(new GeneratedCopyReflectionPropertyHandler<IDestructibleGetter, Destructible, IProjectile, IProjectileGetter>("Destructible", value => value.DeepCopy(), DestructibleMixIn.Equals)) },
        { "Flags", new SimpleReflectionFlagPropertyHandler<Projectile.Flag, IProjectile, IProjectileGetter>(
            "Flags",
            preserveUnknownBits: true,
            includedFlags:
            [
                Projectile.Flag.Hitscan,
                Projectile.Flag.Supersonic,
                Projectile.Flag.PinsLimbs,
                Projectile.Flag.PassThroughSmallTransparent,
                Projectile.Flag.DisableCombatAimCorrection,
                Projectile.Flag.Rotation
            ]) },
        { "Trajectory", new ProjectileTrajectoryHandler() },
        { "Light", new SimpleReflectionFormLinkPropertyHandler<ILightGetter, IProjectile, IProjectileGetter>("Light") },
        { "MuzzleFlashBehavior", new ProjectileMuzzleFlashHandler() },
        { "TracerChance", new SimpleReflectionPropertyHandler<float, IProjectile, IProjectileGetter>("TracerChance") },
        { "ExplosionBehavior", new ProjectileExplosionHandler() },
        { "Sound", new SimpleReflectionFormLinkPropertyHandler<ISoundDescriptorGetter, IProjectile, IProjectileGetter>("Sound") },
        { "FadeDuration", new SimpleReflectionPropertyHandler<float, IProjectile, IProjectileGetter>("FadeDuration") },
        { "ImpactForce", new SimpleReflectionPropertyHandler<float, IProjectile, IProjectileGetter>("ImpactForce") },
        { "PickupBehavior", new ProjectilePickupHandler() },
        { "DisableBehavior", new ProjectileDisableHandler() },
        { "Collision", new ProjectileCollisionHandler() },
        { "DecalData", new SimpleReflectionFormLinkPropertyHandler<ITextureSetGetter, IProjectile, IProjectileGetter>("DecalData") },
        { "SoundLevel", new SimpleReflectionPropertyHandler<uint, IProjectile, IProjectileGetter>("SoundLevel") },
    };

    public override IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter>[] GetRecordContexts(
        IModContext<ISkyrimMod, ISkyrimModGetter, IMajorRecord, IMajorRecordGetter> winningContext,
        IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
    {
        if (winningContext.Record is not IProjectileGetter projectileRecord)
        {
            throw new InvalidOperationException($"Expected IProjectileGetter but got {winningContext.Record.GetType()}");
        }

        return projectileRecord
            .ToLink<IProjectileGetter>()
            .ResolveAllContexts<ISkyrimMod, ISkyrimModGetter, IProjectile, IProjectileGetter>(state.LinkCache)
            .ToArray();
    }
}
