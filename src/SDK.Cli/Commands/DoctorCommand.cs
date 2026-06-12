using System.CommandLine;
using System.Diagnostics;
using System.Text.Json;
using SDK.Tools.Sync;

namespace PokeForge.Cli.Commands;

public static class DoctorCommand
{
    public static void Register(RootCommand root)
    {
        var cmd = new Command("doctor", "Check project health: import.json, sprites, database");
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] import.json introuvable : {configPath}");
            Console.ResetColor();
            return 1;
        }

        var config = JsonSerializer.Deserialize<ImportConfig>(File.ReadAllText(configPath))!;
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[OK]   import.json : {configPath}");
        Console.ResetColor();

        bool hasError = false;
        if (!Directory.Exists(config.SpritesRoot))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] sprites_root introuvable : {config.SpritesRoot}");
            Console.ResetColor();
            hasError = true;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK]   sprites_root : {config.SpritesRoot}");
            Console.ResetColor();
        }

        if (!File.Exists(config.DbPath))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[WARN] base de données absente : {config.DbPath}");
            Console.WriteLine("       → Exécutez : pokeforge seed");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK]   db_path : {config.DbPath}");
            Console.ResetColor();
        }

        if (!IsCommandAvailable("piper"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[WARN] piper TTS absent — installez piper depuis https://github.com/rhasspy/piper");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK]   piper TTS disponible");
            Console.ResetColor();
        }

        if (!OperatingSystem.IsWindows())
        {
            if (!IsCommandAvailable("aplay"))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("[WARN] aplay absent — TTS audio silencieux");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("[OK]   aplay disponible");
                Console.ResetColor();
            }
        }

        return hasError ? 1 : 0;
    }

    private static bool IsCommandAvailable(string command)
    {
        var checkCmd = OperatingSystem.IsWindows() ? "where" : "which";
        try
        {
            var psi = new ProcessStartInfo(checkCmd, command)
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
            };
            using var process = Process.Start(psi);
            process?.WaitForExit();
            return process?.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }
}
