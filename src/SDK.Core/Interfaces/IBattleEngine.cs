namespace SDK.Core.Interfaces;

using SDK.Core.ValueObjects;

public interface IBattleEngine
{
    BattleResult RunBattle(BattleRequest request);
}
