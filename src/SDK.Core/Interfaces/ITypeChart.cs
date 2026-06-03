namespace SDK.Core.Interfaces;

public interface ITypeChart
{
    decimal GetFactor(int attackerTypeId, int defenderTypeId, int generation);
}
