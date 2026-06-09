using System.CommandLine;
using System.Text.Json;
using SDK.Tools.Atlas;
using SDK.Tools.Sync;
using SDK.Tools.Validation;

namespace PokeForge.Cli.Commands;

public static class AssetSyncCommand
{
    public static void Register(RootCommand root)
    {
        var cmd = new Command("asset-sync", "Validate sprites, pack atlas, sync to SQLite");
        var configOpt = new Option<string>(
            "--config",
            () => "assets/import.json",
            "Path to import.json");
        cmd.AddOption(configOpt);
        cmd.SetHandler((config) => Environment.Exit(Execute(config)), configOpt);
        root.AddCommand(cmd);
    }

    internal static int Execute(string configPath)
    {
        if (!File.Exists(configPath))
        {
            Console.Error.WriteLine($"import.json introuvable : {configPath}");
            return 1;
        }

        var config = JsonSerializer.Deserialize<ImportConfig>(File.ReadAllText(configPath))!;

        var scanner = new SpriteScanner();
        IEnumerable<string> files;
        try
        {
            files = scanner.Scan(config.SpritesRoot);
        }
        catch (DirectoryNotFoundException ex)
        {
            Console.Error.WriteLine(ex.Message);
            return 1;
        }

        var validator = new SpriteValidator();
        var results = validator.ValidateAll(files);

        foreach (var r in results)
        {
            var color = r.Severity switch
            {
                SeverityLevel.Error => ConsoleColor.Red,
                SeverityLevel.Warn  => ConsoleColor.Yellow,
                _                   => ConsoleColor.Green,
            };
            Console.ForegroundColor = color;
            Console.WriteLine($"[{r.Severity.ToString().ToUpperInvariant()}] {r.Entry.FileName} — {r.Message}");
            Console.ResetColor();
        }

        bool hasError = results.Any(r => r.Severity == SeverityLevel.Error);
        var packable = results
            .Where(r => r.Severity != SeverityLevel.Error)
            .Select(r => r.Entry.FilePath)
            .ToList();

        if (packable.Count > 0)
        {
            var packer = new AtlasPacker();
            var entries = packer.Pack(packable, config.OutputDir);
            var atlasPath = Path.Combine(config.OutputDir, "atlas.png");
            int synced = new SqliteSyncer(config.DbPath).Sync(entries, atlasPath);
            Console.WriteLine($"Atlas packed: {entries.Count} sprites → {atlasPath}");
            Console.WriteLine($"SQLite synced: {synced} entries → {config.DbPath}");
        }
        else
        {
            Console.WriteLine("[WARN] Aucun sprite valide à packer.");
        }

        return hasError ? 1 : 0;
    }
}
