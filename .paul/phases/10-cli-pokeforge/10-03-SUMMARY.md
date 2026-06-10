---
phase: 10-cli-pokeforge
plan: 03
subsystem: cli
tags: [dotnet-tool, nuget, ci, doctor, health-check, github-actions]

requires:
  - phase: 10-02
    provides: asset-sync + seed commands, 10 tests passing, ImportConfig + xunit.runner.json

provides:
  - DoctorCommand (pokeforge doctor) — project health check headless
  - PokeForge.CLI NuGet metadata complète (licence, tags, README, IsPackable)
  - publish-cli.yml CI workflow — tag-triggered NuGet publish

affects: [phase-11-documentation, phase-6-advanced-systems]

tech-stack:
  added: []
  patterns: [E-03 NuGet secret guard in run-shell, IsPackable=true override for net10.0 Exe]

key-files:
  created:
    - src/SDK.Cli/Commands/DoctorCommand.cs
    - tests/SDK.Cli.Tests/DoctorCommandTests.cs
    - .github/workflows/publish-cli.yml
  modified:
    - src/SDK.Cli/Program.cs
    - src/SDK.Cli/SDK.Cli.csproj

key-decisions:
  - "IsPackable=true requis explicitement : .NET 10.0.108 force IsPackable=false sur Exe projects (PackAsTool inclus)"
  - "PackageType DotnetTool ItemGroup supprimé : redondant avec PackAsTool=true"
  - "<None>README.md</None> supprimé : PackageReadmeFile suffit, l'ajout explicit causait NU5118"
  - "doctor = filesystem checks only, jamais ctx.Database.Migrate() (D-17 headless)"

patterns-established:
  - "IDisposable+CWD tempDir pattern pour tous les tests CLI (DoctorCommandTests, SeedCommandTests, AssetSyncCommandTests)"
  - "E-03 : if [ -z \"$SECRET\" ]; then exit 0; fi dans run-shell, jamais dans if: YAML"

duration: ~25min
started: 2026-06-09T19:42:00Z
completed: 2026-06-09T20:05:00Z
---

# Phase 10 Plan 03: DoctorCommand + NuGet Metadata + publish-cli.yml Summary

**`pokeforge doctor` implémenté (3 checks filesystem headless), `PokeForge.CLI` packable avec métadonnées NuGet complètes, CI `publish-cli.yml` prêt pour tag `cli-v0.1.0`.**

## Performance

| Metric | Value |
|--------|-------|
| Duration | ~25 min |
| Started | 2026-06-09T19:42:00Z |
| Completed | 2026-06-09T20:05:00Z |
| Tasks | 3/3 complètes |
| Files modified | 5 |

## Acceptance Criteria Results

| Criterion | Status | Notes |
|-----------|--------|-------|
| AC-1: doctor config manquante → exit 1 | **Pass** | `Execute_MissingConfig_ReturnsOne` vert |
| AC-2: doctor config valide, DB manquante → exit 0 + WARN | **Pass** | `Execute_ValidConfigNoDb_ReturnsZero` vert |
| AC-3: doctor config valide, tout présent → exit 0 | **Pass** | `Execute_ValidConfigAllPresent_ReturnsZero` vert |
| AC-4: PokeForge.CLI se packe proprement | **Pass** | `PokeForge.CLI.0.1.0.nupkg` + `DotnetToolSettings.xml` confirmés, 0 warnings |
| AC-5: build + tests verts | **Pass (avec note)** | `dotnet test` → **13/13** (plan estimait 11 — voir Déviations). Build CLI/tests 0 erreurs. |

## Accomplishments

- `pokeforge doctor` opérationnel : 3 checks (import.json → exit 1 si absent ; sprites_root → ERROR+exit 1 ; db_path → WARN only, suggère `pokeforge seed`)
- `PokeForge.CLI.0.1.0.nupkg` valide produit par `dotnet pack` : `DotnetToolSettings.xml` à `tools/net10.0/any/`, `README.md` inclus, 0 NU* warnings
- CI `publish-cli.yml` créé : déclenché par tag `cli-v*.*.*`, guard E-03 dans shell, `upload-artifact@v6` pour archivage même sans NUGET_API_KEY

## Files Created/Modified

| File | Change | Purpose |
|------|--------|---------|
| `src/SDK.Cli/Commands/DoctorCommand.cs` | Créé | `pokeforge doctor` — 3 checks filesystem headless |
| `src/SDK.Cli/Program.cs` | Modifié | `DoctorCommand.Register(rootCommand)` ajouté |
| `src/SDK.Cli/SDK.Cli.csproj` | Modifié | NuGet metadata + `<IsPackable>true</IsPackable>` |
| `tests/SDK.Cli.Tests/DoctorCommandTests.cs` | Créé | 3 tests IDisposable+CWD pattern |
| `.github/workflows/publish-cli.yml` | Créé | CI tag-triggered NuGet publish |

## Decisions Made

| Decision | Rationale | Impact |
|----------|-----------|--------|
| `<IsPackable>true</IsPackable>` ajouté | .NET 10.0.108 force `IsPackable=false` pour tous les Exe projects sur `net10.0` — `PackAsTool=true` seul insuffisant | Sans ce flag, `dotnet pack` s'exécute silencieusement sans produire de `.nupkg` |
| `PackageType DotnetTool` ItemGroup supprimé | `PackAsTool=true` l'inclut implicitement — doublon confirmé via inspection MSBuild | Csproj plus propre, 0 comportement changé |
| `<None Include="README.md">` supprimé | `PackageReadmeFile=README.md` avec `PackAsTool=true` auto-inclut le fichier — l'ItemGroup explicit causait NU5118 "already in package" | 0 warnings, README toujours présent dans le `.nupkg` |

## Deviations from Plan

### Summary

| Type | Count | Impact |
|------|-------|--------|
| Auto-fixed | 3 | Essentiels — correctifs MSBuild/NuGet découverts à l'exécution |
| Scope additions | 0 | Aucun |
| Deferred | 0 | Aucun |

**Total impact:** Correctifs nécessaires, aucun scope creep.

### Auto-fixed Issues

**1. `IsPackable=false` bloquait silencieusement `dotnet pack`**
- **Trouvé pendant :** Task 2 (NuGet metadata)
- **Issue :** `dotnet pack src/SDK.Cli/` retournait exit 0 mais produisait 0 `.nupkg`. Cause : `.NET SDK 10.0.108` injecte `IsPackable=false` pour les projets `OutputType=Exe` ciblant `net10.0`. `PackAsTool=true` ne surcharge pas ce flag.
- **Fix :** `<IsPackable>true</IsPackable>` ajouté explicitement dans `<PropertyGroup>`
- **Vérification :** `dotnet pack -c Release -o /tmp/test-nupkg` → `PokeForge.CLI.0.1.0.nupkg` produit avec `DotnetToolSettings.xml`

**2. `PackageType DotnetTool` ItemGroup — redondant**
- **Trouvé pendant :** Task 2, investigation MSBuild `-v:diag`
- **Issue :** Le plan spécifiait d'ajouter `<ItemGroup><PackageType Include="DotnetTool"/></ItemGroup>` mais `PackAsTool=true` l'insère déjà
- **Fix :** ItemGroup non ajouté (pas dans le csproj final)

**3. `<None Include="README.md">` — NU5118 duplicate**
- **Trouvé pendant :** Task 2, vérification `dotnet pack`
- **Issue :** `PackageReadmeFile=README.md` avec `PackAsTool=true` auto-inclut README — ajouter l'ItemGroup explicit causait `NU5118: The package already contains a file with the path 'README.md'`
- **Fix :** ItemGroup non ajouté — `PackageReadmeFile` seul suffit

### Déviation de count (non bloquante)

AC-5 plan estimait `11/11` tests (`8 existants + 3 nouveaux`). La suite réelle était `10 existants + 3 nouveaux = 13/13`. Le plan 10-02 SUMMARY documentait 10 tests (pas 8) — erreur d'estimation au moment de la création du plan 10-03. Résultat : 13/13 vert est le comportement correct.

## Issues Encountered

| Issue | Resolution |
|-------|------------|
| `dotnet pack` exit 0 mais 0 `.nupkg` produit | Tracé via `dotnet msbuild -t:Pack -v:diag` → `IsPackable=false` détecté → override `<IsPackable>true</IsPackable>` |
| EF Core 10.0.8 vs 10.0.9 CS1705 sur 3 projets | Pré-existant confirmé via `git stash` — hors scope plan 10-03. Documenter comme dette technique. |

## Deferred Items

- **EF Core version mismatch** (SDK.Data.Tests/SDK.Tools.Tests/SDK.MonoGame) : CS1705 sur 3 projets. Pré-existant (confirmé git stash). À corriger avant Phase 11 pour un build solution propre.

## Next Phase Readiness

**Ready:**
- `pokeforge new | asset-sync | seed | doctor` — 4 commandes CLI opérationnelles
- `PokeForge.CLI.0.1.0.nupkg` packable et validé
- CI publish-cli.yml prêt pour le tag `cli-v0.1.0`
- 13/13 tests SDK.Cli.Tests verts
- Phase 10 complète → prêt pour Phase 11 (Documentation) ou Phase 6 (Advanced Systems)

**Concerns:**
- EF Core 10.0.8 vs 10.0.9 : build solution complète a 3 CS1705. CLI build individuel 0 erreurs. À corriger avant que CI sur solution complète soit propre.

**Blockers:** Aucun pour Phase 11 ou Phase 6.

---
*Phase: 10-cli-pokeforge, Plan: 03*
*Completed: 2026-06-09*
