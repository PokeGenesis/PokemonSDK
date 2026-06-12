using System.CommandLine;
using Microsoft.EntityFrameworkCore;
using SDK.Data;
using SDK.Tools.Fakemons;
using SDK.Tools.Fakemons.Models;

namespace PokeForge.Cli.Commands;

public static class FakemonCommand
{
    public static void Register(RootCommand root)
    {
        var cmd = new Command("fakemon", "Fakemon assembly pipeline — list parts and assemble sprites");
        RegisterListParts(cmd);
        RegisterAssemble(cmd);
        root.AddCommand(cmd);
    }

    private static void RegisterListParts(Command parent)
    {
        var cmd = new Command("list-parts", "List available PNG parts in a directory");
        var partsDirOpt = new Option<string>("--parts-dir", () => "assets/parts", "Directory containing PNG parts");
        var filterOpt = new Option<string?>("--filter", () => null, "Filter expression (e.g. type:fire,gen:1)");
        cmd.AddOption(partsDirOpt);
        cmd.AddOption(filterOpt);
        cmd.SetHandler((partsDir, filter) => Environment.Exit(ExecuteListParts(partsDir, filter)),
            partsDirOpt, filterOpt);
        parent.AddCommand(cmd);
    }

    private static void RegisterAssemble(Command parent)
    {
        var cmd = new Command("assemble", "Assemble PNG parts into a Fakemon sprite and register in DB");
        var nameOpt          = new Option<string>("--name",              "Unique identifier for the Fakemon") { IsRequired = true };
        var outputOpt        = new Option<string>("--output",            () => "assets/sprites/fakemons",     "Output directory");
        var partsDirOpt      = new Option<string>("--parts-dir",         () => "assets/parts",                "Directory containing PNG parts");
        var dbOpt            = new Option<string>("--db-path",           () => "src/SDK.Data/data/PokemonSDK.db", "Path to SQLite database");
        var filterOpt        = new Option<string?>("--filter",           () => null,                          "Filter expression");
        var translationsOpt  = new Option<string?>("--translations-json",() => null,                          "Path to translations JSON");
        var generationOpt    = new Option<int>("--generation",           () => 1,                             "Generation number");
        var type1Opt         = new Option<int>("--type1",                () => 1,                             "Primary type ID");
        var eggGroup1Opt     = new Option<string>("--egg-group1",        () => "field",                       "Primary egg group");
        var strictOpt        = new Option<bool>("--strict",              () => false,                         "Fail if no parts match the filter");
        cmd.AddOption(nameOpt); cmd.AddOption(outputOpt); cmd.AddOption(partsDirOpt);
        cmd.AddOption(dbOpt);   cmd.AddOption(filterOpt); cmd.AddOption(translationsOpt);
        cmd.AddOption(generationOpt); cmd.AddOption(type1Opt);
        cmd.AddOption(eggGroup1Opt);  cmd.AddOption(strictOpt);
        cmd.SetHandler(ctx =>
        {
            var opts = new FakemonAssemblyOptions(
                PartsDirectory:      ctx.ParseResult.GetValueForOption(partsDirOpt)!,
                OutputDirectory:     ctx.ParseResult.GetValueForOption(outputOpt)!,
                Identifier:          ctx.ParseResult.GetValueForOption(nameOpt)!,
                Generation:          ctx.ParseResult.GetValueForOption(generationOpt),
                Type1Id:             ctx.ParseResult.GetValueForOption(type1Opt),
                Type2Id:             null,
                EggGroup1:           ctx.ParseResult.GetValueForOption(eggGroup1Opt)!,
                EggGroup2:           null,
                IsLegendary:         false,
                FilterExpression:    ctx.ParseResult.GetValueForOption(filterOpt),
                TranslationsJsonPath:ctx.ParseResult.GetValueForOption(translationsOpt),
                Strict:              ctx.ParseResult.GetValueForOption(strictOpt));
            var dbPath = ctx.ParseResult.GetValueForOption(dbOpt)!;
            ctx.ExitCode = ExecuteAssemble(opts, dbPath).GetAwaiter().GetResult();
        });
        parent.AddCommand(cmd);
    }

    internal static int ExecuteListParts(string partsDir, string? filter)
    {
        if (!Directory.Exists(partsDir))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[INFO] Répertoire absent : {partsDir}");
            Console.ResetColor();
            return 0;
        }

        var catalogInstance = FakemonPartsCatalog.Scan(partsDir);
        var filtered = FakemonFilter.Apply(catalogInstance.Layers, catalogInstance, filter);

        if (filtered.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("[INFO] Aucune partie compatible");
            Console.ResetColor();
            return 0;
        }

        foreach (var layer in filtered)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] {layer.Path} (ZOrder={layer.ZOrder})");
            Console.ResetColor();
        }

        return 0;
    }

    internal static async Task<int> ExecuteAssemble(FakemonAssemblyOptions opts, string dbPath)
    {
        try
        {
            var dbOptions = new DbContextOptionsBuilder<PokemonDbContext>()
                .UseSqlite($"Data Source={dbPath}")
                .Options;
            using var ctx = new PokemonDbContext(dbOptions);
            var outputPath = await FakemonAssemblyPipeline.RunAsync(opts, ctx);
            if (string.IsNullOrEmpty(outputPath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[ERROR] 0 parties après filtre");
                Console.ResetColor();
                return 1;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] Fakemon '{opts.Identifier}' créé : {outputPath}");
            Console.ResetColor();
            return 0;
        }
        catch (FakemonAssemblyException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {ex.Message}");
            Console.ResetColor();
            return 1;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] Erreur inattendue : {ex.Message}");
            Console.ResetColor();
            return 1;
        }
    }
}
