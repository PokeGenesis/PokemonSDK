---
phase: 09-sample-project
plan: 01
subsystem: nuget
tags: [nuget, meta-package, distribution, sdk-bundle]

requires:
  - phase: 08-nuget-distribution
    provides: 7 packages publiés sur NuGet.org (PokeForge.SDK.Core/Data/Battle/Scripting/Plugins.*)

provides:
  - src/SDK.Bundle/SDK.Bundle.csproj — meta-package PokeForge.SDK (8e package)
  - tests/SDK.Plugins.Turbo.Tests/ — dette Phase 5 comblée (5 tests)

affects: [09-02-sample-project, publish-nuget]

tech-stack:
  added: []
  patterns:
    - "Meta-package NuGet : IncludeBuildOutput=false + IsPackable=true + 7 PackageReference $(Version)"
    - "NU5128 supprimé via NoWarn — attendu pour tout package metadata-only"

key-files:
  created:
    - src/SDK.Bundle/SDK.Bundle.csproj
    - tests/SDK.Plugins.Turbo.Tests/SDK.Plugins.Turbo.Tests.csproj
    - tests/SDK.Plugins.Turbo.Tests/TurboPluginTests.cs
  modified:
    - PokemonSDK.slnx
    - .github/workflows/publish-nuget.yml

key-decisions:
  - "IsPackable=true requis explicitement — IncludeBuildOutput=false seul ne déclenche pas dotnet pack"
  - "$(Version) dans les 7 PackageReference — cohérence versioning garantie (D-18)"

patterns-established:
  - "Meta-package pattern : SDK.Bundle comme référence pour tout futur package agrégateur"

duration: ~30min
started: 2026-06-07T17:49:00Z
completed: 2026-06-07T17:58:00Z
---

# Phase 9 Plan 01: SDK.Bundle Meta-Package Summary

**Meta-package `PokeForge.SDK` créé : agrège les 7 packages PokeForge.SDK.* — `dotnet add package PokeForge.SDK` suffit. 161/161 tests verts.**

## Performance

| Métrique | Valeur |
|---------|--------|
| Durée | ~30 min |
| Démarré | 2026-06-07T17:49Z |
| Terminé | 2026-06-07T17:58Z |
| Tâches | 2 complètes |
| Fichiers modifiés | 5 |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1 : Meta-package packable | Pass | `PokeForge.SDK.0.1.0.nupkg` — 7 deps dans nuspec (vérifié via unzip) |
| AC-2 : Solution compile avec SDK.Bundle | Pass | `dotnet build` 0 erreur, 0 warning |
| AC-3 : publish-nuget.yml pack le 8e package | Pass | Step renommé, `dotnet pack PokemonSDK.slnx` inclut SDK.Bundle via slnx |

## Accomplissements

- `PokeForge.SDK` meta-package créé — `dotnet add package PokeForge.SDK` installe les 7 sous-packages en une commande (D-18 respecté via `$(Version)`)
- Dette Phase 5 comblée : `SDK.Plugins.Turbo.Tests` créé (5 tests), total 156 → 161 tests
- CI publish-nuget.yml prêt à publier 8 packages au prochain tag `v*.*.*`

## Task Commits

| Tâche | Commit | Type | Description |
|-------|--------|------|-------------|
| Task 1+2 : SDK.Bundle + slnx + CI | `1685497` | feat | SDK.Bundle meta-package PokeForge.SDK (plan 09-01) |
| Hors-plan : Turbo.Tests | `c14b600` | test | ajout SDK.Plugins.Turbo.Tests manquant (5 tests) |

## Fichiers Créés/Modifiés

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `src/SDK.Bundle/SDK.Bundle.csproj` | Créé | Meta-package PokeForge.SDK — 7 PackageReference $(Version), IncludeBuildOutput=false |
| `PokemonSDK.slnx` | Modifié | SDK.Bundle ajouté dans `/src/` ; SDK.Plugins.Turbo.Tests dans `/tests/` |
| `.github/workflows/publish-nuget.yml` | Modifié | Step name : "Pack 7 packages..." → "Pack packages..." |
| `tests/SDK.Plugins.Turbo.Tests/SDK.Plugins.Turbo.Tests.csproj` | Créé | Tests dédiés TurboPlugin |
| `tests/SDK.Plugins.Turbo.Tests/TurboPluginTests.cs` | Créé | 5 tests : defaults, disabled, multiplier, Name, lifecycle hooks |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| `IsPackable=true` ajouté (hors plan) | `IncludeBuildOutput=false` seul ne déclenche pas `dotnet pack` sur .NET 10 | Déviation nécessaire, AC-1 impossible sans ça |
| SDK.Plugins.Turbo.Tests créé (hors plan) | Turbo n'avait aucun test depuis Phase 5 ; dette détectée pendant vérification | Robustesse +5 tests, 0 régression |

## Déviations du Plan

### Résumé

| Type | Nombre | Impact |
|------|--------|--------|
| Auto-fixé | 1 | `IsPackable=true` — essentiel, non documenté dans le plan |
| Ajout de portée | 1 | `SDK.Plugins.Turbo.Tests` — dette Phase 5 comblée opportunément |
| Différé | 0 | — |

**Impact total :** Corrections essentielles, ajout de valeur net.

### Auto-fixé

**1. NuGet `IsPackable=true` manquant**
- **Trouvé pendant :** Task 1 (vérification AC-1)
- **Problème :** `dotnet pack` ne génère pas de `.nupkg` sans `IsPackable=true` explicite, même avec `IncludeBuildOutput=false`
- **Fix :** Ajout `<IsPackable>true</IsPackable>` dans `SDK.Bundle.csproj`
- **Vérification :** `PokeForge.SDK.0.1.0.nupkg` créé avec 7 deps dans nuspec
- **Commit :** `1685497`

### Ajout de portée

**1. SDK.Plugins.Turbo.Tests (dette Phase 5)**
- **Trouvé pendant :** Vérification test count post-Task 2
- **Problème :** TurboPlugin existait depuis Phase 5 sans aucun test
- **Action :** Créé `tests/SDK.Plugins.Turbo.Tests/` — csproj + 5 tests (defaults, disabled, multiplier, Name, hooks no-throw)
- **Impact :** 156 → 161 tests ; couverture complète des 3 plugins

## Problèmes Rencontrés

| Problème | Résolution |
|---------|-----------|
| `BattleAction(move, pokemon)` → CS1503 | Signature correcte : `BattleAction(int MoveId, bool IsPlayer)` |
| `move.Id` → CS1061 | Propriété correcte : `move.MoveId` |
| `dotnet pack` sans `.nupkg` | `IsPackable=true` requis (voir déviations) |

## Readiness Phase 9 — Suite

**Prêt :**
- `PokeForge.SDK` meta-package disponible localement + publié au prochain tag
- Solution compile 0 erreur, 161/161 tests verts
- Base stable pour Plan 09-02 (sample project consommateur NuGet)

**Concerns :**
- Le sample project (09-02) consomme via NuGet — nécessite NuGet local feed ou publication d'abord (D-19)
- Signature BattleAction/BattleMove non-intuitive (MoveId vs Id) — doc Plan 11

**Bloqueurs :** Aucun

---
*Phase: 09-sample-project, Plan: 01*
*Terminé: 2026-06-07*
