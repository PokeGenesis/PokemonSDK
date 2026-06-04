namespace SDK.Core.ValueObjects;

public sealed record BattlePokemon(
    int SpeciesId,
    string Nickname,
    int Level,
    int CurrentHp,
    int MaxHp,
    int Attack,
    int Defense,
    int SpecialAttack,
    int SpecialDefense,
    int Speed,
    int Type1Id,
    int? Type2Id,
    IReadOnlyList<BattleMove> Moves);
