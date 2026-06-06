namespace SDK.Plugins.Nuzlocke.Tests;

using Moq;
using SDK.Battle;
using SDK.Battle.Difficulty;
using SDK.Battle.Formulas;
using SDK.Battle.Plugins;
using SDK.Core.Enums;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using SDK.Plugins.Randomizer;
using SDK.Plugins.Turbo;
using FluentAssertions;

public class AllPluginsSmokeTests
{
    private static BattlePokemon MakePokemon(int speciesId = 1, int hp = 100, int speed = 50) =>
        new BattlePokemon(speciesId, "TestMon", 50, hp, hp, 50, 50, 50, 50, speed,
            1, null, new[] { new BattleMove(1, "tackle", 1, MoveCategory.Physical, 40, 100, 35, 35) });

    private static BattleEngine MakeEngine(PluginRegistry registry)
    {
        var move = new BattleMove(1, "tackle", 1, MoveCategory.Physical, 40, 100, 35, 35);

        var formula = new Mock<IDamageFormula>();
        formula.Setup(f => f.Generation).Returns(1);
        formula.Setup(f => f.Calculate(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()))
            .Returns(new DamageResult(9999, false, 1.0m));

        var chart = new Mock<ITypeChart>();
        chart.Setup(t => t.GetFactor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1.0m);

        var strat = new Mock<IDifficultyMode>();
        strat.Setup(s => s.SelectMove(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
            It.IsAny<BattleConfig>())).Returns(move);

        return new BattleEngine(formula.Object, strat.Object, strat.Object, chart.Object, registry);
    }

    [Fact]
    public void AllThreePlugins_RegisterAndRunWithBattleEngine_NoException()
    {
        string? capturedKey = null;
        var nuzlocke = new NuzlockePlugin((key, _) => capturedKey = key);
        var randomizer = new RandomizerPlugin(seed: 1);
        var turbo = new TurboPlugin();

        var registry = new PluginRegistry();
        registry.Register(nuzlocke);
        registry.Register(randomizer);
        registry.Register(turbo);

        var engine = MakeEngine(registry);
        var request = new BattleRequest(
            MakePokemon(speciesId: 1, hp: 100, speed: 200),
            MakePokemon(speciesId: 7, hp: 1, speed: 50),
            new BattleConfig(CritEnabled: false));

        BattleResult result = default!;
        var act = () => result = engine.RunBattle(request);

        act.Should().NotThrow();
        result.TurnsElapsed.Should().BeGreaterThanOrEqualTo(1);
        capturedKey.Should().NotBeNull();
    }

    [Fact]
    public void TurboPlugin_IsActiveTrue_ByDefault()
    {
        var turbo = new TurboPlugin();

        turbo.IsActive.Should().BeTrue();
        turbo.Name.Should().Be("Turbo");
    }
}
