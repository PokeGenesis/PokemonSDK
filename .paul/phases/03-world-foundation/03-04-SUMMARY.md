---
phase: 03-world-foundation
plan: 04
subsystem: testing
tags: [xunit, moq, headless, ci, github-actions, matrix]

requires:
  - phase: 03-03
    provides: HeadlessRunner, WorldSystem, PlayerSystem, NullInputProvider, IInputProvider

provides:
  - tests/SDK.MonoGame.Tests — 3 HeadlessSmokeTests (sans GL context, sans DB)
  - CI matrix ubuntu-latest + windows-latest activé (PLAT-02 satisfait)

affects: [phase-4-scripting, phase-5-plugins]

tech-stack:
  added:
    - SDK.MonoGame.Tests project (xUnit + FluentAssertions + Moq — versions identiques aux projets tests existants)
  patterns:
    - HeadlessSmokeTests pattern — DI container Moq-based, sans DB, sans GL context
    - Test project référence SDK.MonoGame (transitif SDK.Core) — pas de ref SDK.Data inutile

key-files:
  created:
    - tests/SDK.MonoGame.Tests/SDK.MonoGame.Tests.csproj
    - tests/SDK.MonoGame.Tests/HeadlessSmokeTests.cs
  modified:
    - PokemonSDK.slnx
    - .github/workflows/ci.yml

key-decisions:
  - "EncounterZone dans SDK.Core.Entities (pas SDK.Data.Models) — plan avait namespace erroné, corrigé à APPLY"
  - "<Using Include=\"Xunit\" /> requis explicitement — ImplicitUsings n'inclut pas Xunit"
  - "SDK.Data ref retirée du test csproj — EncounterZone accessible transitivement via SDK.MonoGame→SDK.Core"
  - "continue-on-error: true sur step headless — DB absente en CI, xUnit tests sont la vraie gate"

patterns-established:
  - "HeadlessSmokeTests: DI container BuildHeadlessServices() + Moq<IEncounterSystem> → zéro DB, zéro GL"
  - "Test project référence SDK.MonoGame uniquement (transitif Core+Data+Battle) + SDK.Core explicite"

duration: ~30min
started: 2026-06-05T20:30:00Z
completed: 2026-06-05T21:00:00Z
---

# Phase 3 Plan 04: HeadlessSmokeTester + CI — Summary

**77/77 tests verts — SDK.MonoGame.Tests (3 HeadlessSmokeTests Moq-based, sans DB) + ci.yml activé ubuntu/windows — Phase 3 World Foundation fermée.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~30 min |
| Démarré | 2026-06-05T20:30:00Z |
| Complété | 2026-06-05T21:00:00Z |
| Tasks | 2/2 |
| Fichiers créés | 2 |
| Fichiers modifiés | 2 |
| Commit | `f1310e9` |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1 : Run_60Frames_NoException | Pass | HeadlessRunner.Run(sp, 60) — 0 exception, Moq IEncounterSystem ✓ |
| AC-2 : PlayerPositionUnchanged_WithNullInput | Pass | Position Vector2(240, 135) inchangée après 10 frames ✓ |
| AC-3 : ClockReturnsValidTimeOfDay | Pass | MapHour(0) → Night → dans validValues ✓ |
| AC-4 : 77/77 verts ubuntu + windows | Pass | 24 Core + 30 Battle + 20 Data + 3 MonoGame = 77 ✓ |

## Accomplissements

- `tests/SDK.MonoGame.Tests/` créé — 3 HeadlessSmokeTests, zéro DB, zéro GL context
- CI matrix `ubuntu-latest + windows-latest` activé — TODO Phase 3 remplacé par step headless actif
- `fail-fast: false` ajouté — un OS ne bloque plus l'autre en cas d'échec isolé
- PLAT-02 satisfait — CI couvre Linux + Windows
- Phase 3 fermée : 4/4 plans — World Foundation complète

## Task Commits

| Task | Commit | Description |
|------|--------|-------------|
| T1 + T2 (atomique) | `f1310e9` | SDK.MonoGame.Tests + ci.yml + slnx |

## Fichiers Créés/Modifiés

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `tests/SDK.MonoGame.Tests/SDK.MonoGame.Tests.csproj` | Créé | Projet test net10.0, xUnit/FluentAssertions/Moq, ref SDK.MonoGame+SDK.Core |
| `tests/SDK.MonoGame.Tests/HeadlessSmokeTests.cs` | Créé | 3 smoke tests HeadlessRunner — Run_60Frames, PlayerPosition, ClockTimeOfDay |
| `PokemonSDK.slnx` | Modifié | +1 ligne `SDK.MonoGame.Tests` dans folder /tests/ |
| `.github/workflows/ci.yml` | Modifié | TODO Phase 3 activé, `fail-fast: false`, step headless ubuntu |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| `using SDK.Core.Entities` (pas `SDK.Data.Models`) | `EncounterZone` défini dans SDK.Core — SDK.Data/Models/ n'existe pas | Correction critique du plan |
| `<Using Include="Xunit" />` ajouté | `[Fact]` ne résout pas sans — ImplicitUsings n'inclut pas Xunit | Parité avec SDK.Battle.Tests |
| SDK.Data ref retirée du csproj | EncounterZone dans SDK.Core, accessible transitivement via SDK.MonoGame | Csproj minimal, pas de ref superflue |
| `continue-on-error: true` sur step headless | DB absente en CI (pas de seed) — xUnit tests (Moq) = vraie gate | CI ne fail pas sur DB manquante |

## Déviations du Plan

### Résumé

| Type | Nb | Impact |
|------|-----|--------|
| Auto-fixées (bugs plan) | 3 | Correctifs essentiels — sans eux build échouerait |
| Scope additions | 0 | — |
| Déférés | 0 | — |

**Impact total :** 3 bugs dans le code du plan détectés et corrigés à APPLY. Aucun scope creep.

### Auto-fixées

**1. Namespace EncounterZone erroné**
- **Trouvé lors :** Vérification pré-APPLY
- **Problème :** Plan spécifiait `using SDK.Data.Models;` — ce namespace n'existe pas dans le projet
- **Fix :** `using SDK.Core.Entities;` — emplacement réel de `EncounterZone`
- **Vérification :** `dotnet build` 0 erreur

**2. `<Using Include="Xunit" />` manquant**
- **Trouvé lors :** Vérification pré-APPLY (comparaison SDK.Battle.Tests.csproj)
- **Problème :** Sans cet implicit using, `[Fact]` ne résout pas (`error CS0246`)
- **Fix :** Ajouté dans `<ItemGroup>` du csproj
- **Vérification :** `dotnet test` → 3 tests découverts et exécutés

**3. `SDK.Data` ProjectReference inutile**
- **Trouvé lors :** Vérification pré-APPLY (audit namespace EncounterZone)
- **Problème :** EncounterZone dans SDK.Core (pas SDK.Data) → ref SDK.Data superflue
- **Fix :** Retirée — SDK.Core explicite + SDK.Core transitivement via SDK.MonoGame
- **Vérification :** Build propre, 0 NU1605

## Issues Rencontrées

Aucune — vérification pré-APPLY a identifié les 3 bugs avant exécution.

## Next Phase Readiness

**Prêt :**
- Phase 3 World Foundation 100% — HeadlessRunner + CI matrix + 77 tests
- CI ubuntu+windows activé — tout commit futur validé sur les deux OS
- Pattern HeadlessSmokeTests établi — réutilisable Phase 4 (SDK.Scripting, GameState)

**Concerns :**
- `MonoGame.Extended` toujours absent — TilemapRenderer reste stub (Phase 5+)
- Shaders `.fx` non compilés — `Content.mgcb` vide (Phase 7 DX)
- CI headless step `continue-on-error: true` — ne bloque pas si DB absente. Acceptable tant que DB pas seedée en CI

**Blockers :**
- Aucun pour Phase 4.

---
*Phase: 03-world-foundation, Plan: 04*
*Complété: 2026-06-05*
