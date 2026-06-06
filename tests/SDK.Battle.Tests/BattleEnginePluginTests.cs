namespace SDK.Battle.Tests;

using Moq;
using SDK.Battle;
using SDK.Battle.Difficulty;
using SDK.Battle.Formulas;
using SDK.Battle.Plugins;
using SDK.Battle.Tests.Helpers;
using SDK.Core.Enums;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using FluentAssertions;

public class BattleEnginePluginTests
{
    private static (BattleEngine engine, PluginRegistry registry, Mock<IBattlePlugin> plugin)
        MakeEngineWithSpy(int damage = 9999, int playerHp = 100, int opponentHp = 1, int playerSpeed = 200)
    {
        var move = BattleTestHelpers.MakeMove(typeId: 1, MoveCategory.Physical, accuracy: 100);

        var formula = new Mock<IDamageFormula>();
        formula.Setup(f => f.Generation).Returns(1);
        formula.Setup(f => f.Calculate(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()))
            .Returns(new DamageResult(damage, false, 1.0m));

        var chart = new Mock<ITypeChart>();
        chart.Setup(t => t.GetFactor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(1.0m);

        var playerStrat = new Mock<IDifficultyMode>();
        playerStrat.Setup(s => s.SelectMove(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);

        var opponentStrat = new Mock<IDifficultyMode>();
        opponentStrat.Setup(s => s.SelectMove(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);

        var plugin = new Mock<IBattlePlugin>();
        plugin.Setup(p => p.Name).Returns("Spy");
        plugin.Setup(p => p.OnBeforeMove(It.IsAny<BattleState>(), It.IsAny<BattleAction>()))
            .Returns((BattleState?)null);
        plugin.Setup(p => p.OnBeforeDamage(It.IsAny<BattleState>(), It.IsAny<DamageResult>()))
            .Returns((BattleState?)null);

        var registry = new PluginRegistry();
        registry.Register(plugin.Object);

        var engine = new BattleEngine(
            formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object, registry);

        return (engine, registry, plugin);
    }

    private static BattleRequest MakeRequest(int playerHp = 100, int opponentHp = 1, int playerSpeed = 200) =>
        new BattleRequest(
            BattleTestHelpers.MakePokemon(hp: playerHp, speed: playerSpeed),
            BattleTestHelpers.MakePokemon(hp: opponentHp, speed: 50),
            BattleTestHelpers.NoCritConfig());

    [Fact]
    public void BattleEngine_WithNoPlugin_ProducesSameResult()
    {
        var move = BattleTestHelpers.MakeMove(typeId: 1, MoveCategory.Physical, accuracy: 100);
        var formula = new Mock<IDamageFormula>();
        formula.Setup(f => f.Generation).Returns(1);
        formula.Setup(f => f.Calculate(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()))
            .Returns(new DamageResult(9999, false, 1.0m));
        var chart = new Mock<ITypeChart>();
        chart.Setup(t => t.GetFactor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1.0m);
        var playerStrat = new Mock<IDifficultyMode>();
        playerStrat.Setup(s => s.SelectMove(
            It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>())).Returns(move);
        var opponentStrat = new Mock<IDifficultyMode>();
        opponentStrat.Setup(s => s.SelectMove(
            It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>())).Returns(move);

        var engineNoPlugin = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object);
        var engineEmptyRegistry = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object, new PluginRegistry());

        var request = MakeRequest();
        var r1 = engineNoPlugin.RunBattle(request);
        var r2 = engineEmptyRegistry.RunBattle(request);

        r1.PlayerWon.Should().Be(r2.PlayerWon);
        r1.TurnsElapsed.Should().Be(r2.TurnsElapsed);
    }

    [Fact]
    public void BattleEngine_WithSpyPlugin_CallsBattleStartOnce()
    {
        var (engine, _, plugin) = MakeEngineWithSpy();
        engine.RunBattle(MakeRequest());

        plugin.Verify(p => p.OnBattleStart(It.IsAny<BattleState>()), Times.Once);
    }

    [Fact]
    public void BattleEngine_WithSpyPlugin_CallsTurnStartAndEndPerTurn()
    {
        var (engine, _, plugin) = MakeEngineWithSpy();
        var result = engine.RunBattle(MakeRequest());

        plugin.Verify(p => p.OnTurnStart(It.IsAny<BattleState>()), Times.Exactly(result.TurnsElapsed));
        plugin.Verify(p => p.OnTurnEnd(It.IsAny<BattleState>()), Times.Exactly(result.TurnsElapsed));
    }

    [Fact]
    public void BattleEngine_WithSpyPlugin_CallsFaintedWhenBattleEnds()
    {
        var (engine, _, plugin) = MakeEngineWithSpy();
        engine.RunBattle(MakeRequest());

        plugin.Verify(p => p.OnPokemonFainted(It.IsAny<BattleState>(), It.IsAny<BattlePokemon>()), Times.Once);
    }
}
