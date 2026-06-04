namespace SDK.Battle.Difficulty;

using SDK.Core.Enums;
using SDK.Core.ValueObjects;

public sealed class HardDifficultyMode : IDifficultyMode
{
    public DifficultyMode Mode => DifficultyMode.Hard;

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
