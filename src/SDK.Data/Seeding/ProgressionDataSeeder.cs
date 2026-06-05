namespace SDK.Data.Seeding;

using SDK.Core.Entities;

public static class ProgressionDataSeeder
{
    public static void SeedAll(PokemonDbContext ctx)
    {
        SeedTrainers(ctx);
        SeedBadges(ctx);
        SeedBadgeTranslations(ctx);
    }

    public static void SeedTrainers(PokemonDbContext ctx)
    {
        if (ctx.Trainers.Any()) return;

        ctx.Trainers.AddRange(
            new Trainer { Id = 1, Identifier = "brock",    Generation = 1 },
            new Trainer { Id = 2, Identifier = "misty",    Generation = 1 },
            new Trainer { Id = 3, Identifier = "lt-surge", Generation = 1 },
            new Trainer { Id = 4, Identifier = "erika",    Generation = 1 },
            new Trainer { Id = 5, Identifier = "koga",     Generation = 1 },
            new Trainer { Id = 6, Identifier = "sabrina",  Generation = 1 },
            new Trainer { Id = 7, Identifier = "blaine",   Generation = 1 },
            new Trainer { Id = 8, Identifier = "giovanni", Generation = 1 }
        );
        ctx.SaveChanges();
    }

    public static void SeedBadges(PokemonDbContext ctx)
    {
        if (ctx.Badges.Any()) return;

        ctx.Badges.AddRange(
            new Badge { Id = 1, Identifier = "boulder", Generation = 1, GymLeaderId = 1 },
            new Badge { Id = 2, Identifier = "cascade",  Generation = 1, GymLeaderId = 2 },
            new Badge { Id = 3, Identifier = "thunder",  Generation = 1, GymLeaderId = 3 },
            new Badge { Id = 4, Identifier = "rainbow",  Generation = 1, GymLeaderId = 4 },
            new Badge { Id = 5, Identifier = "soul",     Generation = 1, GymLeaderId = 5 },
            new Badge { Id = 6, Identifier = "marsh",    Generation = 1, GymLeaderId = 6 },
            new Badge { Id = 7, Identifier = "volcano",  Generation = 1, GymLeaderId = 7 },
            new Badge { Id = 8, Identifier = "earth",    Generation = 1, GymLeaderId = 8 }
        );
        ctx.SaveChanges();
    }

    public static void SeedBadgeTranslations(PokemonDbContext ctx)
    {
        if (ctx.Translations.Any(t => t.EntityType == "Badge")) return;

        var translations = new (int id, string en, string es, string fr, string de, string it, string ja)[]
        {
            (1, "Boulder Badge", "Medalla Roca",     "Badge Pierre",      "Felsorden",       "Medaglia Roccia",    "グレーバッジ"),
            (2, "Cascade Badge", "Medalla Cascada",  "Badge Cascade",     "Kaskadenorden",   "Medaglia Cascata",   "ブルーバッジ"),
            (3, "Thunder Badge", "Medalla Trueno",   "Badge Tonnerre",    "Donnerorden",     "Medaglia Tuono",     "イエローバッジ"),
            (4, "Rainbow Badge", "Medalla Arcoíris", "Badge Arc-en-ciel", "Regenbogenorden", "Medaglia Arcobaleno","グリーンバッジ"),
            (5, "Soul Badge",    "Medalla Alma",     "Badge Âme",         "Seelenorden",     "Medaglia Anima",     "ゴールドバッジ"),
            (6, "Marsh Badge",   "Medalla Pantano",  "Badge Marécage",    "Sumpforden",      "Medaglia Palude",    "クリムゾンバッジ"),
            (7, "Volcano Badge", "Medalla Volcán",   "Badge Volcan",      "Vulkanorden",     "Medaglia Vulcano",   "マグマバッジ"),
            (8, "Earth Badge",   "Medalla Tierra",   "Badge Terre",       "Erdorden",        "Medaglia Terra",     "アースバッジ"),
        };

        foreach (var (id, en, es, fr, de, it, ja) in translations)
        {
            foreach (var (locale, value) in new[] { ("en", en), ("es", es), ("fr", fr), ("de", de), ("it", it), ("ja", ja) })
                ctx.Translations.Add(new Translation
                {
                    EntityType = "Badge",
                    EntityId   = id,
                    Locale     = locale,
                    Field      = "name",
                    Value      = value
                });
        }
        ctx.SaveChanges();
    }
}
