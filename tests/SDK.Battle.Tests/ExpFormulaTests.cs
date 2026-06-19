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

    // Erratic — valeurs connues (Bulbapedia)
    [Theory]
    [InlineData(10,  1800)]    // n<=49: 10^3*(100-10)/50 = 1000*90/50
    [InlineData(50,  125000)]  // n<=67: 50^3*(150-50)/100 = 125000*100/100
    [InlineData(100, 600000)]  // n>=98: 100^3*(160-100)/100 = 1000000*60/100
    public void Gen1_Erratic_MatchesKnownValues(int level, int expected)
    {
        var formula = new Gen1ExpFormula();
        formula.ExpThreshold(level, GrowthRate.Erratic).Should().Be(expected);
    }

    // Fluctuating — valeurs connues (Bulbapedia)
    [Theory]
    [InlineData(10, 540)]     // n<=14: 10^3*((11/3)+24)/50 = 1000*27/50
    [InlineData(20, 5440)]    // n<=35: 20^3*(20+14)/50 = 8000*34/50
    [InlineData(50, 142500)]  // n>=36: 50^3*(25+32)/50 = 125000*57/50
    public void Gen1_Fluctuating_MatchesKnownValues(int level, int expected)
    {
        var formula = new Gen1ExpFormula();
        formula.ExpThreshold(level, GrowthRate.Fluctuating).Should().Be(expected);
    }

    // Erratic et Fluctuating strictement croissants
    [Theory]
    [InlineData(GrowthRate.Erratic)]
    [InlineData(GrowthRate.Fluctuating)]
    public void Gen1_ExpThreshold_IsStrictlyIncreasing_ForNewRates(GrowthRate rate)
    {
        var formula = new Gen1ExpFormula();
        for (int level = 2; level <= 99; level++)
            formula.ExpThreshold(level, rate)
                .Should().BeGreaterThan(
                    formula.ExpThreshold(level - 1, rate),
                    $"{rate} threshold at level {level} should be > level {level - 1}");
    }

    // MediumSlow — formule cubique non-standard (1.2n³ - 15n² + 100n - 140)
    [Theory]
    [InlineData(10, GrowthRate.MediumSlow, 560)]   // (int)(1200 - 1500 + 1000 - 140)
    [InlineData(20, GrowthRate.MediumSlow, 5460)]  // (int)(9600 - 6000 + 2000 - 140)
    public void Gen1_ExpThreshold_MediumSlow_MatchesFormula(int level, GrowthRate rate, int expected)
    {
        var formula = new Gen1ExpFormula();
        formula.ExpThreshold(level, rate).Should().Be(expected);
    }

    [Theory]
    [InlineData(10, GrowthRate.MediumSlow, 560)]
    [InlineData(20, GrowthRate.MediumSlow, 5460)]
    public void Gen5_ExpThreshold_MediumSlow_MatchesFormula(int level, GrowthRate rate, int expected)
    {
        var formula = new Gen5ExpFormula();
        formula.ExpThreshold(level, rate).Should().Be(expected);
    }

    // Gen5 CalcExpGain — vérifie la formule exacte (puissance 1/2.5, non-linéaire)
    [Theory]
    [InlineData(64, 25, false)]
    [InlineData(64, 50, true)]
    [InlineData(100, 50, false)]
    public void Gen5_CalcExpGain_MatchesExactFormula(int baseYield, int opponentLevel, bool trainerBattle)
    {
        double multiplier = trainerBattle ? 1.5 : 1.0;
        int expected = (int)(Math.Pow(baseYield * opponentLevel, 1.0 / 2.5) * multiplier / 5.0 + 2);
        var formula = new Gen5ExpFormula();
        formula.CalcExpGain(baseYield, opponentLevel, trainerBattle).Should().Be(expected);
    }
}
