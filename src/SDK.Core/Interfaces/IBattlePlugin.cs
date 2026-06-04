namespace SDK.Core.Interfaces;

using SDK.Core.ValueObjects;

public interface IBattlePlugin
{
    string Name { get; }
    void OnBattleStart(BattleConfig config);
    void OnTurnEnd(int turnNumber);
    void OnBattleEnd(BattleResult result);
}
