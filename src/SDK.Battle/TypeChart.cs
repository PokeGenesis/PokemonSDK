namespace SDK.Battle;

using SDK.Core.Entities;
using SDK.Core.Interfaces;

public sealed class TypeChart : ITypeChart
{
    private readonly Dictionary<(int, int, int), decimal> _chart;

    public TypeChart(IEnumerable<TypeEffectiveness> entries)
    {
        _chart = entries.ToDictionary(
            e => (e.AttackerTypeId, e.DefenderTypeId, e.Generation),
            e => e.DamageFactor);
    }

    public decimal GetFactor(int attackerTypeId, int defenderTypeId, int generation)
    {
        return _chart.TryGetValue((attackerTypeId, defenderTypeId, generation), out var factor)
            ? factor
            : 1.0m;
    }
}
