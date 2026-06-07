---
phase: 08-nuget-distribution
plan: 02
subsystem: security
tags: [cve, imagesharp, fluentassertions, license, nuget, security]

requires:
  - phase: 08-nuget-distribution
    plan: 01
    provides: 7 packages NuGet configurés — base pour audit licence/CVE

provides:
  - SixLabors.ImageSharp 4.0.0 sans CVE dans SDK.Tools
  - FluentAssertions 8.10.0 standardisé sur 8/8 projets tests
  - dotnet list package --vulnerable → 0 HIGH/MODERATE sur toute la solution
  - Deferred issues FluentAssertions + ImageSharp CVE fermés dans STATE.md

affects: [08-03-publish-workflow, 08-04-smoke-test]

tech-stack:
  upgraded:
    - SixLabors.ImageSharp 2.1.9 → 4.0.0 (src/SDK.Tools/SDK.Tools.csproj)
    - FluentAssertions 8.4.0 → 8.10.0 (tests/SDK.Tools.Tests/SDK.Tools.Tests.csproj)
  patterns:
    - "sixlabors.lic gitignored — ne jamais committer la clé de licence"
    - "CI : secret SIXLABORS_LICENSE_KEY + -p:SixLaborsLicenseKey pour build headless"

key-files:
  modified:
    - src/SDK.Tools/SDK.Tools.csproj
    - tests/SDK.Tools.Tests/SDK.Tools.Tests.csproj
    - .gitignore
    - .paul/STATE.md
  gitignored:
    - src/SDK.Tools/sixlabors.lic
  created: [.paul/phases/08-nuget-distribution/08-02-SUMMARY.md]

key-decisions:
  - "ImageSharp 4.0.0 retenu — seule version fixant les deux CVEs (HIGH + MODERATE). Six Labors Split License gratuite pour open-source MIT."
  - "sixlabors.lic en local (gitignored) — contributeurs demandent leur clé sur licensing.sixlabors.com"
  - "CI GitHub Actions : ajouter secret SIXLABORS_LICENSE_KEY + flag -p:SixLaborsLicenseKey (plan 08-03)"
  - "FA v8.10.0 retenu — Xceed License gratuite pour MIT. Test-only, 0 impact consommateurs NuGet."

duration: ~60min
started: 2026-06-07T14:00:00Z
completed: 2026-06-07T16:00:00Z
---

# Phase 8 Plan 02: Licence/CVE Cleanup Summary

**0 CVE HIGH/MODERATE sur toute la solution. ImageSharp 4.0.0 + FA 8.10.0 — 166 tests verts.**

## Performance

| Métrique | Valeur |
|---------|--------|
| Durée | ~60 min |
| Tâches | 2/2 complètes |
| Fichiers modifiés | 3 (.csproj ×2, .gitignore) |
| Tests | 166 verts (0 régressions) |

## Acceptance Criteria Results

| Critère | Statut | Détails |
|---------|--------|---------|
| AC-1 : ImageSharp sans CVE | **Pass** | 4.0.0 + sixlabors.lic — 0 NU1902/NU1903 sur SDK.Tools |
| AC-2 : FA v8.10.0 standardisé, ⚠️ fermé | **Pass** | SDK.Tools.Tests 8.4.0 → 8.10.0. Deferred issue fermé ✅ |
| AC-3 : `--vulnerable --include-transitive` clean | **Pass** | 0 CVE sur 17 projets de la solution |

## Accomplissements

- `SixLabors.ImageSharp` upgradé 2.1.9 → 4.0.0 — les deux CVEs (HIGH GHSA-2cmq-823j-5qj8 + MODERATE GHSA-rxmq-m78w-7wmc) résolus
- `sixlabors.lic` placé dans `src/SDK.Tools/` (gitignored) — licence communautaire gratuite open-source
- `**/sixlabors.lic` ajouté au `.gitignore` racine
- `FluentAssertions` standardisé 8.10.0 sur 8/8 projets tests — dérive 8.4.0 éliminée
- `dotnet list package --vulnerable` → 0 warning sur toute la solution
- 166 tests verts — 0 régression

## Fichiers Modifiés

| Fichier | Changement | But |
|---------|-----------|-----|
| `src/SDK.Tools/SDK.Tools.csproj` | ImageSharp 2.1.9 → 4.0.0 | Résoudre CVEs HIGH + MODERATE |
| `tests/SDK.Tools.Tests/SDK.Tools.Tests.csproj` | FluentAssertions 8.4.0 → 8.10.0 | Standardiser FA sur tous les projets tests |
| `.gitignore` | `**/sixlabors.lic` ajouté | Protéger la clé de licence |
| `src/SDK.Tools/sixlabors.lic` | Fichier créé (gitignored) | Licence communautaire SixLabors |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| ImageSharp 4.0.0 (pas 3.1.7) | 3.1.7 fixe le HIGH mais MODERATE persiste — AC-3 non satisfait | 0 CVE sur toute la solution |
| `sixlabors.lic` local gitignored | Doc SixLabors : ne jamais committer en open-source public | Contributeurs → `licensing.sixlabors.com` |
| CI via secret `SIXLABORS_LICENSE_KEY` | Build headless sans fichier .lic — `dotnet build -p:SixLaborsLicenseKey=...` | À configurer en 08-03 (publish-nuget.yml) |
| FA v8.10.0 retenu | Xceed License gratuite pour MIT — pas besoin de rétrograder v7.x | Deferred issue Plan 01-01 fermé |

## Déviations par rapport au Plan

| Déviation | Raison |
|-----------|--------|
| ImageSharp 4.0.0 au lieu de 3.x ciblé dans le plan | 3.1.7 (latest 3.x) ne fixe pas MODERATE CVE — upgrade 4.0.0 requis pour AC-3 |
| `sixlabors.lic` fichier local (non prévu) | 4.0.0 impose validation licence au build — fichier .lic = solution locale sans hardcoder la clé |

## Issues Déférées

| Issue | Plan cible |
|-------|-----------|
| CI `publish-nuget.yml` : ajouter secret `SIXLABORS_LICENSE_KEY` + flag MSBuild | 08-03 |
| Contributeurs : doc CONTRIBUTING.md → demander clé sur `licensing.sixlabors.com` | Phase 11 (docs) |

## Readiness Phase 8 — Plans Suivants

**Prêt :**
- 0 CVE → 08-03 peut configurer `publish-nuget.yml` sans dette de sécurité
- FA 8.10.0 uniforme → 0 dérive de version dans les tests

**Action requise en 08-03 :**
- Ajouter secret GitHub `SIXLABORS_LICENSE_KEY` + `-p:SixLaborsLicenseKey="$SIXLABORS_LICENSE_KEY"` dans le workflow CI

---
*Phase: 08-nuget-distribution, Plan: 02*
*Complété: 2026-06-07*
