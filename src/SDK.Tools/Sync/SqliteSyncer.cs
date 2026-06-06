namespace SDK.Tools.Sync;

using Microsoft.EntityFrameworkCore;
using SDK.Core.Entities;
using SDK.Data;
using SDK.Tools.Atlas;

public sealed class SqliteSyncer
{
    private readonly string _dbPath;

    public SqliteSyncer(string dbPath)
    {
        _dbPath = dbPath;
    }

    public int Sync(IEnumerable<AtlasEntry> entries, string atlasPath)
    {
        var opts = new DbContextOptionsBuilder<PokemonDbContext>()
            .UseSqlite($"Data Source={_dbPath}")
            .Options;

        using var ctx = new PokemonDbContext(opts);
        ctx.Database.EnsureCreated();

        int count = 0;
        foreach (var entry in entries)
        {
            var existing = ctx.SpriteAtlasEntries
                .FirstOrDefault(e => e.AssetKey == entry.AssetKey);

            if (existing is null)
            {
                ctx.SpriteAtlasEntries.Add(new SpriteAtlasEntry
                {
                    AssetKey  = entry.AssetKey,
                    View      = entry.View,
                    AtlasPath = atlasPath,
                    X         = entry.X,
                    Y         = entry.Y,
                    Width     = entry.Width,
                    Height    = entry.Height,
                });
            }
            else
            {
                existing.AtlasPath = atlasPath;
                existing.X         = entry.X;
                existing.Y         = entry.Y;
                existing.Width     = entry.Width;
                existing.Height    = entry.Height;
                existing.View      = entry.View;
            }
            count++;
        }
        ctx.SaveChanges();
        return count;
    }
}
