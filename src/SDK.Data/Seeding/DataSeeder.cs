namespace SDK.Data.Seeding;

using SDK.Core.Entities;

public static class DataSeeder
{
    public static void SeedAll(PokemonDbContext ctx)
    {
        SeedTypes(ctx);
        SeedTypeTranslations(ctx);
        SeedSpecies(ctx);
        SeedSpeciesTranslations(ctx);
        BattleDataSeeder.SeedAll(ctx);
        ProgressionDataSeeder.SeedAll(ctx);
    }

    public static void SeedTypes(PokemonDbContext ctx)
    {
        if (ctx.PokemonTypes.Any()) return;

        ctx.PokemonTypes.AddRange(
            new PokemonType { Id =  1, Identifier = "normal",   Generation = 1 },
            new PokemonType { Id =  2, Identifier = "fire",     Generation = 1 },
            new PokemonType { Id =  3, Identifier = "water",    Generation = 1 },
            new PokemonType { Id =  4, Identifier = "electric", Generation = 1 },
            new PokemonType { Id =  5, Identifier = "grass",    Generation = 1 },
            new PokemonType { Id =  6, Identifier = "ice",      Generation = 1 },
            new PokemonType { Id =  7, Identifier = "fighting",  Generation = 1 },
            new PokemonType { Id =  8, Identifier = "poison",   Generation = 1 },
            new PokemonType { Id =  9, Identifier = "ground",   Generation = 1 },
            new PokemonType { Id = 10, Identifier = "flying",   Generation = 1 },
            new PokemonType { Id = 11, Identifier = "psychic",  Generation = 1 },
            new PokemonType { Id = 12, Identifier = "bug",      Generation = 1 },
            new PokemonType { Id = 13, Identifier = "rock",     Generation = 1 },
            new PokemonType { Id = 14, Identifier = "ghost",    Generation = 1 },
            new PokemonType { Id = 15, Identifier = "dragon",   Generation = 1 },
            new PokemonType { Id = 16, Identifier = "dark",     Generation = 2 },
            new PokemonType { Id = 17, Identifier = "steel",    Generation = 2 },
            new PokemonType { Id = 18, Identifier = "fairy",    Generation = 6 }
        );
        ctx.SaveChanges();
    }

    public static void SeedTypeTranslations(PokemonDbContext ctx)
    {
        if (ctx.Translations.Any(t => t.EntityType == "PokemonType")) return;

        var translations = new (int id, string en, string es, string fr, string de, string it, string ja)[]
        {
            ( 1, "Normal",   "Normal",    "Normal",   "Normal",  "Normale",    "ノーマル"),
            ( 2, "Fire",     "Fuego",     "Feu",      "Feuer",   "Fuoco",      "ほのお"),
            ( 3, "Water",    "Agua",      "Eau",      "Wasser",  "Acqua",      "みず"),
            ( 4, "Electric", "Eléctrico", "Électrik", "Elektro", "Elettro",    "でんき"),
            ( 5, "Grass",    "Planta",    "Plante",   "Pflanze", "Erba",       "くさ"),
            ( 6, "Ice",      "Hielo",     "Glace",    "Eis",     "Ghiaccio",   "こおり"),
            ( 7, "Fighting", "Lucha",     "Combat",   "Kampf",   "Lotta",      "かくとう"),
            ( 8, "Poison",   "Veneno",    "Poison",   "Gift",    "Veleno",     "どく"),
            ( 9, "Ground",   "Tierra",    "Sol",      "Boden",   "Terra",      "じめん"),
            (10, "Flying",   "Volador",   "Vol",      "Flug",    "Volante",    "ひこう"),
            (11, "Psychic",  "Psíquico",  "Psy",      "Psycho",  "Psico",      "エスパー"),
            (12, "Bug",      "Bicho",     "Insecte",  "Käfer",   "Coleottero", "むし"),
            (13, "Rock",     "Roca",      "Roche",    "Gestein", "Roccia",     "いわ"),
            (14, "Ghost",    "Fantasma",  "Spectre",  "Geist",   "Spettro",    "ゴースト"),
            (15, "Dragon",   "Dragón",    "Dragon",   "Drache",  "Drago",      "ドラゴン"),
            (16, "Dark",     "Siniestro", "Ténèbres", "Unlicht", "Buio",       "あく"),
            (17, "Steel",    "Acero",     "Acier",    "Stahl",   "Acciaio",    "はがね"),
            (18, "Fairy",    "Hada",      "Fée",      "Fee",     "Folletto",   "フェアリー"),
        };

        foreach (var (id, en, es, fr, de, it, ja) in translations)
        {
            foreach (var (locale, value) in new[] { ("en", en), ("es", es), ("fr", fr), ("de", de), ("it", it), ("ja", ja) })
                ctx.Translations.Add(new Translation
                {
                    EntityType = "PokemonType", EntityId = id,
                    Locale = locale, Field = "name", Value = value
                });
        }
        ctx.SaveChanges();
    }

    public static void SeedSpecies(PokemonDbContext ctx)
    {
        if (ctx.PokemonSpecies.Any()) return;

        ctx.PokemonSpecies.AddRange(
            new PokemonSpecies { Id =   1, Identifier = "bulbasaur", Generation = 1, Type1Id = 5, Type2Id = 8 },
            new PokemonSpecies { Id =  25, Identifier = "pikachu",   Generation = 1, Type1Id = 4, Type2Id = null },
            new PokemonSpecies { Id = 175, Identifier = "togepi",    Generation = 2, Type1Id = 1, Type2Id = null }
        );
        ctx.SaveChanges();
    }

    public static void SeedSpeciesTranslations(PokemonDbContext ctx)
    {
        if (ctx.Translations.Any(t => t.EntityType == "PokemonSpecies")) return;

        var data = new (int id, string en, string es, string fr, string de, string it, string ja)[]
        {
            (  1, "Bulbasaur", "Bulbasaur", "Bulbizarre", "Bisasam",  "Bulbasaur", "フシギダネ"),
            ( 25, "Pikachu",   "Pikachu",   "Pikachu",    "Pikachu",  "Pikachu",   "ピカチュウ"),
            (175, "Togepi",    "Togepi",    "Togepi",     "Togepi",   "Togepi",    "トゲピー"),
        };

        foreach (var (id, en, es, fr, de, it, ja) in data)
        {
            foreach (var (locale, value) in new[] { ("en", en), ("es", es), ("fr", fr), ("de", de), ("it", it), ("ja", ja) })
                ctx.Translations.Add(new Translation
                {
                    EntityType = "PokemonSpecies", EntityId = id,
                    Locale = locale, Field = "name", Value = value
                });
        }
        ctx.SaveChanges();
    }
}
