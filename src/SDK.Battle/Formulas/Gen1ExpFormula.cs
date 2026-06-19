using SDK.Core.Enums;
using SDK.Core.Interfaces;

namespace SDK.Battle.Formulas;

public sealed class Gen1ExpFormula : IExpFormula
{
    public int CalcExpGain(int baseExpYield, int opponentLevel, bool trainerBattle)
    {
        double multiplier = trainerBattle ? 1.5 : 1.0;
        return (int)(multiplier * baseExpYield * opponentLevel / 7.0);
    }

    public int ExpThreshold(int level, GrowthRate growthRate)
    {
        if (level < 1) return 0;
        return growthRate switch
        {
            GrowthRate.MediumFast  => (int)Math.Pow(level, 3),
            GrowthRate.MediumSlow  => (int)(1.2 * Math.Pow(level, 3) - 15 * Math.Pow(level, 2) + 100 * level - 140),
            GrowthRate.Fast        => (int)(4 * Math.Pow(level, 3) / 5),
            GrowthRate.Slow        => (int)(5 * Math.Pow(level, 3) / 4),
            GrowthRate.Erratic     => ErraticThreshold(level),
            GrowthRate.Fluctuating => FluctuatingThreshold(level),
            _                      => (int)Math.Pow(level, 3),
        };
    }

    private static int ErraticThreshold(int n)
    {
        long n3 = (long)n * n * n;
        return n switch
        {
            <= 49 => (int)(n3 * (100 - n) / 50),
            <= 67 => (int)(n3 * (150 - n) / 100),
            <= 97 => (int)(n3 * ((1911 - 10 * n) / 3) / 500),
            _     => (int)(n3 * (160 - n) / 100),
        };
    }

    private static int FluctuatingThreshold(int n)
    {
        long n3 = (long)n * n * n;
        return n switch
        {
            <= 14 => (int)(n3 * ((n + 1) / 3 + 24) / 50),
            <= 35 => (int)(n3 * (n + 14) / 50),
            _     => (int)(n3 * (n / 2 + 32) / 50),
        };
    }
}
