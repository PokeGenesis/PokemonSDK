---
phase: 13-exp-levelup-evolution
plan: 03
subsystem: ui
tags: [monogame, battlescene, evolution, overlay, animation]

requires:
  - phase: 13-02c
    provides: HP bar lerp + LevelUp/MoveLearn overlays + FIGHT/RUN menu

provides:
  - EvolutionData record (SDK.Core.ValueObjects)
  - BattlePokemon evolution fields (EvolvesAtLevel, EvolvesToSpeciesId, EvolvesToName)
  - BattleState.PendingEvolution
  - BattleEngine evolution detection in AwardExp()
  - EvolutionOverlay with flash/cancel/confirm phases
  - BattleScene.ShowEvolution phase
  - F7 debug scenario (BULBASAUR Lv5 → IVYSAUR)

affects: [Phase 14 items, Phase 17 PokeAPI wiring]

tech-stack:
  added: []
  patterns: [Trigger/Update/Draw overlay lifecycle, EvoPhase state machine with Cancelled variant]

key-files:
  created:
    - src/SDK.Core/ValueObjects/EvolutionData.cs
    - src/SDK.MonoGame/UI/EvolutionOverlay.cs
  modified:
    - src/SDK.Core/ValueObjects/BattlePokemon.cs
    - src/SDK.Core/ValueObjects/BattleState.cs
    - src/SDK.Battle/BattleEngine.cs
    - src/SDK.MonoGame/Scenes/BattleScene.cs
    - src/SDK.MonoGame/Scenes/WorldScene.cs

key-decisions:
  - "EvoPhase.Cancelled ajouté post-plan: X → message 'Oh? X stopped evolving!' → Space dismiss"
  - "Evolution MVP: level-up uniquement. Pierres/échange = Phase 17"
  - "EvolutionOverlay pattern identique LevelUpOverlay (Trigger/Update/Draw)"

patterns-established:
  - "3-phase overlay (Flashing → Done | Cancelled) pour séquences annulables"

duration: ~45min
started: 2026-06-17T19:42:00Z
completed: 2026-06-17T22:08:00Z
---

# Phase 13 Plan 03: Evolution UI Summary

**EvolutionOverlay complet: flash 2s + cancel "Oh? stopped evolving!" + confirm "evolved into X!" — Phase 13 (BTLUI-02) clôturée à 100%.**

## Performance

| Metric | Value |
|--------|-------|
| Duration | ~45 min |
| Started | 2026-06-17T19:42Z |
| Completed | 2026-06-17T22:08Z |
| Tasks | 2 complètes |
| Files modified | 5 modifiés + 2 créés |

## Acceptance Criteria Results

| Criterion | Status | Notes |
|-----------|--------|-------|
| AC-1: BattleEngine détecte PendingEvolution | Pass | AwardExp() set PendingEvolution quand newLevel == EvolvesAtLevel |
| AC-2: EvolutionOverlay flash + annulable X | Pass | X → EvoPhase.Cancelled (scope add: message "Oh? stopped evolving!") |
| AC-3: BattleScene orchestre ShowEvolution | Pass | ShowLog → ShowLevelUp → ShowEvolution → apply/rollback + NextPhaseAfterBattle |
| AC-4: 0 régression — 293+ tests | Pass | 293/293 verts, 0 erreurs build |

## Accomplishments

- `EvolutionData` record immuable dans SDK.Core, sans dépendance NuGet
- `BattlePokemon` : 3 champs evolution optionnels ajoutés en fin de record (0 régression)
- `BattleEngine.AwardExp()` détecte le seuil d'évolution dans la boucle level-up
- `EvolutionOverlay` : Flashing (flash blanc/noir 0.1s) → Done ("Congratulations!") ou Cancelled ("Oh? stopped!")
- `BattleScene.ShowEvolution` : applique `Nickname + SpeciesId` si confirm, rollback si cancel
- F7 debug : BULBASAUR Lv5 → Lv6 → IVYSAUR (confirm) ou BULBASAUR inchangé (cancel X)
- ROADMAP SC5 fermé : évolution annulable par X pendant le flash

## Task Commits

Implémentation continue (pas de commits atomiques par tâche dans cette session).

## Files Created/Modified

| File | Change | Purpose |
|------|--------|---------|
| `src/SDK.Core/ValueObjects/EvolutionData.cs` | Créé | Record `(OldName, NewName, NewSpeciesId)` |
| `src/SDK.Core/ValueObjects/BattlePokemon.cs` | Modifié | +3 champs optionnels évolution |
| `src/SDK.Core/ValueObjects/BattleState.cs` | Modifié | +`PendingEvolution` nullable |
| `src/SDK.Battle/BattleEngine.cs` | Modifié | `AwardExp()` détecte et publie `PendingEvolution` |
| `src/SDK.MonoGame/UI/EvolutionOverlay.cs` | Créé | Overlay flash/cancel/confirm (3 phases) |
| `src/SDK.MonoGame/Scenes/BattleScene.cs` | Modifié | Phase `ShowEvolution` + `TriggerEvolution()` |
| `src/SDK.MonoGame/Scenes/WorldScene.cs` | Modifié | F7 debug + `ScenarioEvolution()` |

## Decisions Made

| Decision | Rationale | Impact |
|----------|-----------|--------|
| `EvoPhase.Cancelled` ajouté (scope add) | Pokémon authentique: "Oh? X stopped evolving!" manquait au cancel | Meilleure UX, AC-2 enrichi |
| MVP level-up uniquement | Pierres/échange = Phase 17 avec vraies données PokeAPI | 0 scope creep UI Phase 13 |
| Pas de sprite évoluée | Placeholder gris existant suffit | Phase 17 câble les sprites réels |

## Deviations from Plan

### Summary

| Type | Count | Impact |
|------|-------|--------|
| Scope additions | 1 | EvoPhase.Cancelled — UX authentique Pokémon |
| Auto-fixed | 0 | |
| Deferred | 0 | |

**Total impact:** Addition mineure post-checkpoint demandée par le joueur, aucun scope creep non sollicité.

### Scope Addition

**EvoPhase.Cancelled — "Oh? [Name] stopped evolving!" message**
- **Découvert pendant:** checkpoint:human-verify
- **Demande:** user signale absence du message de cancel officiel Pokémon
- **Solution:** 3e phase `Cancelled` dans `EvoPhase` — X → Cancelled → message → Space dismiss
- **Impact AC-2:** satisfait + enrichi (cancel maintenant deux étapes: flash interrompu → confirmation dismiss)

## Issues Encountered

| Issue | Resolution |
|-------|------------|
| User voit "5→8" niveaux en F7 | F7 donne 1 niveau (5→6) — user pressait F5 (ScenarioMultiLevelUp) par confusion |
| PLAN AC-2 spécifiait cancel immédiat (IsComplete=true direct) | Enrichi: cancel montre message "Oh? stopped" → Space. Plus fidèle à l'original. |

## Next Phase Readiness

**Ready:**
- Phase 13 (BTLUI-02) clôturée à 100% — EXP + Level-up + Évolution complets
- `PendingEvolution` sur `BattleState` extensible (Phase 17 câblera les vraies évolutions PokeAPI)
- Pattern overlay Trigger/Update/Draw établi pour Phase 14 (Items/Bag UI)
- `BattlePokemon.EvolvesAtLevel` MVP prêt — Phase 17 remplacera par données DB

**Concerns:**
- Evolution par pierre / échange non implémentée (déféré Phase 17, hors scope MVP)
- `SpeciesId` mis à jour dans `BattleState.Player` mais pas persisté en GameState/SaveSystem (hors scope Phase 13)

**Blockers:** Aucun.

---
*Phase: 13-exp-levelup-evolution, Plan: 03*
*Completed: 2026-06-17*
