---
phase: 01-sdk-core-data
plan: 03
subsystem: database
tags: [ef-core, sqlite, seeding, extensions, cli]

requires:
  - phase: 01-sdk-core-data plan 01-02
    provides: PokemonDbContext, migrations, SqliteTestFixture

provides:
  - DbContextExtensions (GetSpeciesByGeneration, GetTypesByGeneration, GetTranslations, GetTranslation)
  - DataSeeder idempotent — 18 types + traductions fr/en
  - SDK.Tools CLI — dotnet run -- seed <path>

affects: [01-04, 02-02, battle-engine, scripting]

tech-stack:
  added: [SDK.Tools console app (net10.0)]
  patterns:
    - DbContext extension methods (IQueryable composable)
    - Idempotent seeder with Any() guard
    - CLI console app sans MonoGame (D-17)

key-files:
  created:
    - src/SDK.Data/Extensions/DbContextExtensions.cs
    - src/SDK.Data/Seeding/DataSeeder.cs
    - src/SDK.Tools/SDK.Tools.csproj
    - src/SDK.Tools/Program.cs
    - tests/SDK.Data.Tests/DbContextExtensionsTests.cs
    - tests/SDK.Data.Tests/DataSeederTests.cs
  modified:
    - PokemonSDK.slnx

key-decisions:
  - "GetSpeciesByGeneration/GetTypesByGeneration utilisent <= maxGeneration (pas == introduced_in)"
  - "TypeEffectiveness seeding différé à 02-02 (18×18=324 rows)"
  - "DataSeederTests : fixture locale par test (SeedAll mute l'état — pas IClassFixture)"
  - "SDK.Tools zéro package direct NuGet — EF Core transitif via SDK.Data (D-17)"

patterns-established:
  - "DbContextExtensions dans SDK.Data.Extensions — IQueryable composable côté serveur"
  - "DataSeeder.SeedAll() guard Any() avant insert"
  - "SqliteTestFixture locale (new) pour tests qui mutent — IClassFixture pour tests read-only"

duration: ~60min
started: 2026-06-02T18:30:00Z
completed: 2026-06-02T20:43:00Z
---

# Phase 1 Plan 03: DbContextExtensions + DataSeeder + SDK.Tools Summary

**DbContextExtensions (filtre génération + lookup traductions), DataSeeder idempotent 18 types fr/en, et CLI `dotnet run -- seed <path>` ajoutés à SDK.Data et SDK.Tools.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~60 min |
| Démarré | 2026-06-02T18:30Z |
| Complété | 2026-06-02T20:43Z |
| Tâches | 3/3 complétées |
| Fichiers créés | 6 |
| Fichiers modifiés | 1 |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: GetSpeciesByGeneration filtre gen <= N | Pass | Test vert : bulbasaur + togepi inclus, ralts exclu |
| AC-2: GetTranslation retourne valeur correcte | Pass | "Plante" retourné pour locale=fr, field=name |
| AC-3: DataSeeder.SeedAll 18 types idempotent | Pass | Count=18, 2ème appel sans exception ni doublon |
| AC-4: SDK.Tools seed CLI exit 0 | Pass | "Seed complete: 18 types in /tmp/test-seed-01-03.db", exit 0 |

## Accomplishments

- `DbContextExtensions` : 4 méthodes d'extension IQueryable composables (GetSpeciesByGeneration, GetTypesByGeneration, GetTranslations, GetTranslation)
- `DataSeeder` : 18 PokemonType (15 gen1 + dark/steel gen2 + fairy gen6) + 36 traductions (fr + en × 18) — pleinement idempotent
- `SDK.Tools` console app net10.0 sans MonoGame — commande `seed <path>` applique migrations puis seeds
- 8/8 tests verts dans SDK.Data.Tests ; CoreDependencyTests non régressé (1/1)
- D-01 (SDK.Core zéro NuGet), D-17 (SDK.Tools zéro MonoGame), D-07 (traductions centrales) respectés

## Fichiers Créés / Modifiés

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `src/SDK.Data/Extensions/DbContextExtensions.cs` | Créé | 4 méthodes d'extension sur PokemonDbContext |
| `src/SDK.Data/Seeding/DataSeeder.cs` | Créé | Seeder idempotent 18 types + traductions fr/en |
| `src/SDK.Tools/SDK.Tools.csproj` | Créé | Console app net10.0, SDK.Core + SDK.Data uniquement |
| `src/SDK.Tools/Program.cs` | Créé | CLI seed : migrate + SeedAll + exit 0 |
| `tests/SDK.Data.Tests/DbContextExtensionsTests.cs` | Créé | 3 tests GetSpeciesByGeneration, GetTypesByGeneration, GetTranslation |
| `tests/SDK.Data.Tests/DataSeederTests.cs` | Créé | 2 tests : SeedAll_Creates18Types, SeedAll_IsIdempotent |
| `PokemonSDK.slnx` | Modifié | Ajout SDK.Tools dans le dossier /src/ |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| `<= maxGeneration` dans les filtres | Sémantique "disponible jusqu'à la gen N", pas "introduit en gen N" | Correct pour filtrer les types (ex: Fairy gen6 exclus avant gen6) |
| TypeEffectiveness différé à 02-02 | 18×18=324 rows — trop lourd pour ce plan; nécessite contexte battle | BattleDataSeeder en 02-02 |
| PokemonSpecies différé à 01-04 | End-to-end test a besoin de données complètes dans un contexte intégré | 01-04 seed species |
| Fixture locale dans DataSeederTests | SeedAll mute l'état DB — IClassFixture partagée provoquerait pollution inter-tests | Pattern documenté pour tests stateful |

## Déviations du Plan

Aucune. Plan exécuté exactement comme spécifié.

## Issues Rencontrées

Aucune.

## Readiness pour Plan 01-04

**Prêt :**
- DB interrogeable via DbContextExtensions (filtre génération)
- 18 types + traductions fr/en disponibles en DB après seed
- CLI `dotnet run -- seed` fonctionnel et idempotent
- SDK.Tools établi comme point d'entrée CLI headless (D-17)

**Déférés à surveiller :**
- TypeEffectiveness seeding → Plan 02-02
- PokemonSpecies seeding → Plan 01-04 (scope du prochain plan)

**Blockers :**
Aucun.

---
*Phase: 01-sdk-core-data, Plan: 03*
*Complété: 2026-06-02*
