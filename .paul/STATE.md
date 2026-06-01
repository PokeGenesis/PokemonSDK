# Project State

## Project Reference

See: PROJECT.md | REQUIREMENTS.md | ROADMAP.md | .claude/ARCHITECTURE.md

**Core value:** Brancher le SDK → moteur de combat + DB multilingue + quêtes, sans réimplémenter les règles de base.

**Current focus:** v0.1 — Phase 1 (SDK.Core + SDK.Data) — Plan 01-01 exécuté, prêt pour UNIFY

## Current Position

Milestone: v0.1 Proof of Concept
Phase: 1 of 4 (SDK.Core + SDK.Data) — Execution complete
Plan: 01-01 exécuté — 3/3 tâches PASS
Status: APPLY complete, prêt pour UNIFY
Last activity: 2026-06-01 — Plan 01-01 APPLY terminé avec succès

Progress:

- Milestone v0.1: [█░░░░░░░░░] ~5%
- Phase 1: [██░░░░░░░░] ~25%

## Loop Position

Current loop state:

```text
PLAN ──▶ APPLY ──▶ UNIFY
  ✓        ✓        ○     [APPLY complete, en attente de UNIFY]
```

## Accumulated Context

### Decisions

Architecture figée D-01→D-21 — voir CLAUDE.md section 8.

Clés pour Phase 1 :

- D-01 : .NET 10 — `net10.0` dans tous les .csproj
- D-03 : EF Core 10 — jamais Dapper seul
- D-07 : Table `translations` centrale — jamais colonnes `name_fr`, `name_en` sur les entités
- D-09 : `generation` INT NOT NULL sur toutes entités concernées

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
Stopped at: Plan 01-01 APPLY terminé
Next action: `/paul:unify .paul/phases/01-sdk-core-data/01-01-PLAN.md`
Resume file: `.paul/phases/01-sdk-core-data/01-01-PLAN.md`

---

*STATE.md — Updated after every significant action*
