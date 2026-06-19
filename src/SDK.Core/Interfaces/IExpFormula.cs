using SDK.Core.Enums;

namespace SDK.Core.Interfaces;

public interface IExpFormula
{
    int CalcExpGain(int baseExpYield, int opponentLevel, bool trainerBattle);
    int ExpThreshold(int level, GrowthRate growthRate);
}
