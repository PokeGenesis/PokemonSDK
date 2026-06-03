# Project State

## Project Reference

See: PROJECT.md | REQUIREMENTS.md | ROADMAP.md | .claude/ARCHITECTURE.md

**Core value:** Brancher le SDK → moteur de combat + DB multilingue + quêtes, sans réimplémenter les règles de base.

**Current focus:** v0.1 — Phase 2 (Battle Engine Core) — Phase 1 complete, prêt à planifier

## Current Position

Milestone: v0.1 Proof of Concept
Phase: 2 of 4 (Battle Engine Core) — Planning
Plan: 02-01 fermé (UNIFY complet)
Status: Ready for Plan 02-02
Last activity: 2026-06-03 — Plan 02-01 complet — 10 fichiers créés, 13/13 tests verts

Progress:

- Milestone v0.1: [███░░░░░░░] ~25%
- Phase 1: [██████████] 100% ✅
- Phase 2: [█░░░░░░░░░] 25% (1/4 plans)

## Loop Position

Current loop state:

```text
PLAN ──▶ APPLY ──▶ UNIFY
  ✓        ✓        ✓     [Plan 02-01 fermé — prêt pour Plan 02-02]
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

Last session: 2026-06-03
Stopped at: Plan 02-01 fermé — 10 fichiers créés, 13/13 tests verts, commit 63ea5f5
Next action: `/paul:plan 02-02` — Phase 2 : Migration 002 + BattleDataSeeder
Resume file: `.paul/phases/02-battle-engine-core/02-01-SUMMARY.md`

---

*STATE.md — Updated after every significant action*
