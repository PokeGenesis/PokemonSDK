namespace SDK.Core.ValueObjects;

public sealed record BattleResult(
    bool PlayerWon,
    int TurnsElapsed,
    string? EndReason = null);
