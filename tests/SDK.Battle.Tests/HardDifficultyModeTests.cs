namespace SDK.Battle.Tests;

using SDK.Battle.Difficulty;
using SDK.Battle.Tests.Helpers;
using SDK.Core.Enums;
using SDK.Core.ValueObjects;
using FluentAssertions;

public class HardDifficultyModeTests
{
    private readonly HardDifficultyMode _mode = new();

    [Fact]
    public void Mode_Is_Hard()
    {
        _mode.Mode.Should().Be(DifficultyMode.Hard);
    }

    [Fact]
    public void DefeatExpMultiplier_DefaultIs0Point5()
    {
        _mode.DefeatExpMultiplier.Should().Be(0.5f);
    }

    [Theory]
    [InlineData(0f, 0f)]
    [InlineData(0.3f, 0.3f)]
    [InlineData(1f, 1f)]
    [InlineData(-0.5f, 0f)]    // clamp min
    [InlineData(2f, 1f)]       // clamp max
    public void DefeatExpMultiplier_IsClampedAndConfigurable(float input, float expected)
    {
        var mode = new HardDifficultyMode(defeatExpMultiplier: input);
        mode.DefeatExpMultiplier.Should().Be(expected);
    }


    [Fact]
    public void VictoryExpMultiplier_DefaultIs1()
    {
        _mode.VictoryExpMultiplier.Should().Be(1.0f);
    }

    [Theory]
    [InlineData(1.0f, 1.0f)]
    [InlineData(1.5f, 1.5f)]
    [InlineData(2.0f, 2.0f)]
    [InlineData(2.5f, 2.5f)]
    [InlineData(3.0f, 3.0f)]
    [InlineData(0.5f, 1.0f)]   // clamp min → 1.0
    [InlineData(4.0f, 3.0f)]   // clamp max → 3.0
    [InlineData(1.3f, 1.5f)]   // snap au 0.5 le plus proche
    [InlineData(1.7f, 1.5f)]   // 1.7*2=3.4, Round=3, 3/2=1.5
    public void VictoryExpMultiplier_IsSnappedAndClamped(float input, float expected)
    {
        var mode = new HardDifficultyMode(victoryExpMultiplier: input);
        mode.VictoryExpMultiplier.Should().Be(expected);
    }

    [Fact]
    public void Returns_Highest_Power_Move()
    {
        var moves = new BattleMove[]
        {
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical, power: 40),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical, power: 80),
            BattleTestHelpers.MakeMove(1, MoveCategory.Special, power: 60)
        };
        var pokemon = BattleTestHelpers.MakePokemon(moves: moves);
        var config = BattleTestHelpers.NoCritConfig();

        var selected = _mode.SelectMove(pokemon, pokemon, config);

        selected.Power.Should().Be(80);
    }

    [Fact]
    public void Skips_Moves_With_Zero_PP()
    {
        var moves = new BattleMove[]
        {
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical, power: 80, pp: 0),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical, power: 40, pp: 5)
        };
        var pokemon = BattleTestHelpers.MakePokemon(moves: moves);
        var config = BattleTestHelpers.NoCritConfig();

        var selected = _mode.SelectMove(pokemon, pokemon, config);

        selected.Power.Should().Be(40);
        selected.CurrentPP.Should().BeGreaterThan(0);
    }

    [Fact]
    public void All_Status_Returns_First_Available_Move()
    {
        var moves = new BattleMove[]
        {
            BattleTestHelpers.MakeStatusMove(),
            BattleTestHelpers.MakeStatusMove()
        };
        var pokemon = BattleTestHelpers.MakePokemon(moves: moves);
        var config = BattleTestHelpers.NoCritConfig();

        var selected = _mode.SelectMove(pokemon, pokemon, config);

        selected.Should().Be(moves[0]);
    }

    [Fact]
    public void All_PP_Zero_Returns_First_Move()
    {
        var moves = new BattleMove[]
        {
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical, power: 40, pp: 0),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical, power: 80, pp: 0)
        };
        var pokemon = BattleTestHelpers.MakePokemon(moves: moves);
        var config = BattleTestHelpers.NoCritConfig();

        var selected = _mode.SelectMove(pokemon, pokemon, config);

        selected.Should().Be(moves[0]);
    }
}
