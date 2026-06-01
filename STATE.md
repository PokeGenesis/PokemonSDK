---
state_version: 2.0
milestone: v0.1
milestone_name: Proof of Concept
status: ready_to_plan
last_updated: 2026-06-01T00:00:00.000Z
last_activity: 2026-06-01 -- Fresh start .NET 10, all planning files initialized
progress:
  horizon: v0.1
  total_phases: 4
  completed_phases: 0
  current_phase: 1
  current_wave: 0
  total_plans: 0
  completed_plans: 0
  percent: 0
stopped_at: Ready to start Phase 1 Wave 1 — solution scaffold .NET 10
---

# Project State

## Project Reference

See: PROJECT.md | REQUIREMENTS.md | ROADMAP.md | CLAUDE.md

**Core value:** Un développeur peut brancher ce SDK et obtenir immédiatement un moteur de combat, une base de données Pokémon multilingue et un système de quêtes fonctionnel — sans réimplémenter les règles de base.

**Current horizon:** v0.1 — Proof of Concept (Phases 1→4)

## Current Position

| Field | Value |
|-------|-------|
| Horizon | v0.1 |
| Phase | 1 — SDK.Core + SDK.Data |
| Wave | Not started |
| Status | Ready to plan |
| Last activity | 2026-06-01 |

Progress v0.1: `[░░░░░░░░░░]` 0%

## Context du redémarrage

**Pourquoi from scratch :** Migration .NET 8 → .NET 10. L'ancienne base a validé les décisions architecturales (schéma SQLite, EF Core, BattleState immuable, MoonSharp SoftSandbox, etc.) — voir CLAUDE.md section 4. Le code est à réécrire proprement sur .NET 10.

**Ce qui était validé conceptuellement (ancienne base .NET 8) :**
- Phase 1 : schéma SQLite, EF Core migrations, translations, filtres génération
- Phase 2 : BattleEngine, IDamageFormula ×2, IDifficultyMode ×2, correction Sleep/Freeze (D-11)
- Phase 4 : MoonSharp SoftSandbox, GameState, SaveSystem JSON, badges

**Ce qui n'était PAS fait :**
- Phase 3 (World/Overworld) — not started
- Phase 5+ (Plugins, Characters, DX, NuGet, Sample, CLI, Docs) — not started

## Accumulated Decisions (à ne pas remettre en question)

Voir CLAUDE.md section 8 — Décisions Architecturales Figées (D-01 à D-21).

Décisions critiques à garder en tête pour Phase 1 :
- D-01 : .NET 10 — `net10.0` dans tous les `.csproj`
- D-03 : EF Core 10 — jamais Dapper seul
- D-07 : Table `translations` centrale — jamais colonnes `name_fr`, `name_en` sur les entités
- D-09 : `generation` INT NOT NULL sur toutes entités concernées

## Pending Todos

- [ ] Vérifier compatibilité MonoGame.Framework.DesktopGL avec .NET 10 avant Phase 3
- [ ] Vérifier compatibilité MoonSharp 2.0.0 avec .NET 10 avant Phase 4
- [ ] Créer compte NuGet.org + réserver `PokéForge.SDK` avant Phase 8

## Deferred Items

| Category | Item | Status | Deferred At |
|----------|------|--------|-------------|
| MAP | Tiled .tmx import (MAP-V2-01) | v2 | Init |
| MAP | Avalonia map editor (MAP-V2-02) | v2 / editor repo | Init |
| QUEST | Quest system with flags (QUEST-V2-01/02) | v2 | Init |
| BATTLE | Double battles 2v2 (BATTLE-V2-01) | v2 | Init |
| BATTLE | Ability + held-item hook arch (BATTLE-V2-02) | v2 | Init |
| DX | CLI `pokeforge` (DX-05) | v2.0 — Phase 10 | Init |
| DX | Site docs (DX-06) | v2.0 — Phase 11 | Init |
| ADV | TTS (ADV-03) | v2.0 — Phase 6 | Init |
| ADV | Fakemon generator (ADV-04) | v2.0 — Phase 6 | Init |

## Session Continuity

Last session: 2026-06-01
Stopped at: Planning initialized — ready to start Phase 1 Wave 1 with Claude Code
Next action: `/paul:init` → `/office-hours` → `/paul:plan` → Phase 1 Wave 1 scaffold
