namespace SDK.Scripting.Bindings;

using MoonSharp.Interpreter;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;

public static class NpcInteractionRunner
{
    public static GameState Run(IScriptEngine engine, GameState state, string luaScript)
        => Run(engine, state, luaScript, null);

    public static GameState Run(IScriptEngine engine, GameState state, string luaScript, INarrationPlugin? tts)
    {
        var api = new BadgeApi(state);
        engine.RegisterApi("badges", api);
        if (tts is not null)
        {
            UserData.RegisterType<TtsApi>();
            engine.RegisterApi("sdk", new SdkGlobals(new TtsApi(tts)));
        }
        engine.Execute(luaScript);
        return api.GetState();
    }
}

public sealed class SdkGlobals
{
    public TtsApi tts { get; }
    public SdkGlobals(TtsApi t) => tts = t;
}
