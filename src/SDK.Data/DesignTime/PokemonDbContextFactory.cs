namespace SDK.Data.DesignTime;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class PokemonDbContextFactory : IDesignTimeDbContextFactory<PokemonDbContext>
{
    public PokemonDbContext CreateDbContext(string[] args)
    {
        var dbPath = Path.GetFullPath(
            Path.Combine(Directory.GetCurrentDirectory(), "data", "PokemonSDK.db"));
        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);
        var options = new DbContextOptionsBuilder<PokemonDbContext>()
            .UseSqlite($"DataSource={dbPath}")
            .Options;
        return new PokemonDbContext(options);
    }
}
