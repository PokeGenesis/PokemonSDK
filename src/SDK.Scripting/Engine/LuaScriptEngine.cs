namespace SDK.Scripting.Engine;

using MoonSharp.Interpreter;
using SDK.Core.Interfaces;

public class LuaScriptEngine : IScriptEngine
{
    private readonly Script _script;

    public LuaScriptEngine()
    {
        // D-04 : Preset_SoftSandbox — jamais Preset_Default (non sandboxé) ni HardSandbox (trop restrictif)
        _script = new Script(CoreModules.Preset_SoftSandbox);
    }

    public void Execute(string luaCode)
        => _script.DoString(luaCode);

    public T? Evaluate<T>(string luaExpression)
    {
        var result = _script.DoString(luaExpression);
        return result.ToObject<T>();
    }

    public void RegisterApi(string name, object api)
    {
        UserData.RegisterType(api.GetType());
        _script.Globals[name] = api;
    }

    public void LoadFile(string path)
        => _script.DoFile(path);
}
