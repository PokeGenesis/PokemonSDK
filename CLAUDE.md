# CLAUDE.md — PokemonSDK

> Lu automatiquement à chaque session. Détails dans `.claude/`.
> **Mise à jour :** 2026-06-01 — Redémarrage from scratch .NET 10

---

## 1. PROJET

**PokemonSDK** — SDK open-source C# / .NET 10 pour fan-games Pokémon. Moteur de données SQLite, battle engine, runtime MonoGame, plugins modulaires. Repo éditeur séparé : **PokeForge-Editor** (Avalonia).

**Core Value :** Brancher le SDK → moteur de combat + DB multilingue + quêtes, sans réimplémenter les règles de base.

---

## 2. STACK

| Couche | Tech | Notes |
|--------|------|-------|
| Runtime | **.NET 10** | `net10.0` partout |
| ORM | **EF Core 10 + SQLite** | Migrations obligatoires, jamais Dapper seul |
| Rendu | **MonoGame.Framework.DesktopGL** | OpenGL cross-platform, WindowsDX interdit |
| Tilemaps | **MonoGame.Extended.Tiled** | Import `.tmx` natif |
| Shaders | **MojoShader** (intégré) | HLSL→GLSL auto. xBR, Bloom, DayNight, PaletteSwap |
| Scripting | **MoonSharp 2.0.0** | Pure C#, `Preset_SoftSandbox` |
| Tests | **xUnit + FluentAssertions + Moq** | Coverage via `coverlet.collector` |
| DI | **MS.Extensions.DependencyInjection** | Composition root dans SDK.MonoGame uniquement |
| Logs | **Serilog** | Structured, sink fichier |
| JSON | **System.Text.Json** | Intégré .NET 10, jamais Newtonsoft |

**Packages INTERDITS :** `Newtonsoft.Json` · `AutoMapper` · `MediatR` · `NLua/KeraLua` · `MonoGame.WindowsDX` · `EF Core < 10`

→ Détails stack et rationales : `.claude/ARCHITECTURE.md`

---

## 3. ARCHITECTURE

→ Arborescence complète + règles de dépendances : `.claude/ARCHITECTURE.md`

**Règle résumée (NON NÉGOCIABLE) :**
```
SDK.Core      ← ZÉRO dépendance NuGet externe
SDK.Data      ← SDK.Core + EF Core
SDK.Battle    ← SDK.Core uniquement
SDK.Scripting ← SDK.Core + MoonSharp
SDK.Plugins.* ← SDK.Core + SDK.Battle uniquement
SDK.Tools     ← SDK.Core + SDK.Data + SDK.Scripting (jamais MonoGame)
SDK.MonoGame  ← SDK.Core + MonoGame + SDK.Battle + SDK.Scripting (via Func factory)
```

---

## 4. ÉTAT — REDÉMARRAGE .NET 10

**Validé conceptuellement (ancienne base .NET 8) :**
- Phase 1 : schéma SQLite, EF Core migrations, `translations` centrale, filtres génération
- Phase 2 : BattleState immuable, IDamageFormula ×2, IDifficultyMode ×2, Sleep/Freeze no-skip (D-11)
- Phase 4 : MoonSharp SoftSandbox, GameState, SaveSystem JSON, badges

**Pas encore fait :** Phase 3 (World/MAP), Phases 5→11

---

## 5. REQUIREMENTS — STATUTS

| ID | Description | Statut |
|----|-------------|--------|
| DATA-01→06 | Solution, SQLite 9 gens, formes, migrations, filtre, translations | ✅ À recréer |
| PLAT-01,03 | .NET 10 + DesktopGL cross-platform, chemins | ✅ À recréer |
| PLAT-02 | Builds Windows + Linux CI | 🔲 Phase 3 |
| BATTLE-01→03,07 | Combat 1v1, formules, IA, config | ✅ À recréer |
| BATTLE-04→06 | Plugins Nuzlocke / Randomizer / Turbo | 🔲 Phase 5 |
| MAP-01→03 | Renderer HD, overworld, jour/nuit | 🔲 Phase 3 |
| SCRIPT-01→03 | Lua sandbox, badges, save | ✅ À recréer |
| CHAR-01→03 | Personnages, rivaux, antagonistes | 🔲 Phase 5 |
| ADV-01→02 | Pokégear, objets terrain | 🔲 Phase 5 |
| ADV-03→04 | TTS, Fakemons | 🔲 Phase 6 |
| DX-01→02 | Asset pipeline, hot reload Lua | 🔲 Phase 7 |
| DX-03→04 | NuGet publish, Sample project | 🔲 Phase 8-9 |
| DX-05→06 | CLI pokeforge, Docs | 🔲 Phase 10-11 |

→ Référence complète : `REQUIREMENTS.md`

---

## 6. ROADMAP — 3 HORIZONS

```
v0.1 → Phases 1+2+3+4   (~3-4 mois)  Moteur core jouable
v1.0 → + Phases 5+7+8+9 (~3-4 mois)  SDK distribuable NuGet
v2.0 → + Phases 6+10+11 (post v1.0)  CLI + docs + features avancées
```

| Phase | Description | Horizon |
|-------|-------------|---------|
| 1 | SDK.Core + SDK.Data | v0.1 |
| 2 | Battle Engine Core | v0.1 |
| 3 | World Foundation | v0.1 |
| 4 | Scripting + Progression | v0.1 🏁 |
| 5 | Plugins + Characters | v1.0 |
| 7 | Developer Experience | v1.0 |
| 8 | NuGet Distribution | v1.0 |
| 9 | Sample Project | v1.0 🏁 |
| 6 | Advanced Systems | v2.0 |
| 10 | CLI `pokeforge` | v2.0 |
| 11 | Documentation | v2.0 🏁 |

→ Détails waves + critères de succès : `ROADMAP.md`

---

## 7. DÉCISIONS FIGÉES

| # | Décision |
|---|----------|
| D-01 | **.NET 10** — pas de retour en arrière |
| D-02 | **MonoGame.DesktopGL** — jamais WindowsDX |
| D-03 | **EF Core + migrations** — jamais Dapper seul |
| D-04 | **MoonSharp Preset_SoftSandbox** — jamais NLua/KeraLua |
| D-05 | **BattleState immuable** — record + `with` uniquement |
| D-06 | **SDK.MonoGame ne référence pas SDK.Scripting** — Func factory dans Game1 |
| D-07 | **Table `translations` centrale** — jamais colonnes `name_fr` sur les entités |
| D-08 | **Deux repos distincts** — SDK / PokeForge-Editor |
| D-09 | **`generation` INT NOT NULL** — sur toutes entités concernées |
| D-10 | **System.Text.Json** — jamais Newtonsoft.Json |
| D-11 | **Sleep/Freeze ne sautent pas les tours** — correction critique validée |
| D-12 | **`GameState.Flags = Dictionary<string, JsonElement>`** |
| D-13 | **Nuzlocke/Randomizer/Turbo = plugins** — jamais des modes hardcodés |
| D-14 | **Résolution interne 480×270 → ×4 → 1920×1080** — figée, ne pas changer |
| D-15 | **xBR shader d'upscaling** — intégré dans RenderPipeline |
| D-16 | **Nommage sprites : `{dexid5}_{identifier}_{view}.png`** — convention stricte |
| D-17 | **SDK.Tools sans MonoGame** — tourne en CI headless |
| D-18 | **NuGet SemVer strict** — pas de breaking change sans bump majeur |
| D-19 | **Sample consomme via NuGet** — jamais référence projet |
| D-20 | **CLI scaffold = sample stabilisé** — template embarqué |
| D-21 | **Docs = APIs stables uniquement** — pas de doc sur du WIP |

---

## 8. WORKFLOW (gstack + PAUL + Superpowers)

```
gstack      → DÉCISION  : archi, choix techniques, review
PAUL        → CONTEXTE  : STATE.md, plans atomiques, réconciliation
Superpowers → EXÉCUTION : TDD, implémentation, vérification
```

**Séquence par session :**
```
/paul:init           → charge PROJECT.md + STATE.md
/office-hours        → (gstack) brainstorm / archi
/paul:plan           → plan atomique validé
[Superpowers]        → TDD + implémentation
/paul:unify          → STATE.md mis à jour
/review              → (gstack) revue qualité
```

**Règles :** gstack et Superpowers jamais simultanément · `dotnet build + test` avant de clore un plan · STATE.md géré par PAUL uniquement

---

## 9. COMMANDES UTILES

```bash
dotnet build PokemonSDK.sln
dotnet test tests/ --collect:"XPlat Code Coverage"
dotnet ef migrations add InitialCreate --project src/SDK.Data --startup-project src/SDK.MonoGame
dotnet ef database update --project src/SDK.Data --startup-project src/SDK.MonoGame
dotnet run --project src/SDK.MonoGame --configuration Debug
dotnet run --project src/SDK.MonoGame -- --headless
dotnet list src/SDK.Core/SDK.Core.csproj package  # doit être vide
sqlite3 data/PokemonSDK.db ".tables"
git checkout dev && git pull && git checkout -b feature/nom-feature
```

---

## 10. FICHIERS DÉTAILLÉS

| Fichier | Contenu |
|---------|---------|
| `.claude/ARCHITECTURE.md` | Arborescence complète, règles dépendances, schéma SQLite |
| `.claude/CONVENTIONS.md` | Conventions code C#/Lua, BattleState, MonoGame perf, tests |
| `.claude/RENDERING.md` | Pipeline HD, shaders, tailles sprites, 3 passes Draw() |
| `.claude/PLUGINS.md` | IBattlePlugin, PluginRegistry, NuzlockePlugin exemple |
| `.claude/DX.md` | Asset pipeline, SpriteValidator, hot reload Lua, console REPL |
| `.claude/CICD.md` | GitHub Actions workflows (ci, dev, staging, prod, publish) |
| `ROADMAP.md` | Phases 1→11, waves, critères de succès |
| `REQUIREMENTS.md` | 34 requirements, traceability complète |
| `PROJECT.md` | Vision, contraintes, 21 décisions clés |
| `STATE.md` | État courant (géré par PAUL) |

---

*Généré 2026-06-01 — .NET 10 fresh start*

## Skill routing

When the user's request matches an available skill, invoke it via the Skill tool. When in doubt, invoke the skill.

Key routing rules:
- Product ideas/brainstorming → invoke /office-hours
- Strategy/scope → invoke /plan-ceo-review
- Architecture → invoke /plan-eng-review
- Design system/plan review → invoke /design-consultation or /plan-design-review
- Full review pipeline → invoke /autoplan
- Bugs/errors → invoke /investigate
- QA/testing site behavior → invoke /qa or /qa-only
- Code review/diff check → invoke /review
- Visual polish → invoke /design-review
- Ship/deploy/PR → invoke /ship or /land-and-deploy
- Save progress → invoke /context-save
- Resume context → invoke /context-restore
- Author a backlog-ready spec/issue → invoke /spec
