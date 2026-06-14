---
phase: 12-battlescene-ui
plan: 05
type: summary
status: complete
date: 2026-06-14
---

# Summary: Plan 12-05 — WipeTransition + BattleEndOverlay + Full BTLUI-01 Loop

## What Was Built

Flux complet BTLUI-01 fermé en Wave 3. Trois correctifs additionnels appliqués suite aux tests
en session, plus la fonctionnalité de log de combat.

### Fonctionnalités livrées (selon PLAN)

- `WipeTransition.cs`: fondu noir 15 frames overworld → BattleScene
- `BattleEndOverlay.cs`: overlay "YOU WIN!" / "YOU LOSE..." avec prompt espace
- `WorldScene.cs`: déclencheur debug X (remplacé B→X via InputMap.DebugBattle), wipe lancé puis SwitchToScene
- `BattleScene.cs`: phase `BattleEnd`, retour WorldScene via Space après victoire/défaite

### Correctifs appliqués en session (non planifiés)

1. **Space auto-dismiss** (SHA 6d72be9): `_prevKs` manquait snapshot à l'entrée de BattleEnd.
   La touche Space tenue depuis le menu moves déclenchait immédiatement le retour à l'overworld.
   Fix: `_prevKs = Keyboard.GetState()` snapshotté au moment de la transition vers BattleEnd.

2. **Redesign layout** (SHA 500269e): sprites adversaire/joueur se chevauchaient (overlap 36px vertical).
   Nouveau layout classique Pokémon: adversaire top-right (x=262,y=8), joueur bottom-left (x=48,y=90).
   WIN/LOSE déplacé vers le panel dialog en bas (y=193) au lieu du centre écran.

3. **InputMap centralisé** (SHA 222b602): `SDK.MonoGame.Input.InputMap` source unique pour tous
   les bindings. `DebugBattle = Keys.X` (pas B). Tous les sites migrent vers `InputMap.*`.

4. **Remapping touches** (SHA 3f67121): Space=confirm, X=cancel/debug-battle, Shift=turbo.

### Fonctionnalité additionnelle: Log de combat (SHA e2651d4)

Non planifiée dans 12-05 mais livrée suite au feedback utilisateur ("Growl ne loggue rien").

- `BattleEngine.ApplyMove`: génère des messages de log pour chaque outcome (attaque utilisée,
  raté, no effet, super efficace, not very, dégâts, KO, Status placeholder Phase 13).
- `BattleState.Log` réinitialisé à chaque `RunTurn` (un tour = un log).
- Nouvelle phase `ShowLog` dans `BattleScene`: affiche jusqu'à 5 lignes de log dans le panel bas,
  hint "Space" bottom-right, pression Space avance vers SelectMove ou BattleEnd.

## Acceptance Criteria Results

| AC | Description | Résultat |
|----|-------------|----------|
| AC-1 | WipeTransition 15 frames, OnComplete déclenché | ✅ Visuellement validé |
| AC-2 | Déclencheur debug X (InputMap.DebugBattle) → wipe → BattleScene | ✅ Fonctionne |
| AC-3 | BattleEndOverlay WIN/LOSE + Space retour WorldScene | ✅ Fonctionne, panel bas |
| AC-4 | HeadlessSmokeTests 3/3 verts | ✅ |

## Commits (Wave 3)

| SHA | Description |
|-----|-------------|
| `3f67121` | feat(input): remap controls — Space=confirm, X=cancel, Shift=turbo cycle |
| `222b602` | feat(input): add InputMap — centralize all key bindings |
| `6d72be9` | fix(battle): snapshot _prevKs on BattleEnd entry — prevent Space auto-dismiss |
| `500269e` | fix(battle-ui): redesign layout — no sprite overlap, WIN/LOSE in bottom panel |
| `e2651d4` | feat(battle-log): log combat events + ShowLog phase between turns |

## Files Modified

| Fichier | Type |
|---------|------|
| `src/SDK.MonoGame/Scenes/WipeTransition.cs` | Nouveau |
| `src/SDK.MonoGame/UI/BattleEndOverlay.cs` | Nouveau + modifié |
| `src/SDK.MonoGame/Scenes/WorldScene.cs` | Modifié |
| `src/SDK.MonoGame/Scenes/BattleScene.cs` | Modifié |
| `src/SDK.MonoGame/Input/InputMap.cs` | Nouveau |
| `src/SDK.Battle/BattleEngine.cs` | Modifié |

## Decisions Made

- `BattleEndOverlay` dans panel bas (y=178-270) plutôt que centre écran: cohérent avec le style
  dialog classique Pokémon.
- `ShowLog` phase entre SelectMove et BattleEnd/SelectMove: force le joueur à lire les messages
  avant de continuer, évite le flash instantané.
- Status moves (Growl, etc.) loggués comme "stat effects: Phase 13" — implémentation réelle
  des stages ATK/DEF déférée à Phase 13.
- `_prevKs` doit être snapshotté À CHAQUE transition de phase pour éviter le key bleed-through.

## Deferred Issues

| Issue | Phase |
|-------|-------|
| Stat stage system (ATK/DEF +/- stages, Growl réel) | Phase 13 |
| Sprites réels PNG (D-16 naming convention) | Phase 17 |
| Audio SFX transition + cries | Phase 18 |
| Smooth HP bar decrease animation | Phase 13+ |

## Requirement Status

**BTLUI-01**: HP bars + sprites placeholder + MoveMenu + StatusIcon + BattleEnd overlay + retour
WorldScene = **DONE**. Checkpoint human-verify approuvé le 2026-06-14.
