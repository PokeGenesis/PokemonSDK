---
phase: 13-exp-levelup-evolution
plan: 02
subsystem: ui
tags: [monogame, expbar, levelup, movelearnoverlay, battlescene, exp]

requires:
  - phase: 13-01
    provides: AwardExp headless engine, PendingLearnedMoves, level-up log messages

provides:
  - ExpBar animee avec intra-level ratio correct (Update/Draw split)
  - LevelUpOverlay avec stat deltas dismissable (Space)
  - MoveLearnOverlay avec choix oubli move (Space/X)
  - BattlePhase: ShowLevelUp + ShowMoveLearn integres dans la state machine
  - 5 scenarios debug F1-F5 dans WorldScene
affects: [13-03, 13-02b]

tech-stack:
  added: []
  patterns:
    - "ExpBar Update/Draw split — animation interpolee separee du rendu"
    - "MoveLearnOverlay pattern identique BattleEndOverlay — IsVisible + DecisionMade"
    - "ShowLog comme hub de transition universel apres chaque sous-phase"

key-files:
  created:
    - src/SDK.MonoGame/UI/ExpBar.cs
    - src/SDK.MonoGame/UI/LevelUpOverlay.cs
    - src/SDK.MonoGame/UI/MoveLearnOverlay.cs
  modified:
    - src/SDK.MonoGame/Scenes/BattleScene.cs
    - src/SDK.MonoGame/Scenes/WorldScene.cs

key-decisions:
  - "ExpBar animee livrée immediatement (plan disait instantanée — scope positif)"
  - "MoveLearnOverlay integree dans 13-02 (etait prevue plus tard)"
  - "ShowLog = hub universel apres ShowMoveLearn (pas de NextPhaseAfterBattle direct)"
  - "Phase 12 gaps P12-G1..G6 decouverts → planifies 13-02b"
  - "P12-G7 (masquer HP adversaire) droppe — jeux modernes affichent les chiffres"

patterns-established:
  - "BattlePhase enum: Init → SelectMove → ShowLog → ShowLevelUp → ShowMoveLearn → BattleEnd"
  - "ApplyMoveLearnDecision() appende a _lastLog avant de retourner a ShowLog"
  - "Intra-level ratio: (currentExp - threshold(level)) / (threshold(level+1) - threshold(level))"

duration: ~4h (session 2026-06-16)
started: 2026-06-16T19:00:00Z
completed: 2026-06-16T22:30:00Z
---

# Phase 13 Plan 02: ExpBar UI + LevelUpOverlay + MoveLearnOverlay Summary

**ExpBar animee + LevelUpOverlay + MoveLearnOverlay livres avec etat machine ShowLevelUp/ShowMoveLearn et 3 bug fixes critiques decouverts lors des tests visuels.**

## Performance

| Metrique | Valeur |
|---------|--------|
| Duree | ~4h |
| Demarree | 2026-06-16 ~19h00 |
| Completee | 2026-06-16 ~22h30 |
| Taches | 4 (3 auto + 1 human-verify) |
| Fichiers modifies | 5 |

## Acceptance Criteria Results

| Critere | Statut | Notes |
|---------|--------|-------|
| AC-1: ExpBar visible sous HP joueur | Pass | Animee (bonus vs plan instantane) |
| AC-2: LevelUpOverlay declenche apres level-up | Pass | _leveledUp flag + Trigger() |
| AC-3: Overlay affiche stat deltas corrects | Pass | +ATK/DEF/SpA/SpD/Spe/HP |
| AC-4: Overlay dismissable -> flux normal | Pass | Space → ShowLog → BattleEnd/SelectMove |
| AC-5: Backward-compat sans IExpFormula | Pass | Build 0 erreurs, constructeur inchange |

## Accomplissements

- ExpBar anime avec ratio intra-level correct (fix ratio bug decouvert visuellement)
- LevelUpOverlay stat deltas instantanes, dismissable Space
- MoveLearnOverlay — choix "forget which move?" avec navigation Up/Down/Space/X
- Phase state machine etendue: ShowLevelUp + ShowMoveLearn integres
- PendingMoveQueue: plusieurs moves a apprendre par tour gere via Queue<BattleMove>
- 5 scenarios debug F1-F5 dans WorldScene (EXP bar, level-up auto-learn, overlay, level cap, multi-level)
- Phase 12 gap audit independant (7 gaps identifies, 6 retenus pour 13-02b, 1 droppe)

## Files Created/Modified

| Fichier | Changement | Objet |
|---------|------------|-------|
| `src/SDK.MonoGame/UI/ExpBar.cs` | Cree + modifie | Barre EXP avec Update() animation + Draw() |
| `src/SDK.MonoGame/UI/LevelUpOverlay.cs` | Cree | Overlay stat deltas dismissable |
| `src/SDK.MonoGame/UI/MoveLearnOverlay.cs` | Cree (non planifie) | Choix oubli move — cursor Up/Down/Space/X |
| `src/SDK.MonoGame/Scenes/BattleScene.cs` | Modifie | Phase machine etendue + 3 bug fixes |
| `src/SDK.MonoGame/Scenes/WorldScene.cs` | Modifie | 5 scenarios debug F1-F5 |

## Decisions Prises

| Decision | Rationale | Impact |
|----------|-----------|--------|
| ExpBar animee (Update/Draw split) | Qualite visuelle immediate | Pattern reutilisable HpBar smooth |
| MoveLearnOverlay dans 13-02 | Logiquement lie a ShowLog chain | Phase 13-03 (Evolution) peut builder dessus |
| ShowLog comme hub universel | Routing deja complet (_leveledUp, _pendingMoveQueue) | Pas de duplication logique de transition |
| Intra-level ratio corrige | Ancien ratio total/nextThreshold faux apres level-up | Barre correcte dans tous les scenarios |

## Deviations du Plan

### Recap

| Type | Nombre | Impact |
|------|--------|--------|
| Scope additions | 3 | Positif — MoveLearnOverlay + animation + debug scenarios |
| Bugs auto-fixes | 3 | Critique — decouverts tests visuels uniquement |
| Deferred | 6 | Planifies 13-02b |

### Scope Additions

**1. ExpBar animee (Update/Draw split)**
- Plan disait: fill instantane
- Livre: interpolation via Update(gameTime, intraExp, intraRange)
- Pas de regression

**2. MoveLearnOverlay (non planifie dans 13-02)**
- Plan 13-02 ne mentionnait pas MoveLearnOverlay
- Livre: MoveLearnOverlay.cs complet + ShowMoveLearn phase + PendingMoveQueue
- Consequence: 13-03 (Evolution) peut directement builder sur cette base

**3. WorldScene debug scenarios F1-F5**
- Plan ne mentionnait pas de scenarios debug
- Livre: 5 scenarios via touches F1-F5 pour tester EXP/level-up/overlay

### Bugs Auto-fixes

**Bug #1: Ratio intra-level ExpBar**
- Trouve: test visuel F1 — barre affichait ~74% au lieu de ~31% apres level-up
- Cause: `totalExp / nextThreshold` (ratio cumulatif) au lieu de `(currentExp - currentThreshold) / (nextThreshold - currentThreshold)` (ratio intra-level)
- Fix: BattleScene.Update() — deux appels ExpThreshold (current + next)
- Fichier: `src/SDK.MonoGame/Scenes/BattleScene.cs`

**Bug #2: MoveMenu stale apres move-swap**
- Trouve: apres MoveLearnOverlay decision, MoveMenu affichait anciens moves
- Cause: `_moveMenu` non rebuild apres `_state with { Moves = newMoves }`
- Fix: `_moveMenu = new MoveMenu(_state.Player.Moves, _graphicsDevice!)` dans ApplyMoveLearnDecision()
- Fichier: `src/SDK.MonoGame/Scenes/BattleScene.cs`

**Bug #3: Message "forgot X learned Y" absent + sortie directe**
- Trouve: F3 — apres selection move a oublier, combat quittait directement sans afficher le message
- Cause: (a) ApplyMoveLearnDecision() loggait seulement en Serilog, pas dans _lastLog; (b) ShowMoveLearn appelait NextPhaseAfterBattle() direct
- Fix: ApplyMoveLearnDecision() appende a _lastLog; ShowMoveLearn toujours vers ShowLog
- Fichier: `src/SDK.MonoGame/Scenes/BattleScene.cs`

**Corrections donnees WorldScene**
- F1/F4/F5 avaient CurrentExp sous le seuil MediumFast de leur niveau (masque par l'ancien ratio)
- Fix: F1 950→1235, F4 2150→2300, F5 100→150

### Deferred (Phase 13-02b)

| Gap | Description |
|-----|-------------|
| P12-G1 | HpBar smooth animation (snap instantane actuellement) |
| P12-G2 | PP display dans MoveMenu |
| P12-G3 | PP deduction dans BattleEngine.ApplyMove |
| P12-G4 | Nickname dans HpBar au lieu de "FOE"/"PLR" |
| P12-G5 | Menu FIGHT/RUN au-dessus du MoveMenu |
| P12-G6 | Affichage meteo active en combat |

## Next Phase Readiness

**Prets:**
- ShowLevelUp + ShowMoveLearn pleinement fonctionnels et testes visuellement
- PendingMoveQueue gere les multi-moves par tour
- Pattern BattlePhase extensible pour 13-03 (Evolution)
- 5 scenarios debug reutilisables pour valider 13-02b et 13-03

**Concerns:**
- 13-02b a planifier avant 13-03 (gaps P12-G1..G6 encore ouverts)
- MoveLearnOverlay.cs non commite — a inclure dans le commit de cloture

**Blockers:**
- Aucun

---
*Phase: 13-exp-levelup-evolution, Plan: 02*
*Complete: 2026-06-16*
