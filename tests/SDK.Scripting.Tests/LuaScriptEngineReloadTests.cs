using FluentAssertions;
using SDK.Scripting.Engine;
using Xunit;

namespace SDK.Scripting.Tests;

public class LuaScriptEngineReloadTests : IDisposable
{
    private readonly string _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

    public LuaScriptEngineReloadTests() => Directory.CreateDirectory(_tempDir);

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private string WriteLua(string name, string content)
    {
        var path = Path.Combine(_tempDir, name);
        File.WriteAllText(path, content);
        return path;
    }

    [Fact]
    public void Reload_ClearsOldState_AndLoadsNewFile()
    {
        var engine = new LuaScriptEngine();
        engine.Execute("x = 42");
        engine.Evaluate<double>("return x").Should().Be(42);

        var newFile = WriteLua("new.lua", "y = 99");
        engine.Reload(newFile);

        // nouveau contexte chargé
        engine.Evaluate<double>("return y").Should().Be(99);
        // x absent du nouveau contexte — MoonSharp retourne nil (conversion throws ou retourne 0)
        bool xIsGone = false;
        try   { xIsGone = engine.Evaluate<double>("return x") != 42; }
        catch { xIsGone = true; }
        xIsGone.Should().BeTrue("x ne doit plus avoir la valeur de l'ancien contexte");
    }

    [Fact]
    public void Reload_NonExistentPath_Throws()
    {
        var engine = new LuaScriptEngine();
        var act = () => engine.Reload(Path.Combine(_tempDir, "doesnt_exist.lua"));
        act.Should().Throw<Exception>();
    }
}
