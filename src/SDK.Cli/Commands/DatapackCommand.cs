namespace PokeForge.Cli.Commands;

using System.CommandLine;
using SDK.Tools.Atlas;
using SDK.Tools.Sync;
using SDK.Tools.Validation;

public static class DatapackCommand
{
    public static void Register(RootCommand root)
    {
        var cmd = new Command("datapack", "Importer un DataPack Pokémon (sprites front + back)");
        var useOpt  = new Option<string>("--use",    "Chemin vers le répertoire DataPack") { IsRequired = true };
        var dbOpt   = new Option<string>("--db",     () => "src/SDK.Data/data/PokemonSDK.db", "Chemin vers la DB SQLite");
        var outOpt  = new Option<string>("--output", () => "Content", "Répertoire de sortie pour les atlases");
        cmd.AddOption(useOpt);
        cmd.AddOption(dbOpt);
        cmd.AddOption(outOpt);
        cmd.SetHandler(
            (use, db, output) => Environment.Exit(Execute(use, db, output)),
            useOpt, dbOpt, outOpt);
        root.AddCommand(cmd);
    }

    internal static int Execute(string datapackPath, string dbPath, string outputDir)
    {
        if (!Directory.Exists(datapackPath))
        {
            Console.Error.WriteLine($"[DATAPACK] Répertoire introuvable : {datapackPath}");
            return 1;
        }

        Console.WriteLine($"[DATAPACK] Source : {datapackPath}");
        int exitCode = 0;

        var frontRoot = Path.Combine(datapackPath, "sprites", "front");
        if (Directory.Exists(frontRoot))
        {
            Console.WriteLine("\n[DATAPACK] Import sprites front...");
            exitCode |= ImportView(frontRoot, Path.Combine(outputDir, "atlas"), dbPath);
        }
        else
            Console.WriteLine($"[WARN] Dossier front absent : {frontRoot}");

        var backRoot = Path.Combine(datapackPath, "sprites", "back");
        if (Directory.Exists(backRoot))
        {
            Console.WriteLine("\n[DATAPACK] Import sprites back...");
            exitCode |= ImportView(backRoot, Path.Combine(outputDir, "atlas-back"), dbPath);
        }
        else
            Console.WriteLine($"[WARN] Dossier back absent : {backRoot}");

        return exitCode;
    }

    private static int ImportView(string spritesRoot, string outputDir, string dbPath)
    {
        var scanner = new SpriteScanner();
        IEnumerable<string> files;
        try { files = scanner.Scan(spritesRoot); }
        catch (DirectoryNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        var tempDir  = Path.Combine(outputDir, ".resize-tmp");
        var fileList = files.ToList();
        new SpriteResizer().ResizeAll(fileList, tempDir);
        files = Directory.EnumerateFiles(tempDir, "*.png");

        var results  = new SpriteValidator().ValidateAll(files);
        int errors   = 0;
        foreach (var r in results.Where(r => r.Severity == SeverityLevel.Error))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {r.Entry.FileName} — {r.Message}");
            Console.ResetColor();
            errors++;
        }

        var packable = results
            .Where(r => r.Severity != SeverityLevel.Error)
            .Select(r => r.Entry.FilePath)
            .ToList();

        if (packable.Count > 0)
        {
            var entries    = new AtlasPacker().Pack(packable, outputDir);
            var atlasPath  = Path.Combine(outputDir, "atlas.png");
            int synced     = new SqliteSyncer(dbPath).Sync(entries, atlasPath);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] Atlas créé : {atlasPath} ({entries.Count} sprites)");
            Console.WriteLine($"[OK] SQLite sync : {synced} entrées dans sprite_atlas_entries");
            Console.ResetColor();
            Console.WriteLine($"Total : {fileList.Count} sprites | Packés : {packable.Count} | Erreurs : {errors}");
        }
        else
        {
            Console.WriteLine("[WARN] Aucun sprite valide à packer.");
        }

        return errors > 0 ? 1 : 0;
    }
}
