---
phase: 01-sdk-core-data
plan: 02
subsystem: database
tags: [dotnet, efcore, sqlite, migrations, fluent-api, xunit, test-fixture]

requires:
  - plan: "01-01"
    provides: "6 entités SDK.Core (PokemonSpecies, PokemonForm, PokemonBaseStats, Translation, PokemonType, TypeEffectiveness) + namespaces SDK.Core.Entities"

provides:
  - PokemonDbContext avec 6 DbSet + ApplyConfigurationsFromAssembly
  - 6 IEntityTypeConfiguration (Fluent API, D-07 UNIQUE sur translations, composite key TypeEffectiveness)
  - IDesignTimeDbContextFactory — migrations sans --startup-project
  - Migration 001 InitialCreate → src/SDK.Data/data/PokemonSDK.db (8 tables)
  - SDK.Data.Tests — SqliteTestFixture :memory: + 3 tests verts

affects:
  - 01-03 (DataSeeder + DbContextExtensions — consomme PokemonDbContext + Migrations)
  - 01-04 (end-to-end SDK test — consomme couche Data complète)
  - 02-02 (Migration 002 — s'appuie sur IDesignTimeDbContextFactory pattern établi)

tech-stack:
  added:
    - Microsoft.EntityFrameworkCore 10.0.8
    - Microsoft.EntityFrameworkCore.Sqlite 10.0.8
    - Microsoft.EntityFrameworkCore.Design 10.0.8 (PrivateAssets=all)
    - Microsoft.Data.Sqlite 10.0.8 (SDK.Data.Tests)
    - dotnet-ef global tool 10.0.8 (mis à jour depuis 8.0.16)
  patterns:
    - ApplyConfigurationsFromAssembly — auto-découverte des IEntityTypeConfiguration
    - IDesignTimeDbContextFactory — migrations sans startup project (D-03)
    - SqliteTestFixture avec connexion :memory: ouverte partagée — isolation test sans fichier
    - IClassFixture<SqliteTestFixture> — fixture partagée entre tests dans la classe

key-files:
  created:
    - src/SDK.Data/PokemonDbContext.cs
    - src/SDK.Data/Configurations/PokemonSpeciesConfiguration.cs
    - src/SDK.Data/Configurations/PokemonFormConfiguration.cs
    - src/SDK.Data/Configurations/PokemonBaseStatsConfiguration.cs
    - src/SDK.Data/Configurations/TranslationConfiguration.cs
    - src/SDK.Data/Configurations/PokemonTypeConfiguration.cs
    - src/SDK.Data/Configurations/TypeEffectivenessConfiguration.cs
    - src/SDK.Data/DesignTime/PokemonDbContextFactory.cs
    - src/SDK.Data/Migrations/20260602165634_InitialCreate.cs
    - src/SDK.Data/Migrations/PokemonDbContextModelSnapshot.cs
    - tests/SDK.Data.Tests/SqliteTestFixture.cs
    - tests/SDK.Data.Tests/PokemonDbContextTests.cs
  modified:
    - src/SDK.Data/SDK.Data.csproj (EF Core 10 packages)
    - PokemonSDK.slnx (SDK.Data.Tests ajouté)

key-decisions:
  - "EF Core 10 ajoute __EFMigrationsLock — 8 tables au total (pas 7 comme plan prévoyait)"
  - "DB créée dans src/SDK.Data/data/ via design-time factory (cwd = répertoire projet) — comportement attendu"
  - "Microsoft.EntityFrameworkCore.Design PrivateAssets auto-configuré par dotnet add package"

patterns-established:
  - "IDesignTimeDbContextFactory dans SDK.Data/DesignTime/ — toutes les futures migrations utilisent ce pattern"
  - "SqliteTestFixture :memory: avec connexion ouverte partagée — pattern test pour SDK.Data.Tests et futurs tests Data"
  - "6 IEntityTypeConfiguration dans SDK.Data/Configurations/ — une classe par entité, auto-découverte via ApplyConfigurationsFromAssembly"

duration: ~15min
started: 2026-06-02T16:50:00Z
completed: 2026-06-02T17:05:00Z
---

# Phase 1 Plan 02: EF Core 10 Data Layer Summary

**PokemonDbContext + 6 Fluent API configurations + IDesignTimeDbContextFactory + Migration 001 InitialCreate (SQLite 8 tables) + SqliteTestFixture :memory: avec 3 tests verts.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~15 min |
| Démarré | 2026-06-02T16:50Z |
| Complété | 2026-06-02T17:05Z |
| Tâches | 3/3 complétées |
| Fichiers créés | 12 |
| Fichiers modifiés | 2 |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1: PokemonDbContext compile avec 6 DbSet | **PASS** | `dotnet build src/SDK.Data` → 0 erreurs, 0 warnings |
| AC-2: Contrainte UNIQUE Translation dans migration | **PASS** | 3 index uniques dans migration (dont UNIQUE translations D-07) |
| AC-3: Migration InitialCreate appliquée sur SQLite | **PASS** | 8 tables créées (6 entités + __EFMigrationsHistory + __EFMigrationsLock) |
| AC-4: SqliteTestFixture isole 3 tests | **PASS** | `dotnet test tests/SDK.Data.Tests` → 3/3 Passed |

## Accomplissements

- PokemonDbContext avec 6 DbSets + ApplyConfigurationsFromAssembly — découverte automatique des configurations
- TranslationConfiguration avec UNIQUE(EntityType, EntityId, Locale, Field) — D-07 enforced en base et testé
- IDesignTimeDbContextFactory — pattern établi pour toutes les futures migrations (Plan 01-03+)
- SqliteTestFixture :memory: — pattern isolation test réutilisable pour SDK.Data.Tests
- CoreDependencyTests toujours vert — D-01 non régressé (SDK.Core zero-NuGet garanti)

## Files Created/Modified

| Fichier | Change | Usage |
|---------|--------|-------|
| `src/SDK.Data/PokemonDbContext.cs` | Créé | DbContext + 6 DbSet + ApplyConfigurationsFromAssembly |
| `src/SDK.Data/Configurations/PokemonSpeciesConfiguration.cs` | Créé | HasKey, Identifier UNIQUE, generation required |
| `src/SDK.Data/Configurations/PokemonFormConfiguration.cs` | Créé | HasKey, AssetKey required, generation required |
| `src/SDK.Data/Configurations/PokemonBaseStatsConfiguration.cs` | Créé | HasKey, 6 stats required |
| `src/SDK.Data/Configurations/TranslationConfiguration.cs` | Créé | D-07 — UNIQUE index (EntityType, EntityId, Locale, Field) |
| `src/SDK.Data/Configurations/PokemonTypeConfiguration.cs` | Créé | HasKey, Identifier UNIQUE, generation required |
| `src/SDK.Data/Configurations/TypeEffectivenessConfiguration.cs` | Créé | Composite key (AttackerTypeId, DefenderTypeId, Generation), DamageFactor précision |
| `src/SDK.Data/DesignTime/PokemonDbContextFactory.cs` | Créé | IDesignTimeDbContextFactory → data/PokemonSDK.db |
| `src/SDK.Data/Migrations/20260602165634_InitialCreate.cs` | Créé | Migration Up()/Down() — schéma complet |
| `src/SDK.Data/Migrations/PokemonDbContextModelSnapshot.cs` | Créé | Snapshot modèle EF Core |
| `tests/SDK.Data.Tests/SqliteTestFixture.cs` | Créé | Fixture :memory: avec CreateContext() |
| `tests/SDK.Data.Tests/PokemonDbContextTests.cs` | Créé | 3 tests : create/query/UNIQUE exception |
| `src/SDK.Data/SDK.Data.csproj` | Modifié | EF Core 10.0.8 packages ajoutés |
| `PokemonSDK.slnx` | Modifié | SDK.Data.Tests ajouté à la solution |

## Decisions Made

| Décision | Rationale | Impact |
|----------|-----------|--------|
| ApplyConfigurationsFromAssembly | Auto-découverte — pas besoin d'enregistrer chaque config manuellement | Pattern réutilisé pour toutes les futures migrations |
| IDesignTimeDbContextFactory dans SDK.Data | SDK.MonoGame n'existe pas encore (Phase 3) — migrations autonomes sans startup project | Plans 01-03, 02-02, 03-01, 04-02 utilisent le même pattern |
| SqliteTestFixture connexion partagée | EnsureCreated() une fois, CreateContext() crée de nouveaux DbContext sur même connexion | Isolation par contexte, pas par base — plus rapide |

## Deviations from Plan

| Type | Nb | Impact |
|------|----|--------|
| Auto-fixed | 0 | — |
| Scope additions | 0 | — |
| Différés | 0 | — |

### Déviations constatées (non bloquantes)

**1. 8 tables au lieu de 7**
- **Prévu :** 7 tables (6 entités + __EFMigrationsHistory)
- **Réel :** 8 tables (6 entités + __EFMigrationsHistory + __EFMigrationsLock)
- **Cause :** EF Core 10 ajoute __EFMigrationsLock pour le distributed migrations lock
- **Impact :** Aucun — toutes les 6 tables entités + __EFMigrationsHistory présentes

**2. DB à src/SDK.Data/data/ au lieu de data/**
- **Prévu :** `data/PokemonSDK.db` (relatif à la racine du repo)
- **Réel :** `src/SDK.Data/data/PokemonSDK.db` (cwd du projet via IDesignTimeDbContextFactory)
- **Cause :** `Directory.GetCurrentDirectory()` retourne le répertoire du projet quand `dotnet ef --project src/SDK.Data`
- **Impact :** Aucun — DB de développement uniquement. Runtime déterminé par l'application consommatrice.

## Issues Encountered

| Problème | Résolution |
|----------|------------|
| dotnet-ef 8.0.16 installé (pas 10.x) | Mis à jour vers 10.0.8 : `dotnet tool update --global dotnet-ef` |

## Next Phase Readiness

**Prêt :**
- PokemonDbContext opérationnel — Plan 01-03 peut implémenter GetByGeneration/GetTranslations
- Migration 001 appliquée — Plan 01-03 peut ajouter DataSeeder sans toucher au schéma
- IDesignTimeDbContextFactory pattern établi — réutilisable pour Plans 02-02, 03-01, 04-02
- SqliteTestFixture pattern établi — SDK.Data.Tests peut accueillir les tests Plan 01-03

**Concerns :**
- FluentAssertions v8 licence Xceed — OK non-commercial, à revoir avant Phase 8 NuGet

**Blockers :**
- Aucun

---
*Phase: 01-sdk-core-data, Plan: 02*
*Complété: 2026-06-02*
