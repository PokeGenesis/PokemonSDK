namespace SDK.Data.Seeding;

using SDK.Core.Entities;

public static class FakemonDataSeeder
{
    public static void Seed(PokemonDbContext ctx)
    {
        if (ctx.FakemonSpecies.Any()) return;

        var sample = new FakemonSpecies
        {
            Id             = 1,
            Identifier     = "test-dragon",
            Generation     = 1,
            BaseHp         = 45,
            BaseAttack     = 60,
            BaseDefense    = 55,
            BaseSpecialAtk = 70,
            BaseSpecialDef = 55,
            BaseSpeed      = 50,
            Type1Id        = 15,
            IsLegendary    = false,
            EggGroup1      = "dragon",
            PartsManifest  = null,
        };
        ctx.FakemonSpecies.Add(sample);
        ctx.SaveChanges();

        if (ctx.Translations.Any(t => t.EntityType == "FakemonSpecies")) return;

        (string locale, string value)[] translations =
        [
            ("en", "Test Dragon"),
            ("es", "Dragón Prueba"),
            ("fr", "Dragon Test"),
            ("de", "Testdrache"),
            ("it", "Drago Test"),
            ("ja", "テストドラゴン"),
        ];

        foreach (var (locale, value) in translations)
        {
            ctx.Translations.Add(new Translation
            {
                EntityType = "FakemonSpecies",
                EntityId   = sample.Id,
                Locale     = locale,
                Value      = value,
            });
        }
        ctx.SaveChanges();
    }
}
