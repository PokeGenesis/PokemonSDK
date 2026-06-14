namespace SDK.Core.Interfaces;

using SDK.Core.ValueObjects;

public interface IBattleEngine
{
    BattleResult RunBattle(BattleRequest request);
    BattleState RunTurn(BattleState state, BattleMove playerMove, BattleMove opponentMove);
    BattleMove SelectOpponentMove(BattleState state);
}
