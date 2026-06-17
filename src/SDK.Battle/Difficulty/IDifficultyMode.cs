namespace SDK.Battle.Difficulty;

using SDK.Core.Enums;
using SDK.Core.ValueObjects;

public interface IDifficultyMode
{
    DifficultyMode Mode { get; }
    float DefeatExpMultiplier => 1.0f;
    float VictoryExpMultiplier => 1.0f;
    BattleMove SelectMove(BattlePokemon self, BattlePokemon opponent, BattleConfig config);
}
