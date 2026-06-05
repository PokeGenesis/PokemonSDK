namespace SDK.Scripting.Tests;

using FluentAssertions;
using MoonSharp.Interpreter;
using SDK.Core.ValueObjects;
using SDK.Scripting.Engine;

public class LuaScriptEngineTests
{
    [Fact]
    public void Execute_ValidCode_NoException()
    {
        var engine = new LuaScriptEngine();
        var act = () => engine.Execute("return 1 + 1");
        act.Should().NotThrow();
    }

    [Fact]
    public void Evaluate_ReturnsInt_FromLuaExpression()
    {
        var engine = new LuaScriptEngine();
        var result = engine.Evaluate<int>("return 42");
        result.Should().Be(42);
    }

    [Fact]
    public void Execute_OsExit_ThrowsScriptRuntimeException()
    {
        // Preset_SoftSandbox retire le module os — os est nil, appel lève ScriptRuntimeException
        var engine = new LuaScriptEngine();
        var act = () => engine.Execute("os.exit(0)");
        act.Should().Throw<ScriptRuntimeException>();
    }

    [Fact]
    public void GameState_WithFlag_RoundTrip()
    {
        var state = new GameState().WithFlag("badge_boulder", true);
        state.GetFlag<bool>("badge_boulder").Should().BeTrue();
    }

    [Fact]
    public void GameState_WithFlag_IsImmutable()
    {
        var original = new GameState();
        var updated  = original.WithFlag("badge_boulder", true);

        updated.GetFlag<bool>("badge_boulder").Should().BeTrue();
        // original inchangé — aucun flag
        original.GetFlag<bool>("badge_boulder").Should().BeFalse();
    }
}
