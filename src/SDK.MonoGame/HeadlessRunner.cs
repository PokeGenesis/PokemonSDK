namespace SDK.MonoGame;

using Microsoft.Extensions.DependencyInjection;
using SDK.MonoGame.World;
using SDK.Core.Interfaces;

public static class HeadlessRunner
{
    public static void Run(IServiceProvider sp, int frames = 60)
    {
        var clock  = sp.GetRequiredService<IGameClock>();
        var world  = sp.GetRequiredService<WorldSystem>();
        var player = sp.GetRequiredService<PlayerSystem>();
        var delta  = TimeSpan.FromMilliseconds(1000.0 / 60.0);

        for (int i = 0; i < frames; i++)
        {
            world.Update(delta);  // WorldSystem appelle clock.Update() en interne
            player.Update();
        }
        Console.WriteLine($"[HeadlessRunner] {frames} frames OK — no exceptions");
    }
}
