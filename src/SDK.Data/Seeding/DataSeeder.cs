namespace SDK.Data.Seeding;

using SDK.Core.Entities;

public static class DataSeeder
{
    public static void SeedAll(PokemonDbContext ctx)
    {
        SeedTypes(ctx);
        SeedTypeTranslations(ctx);
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

        var translations = new (int id, string en, string fr)[]
        {
            ( 1, "Normal",   "Normal"),
            ( 2, "Fire",     "Feu"),
            ( 3, "Water",    "Eau"),
            ( 4, "Electric", "Électrik"),
            ( 5, "Grass",    "Plante"),
            ( 6, "Ice",      "Glace"),
            ( 7, "Fighting", "Combat"),
            ( 8, "Poison",   "Poison"),
            ( 9, "Ground",   "Sol"),
            (10, "Flying",   "Vol"),
            (11, "Psychic",  "Psy"),
            (12, "Bug",      "Insecte"),
            (13, "Rock",     "Roche"),
            (14, "Ghost",    "Spectre"),
            (15, "Dragon",   "Dragon"),
            (16, "Dark",     "Ténèbres"),
            (17, "Steel",    "Acier"),
            (18, "Fairy",    "Fée"),
        };

        foreach (var (id, en, fr) in translations)
        {
            ctx.Translations.Add(new Translation
            {
                EntityType = "PokemonType", EntityId = id,
                Locale = "en", Field = "name", Value = en
            });
            ctx.Translations.Add(new Translation
            {
                EntityType = "PokemonType", EntityId = id,
                Locale = "fr", Field = "name", Value = fr
            });
        }
        ctx.SaveChanges();
    }
}
