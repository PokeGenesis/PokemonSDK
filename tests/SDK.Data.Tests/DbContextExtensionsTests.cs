namespace SDK.Data.Tests;

using FluentAssertions;
using SDK.Core.Entities;
using SDK.Data.Extensions;

public class DbContextExtensionsTests : IClassFixture<SqliteTestFixture>
{
    private readonly SqliteTestFixture _fixture;

    public DbContextExtensionsTests(SqliteTestFixture fixture) => _fixture = fixture;

    [Fact]
    public void GetSpeciesByGeneration_ReturnsOnlySpeciesUpToMaxGen()
    {
        using var ctx = _fixture.CreateContext();
        ctx.PokemonSpecies.AddRange(
            new PokemonSpecies { Identifier = "bulbasaur-ext",   Generation = 1, Type1Id = 1 },
            new PokemonSpecies { Identifier = "togepi-ext",      Generation = 2, Type1Id = 1 },
            new PokemonSpecies { Identifier = "ralts-ext",       Generation = 3, Type1Id = 1 }
        );
        ctx.SaveChanges();

        using var ctx2 = _fixture.CreateContext();
        var result = ctx2.GetSpeciesByGeneration(2).Select(s => s.Identifier).ToList();

        result.Should().Contain("bulbasaur-ext");
        result.Should().Contain("togepi-ext");
        result.Should().NotContain("ralts-ext");
    }

    [Fact]
    public void GetTypesByGeneration_ExcludesFairyBeforeGen6()
    {
        using var ctx = _fixture.CreateContext();
        ctx.PokemonTypes.AddRange(
            new PokemonType { Identifier = "dark-ext",  Generation = 2 },
            new PokemonType { Identifier = "fairy-ext", Generation = 6 }
        );
        ctx.SaveChanges();

        using var ctx2 = _fixture.CreateContext();
        var result = ctx2.GetTypesByGeneration(5).Select(t => t.Identifier).ToList();

        result.Should().Contain("dark-ext");
        result.Should().NotContain("fairy-ext");
    }

    [Fact]
    public void GetTranslation_ReturnsCorrectValue()
    {
        using var ctx = _fixture.CreateContext();
        ctx.Translations.Add(new Translation
        {
            EntityType = "PokemonType", EntityId = 5,
            Locale = "fr", Field = "name", Value = "Plante"
        });
        ctx.SaveChanges();

        using var ctx2 = _fixture.CreateContext();
        var value = ctx2.GetTranslation("PokemonType", 5, "fr", "name");

        value.Should().Be("Plante");
    }
}
