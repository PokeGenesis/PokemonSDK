using StarterGame;

var headless = args.Contains("--headless");
using var game = new Game1(headless);
game.Run();
