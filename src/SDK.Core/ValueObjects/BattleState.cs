namespace SDK.Core.ValueObjects;

using SDK.Core.Enums;

public sealed record BattleState(
    BattlePokemon Player,
    BattlePokemon Opponent,
    int Turn,
    WeatherType Weather,
    BattleConfig Config,
    IReadOnlyList<string> Log);
