namespace SDK.Scripting.Tests;

using FluentAssertions;
using SDK.Core.ValueObjects;
using SDK.Scripting.Bindings;
using SDK.Scripting.Engine;

public class ProdScriptTests
{
    private const string GymBrockScript = "badges:AwardBadge('boulder')";

    [Fact]
    public void GymBrockScript_AwardsBoulderBadge()
    {
        var engine = new LuaScriptEngine();
        var state = new GameState();

        var result = NpcInteractionRunner.Run(engine, state, GymBrockScript);

        result.GetFlag<bool>("badge_boulder").Should().BeTrue();
    }

    [Fact]
    public void GymBrockScript_IsIdempotent()
    {
        var engine = new LuaScriptEngine();
        var state = new GameState();

        var first  = NpcInteractionRunner.Run(engine, state, GymBrockScript);
        var second = NpcInteractionRunner.Run(engine, first, GymBrockScript);

        second.GetFlag<bool>("badge_boulder").Should().BeTrue();
    }
}
