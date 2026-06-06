---
phase: 07-developer-experience
plan: 01
subsystem: tooling
tags: [sprite-validator, asset-pipeline, cli, sdk-tools, ci]

requires:
  - phase: 05-plugins-characters
    provides: SDK.Tools project base avec commande seed

provides:
  - SpriteValidator + SpriteScanner (validation D-16 BCL pur)
  - CLI asset-validate avec exit codes CI
  - SDK.Tools.Tests (17 tests verts)

affects: [phase8-nuget, cicd]

tech-stack:
  added: [SDK.Tools.Tests (xUnit + FluentAssertions)]
  patterns: [PNG header parsing BCL FileStream 26 bytes, CLI exit code 1 si ERROR]

key-files:
  created:
    - src/SDK.Tools/Validation/SpriteEntry.cs
    - src/SDK.Tools/Validation/SpriteValidationResult.cs
    - src/SDK.Tools/Validation/SpriteValidator.cs
    - src/SDK.Tools/Validation/SpriteScanner.cs
    - tests/SDK.Tools.Tests/SDK.Tools.Tests.csproj
    - tests/SDK.Tools.Tests/SpriteValidatorTests.cs
  modified:
    - src/SDK.Tools/Program.cs
    - PokemonSDK.slnx

key-decisions:
  - "D-17 intact : SDK.Tools zéro NuGet — PNG parsing via FileStream BCL 26 bytes (signature + IHDR)"
  - "SeverityLevel : nommage non-D16 → WARN, taille/alpha/corruption → ERROR"

patterns-established:
  - "PNG header synthétique 26 bytes pour tests sans vraie image (CreateTempPng helper)"

duration: ~45min
started: 2026-06-06T19:20:00Z
completed: 2026-06-06T20:05:00Z
---

# Phase 7 Plan 01: SpriteValidator + SpriteScanner + CLI asset-validate Summary

**SpriteValidator BCL pur (zéro NuGet), SpriteScanner récursif PNG, CLI `asset-validate` avec exit code 1 si ERROR — 17 tests verts.**

## Performance

| Metric | Value |
|--------|-------|
| Duration | ~45min |
| Started | 2026-06-06T19:20:00Z |
| Completed | 2026-06-06T20:05:00Z |
| Tasks | 3 completed |
| Files modified | 7 |

## Acceptance Criteria Results

| Criterion | Status | Notes |
|-----------|--------|-------|
| AC-1: D-17 intact — zéro NuGet SDK.Tools | Pass | `dotnet list` → liste vide, FileStream BCL uniquement |
| AC-2: CLI asset-validate exit codes corrects | Pass | exit 1 si ERROR, exit 0 si OK/WARN uniquement |
| AC-3: Règles validation D-16 correctes | Pass | Naming→WARN, size/alpha/corruption→ERROR |
| AC-4: SpriteValidatorTests ≥12 tests verts | Pass | 17 tests verts |

## Accomplishments

- SpriteValidator valide nommage D-16, taille (96×96/48×48/128×128/32×32), canal alpha, corruption PNG — uniquement FileStream BCL
- CLI `asset-validate [path]` intégrable en CI GitHub Actions sans setup NuGet supplémentaire
- SDK.Tools.Tests créé — pattern helper `CreateTempPng` réutilisable pour tests assets

## Task Commits

| Task | Commit | Type | Description |
|------|--------|------|-------------|
| T0: Types + SpriteValidator | `3e976b0` | feat | SpriteEntry, SpriteValidationResult, SpriteValidator (logique pure BCL) |
| T1: SpriteScanner + CLI | `3e976b0` | feat | SpriteScanner récursif + asset-validate dans Program.cs |
| T2: SDK.Tools.Tests | `3e976b0` | test | 17 tests SpriteValidatorTests, slnx mis à jour |

## Files Created/Modified

| File | Change | Purpose |
|------|--------|---------|
| `src/SDK.Tools/Validation/SpriteEntry.cs` | Created | Record immutable (FilePath, DexId, Identifier, View) |
| `src/SDK.Tools/Validation/SpriteValidationResult.cs` | Created | Record immutable + enum SeverityLevel |
| `src/SDK.Tools/Validation/SpriteValidator.cs` | Created | Logique validation D-16 + parsing PNG header 26 bytes |
| `src/SDK.Tools/Validation/SpriteScanner.cs` | Created | Découverte récursive *.png |
| `src/SDK.Tools/Program.cs` | Modified | Ajout branche `asset-validate` + Usage mis à jour |
| `tests/SDK.Tools.Tests/SDK.Tools.Tests.csproj` | Created | Projet test xUnit + FluentAssertions |
| `tests/SDK.Tools.Tests/SpriteValidatorTests.cs` | Created | 17 tests (naming, size, alpha, corruption) |
| `PokemonSDK.slnx` | Modified | Ajout SDK.Tools.Tests |

## Decisions Made

| Decision | Rationale | Impact |
|----------|-----------|--------|
| PNG header 26 bytes (signature 8 + IHDR chunk) | D-17 : zéro NuGet, FileStream BCL suffisant pour signature + width/height/colortype | Pattern réutilisable dans SDK.Tools |
| Nommage non-D16 → WARN (pas ERROR) | Un fichier mal nommé est utilisable, juste non conforme convention | Moins bloquant en CI, évite faux positifs |
| `CreateTempPng` helper dans tests | Évite dépendance à de vraies images PNG, 100% déterministe | Pattern réutilisé dans SDK.Tools.Tests futurs |

## Deviations from Plan

None — plan exécuté exactement.

## Next Phase Readiness

**Ready:**
- SDK.Tools.Tests opérationnel, pattern test PNG établi
- CLI asset-validate intégrable en CI immédiatement
- SpriteScanner réutilisable par AtlasPacker (plan 07-02)

**Concerns:**
- SixLabors.ImageSharp pas encore utilisé — prévu plan 07-02 pour AtlasPacker

**Blockers:** None

---
*Phase: 07-developer-experience, Plan: 01*
*Completed: 2026-06-06*
