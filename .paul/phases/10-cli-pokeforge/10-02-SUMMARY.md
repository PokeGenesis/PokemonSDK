---
phase: 10-cli-pokeforge
plan: 02
subsystem: cli
tags: [dotnet-global-tool, system-commandline, ef-core, sqlite, sprite-pipeline, xunit]

requires:
  - phase: 10-01
    provides: SDK.Cli project, NewCommand, pokeforge new scaffold
  - phase: 07
    provides: SDK.Tools pipeline (SpriteScanner, SpriteValidator, AtlasPacker, SqliteSyncer)
  - phase: 01
    provides: SDK.Data, PokemonDbContext, DataSeeder, EF Core migrations

provides:
  - pokeforge asset-sync command (validate→pack→sync sprite pipeline via SDK.Tools)
  - pokeforge seed command (migrate + SeedAll via SDK.Data)
  - SDK.Cli → SDK.Tools ProjectReference (transitive EF Core + ImageSharp)
  - 3 nouveaux tests (total suite : 10/10)

affects: [10-03, phase-11-docs]

tech-stack:
  added: [SDK.Tools ProjectReference dans SDK.Cli]
  patterns:
    - "SpriteScanner→SpriteValidator→AtlasPacker→SqliteSyncer pipeline exposé via CLI --config"
    - "DbContextOptionsBuilder inline dans Execute() sans DI (CLI one-shot pattern)"
    - "Environment.Exit(Execute(...)) dans SetHandler pour exit codes propres"
    - "xunit.runner.json parallelizeTestCollections=false pour IDisposable+CWD test classes"

key-files:
  created:
    - src/SDK.Cli/Commands/AssetSyncCommand.cs
    - src/SDK.Cli/Commands/SeedCommand.cs
    - tests/SDK.Cli.Tests/AssetSyncCommandTests.cs
    - tests/SDK.Cli.Tests/SeedCommandTests.cs
    - tests/SDK.Cli.Tests/xunit.runner.json
  modified:
    - src/SDK.Cli/SDK.Cli.csproj
    - src/SDK.Cli/Program.cs
    - tests/SDK.Cli.Tests/SDK.Cli.Tests.csproj

key-decisions:
  - "ImportConfig utilise [JsonPropertyName] snake_case strict — pas de PropertyNameCaseInsensitive"
  - "SDK.Tools Exe ProjectReference : types publics accessibles, <Program>$ internal seul exclu"
  - "xunit.runner.json requis dès que 2+ IDisposable classes appellent Directory.SetCurrentDirectory"

patterns-established:
  - "CLI Execute() inline sans DI : DbContextOptionsBuilder construit directement — adapté aux commandes one-shot"
  - "JSON import.json : toujours snake_case keys (sprites_root, output_dir, db_path, include_views)"

duration: ~45min
started: 2026-06-09T18:45:00Z
completed: 2026-06-09T19:32:00Z
---

# Phase 10 Plan 02: pokeforge asset-sync + seed Summary

**`pokeforge asset-sync` et `pokeforge seed` implémentés dans SDK.Cli via ProjectReference SDK.Tools — pipeline validate→pack→sync + migrate+seed avec 10/10 tests verts.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~45 min |
| Démarré | 2026-06-09T18:45:00Z |
| Complété | 2026-06-09T19:32:00Z |
| Tâches | 3/3 complètes |
| Fichiers modifiés | 8 (5 créés, 3 modifiés) |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1 : asset-sync config manquante → exit 1 | Pass | `Execute_MissingConfig_ReturnsOne` ✓ |
| AC-2 : asset-sync config valide → exit 0 | Pass | `Execute_ValidConfigEmptySprites_ReturnsZero` ✓ |
| AC-3 : seed crée DB et retourne 0 | Pass | `Execute_CreatesDbAndReturnsZero` ✓ |
| AC-4 : seed --db chemin custom | Pass | Option registrée, default + override fonctionnels |
| AC-5 : build + 10/10 tests verts | Pass | 0 erreurs, 0 warnings, Passed: 10, Failed: 0 |

## Accomplissements

- `pokeforge asset-sync --config <path>` expose le pipeline complet SDK.Tools sans exposer SDK.Tools directement aux makers
- `pokeforge seed --db <path>` applique `ctx.Database.Migrate()` + `DataSeeder.SeedAll(ctx)` avec exit code 0 (D-03 respecté)
- ProjectReference vers SDK.Tools (OutputType=Exe) établit le pattern : types namespace-publics accessibles, EF Core + ImageSharp transitifs
- D-17 respecté : SDK.Cli.csproj sans aucune référence SDK.MonoGame

## Task Commits

Phase 10 non encore commitée (untracked) — commit `feat(phase10)` attendu en fin de phase.

| Tâche | Statut | Fichiers |
|-------|--------|---------|
| Task 1: SDK.Tools ProjectRef + AssetSyncCommand | ✓ | SDK.Cli.csproj, AssetSyncCommand.cs, Program.cs |
| Task 2: SeedCommand | ✓ | SeedCommand.cs, Program.cs |
| Task 3: Tests AssetSync + Seed | ✓ | AssetSyncCommandTests.cs, SeedCommandTests.cs, xunit.runner.json, SDK.Cli.Tests.csproj |

## Fichiers Créés/Modifiés

| Fichier | Changement | Objectif |
|---------|-----------|----------|
| `src/SDK.Cli/SDK.Cli.csproj` | Modifié | ProjectReference SDK.Tools ajoutée |
| `src/SDK.Cli/Program.cs` | Modifié | AssetSyncCommand + SeedCommand enregistrés |
| `src/SDK.Cli/Commands/AssetSyncCommand.cs` | Créé | Pipeline validate→pack→sync via SDK.Tools |
| `src/SDK.Cli/Commands/SeedCommand.cs` | Créé | Migrate + SeedAll via SDK.Data |
| `tests/SDK.Cli.Tests/AssetSyncCommandTests.cs` | Créé | 2 tests AC-1 + AC-2 |
| `tests/SDK.Cli.Tests/SeedCommandTests.cs` | Créé | 1 test AC-3 |
| `tests/SDK.Cli.Tests/xunit.runner.json` | Créé | Disable parallel (fix CWD race IDisposable) |
| `tests/SDK.Cli.Tests/SDK.Cli.Tests.csproj` | Modifié | Content item xunit.runner.json PreserveNewest |

## Décisions

| Décision | Rationale | Impact |
|----------|-----------|--------|
| JSON snake_case strict, sans `PropertyNameCaseInsensitive` | `ImportConfig` a des `[JsonPropertyName("sprites_root")]` explicites — case-insensitive compare `spritesroot` vs `sprites_root`, underscore diffère | Tests 10-02 fixture corrigée en snake_case |
| xunit.runner.json `parallelizeTestCollections: false` | 3 classes IDisposable appellent `Directory.SetCurrentDirectory()` — xUnit parallélise par défaut → IO race condition | Toute future classe IDisposable+CWD dans ce projet est couverte |
| `DbContextOptionsBuilder` inline dans `SeedCommand.Execute()` | CLI = one-shot, pas de conteneur DI en scope — identique au pattern SDK.Tools/Program.cs | Pattern documenté pour plan 10-03 (doctor command) |

## Déviations du Plan

### Résumé

| Type | Compte | Impact |
|------|--------|--------|
| Auto-fixés | 2 | Correctifs essentiels |
| Ajouts de scope | 1 | Nécessaire (test isolation) |
| Différés | 0 | — |

### Auto-fixés

**1. Clés JSON camelCase → snake_case dans le fixture de test**
- **Découvert lors de :** Task 3 (AssetSyncCommandTests)
- **Problème :** Fixture utilisait `"spritesRoot"` (camelCase), `ImportConfig` attend `"sprites_root"` (snake_case via `[JsonPropertyName]`). Désérialisation silencieuse → `SpritesRoot = "assets/sprites"` (default) → DirectoryNotFoundException → return 1
- **Fix :** Fixture corrigée en snake_case (`sprites_root`, `output_dir`, `db_path`, `include_views`)
- **Fichiers :** `tests/SDK.Cli.Tests/AssetSyncCommandTests.cs`

**2. Suppression de `PropertyNameCaseInsensitive = true` dans AssetSyncCommand**
- **Découvert lors de :** Task 3 debugging
- **Problème :** Workaround masquait le vrai problème (underscore non case-foldable)
- **Fix :** Supprimé — comportement aligné avec SDK.Tools/Program.cs
- **Fichiers :** `src/SDK.Cli/Commands/AssetSyncCommand.cs`

### Ajouts de Scope

**xunit.runner.json + Content item csproj**
- **Découvert lors de :** Task 3 (Execute_NewProject_BinariesUnchanged flaky en suite complète)
- **Cause :** xUnit parallélise les test collections par défaut, plusieurs classes IDisposable+CWD interfèrent
- **Ajout :** `tests/SDK.Cli.Tests/xunit.runner.json` + `<Content>` item dans csproj
- **Impact :** Résout la flakyness sans modifier NewCommandTests.cs (boundary 10-01 respectée)

## Readiness Plan 10-03

**Prêt :**
- SDK.Cli architecture complète : new + asset-sync + seed
- Pattern Execute() one-shot documenté (inline DbContext, no DI)
- xunit.runner.json en place pour futurs tests IDisposable+CWD

**À noter pour 10-03 :**
- `pokeforge doctor` doit être headless (D-17) — vérifier SDK.Core + SDK.Data + SDK.Tools sans MonoGame
- Publication NuGet tool : `dotnet pack` + `dotnet tool install` → tester install globale
- CI `publish-cli.yml` : secret NUGET_API_KEY guard (pattern E-03 établi en Plan 08-02)

**Blockers :**
- Aucun

---
*Phase: 10-cli-pokeforge, Plan: 02*
*Complété: 2026-06-09*
