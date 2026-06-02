namespace SDK.Data.Tests;

using SDK.Data.Extensions;
using SDK.Data.Seeding;
using FluentAssertions;

public class Phase1EndToEndTests
{
    [Fact]
    public void Phase1_Can_Persist_Query_By_Generation_And_Read_In_5_Locales()
    {
        using var fixture = new SqliteTestFixture();
        using (var ctx = fixture.CreateContext())
            DataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        var gen1Species = ctx2.GetSpeciesByGeneration(1).Select(s => s.Identifier).ToList();
        gen1Species.Should().Contain("bulbasaur");
        gen1Species.Should().Contain("pikachu");
        gen1Species.Should().NotContain("togepi");

        using var ctx3 = fixture.CreateContext();
        var locales = new[] { "en", "fr", "de", "es", "ja" };
        foreach (var locale in locales)
        {
            var name = ctx3.GetTranslation("PokemonSpecies", 1, locale, "name");
            name.Should().NotBeNullOrEmpty($"Bulbasaur doit avoir un nom en {locale}");
        }

        ctx3.GetTranslation("PokemonSpecies", 1, "fr", "name").Should().Be("Bulbizarre");
        ctx3.GetTranslation("PokemonSpecies", 25, "ja", "name").Should().Be("ピカチュウ");

        using var ctx4 = fixture.CreateContext();
        var distinctLocales = ctx4.Translations
            .Where(t => t.EntityType == "PokemonSpecies")
            .Select(t => t.Locale)
            .Distinct()
            .ToList();
        distinctLocales.Should().HaveCount(5);
        distinctLocales.Should().BeEquivalentTo(locales);
    }
}
