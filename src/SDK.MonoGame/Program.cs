using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SDK.Core.Interfaces;
using SDK.Core.Services;
using SDK.Data;
using SDK.Data.Services;
using SDK.MonoGame;
using SDK.MonoGame.Input;
using SDK.MonoGame.World;

var isHeadless = args.Contains("--headless");
int headlessFrames = 60;
var framesArg = Array.Find(args, a => a.StartsWith("--max-frames="));
if (framesArg is not null && int.TryParse(framesArg.Split('=')[1], out var n))
    headlessFrames = n;

var services = new ServiceCollection();
services.AddDbContext<PokemonDbContext>(opt =>
    opt.UseSqlite("Data Source=src/SDK.Data/data/PokemonSDK.db"));
services.AddSingleton<IGameClock, GameTimeClock>();
services.AddSingleton<IWeatherSystem, WeatherSystem>();
services.AddScoped<IEncounterSystem, EncounterSystem>();
services.AddSingleton<WorldSystem>();
services.AddSingleton<PlayerSystem>();

if (isHeadless)
    services.AddSingleton<IInputProvider, NullInputProvider>();
else
    services.AddSingleton<IInputProvider, KeyboardInputProvider>();

var sp = services.BuildServiceProvider();

if (isHeadless)
    HeadlessRunner.Run(sp, headlessFrames);
else
{
    using var game = new Game1(sp);
    game.Run();
}
