namespace SDK.Battle.Tests;

using FluentAssertions;
using SDK.Battle.Formulas;
using SDK.Core.Enums;
using Xunit;

public sealed class ExpFormulaTests
{
    // Gen1 — CalcExpGain
    [Theory]
    [InlineData(64, 25, false, 228)]  // (int)(1.0 * 64 * 25 / 7)
    [InlineData(64, 25, true,  342)]  // (int)(1.5 * 64 * 25 / 7)
    [InlineData(100, 50, false, 714)] // (int)(1.0 * 100 * 50 / 7)
    public void Gen1_CalcExpGain_MatchesFormula(int baseYield, int opponentLevel, bool trainerBattle, int expected)
    {
        var formula = new Gen1ExpFormula();
        formula.CalcExpGain(baseYield, opponentLevel, trainerBattle).Should().Be(expected);
    }

    // Gen1 — ExpThreshold par GrowthRate
    [Theory]
    [InlineData(10, GrowthRate.MediumFast, 1000)]  // 10^3
    [InlineData(10, GrowthRate.Fast,        800)]  // 4*10^3/5
    [InlineData(10, GrowthRate.Slow,        1250)] // 5*10^3/4
    public void Gen1_ExpThreshold_MatchesGrowthRate(int level, GrowthRate rate, int expected)
    {
        var formula = new Gen1ExpFormula();
        formula.ExpThreshold(level, rate).Should().Be(expected);
    }

    // Gen5 — CalcExpGain (non-linéaire)
    [Theory]
    [InlineData(64, 25, false)]
    [InlineData(64, 50, true)]
    public void Gen5_CalcExpGain_ReturnsPositive(int baseYield, int opponentLevel, bool trainerBattle)
    {
        var formula = new Gen5ExpFormula();
        formula.CalcExpGain(baseYield, opponentLevel, trainerBattle).Should().BeGreaterThan(0);
    }

    // Gen5 — ExpThreshold identique à Gen1 (mêmes courbes)
    [Theory]
    [InlineData(10, GrowthRate.MediumFast, 1000)]
    [InlineData(20, GrowthRate.Slow,        10000)] // (int)(5*20^3/4) = (int)(10000)
    public void Gen5_ExpThreshold_SameAsGen1(int level, GrowthRate rate, int expected)
    {
        var formula = new Gen5ExpFormula();
        formula.ExpThreshold(level, rate).Should().Be(expected);
    }

    // ExpThreshold strictement croissant avec le niveau pour MediumFast
    [Fact]
    public void Gen1_ExpThreshold_IsStrictlyIncreasing_ForMediumFast()
    {
        var formula = new Gen1ExpFormula();
        for (int level = 2; level <= 99; level++)
            formula.ExpThreshold(level, GrowthRate.MediumFast)
                .Should().BeGreaterThan(
                    formula.ExpThreshold(level - 1, GrowthRate.MediumFast),
                    $"threshold at level {level} should be > threshold at level {level - 1}");
    }
}
