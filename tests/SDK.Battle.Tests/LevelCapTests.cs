namespace SDK.Battle.Tests;

using FluentAssertions;
using Moq;
using SDK.Battle;
using SDK.Battle.Difficulty;
using SDK.Battle.Formulas;
using SDK.Battle.Tests.Helpers;
using SDK.Core.Enums;
using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;
using Xunit;

public sealed class LevelCapTests
{
    // ──────────────────────────────────────────────
    // BattleConfig.GetLevelCap()
    // ──────────────────────────────────────────────

    [Fact]
    public void GetLevelCap_ReturnsNull_WhenNoTableSet()
    {
        var config = new BattleConfig(PlayerBadges: 0);
        config.GetLevelCap().Should().BeNull("null table = aucun cap (opt-in)");
    }

    [Theory]
    [InlineData(0, 13)]
    [InlineData(1, 18)]
    [InlineData(3, 30)]
    [InlineData(7, 51)]
    [InlineData(8, 58)]
    public void GetLevelCap_BW2Table_ReturnsCorrectCap(int badges, int expectedCap)
    {
        var config = new BattleConfig(PlayerBadges: badges, LevelCapTable: BattleConfig.LevelCaps8Badges);
        config.GetLevelCap().Should().Be(expectedCap);
    }

    [Fact]
    public void GetLevelCap_BW2Table_ReturnsNull_WhenPostGame()
    {
        var config = new BattleConfig(PlayerBadges: 9, LevelCapTable: BattleConfig.LevelCaps8Badges);
        config.GetLevelCap().Should().BeNull("valeur 100 = sentinelle pas de cap");
    }

    [Fact]
    public void GetLevelCap_Clamps_WhenBadgesExceedTableLength()
    {
        var config = new BattleConfig(PlayerBadges: 999, LevelCapTable: BattleConfig.LevelCaps8Badges);
        config.GetLevelCap().Should().BeNull("clamp sur last entry 100 = pas de cap");
    }

    [Theory]
    [InlineData(0, 15)]
    [InlineData(9, 44)]
    [InlineData(17, 85)]
    public void GetLevelCap_18BadgeTable_ReturnsCorrectCap(int badges, int expectedCap)
    {
        var config = new BattleConfig(PlayerBadges: badges, LevelCapTable: BattleConfig.LevelCaps18Badges);
        config.GetLevelCap().Should().Be(expectedCap);
    }

    [Fact]
    public void GetLevelCap_18BadgeTable_ReturnsNull_WhenPostGame()
    {
        var config = new BattleConfig(PlayerBadges: 18, LevelCapTable: BattleConfig.LevelCaps18Badges);
        config.GetLevelCap().Should().BeNull();
    }

    [Fact]
    public void GetLevelCap_CustomTable_UsedOverPresets()
    {
        var custom = new[] { 10, 20, 100 };
        var config = new BattleConfig(PlayerBadges: 1, LevelCapTable: custom);
        config.GetLevelCap().Should().Be(20);
    }

    // ──────────────────────────────────────────────
    // BattleEngine — cap bloque l'EXP
    // ──────────────────────────────────────────────

    private static BattleEngine MakeEngine(int opponentDamage = 9999)
    {
        var move = BattleTestHelpers.MakeMove(1, MoveCategory.Physical, accuracy: 100);

        var dmg = new Mock<IDamageFormula>();
        dmg.Setup(f => f.Generation).Returns(1);
        dmg.Setup(f => f.Calculate(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()))
            .Returns(new DamageResult(opponentDamage, false, 1.0m));

        var strat = new Mock<IDifficultyMode>();
        strat.Setup(s => s.SelectMove(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);
        strat.Setup(s => s.DefeatExpMultiplier).Returns(0f);
        strat.Setup(s => s.VictoryExpMultiplier).Returns(1.0f);

        var chart = new Mock<ITypeChart>();
        chart.Setup(c => c.GetFactor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1.0m);

        return new BattleEngine(dmg.Object, strat.Object, strat.Object, chart.Object,
            expFormula: new Gen1ExpFormula());
    }

    private static BattleState MakeState(BattlePokemon player, BattlePokemon opponent, BattleConfig config) =>
        new BattleState(player, opponent, 0, WeatherType.None, config, Array.Empty<string>());

    [Fact]
    public void AwardExp_Blocked_WhenPlayerAtCap()
    {
        var engine = MakeEngine();
        var config = new BattleConfig(
            CritEnabled: false,
            PlayerBadges: 0,
            LevelCapTable: BattleConfig.LevelCaps8Badges);  // 0 badges = cap 13

        var player = new BattlePokemon(1, "Pika", 13, 100, 100, 45, 45, 45, 45, 50,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            CurrentExp: 0, BaseExpYield: 64, GrowthRate: GrowthRate.MediumFast);
        var opponent = new BattlePokemon(2, "Rattata", 10, 1, 100, 30, 30, 30, 30, 30,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            BaseExpYield: 64);

        var state = MakeState(player, opponent, config);
        var result = engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        result.Player.CurrentExp.Should().Be(0, "EXP bloquée : player Lv.13 >= cap 13 (0 badges)");
        result.Log.Should().Contain(m => m.Contains("EXP bloquée"), "log doit mentionner le blocage");
        result.Log.Should().Contain(m => m.Contains("13"), "log doit indiquer le cap");
    }

    [Fact]
    public void AwardExp_Granted_WhenPlayerBelowCap()
    {
        var engine = MakeEngine();
        var config = new BattleConfig(
            CritEnabled: false,
            PlayerBadges: 1,
            LevelCapTable: BattleConfig.LevelCaps8Badges);  // 1 badge = cap 18

        var player = new BattlePokemon(1, "Pika", 15, 100, 100, 45, 45, 45, 45, 50,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            CurrentExp: 0, BaseExpYield: 64, GrowthRate: GrowthRate.MediumFast);
        var opponent = new BattlePokemon(2, "Rattata", 10, 1, 100, 30, 30, 30, 30, 30,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            BaseExpYield: 64);

        var state = MakeState(player, opponent, config);
        var result = engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        result.Player.CurrentExp.Should().BeGreaterThan(0, "Lv.15 < cap 18 (1 badge) → EXP accordée");
        result.Log.Should().Contain(m => m.Contains("EXP"), "log doit mentionner l'EXP gagnée");
        result.Log.Should().NotContain(m => m.Contains("bloquée"));
    }

    [Fact]
    public void AwardExp_Granted_WhenNoTableSet()
    {
        var engine = MakeEngine();
        var config = new BattleConfig(CritEnabled: false, PlayerBadges: 0);  // pas de table = pas de cap

        var player = new BattlePokemon(1, "Pika", 99, 100, 100, 45, 45, 45, 45, 50,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            CurrentExp: 0, BaseExpYield: 64, GrowthRate: GrowthRate.MediumFast);
        var opponent = new BattlePokemon(2, "Rattata", 10, 1, 100, 30, 30, 30, 30, 30,
            1, null, new[] { BattleTestHelpers.MakeMove(1, MoveCategory.Physical) },
            BaseExpYield: 64);

        var state = MakeState(player, opponent, config);
        var result = engine.RunTurn(state,
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical));

        result.Player.CurrentExp.Should().BeGreaterThan(0, "aucune table = aucun cap → EXP toujours accordée");
    }

    // ──────────────────────────────────────────────
    // XP en défaite — DefeatExpMultiplier
    // ──────────────────────────────────────────────

    [Theory]
    [InlineData(0.25f)]
    [InlineData(0.5f)]
    public void AwardExp_Defeat_GrantsReducedExp(float multiplier)
    {
        var move = BattleTestHelpers.MakeMove(1, MoveCategory.Physical, accuracy: 100);
        var dmg = new Mock<IDamageFormula>();
        dmg.Setup(f => f.Generation).Returns(1);
        dmg.Setup(f => f.Calculate(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()))
            .Returns(new DamageResult(9999, false, 1.0m));

        var playerStrat = new Mock<IDifficultyMode>();
        playerStrat.Setup(s => s.SelectMove(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);
        playerStrat.Setup(s => s.DefeatExpMultiplier).Returns(multiplier);

        var opponentStrat = new Mock<IDifficultyMode>();
        opponentStrat.Setup(s => s.SelectMove(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);
        opponentStrat.Setup(s => s.DefeatExpMultiplier).Returns(0f);

        var chart = new Mock<ITypeChart>();
        chart.Setup(c => c.GetFactor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1.0m);

        var engine = new BattleEngine(dmg.Object, playerStrat.Object, opponentStrat.Object, chart.Object,
            expFormula: new Gen1ExpFormula());

        // Player mourra au premier coup (HP=1), opponent survivra (HP=9999)
        var player = new BattlePokemon(1, "Pika", 10, 1, 1, 45, 45, 45, 45, 1,
            1, null, new[] { move },
            CurrentExp: 0, BaseExpYield: 64, GrowthRate: GrowthRate.MediumFast);
        var opponent = new BattlePokemon(2, "Rattata", 10, 9999, 9999, 30, 30, 30, 30, 100,
            1, null, new[] { move }, BaseExpYield: 64);

        var config = new BattleConfig(CritEnabled: false);
        var state = new BattleState(player, opponent, 0, WeatherType.None, config, Array.Empty<string>());
        var result = engine.RunTurn(state, move, move);

        result.Player.CurrentHp.Should().BeLessThanOrEqualTo(0, "player doit être KO");
        result.Player.CurrentExp.Should().BeGreaterThan(0,
            $"défaite avec multiplier={multiplier} → EXP réduite accordée");
    }

    [Fact]
    public void AwardExp_Defeat_NoExp_WhenMultiplierZero()
    {
        var move = BattleTestHelpers.MakeMove(1, MoveCategory.Physical, accuracy: 100);
        var dmg = new Mock<IDamageFormula>();
        dmg.Setup(f => f.Generation).Returns(1);
        dmg.Setup(f => f.Calculate(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(),
                It.IsAny<BattleMove>(), It.IsAny<decimal>(), It.IsAny<BattleConfig>()))
            .Returns(new DamageResult(9999, false, 1.0m));

        var playerStrat = new Mock<IDifficultyMode>();
        playerStrat.Setup(s => s.SelectMove(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);
        playerStrat.Setup(s => s.DefeatExpMultiplier).Returns(0f);

        var opponentStrat = new Mock<IDifficultyMode>();
        opponentStrat.Setup(s => s.SelectMove(It.IsAny<BattlePokemon>(), It.IsAny<BattlePokemon>(), It.IsAny<BattleConfig>()))
            .Returns(move);
        opponentStrat.Setup(s => s.DefeatExpMultiplier).Returns(0f);

        var chart = new Mock<ITypeChart>();
        chart.Setup(c => c.GetFactor(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).Returns(1.0m);

        var engine = new BattleEngine(dmg.Object, playerStrat.Object, opponentStrat.Object, chart.Object,
            expFormula: new Gen1ExpFormula());

        var player = new BattlePokemon(1, "Pika", 10, 1, 1, 45, 45, 45, 45, 1,
            1, null, new[] { move },
            CurrentExp: 0, BaseExpYield: 64, GrowthRate: GrowthRate.MediumFast);
        var opponent = new BattlePokemon(2, "Rattata", 10, 9999, 9999, 30, 30, 30, 30, 100,
            1, null, new[] { move }, BaseExpYield: 64);

        var config = new BattleConfig(CritEnabled: false);
        var state = new BattleState(player, opponent, 0, WeatherType.None, config, Array.Empty<string>());
        var result = engine.RunTurn(state, move, move);

        result.Player.CurrentExp.Should().Be(0, "multiplier=0 → aucune EXP en défaite");
    }
}
