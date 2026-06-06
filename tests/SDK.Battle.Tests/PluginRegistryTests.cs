namespace SDK.Battle.Tests;

using Moq;
using SDK.Battle.Plugins;
using SDK.Battle.Tests.Helpers;
using SDK.Core.Enums;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using FluentAssertions;

public class PluginRegistryTests
{
    private static BattleState MakeState(int turn = 1) =>
        new BattleState(
            BattleTestHelpers.MakePokemon(),
            BattleTestHelpers.MakePokemon(),
            turn,
            WeatherType.None,
            BattleTestHelpers.NoCritConfig(),
            Array.Empty<string>());

    private static Mock<IBattlePlugin> MakePlugin(string name = "TestPlugin")
    {
        var mock = new Mock<IBattlePlugin>();
        mock.Setup(p => p.Name).Returns(name);
        mock.Setup(p => p.OnBeforeMove(It.IsAny<BattleState>(), It.IsAny<BattleAction>()))
            .Returns((BattleState?)null);
        mock.Setup(p => p.OnBeforeDamage(It.IsAny<BattleState>(), It.IsAny<DamageResult>()))
            .Returns((BattleState?)null);
        return mock;
    }

    [Fact]
    public void Register_ThenIsRegistered_ReturnsTrue()
    {
        var registry = new PluginRegistry();
        registry.Register(MakePlugin("A").Object);
        registry.IsRegistered("A").Should().BeTrue();
    }

    [Fact]
    public void Register_DuplicateName_ThrowsInvalidOperation()
    {
        var registry = new PluginRegistry();
        registry.Register(MakePlugin("A").Object);
        var act = () => registry.Register(MakePlugin("A").Object);
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*'A'*already registered*");
    }

    [Fact]
    public void Unregister_RemovesPlugin_IsRegisteredReturnsFalse()
    {
        var registry = new PluginRegistry();
        registry.Register(MakePlugin("A").Object);
        registry.Unregister("A");
        registry.IsRegistered("A").Should().BeFalse();
    }

    [Fact]
    public void ApplyBeforeMove_NullReturn_PassesStateUnchanged()
    {
        var registry = new PluginRegistry();
        var plugin = MakePlugin("A");
        plugin.Setup(p => p.OnBeforeMove(It.IsAny<BattleState>(), It.IsAny<BattleAction>()))
            .Returns((BattleState?)null);
        registry.Register(plugin.Object);

        var initial = MakeState(turn: 5);
        var action = new BattleAction(1, true);
        var result = registry.ApplyBeforeMove(initial, action);

        result.Should().Be(initial);
    }

    [Fact]
    public void ApplyBeforeMove_NonNull_ChainsStateModification()
    {
        var registry = new PluginRegistry();
        var plugin = MakePlugin("A");
        plugin.Setup(p => p.OnBeforeMove(It.IsAny<BattleState>(), It.IsAny<BattleAction>()))
            .Returns<BattleState, BattleAction>((s, _) => s with { Turn = 99 });
        registry.Register(plugin.Object);

        var initial = MakeState(turn: 1);
        var result = registry.ApplyBeforeMove(initial, new BattleAction(1, true));

        result.Turn.Should().Be(99);
    }

    [Fact]
    public void ApplyBeforeMove_TwoPlugins_ChainsSequentially()
    {
        var registry = new PluginRegistry();

        var p1 = MakePlugin("P1");
        p1.Setup(p => p.OnBeforeMove(It.IsAny<BattleState>(), It.IsAny<BattleAction>()))
            .Returns<BattleState, BattleAction>((s, _) => s with { Turn = 10 });

        var p2 = MakePlugin("P2");
        p2.Setup(p => p.OnBeforeMove(It.IsAny<BattleState>(), It.IsAny<BattleAction>()))
            .Returns<BattleState, BattleAction>((s, _) => s with { Turn = s.Turn + 5 });

        registry.Register(p1.Object);
        registry.Register(p2.Object);

        var result = registry.ApplyBeforeMove(MakeState(turn: 1), new BattleAction(1, true));
        result.Turn.Should().Be(15); // p1 sets 10, p2 adds 5
    }

    [Fact]
    public void NotifyBattleStart_CallsAllPlugins()
    {
        var registry = new PluginRegistry();
        var p1 = MakePlugin("P1");
        var p2 = MakePlugin("P2");
        registry.Register(p1.Object);
        registry.Register(p2.Object);

        var state = MakeState();
        registry.NotifyBattleStart(state);

        p1.Verify(p => p.OnBattleStart(state), Times.Once);
        p2.Verify(p => p.OnBattleStart(state), Times.Once);
    }
}
