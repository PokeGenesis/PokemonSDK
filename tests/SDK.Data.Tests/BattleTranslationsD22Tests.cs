namespace SDK.Data.Tests;

using FluentAssertions;
using SDK.Data.Seeding;

public class BattleTranslationsD22Tests
{
    [Fact]
    public void MoveTranslations_AllSixLocales_Seeded()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        DataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        // 15 moves × 6 locales = 90
        ctx2.Translations.Count(t => t.EntityType == "Move").Should().Be(90);
    }

    [Fact]
    public void AbilityTranslations_AllSixLocales_Seeded()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        DataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        // 6 abilities × 6 locales = 36
        ctx2.Translations.Count(t => t.EntityType == "Ability").Should().Be(36);
    }
}
