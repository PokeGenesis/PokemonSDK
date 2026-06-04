namespace SDK.Battle.Tests;

using SDK.Battle.Formulas;
using SDK.Battle.Tests.Helpers;
using SDK.Core.Enums;
using FluentAssertions;

public class StandardDamageFormulaTests
{
    private readonly StandardDamageFormula _formula = new();

    [Fact]
    public void Generation_Is_4()
    {
        _formula.Generation.Should().Be(4);
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
    public void Standard_Special_Uses_SpDef_Not_SpAtk_For_Defense()
    {
        // Standard: d = defender.SpecialDefense for Special moves (Gen4+ distinction)
        // Attacker SpAtk=200, Defender SpAtk=500 (irrelevant), SpDef=10 (low d)
        var attacker = BattleTestHelpers.MakePokemon(spAtk: 200);
        var defenderLowSpDef = BattleTestHelpers.MakePokemon(spAtk: 500, spDef: 10);
        var defenderHighSpDef = BattleTestHelpers.MakePokemon(spAtk: 10, spDef: 200);
        var move = BattleTestHelpers.MakeMove(1, MoveCategory.Special, power: 80);
        var config = BattleTestHelpers.NoCritConfig();

        int minDmgVsLowSpDef = Enumerable.Range(0, 50)
            .Select(_ => _formula.Calculate(attacker, defenderLowSpDef, move, 1.0m, config).Damage)
            .Min();
        int maxDmgVsHighSpDef = Enumerable.Range(0, 50)
            .Select(_ => _formula.Calculate(attacker, defenderHighSpDef, move, 1.0m, config).Damage)
            .Max();

        minDmgVsLowSpDef.Should().BeGreaterThan(maxDmgVsHighSpDef,
            "Standard uses SpDef as defense for Special — lower SpDef means more damage taken");
    }

    [Fact]
    public void Gen1_And_Standard_Differ_For_Special_When_SpAtk_NotEquals_SpDef()
    {
        // Defender SpAtk=10, SpDef=200
        // Gen1 uses SpAtk(10) as d → higher damage
        // Standard uses SpDef(200) as d → lower damage
        var gen1 = new Gen1DamageFormula();
        var attacker = BattleTestHelpers.MakePokemon(spAtk: 200);
        var defender = BattleTestHelpers.MakePokemon(spAtk: 10, spDef: 200);
        var move = BattleTestHelpers.MakeMove(1, MoveCategory.Special, power: 80);
        var config = BattleTestHelpers.NoCritConfig();

        int minGen1Dmg = Enumerable.Range(0, 50)
            .Select(_ => gen1.Calculate(attacker, defender, move, 1.0m, config).Damage)
            .Min();
        int maxStandardDmg = Enumerable.Range(0, 50)
            .Select(_ => _formula.Calculate(attacker, defender, move, 1.0m, config).Damage)
            .Max();

        minGen1Dmg.Should().BeGreaterThan(maxStandardDmg,
            "Gen1 (d=SpAtk=10) should deal more damage than Standard (d=SpDef=200) on same target");
    }
}
