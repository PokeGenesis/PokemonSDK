namespace SDK.Battle.Tests;

using SDK.Battle;
using SDK.Core.Entities;
using FluentAssertions;

public class TypeChartTests
{
    [Fact]
    public void Known_Entry_Returns_Correct_Factor()
    {
        var entries = new[]
        {
            new TypeEffectiveness { AttackerTypeId = 1, DefenderTypeId = 2, Generation = 1, DamageFactor = 2.0m }
        };
        var chart = new TypeChart(entries);

        chart.GetFactor(1, 2, 1).Should().Be(2.0m);
    }

    [Fact]
    public void Absent_Entry_Returns_1()
    {
        var chart = new TypeChart(Array.Empty<TypeEffectiveness>());

        chart.GetFactor(5, 7, 1).Should().Be(1.0m);
    }

    [Fact]
    public void Immunity_Entry_Returns_Zero()
    {
        var entries = new[]
        {
            new TypeEffectiveness { AttackerTypeId = 1, DefenderTypeId = 3, Generation = 1, DamageFactor = 0m }
        };
        var chart = new TypeChart(entries);

        chart.GetFactor(1, 3, 1).Should().Be(0m);
    }

    [Fact]
    public void Different_Generations_Are_Independent()
    {
        var entries = new[]
        {
            new TypeEffectiveness { AttackerTypeId = 1, DefenderTypeId = 2, Generation = 1, DamageFactor = 2.0m },
            new TypeEffectiveness { AttackerTypeId = 1, DefenderTypeId = 2, Generation = 4, DamageFactor = 0.5m }
        };
        var chart = new TypeChart(entries);

        chart.GetFactor(1, 2, 1).Should().Be(2.0m);
        chart.GetFactor(1, 2, 4).Should().Be(0.5m);
    }

    [Fact]
    public void Lookup_Is_Directional_Attacker_Defender_Not_Swapped()
    {
        var entries = new[]
        {
            new TypeEffectiveness { AttackerTypeId = 1, DefenderTypeId = 2, Generation = 1, DamageFactor = 2.0m }
        };
        var chart = new TypeChart(entries);

        chart.GetFactor(1, 2, 1).Should().Be(2.0m);
        chart.GetFactor(2, 1, 1).Should().Be(1.0m, "reversed direction is absent → default 1.0m");
    }
}
