namespace SDK.Data.Seeding;

using SDK.Core.Entities;

public static class CharacterDataSeeder
{
    public static void SeedAll(PokemonDbContext ctx)
    {
        SeedCharacters(ctx);
        SeedCharacterTranslations(ctx);
        SeedVillainGroups(ctx);
        SeedVillainGroupTranslations(ctx);
        SeedVillainMembers(ctx);
    }

    private static void SeedCharacters(PokemonDbContext ctx)
    {
        if (ctx.Characters.Any()) return;

        ctx.Characters.AddRange(
            new Character { Id = 1, Identifier = "ash-ketchum", Role = "Rival",      Generation = 1 },
            new Character { Id = 2, Identifier = "gary-oak",    Role = "Rival",      Generation = 1 },
            new Character { Id = 3, Identifier = "red",         Role = "Champion",   Generation = 1 },
            new Character { Id = 4, Identifier = "jessie",      Role = "Antagonist", Generation = 1 },
            new Character { Id = 5, Identifier = "james",       Role = "Antagonist", Generation = 1 }
        );
        ctx.SaveChanges();
    }

    private static void SeedCharacterTranslations(PokemonDbContext ctx)
    {
        if (ctx.Translations.Any(t => t.EntityType == "Character")) return;

        (int id, string en, string es, string fr, string de, string it, string ja)[] data =
        [
            (1, "Ash Ketchum",  "Ash Ketchum",  "Sacha Ketchum", "Ash Ketchum", "Ash Ketchum", "サトシ"),
            (2, "Gary Oak",     "Gary Oak",     "Pierre Carpin", "Gary Oak",    "Gary Oak",    "シゲル"),
            (3, "Red",          "Rojo",         "Rouge",         "Rot",         "Rosso",       "レッド"),
            (4, "Jessie",       "Jessie",       "Jessie",        "Jessie",      "Jessie",      "ムサシ"),
            (5, "James",        "James",        "James",         "James",       "James",       "コジロウ"),
        ];

        foreach (var (id, en, es, fr, de, it, ja) in data)
        {
            foreach (var (locale, value) in new[] { ("en", en), ("es", es), ("fr", fr), ("de", de), ("it", it), ("ja", ja) })
            {
                ctx.Translations.Add(new Translation
                {
                    EntityType = "Character",
                    EntityId   = id,
                    Locale     = locale,
                    Field      = "name",
                    Value      = value
                });
            }
        }
        ctx.SaveChanges();
    }

    private static void SeedVillainGroups(PokemonDbContext ctx)
    {
        if (ctx.VillainGroups.Any()) return;

        ctx.VillainGroups.Add(new VillainGroup { Id = 1, Identifier = "team-rocket", Generation = 1 });
        ctx.SaveChanges();
    }

    private static void SeedVillainGroupTranslations(PokemonDbContext ctx)
    {
        if (ctx.Translations.Any(t => t.EntityType == "VillainGroup")) return;

        (int id, string en, string es, string fr, string de, string it, string ja)[] data =
        [
            (1, "Team Rocket", "Equipo Rocket", "Team Rocket", "Team Rocket", "Team Rocket", "ロケット団"),
        ];

        foreach (var (id, en, es, fr, de, it, ja) in data)
        {
            foreach (var (locale, value) in new[] { ("en", en), ("es", es), ("fr", fr), ("de", de), ("it", it), ("ja", ja) })
            {
                ctx.Translations.Add(new Translation
                {
                    EntityType = "VillainGroup",
                    EntityId   = id,
                    Locale     = locale,
                    Field      = "name",
                    Value      = value
                });
            }
        }
        ctx.SaveChanges();
    }

    private static void SeedVillainMembers(PokemonDbContext ctx)
    {
        if (ctx.VillainMembers.Any()) return;

        ctx.VillainMembers.AddRange(
            new VillainMember { Id = 1, CharacterId = 4, VillainGroupId = 1 },
            new VillainMember { Id = 2, CharacterId = 5, VillainGroupId = 1 }
        );
        ctx.SaveChanges();
    }
}
