---
phase: 13-exp-levelup-evolution
plan: 02c
subsystem: btlui
tags: [monoGame, battlescene, hpbar, lerp, fight-run, flee, input]

requires:
  - phase: 13-02b
    provides: PP engine, ghost-input fix MoveMenu, Nickname HpBar, weather UI

provides:
  - HpBar smooth lerp animation (_playerDisplayHp / _opponentDisplayHp floats, 8f/sec)
  - SelectAction phase: FIGHT/RUN top-level menu avant SelectMove
  - Flee auto-exit 5s: "Got away safely!" + timer → retour monde automatique
  - Snap-to-zero fix: barre HP ne reste plus à 1 quand Pokémon KO
  - F6 debug scenario: PIDGEY 50/50 HP pour tester lerp visuellement

affects: [13-03, Phase 14 (Bag/Items)]

tech-stack:
  added: []
  patterns:
    - "HP display lerp: float _displayHp + MathHelper.Lerp + Math.Ceiling — stateless HpBar préservé"
    - "Flee via _fleeTimer float: ShowLog avec countdown auto-exit, jamais BattleEnd"
    - "Snap-to-zero: if (actualHp == 0 && displayHp < 0.5f) displayHp = 0f — évite Math.Ceiling bug"

key-files:
  created: []
  modified:
    - src/SDK.MonoGame/Scenes/BattleScene.cs
    - src/SDK.MonoGame/Scenes/WorldScene.cs

key-decisions:
  - "Fuite sans panneau noir: ShowLog + timer 5s → SwitchToScene direct, BattleEnd ignoré pour RUN"
  - "Math.Ceiling snap: quand HP réel = 0 et display < 0.5f, snap à 0 dans Update() pas Draw()"
  - "F6 scénario dédié lerp: adversaire 50/50 HP pour que l'animation soit visible"

patterns-established:
  - "Timer auto-exit: _fleeTimer float décrémenté dans ShowLog quand _playerRanAway — pattern réutilisable pour cinématiques"
  - "Lerp snap guard: tester le HP réel ET le display avant de snapper — préserve l'animation jusqu'au seuil"

duration: ~35min
started: 2026-06-17T18:30:00Z
completed: 2026-06-17T21:15:00Z
---

# Phase 13 Plan 02c: HpBar Lerp + FIGHT/RUN Menu Summary

**HP bars animées par lerp (8f/sec) + menu FIGHT/RUN avant les moves + fuite auto-exit 5s — P12-G1 et P12-G5 fermés.**

## Performance

| Metric | Value |
|--------|-------|
| Duration | ~35 min |
| Started | 2026-06-17 20:30 CEST |
| Completed | 2026-06-17 23:15 CEST |
| Tasks | 2 complètes + 3 auto-fixes post human-verify |
| Files modified | 2 |

## Acceptance Criteria Results

| Criterion | Status | Notes |
|-----------|--------|-------|
| AC-1: HP bars animent par lerp | Pass | `_playerDisplayHp`/`_opponentDisplayHp` lerp 8f/sec, Math.Ceiling cast |
| AC-2: Sélection FIGHT avant moves | Pass | SelectAction phase: "> FIGHT" / "  RUN", Up/Down navigue, Space confirme |
| AC-3: RUN quitte le combat | Pass | Timer 5s dans ShowLog → SwitchToScene automatique, log "Got away safely!" |
| AC-4: Retour à SelectAction après chaque tour | Pass | NextPhaseAfterBattle() retourne SelectAction si aucun KO |

## Accomplishments

- HP bar lerp 8f/sec : transition douce ~0.4s après chaque coup, HpBar.cs stateless préservé
- Menu FIGHT/RUN : sélection par Up/Down, ghost-input évité (MoveMenu reconstruit avec Keyboard.GetState() live)
- Fuite sans panneau : ShowLog + _fleeTimer 5s → SwitchToScene direct, aucun écran BattleEnd intermédiaire
- Snap-to-zero : Math.Ceiling bug corrigé (1 HP KO → barre ne restait pas à 1 pendant Victory)
- F6 scénario : PIDGEY Lv10 50/50 HP pour vérifier l'animation lerp visuellement

## Files Created/Modified

| File | Change | Purpose |
|------|--------|---------|
| `src/SDK.MonoGame/Scenes/BattleScene.cs` | Modified | Lerp fields, SelectAction phase, flee timer, snap-to-zero fix |
| `src/SDK.MonoGame/Scenes/WorldScene.cs` | Modified | F6 ScenarioHpLerp() ajouté |

## Decisions Made

| Decision | Rationale | Impact |
|----------|-----------|--------|
| Fuite via timer ShowLog (pas BattleEnd) | Évite panneau noir superflu, "Got away safely!" suffit comme feedback | BattleEnd jamais atteint pour RUN — flux plus propre |
| Snap-to-zero dans Update() pas Draw() | Préserve l'animation jusqu'au seuil 0.5, puis snap — Draw() aurait coupé l'anim dès le premier frame | Balance animation/exactitude |
| _fleeTimer = 5f (pas de Space) | Retour monde automatique : meilleure UX, pas besoin d'input supplémentaire | Patterns timer réutilisables pour cinématiques futures |

## Deviations from Plan

### Summary

| Type | Count | Impact |
|------|-------|--------|
| Auto-fixed | 3 | Essentiels, zéro scope creep |
| Scope additions | 1 | F6 scénario debug, additive |
| Deferred | 0 | Aucun |

**Total impact:** Corrections critiques découvertes au human-verify, comportement utilisateur correctement bouclé.

### Auto-fixed Issues

**1. "You Win" affichée après une fuite réussie**
- **Found during:** checkpoint:human-verify
- **Issue:** `_battleEndOverlay.Draw()` appelé inconditionnellement même quand `_playerRanAway = true` (HP joueur > 0 → Victory affiché)
- **Fix:** Guard `if (!_playerRanAway)` avant `_battleEndOverlay.Draw()` dans Draw()
- **Files:** `src/SDK.MonoGame/Scenes/BattleScene.cs`

**2. Panneau noir après fuite, Space requis deux fois**
- **Found during:** checkpoint:human-verify
- **Issue:** RUN → ShowLog → Space → BattleEnd (panneau noir) → Space → monde. Deux confirmations inutiles.
- **Fix:** `_fleeTimer = 5f` dans ShowLog, countdown dans Update(), `SwitchToScene` auto après 5s. BattleEnd jamais atteint pour RUN.
- **Files:** `src/SDK.MonoGame/Scenes/BattleScene.cs`

**3. Math.Ceiling bug: barre HP reste à 1 après KO**
- **Found during:** human-verify F1 (adversaire 1 HP)
- **Issue:** `_opponentDisplayHp` lerpe vers 0 mais `Math.Ceiling(0.001) = 1` → barre affiche 1 HP pendant Victory
- **Fix:** Snap dans Update(): `if (actualHp == 0 && displayHp < 0.5f) displayHp = 0f`
- **Files:** `src/SDK.MonoGame/Scenes/BattleScene.cs`

### Scope Addition (non-breaking)

**F6 ScenarioHpLerp() — scénario debug dédié**
- PIDGEY Lv10, 50/50 HP, Tackle Lv15 Bulbasaur → ~15-18 dégâts → lerp clairement visible
- Ajouté car tous les scénarios F1-F5 ont l'adversaire à 1 HP (lerp invisible)

## Issues Encountered

| Issue | Resolution |
|-------|------------|
| Plan note "BattleEndOverlay inchangé acceptable MVP" — validé faux au human-verify | Corrigé immédiatement avec guard `!_playerRanAway` |
| Math.Ceiling empêche barre de tomber à 0 | Snap dans Update() quand display < 0.5 et HP réel = 0 |

## Next Phase Readiness

**Ready:**
- HpBar smooth animation opérationnelle (lerp 8f/sec, snap-to-zero propre)
- FIGHT/RUN menu fonctionnel, ghost-input évité
- Fuite UX complète: message 5s + retour automatique monde
- 293/293 tests verts, 0 régression

**Concerns:**
- Multi level-up overlays séquentiels déféré (BattleEngine ne retourne qu'un seul level-up par RunTurn)
- Formule de fuite: RUN réussit toujours (MVP) — Phase 14 ajoutera la formule Gen1/Gen2

**Blockers:**
- Aucun

---
*Phase: 13-exp-levelup-evolution, Plan: 02c*
*Completed: 2026-06-17*
