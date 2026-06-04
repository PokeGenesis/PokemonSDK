namespace SDK.Data.Tests;

using FluentAssertions;
using SDK.Core.Enums;
using SDK.Data.Seeding;

public class BattleDataSeederTests
{
    [Fact]
    public void SeedTypeEffectiveness_Gen1_HasNonNeutralEntries()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        DataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        ctx2.TypeEffectiveness.Count().Should().BeGreaterThanOrEqualTo(50);
        ctx2.TypeEffectiveness.All(e => e.Generation == 1).Should().BeTrue();
        ctx2.TypeEffectiveness.All(e => e.DamageFactor != 1.0m).Should().BeTrue();
    }

    [Fact]
    public void SeedMoves_Gen1_HasAllThreeCategories()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        DataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        ctx2.Moves.Any(m => m.Category == MoveCategory.Physical).Should().BeTrue();
        ctx2.Moves.Any(m => m.Category == MoveCategory.Special).Should().BeTrue();
        ctx2.Moves.Any(m => m.Category == MoveCategory.Status).Should().BeTrue();
    }

    [Fact]
    public void SeedMoves_Tackle_IsCorrect()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        DataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        var tackle = ctx2.Moves.Single(m => m.Identifier == "tackle");
        tackle.Power.Should().Be(35);
        tackle.Category.Should().Be(MoveCategory.Physical);
        tackle.Generation.Should().Be(1);
    }

    [Fact]
    public void SeedAbilities_HasExpectedCount()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        DataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        ctx2.Abilities.Count().Should().BeGreaterThanOrEqualTo(5);
    }
}
