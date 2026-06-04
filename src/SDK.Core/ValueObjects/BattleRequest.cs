namespace SDK.Core.ValueObjects;

public sealed record BattleRequest(
    BattlePokemon Player,
    BattlePokemon Opponent,
    BattleConfig Config);
