# Project State

## Project Reference

See: PROJECT.md | REQUIREMENTS.md | ROADMAP.md | .claude/ARCHITECTURE.md

**Core value:** Brancher le SDK → moteur de combat + DB multilingue + quêtes, sans réimplémenter les règles de base.

**Current focus:** v0.1 — Phase 1 (SDK.Core + SDK.Data) — Plan 01-03 UNIFY complet, prêt pour Plan 01-04

## Current Position

Milestone: v0.1 Proof of Concept
Phase: 1 of 4 (SDK.Core + SDK.Data) — In progress (3/4 plans complets)
Plan: 01-03 UNIFY complet — prochain : 01-04
Status: Ready for /paul:plan 01-04
Last activity: 2026-06-02 — Plan 01-03 UNIFY complet

Progress:

- Milestone v0.1: [███░░░░░░░] ~25%
- Phase 1: [███████░░░] ~75%

## Loop Position

Current loop state:

```text
PLAN ──▶ APPLY ──▶ UNIFY
  ✓        ✓        ✓     [Loop complet — prêt pour Plan 01-04]
```

## Accumulated Context

### Decisions

Architecture figée D-01→D-21 — voir CLAUDE.md section 8.

Clés pour Phase 1 :

- D-01 : .NET 10 — `net10.0` dans tous les .csproj
- D-03 : EF Core 10 — jamais Dapper seul
- D-07 : Table `translations` centrale — jamais colonnes `name_fr`, `name_en` sur les entités
- D-09 : `generation` INT NOT NULL sur toutes entités concernées

Décisions émergentes (Plan 01-01) :

- `.slnx` format : .NET 10 `dotnet new sln` crée `PokemonSDK.slnx` (XML, sans GUID). Toutes commandes build utilisent `.slnx`.
- `PokemonType` nommé avec préfixe — évite conflit `System.Type`
- CoreDependencyTests path traversal : 5× `..` depuis `AppContext.BaseDirectory` → root projet
- `TypeEffectiveness` clé composite `(AttackerTypeId, DefenderTypeId, Generation)` — un couple de types peut changer entre générations

Décisions émergentes (Plan 01-02) :

- EF Core 10 crée `__EFMigrationsLock` en plus de `__EFMigrationsHistory` — 8 tables au total (6 entités + 2 system)
- `data/PokemonSDK.db` créé dans `src/SDK.Data/data/` via design-time factory (cwd = répertoire projet, pas racine repo) — comportement attendu
- `Microsoft.EntityFrameworkCore.Design` PrivateAssets auto-configuré par `dotnet add package`
- Pattern IDesignTimeDbContextFactory établi dans `SDK.Data/DesignTime/` — réutilisable Plans 02-02, 03-01, 04-02
- Pattern SqliteTestFixture :memory: établi — réutilisable pour tous les tests Data futurs

### Deferred Issues

| Issue | Origin | Revisit | Status |
|-------|--------|---------|--------|
| Vérifier compat MonoGame.DesktopGL / .NET 10 | Init | Avant Phase 3 | ✅ MonoGame 3.8.4.1 compile sur net10.0 (smoke 2026-06-01). Template `dotnet new mgdesktopgl` non testé. |
| Vérifier compat MoonSharp 2.0.0 / .NET 10 | Init | Avant Phase 4 | 🔲 |
| Créer compte NuGet + réserver PokéForge.SDK | Init | Avant Phase 8 | 🔲 |
| FluentAssertions v8 licence Xceed (commercial) | Plan 01-01 | Avant Phase 8 | ⚠️ OK open-source/non-commercial. Envisager pin v7.x (Apache 2.0) si SDK distribué commercialement. |

### Blockers/Concerns

None.

## Session Continuity

Last session: 2026-06-02
Stopped at: Plan 01-03 UNIFY complet
Next action: `/paul:plan 01-04` — end-to-end SDK test + PLAT-01/03 cross-target scan
Resume file: `.paul/phases/01-sdk-core-data/01-03-SUMMARY.md`

---

*STATE.md — Updated after every significant action*
