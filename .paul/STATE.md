# Project State

## Project Reference

See: PROJECT.md | REQUIREMENTS.md | ROADMAP.md | .claude/ARCHITECTURE.md

**Core value:** Brancher le SDK → moteur de combat + DB multilingue + quêtes, sans réimplémenter les règles de base.

**Current focus:** v0.1 — Phase 4 complète ✅ — Prêt pour Phase 5 (Plugins + Characters)

## Current Position

Milestone: v0.1 Proof of Concept
Phase: 4 of 4 (Scripting + Progression) — COMPLETE ✅
Plan: 04-03 UNIFY complet
Status: Phase 4 complète — 97 tests, SCRIPT-01→03 satisfaits
Last activity: 2026-06-05 — Plan 04-03 APPLY + UNIFY (SaveSystem + DialogueBox + Game1 wiring + cherry-pick 98c3299)

Progress:

- Milestone v0.1: [█████████░] ~95%
- Phase 1: [██████████] 100% ✅
- Phase 2: [██████████] 100% ✅
- Phase 3: [██████████] 100% ✅
- Phase 4: [██████████] 100% ✅

## Loop Position

Current loop state:

```text
PLAN ──▶ APPLY ──▶ UNIFY
  ✓        ✓        ✓     [Plan 04-03 complet — Phase 4 bouclée]
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

Décisions émergentes (SDK.Tools) :

- `SDK.Tools` default db-path = `src/SDK.Data/data/PokemonSDK.db` (relatif repo root) — jamais `data/PokemonSDK.db` qui pointe ailleurs. Toujours lancer depuis la racine du repo.

Décisions émergentes (Plan 03-01) :

- EF migrations `--startup-project src/SDK.Data` jusqu'à Plan 03-03 (SDK.MonoGame absent) — pattern établi
- `EncounterZone.SpeciesId` FK direct (une row par espèce/zone) — modèle table de rencontre simple
- `EntityType = "Move"` / `"Ability"` en PascalCase — cohérent avec `"PokemonType"` / `"PokemonSpecies"`

Décisions émergentes (Plan 03-02) :

- `IGameClock` séparé de `IRealTimeClock` — deux contrats distincts : real-time (Gen 2) vs game-time configurable
- `GameTimeClock.Speed` = game-minutes par real-second — `Speed=1f/60f` (1:1), `Speed=1f` (1s=1 min game), `Speed=60f` (1s=1h game)
- `RealTimeClock.MapHour(int hour)` internal static — partagé avec `GameTimeClock` pour éviter duplication du switch
- `IGameClock.SetGameTime(TimeSpan)` — contrat save/load, Plan 04-03 (ISaveSystem) persistera `GameElapsed`
- Pokégear (Phase 5+) configure `Speed` via DI — `IGameClock` injecté dans le service Pokégear

Décisions émergentes (Plan 03-03) :

- `WorldSystem.Update(delta)` appelle `_clock.Update(delta)` en interne — HeadlessRunner et Game1 n'appellent que `world.Update()`, pas clock directement
- `NullInputProvider` enregistré via DI (`IInputProvider`) quand `--headless` — même interface, zéro branchement conditionnel dans PlayerSystem
- `MonoGame.Extended` absent du projet — compat avec MonoGame 3.8.4.1 non vérifiée. TilemapRenderer stub jusqu'à Phase 5+
- `MS.DI 10.0.8` minimum — EF Core 10.0.8 impose cette contrainte transitive (NU1605 si inférieur)
- Shaders `.fx` hors `Content.mgcb` — compilation MGCB déférée Phase 7 DX ; null-safe via try/catch dans RenderPipeline

Décisions émergentes (Plan 03-04) :

- `EncounterZone` dans `SDK.Core.Entities` (pas `SDK.Data.Models`) — SDK.Data/Models/ n'existe pas
- `<Using Include="Xunit" />` requis explicitement dans tout projet test — ImplicitUsings n'inclut pas Xunit
- Test project référence `SDK.MonoGame` + `SDK.Core` explicite — pas de ref SDK.Data si pas d'accès DB direct
- `continue-on-error: true` sur step headless CI — DB absente en CI, xUnit Moq-based = vraie gate qualité

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

Décisions émergentes (Plan 04-01) :

- D-04 confirmé : Preset_SoftSandbox — os est nil dans Lua, os.exit(0) lève ScriptRuntimeException
- D-12 confirmé : GameState.Flags = Dictionary<string,JsonElement> via System.Text.Json BCL
- WithFlag retourne new GameState (record with-expression) — GetFlag<T> retourne default si clé absente
- UserData.RegisterType(api.GetType()) avant Globals[] — MoonSharp exige enregistrement explicite des types CLR

Décisions émergentes (Plan 04-02) :

- `BadgeApi` accumule mutations via copy-on-write sur `GameState` — pattern identique à BattleState (D-05)
- `NpcInteractionRunner.Run()` static : crée BadgeApi, RegisterApi("badges"), Execute, retourne api.GetState()
- `EntityType = "Badge"` (PascalCase) confirmé — cohérent avec "Move" / "Ability"
- D-09 confirmé sur Trainer + Badge — `Generation` NOT NULL dans migration `20260605201847_AddProgressionData`
- `SDK.Scripting.Bindings/` namespace établi — répertoire dédié aux bindings Lua, séparé de Engine/

### Deferred Issues

| Issue | Origin | Revisit | Status |
|-------|--------|---------|--------|
| Vérifier compat MonoGame.DesktopGL / .NET 10 | Init | Avant Phase 3 | ✅ MonoGame 3.8.4.1 compile sur net10.0 (smoke 2026-06-01). Template `dotnet new mgdesktopgl` non testé. |
| Vérifier compat MoonSharp 2.0.0 / .NET 10 | Init | Avant Phase 4 | ✅ MoonSharp 2.0.0 compatible .NET 10 — NuGet restore propre, build 0 warnings. |
| Créer compte NuGet + réserver PokéForge.SDK | Init | Avant Phase 8 | 🔲 |
| FluentAssertions v8 licence Xceed (commercial) | Plan 01-01 | Avant Phase 8 | ⚠️ OK open-source/non-commercial. Envisager pin v7.x (Apache 2.0) si SDK distribué commercialement. |
| Translations Move manquantes (D-22) | Phase 2 | Plan 03-01 | ✅ Résolu — SeedMoveTranslations (15×6=90 rows), BattleTranslationsD22Tests passe. |
| Translations Ability manquantes (D-22) | Phase 2 | Plan 03-01 | ✅ Résolu — SeedAbilityTranslations (6×6=36 rows), BattleTranslationsD22Tests passe. |

Décisions émergentes (Plan 04-03) :

- D-06 résolu : SDK.MonoGame.csproj PEUT référencer SDK.Scripting (cf. CLAUDE.md arch section 3 : `SDK.MonoGame ← ... + SDK.Scripting (via Func factory)`) — la contrainte est que Game1.cs n'utilise JAMAIS LuaScriptEngine directement
- `Func<IScriptEngine>` factory enregistrée dans Program.cs uniquement — Game1 reçoit la factory via DI, appelle `_scriptEngineFactory()` pour créer un engine par interaction
- `SaveSystem` va dans `SDK.Core.Services/` (System.Text.Json BCL only — zéro NuGet ajouté à SDK.Core.csproj)
- `DialogueBox.Draw()` stub no-op — SpriteFont/MGCB déféré Phase 7 DX
- `gym_brock.lua` contenu minimal : `badges:AwardBadge('boulder')` uniquement (pas de `dialogue:` — MoonSharp lèverait exception sur nil en SoftSandbox)

### Blockers/Concerns

None.

### Erreurs opérationnelles (NE PLUS FAIRE)

| # | Erreur | Fix |
| --- | ------ | --- |
| E-01 | `gh pr merge --delete-branch` sur PR dont source = `dev`/`staging`/`main` → branche permanente supprimée | `--delete-branch` UNIQUEMENT sur `feature/*`, `fix/*`, `hotfix/*`. Jamais sur branches permanentes. |
| E-02 | GitHub API `git/refs` POST avec SHA court (7 chars) → 422 "At least 40 characters are required" | Toujours `git rev-parse origin/branche` pour SHA complet 40 chars avant appel API. |
| E-03 | YAML `if: env.NUGET_API_KEY != ''` ne lit pas les secrets du bloc `env:` du même step | Guard de secret DANS le `run:` shell : `if [ -z "$NUGET_API_KEY" ]; then exit 0; fi` |
| E-04 | Squash merge feature→dev→staging→main crée divergence historique (commit absent de la branche cible) → PR CONFLICTING | Après tout squash merge sur main, sync immédiat : `main → staging → dev`. Ne jamais laisser stagner. |

## Session Continuity

Last session: 2026-06-05
Stopped at: Plan 04-03 UNIFY complet — Phase 4 Scripting + Progression bouclée (97 tests)
Next action: `/paul:plan 05` (Phase 5 — Plugins + Characters)
Resume file: `.paul/phases/04-scripting-progression/04-03-SUMMARY.md`

---

*STATE.md — Updated after every significant action*
