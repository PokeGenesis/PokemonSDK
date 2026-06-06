namespace SDK.Data.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SDK.Core.Entities;
using SDK.Data.Seeding;

public class CharacterTranslationsD22Tests
{
    [Fact]
    public void AllCharacters_HaveExactlySixTranslations()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        CharacterDataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        var counts = ctx2.Translations
            .Where(t => t.EntityType == "Character")
            .GroupBy(t => t.EntityId)
            .Select(g => g.Count())
            .ToList();

        counts.Should().NotBeEmpty();
        counts.Should().AllSatisfy(c => c.Should().Be(6));
    }

    [Fact]
    public void AllVillainGroups_HaveExactlySixTranslations()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        CharacterDataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        var counts = ctx2.Translations
            .Where(t => t.EntityType == "VillainGroup")
            .GroupBy(t => t.EntityId)
            .Select(g => g.Count())
            .ToList();

        counts.Should().NotBeEmpty();
        counts.Should().AllSatisfy(c => c.Should().Be(6));
    }

    [Fact]
    public void AllSixLocales_PresentForCharacter_Ash()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        CharacterDataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        var locales = ctx2.Translations
            .Where(t => t.EntityType == "Character" && t.EntityId == 1)
            .Select(t => t.Locale)
            .ToList();

        locales.Should().Contain(new[] { "en", "es", "fr", "de", "it", "ja" });
    }
}

public class CharacterSeederIntegrationTests
{
    [Fact]
    public void CharacterDataSeeder_Seeds_FiveCharacters()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        CharacterDataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        ctx2.Characters.Count().Should().Be(5);
    }

    [Fact]
    public void CharacterDataSeeder_Seeds_OneVillainGroup()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        CharacterDataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        ctx2.VillainGroups.Count().Should().Be(1);
        ctx2.VillainGroups.First().Identifier.Should().Be("team-rocket");
    }

    [Fact]
    public void CharacterDataSeeder_Seeds_TwoVillainMembers()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        CharacterDataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        ctx2.VillainMembers.Count().Should().Be(2);
    }

    [Fact]
    public void CharacterDataSeeder_IsIdempotent()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();
        CharacterDataSeeder.SeedAll(ctx);
        CharacterDataSeeder.SeedAll(ctx);

        using var ctx2 = fixture.CreateContext();
        ctx2.Characters.Count().Should().Be(5);
        ctx2.Translations.Count(t => t.EntityType == "Character").Should().Be(30);
    }
}

public class CharacterCrudTests
{
    [Fact]
    public void Character_CanBeCreatedAndQueried()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();

        ctx.Characters.Add(new Character { Id = 99, Identifier = "misty", Role = "GymLeader", Generation = 1 });
        ctx.SaveChanges();

        using var ctx2 = fixture.CreateContext();
        var found = ctx2.Characters.FirstOrDefault(c => c.Identifier == "misty");
        found.Should().NotBeNull();
        found!.Role.Should().Be("GymLeader");
    }

    [Fact]
    public void VillainMember_LinksCharacterToVillainGroup()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();

        ctx.Characters.Add(new Character { Id = 10, Identifier = "meowth", Role = "Antagonist", Generation = 1 });
        ctx.VillainGroups.Add(new VillainGroup { Id = 10, Identifier = "team-rocket-test", Generation = 1 });
        ctx.SaveChanges();
        ctx.VillainMembers.Add(new VillainMember { Id = 10, CharacterId = 10, VillainGroupId = 10 });
        ctx.SaveChanges();

        using var ctx2 = fixture.CreateContext();
        var member = ctx2.VillainMembers
            .Include(m => m.Character)
            .Include(m => m.VillainGroup)
            .First(m => m.Id == 10);

        member.Character.Identifier.Should().Be("meowth");
        member.VillainGroup.Identifier.Should().Be("team-rocket-test");
    }

    [Fact]
    public void Character_Identifier_IsUnique()
    {
        using var fixture = new SqliteTestFixture();
        using var ctx = fixture.CreateContext();

        ctx.Characters.Add(new Character { Id = 20, Identifier = "brock-char", Role = "GymLeader", Generation = 1 });
        ctx.SaveChanges();

        using var ctx2 = fixture.CreateContext();
        ctx2.Characters.Add(new Character { Id = 21, Identifier = "brock-char", Role = "GymLeader", Generation = 1 });
        var act = () => ctx2.SaveChanges();
        act.Should().Throw<DbUpdateException>();
    }
}
