namespace SDK.Tools.Tests;

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SDK.Data;
using SDK.Tools.Atlas;
using SDK.Tools.Sync;

public sealed class SqliteSyncerTests
{
    private static PokemonDbContext CreateInMemoryContext()
    {
        var opts = new DbContextOptionsBuilder<PokemonDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;
        var ctx = new PokemonDbContext(opts);
        ctx.Database.OpenConnection();
        ctx.Database.EnsureCreated();
        return ctx;
    }

    // SqliteSyncer prend un dbPath — pour les tests :memory:, on passe via une fixture légère
    private sealed class InMemorySyncer : IDisposable
    {
        private readonly PokemonDbContext _ctx;
        private readonly string _tmpDb;

        public InMemorySyncer()
        {
            _tmpDb = Path.Combine(Path.GetTempPath(), $"synctest_{Guid.NewGuid():N}.db");
        }

        public SqliteSyncer Syncer => new(_tmpDb);
        public string DbPath => _tmpDb;

        public PokemonDbContext OpenContext()
        {
            var opts = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseSqlite($"Data Source={_tmpDb}")
                .Options;
            var ctx = new PokemonDbContext(opts);
            ctx.Database.EnsureCreated();
            return ctx;
        }

        public void Dispose()
        {
            if (File.Exists(_tmpDb)) File.Delete(_tmpDb);
        }
    }

    [Fact]
    public void Sync_NewEntries_InsertsRows()
    {
        using var fixture = new InMemorySyncer();
        var entries = new[]
        {
            new AtlasEntry("00025_pikachu_front", "front", 0, 0, 96, 96),
            new AtlasEntry("00025_pikachu_back", "back", 96, 0, 96, 96),
        };

        var count = fixture.Syncer.Sync(entries, "Content/atlas/atlas.png");

        count.Should().Be(2);
        using var ctx = fixture.OpenContext();
        ctx.SpriteAtlasEntries.Count().Should().Be(2);
    }

    [Fact]
    public void Sync_SameAssetKey_UpdatesNotInserts()
    {
        using var fixture = new InMemorySyncer();
        var entry = new AtlasEntry("00001_bulbasaur_front", "front", 0, 0, 96, 96);

        fixture.Syncer.Sync([entry], "atlas.png");
        fixture.Syncer.Sync([entry], "atlas.png");

        using var ctx = fixture.OpenContext();
        ctx.SpriteAtlasEntries.Count().Should().Be(1);
    }

    [Fact]
    public void Sync_UpdatesCoordinates_OnRerun()
    {
        using var fixture = new InMemorySyncer();
        var first  = new AtlasEntry("00006_charizard_front", "front", 0, 0, 96, 96);
        var second = new AtlasEntry("00006_charizard_front", "front", 128, 0, 96, 96);

        fixture.Syncer.Sync([first], "atlas_v1.png");
        fixture.Syncer.Sync([second], "atlas_v2.png");

        using var ctx = fixture.OpenContext();
        var row = ctx.SpriteAtlasEntries.Single();
        row.X.Should().Be(128);
        row.AtlasPath.Should().Be("atlas_v2.png");
    }

    [Fact]
    public void Sync_EmptyList_ReturnsZero()
    {
        using var fixture = new InMemorySyncer();

        var count = fixture.Syncer.Sync([], "atlas.png");

        count.Should().Be(0);
        using var ctx = fixture.OpenContext();
        ctx.SpriteAtlasEntries.Should().BeEmpty();
    }
}
