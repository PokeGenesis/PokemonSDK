namespace SDK.Battle.Difficulty;

using SDK.Core.Enums;
using SDK.Core.ValueObjects;

public sealed class HardDifficultyMode : IDifficultyMode
{
    private readonly float _defeatExpMultiplier;
    private readonly float _victoryExpMultiplier;

    public HardDifficultyMode(float defeatExpMultiplier = 0.5f, float victoryExpMultiplier = 1.0f)
    {
        _defeatExpMultiplier = Math.Clamp(defeatExpMultiplier, 0f, 1f);
        _victoryExpMultiplier = Math.Clamp(MathF.Round(victoryExpMultiplier * 2f) / 2f, 1.0f, 3.0f);
    }

    public DifficultyMode Mode => DifficultyMode.Hard;
    public float DefeatExpMultiplier => _defeatExpMultiplier;
    public float VictoryExpMultiplier => _victoryExpMultiplier;

    public BattleMove SelectMove(BattlePokemon self, BattlePokemon opponent, BattleConfig config)
    {
        var available = self.Moves.Where(m => m.CurrentPP > 0).ToList();
        if (available.Count == 0)
            return self.Moves[0];

        var damaging = available.Where(m => m.Power.HasValue).ToList();
        if (damaging.Count == 0)
            return available[0];

        return damaging.OrderByDescending(m => m.Power!.Value).First();
    }
}
