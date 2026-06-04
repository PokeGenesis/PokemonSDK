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
