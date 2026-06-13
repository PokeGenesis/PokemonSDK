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
| DATA-01→06 | Solution, SQLite 9 gens, formes, migrations, filtre, translations | ✅ Phase 1 |
| PLAT-01,03 | .NET 10 + DesktopGL cross-platform, chemins | ✅ Phase 1 |
| PLAT-02 | Builds Windows + Linux CI | ✅ Phase 3 |
| BATTLE-01→03,07 | Combat 1v1, formules, IA, config | ✅ Phase 2 |
| MAP-01→03 | Renderer HD, overworld, jour/nuit | ✅ Phase 3 |
| SCRIPT-01→03 | Lua sandbox, badges, save | ✅ Phase 4 |
| BATTLE-04→06 | Plugins Nuzlocke / Randomizer / Turbo | ✅ Phase 5 |
| CHAR-01→03 | Personnages, rivaux, antagonistes | ✅ Phase 5 |
| ADV-01→02 | Pokégear, objets terrain | ✅ Phase 5 |
| DX-01→02 | Asset pipeline, hot reload Lua | ✅ Phase 7 |
| DX-03→04 | NuGet publish, Sample project | ✅ Phase 8-9 |
| ADV-03→04 | TTS, Fakemons | ✅ Phase 6 |
| DX-05→06 | CLI pokeforge, Docs | 🔲 Phase 10-11 |
| BTLUI-01 | BattleScene UI — HP bars, sprites, move menu | 🔲 Phase 12 |
| BTLUI-02 | EXP + Level-up + Évolution | 🔲 Phase 13 |
| BTLUI-03 | Items en combat + Bag + PokéMart | 🔲 Phase 14 |
| UI-01→03 | PartyScene, PCScene, PokédexScene | 🔲 Phase 15 |
| QUEST-01→02 | QuestPlugin chaînable Lua + tracker UI | 🔲 Phase 16 |
| DATA-07 | Import PokeAPI 1010 Pokémon réels | 🔲 Phase 17 |
| SFX-01 | Audio complet — cries + SFX UI | 🔲 Phase 18 |
| MOD-01→03 | Méga / Z-moves / Dynamax plugins | 🔲 Phase 19 |
| DUNGEON-01→02 | DungeonPlugin — BSP floors + IDungeonMode | 🔲 Phase 20 |
| STREAM-01→02 | StreamerPlugin — Twitch/YouTube + HUD overlay | 🔲 Phase 21 |
| NET-01 | SDK.Network — combat online + trade + GTS | 🔲 Phase 22 |

→ Référence complète : `REQUIREMENTS.md`

---

## 6. ROADMAP — 6 HORIZONS

```
v0.1 → Phases 1+2+3+4       ✅ Moteur core jouable
v0.2 → + Phases 5+7+8+9     ✅ SDK distribuable NuGet
v0.3 → + Phases 6+10+11     🔲 CLI + docs + features avancées
v1.0 → + Phases 12→17       🔲 Moteur jouable complet (vraie v1.0)
v1.x → + Phases 18→21+      🔲 Plugin Era (v1.1 Audio → v1.4 Streamer → v1.5+)
v2.0 → + Phase 22           🔲 Réseau en ligne
```

| Phase | Description | Horizon | Statut |
|-------|-------------|---------|--------|
| 1 | SDK.Core + SDK.Data | v0.1 | ✅ |
| 2 | Battle Engine Core | v0.1 | ✅ |
| 3 | World Foundation | v0.1 | ✅ |
| 4 | Scripting + Progression | v0.1 🏁 | ✅ |
| 5 | Plugins + Characters | v0.2 | ✅ |
| 7 | Developer Experience | v0.2 | ✅ |
| 8 | NuGet Distribution | v0.2 | ✅ |
| 9 | Sample Project | v0.2 🏁 | ✅ |
| 6 | Advanced Systems | v0.3 | ✅ |
| 10 | CLI `pokeforge` | v0.3 | 🔲 |
| 11 | Documentation | v0.3 🏁 | 🔲 |
| 12 | BattleScene UI | v1.0 | 🔲 |
| 13 | EXP + Level-up + Évolution | v1.0 | 🔲 |
| 14 | Items + Bag + Shop | v1.0 | 🔲 |
| 15 | Party + PC + Pokédex UI | v1.0 | 🔲 |
| 16 | QuestPlugin | v1.0 | 🔲 |
| 17 | Real Data Pipeline (PokeAPI) | v1.0 🏁 | 🔲 |
| 18 | Audio complet (SFX + cries) | v1.1 | 🔲 |
| 19 | Mécaniques modernes (Méga/Z/Dynamax) | v1.2 | 🔲 |
| 20 | DungeonPlugin (Mystery Dungeon) | v1.3 | 🔲 |
| 21 | StreamerPlugin (Twitch/YouTube) | v1.4 🏁 | 🔲 |
| 22 | SDK.Network (combat online + trade) | v2.0 🏁 | 🔲 |

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
| D-16 | **Nommage sprites : `{dexid5}_{identifier}_{view}.png`** — convention stricte. Fakemons : `fk_{identifier}_{view}.png` dans `assets/sprites/fakemons/`. Regex SpriteValidator : `^(\d{5}_[a-z0-9-]+\|fk_[a-z0-9-]+)_(front\|back\|overworld\|portrait\|icon)\.png$` |
| D-17 | **SDK.Tools sans MonoGame** — tourne en CI headless |
| D-18 | **NuGet SemVer X.Y.Z** — X = couche fondamentale/breaking (ex: réseau = v2.0), Y = feature/plugin non-breaking (v0.2→v0.3→v1.x), Z = patch/hotfix. Jamais de breaking change sans bump X. |
| D-19 | **Sample consomme via NuGet** — jamais référence projet |
| D-20 | **CLI scaffold = sample stabilisé** — template embarqué |
| D-21 | **Docs = APIs stables uniquement** — pas de doc sur du WIP |
| D-22 | **6 locales obligatoires** — toute donnée traduite doit avoir en/es/fr/de/it/ja. Jamais moins. |
| D-23 | **Sprite `icon` = 32×32** — 5e view D-16 (party, PC box, Pokédex). Regex : `(front\|back\|overworld\|portrait\|icon)` |
| D-24 | **Cries = OGG Vorbis q8 mono 22050Hz** — `{dexid5}_{identifier}.ogg` dans `assets/sounds/cries/`. Shinies = même cry. |
| D-25 | **SixLabors.ImageSharp dans SDK.Tools** — atlas PNG lossless (plan 07-02). Jamais System.Drawing.Common. |

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
dotnet build PokemonSDK.slnx
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
| `PROJECT.md` | Vision, contraintes, 22 décisions clés |
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

---

## 11. RÈGLES STYLE — DOCUMENTATION MARKDOWN

**Applicables à TOUS les fichiers `.md` présents et futurs.**

### Em dashes INTERDITS ("—" et "—>")

Jamais d'em dash dans la prose, les titres, les tableaux.

| Contexte | Remplacement |
|----------|--------------|
| Introduction/définition : `Entité — description` | `:` → `Entité: description` |
| Continuation de clause : `est sûr — les lignes` | `,` → `est sûr, les lignes` |
| Parenthèse : `auto — pas besoin de redémarrer` | `(pas besoin de redémarrer)` |
| Flèche de liste : `**Passe monde** —> tilemap` | `:` → `**Passe monde**: tilemap` |

**Exception unique :** em dash à l'intérieur d'un bloc de code fencé (`` ` `` `` ` `` `` ` ``) représentant du vrai output CLI ou des commentaires C# — à ne pas modifier.

### Variables Docusaurus i18n

`{year}` n'est PAS interpolé dans `footer.json` i18n (Docusaurus remplace la string entière). Utiliser l'année en dur (ex: `2026`). Mettre à jour manuellement chaque année si nécessaire.

### Résumé anti-IA

Écrire sans : em dashes, "simplement", "il convient de noter", tournures passives excessives. Préférer les phrases courtes et directes.
