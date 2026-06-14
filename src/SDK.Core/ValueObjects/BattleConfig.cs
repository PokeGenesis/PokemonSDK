namespace SDK.Core.ValueObjects;

public sealed record BattleConfig(
    bool ItemsEnabled = true,
    bool FleeEnabled = true,
    bool WeatherEnabled = false,
    bool CritEnabled = true,
    int PlayerBadges = 0,
    int[]? LevelCapTable = null)
{
    // Preset BW2 — 8 gyms (index=badges, valeur=cap, 100=pas de cap)
    // 0b→13 · 1b→18 · 2b→24 · 3b→30 · 4b→33 · 5b→39 · 6b→48 · 7b→51 · 8b→58 · post→libre
    public static readonly int[] LevelCaps8Badges =
        [13, 18, 24, 30, 33, 39, 48, 51, 58, 100];

    // Preset 18 gyms (un badge par type) — progressif jusqu'au Lv.85, post→libre
    // 0b→15 · 1b→18 · ... · 17b→85 · 18b→libre
    public static readonly int[] LevelCaps18Badges =
        [15, 18, 21, 24, 27, 30, 33, 36, 40, 44, 48, 52, 56, 60, 65, 70, 78, 85, 100];

    // null = aucun cap (comportement par défaut, opt-in requis)
    public int? GetLevelCap()
    {
        if (LevelCapTable is null) return null;
        int idx = Math.Clamp(PlayerBadges, 0, LevelCapTable.Length - 1);
        int cap = LevelCapTable[idx];
        return cap >= 100 ? null : cap;
    }
}
