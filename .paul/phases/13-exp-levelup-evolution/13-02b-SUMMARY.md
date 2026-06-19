---
phase: 13-exp-levelup-evolution
plan: 02b
subsystem: btlui
tags: [monoGame, battlescene, movemenu, hpbar, pp, weather, battleengine, exp]

requires:
  - phase: 13-02
    provides: EXP engine, LevelUpOverlay, MoveLearnOverlay, ShowLevelUp/ShowMoveLearn phases

provides:
  - PP deduction in BattleEngine.ApplyMove (engine-side, persisted in BattleState)
  - HP restoration delta on level-up in BattleEngine.AwardExp
  - PP display in MoveMenu with color coding (white/yellow/red)
  - MoveMenu rebuild after each turn (stale PP fix)
  - Nickname labels in HpBar (replacing hardcoded "FOE"/"PLR")
  - Weather label display (SUN/RAIN/SAND/HAIL) top-center
  - Ghost-input fix: MoveMenu constructor accepts initial KeyboardState

affects: [13-02c, 13-03, Phase 14 (Struggle mechanic)]

tech-stack:
  added: []
  patterns:
    - "MoveMenu initialKs constructor parameter: seed _prevKeyState to prevent ghost-input on rebuild"
    - "BattleEngine: track currentHp local var through AwardExp loop, restore delta on MaxHp increase"

key-files:
  created:
    - src/SDK.Core/Enums/BattleMode.cs
    - .paul/phases/13-exp-levelup-evolution/13-02b-PLAN.md
  modified:
    - src/SDK.Battle/BattleEngine.cs
    - src/SDK.MonoGame/UI/MoveMenu.cs
    - src/SDK.MonoGame/Scenes/BattleScene.cs
    - tests/SDK.Battle.Tests/BattleEngineTests.cs
    - tests/SDK.Battle.Tests/LevelCapTests.cs
    - src/SDK.Core/ValueObjects/BattleConfig.cs

key-decisions:
  - "Ghost-input fix: MoveMenu(initialKs) avoids held-key false-positive on first Update()"
  - "D-26: DoubleBattleEngine = architecture parallele (BattleMode enum prep, additive zero-breaking)"

patterns-established:
  - "MoveMenu rebuild: pass Keyboard.GetState() as initialKs when creating after a phase transition"
  - "PP deduction: match by MoveId in attacker.Moves.ToList(), rebuild via with-expression"

duration: ~45min
started: 2026-06-17T17:45:00Z
completed: 2026-06-17T18:11:00Z
---

# Phase 13 Plan 02b: BattleScene Polish Summary

**PP deduction + HP restoration engine-side, PP display + nickname HpBar + weather UI, ghost-input fix — 293/293 tests green.**

## Performance

| Metric | Value |
|--------|-------|
| Duration | ~45 min |
| Started | 2026-06-17 19:45 CEST |
| Completed | 2026-06-17 20:11 CEST |
| Tasks | 3 completed + 1 human-verify |
| Files modified | 7 |

## Acceptance Criteria Results

| Criterion | Status | Notes |
|-----------|--------|-------|
| AC-1: PP decremente apres utilisation | Pass | CurrentPP 35→34 apres RunTurn |
| AC-2: PP clamp a 0 | Pass | CurrentPP 0→0, jamais negatif |
| AC-7: HP restaure au level-up (delta MaxHp) | Pass | 45/60 → 50/65 apres level-up |
| AC-3: PP affiche dans MoveMenu | Pass | "34/35" visible, couleur blanche/jaune/rouge |
| AC-4: Nickname dans HpBar | Pass | "BULBASAUR"/"PIDGEY" (plus "PLR"/"FOE") |
| AC-5: Meteo affichee si active | Pass | "RAIN"/"SUN"/etc visible en haut ecran |
| AC-6: Pas d'affichage meteo si None | Pass | Aucun label si WeatherType.None |

## Accomplishments

- PP deduction persistee dans BattleState immuable via `with`-expression dans `ApplyMove()`
- HP restoration au level-up: `currentHp` local var tracked dans la boucle `while`, `Math.Min(currentHp + delta, maxHp)` safe pour multi-level-up
- 3 nouveaux tests ajoutés dans `BattleEngineTests.cs` (AC-1, AC-2, AC-7)
- PP color-coded dans `MoveMenu.Draw()`: blanc normal, jaune si CurrentPP <= MaxPP/4, rouge si 0
- Ghost-input bug corrige: `MoveMenu(initialKs)` constructor — premier `Update()` ne voit plus la Space tenue de la phase log comme un nouveau press

## Files Created/Modified

| File | Change | Purpose |
|------|--------|---------|
| `src/SDK.Battle/BattleEngine.cs` | Modified | PP deduction dans ApplyMove, HP restore dans AwardExp |
| `src/SDK.MonoGame/UI/MoveMenu.cs` | Modified | PP display Draw(), initialKs constructor parameter |
| `src/SDK.MonoGame/Scenes/BattleScene.cs` | Modified | Nickname HpBar, weather display, MoveMenu rebuild + ghost-input fix |
| `tests/SDK.Battle.Tests/BattleEngineTests.cs` | Modified | 3 nouveaux tests AC-1/AC-2/AC-7 |
| `tests/SDK.Battle.Tests/LevelCapTests.cs` | Modified | Fix assertion string "EXP blocked" (pre-existing) |
| `src/SDK.Core/ValueObjects/BattleConfig.cs` | Modified | BattleMode.Single champ additive (D-26 prep Phase 23) |
| `src/SDK.Core/Enums/BattleMode.cs` | Created | BattleMode enum (Single=0, Double=1) pour Phase 23 D-26 |

## Decisions Made

| Decision | Rationale | Impact |
|----------|-----------|--------|
| Ghost-input fix via initialKs constructor | New MoveMenu avec `_prevKeyState=default` voyait Space tenue comme nouveau press → auto-sélection move[0]. Pattern: passer `Keyboard.GetState()` au constructeur. | MoveMenu ne peut plus auto-fire sur rebuild |
| D-26: BattleMode enum additive | Phase 23 prep ajoutée sans breaking change — `BattleConfig.Mode = BattleMode.Single` default, tous appels existants non impactés | Architecture parallele Double Battle garantie |

## Deviations from Plan

### Summary

| Type | Count | Impact |
|------|-------|--------|
| Auto-fixed | 3 | Essentiels, zero scope creep |
| Scope additions | 1 | D-26 prep additive, zero test break |
| Deferred | 0 | — |

**Total impact:** Corrections critiques + prep architecture propre, aucun scope creep.

### Auto-fixed Issues

**1. Ghost-input auto-fire (non planifié, découvert F3 verify)**
- **Found during:** checkpoint:human-verify
- **Issue:** `MoveMenu._prevKeyState = default` sur rebuild → Space tenue depuis phase log sélectionnait move[0] (Tackle) sans input joueur
- **Fix:** Constructeur `MoveMenu(moves, gd, KeyboardState initialKs = default)` + passage de `Keyboard.GetState()` dans `ExecuteTurn()` et `ApplyMoveLearnDecision()`
- **Files:** `src/SDK.MonoGame/UI/MoveMenu.cs`, `src/SDK.MonoGame/Scenes/BattleScene.cs`
- **Verification:** F3: Growl joué → tour 2 affiche bien le menu sans auto-sélection

**2. LevelCapTests assertion française (pre-existing)**
- **Found during:** Task 1 test run
- **Issue:** `AwardExp_Blocked_WhenPlayerAtCap` assertait `"EXP bloquée"` (FR) mais engine output `"EXP blocked"` (EN)
- **Fix:** Assertion mise a jour: `m.Contains("EXP blocked")`
- **Files:** `tests/SDK.Battle.Tests/LevelCapTests.cs`

**3. FluentAssertions `BeLessOrEqualTo` (API incorrecte)**
- **Found during:** Task 1 test run
- **Issue:** Méthode `BeLessOrEqualTo` n'existe pas en FA v8 — correcte: `BeLessThanOrEqualTo`
- **Fix:** Renamed in `LevelCapTests.cs`
- **Files:** `tests/SDK.Battle.Tests/LevelCapTests.cs`

### Scope Addition (non-breaking)

**D-26: BattleMode enum + BattleConfig.Mode**
- Ajouté en session pour poser l'architecture Phase 23 Double Battles
- `BattleMode.Single = 0` — default, zéro breaking change
- Documenté dans CLAUDE.md décisions D-26 et ROADMAP.md Phase 23

## Issues Encountered

| Issue | Resolution |
|-------|------------|
| FluentAssertions `BeLessOrEqualTo` absent v8 | Renommé en `BeLessThanOrEqualTo` |
| LevelCapTests string française vs engine anglais | Assertion corrigée `"EXP blocked"` |
| Ghost-input: Space tenue auto-sélectionne move[0] | Fix constructeur MoveMenu initialKs |

## Next Phase Readiness

**Ready:**
- PP affiché et toujours à jour dans MoveMenu après chaque tour
- Nicknames corrects dans les deux HpBar
- Weather label fonctionnel pour tous les états WeatherType
- BattleEngine PP/HP engine-side propre et testé (293/293)

**Concerns:**
- HpBar smooth animation (P12-G1) toujours absente — défère 13-02c
- FIGHT/RUN menu (P12-G5) toujours absent — défère 13-02c
- Multi level-up overlays séquentiels — défère 13-02c

**Blockers:**
- Aucun

---
*Phase: 13-exp-levelup-evolution, Plan: 02b*
*Completed: 2026-06-17*
