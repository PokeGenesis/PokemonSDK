namespace SDK.Core.Enums;

// Hook for Phase 23 (Double Battles). Default = Single → zero impact on existing engine.
// DoubleBattleEngine will check Config.Mode == BattleMode.Double to activate 2v2 logic.
// BattleState and IBattleEngine stay 1v1 forever — parallel architecture (D-26).
public enum BattleMode
{
    Single = 0,
    Double = 1,
}
