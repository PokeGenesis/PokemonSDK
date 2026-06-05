namespace SDK.Core.Interfaces;

public interface IScriptEngine
{
    void Execute(string luaCode);
    T? Evaluate<T>(string luaExpression);
    void RegisterApi(string name, object api);
    void LoadFile(string path);
}
