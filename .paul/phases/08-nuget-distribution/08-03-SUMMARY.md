---
phase: 08-nuget-distribution
plan: 03
subsystem: cicd
tags: [nuget, github-actions, sixlabors, imagesharp, publish, workflow, packageicon]

requires:
  - phase: 08-nuget-distribution
    plan: 02
    provides: ImageSharp 4.0.0 sans CVE + sixlabors.lic — CI doit injecter SIXLABORS_LICENSE_KEY

provides:
  - publish-nuget.yml production-ready (7 packs, SIXLABORS_LICENSE_KEY, E-03 guard)
  - ci.yml corrigé — builds PR/feature ne cassent plus depuis ImageSharp 4.0.0
  - PackageIcon pokegenesis_logo.png embarquée dans les 7 nupkg
  - Org PokeGenesis NuGet créée, API key configurée, secrets GitHub configurés
  - Demande de réservation namespace PokeForge envoyée à NuGet

affects: [08-04-smoke-test, phase9-sample-project]

tech-stack:
  patterns:
    - "publish-nuget.yml: dotnet tool restore → build Release + SixLabors key → test --no-build → pack --no-build → push E-03 guard → upload-artifact"
    - "PackageIcon: pokegenesis_logo.png bundlé via Directory.Build.props ItemGroup Condition IsPackable=true"
    - "E-03 pattern: guard secret dans run: shell, pas en condition YAML"
    - "CI license pattern: SIXLABORS_LICENSE_KEY secret → -p:SixLaborsLicenseKey= au build uniquement"

key-files:
  modified:
    - .github/workflows/publish-nuget.yml
    - .github/workflows/ci.yml
    - Directory.Build.props
    - .paul/STATE.md
  created:
    - pokegenesis_logo.png
    - .paul/phases/08-nuget-distribution/08-03-PLAN.md
    - .paul/phases/08-nuget-distribution/08-03-SUMMARY.md

key-decisions:
  - "PackageIcon ajouté hors plan — conformité NuGet best practices + email namespace reservation plus crédible"
  - "Namespace reservation via email support@nuget.org (pas self-service) — non bloquant pour publication"
  - "Tag pattern v*.*.* (SemVer strict) au lieu de v* — évite déclenchements sur tags non-versionnés"
  - "API key scope: Push new packages and package versions — requis pour première publication"

duration: ~90min
started: 2026-06-07T16:30:00Z
completed: 2026-06-07T18:00:00Z
---

# Phase 8 Plan 03: CI/CD Publish Workflow Summary

**publish-nuget.yml production-ready (7 packs, SixLabors license, E-03 guard) + ci.yml corrigé + PackageIcon embarquée + secrets GitHub configurés.**

## Performance

| Métrique | Valeur |
|---------|--------|
| Durée | ~90 min |
| Tâches | T1 auto ✅ + T2 human ✅ + T3 human ✅ |
| Fichiers modifiés | 4 (workflows ×2, Directory.Build.props, STATE.md) |
| Fichiers créés | 1 (pokegenesis_logo.png) |

## Acceptance Criteria Results

| Critère | Statut | Détails |
|---------|--------|---------|
| AC-1 : publish-nuget.yml — 7 packs + licence + E-03 | **Pass** | `dotnet pack PokemonSDK.slnx --no-build` via IsPackable=true ; SIXLABORS_LICENSE_KEY au build ; guard `if [ -z "$NUGET_API_KEY" ]` dans run: |
| AC-2 : ci.yml — build step SIXLABORS_LICENSE_KEY | **Pass** | Build step + env + `-p:SixLaborsLicenseKey=` sur ubuntu-latest + windows-latest |
| AC-3 : Secrets GitHub configurés | **Pass** | NUGET_API_KEY + SIXLABORS_LICENSE_KEY ajoutés dans GitHub Secrets (confirmés par l'utilisateur) |

## Accomplissements

- `publish-nuget.yml` réécrit entièrement : `dotnet tool restore` (MGCB), build Release + `SIXLABORS_LICENSE_KEY`, `dotnet pack PokemonSDK.slnx --no-build` (7 packages via `IsPackable=true`), guard E-03, `upload-artifact`
- `ci.yml` patché : build step injecte `SIXLABORS_LICENSE_KEY` — corrige tous les builds PR/feature cassés depuis ImageSharp 4.0.0 (08-02)
- `Directory.Build.props` : `<PackageIcon>pokegenesis_logo.png</PackageIcon>` + bundle `ItemGroup` — icône 1024×1024 embarquée dans les 7 nupkg
- Org **PokeGenesis** créée sur NuGet.org, API key `PokemonSDK-CI` générée (scope `PokeForge.*`, Push new packages and package versions)
- `NUGET_API_KEY` + `SIXLABORS_LICENSE_KEY` configurés dans GitHub Secrets du repo
- Demande de réservation namespace `PokeForge` envoyée à support@nuget.org avec email complet (préfixe, packages actuels + futurs, conformité métadonnées)
- Deferred issue `Créer compte NuGet + réserver PokéForge.SDK` (Init, 🔲) → **fermé** ✅

## Fichiers Modifiés

| Fichier | Changement | But |
|---------|-----------|-----|
| `.github/workflows/publish-nuget.yml` | Réécriture complète | 7 packs, SixLabors license, E-03 guard, upload-artifact |
| `.github/workflows/ci.yml` | Build step uniquement | Injecter SIXLABORS_LICENSE_KEY — corrige CI cassé depuis 08-02 |
| `Directory.Build.props` | PackageIcon + ItemGroup | pokegenesis_logo.png embarquée dans les 7 nupkg |
| `pokegenesis_logo.png` | Créé (1024×1024 PNG) | Logo PokeGenesis — icône NuGet officielle |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| PackageIcon ajouté (hors plan initial) | Critère best practices NuGet + renforce email namespace reservation | 7 packages affichent l'icône sur nuget.org |
| Namespace reservation via email | NuGet prefix reservation n'est pas self-service — soumission à support@nuget.org requise | Non bloquant : publication possible sans badge "verified" |
| Tag pattern `v*.*.*` (pas `v*`) | SemVer strict — évite déclenchements accidentels sur tags `v1-beta` etc. | Workflow se déclenche uniquement sur tags SemVer complets |
| API key scope "Push new packages and package versions" | Requis pour créer 7 nouveaux package IDs inexistants sur NuGet.org | Sans ce scope, première publication bloquée |

## Déviations par rapport au Plan

| Déviation | Raison |
|-----------|--------|
| PackageIcon ajouté (scope addition) | Découverte en préparant l'email namespace reservation — critère explicite NuGet best practices |
| Namespace reservation = email (pas UI) | NuGet.org n'offre pas de réservation self-service directe — découverte lors de T2 |
| Tag pattern changé `v*` → `v*.*.*` | `v*` trop large, peut matcher `v1-rc1` — SemVer strict plus safe |

## Issues Déférées

| Issue | Plan cible |
|-------|-----------|
| Réservation namespace `PokeForge` en attente réponse NuGet | Suivi manuel — non bloquant pour 08-04 |
| Contributeurs : doc CONTRIBUTING.md → `sixlabors.lic` sur licensing.sixlabors.com | Phase 11 (docs) |
| Smoke test local NuGet feed — vérifier consommation via NuGet | 08-04 |

## Readiness Phase 8 — Plan Suivant

**Prêt :**
- publish-nuget.yml production-ready — déclencher sur tag `v0.1.0` suffira
- ci.yml corrigé — tous les PR/feature builds passent (SIXLABORS_LICENSE_KEY injecté)
- Secrets GitHub configurés — workflow fonctionnel dès maintenant
- 7 nupkg avec icône, licence MIT, README, métadonnées complètes

**Action requise en 08-04 :**
- Smoke test local NuGet feed : publier sur feed local, consommer depuis projet test, vérifier les 7 packages installables

**Aucun bloqueur.**

---
*Phase: 08-nuget-distribution, Plan: 03*
*Complété: 2026-06-07*
