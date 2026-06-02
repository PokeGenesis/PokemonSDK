using Microsoft.EntityFrameworkCore;
using SDK.Data;
using SDK.Data.Seeding;

if (args.Length == 0 || args[0] != "seed")
{
    Console.Error.WriteLine("Usage: SDK.Tools seed <db-path>");
    Console.Error.WriteLine("  seed <db-path>  — applies migrations and seeds reference data");
    return 1;
}

var dbPath = args.Length > 1 ? args[1] : "data/PokemonSDK.db";
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(dbPath))!);

var options = new DbContextOptionsBuilder<PokemonDbContext>()
    .UseSqlite($"DataSource={dbPath}")
    .Options;

using var ctx = new PokemonDbContext(options);
ctx.Database.Migrate();
DataSeeder.SeedAll(ctx);

Console.WriteLine($"Seed complete: {ctx.PokemonTypes.Count()} types in {dbPath}");
return 0;
