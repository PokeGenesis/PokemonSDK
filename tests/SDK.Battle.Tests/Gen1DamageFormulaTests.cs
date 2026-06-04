namespace SDK.Battle.Tests;

using SDK.Battle.Formulas;
using SDK.Battle.Tests.Helpers;
using SDK.Core.Enums;
using FluentAssertions;

public class Gen1DamageFormulaTests
{
    private readonly Gen1DamageFormula _formula = new();

    [Fact]
    public void Generation_Is_1()
    {
        _formula.Generation.Should().Be(1);
    }

    [Fact]
    public void Status_Move_Returns_Zero_Damage()
    {
        var attacker = BattleTestHelpers.MakePokemon();
        var defender = BattleTestHelpers.MakePokemon();
        var move = BattleTestHelpers.MakeStatusMove();
        var config = BattleTestHelpers.NoCritConfig();

        var result = _formula.Calculate(attacker, defender, move, 1.0m, config);

        result.Damage.Should().Be(0);
        result.IsCritical.Should().BeFalse();
    }

    [Fact]
    public void Physical_Move_Returns_Positive_Damage()
    {
        var attacker = BattleTestHelpers.MakePokemon(atk: 80);
        var defender = BattleTestHelpers.MakePokemon(def: 50);
        var move = BattleTestHelpers.MakeMove(1, MoveCategory.Physical, power: 80);
        var config = BattleTestHelpers.NoCritConfig();

        var result = _formula.Calculate(attacker, defender, move, 1.0m, config);

        result.Damage.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Special_Move_Returns_Positive_Damage()
    {
        var attacker = BattleTestHelpers.MakePokemon(spAtk: 80);
        var defender = BattleTestHelpers.MakePokemon(spAtk: 50);
        var move = BattleTestHelpers.MakeMove(1, MoveCategory.Special, power: 80);
        var config = BattleTestHelpers.NoCritConfig();

        var result = _formula.Calculate(attacker, defender, move, 1.0m, config);

        result.Damage.Should().BeGreaterThan(0);
    }

    [Fact]
    public void CritEnabled_False_Never_Crits()
    {
        var attacker = BattleTestHelpers.MakePokemon(atk: 100);
        var defender = BattleTestHelpers.MakePokemon(def: 50);
        var move = BattleTestHelpers.MakeMove(1, MoveCategory.Physical, power: 40);
        var config = BattleTestHelpers.NoCritConfig();

        for (int i = 0; i < 200; i++)
        {
            var result = _formula.Calculate(attacker, defender, move, 1.0m, config);
            result.IsCritical.Should().BeFalse($"iteration {i}");
        }
    }

    [Fact]
    public void Gen1_Special_Uses_SpAtk_For_Defense_Not_SpDef()
    {
        // Gen1: d = defender.SpecialAttack for Special moves — no SpDef distinction
        // Attacker SpAtk=200, Defender SpAtk=10 (low d) vs another Defender SpAtk=200 (high d)
        // SpDef is irrelevant in Gen1 formula
        var attacker = BattleTestHelpers.MakePokemon(spAtk: 200);
        var defenderLowSpAtk = BattleTestHelpers.MakePokemon(spAtk: 10, spDef: 500);
        var defenderHighSpAtk = BattleTestHelpers.MakePokemon(spAtk: 200, spDef: 10);
        var move = BattleTestHelpers.MakeMove(1, MoveCategory.Special, power: 80);
        var config = BattleTestHelpers.NoCritConfig();

        int minDmgVsLowSpAtk = Enumerable.Range(0, 50)
            .Select(_ => _formula.Calculate(attacker, defenderLowSpAtk, move, 1.0m, config).Damage)
            .Min();
        int maxDmgVsHighSpAtk = Enumerable.Range(0, 50)
            .Select(_ => _formula.Calculate(attacker, defenderHighSpAtk, move, 1.0m, config).Damage)
            .Max();

        minDmgVsLowSpAtk.Should().BeGreaterThan(maxDmgVsHighSpAtk,
            "Gen1 uses SpAtk as defense for Special — lower SpAtk means more damage taken");
    }

    [Fact]
    public void Higher_TypeMultiplier_Scales_Damage_Up()
    {
        var attacker = BattleTestHelpers.MakePokemon(atk: 80);
        var defender = BattleTestHelpers.MakePokemon(def: 50);
        var move = BattleTestHelpers.MakeMove(1, MoveCategory.Physical, power: 80);
        var config = BattleTestHelpers.NoCritConfig();

        int minWith2x = Enumerable.Range(0, 50)
            .Select(_ => _formula.Calculate(attacker, defender, move, 2.0m, config).Damage)
            .Min();
        int maxWith1x = Enumerable.Range(0, 50)
            .Select(_ => _formula.Calculate(attacker, defender, move, 1.0m, config).Damage)
            .Max();

        minWith2x.Should().BeGreaterThan(maxWith1x);
    }
}
