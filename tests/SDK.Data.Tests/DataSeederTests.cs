namespace SDK.Data.Tests;

using FluentAssertions;
using SDK.Data.Seeding;

public class DataSeederTests
{
    [Fact]
    public void SeedAll_Creates18Types()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();

        DataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        ctx2.PokemonTypes.Count().Should().Be(18);
    }

    [Fact]
    public void SeedAll_IsIdempotent()
    {
        using var fixture = new SqliteTestFixture();

        using (var ctx = fixture.CreateContext())
            DataSeeder.SeedAll(ctx);

        // second call — must not throw and must not duplicate
        using (var ctx = fixture.CreateContext())
        {
            var act = () => DataSeeder.SeedAll(ctx);
            act.Should().NotThrow();
        }

        using var ctx3 = fixture.CreateContext();
        ctx3.PokemonTypes.Count().Should().Be(18);
    }
}
