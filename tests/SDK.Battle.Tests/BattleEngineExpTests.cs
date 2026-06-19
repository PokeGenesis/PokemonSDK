namespace SDK.Battle.Tests;

using FluentAssertions;
using Moq;
using SDK.Battle;
using SDK.Battle.Difficulty;
using SDK.Battle.Formulas;
using SDK.Battle.Plugins;
using SDK.Battle.Tests.Helpers;
using SDK.Core.Enums;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using Xunit;

public sealed class BattleEngineExpTests
{
    private static (Mock<IDamageFormula> formula, Mock<IDifficultyMode> player, Mock<IDifficultyMode> opponent, Mock<ITypeChart> chart)
        MakeMocks(int damage = 9999)
    {
        var move = BattleTestHelpers.MakeMove(typeId: 1, MoveCategory.Physical, accuracy: 100);

        var formula = new Mock<IDamageFormula>();
        formula.Setup(f => f.Generation).Returns(1);
        formula.Setup(f => f.Calculate(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()))
            .Returns(new DamageResult(damage, false, 1.0m));

        var playerStrat = new Mock<IDifficultyMode>();
        playerStrat.Setup(s => s.SelectMove(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);
        playerStrat.Setup(s => s.VictoryExpMultiplier).Returns(1.0f);

        var opponentStrat = new Mock<IDifficultyMode>();
        opponentStrat.Setup(s => s.SelectMove(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);

        var chart = new Mock<ITypeChart>();
        chart.Setup(c => c.GetFactor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(1.0m);

        return (formula, playerStrat, opponentStrat, chart);
    }

    private static BattleState MakeState(BattlePokemon player, BattlePokemon opponent) =>
        new BattleState(player, opponent, 0, WeatherType.None,
            BattleTestHelpers.NoCritConfig(), Array.Empty<string>());

    // Test 1: EXP attribuée après KO
    [Fact]
    public void RunTurn_AwardsExp_WhenOpponentKO()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);

        var expFormula = new Mock<IExpFormula>();
        expFormula.Setup(e => e.CalcExpGain(64, 25, false)).Returns(228);
        expFormula.Setup(e => e.ExpThreshold(It.IsAny<int>(), It.IsAny<GrowthRate>())).Returns(int.MaxValue);

        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object,
            expFormula: expFormula.Object);

        var player = new BattlePokemon(1, "Pika", 50, 100, 100, 50, 50, 50, 50, 100,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            CurrentExp: 0, BaseExpYield: 64, GrowthRate: GrowthRate.MediumFast);
        var opponent = new BattlePokemon(2, "Rattata", 25, 1, 100, 30, 30, 30, 30, 50,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            BaseExpYield: 64);

        var state = MakeState(player, opponent);
        var result = engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        result.Player.CurrentExp.Should().Be(228, "player should receive 228 EXP after KO");
        result.Log.Should().Contain(m => m.Contains("228 EXP"), "log should mention EXP gained");
    }

    // Test 2: Level-up déclenché quand EXP >= seuil MediumFast
    [Fact]
    public void RunTurn_TriggersLevelUp_WhenExpReachesThreshold()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);

        // Player Level=9, CurrentExp=900. Seuil level 10 MediumFast = 1000. Gain = 200 → 1100 >= 1000.
        var expFormula = new Gen1ExpFormula();
        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object,
            expFormula: expFormula);

        // Gen1: CalcExpGain(70, 20, false) = (int)(70*20/7.0) = 200
        var player = new BattlePokemon(1, "Pika", 9, 100, 100, 45, 45, 45, 45, 50,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            CurrentExp: 900, BaseExpYield: 64, GrowthRate: GrowthRate.MediumFast);
        var opponent = new BattlePokemon(2, "Rattata", 20, 1, 100, 30, 30, 30, 30, 30,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            BaseExpYield: 70);

        var state = MakeState(player, opponent);
        var result = engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        result.Player.Level.Should().Be(10, "player should level up from 9 to 10");
        result.Log.Should().Contain(m => m.Contains("level 10"), "log should mention level 10");
    }

    // Test 3: Pas d'EXP sans IExpFormula (backward-compat)
    [Fact]
    public void RunTurn_NoExp_WhenNoExpFormula()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);
        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object);

        var player = new BattlePokemon(1, "Pika", 50, 100, 100, 50, 50, 50, 50, 100,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) });
        var opponent = new BattlePokemon(2, "Rattata", 25, 1, 100, 30, 30, 30, 30, 50,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) });

        var state = MakeState(player, opponent);
        var result = engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        result.Player.CurrentExp.Should().Be(0, "no EXP should be awarded without expFormula");
        result.Player.Level.Should().Be(50, "level should not change without expFormula");
    }

    // Test 4: NotifyLevelUp appelé sur le plugin
    [Fact]
    public void RunTurn_NotifiesPlugin_OnLevelUp()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);

        var expFormula = new Gen1ExpFormula();
        var plugin = new Mock<IBattlePlugin>();
        plugin.Setup(p => p.Name).Returns("test-plugin");
        plugin.Setup(p => p.OnBattleStart(It.IsAny<BattleState>()));
        plugin.Setup(p => p.OnTurnStart(It.IsAny<BattleState>()));
        plugin.Setup(p => p.OnTurnEnd(It.IsAny<BattleState>()));
        plugin.Setup(p => p.OnBeforeMove(It.IsAny<BattleState>(), It.IsAny<BattleAction>())).Returns((BattleState?)null);
        plugin.Setup(p => p.OnBeforeDamage(It.IsAny<BattleState>(), It.IsAny<DamageResult>())).Returns((BattleState?)null);
        plugin.Setup(p => p.OnPokemonFainted(It.IsAny<BattleState>(), It.IsAny<BattlePokemon>()));
        plugin.Setup(p => p.OnPokemonCaught(It.IsAny<BattleState>(), It.IsAny<BattlePokemon>(), It.IsAny<string>()));
        plugin.Setup(p => p.OnBattleEnd(It.IsAny<BattleState>(), It.IsAny<BattleResult>()));
        plugin.Setup(p => p.OnPokemonLevelUp(It.IsAny<BattlePokemon>(), It.IsAny<int>(), It.IsAny<int>()));

        var registry = new PluginRegistry();
        registry.Register(plugin.Object);

        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object,
            registry, expFormula);

        // Player Level=9, CurrentExp=950, gain=200 → 1150 >= 1000 (seuil level 10 MediumFast)
        // Gen1: CalcExpGain(70, 20, false) = 200
        var player = new BattlePokemon(1, "Pika", 9, 100, 100, 45, 45, 45, 45, 50,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            CurrentExp: 950, BaseExpYield: 64, GrowthRate: GrowthRate.MediumFast);
        var opponent = new BattlePokemon(2, "Rattata", 20, 1, 100, 30, 30, 30, 30, 30,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            BaseExpYield: 70);

        var state = MakeState(player, opponent);
        engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        plugin.Verify(p => p.OnPokemonLevelUp(It.IsAny<BattlePokemon>(), 9, 10), Times.Once,
            "NotifyLevelUp should be called with oldLevel=9, newLevel=10");
    }

    // Test 5: Pas de level-up si EXP insuffisante
    [Fact]
    public void RunTurn_NoLevelUp_WhenExpInsufficient()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);

        var expFormula = new Mock<IExpFormula>();
        expFormula.Setup(e => e.CalcExpGain(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<bool>())).Returns(50);
        expFormula.Setup(e => e.ExpThreshold(It.IsAny<int>(), It.IsAny<GrowthRate>())).Returns(1000);

        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object,
            expFormula: expFormula.Object);

        var player = new BattlePokemon(1, "Pika", 9, 100, 100, 45, 45, 45, 45, 50,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            CurrentExp: 0, GrowthRate: GrowthRate.MediumFast);
        var opponent = new BattlePokemon(2, "Rattata", 25, 1, 100, 30, 30, 30, 30, 50,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) });

        var state = MakeState(player, opponent);
        var result = engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        result.Player.Level.Should().Be(9, "level should not increase with only 50 EXP (seuil 1000)");
    }

    // Test 6: Stats scalent après level-up
    [Fact]
    public void RunTurn_ScalesStats_OnLevelUp()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);

        var expFormula = new Gen1ExpFormula();
        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object,
            expFormula: expFormula);

        // Player Level=9, Attack=45. Level-up to 10.
        // scale = (10+5.0)/(9+5.0) = 15/14. Attack = (int)(45 * 15/14) = (int)(48.21) = 48
        // Gen1 CalcExpGain(70, 20, false) = 200. CurrentExp=900+200=1100 >= 1000 (seuil)
        var player = new BattlePokemon(1, "Pika", 9, 100, 100, 45, 45, 45, 45, 50,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            CurrentExp: 900, BaseExpYield: 64, GrowthRate: GrowthRate.MediumFast);
        var opponent = new BattlePokemon(2, "Rattata", 20, 1, 100, 30, 30, 30, 30, 30,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            BaseExpYield: 70);

        var state = MakeState(player, opponent);
        var result = engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        result.Player.Level.Should().Be(10);
        result.Player.Attack.Should().Be(48, "Attack scales from 45 at level 9 to 48 at level 10 (scale=15/14)");
    }

    // Test: PendingEvolution défini quand newLevel == EvolvesAtLevel
    [Fact]
    public void AwardExp_SetsPendingEvolution_WhenLevelMatchesEvolvesAtLevel()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);
        var expFormula = new Gen1ExpFormula();
        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object,
            expFormula: expFormula);

        // Level 5 → 6 avec Gen1: CalcExpGain(64, 25, false) = 228 >= ExpThreshold(6, MediumFast) = 216
        var player = new BattlePokemon(1, "Bulbasaur", 5, 45, 45, 49, 49, 45, 65, 100,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            CurrentExp: 0, BaseExpYield: 64, GrowthRate: GrowthRate.MediumFast,
            EvolvesAtLevel: 6, EvolvesToSpeciesId: 2, EvolvesToName: "Ivysaur");
        var opponent = new BattlePokemon(2, "Rattata", 25, 1, 100, 30, 30, 30, 30, 64,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            BaseExpYield: 64);

        var state = MakeState(player, opponent);
        var result = engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        result.Player.Level.Should().Be(6, "player should level up from 5 to 6");
        result.PendingEvolution.Should().NotBeNull("evolution should be pending at level 6");
        result.PendingEvolution!.OldName.Should().Be("Bulbasaur");
        result.PendingEvolution.NewName.Should().Be("Ivysaur");
        result.PendingEvolution.NewSpeciesId.Should().Be(2);
    }

    // Test: PendingLearnedMoves rempli quand slots pleins (>=4 moves)
    [Fact]
    public void AwardExp_SetsPendingLearnedMoves_WhenMoveSlotsAreFull()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);
        var expFormula = new Gen1ExpFormula();
        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object,
            expFormula: expFormula);

        var tackle    = BattleTestHelpers.MakeMove(1, MoveCategory.Physical);
        var scratch   = BattleTestHelpers.MakeMove(2, MoveCategory.Physical);
        var growl     = BattleTestHelpers.MakeMove(3, MoveCategory.Status);
        var vine      = BattleTestHelpers.MakeMove(4, MoveCategory.Physical);
        var newLearn  = BattleTestHelpers.MakeMove(5, MoveCategory.Special);

        // 4 moves déjà en slot → le move du learnset ira en PendingLearnedMoves
        var learnset = new List<(int Level, BattleMove Move)> { (10, newLearn) };
        var player = new BattlePokemon(1, "Pika", 9, 100, 100, 45, 45, 45, 45, 50,
            1, null, new[] { tackle, scratch, growl, vine },
            CurrentExp: 900, BaseExpYield: 64, GrowthRate: GrowthRate.MediumFast,
            FullLearnset: learnset);
        var opponent = new BattlePokemon(2, "Rattata", 20, 1, 100, 30, 30, 30, 30, 30,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            BaseExpYield: 70);

        var state = MakeState(player, opponent);
        var result = engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        result.Player.Level.Should().Be(10, "player should reach level 10");
        result.PendingLearnedMoves.Should().HaveCount(1, "one move should be pending (full slots)");
        result.PendingLearnedMoves[0].Identifier.Should().Be(newLearn.Identifier);
    }

    // Test: Multi-level-up en un seul tour (boucle while)
    [Fact]
    public void AwardExp_MultiLevelUp_InOneTurn()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);
        var expFormula = new Gen1ExpFormula();
        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object,
            expFormula: expFormula);

        // Gen1: CalcExpGain(84, 50, false) = (int)(84*50/7) = 600
        // MediumFast: seuil 6=216, 7=343, 8=512, 9=729
        // Depuis niveau 5 avec 0 EXP + 600 → passe niveaux 6, 7, 8 (600 < 729)
        var player = new BattlePokemon(1, "Pika", 5, 50, 50, 30, 30, 30, 30, 100,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            CurrentExp: 0, GrowthRate: GrowthRate.MediumFast);
        var opponent = new BattlePokemon(2, "Rattata", 50, 1, 100, 30, 30, 30, 30, 84,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            BaseExpYield: 84);

        var state = MakeState(player, opponent);
        var result = engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        result.Player.Level.Should().Be(8, "600 EXP from level 5 should advance through levels 6, 7, 8");
        result.Log.Should().Contain(m => m.Contains("level 6"), "log should mention level 6");
        result.Log.Should().Contain(m => m.Contains("level 7"), "log should mention level 7");
        result.Log.Should().Contain(m => m.Contains("level 8"), "log should mention level 8");
        result.Log.Should().NotContain(m => m.Contains("level 9"), "600 EXP < 729 (seuil 9)");
    }
}
