---
phase: 05-plugins-characters
plan: 01
subsystem: battle
tags: [plugins, ibattleplugin, pluginregistry, battleengine, hooks]

requires:
  - phase: 02-battle-engine
    provides: BattleState immuable, BattleEngine, DamageResult, IDamageFormula, IDifficultyMode
  - phase: 04-scripting-progression
    provides: GameState, SaveSystem, BattleTestHelpers factory

provides:
  - BattleAction record dans SDK.Core.ValueObjects
  - IBattlePlugin 9-membre — contrat NuGet public stable (D-18)
  - PluginRegistry avec fold-pattern state chain dans SDK.Battle.Plugins
  - BattleEngine avec 7 hook-calls injectés, PluginRegistry optionnel (backward-compat)

affects: [05-02-plugins-concrets, 05-03-characters, 08-nuget-distribution]

tech-stack:
  added: []
  patterns:
    - "Fold pattern : BattleState? null = passthrough, non-null remplace état (chaîne séquentielle)"
    - "PluginRegistry dedup par Name — InvalidOperationException sur doublon"
    - "PluginRegistry injectable optionnel — `plugins ?? new PluginRegistry()` dans BattleEngine"

key-files:
  created:
    - src/SDK.Core/ValueObjects/BattleAction.cs
    - src/SDK.Battle/Plugins/PluginRegistry.cs
    - tests/SDK.Battle.Tests/PluginRegistryTests.cs
    - tests/SDK.Battle.Tests/BattleEnginePluginTests.cs
  modified:
    - src/SDK.Core/Interfaces/IBattlePlugin.cs
    - src/SDK.Battle/BattleEngine.cs

key-decisions:
  - "IBattlePlugin = 9 membres : Name + 5 observers void + 2 stubs (OnPokemonCaught/LevelUp) + 2 chain-state (BattleState?)"
  - "OnPokemonCaught + OnPokemonLevelUp définis dans interface mais non appelés par BattleEngine Phase 5 (EncounterSystem non câblé)"
  - "ApplyBeforeDamage injecté APRÈS formula.Calculate, AVANT mutation HP — re-lecture defender depuis state modifié"
  - "NotifyTurnStart placé APRÈS vérification HP KO au début du while — évite appel sur tour terminé"
  - "WeatherType.None (pas .Clear) pour état météo neutre dans tests"

patterns-established:
  - "IDamageFormula et IDifficultyMode sont dans SDK.Battle.Formulas / SDK.Battle.Difficulty — tout fichier les mockant doit importer ces namespaces"
  - "Après ApplyBeforeDamage, re-dériver defender depuis state potentiellement modifié (pas depuis variable locale stale)"
  - "MakeEngineWithSpy() helper pattern : formule dommage=9999, opponentHp=1 → KO en 1 tour garanti"

duration: ~45min
started: 2026-06-06T12:00:00Z
completed: 2026-06-06T14:00:00Z
---

# Phase 5 Plan 01 : Plugin Foundation — Summary

**IBattlePlugin 9-membre + PluginRegistry fold-chain + BattleEngine 7 hooks injectés — 108 tests verts, 0 régression.**

## Performance

| Métrique | Valeur |
|---------|--------|
| Durée | ~45 min |
| Démarré | 2026-06-06 |
| Complété | 2026-06-06 |
| Tâches | 4/4 complétées |
| Fichiers modifiés | 6 |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1 : BattleAction record | Pass | `new BattleAction(25, true)` — MoveId==25, IsPlayer==true |
| AC-2 : IBattlePlugin 9 membres | Pass | Name + 8 hooks; `grep -c "BattleState?" IBattlePlugin.cs` → 2 |
| AC-3 : PluginRegistry dedup + state chain | Pass | 7 tests verts — doublon → InvalidOperationException, fold-chain → Turn==15 |
| AC-4 : Zéro régression | Pass | 97 tests préexistants → tous verts, aucun modifié |
| AC-5 : Hook calls dans BattleEngine | Pass | 4 tests BattleEnginePlugin — BattleStart×1, TurnStart/End×N, Fainted×1 |

## Accomplissements

- `BattleAction(int MoveId, bool IsPlayer)` record créé dans SDK.Core.ValueObjects — requis par OnBeforeMove
- IBattlePlugin remplace le stub 3-hook par un contrat NuGet public stable 9-membre
- PluginRegistry implémente le fold-pattern : chaque plugin peut retourner `null` (passthrough) ou un nouveau `BattleState` (remplace état pour le suivant)
- BattleEngine accepte `PluginRegistry? plugins = null` en 5ème param — aucun code existant cassé
- 7 hook-calls injectés dans RunBattle : NotifyBattleStart, NotifyTurnStart, NotifyTurnEnd, NotifyBattleEnd, NotifyFainted, ApplyBeforeMove×2, ApplyBeforeDamage (dans ApplyMove)

## Files Created/Modified

| Fichier | Changement | Objet |
|---------|-----------|-------|
| `src/SDK.Core/ValueObjects/BattleAction.cs` | Créé | Record requis par OnBeforeMove hook |
| `src/SDK.Core/Interfaces/IBattlePlugin.cs` | Remplacé | Stub 3-hook → contrat 9-membre stable |
| `src/SDK.Battle/Plugins/PluginRegistry.cs` | Créé | Dispatcher + fold-pattern chain-state |
| `src/SDK.Battle/BattleEngine.cs` | Modifié | PluginRegistry optional 5th param + 7 hook-calls |
| `tests/SDK.Battle.Tests/PluginRegistryTests.cs` | Créé | 7 tests AC-3 |
| `tests/SDK.Battle.Tests/BattleEnginePluginTests.cs` | Créé | 4 tests AC-4/AC-5 |

## Decisions Made

| Décision | Rationale | Impact |
|----------|-----------|--------|
| OnPokemonCaught/LevelUp dans interface mais non appelés | Contrat NuGet stable (D-18) — EncounterSystem non câblé avant Phase 6+ | Plugins concrets en 05-02 n'implémentent pas ces hooks pour BattleEngine |
| NotifyTurnStart après vérification KO | Évite TurnStart sur tour déjà terminé — TurnStart×N == TurnsElapsed | Test `CallsTurnStartAndEndPerTurn` vérifie `Times.Exactly(result.TurnsElapsed)` |
| ApplyBeforeDamage re-lit defender depuis state modifié | State peut changer après ApplyBeforeDamage — variable locale `defender` devient stale | Pattern obligatoire pour tout futur hook modifiant BattleState |

## Deviations from Plan

### Summary

| Type | Nb | Impact |
|------|-----|--------|
| Auto-fixed | 3 | Builds corrigés, aucun scope creep |
| Scope additions | 0 | — |
| Déférés | 0 | (D-22 Characters déjà tracé dans STATE.md) |

**Impact total :** Fixes de build mineurs, plan exécuté exactement comme spécifié.

### Auto-fixed Issues

**1. WeatherType.Clear inexistant**
- Trouvé dans : T3 (PluginRegistryTests)
- Problème : Enum `WeatherType` n'a pas de valeur `Clear` — valeurs : None/Sun/Rain/Sand/Hail
- Fix : `WeatherType.Clear` → `WeatherType.None` dans helper `MakeState()`
- Vérification : `dotnet test --filter PluginRegistry` → 7 verts

**2. Using directives manquantes dans BattleEngine.cs**
- Trouvé dans : T4 (réécriture BattleEngine)
- Problème : `using SDK.Battle.Difficulty; using SDK.Battle.Formulas;` omis → 6 erreurs CS0246
- Fix : Deux using directives restaurées en tête de fichier
- Vérification : `dotnet build` → 0 erreurs

**3. Using directives manquantes dans BattleEnginePluginTests.cs**
- Trouvé dans : T4 (tests)
- Problème : `IDamageFormula` et `IDifficultyMode` dans namespaces SDK.Battle, pas SDK.Core.Interfaces
- Fix : `using SDK.Battle.Difficulty; using SDK.Battle.Formulas;` ajoutés
- Vérification : `dotnet test --filter BattleEnginePlugin` → 4 verts

## Issues Encountered

| Problème | Résolution |
|---------|-----------|
| WeatherType.Clear → compile error | Corrigé → WeatherType.None |
| Using directives manquantes ×2 | Ajoutées — leçon : IDamageFormula/IDifficultyMode ∈ SDK.Battle.*, pas SDK.Core |

## Next Phase Readiness

**Prêt :**
- IBattlePlugin stable — 05-02 peut implémenter NuzlockePlugin/RandomizerPlugin/TurboPlugin
- PluginRegistry fonctionnel — `new PluginRegistry(); registry.Register(new NuzlockePlugin())`
- BattleEngine wired — aucune modification nécessaire pour que les plugins concrets fonctionnent
- BattleTestHelpers + pattern MakeEngineWithSpy réutilisables en 05-02

**Concerns :**
- OnPokemonCaught non câblé à BattleEngine — NuzlockePlugin devra recevoir callback depuis EncounterSystem (Plan 05-02 doit définir le pattern)
- Phase 05-02 doit ajouter 3 `.csproj` (SDK.Plugins.Nuzlocke/Randomizer/Turbo) + wiring Game1/Program.cs

**Blockers :** Aucun

---
*Phase: 05-plugins-characters, Plan: 01*
*Complété: 2026-06-06*
