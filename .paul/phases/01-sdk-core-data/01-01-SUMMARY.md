---
phase: 01-sdk-core-data
plan: 01
subsystem: core
tags: [dotnet, sdk-core, entities, ef-core-prep, xunit]

requires: []

provides:
  - PokemonSDK.slnx — solution 5 projets net10.0 compilant
  - SDK.Core domain models Wave 1 (6 entités + 3 enums + 1 interface + 1 VO)
  - CoreDependencyTests — garde automatique zero-NuGet sur SDK.Core
  - Smoke test MonoGame.DesktopGL 3.8.4.1 net10.0 confirmé

affects:
  - 01-02 (EF Core + PokemonDbContext — consomme les entités créées ici)
  - 02-01 (battle models — consomme BattleConfig, enums, IRepository)
  - 03-01 (world primitives — consomme SDK.Core namespace conventions)

tech-stack:
  added:
    - .NET 10 / net10.0
    - xUnit (dotnet new xunit template)
    - FluentAssertions 8.10.0 (non-commercial OK ; pin v7 si distribution commerciale)
    - coverlet.collector 10.0.1
  patterns:
    - SDK.Core zero-NuGet (D-01) — enforced par CoreDependencyTests
    - Table translations centrale (D-07) — pas de colonnes par langue sur entités
    - generation INT NOT NULL (D-09) — sur 4 entités (PokemonSpecies, PokemonForm, PokemonType, TypeEffectiveness)
    - BattleState record immuable (D-05) — BattleConfig sealed record
    - PokemonType nommé avec préfixe — évite conflit System.Type

key-files:
  created:
    - PokemonSDK.slnx
    - src/SDK.Core/Entities/PokemonSpecies.cs
    - src/SDK.Core/Entities/PokemonForm.cs
    - src/SDK.Core/Entities/PokemonBaseStats.cs
    - src/SDK.Core/Entities/Translation.cs
    - src/SDK.Core/Entities/PokemonType.cs
    - src/SDK.Core/Entities/TypeEffectiveness.cs
    - src/SDK.Core/Enums/MoveType.cs
    - src/SDK.Core/Enums/TimeOfDay.cs
    - src/SDK.Core/Enums/DifficultyMode.cs
    - src/SDK.Core/Interfaces/IRepository.cs
    - src/SDK.Core/ValueObjects/BattleConfig.cs
    - tests/SDK.Core.Tests/CoreDependencyTests.cs
  modified:
    - CLAUDE.md (build command .sln → .slnx)

key-decisions:
  - ".slnx format (not .sln) — .NET 10 dotnet new sln default changed; .slnx is cleaner (no GUIDs)"
  - "PokemonType entity name — prefixed to avoid System.Type conflict"
  - "FluentAssertions 8.10.0 — Xceed license OK non-commercial; pin v7 before Phase 8 NuGet"

patterns-established:
  - "Zero-NuGet on SDK.Core verified by CoreDependencyTests — run on every PR"
  - "CoreDependencyTests path traversal: 5× '..' from AppContext.BaseDirectory to project root"
  - "MonoGame smoke test runs in /tmp, never added to solution"

duration: ~30min
started: 2026-06-01T20:09:00Z
completed: 2026-06-01T20:19:00Z
---

# Phase 1 Plan 01: SDK.Core Wave 1 Summary

**PokemonSDK.slnx scaffoldé (.NET 10, 5 projets), 11 fichiers SDK.Core créés (6 entités + 3 enums + IRepository + BattleConfig), CoreDependencyTests vert, MonoGame 3.8.4.1 net10.0 confirmé.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~30 min |
| Démarré | 2026-06-01T20:09Z |
| Complété | 2026-06-01T20:19Z |
| Tâches | 3/3 complétées |
| Fichiers créés | 18 |
| Fichiers modifiés | 1 (CLAUDE.md) |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: Solution compile | **PASS** | `dotnet build PokemonSDK.slnx` → 0 erreurs, 0 warnings |
| AC-2: SDK.Core sans NuGet | **PASS** | `dotnet list SDK.Core.csproj package` → liste vide |
| AC-3: CoreDependencyTests | **PASS** | 1/1 test vert : `SdkCore_HasNoExternalNuGetPackages` |
| AC-4: Models compilent | **PASS** | `dotnet build SDK.Core` → 0 erreurs, 11 fichiers |
| AC-5: Smoke test MonoGame | **PASS** | MonoGame.Framework.DesktopGL 3.8.4.1 compile sur net10.0 |

## Accomplissements

- Solution `.slnx` 5 projets net10.0 compilant, dépendances correctement câblées
- 6 entités D-07/D-09 conformes : `generation` NOT NULL sur 4 entités, aucune colonne `name_fr`/`name_en`
- CoreDependencyTests — garde automatisée qui échouera si quelqu'un ajoute un NuGet à SDK.Core
- MonoGame 3.8.4.1 net10.0 confirmé compatible (risque Phase 3 résolu)

## Files Created/Modified

| Fichier | Change | Usage |
|---------|--------|-------|
| `PokemonSDK.slnx` | Créé | Solution .NET 10, 5 projets |
| `src/SDK.Core/Entities/PokemonSpecies.cs` | Créé | Espèce Pokémon, gen + types |
| `src/SDK.Core/Entities/PokemonForm.cs` | Créé | Forme (assetKey, isDefault, gen) |
| `src/SDK.Core/Entities/PokemonBaseStats.cs` | Créé | Stats HP/Atk/Def/SpA/SpD/Spe |
| `src/SDK.Core/Entities/Translation.cs` | Créé | Table traductions centrale (D-07) |
| `src/SDK.Core/Entities/PokemonType.cs` | Créé | Type (Normal, Feu…) avec gen |
| `src/SDK.Core/Entities/TypeEffectiveness.cs` | Créé | Tableau des types, DamageFactor decimal |
| `src/SDK.Core/Enums/MoveType.cs` | Créé | 18 types Pokémon |
| `src/SDK.Core/Enums/TimeOfDay.cs` | Créé | Morning/Day/Evening/Night |
| `src/SDK.Core/Enums/DifficultyMode.cs` | Créé | Story/Hard |
| `src/SDK.Core/Interfaces/IRepository.cs` | Créé | Generic CRUD async |
| `src/SDK.Core/ValueObjects/BattleConfig.cs` | Créé | sealed record, D-05/D-06 |
| `tests/SDK.Core.Tests/CoreDependencyTests.cs` | Créé | Garde zero-NuGet SDK.Core |
| `CLAUDE.md` | Modifié | Ligne 166 : `.sln` → `.slnx` |

## Deviations from Plan

| Type | Nb | Impact |
|------|----|--------|
| Auto-fixed | 1 | Essentiel — format solution |
| Scope additions | 0 | - |
| Différés | 1 | FluentAssertions v8 licence |

### Auto-fixed : Format solution .sln → .slnx

- **Trouvé pendant :** T1 (scaffold solution)
- **Problème :** `.NET 10` `dotnet new sln` crée `PokemonSDK.slnx` (format XML, sans GUID), pas `PokemonSDK.sln`
- **Fix :** Toutes les commandes mises à jour vers `PokemonSDK.slnx`. CLAUDE.md ligne 166 corrigée.
- **Fichiers :** CLAUDE.md, PLAN.md (docs internes)
- **Vérification :** `dotnet build PokemonSDK.slnx` → Build succeeded

### Différés : FluentAssertions v8 licence Xceed

- Découvert pendant T3. v8.x = licence Xceed (non-commercial gratuit). Envisager pin v7.x (Apache 2.0) avant Phase 8 NuGet distribution.

## Issues Encountered

| Problème | Résolution |
|----------|------------|
| `.NET 8` installé (pas `.NET 10`) | Bloquant — utilisateur a installé .NET 10.0.108. Reprise sans changement de plan. |
| `dotnet test` MSB1009 hors projet root | Préfixer `cd /home/subarnan/projects/PokemonSDK &&` après travail dans `/tmp` |

## Next Phase Readiness

**Prêt :**
- SDK.Core entités disponibles pour EF Core mapping (Plan 01-02)
- Namespace conventions établies (`SDK.Core.Entities`, `SDK.Core.Interfaces`, etc.)
- CoreDependencyTests fonctionnel — D-01 garanti automatiquement
- MonoGame 3.8.4.1 net10.0 confirmé (risque Phase 3 levé)

**Concerns :**
- FluentAssertions v8 licence — OK pour Phase 1-7, revoir avant Phase 8 NuGet
- Template `dotnet new mgdesktopgl` non testé — à valider avant Phase 3

**Blockers :**
- Aucun

---
*Phase: 01-sdk-core-data, Plan: 01*
*Complété: 2026-06-01*
