---
phase: 09-sample-project
plan: 04
subsystem: sample
tags: [monogame, battle-engine, lua-scripting, save-system, nuzlocke, nuget]

requires:
  - phase: 09-03
    provides: OverworldScene playable tilemap + NPC stub + BGM
  - phase: 08-01
    provides: PokeForge.SDK 0.1.0 NuGet meta-package (SDK.Battle, SDK.Scripting, ISaveSystem)

provides:
  - StarterGame fully wired avec BattleEngine 1v1 headless + NuzlockePlugin
  - Lua badge scripting (BadgeApi.AwardBadge via npc_dialogue.lua)
  - Save/load GameState JSON F5/F9
  - HUD badge visuel en or
  - README getting-started DX-04

affects: [phase-10-cli, phase-11-docs]

tech-stack:
  added: []
  patterns:
    - "NeutralTypeChart private inner class — ITypeChart minimal pour sample"
    - "BadgeApi pattern par interaction — new api, RegisterApi, LoadFile, GetState()"
    - "Just-pressed edge detection via _prevKb (KeyboardState diff)"
    - "NuzlockePlugin lambda capture _gameState — copy-on-write sur field instance"

key-files:
  created:
    - samples/StarterGame/Content/Scripts/npc_dialogue.lua
    - samples/StarterGame/README.md
  modified:
    - samples/StarterGame/Game1.cs
    - samples/StarterGame/Scenes/OverworldScene.cs
    - samples/StarterGame/Content/Content.mgcb

key-decisions:
  - "NeutralTypeChart inline (inner class) — pas de fichier séparé dans sample"
  - "F5/F9 just-pressed (not held) — edge detection via _prevKb pour single fire"
  - "BadgeApi créé par interaction NPC (pas singleton) — accumulation via GetState()"
  - "File.Exists guard sur _scriptPath — safe fallback si MGCB non build"
  - "_dialogue reset uniquement quand aucune touche action pressée"

patterns-established:
  - "Sample consomme SDK via NuGet uniquement (D-19 zéro ProjectReference)"
  - "StarterGame = preuve DX-04 : dotnet add package PokeForge.SDK → combat + scripting + save"

duration: ~25min
started: 2026-06-07T19:04:00Z
completed: 2026-06-07T21:12:00Z
---

# Phase 9 Plan 04 : StarterGame Wave 3 — SDK Integration Complète

**BattleEngine 1v1 headless + NuzlockePlugin + Lua badge + ISaveSystem F5/F9 câblés dans StarterGame NuGet-only (D-19) — Phase 9 complète, DX-04 livré.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~25 min |
| Démarré | 2026-06-07T19:04Z |
| Complété | 2026-06-07T21:12Z |
| Tâches | 3/3 complétées |
| Fichiers modifiés | 5 |

## Acceptance Criteria Results

| Critère | Statut | Détail |
|---------|--------|--------|
| AC-1 : Combat NPC complet | **PASS** | BattleEngine.RunBattle(Bulbasaur L5 vs Rattata L3) → BattleResult(PlayerWon, TurnsElapsed > 0) affiché en dialogue box |
| AC-2 : Badge Lua | **PASS** | npc_dialogue.lua `badges:AwardBadge('boulder')` → badge_boulder flag true → "Badge: boulder ✓" HUD or |
| AC-3 : Save/load cycle | **PASS** | F5 just-pressed → save1.json JSON, F9 just-pressed → reload → badge HUD persiste |
| AC-4 : Build 0 err + headless exit 0 + D-19 | **PASS** | 0 Error(s) Release, headless EXIT:0, `grep ProjectReference \| wc -l` → 0 |

## Accomplissements

- **BattleEngine synchrone câblé** : Bulbasaur L5 vs Rattata L3 (Tackle×2), NuzlockePlugin avec callback lambda capturant `this._gameState` pour mutation copy-on-write
- **Pipeline Lua badge** : `new BadgeApi(_gameState)` → `RegisterApi("badges", api)` → `LoadFile(_scriptPath)` → `_gameState = api.GetState()` — pattern identique à Plan 04-02
- **Save/load just-pressed** : `_prevKb` diff (edge detection), `Directory.CreateDirectory("data")` guard, `ISaveSystem.Save/Load` avec `data/save1.json`
- **HUD badge or** : `DrawString(_font, "Badge: boulder ✓", ..., Color.Gold)` conditionnel sur `GetFlag<bool>("badge_boulder")`
- **MGCB `/copy:` directive** : npc_dialogue.lua copié verbatim vers `Content/bin/DesktopGL/Content/Scripts/`

## Task Commits

| Tâche | Commit | Type | Description |
|-------|--------|------|-------------|
| Task 1+2+3 groupés | `b9a2990` | feat | StarterGame Wave 3 — SDK.Battle + Scripting + ISaveSystem (5 fichiers, 160 insertions) |
| Paul state | `9f06d0d` | chore | STATE.md + paul.json — loop 09-04 APPLY ✓ |

## Fichiers Créés/Modifiés

| Fichier | Changement | Objet |
|---------|-----------|-------|
| `samples/StarterGame/Game1.cs` | Modifié | +ISaveSystem, +LuaScriptEngine fields, OverworldScene 3-args |
| `samples/StarterGame/Scenes/OverworldScene.cs` | Réécriture complète | BattleEngine + NuzlockePlugin + Lua badge + F5/F9 + HUD |
| `samples/StarterGame/Content/Scripts/npc_dialogue.lua` | Créé | `badges:AwardBadge('boulder')` |
| `samples/StarterGame/Content/Content.mgcb` | Modifié | `/copy:Scripts/npc_dialogue.lua` |
| `samples/StarterGame/README.md` | Créé | Getting-started 15 lignes, contrôles, features SDK |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| NeutralTypeChart = inner class privée | Sample self-contained, pas d'export inutile | Aucune API publique ajoutée |
| BadgeApi instanciée par interaction NPC | Accumulation via GetState() — stateless engine | Pattern cohérent avec Plan 04-02 |
| F5/F9 just-pressed (edge detection) | Prévient déclenchements répétés si touche tenue | _prevKb diff à chaque frame Update() |
| File.Exists guard sur _scriptPath | Safe fallback si MGCB content non build | Headless ne crash pas sans Content |
| _dialogue reset si aucune touche action | Dialogue persiste tant que touche pressée | UX : résultat visible le temps de lire |

## Déviations du Plan

### Résumé

| Type | Nombre | Impact |
|------|--------|--------|
| Auto-fixées | 1 | Essentiel, 0 scope creep |
| Additions de scope | 0 | — |
| Différées | 0 | — |

**Impact total :** Fix nécessaire découvert en Task 1, résolu immédiatement.

### Auto-fixées

**1. Constructor mismatch temporaire (build error CS1729)**
- **Découvert pendant :** Task 1 (Game1.cs mis à jour avant OverworldScene)
- **Problème :** Game1.cs passe 3 args à OverworldScene, mais OverworldScene encore 1-arg
- **Fix :** Task 2 immédiate — OverworldScene réécriture avec constructeur 3-args
- **Vérification :** Build 0 Error(s) après Task 2
- **Commit :** b9a2990 (tasks 1+2+3 groupés)

## Issues Rencontrées

| Issue | Résolution |
|-------|-----------|
| cbm-code-discovery-gate bloque `Read` sur .cs | Fallback Bash `grep -n "^namespace"` + `mcp__codebase-memory-mcp__get_code_snippet` pour lire source |
| MGCB invocation nécessite `/workingDir` | `dotnet-mgcb build <path>` sans cwd échoue silencieusement ; résolu via invocation correcte |

## Aucun Problème Résiduel

Phase 9 complète. 0 dette technique dans StarterGame.

## Next Phase Readiness

**Prêt :**
- StarterGame = demo jouable DX-04 ✅ — `dotnet run` → overworld + NPC combat + badge Lua + save
- PokeForge.SDK 0.1.0 NuGet validé end-to-end par un consumer réel
- README getting-started prêt pour documentation Phase 11

**Préoccupations :**
- Phase 10 (CLI `pokeforge`) : dépend du sample stabilisé (D-20) ✅ — condition remplie
- Phase 11 (Docs) : APIs stables uniquement (D-21) — SDK.Battle, SDK.Scripting, ISaveSystem tous stables

**Bloqueurs :**
- Aucun

---
*Phase: 09-sample-project, Plan: 04*
*Complété: 2026-06-07*
