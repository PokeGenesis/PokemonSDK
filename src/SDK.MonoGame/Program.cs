using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SDK.Battle;
using SDK.Battle.Difficulty;
using SDK.Battle.Formulas;
using SDK.Battle.Plugins;
using SDK.Core.Interfaces;
using SDK.Core.Services;
using SDK.Data;
using SDK.Data.Services;
using SDK.MonoGame;
using SDK.MonoGame.Input;
using SDK.MonoGame.Scenes;
using SDK.MonoGame.World;
using SDK.Scripting.Engine;
using Serilog;

var isHeadless = args.Contains("--headless");
int headlessFrames = 60;
var framesArg = Array.Find(args, a => a.StartsWith("--max-frames="));
if (framesArg is not null && int.TryParse(framesArg.Split('=')[1], out var n))
    headlessFrames = n;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/battle-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var services = new ServiceCollection();
services.AddDbContext<PokemonDbContext>(opt =>
    opt.UseSqlite("Data Source=src/SDK.Data/data/PokemonSDK.db"));
services.AddSingleton<IGameClock, GameTimeClock>();
services.AddSingleton<IWeatherSystem, WeatherSystem>();
services.AddScoped<IEncounterSystem, EncounterSystem>();
services.AddSingleton<WorldSystem>();
services.AddSingleton<PlayerSystem>();

services.AddSingleton<Func<IScriptEngine>>(_ => () => new LuaScriptEngine());
services.AddSingleton<ISaveSystem, SaveSystem>();

if (isHeadless)
    services.AddSingleton<IInputProvider, NullInputProvider>();
else
    services.AddSingleton<IInputProvider, KeyboardInputProvider>();

services.AddSingleton<ITypeChart, TypeChart>();
services.AddSingleton<IDamageFormula, StandardDamageFormula>();
services.AddSingleton<IDifficultyMode, StoryDifficultyMode>();
services.AddSingleton<IBattleEngine, BattleEngine>();
services.AddSingleton<IExpFormula, Gen1ExpFormula>();
services.AddSingleton<BattleScene>();

// Plugin system — D-13 : Nuzlocke/Randomizer/Turbo = plugins activables via PluginRegistry
var pluginRegistry = new PluginRegistry();
// Pour activer Nuzlocke : pluginRegistry.Register(new NuzlockePlugin((key, val) => { /* persister dans GameState */ }));
// Pour activer Turbo    : pluginRegistry.Register(new TurboPlugin());
services.AddSingleton(pluginRegistry);

var sp = services.BuildServiceProvider();

if (isHeadless)
    HeadlessRunner.Run(sp, headlessFrames);
else
{
    using var game = new Game1(sp);
    game.Run();
}
