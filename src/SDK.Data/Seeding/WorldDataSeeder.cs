namespace SDK.Data.Seeding;

using SDK.Core.Entities;
using SDK.Core.Enums;

public static class WorldDataSeeder
{
    public static void SeedAll(PokemonDbContext ctx) => SeedEncounterZones(ctx);

    public static void SeedEncounterZones(PokemonDbContext ctx)
    {
        if (ctx.EncounterZones.Any()) return;

        ctx.EncounterZones.AddRange(
            new EncounterZone { Id = 1, ZoneIdentifier = "pallet-route-1",  Generation = 1, BiomeType = BiomeType.Grass, SpeciesId = 1,   MinLevel = 2, MaxLevel = 5, SpawnRate = 0.10m },
            new EncounterZone { Id = 2, ZoneIdentifier = "pallet-route-1",  Generation = 1, BiomeType = BiomeType.Grass, SpeciesId = 25,  MinLevel = 3, MaxLevel = 6, SpawnRate = 0.15m },
            new EncounterZone { Id = 3, ZoneIdentifier = "pallet-route-1",  Generation = 1, BiomeType = BiomeType.Grass, SpeciesId = 175, MinLevel = 2, MaxLevel = 4, SpawnRate = 0.05m },
            new EncounterZone { Id = 4, ZoneIdentifier = "viridian-forest", Generation = 1, BiomeType = BiomeType.Cave,  SpeciesId = 25,  MinLevel = 4, MaxLevel = 7, SpawnRate = 0.20m },
            new EncounterZone { Id = 5, ZoneIdentifier = "viridian-forest", Generation = 1, BiomeType = BiomeType.Grass, SpeciesId = 1,   MinLevel = 3, MaxLevel = 6, SpawnRate = 0.12m }
        );
        ctx.SaveChanges();
    }
}
