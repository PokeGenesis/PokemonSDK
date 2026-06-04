# Project State

## Project Reference

See: PROJECT.md | REQUIREMENTS.md | ROADMAP.md | .claude/ARCHITECTURE.md

**Core value:** Brancher le SDK → moteur de combat + DB multilingue + quêtes, sans réimplémenter les règles de base.

**Current focus:** v0.1 — Phase 3 (World Foundation) — Phases 1 & 2 complètes, prêt à planifier

## Current Position

Milestone: v0.1 Proof of Concept
Phase: 3 of 4 (World Foundation) — Ready to plan
Plan: Not started
Status: Phase 2 complète — prêt pour Phase 3
Last activity: 2026-06-04 — Phase 2 UNIFY+TRANSITION — 47/47 tests, Phase 2 100%

Progress:

- Milestone v0.1: [██████░░░░] ~55%
- Phase 1: [██████████] 100% ✅
- Phase 2: [██████████] 100% ✅
- Phase 3: [░░░░░░░░░░] 0% (not started)

## Loop Position

Current loop state:

```text
PLAN ──▶ APPLY ──▶ UNIFY
  ✓        ✓        ✓     [Plan 02-04 fermé — Phase 2 complète — prêt pour Phase 3]
```

## Accumulated Context

### Decisions

Architecture figée D-01→D-22 — voir CLAUDE.md section 8.

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
- Pattern IDesignTimeDbContextFactory établi dans `SDK.Data/DesignTime/` — réutilisable Plans 03-01, 04-02
- Pattern SqliteTestFixture :memory: établi — réutilisable pour tous les tests Data futurs

Décisions émergentes (Phase 2) :

- D-11 : Sleep/Freeze ne sautent pas les tours — correction critique validée, testée
- `BattleConfig` n'a pas de `AccuracyEnabled` — déterminisme via `move.Accuracy=100` dans les tests
- STAB (×1.5) calculé dans `BattleEngine.ApplyMove`, passé comme argument `typeMultiplier` à `IDamageFormula.Calculate`
- `BattleResult` : propriétés `PlayerWon`, `TurnsElapsed`, `EndReason`
- Immunité type (typeChart retourne 0.0m) court-circuite `formula.Calculate` — jamais appelé
- Gen1DamageFormula utilise `defender.SpecialAttack` pour D (pas de SpDef en Gen1) — StandardDamageFormula utilise `defender.SpecialDefense`
- `BattleEngine.MaxTurns = 200` — timeout → `EndReason = "MaxTurns"`, `PlayerWon = false`
- Pattern Moq Callback pour capturer les arguments passés aux interfaces (vérification STAB)
- `BattleTestHelpers` factory établi — réutilisable Phase 5 (plugins)

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

Last session: 2026-06-04
Stopped at: Phase 2 complète — SDK.Battle.Tests (30 tests), 47/47 solution verts, transition Phase 3 prête
Next action: `/paul:plan 03-01` — Migration 003 + SDK.Core world primitives
Resume file: `.paul/ROADMAP.md`

---

*STATE.md — Updated after every significant action*
