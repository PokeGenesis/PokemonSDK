namespace SDK.Battle.Tests;

using Moq;
using SDK.Battle;
using SDK.Battle.Difficulty;
using SDK.Battle.Formulas;
using SDK.Battle.Tests.Helpers;
using SDK.Core.Enums;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using FluentAssertions;

public class BattleEngineTests
{
    private static BattleEngine MakeEngine(
        Mock<IDamageFormula> formula,
        Mock<IDifficultyMode> playerStrat,
        Mock<IDifficultyMode> opponentStrat,
        Mock<ITypeChart> typeChart) =>
        new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, typeChart.Object);

    private static (Mock<IDamageFormula> formula, Mock<IDifficultyMode> player, Mock<IDifficultyMode> opponent, Mock<ITypeChart> chart)
        MakeMocks(int damage = 9999, decimal typeFactor = 1.0m)
    {
        var move = BattleTestHelpers.MakeMove(typeId: 1, MoveCategory.Physical, accuracy: 100);

        var formula = new Mock<IDamageFormula>();
        formula.Setup(f => f.Generation).Returns(1);
        formula.Setup(f => f.Calculate(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()))
            .Returns(new DamageResult(damage, false, typeFactor));

        var chart = new Mock<ITypeChart>();
        chart.Setup(t => t.GetFactor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(typeFactor);

        var player = new Mock<IDifficultyMode>();
        player.Setup(s => s.SelectMove(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);

        var opponent = new Mock<IDifficultyMode>();
        opponent.Setup(s => s.SelectMove(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);

        return (formula, player, opponent, chart);
    }

    [Fact]
    public void Player_Wins_When_Opponent_HP_Reaches_Zero()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);
        var engine = MakeEngine(formula, playerStrat, opponentStrat, chart);

        var player = BattleTestHelpers.MakePokemon(hp: 100, speed: 200);
        var opponent = BattleTestHelpers.MakePokemon(hp: 1, speed: 50);
        var request = new BattleRequest(player, opponent, BattleTestHelpers.NoCritConfig());

        var result = engine.RunBattle(request);

        result.PlayerWon.Should().BeTrue();
        result.TurnsElapsed.Should().Be(1);
    }

    [Fact]
    public void Opponent_Wins_When_Player_HP_Reaches_Zero()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);
        var engine = MakeEngine(formula, playerStrat, opponentStrat, chart);

        var player = BattleTestHelpers.MakePokemon(hp: 1, speed: 50);
        var opponent = BattleTestHelpers.MakePokemon(hp: 100, speed: 200);
        var request = new BattleRequest(player, opponent, BattleTestHelpers.NoCritConfig());

        var result = engine.RunBattle(request);

        result.PlayerWon.Should().BeFalse();
        result.TurnsElapsed.Should().Be(1);
    }

    [Fact]
    public void Max_Turns_Returns_Timeout()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 0);
        var engine = MakeEngine(formula, playerStrat, opponentStrat, chart);

        var player = BattleTestHelpers.MakePokemon(hp: 1000, speed: 100);
        var opponent = BattleTestHelpers.MakePokemon(hp: 1000, speed: 50);
        var request = new BattleRequest(player, opponent, BattleTestHelpers.NoCritConfig());

        var result = engine.RunBattle(request);

        result.PlayerWon.Should().BeFalse();
        result.TurnsElapsed.Should().Be(200);
        result.EndReason.Should().Be("MaxTurns");
    }

    [Fact]
    public void Type_Immunity_Prevents_Formula_From_Being_Called()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(typeFactor: 0.0m);
        var engine = MakeEngine(formula, playerStrat, opponentStrat, chart);

        var player = BattleTestHelpers.MakePokemon(hp: 1000, speed: 100);
        var opponent = BattleTestHelpers.MakePokemon(hp: 1000, speed: 50);
        var request = new BattleRequest(player, opponent, BattleTestHelpers.NoCritConfig());

        engine.RunBattle(request);

        formula.Verify(
            f => f.Calculate(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()),
            Times.Never(),
            "formula must not be called when type immunity (factor=0) short-circuits damage");
    }

    [Fact]
    public void STAB_Applied_When_Move_Type_Matches_Attacker_Type()
    {
        // move.TypeId=5, player.Type1Id=5 → STAB ×1.5
        // typeChart returns 1.0m → typeMultiplier = 1.0 * 1.0 * 1.5 = 1.5m
        var formula = new Mock<IDamageFormula>();
        formula.Setup(f => f.Generation).Returns(1);

        decimal capturedMultiplier = 0m;
        formula.Setup(f => f.Calculate(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()))
            .Callback<BattlePokemon, BattlePokemon, BattleMove, decimal, BattleConfig>(
                (_, _, _, tm, _) => capturedMultiplier = tm)
            .Returns(new DamageResult(9999, false, 1.5m));

        var chart = new Mock<ITypeChart>();
        chart.Setup(t => t.GetFactor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(1.0m);

        var stabMove = BattleTestHelpers.MakeMove(typeId: 5, MoveCategory.Physical, accuracy: 100);

        var playerStrat = new Mock<IDifficultyMode>();
        playerStrat.Setup(s => s.SelectMove(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(stabMove);

        var opponentStrat = new Mock<IDifficultyMode>();
        opponentStrat.Setup(s => s.SelectMove(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(stabMove);

        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object);

        var player = BattleTestHelpers.MakePokemon(type1Id: 5, hp: 100, speed: 200);
        var opponent = BattleTestHelpers.MakePokemon(type1Id: 1, hp: 1, speed: 50);
        var request = new BattleRequest(player, opponent, BattleTestHelpers.NoCritConfig());

        engine.RunBattle(request);

        capturedMultiplier.Should().Be(1.5m,
            "STAB ×1.5 must be included in typeMultiplier when move.TypeId matches attacker.Type1Id");
    }

    [Fact]
    public void No_STAB_When_Move_Type_Differs_From_Attacker_Type()
    {
        // move.TypeId=2, player.Type1Id=5 → no STAB → typeMultiplier = 1.0m
        var formula = new Mock<IDamageFormula>();
        formula.Setup(f => f.Generation).Returns(1);

        decimal capturedMultiplier = 0m;
        formula.Setup(f => f.Calculate(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()))
            .Callback<BattlePokemon, BattlePokemon, BattleMove, decimal, BattleConfig>(
                (_, _, _, tm, _) => capturedMultiplier = tm)
            .Returns(new DamageResult(9999, false, 1.0m));

        var chart = new Mock<ITypeChart>();
        chart.Setup(t => t.GetFactor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()))
            .Returns(1.0m);

        var noStabMove = BattleTestHelpers.MakeMove(typeId: 2, MoveCategory.Physical, accuracy: 100);

        var playerStrat = new Mock<IDifficultyMode>();
        playerStrat.Setup(s => s.SelectMove(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(noStabMove);

        var opponentStrat = new Mock<IDifficultyMode>();
        opponentStrat.Setup(s => s.SelectMove(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(noStabMove);

        var engine = new BattleEngine(formula.Object, playerStrat.Object, opponentStrat.Object, chart.Object);

        var player = BattleTestHelpers.MakePokemon(type1Id: 5, hp: 100, speed: 200);
        var opponent = BattleTestHelpers.MakePokemon(type1Id: 1, hp: 1, speed: 50);
        var request = new BattleRequest(player, opponent, BattleTestHelpers.NoCritConfig());

        engine.RunBattle(request);

        capturedMultiplier.Should().Be(1.0m,
            "no STAB when move.TypeId differs from attacker.Type1Id");
    }

    // ------------------------------------------------------------------
    // RunTurn / SelectOpponentMove — Phase 12-01
    // ------------------------------------------------------------------

    private static BattleState MakeState(BattlePokemon player, BattlePokemon opponent, int turn = 0) =>
        new BattleState(player, opponent, turn, WeatherType.None,
            BattleTestHelpers.NoCritConfig(), Array.Empty<string>());

    [Fact]
    public void RunTurn_ReducesOpponentHp_WhenPlayerMoveHits()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 30);
        var engine = MakeEngine(formula, playerStrat, opponentStrat, chart);

        var player   = BattleTestHelpers.MakePokemon(hp: 100, speed: 200);
        var opponent = BattleTestHelpers.MakePokemon(hp: 100, speed: 50);
        var state    = MakeState(player, opponent);
        var move     = BattleTestHelpers.MakeMove(1, MoveCategory.Physical, accuracy: 100);

        var result = engine.RunTurn(state, move, move);

        result.Opponent.CurrentHp.Should().BeLessThan(100,
            "player moves first (faster) and deals 30 damage");
    }

    [Fact]
    public void RunTurn_IncrementsTurnCounter()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 0);
        var engine = MakeEngine(formula, playerStrat, opponentStrat, chart);

        var player   = BattleTestHelpers.MakePokemon(hp: 100, speed: 100);
        var opponent = BattleTestHelpers.MakePokemon(hp: 100, speed: 50);
        var move     = BattleTestHelpers.MakeMove(1, MoveCategory.Physical);
        var state    = MakeState(player, opponent, turn: 3);

        var result = engine.RunTurn(state, move, move);

        result.Turn.Should().Be(4, "RunTurn must increment Turn by 1");
    }

    [Fact]
    public void RunTurn_ReturnsNewState_OriginalTurnUnchanged()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 0);
        var engine = MakeEngine(formula, playerStrat, opponentStrat, chart);

        var player   = BattleTestHelpers.MakePokemon(hp: 100, speed: 100);
        var opponent = BattleTestHelpers.MakePokemon(hp: 100, speed: 50);
        var move     = BattleTestHelpers.MakeMove(1, MoveCategory.Physical);
        var state    = MakeState(player, opponent, turn: 0);

        var result = engine.RunTurn(state, move, move);

        state.Turn.Should().Be(0, "BattleState is immutable — original must not change");
        result.Turn.Should().Be(1);
    }

    [Fact]
    public void RunTurn_PlayerGoesFirst_WhenFaster_OpponentNeverAttacks()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);
        var engine = MakeEngine(formula, playerStrat, opponentStrat, chart);

        var player   = BattleTestHelpers.MakePokemon(hp: 100, speed: 200);
        var opponent = BattleTestHelpers.MakePokemon(hp: 1,   speed: 50);
        var move     = BattleTestHelpers.MakeMove(1, MoveCategory.Physical, accuracy: 100);
        var state    = MakeState(player, opponent);

        var result = engine.RunTurn(state, move, move);

        result.Opponent.CurrentHp.Should().Be(0, "player one-shots opponent going first");
        result.Player.CurrentHp.Should().Be(100, "opponent is dead before it can attack");
    }

    [Fact]
    public void RunTurn_OpponentGoesFirst_WhenFaster_PlayerNeverAttacks()
    {
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks(damage: 9999);
        var engine = MakeEngine(formula, playerStrat, opponentStrat, chart);

        var player   = BattleTestHelpers.MakePokemon(hp: 1,   speed: 50);
        var opponent = BattleTestHelpers.MakePokemon(hp: 100, speed: 200);
        var move     = BattleTestHelpers.MakeMove(1, MoveCategory.Physical, accuracy: 100);
        var state    = MakeState(player, opponent);

        var result = engine.RunTurn(state, move, move);

        result.Player.CurrentHp.Should().Be(0, "opponent one-shots player going first");
        result.Opponent.CurrentHp.Should().Be(100, "player is dead before it can attack");
    }

    [Fact]
    public void SelectOpponentMove_DelegatesToOpponentStrategy()
    {
        var move = BattleTestHelpers.MakeMove(1, MoveCategory.Physical);
        var (formula, playerStrat, opponentStrat, chart) = MakeMocks();
        opponentStrat.Setup(s => s.SelectMove(
                It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);
        var engine = MakeEngine(formula, playerStrat, opponentStrat, chart);

        var player   = BattleTestHelpers.MakePokemon(hp: 100, speed: 100);
        var opponent = BattleTestHelpers.MakePokemon(hp: 100, speed: 50);
        var state    = MakeState(player, opponent);

        var result = engine.SelectOpponentMove(state);

        result.Should().Be(move, "SelectOpponentMove must delegate to _opponentStrategy.SelectMove");
        opponentStrat.Verify(s => s.SelectMove(
            state.Opponent, state.Player, state.Config), Times.AtLeastOnce);
    }

}
