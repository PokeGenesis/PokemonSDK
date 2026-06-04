namespace SDK.Core.ValueObjects;

using SDK.Core.Enums;

public sealed record BattleMove(
    int MoveId,
    string Identifier,
    int TypeId,
    MoveCategory Category,
    int? Power,
    int Accuracy,
    int CurrentPP,
    int MaxPP);
