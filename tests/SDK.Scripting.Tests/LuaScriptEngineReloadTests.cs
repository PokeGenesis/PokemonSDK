using FluentAssertions;
using SDK.Scripting.Engine;
using Xunit;

namespace SDK.Scripting.Tests;

public class LuaScriptEngineReloadTests : IDisposable
{
    private readonly string _tempDir = Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString());

    public LuaScriptEngineReloadTests() => Directory.CreateDirectory(_tempDir);

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private string WriteLua(string name, string content)
    {
        if (Path.IsPathRooted(name)) throw new ArgumentException("name must be relative", nameof(name));
        var path = Path.Join(_tempDir, name);
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
        // x absent du nouveau contexte — vérification par exception (nil → throw en MoonSharp)
        bool xIsGone = false;
        try   { engine.Evaluate<double>("return x"); }
        catch { xIsGone = true; }
        xIsGone.Should().BeTrue("x ne doit plus exister dans le nouveau contexte SoftSandbox");
    }

    [Fact]
    public void Reload_NonExistentPath_Throws()
    {
        var engine = new LuaScriptEngine();
        var act = () => engine.Reload(Path.Join(_tempDir, "doesnt_exist.lua"));
        act.Should().Throw<Exception>();
    }
}
