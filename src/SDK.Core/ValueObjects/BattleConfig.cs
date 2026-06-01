namespace SDK.Core.ValueObjects;

// Référencé par IBattleEngine (Phase 2) — défini ici car SDK.Core only (D-05, D-06)
public sealed record BattleConfig(
    bool ItemsEnabled = true,
    bool FleeEnabled = true,
    bool WeatherEnabled = false,
    bool CritEnabled = true);
