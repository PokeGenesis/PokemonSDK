namespace SDK.Battle.Tests;

using SDK.Battle.Difficulty;
using SDK.Battle.Tests.Helpers;
using SDK.Core.Enums;
using SDK.Core.ValueObjects;
using FluentAssertions;

public class StoryDifficultyModeTests
{
    private readonly StoryDifficultyMode _mode = new();

    [Fact]
    public void Mode_Is_Story()
    {
        _mode.Mode.Should().Be(DifficultyMode.Story);
    }

    [Fact]
    public void DefeatExpMultiplier_Is0Point25()
    {
        _mode.DefeatExpMultiplier.Should().Be(0.25f);
    }


    [Fact]
    public void VictoryExpMultiplier_Is1()
    {
        _mode.VictoryExpMultiplier.Should().Be(1.0f);
    }

    [Fact]
    public void Returns_Move_With_PP_When_Available()
    {
        var moves = new BattleMove[]
        {
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical, power: 40, pp: 0),
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical, power: 60, pp: 5),
            BattleTestHelpers.MakeMove(1, MoveCategory.Special, power: 80, pp: 0)
        };
        var pokemon = BattleTestHelpers.MakePokemon(moves: moves);
        var config = BattleTestHelpers.NoCritConfig();

        for (int i = 0; i < 20; i++)
        {
            var selected = _mode.SelectMove(pokemon, pokemon, config);
            selected.CurrentPP.Should().BeGreaterThan(0, $"iteration {i}: must only pick moves with PP > 0");
        }
    }

    [Fact]
    public void All_PP_Zero_Returns_First_Move()
    {
        var moves = new BattleMove[]
        {
            BattleTestHelpers.MakeMove(1, MoveCategory.Physical, pp: 0),
            BattleTestHelpers.MakeMove(1, MoveCategory.Special, pp: 0)
        };
        var pokemon = BattleTestHelpers.MakePokemon(moves: moves);
        var config = BattleTestHelpers.NoCritConfig();

        var selected = _mode.SelectMove(pokemon, pokemon, config);

        selected.Should().Be(moves[0]);
    }
}
