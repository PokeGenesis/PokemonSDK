namespace SDK.Scripting.Tests;

using FluentAssertions;
using SDK.Core.ValueObjects;
using SDK.Scripting.Bindings;
using SDK.Scripting.Engine;

public class BadgeApiTests
{
    [Fact]
    public void AwardBadge_SetsFlagTrue()
    {
        var api = new BadgeApi(new GameState());
        api.AwardBadge("boulder");
        api.HasBadge("boulder").Should().BeTrue();
    }

    [Fact]
    public void AwardBadge_IsImmutable()
    {
        var original = new GameState();
        var api = new BadgeApi(original);
        api.AwardBadge("boulder");

        api.GetState().Should().NotBe(original);
        original.GetFlag<bool>("badge_boulder").Should().BeFalse();
    }

    [Fact]
    public void NpcInteractionRunner_AwardsBadge_ViaLua()
    {
        var engine = new LuaScriptEngine();
        var state = new GameState();

        var result = NpcInteractionRunner.Run(engine, state, "badges:AwardBadge('boulder')");

        result.GetFlag<bool>("badge_boulder").Should().BeTrue();
    }
}
