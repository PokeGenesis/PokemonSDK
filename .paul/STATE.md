# Project State

## Project Reference

See: PROJECT.md | REQUIREMENTS.md | ROADMAP.md | .claude/ARCHITECTURE.md

**Core value:** Brancher le SDK → moteur de combat + DB multilingue + quêtes, sans réimplémenter les règles de base.

**Current focus:** Phase 12 BattleScene UI ✅ COMPLÈTE — BTLUI-01 livré. HP bars + MoveMenu + StatusIcon + BattleEnd overlay + log de combat + flux complet overworld→battle→overworld. Prochaine étape : v1.0 Phase 13 (EXP + Level-up + Évolution).

## Current Position

Milestone: v1.0 Moteur Complet
Phase: 12 (BattleScene UI) — COMPLÈTE ✅
Plan: 12-01 ✅ | 12-02 ✅ | 12-03 ✅ | 12-04 ✅ | 12-05 ✅
Status: APPLY ✅ UNIFY ✅ — BTLUI-01 DONE. Prochaine phase : 13 (EXP + Level-up + Évolution).
Last activity: 2026-06-14 — Phase 12 complète. Checkpoint human-verify approuvé. 5 commits Wave 3.

Progress:

- Milestone v0.1: [██████████] 100% ✅ (Phases 1→4 complètes, 2026-06-05)
- Milestone v0.2: [██████████] 100% ✅ (Phase 5 ✅ — Phase 7 ✅ — Phase 8 ✅ — Phase 9 ✅)
- Milestone v0.3: [██████████] 100% ✅ (Phase 6 ✅ — Phase 10 ✅ — Phase 11 ✅)
- Phase 1: [██████████] 100% ✅
- Phase 2: [██████████] 100% ✅
- Phase 3: [██████████] 100% ✅
- Phase 4: [██████████] 100% ✅
- Phase 5: [██████████] 100% ✅ (05-01 ✅ 05-02 ✅ 05-03 ✅)
- Phase 6: [██████████] 100% ✅ (06-01 ✅ 06-04 ✅ 06-02 ✅ 06-03 ✅ 06-05 ✅)
- Phase 7: [██████████] 100% ✅ (07-01 ✅ 07-02 ✅ 07-03 ✅ 07-04 ✅)
- Phase 8: [██████████] 100% ✅ (08-01 ✅ 08-02 ✅ 08-03 ✅ 08-04 ✅)
- Phase 9: [██████████] 100% ✅ (09-01 ✅ 09-02 ✅ 09-03 ✅ 09-04 ✅)
- Phase 10: [██████████] 100% ✅ (10-01 ✅ 10-02 ✅ 10-03 ✅)
- Phase 11: [██████████] 100% ✅ (11-01 ✅ — 11-02 ✅ — 11-03 ✅ — 11-04 ✅)
- Phase 12: [██████████] 100% ✅ (12-01 ✅ 12-02 ✅ 12-03 ✅ 12-04 ✅ 12-05 ✅)

## Loop Position

Current loop state:

```text
PLAN ──▶ APPLY ──▶ UNIFY
  ✓        ✓        ✓     [Phase 12 complète — BTLUI-01 DONE]
```

Phase 6 plans : 06-01 ✅ | 06-04 ✅ | 06-02 ✅ | 06-03 ✅ | 06-05 ✅
Phase 11 plans : 11-01 ✅ | 11-02 ✅ | 11-03 ✅ | 11-04 ✅
Phase 12 plans : 12-01 (Wave 1) | 12-02 (Wave 1) | 12-03 (Wave 2) | 12-04 (Wave 2) | 12-05 (Wave 3 + human-verify)

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
| Créer compte NuGet + réserver PokéForge.SDK | Init | Avant Phase 8 | ✅ Org PokeGenesis créée, API key configurée, demande namespace envoyée par mail. |
| FluentAssertions v8 licence Xceed (commercial) | Plan 01-01 | Avant Phase 8 | ✅ Résolu — FA v8 gratuit pour open-source MIT. v8.10.0 standardisé sur 8/8 projets tests. FA test-only, ne transite pas vers les consumers NuGet. |
| Translations Move manquantes (D-22) | Phase 2 | Plan 03-01 | ✅ Résolu — SeedMoveTranslations (15×6=90 rows), BattleTranslationsD22Tests passe. |
| Translations Ability manquantes (D-22) | Phase 2 | Plan 03-01 | ✅ Résolu — SeedAbilityTranslations (6×6=36 rows), BattleTranslationsD22Tests passe. |
| D-22 Characters (Plan 05-03) — noms à translater | Phase 5 | Plan 05-03 | ✅ Résolu — CharacterDataSeeder (5×6=30 rows Character + 1×6=6 rows VillainGroup), CharacterTranslationsD22Tests 3/3 verts. |
| SixLabors.ImageSharp 2.1.9 CVEs (GHSA-2cmq-823j-5qj8 high + GHSA-rxmq-m78w-7wmc moderate) | Plan 07-02 | Avant Phase 8 NuGet | ✅ Résolu — ImageSharp 4.0.0 + sixlabors.lic (gitignored). 0 CVE HIGH/MODERATE sur tous les projets. |

Décisions émergentes (Plan 05-03 — characters) :

- `TurboPlugin.TextSpeedMultiplier` exposé comme property float — renderers lisent IsActive + TextSpeedMultiplier, logique interne zéro
- `VillainMemberConfiguration` minimal (table + HasKey) — FK déjà déclarés dans CharacterConfiguration et VillainGroupConfiguration
- `Character.Role` = string libre — pas d'enum SDK.Core pour éviter breaking changes futurs
- EntityType "Character" / "VillainGroup" PascalCase — cohérent avec Badge, Move, Ability

Décisions émergentes (Plan 05-02 — plugins structure) :

- `src/plugins/` sous-dossier physique pour tous les `SDK.Plugins.*` — convention définitive à partir de 2026-06-06
- Prochain plugin → `src/plugins/SDK.Plugins.{Nom}/` + tests dans `tests/SDK.Plugins.{Nom}.Tests/` (flat)
- csproj plugin : refs SDK.Core/SDK.Battle avec `../../` (deux niveaux depuis `src/plugins/SDK.Plugins.X/`)
- csproj test plugin : refs `../../src/plugins/SDK.Plugins.{Nom}/`
- slnx : dossier solution `/src/` contient `src/plugins/SDK.Plugins.*/` (pas de dossier solution séparé)
- Performance : aucun impact. Runtime coût = plugins Register()-és uniquement, pas plugins installés/existants.

Décisions émergentes (Plan 08-02 — licence/CVE) :

- SixLabors.ImageSharp 4.0.0 retenu — Six Labors Split License gratuite open-source MIT. `sixlabors.lic` dans `src/SDK.Tools/` (gitignored). Contributeurs : demander clé sur `https://licensing.sixlabors.com/`. CI : secret `SIXLABORS_LICENSE_KEY` + `-p:SixLaborsLicenseKey="$SIXLABORS_LICENSE_KEY"`. CVEs GHSA-2cmq-823j-5qj8 (HIGH) + GHSA-rxmq-m78w-7wmc (MODERATE) résolus. D-25 mis à jour.
- FluentAssertions v8.10.0 retenu — Xceed License gratuite pour projets open-source MIT. Test-only, 0 impact consommateurs NuGet. Standardisé sur 8/8 projets tests.

Décisions émergentes (Plan 07-02 — atlas pipeline) :

- `SixLabors.ImageSharp 2.1.9` (Apache 2.0) dans SDK.Tools — v4.0.0 impose licence au MSBuild build (résolue en 08-02 via sixlabors.lic). D-25 confirmé.
- EF migrations : `--startup-project src/SDK.Data` définitif — SDK.MonoGame manque EF Design package (PrivateAssets="all" non transitif)
- `AtlasPacker` tests : vrais PNG via `new Image<Rgba32>(w,h).SaveAsPng` — `Image.Load()` valide format complet (headers synthétiques 26 bytes invalides)
- `SqliteSyncer` test fixture : temp file `.db` (pas `:memory:`) — constructeur prend `dbPath` string, pas DbContext injectable directement
- CLI pipeline `asset-sync` : filter ERROR → pack OK+WARN → sync. Exit code 1 si au moins 1 ERROR.

Décisions émergentes (Plan 07-04 — LuaConsole + MGCB) :

- `DefaultFont.spritefont` : DejaVu Sans Mono 16pt — installée nativement sur Ubuntu/Debian, CI headless sans setup TTF supplémentaire
- Shaders xBR.fx + DayNight.fx commentés dans `Content.mgcb` — MGFXC requiert wine64, Ubuntu 24.04 t64 transition casse les dépendances apt ; PointClamp fallback actif
- `dotnet-mgcb 3.8.4.1` pinned dans `.config/dotnet-tools.json` — `dotnet tool restore` avant build CI
- `LuaConsole` uniquement `#if DEBUG` — toggle tilde, buffer TextInput, history circulaire, Draw SpriteBatch overlay
- `Game1` fields : `_defaultFont SpriteFont`, `_prevKeyState KeyboardState`, `_luaConsole LuaConsole` (DEBUG only)
- `Draw()` réels : LuaErrorOverlay + DialogueBox + LuaConsole via SpriteBatch — tous les stubs no-op remplacés

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

Décisions émergentes (Phase 6) :

- `TtsApi` méthodes en minuscules (`speak`, `stop`, `is_speaking`) — MoonSharp mappe les noms C# sensible à la casse vers identifiants Lua ; PascalCase forcerait `sdk.tts.Speak()` en Lua
- `SdkGlobals public sealed class { public TtsApi tts }` (property minuscule) — seul pattern pour namespaces Lua imbriqués (`sdk.tts.speak()`). Réutilisable : `sdk.items`, `sdk.party` (Phase 14-15)
- `UserData.RegisterType<TtsApi>()` obligatoire avant `RegisterApi("sdk", SdkGlobals)` — MoonSharp ne réfléchit pas automatiquement les types CLR imbriqués en SoftSandbox
- `InvocationContext` (`ctx.ParseResult.GetValueForOption`) pour commandes CLI >8 options — `SetHandler` typé limité à 8 paramètres en System.CommandLine 2.0.0-beta4
- `FakemonPartsCatalog.Scan` filtre par extension `.png` uniquement (pas validation format) — tests peuvent utiliser `new byte[1]` comme fake PNG
- E-05 : MSB3492 `CoreCompileInputs.cache` stale sur .NET 10.0.108 — fix : `rm obj/Release/net10.0/*.csproj.CoreCompileInputs.cache`

Décisions émergentes (Phase 10) :

- `<IsPackable>true</IsPackable>` obligatoire sur tout `OutputType=Exe` ciblant `net10.0` avec `PackAsTool=true` — `.NET 10.0.108` force `IsPackable=false` sinon (`dotnet pack` exit 0 sans produire de .nupkg)
- `<PackageType Include="DotnetTool" />` ItemGroup redondant — `PackAsTool=true` l'insère automatiquement
- `<PackageReadmeFile>` seul suffit pour inclure README dans le nupkg — un `<None Include>` explicit duplique → NU5118
- Suite tests CLI : 13 (10 existants + 3 DoctorCommand), pas 11 comme estimé dans le plan

## Session Continuity

Last session: 2026-06-14
Stopped at: Phase 12 COMPLÈTE. Plan 12-05 APPLY+UNIFY. Checkpoint human-verify approuvé. 5 commits Wave 3 (SHA e2651d4).
Next action: Merger feature/phase12-battlescene-ui → dev → staging → main (E-04 sync protocol), puis démarrer Phase 13.

Resume file: `.paul/phases/12-battlescene-ui/12-01-PLAN.md`

---

*STATE.md — Updated after every significant action*
