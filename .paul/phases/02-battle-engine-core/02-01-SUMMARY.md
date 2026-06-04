---
phase: 02-battle-engine-core
plan: 01
subsystem: domain
tags: [sdk-core, entities, enums, interfaces, value-objects, battle-engine]

requires:
  - phase: 01-sdk-core-data plan 01-04
    provides: SDK.Core (PokemonSpecies, PokemonType, BattleConfig, IRepository), SqliteTestFixture, PlatformTests pattern

provides:
  - Move entity — Id, Identifier, TypeId, Category, Power?, Accuracy, PP, Generation, nav Type
  - Ability entity — Id, Identifier, Generation
  - Learnset entity — Id, SpeciesId, MoveId, LearnLevel, Generation, nav Species, nav Move
  - MoveCategory enum — Physical | Special | Status
  - WeatherType enum — None | Sun | Rain | Sand | Hail
  - IBattleEngine interface — RunBattle(BattleConfig) → BattleResult
  - IBattlePlugin interface — Name, OnBattleStart, OnTurnEnd, OnBattleEnd
  - BattleResult sealed record — PlayerWon, TurnsElapsed, EndReason?
  - DamageResult sealed record — Damage, IsCritical, TypeMultiplier
  - CoreBattleDependencyTests — vérifie SDK.Battle 0 NuGet externe

affects: [02-02, 02-03, 02-04, battle-engine, all-future-battle-tests]

tech-stack:
  added: []
  patterns:
    - Interfaces battle dans SDK.Core/Interfaces (IBattleEngine, IBattlePlugin)
    - Value objects immuables comme sealed record (BattleResult, DamageResult)
    - Dependency scan test via XDocument.Load(.csproj) — même pattern que PlatformTests

key-files:
  created:
    - src/SDK.Core/Entities/Move.cs
    - src/SDK.Core/Entities/Ability.cs
    - src/SDK.Core/Entities/Learnset.cs
    - src/SDK.Core/Enums/MoveCategory.cs
    - src/SDK.Core/Enums/WeatherType.cs
    - src/SDK.Core/Interfaces/IBattleEngine.cs
    - src/SDK.Core/Interfaces/IBattlePlugin.cs
    - src/SDK.Core/ValueObjects/BattleResult.cs
    - src/SDK.Core/ValueObjects/DamageResult.cs
    - tests/SDK.Core.Tests/CoreBattleDependencyTests.cs
  modified: []

key-decisions:
  - "Move.Power nullable (int?) — les moves de statut (ex: Rugissement) n'ont pas de puissance"
  - "Learnset.LearnLevel = 0 → CT/CS, >0 → montée de niveau — convention simple sans enum"
  - "BattleResult/DamageResult = sealed record → immuabilité garantie par le compilateur (D-05)"
  - "IBattleEngine.RunBattle prend BattleConfig, pas BattleState — BattleState défini en Plan 02-03"

patterns-established:
  - "Dependency scan tests via XDocument.Load — réutilisable pour SDK.Scripting, SDK.Tools (futurs plans)"
  - "Interfaces domaine dans SDK.Core/Interfaces — implémentations dans projets enfants"

duration: ~10min
started: 2026-06-03T19:20:00Z
completed: 2026-06-03T19:30:00Z
---

# Phase 2 Plan 01 : SDK.Core Battle Models Summary

**9 types de domaine ajoutés dans SDK.Core (Move/Ability/Learnset/MoveCategory/WeatherType/IBattleEngine/IBattlePlugin/BattleResult/DamageResult) — SDK.Core reste 0 NuGet externe, 13/13 tests verts.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~10 min |
| Démarré | 2026-06-03T19:20Z |
| Complété | 2026-06-03T19:30Z |
| Tâches | 2/2 complétées |
| Fichiers créés | 10 |
| Fichiers modifiés | 0 |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: Entités battle compilables | Pass | Move/Ability/Learnset compilent, nav properties correctes |
| AC-2: Enums/interfaces/value objects compilables | Pass | 5 types ajoutés, IBattleEngine/IBattlePlugin signés, records immuables |
| AC-3: SDK.Battle zéro NuGet | Pass | CoreBattleDependencyTests vert, 0 PackageReference dans SDK.Battle.csproj |

## Accomplishments

- `Move` : entité complète avec `Power?` nullable (moves statut sans puissance) et nav property vers `PokemonType`
- `Ability`, `Learnset` : entités minimales et correctes — `Learnset.LearnLevel = 0` pour CT/CS
- `IBattleEngine` / `IBattlePlugin` : contrats domaine dans SDK.Core, implémentations déléguées à SDK.Battle (Plan 02-03)
- `BattleResult` / `DamageResult` : `sealed record` → immuabilité compilateur (D-05)
- `CoreBattleDependencyTests` : scan automatique SDK.Battle.csproj via XDocument — même pattern que PlatformTests (réutilisable)
- **13/13 tests verts** : 4 SDK.Core.Tests + 9 SDK.Data.Tests (aucune régression)

## Fichiers Créés / Modifiés

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `src/SDK.Core/Entities/Move.cs` | Créé | Entité attaque — TypeId FK, Category, Power? |
| `src/SDK.Core/Entities/Ability.cs` | Créé | Entité talent passif |
| `src/SDK.Core/Entities/Learnset.cs` | Créé | Liaison espèce ↔ move ↔ génération ↔ niveau |
| `src/SDK.Core/Enums/MoveCategory.cs` | Créé | Physical / Special / Status |
| `src/SDK.Core/Enums/WeatherType.cs` | Créé | None / Sun / Rain / Sand / Hail |
| `src/SDK.Core/Interfaces/IBattleEngine.cs` | Créé | Contrat moteur — RunBattle(BattleConfig) → BattleResult |
| `src/SDK.Core/Interfaces/IBattlePlugin.cs` | Créé | Contrat plugin — 3 hooks : Start/TurnEnd/End |
| `src/SDK.Core/ValueObjects/BattleResult.cs` | Créé | Résultat immuable (PlayerWon, TurnsElapsed, EndReason?) |
| `src/SDK.Core/ValueObjects/DamageResult.cs` | Créé | Dégâts immuables (Damage, IsCritical, TypeMultiplier) |
| `tests/SDK.Core.Tests/CoreBattleDependencyTests.cs` | Créé | Scan csproj SDK.Battle — 0 NuGet autorisé |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| `Move.Power` nullable | Moves de statut (Rugissement, Rugissement) n'ont pas de puissance | Plan 02-02 seed sans valeur Power pour ces moves |
| `Learnset.LearnLevel = 0` → CT/CS | Simple, pas d'enum, compatible filtrage par niveau | Plan 02-03 utilise `LearnLevel > 0` pour filtrer level-up moves |
| `IBattleEngine.RunBattle(BattleConfig)` sans BattleState | BattleState n'existe pas encore (Plan 02-03) | Plan 02-03 peut signer BattleState librement puis implémenter IBattleEngine |

## Déviations du Plan

**1. Build flag MSB3492 sur WSL2**
- **Trouvé pendant :** Task 1 verify
- **Problème :** `dotnet build -q` retournait MSB3492 (cache file lock) — faux négatif WSL2
- **Fix :** Build sans `-q` → 0 erreur, 0 warning
- **Impact :** Aucun sur le code produit

## Issues Rencontrées

| Issue | Résolution |
|-------|------------|
| `dotnet build -q` MSB3492 faux-positif WSL2 | Suppresseur de sortie `-q` incompatible avec WSL2 cache check — build sans flag résout |

## Readiness pour Plan 02-02

**Prêt :**
- `Move`, `Ability` entités SDK.Core disponibles pour migration EF Core (Plan 02-02)
- `Learnset` prêt pour seeding génération 1 (Tackle / Growl / Scratch...)
- Interfaces domaine stabilisées — SDK.Battle (Plan 02-03) peut implémenter sans attendre
- Pattern CoreBattleDependencyTests établi — réutilisable pour SDK.Scripting, SDK.Tools

**Déférés à surveiller :**
- Configuration EF Core Fluent API (Move, Ability, Learnset) → Plan 02-02
- BattleDataSeeder (type effectiveness chart 18×18, moves gen 1) → Plan 02-02
- BattleState immuable, IDamageFormula, IDifficultyMode → Plan 02-03

**Blockers :**
Aucun.

---
*Phase: 02-battle-engine-core, Plan: 01*
*Complété: 2026-06-03*
