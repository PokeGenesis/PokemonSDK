# TODOS

Technical debt and known gaps to address in future phases.

---

## Phase 13 deferred — address before v1.0

### ARCH-01 — Log-string level-up detection (Finding 2)

`BattleScene` detects level-up by scanning the battle log for the string `"leveled up"` (fragile string match). The correct fix is a typed event or return value from `BattleEngine.AwardExp`. Deferred to avoid breaking the BattleState immutability contract mid-ship.

**Impact:** brittle, breaks on i18n log messages.
**Fix:** add a `LevelUpEvents` list to the `AwardExpResult` return type.

### ARCH-02 — Multi-level-up shows only one LevelUpOverlay (Finding 2)

When a Pokémon gains 2+ levels from a single battle, only the final level's stats are shown. The event queue exists (`PendingLearnedMoves`) but `ShowLevelUp` phase only shows once.

**Impact:** missing UX for large EXP gains (e.g., level 1 → 10 via cheats or catch-up EXP).
**Fix:** drain a `Queue<LevelUpEvent>` in sequence in BattleScene phase 5.

### ARCH-03 — `trainerBattle` hardcoded in `Gen5ExpFormula.CalcExpGain` (Finding 6)

`BattleScene` passes `trainerBattle: false` unconditionally. Trainer flag should come from `BattleConfig.IsTrainerBattle` (field exists, not wired).

**Impact:** EXP gain is always wild-battle rate even in trainer fights.
**Fix:** wire `state.Config.IsTrainerBattle` through `AwardExp` call in `BattleScene`.

### ARCH-04 — `_leveledUp` bool desync (Finding 12)

`BattleScene._leveledUp` is a separate bool that can desync from the actual `PendingEvolution` / move-learn queue state. If the exp-gain animation crashes or is skipped, the post-battle sequence can be wrong.

**Impact:** evolution or move-learn can be silently skipped on exception paths.
**Fix:** remove `_leveledUp` and derive the flag from `state.PlayerState.PendingEvolution.HasValue` + move queue length.

---

## Coverage gaps

### COV-01 — Gen1/Gen5 Erratic branch n=68..97 (no test)

`ErraticThreshold` branch `<= 97` (the `(1911 - 10n)/3` branch) has zero unit tests. The parentheses fix in 0.2.0 was validated by formula analysis only.

**Add tests:** n = 68, 80, 97 with values cross-checked against Bulbapedia tables.

### COV-02 — Gen1/Gen5 Fluctuating branch n≤14 (1 test, edge cases missing)

Only n=10 is tested. Add n=1, 5, 14 to cover boundary and off-by-one.

### COV-03 — No-evolution path (PendingEvolution = null after AwardExp)

No test verifies that a Pokémon with `EvolvesAt = null` produces no `PendingEvolution` after level-up.

### COV-04 — MoveLearnOverlay cancel path (B-button)

No test for `ForgottenMoveIndex == -1` when user presses B in `MoveLearnOverlay.Update`.

---

## Pre-Phase 14 blockers

- `SDK.Bundle` must be packed separately: `dotnet pack src/SDK.Bundle/SDK.Bundle.csproj` (after `dotnet nuget push` of the 9 other packages).
- SQLite vulnerability `SQLitePCLRaw.lib.e_sqlite3 2.1.11` (GHSA-2m69-gcr7-jv3q) — upgrade at Phase 14 start.
