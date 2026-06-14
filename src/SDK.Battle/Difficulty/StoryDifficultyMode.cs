namespace SDK.Battle.Difficulty;

using SDK.Core.Enums;
using SDK.Core.ValueObjects;

public sealed class StoryDifficultyMode : IDifficultyMode
{
    public DifficultyMode Mode => DifficultyMode.Story;
    public float DefeatExpMultiplier => 0.25f;
    public float VictoryExpMultiplier => 1.0f;

    public BattleMove SelectMove(BattlePokemon self, BattlePokemon opponent, BattleConfig config)
    {
        var available = self.Moves.Where(m => m.CurrentPP > 0).ToList();
        if (available.Count == 0)
            return self.Moves[0];

        return available[Random.Shared.Next(available.Count)];
    }
}
