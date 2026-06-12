---
phase: 06-advanced-systems
plan: 02
subsystem: tools
tags: [imagesharp, fakemons, sqlite, efcore, pipeline, assembly]

requires:
  - phase: 06-01
    provides: FakemonSpecies entity, EF migration, FakemonSpeciesConfiguration FK constraints

provides:
  - FakemonPartsCatalog — découverte PNG + sidecar JSON metadata
  - FakemonFilter — filtrage par critères (type, gen, egg-group)
  - FakemonAssembler — alpha-composite N couches via ImageSharp ZOrder
  - FakemonExporter — PNG write + fakemon_species insert + 6 D-22 translations
  - FakemonAssemblyPipeline — orchestration catalog→filter→assemble→export
  - FakemonAssemblyOptions (record) + FakemonAssemblyException

affects:
  - 06-03 (CLI fakemon assemble/list-parts — dépend directement de ce pipeline)

tech-stack:
  added: [Microsoft.Data.Sqlite 10.0.* (SDK.Tools.Tests only)]
  patterns:
    - FakemonPartsCatalog static factory Scan() + GetMetadata() sidecar dictionary
    - SqliteConnection("DataSource=:memory:") inline dans tests (pas de fixture partagée)
    - EnsureCreated() + seed FK parent avant insert enfant
    - Pipeline RunAsync retourne string.Empty si 0 parties + non-strict

key-files:
  created:
    - src/SDK.Tools/Fakemons/Models/FakemonAssemblyOptions.cs
    - src/SDK.Tools/Fakemons/Models/FakemonAssemblyException.cs
    - src/SDK.Tools/Fakemons/FakemonPartLayer.cs
    - src/SDK.Tools/Fakemons/FakemonPartsCatalog.cs
    - src/SDK.Tools/Fakemons/FakemonFilter.cs
    - src/SDK.Tools/Fakemons/FakemonAssembler.cs
    - src/SDK.Tools/Fakemons/FakemonExporter.cs
    - src/SDK.Tools/Fakemons/FakemonAssemblyPipeline.cs
    - tests/SDK.Tools.Tests/Fakemons/FakemonAssemblerTests.cs
    - tests/SDK.Tools.Tests/Fakemons/FakemonFilterTests.cs
    - tests/SDK.Tools.Tests/Fakemons/FakemonExporterTests.cs
  modified:
    - tests/SDK.Tools.Tests/SDK.Tools.Tests.csproj

key-decisions:
  - "FakemonExporter.ExportAsync retourne Task<string> (outputPath) — pipeline coordonne via valeur de retour"
  - "Guard doublon AVANT write PNG — atomicité : pas de PNG orphelin si identifier déjà en DB"
  - "SqliteTestFixture partagée inaccessible depuis SDK.Tools.Tests — DbContext inline avec seed PokemonType"
  - "FakemonPartsCatalog : static factory + internal Dictionary<string,JsonDocument> — filtre sans re-parse"

patterns-established:
  - "Sidecar JSON : même basename que PNG, extension .json — tous champs optionnels"
  - "Parties sans sidecar = toujours incluses (compatible tout filtre)"
  - "ZOrder tri LINQ OrderBy (stable) — ordre d'apparition pour égalités"

duration: ~20min
started: 2026-06-12T17:24:00Z
completed: 2026-06-12T17:41:00Z
---

# Phase 6 Plan 02 : FakemonAssemblyPipeline Summary

**Pipeline PNG assembly complet : catalog (sidecar JSON) → filtre critères → ImageSharp alpha-composite N couches → export PNG + fakemon_species SQLite + 6 translations D-22.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~20 min |
| Démarré | 2026-06-12T17:24:00Z |
| Complété | 2026-06-12T17:41:00Z |
| Tâches | 2/2 complétées |
| Fichiers | 12 créés/modifiés |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1 : FakemonAssembler alpha-composite N couches ZOrder | **PASS** | 5 tests : single size, 3 couches, ZOrder sort, path manquant, liste vide |
| AC-2 : Pipeline erreur partie manquante | **PASS** | FakemonAssemblyException avec path dans message |
| AC-3 : FakemonFilter critères + no-sidecar inclus | **PASS** | 4 tests : null filter, type:fire+nosidecar, no-match, ZOrder sidecar |
| AC-4 : FakemonExporter PNG + fakemon_species + D-22 | **PASS** | 3 tests : PNG+insert, 6 translations, doublon throws |

## Accomplissements

- 8 fichiers source `SDK.Tools/Fakemons/` créés — pipeline entièrement fonctionnel
- 12 tests verts (5 Assembler + 4 Filter + 3 Exporter) — dépasse le minimum planifié de ≥6
- 40/40 SDK.Tools.Tests verts, 0 régression solution (11 assemblies)
- D-22 (6 locales) respecté dans FakemonExporter — fallback `opts.Identifier` si pas de translations JSON

## Files Created/Modified

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `src/SDK.Tools/Fakemons/Models/FakemonAssemblyOptions.cs` | Créé | Record options pipeline (PartsDirectory, Identifier, Filter, Strict…) |
| `src/SDK.Tools/Fakemons/Models/FakemonAssemblyException.cs` | Créé | Exception spécifique pipeline (missing part, doublon, 0 couches) |
| `src/SDK.Tools/Fakemons/FakemonPartLayer.cs` | Créé | Record (Path, ZOrder, BlendMode?) par couche PNG |
| `src/SDK.Tools/Fakemons/FakemonPartsCatalog.cs` | Créé | Scan() static factory — énumère PNG, parse sidecars JSON, expose Layers + GetMetadata() |
| `src/SDK.Tools/Fakemons/FakemonFilter.cs` | Créé | Apply() static — filtre par expression "key:value,…"; no-sidecar = always-include |
| `src/SDK.Tools/Fakemons/FakemonAssembler.cs` | Créé | Assemble() — tri ZOrder, Image.Load<Rgba32>, DrawImage alpha-composite |
| `src/SDK.Tools/Fakemons/FakemonExporter.cs` | Créé | ExportAsync() — guard doublon, SaveAsPngAsync, insert FakemonSpecies + 6 translations |
| `src/SDK.Tools/Fakemons/FakemonAssemblyPipeline.cs` | Créé | RunAsync() — orchestre catalog→filter→assemble→export |
| `tests/SDK.Tools.Tests/Fakemons/FakemonAssemblerTests.cs` | Créé | 5 tests Assembler (IDisposable temp dir, CreatePng helper) |
| `tests/SDK.Tools.Tests/Fakemons/FakemonFilterTests.cs` | Créé | 4 tests Filter (WriteSidecar helper, ZOrder parse) |
| `tests/SDK.Tools.Tests/Fakemons/FakemonExporterTests.cs` | Créé | 3 tests async Exporter (SQLite :memory:, seed PokemonType FK) |
| `tests/SDK.Tools.Tests/SDK.Tools.Tests.csproj` | Modifié | Ajout `Microsoft.Data.Sqlite 10.0.*` |

## Décisions prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| `FakemonExporter.ExportAsync` retourne `Task<string>` | Pipeline doit connaître le path du PNG créé pour le retourner | 06-03 CLI peut afficher/utiliser le path sorti |
| Guard doublon AVANT SaveAsPngAsync | Évite PNG orphelin si doublon DB | Atomicité correcte — pas de nettoyage à faire |
| `DbContext` inline dans FakemonExporterTests | SqliteTestFixture non accessible depuis SDK.Tools.Tests | Pattern réutilisable pour futurs tests Tools nécessitant DB |
| Seed `PokemonType {Id=15}` dans test setup | FK constraint `Type1Id → pokemon_types` sur DB vide | Sans seed : SQLiteException Error 19 FOREIGN KEY |

## Déviations du plan

### Résumé

| Type | Nombre | Impact |
|------|--------|--------|
| Auto-fixé | 2 | Corrections nécessaires, pas de scope creep |
| Ajouts scope | 1 | FakemonExporterTests (fichier extra) — requis par AC-4 |
| Déférés | 0 | — |

**Impact total :** Corrections essentielles + 1 fichier test supplémentaire pour couvrir AC-4.

### Auto-fixés

**1. ImageSharp.Drawing — Fill non disponible**
- **Découvert lors de :** Task 2 (FakemonAssemblerTests)
- **Problème :** Helper `CreatePng` avait paramètre `fill` utilisant `ctx.Fill()` — nécessite `SixLabors.ImageSharp.Drawing` non référencé
- **Fix :** Paramètre `fill` supprimé (jamais utilisé dans les tests)
- **Fichiers :** `FakemonAssemblerTests.cs`
- **Vérification :** Build vert après suppression

**2. FK constraint SQLite — PokemonType absent**
- **Découvert lors de :** Task 2 (FakemonExporterTests)
- **Problème :** `Type1Id: 15` → FK vers `pokemon_types.Id=15` absent en DB vide → SQLiteException Error 19
- **Fix :** Seed `PokemonType { Id=15, Identifier="dragon", Generation=1 }` dans constructeur du test
- **Fichiers :** `FakemonExporterTests.cs`
- **Vérification :** 3/3 tests Exporter verts

### Ajout de scope

**FakemonExporterTests.cs** — non listé dans `files_modified` du plan mais requis pour AC-4
- Plan notait : "créer DbContext in-memory dans le test directement" (implicitement inclus dans Task 2)
- Microsoft.Data.Sqlite ajouté à SDK.Tools.Tests.csproj pour supporter ce test

## Résultats vérification

```
dotnet test PokemonSDK.slnx --configuration Release
  SDK.Tools.Tests         : 40/40 ✅ (dont 12 tests Fakemons)
  SDK.Core.Tests          : ✅
  SDK.Data.Tests          : ✅
  SDK.Battle.Tests        : ✅
  SDK.Scripting.Tests     : ✅
  SDK.Plugins.Nuzlocke.Tests : ✅
  SDK.Plugins.Randomizer.Tests : ✅
  SDK.Plugins.Turbo.Tests : ✅
  SDK.Plugins.TTS.Tests   : ✅ (20 tests)
  SDK.MonoGame.Tests      : ✅
  SDK.Cli.Tests           : ✅
  Total : 0 failure, 0 régression
```

## Next Phase Readiness

**Prêt :**
- `FakemonAssemblyPipeline.RunAsync()` accessible depuis CLI (06-03)
- `FakemonAssemblyOptions` record — paramètres complets pour la commande `pokeforge fakemon assemble`
- `FakemonPartsCatalog.Scan()` — `pokeforge fakemon list-parts` peut l'appeler directement
- Pattern test SQLite inline établi pour SDK.Tools.Tests

**Concerns :**
- Pipeline génère seulement la vue "front" — vues back/icon/overworld déléguées au game dev (scope limit confirmé)
- `FakemonExporter` ne valide pas le format du nom d'identifier (caractères spéciaux) — validation CLI déléguée à 06-03

**Blockers :** Aucun

---
*Phase: 06-advanced-systems, Plan: 02*
*Complété: 2026-06-12*
