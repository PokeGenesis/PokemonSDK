using Microsoft.EntityFrameworkCore;
using SDK.Data;
using SDK.Data.Seeding;

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: SDK.Tools <command> [options]");
    Console.Error.WriteLine("  seed [db-path]              — applies migrations and seeds reference data");
    Console.Error.WriteLine("  asset-validate [path]       — validates PNG sprites (D-16, exit code 1 if ERROR)");
    Console.Error.WriteLine("  asset-sync [import.json]    — validate → pack → sync atlas pipeline");
    Console.Error.WriteLine("  default db-path: src/SDK.Data/data/PokemonSDK.db");
    Console.Error.WriteLine("  default assets-path: assets/sprites");
    Console.Error.WriteLine("  default import config: assets/import.json");
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

if (args[0] == "asset-sync")
{
    var configPath = args.Length > 1 ? args[1] : "assets/import.json";

    if (!File.Exists(configPath))
    {
        Console.Error.WriteLine($"import.json introuvable : {configPath}");
        return 1;
    }

    var config = System.Text.Json.JsonSerializer.Deserialize<SDK.Tools.Sync.ImportConfig>(
        File.ReadAllText(configPath))!;

    var scanner   = new SDK.Tools.Validation.SpriteScanner();
    var validator = new SDK.Tools.Validation.SpriteValidator();

    IEnumerable<string> files;
    try { files = scanner.Scan(config.SpritesRoot); }
    catch (DirectoryNotFoundException ex) { Console.Error.WriteLine(ex.Message); return 1; }

    if (config.ResizeToTarget)
    {
        var tempDir = Path.Combine(config.OutputDir, ".resize-tmp");
        var resizer = new SDK.Tools.Validation.SpriteResizer();
        var fileList = files.ToList();
        resizer.ResizeAll(fileList, tempDir);
        files = Directory.EnumerateFiles(tempDir, "*.png");
        Console.WriteLine($"[RESIZE] {fileList.Count} sprites → {tempDir}");
    }

    var results = validator.ValidateAll(files);
    bool hasError = false;

    var packable = results
        .Where(r => r.Severity != SDK.Tools.Validation.SeverityLevel.Error)
        .Select(r => r.Entry.FilePath)
        .ToList();

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

    if (packable.Count > 0)
    {
        var packer  = new SDK.Tools.Atlas.AtlasPacker();
        var entries = packer.Pack(packable, config.OutputDir);
        Console.WriteLine($"\n[OK] Atlas créé : {config.OutputDir}/atlas.png ({entries.Count} sprites)");

        var atlasPath = Path.Combine(config.OutputDir, "atlas.png");
        var syncer    = new SDK.Tools.Sync.SqliteSyncer(config.DbPath);
        int synced    = syncer.Sync(entries, atlasPath);
        Console.WriteLine($"[OK] SQLite sync : {synced} entrées dans sprite_atlas_entries");
    }
    else
    {
        Console.WriteLine("\n[WARN] Aucun sprite valide à packer.");
    }

    int errorCount = results.Count(r => r.Severity == SDK.Tools.Validation.SeverityLevel.Error);
    Console.WriteLine($"\nTotal : {results.Count} sprites | Packés : {packable.Count} | Erreurs : {errorCount}");
    return hasError ? 1 : 0;
}

Console.Error.WriteLine($"Commande inconnue : {args[0]}");
Console.Error.WriteLine("Commandes disponibles : seed, asset-validate, asset-sync");
return 1;
