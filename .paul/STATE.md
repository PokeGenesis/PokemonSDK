# Project State

## Project Reference

See: PROJECT.md | REQUIREMENTS.md | ROADMAP.md | .claude/ARCHITECTURE.md

**Core value:** Brancher le SDK → moteur de combat + DB multilingue + quêtes, sans réimplémenter les règles de base.

**Current focus:** v0.1 — Phase 1 (SDK.Core + SDK.Data) — Plan 01-01 UNIFY complete, prêt pour Plan 01-02

## Current Position

Milestone: v0.1 Proof of Concept
Phase: 1 of 4 (SDK.Core + SDK.Data) — In progress (1/4 plans complete)
Plan: 01-01 fermé — UNIFY ✓
Status: Ready for Plan 01-02
Last activity: 2026-06-01 — Plan 01-01 UNIFY terminé

Progress:

- Milestone v0.1: [█░░░░░░░░░] ~8%
- Phase 1: [███░░░░░░░] ~25%

## Loop Position

Current loop state:

```text
PLAN ──▶ APPLY ──▶ UNIFY
  ✓        ✓        ✓     [Loop 01-01 fermé — prêt pour /paul:plan 01-02]
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

- `.slnx` format : `.NET 10` `dotnet new sln` crée `PokemonSDK.slnx` (XML, sans GUID). Toutes commandes build utilisent `.slnx`.
- `PokemonType` nommé avec préfixe — évite conflit `System.Type`
- CoreDependencyTests path traversal : 5× `..` depuis `AppContext.BaseDirectory` → root projet

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

Last session: 2026-06-01
Stopped at: Plan 01-01 UNIFY terminé
Next action: `/paul:plan` — Plan 01-02 : EF Core 10 + PokemonDbContext + Migration 001 + SqliteTestFixture
Resume file: `.paul/phases/01-sdk-core-data/01-01-SUMMARY.md`

---

*STATE.md — Updated after every significant action*
