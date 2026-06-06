namespace SDK.Plugins.Nuzlocke.Tests;

using Moq;
using SDK.Battle;
using SDK.Battle.Difficulty;
using SDK.Battle.Formulas;
using SDK.Battle.Plugins;
using SDK.Core.Enums;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using FluentAssertions;

public class NuzlockePluginTests
{
    private static BattlePokemon MakePokemon(int speciesId = 1, int hp = 100, int speed = 50) =>
        new BattlePokemon(speciesId, "TestMon", 50, hp, hp, 50, 50, 50, 50, speed,
            1, null, new[] { new BattleMove(1, "tackle", 1, MoveCategory.Physical, 40, 100, 35, 35) });

    private static BattleState MakeState() =>
        new BattleState(MakePokemon(), MakePokemon(), 1, WeatherType.None,
            new BattleConfig(CritEnabled: false), Array.Empty<string>());

    [Fact]
    public void OnPokemonFainted_CallsCallback_WithCorrectKey()
    {
        string? capturedKey = null;
        var plugin = new NuzlockePlugin((key, _) => capturedKey = key);
        var fainted = MakePokemon(speciesId: 25);

        plugin.OnPokemonFainted(MakeState(), fainted);

        capturedKey.Should().Be("nuzlocke_dead_25");
    }

    [Fact]
    public void OnPokemonFainted_CallsCallback_WithValueTrue()
    {
        bool? capturedValue = null;
        var plugin = new NuzlockePlugin((_, val) => capturedValue = val);

        plugin.OnPokemonFainted(MakeState(), MakePokemon(speciesId: 25));

        capturedValue.Should().BeTrue();
    }

    [Fact]
    public void NuzlockePlugin_WithBattleEngine_CallbackFiresOnKO()
    {
        var move = new BattleMove(1, "tackle", 1, MoveCategory.Physical, 40, 100, 35, 35);

        var formula = new Mock<IDamageFormula>();
        formula.Setup(f => f.Generation).Returns(1);
        formula.Setup(f => f.Calculate(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()))
            .Returns(new DamageResult(9999, false, 1.0m));

        var chart = new Mock<ITypeChart>();
        chart.Setup(t => t.GetFactor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1.0m);

        var playerStrat = new Mock<IDifficultyMode>();
        playerStrat.Setup(s => s.SelectMove(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
            It.IsAny<BattleConfig>())).Returns(move);

        var opponentStrat = new Mock<IDifficultyMode>();
        opponentStrat.Setup(s => s.SelectMove(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
            It.IsAny<BattleConfig>())).Returns(move);

        int callCount = 0;
        string? capturedKey = null;
        var nuzlocke = new NuzlockePlugin((key, _) => { callCount++; capturedKey = key; });

        var registry = new PluginRegistry();
        registry.Register(nuzlocke);

        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object,
            chart.Object, registry);

        var request = new BattleRequest(
            MakePokemon(speciesId: 1, hp: 100, speed: 200),
            MakePokemon(speciesId: 7, hp: 1, speed: 50),
            new BattleConfig(CritEnabled: false));

        engine.RunBattle(request);

        callCount.Should().Be(1);
        capturedKey.Should().Be("nuzlocke_dead_7");
    }
}
