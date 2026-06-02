namespace SDK.Data.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SDK.Core.Entities;

public class PokemonDbContextTests : IClassFixture<SqliteTestFixture>
{
    private readonly SqliteTestFixture _fixture;

    public PokemonDbContextTests(SqliteTestFixture fixture) => _fixture = fixture;

    [Fact]
    public void CanCreateDatabase_AllTablesExist()
    {
        using var ctx = _fixture.CreateContext();
        ctx.PokemonSpecies.Should().NotBeNull();
        ctx.PokemonForms.Should().NotBeNull();
        ctx.PokemonBaseStats.Should().NotBeNull();
        ctx.Translations.Should().NotBeNull();
        ctx.PokemonTypes.Should().NotBeNull();
        ctx.TypeEffectiveness.Should().NotBeNull();
    }

    [Fact]
    public void CanAddAndQueryPokemonSpecies()
    {
        using var ctx = _fixture.CreateContext();
        var species = new PokemonSpecies
        {
            Identifier = "bulbasaur",
            Generation = 1,
            Type1Id = 1
        };
        ctx.PokemonSpecies.Add(species);
        ctx.SaveChanges();

        using var ctx2 = _fixture.CreateContext();
        var result = ctx2.PokemonSpecies.Single(s => s.Identifier == "bulbasaur");
        result.Generation.Should().Be(1);
        result.Type1Id.Should().Be(1);
    }

    [Fact]
    public void Translation_DuplicateUniqueKey_ThrowsDbUpdateException()
    {
        using var ctx = _fixture.CreateContext();
        var t1 = new Translation
        {
            EntityType = "PokemonSpecies", EntityId = 999,
            Locale = "fr", Field = "name", Value = "Bulbizarre"
        };
        var t2 = new Translation
        {
            EntityType = "PokemonSpecies", EntityId = 999,
            Locale = "fr", Field = "name", Value = "Bulbizarre2"
        };
        ctx.Translations.Add(t1);
        ctx.SaveChanges();
        ctx.Translations.Add(t2);
        var act = () => ctx.SaveChanges();
        act.Should().Throw<DbUpdateException>("D-07 : clé unique (EntityType, EntityId, Locale, Field) doit rejeter les doublons");
    }
}
