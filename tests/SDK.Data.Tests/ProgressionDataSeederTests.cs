namespace SDK.Data.Tests;

using FluentAssertions;
using SDK.Data.Seeding;

public class ProgressionDataSeederTests
{
    [Fact]
    public void SeedAll_Creates8Trainers()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();

        ProgressionDataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        ctx2.Trainers.Count().Should().Be(8);
    }

    [Fact]
    public void SeedAll_Creates8Badges()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();

        ProgressionDataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        ctx2.Badges.Count().Should().Be(8);
    }

    [Fact]
    public void SeedBadgeTranslations_D22_48Rows()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();

        ProgressionDataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        ctx2.Translations.Count(t => t.EntityType == "Badge").Should().BeGreaterThanOrEqualTo(48);
    }

    [Fact]
    public void SeedAll_IsIdempotent()
    {
        using var fixture = new SqliteTestFixture();

        using (var ctx = fixture.CreateContext())
            ProgressionDataSeeder.SeedAll(ctx);

        using (var ctx = fixture.CreateContext())
        {
            var act = () => ProgressionDataSeeder.SeedAll(ctx);
            act.Should().NotThrow();
        }

        using var ctx3 = fixture.CreateContext();
        ctx3.Badges.Count().Should().Be(8);
    }
}
