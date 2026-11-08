using Mutagen.Bethesda.Plugins;
using Mutagen.Bethesda.Skyrim;

namespace DreadsMashedPatch.Tests;

// Explicit reference targets for the placed-trap forwarding fixtures. Distinct
// record types use distinct FormKeys so full-pipeline tests remain valid plugins.
internal static class PlacedTrapTestTargets
{
    internal static FormKey Key(uint id) => new(CoverageTestHistory.Original, id);

    internal static void Add(SkyrimMod mod, Cell cell, bool projectile)
    {
        if (projectile)
        {
            mod.Projectiles.Add(new Projectile(Key(0x880), SkyrimRelease.SkyrimSE));
            mod.Projectiles.Add(new Projectile(Key(0x801), SkyrimRelease.SkyrimSE));
        }
        else mod.Hazards.Add(new Hazard(Key(0x880), SkyrimRelease.SkyrimSE));
        mod.EncounterZones.Add(new EncounterZone(Key(0x804), SkyrimRelease.SkyrimSE));
        mod.Npcs.Add(new Npc(Key(0x805), SkyrimRelease.SkyrimSE));
        mod.Lights.Add(new Light(Key(0x806), SkyrimRelease.SkyrimSE));
        mod.Locations.Add(new Location(Key(0x808), SkyrimRelease.SkyrimSE));
        mod.Keywords.Add(new Keyword(Key(0x809), SkyrimRelease.SkyrimSE));
        mod.LocationReferenceTypes.Add(new LocationReferenceType(Key(0x803), SkyrimRelease.SkyrimSE));
        mod.LocationReferenceTypes.Add(new LocationReferenceType(Key(0x80D), SkyrimRelease.SkyrimSE));
        foreach (var id in new uint[] { 0x807, 0x80A, 0x80B, 0x80C, 0x881, 0x910 })
            cell.Temporary.Add(new PlacedObject(Key(id), SkyrimRelease.SkyrimSE));
    }
}
