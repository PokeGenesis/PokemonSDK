using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using SDK.Data;
using SDK.Data.Seeding;

namespace PokeForge.Cli.Commands;

public static class SeedCommand
{
    public static void Register(RootCommand root)
    {
        var cmd = new Command("seed", "Apply migrations and seed Pokémon reference data");
        var dbOpt = new Option<string>(
            "--db",
            () => "src/SDK.Data/data/PokemonSDK.db",
            "Path to SQLite database");
        cmd.AddOption(dbOpt);
        cmd.SetHandler((db) => Environment.Exit(Execute(db)), dbOpt);
        root.AddCommand(cmd);
    }

    internal static int Execute(string dbPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(dbPath))!);

        var options = new DbContextOptionsBuilder<PokemonDbContext>()
            .UseSqlite($"Data Source={dbPath}")
            .Options;

        using var ctx = new PokemonDbContext(options);
        ctx.Database.Migrate();
        DataSeeder.SeedAll(ctx);

        Console.WriteLine($"Seed complete: {ctx.PokemonTypes.Count()} types in {dbPath}");
        return 0;
    }
}
