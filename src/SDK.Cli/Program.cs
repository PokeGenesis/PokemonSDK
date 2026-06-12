using PokeForge.Cli.Commands;
using System.CommandLine;

var rootCommand = new RootCommand("pokeforge — PokeForge SDK CLI tool");
NewCommand.Register(rootCommand);
AssetSyncCommand.Register(rootCommand);
SeedCommand.Register(rootCommand);
DoctorCommand.Register(rootCommand);
FakemonCommand.Register(rootCommand);
return await rootCommand.InvokeAsync(args);
