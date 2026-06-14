namespace SDK.Battle.Difficulty;

using SDK.Core.Enums;
using SDK.Core.ValueObjects;

public interface IDifficultyMode
{
    DifficultyMode Mode { get; }
    float DefeatExpMultiplier { get; }
    float VictoryExpMultiplier { get; }
    BattleMove SelectMove(BattlePokemon self, BattlePokemon opponent, BattleConfig config);
}
