using Microsoft.EntityFrameworkCore;
using SDK.Data;
using SDK.Data.Seeding;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: SDK.Tools <command> [options]");
    Console.Error.WriteLine("  seed [db-path]          — applies migrations and seeds reference data");
    Console.Error.WriteLine("  asset-validate [path]   — validates PNG sprites (D-16, exit code 1 if ERROR)");
    Console.Error.WriteLine("  default db-path: src/SDK.Data/data/PokemonSDK.db");
    Console.Error.WriteLine("  default assets-path: assets/sprites");
    return 1;
}

if (args[0] == "seed")
{
    var dbPath = args.Length > 1 ? args[1] : "src/SDK.Data/data/PokemonSDK.db";
    Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(dbPath))!);

    var options = new DbContextOptionsBuilder<PokemonDbContext>()
        .UseSqlite($"DataSource={dbPath}")
        .Options;

    using var ctx = new PokemonDbContext(options);
    ctx.Database.Migrate();
    DataSeeder.SeedAll(ctx);

    Console.WriteLine($"Seed complete: {ctx.PokemonTypes.Count()} types in {dbPath}");
    return 0;
}

if (args[0] == "asset-validate")
{
    var assetsPath = args.Length > 1 ? args[1] : "assets/sprites";
    var scanner    = new SDK.Tools.Validation.SpriteScanner();
    var validator  = new SDK.Tools.Validation.SpriteValidator();

    IEnumerable<string> files;
    try { files = scanner.Scan(assetsPath); }
    catch (DirectoryNotFoundException ex)
    {
        Console.Error.WriteLine(ex.Message);
        return 1;
    }

    var results   = validator.ValidateAll(files);
    bool hasError = false;

    foreach (var r in results)
    {
        Console.ForegroundColor = r.Severity switch
        {
            SDK.Tools.Validation.SeverityLevel.Error => ConsoleColor.Red,
            SDK.Tools.Validation.SeverityLevel.Warn  => ConsoleColor.Yellow,
            _                                        => ConsoleColor.Green,
        };
        Console.WriteLine($"[{r.Severity.ToString().ToUpper()}] {r.Entry.FileName} — {r.Message}");
        Console.ResetColor();

        if (r.Severity == SDK.Tools.Validation.SeverityLevel.Error) hasError = true;
    }

    Console.WriteLine($"\nTotal : {results.Count} fichiers analysés.");
    return hasError ? 1 : 0;
}

Console.Error.WriteLine($"Commande inconnue : {args[0]}");
Console.Error.WriteLine("Commandes disponibles : seed, asset-validate");
return 1;
