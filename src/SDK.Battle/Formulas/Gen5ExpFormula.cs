using SDK.Core.Enums;
using SDK.Core.Interfaces;

namespace SDK.Battle.Formulas;

public sealed class Gen5ExpFormula : IExpFormula
{
    public int CalcExpGain(int baseExpYield, int opponentLevel, bool trainerBattle)
    {
        double multiplier = trainerBattle ? 1.5 : 1.0;
        return (int)(Math.Pow(baseExpYield * opponentLevel, 1.0 / 2.5) * multiplier / 5.0 + 2);
    }

    public int ExpThreshold(int level, GrowthRate growthRate) => growthRate switch
    {
        GrowthRate.MediumFast => (int)Math.Pow(level, 3),
        GrowthRate.MediumSlow => (int)(1.2 * Math.Pow(level, 3) - 15 * Math.Pow(level, 2) + 100 * level - 140),
        GrowthRate.Fast       => (int)(4 * Math.Pow(level, 3) / 5),
        GrowthRate.Slow       => (int)(5 * Math.Pow(level, 3) / 4),
        _                     => (int)Math.Pow(level, 3),
    };
}
