namespace SDK.Battle.Tests.Helpers;

using SDK.Core.Enums;
using SDK.Core.ValueObjects;

internal static class BattleTestHelpers
{
    internal static BattleConfig NoCritConfig() =>
        new BattleConfig(CritEnabled: false);

    internal static BattleMove MakeMove(
        int typeId, MoveCategory category, int power = 40, int accuracy = 100, int pp = 10) =>
        new BattleMove(1, "test-move", typeId, category, power, accuracy, pp, pp);

    internal static BattleMove MakeStatusMove(int typeId = 1) =>
        new BattleMove(1, "status-move", typeId, MoveCategory.Status, null, 100, 10, 10);

    internal static BattlePokemon MakePokemon(
        int type1Id = 1,
        int? type2Id = null,
        int hp = 100,
        int atk = 50,
        int def = 50,
        int spAtk = 50,
        int spDef = 50,
        int speed = 50,
        IReadOnlyList<BattleMove>? moves = null) =>
        new BattlePokemon(
            1, "TestMon", 50,
            hp, hp,
            atk, def, spAtk, spDef, speed,
            type1Id, type2Id,
            moves ?? new[] { MakeMove(type1Id, MoveCategory.Physical) });
}
