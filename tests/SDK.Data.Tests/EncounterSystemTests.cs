namespace SDK.Data.Tests;

using FluentAssertions;
using SDK.Data.Seeding;
using SDK.Data.Services;

public class EncounterSystemTests
{
    private static EncounterSystem CreateSeeded(out SqliteTestFixture fixture)
    {
        fixture = new SqliteTestFixture();
        using var seedCtx = fixture.CreateContext();
        DataSeeder.SeedAll(seedCtx);
        WorldDataSeeder.SeedAll(seedCtx);
        return new EncounterSystem(fixture.CreateContext());
    }

    [Fact]
    public void GetZones_ReturnsZonesForKnownGeneration()
    {
        var sut = CreateSeeded(out var fixture);
        using var _ = fixture;
        sut.GetZones(1).Should().NotBeEmpty();
    }

    [Fact]
    public void GetZones_ReturnsEmptyForUnknownGeneration()
    {
        var sut = CreateSeeded(out var fixture);
        using var _ = fixture;
        sut.GetZones(99).Should().BeEmpty();
    }

    [Fact]
    public void GetZonesByIdentifier_ReturnsMatchingZones()
    {
        var sut = CreateSeeded(out var fixture);
        using var _ = fixture;
        sut.GetZonesByIdentifier("pallet-route-1", 1).Should().HaveCount(3);
    }

    [Fact]
    public void GetZonesByIdentifier_ReturnsEmptyForUnknownZone()
    {
        var sut = CreateSeeded(out var fixture);
        using var _ = fixture;
        sut.GetZonesByIdentifier("unknown-zone", 1).Should().BeEmpty();
    }

    [Fact]
    public void GetZonesByIdentifier_AllSpawnRatesPositive()
    {
        var sut = CreateSeeded(out var fixture);
        using var _ = fixture;
        sut.GetZonesByIdentifier("pallet-route-1", 1)
           .Should().AllSatisfy(z => z.SpawnRate.Should().BePositive());
    }
}
