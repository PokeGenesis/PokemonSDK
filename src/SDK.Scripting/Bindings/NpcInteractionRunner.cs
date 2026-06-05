namespace SDK.Scripting.Bindings;

using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;

public static class NpcInteractionRunner
{
    public static GameState Run(IScriptEngine engine, GameState state, string luaScript)
    {
        var api = new BadgeApi(state);
        engine.RegisterApi("badges", api);
        engine.Execute(luaScript);
        return api.GetState();
    }
}
