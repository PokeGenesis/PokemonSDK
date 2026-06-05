# Roadmap: PokemonSDK

## Overview

PokemonSDK est construit en **3 horizons** qui mènent d'un moteur core fonctionnel à un SDK communautaire incontournable. La logique est simple : faire tourner quelque chose rapidement (v0.1), le rendre distribuable et utilisable par des makers externes (v1.0), puis le polisher jusqu'à devenir la référence de l'écosystème (v2.0).

## Timeline

| Horizon | Phases | Durée estimée | Résultat |
|---------|--------|---------------|---------|
| **v0.1** "Proof of Concept" | 1 + 2 + 3 + 4 | ~3-4 mois | Moteur core jouable, release GitHub |
| **v1.0** "Release" | 5 + 7 + 8 + 9 | ~3-4 mois de plus | SDK distribuable via NuGet, maker peut créer son jeu |
| **v2.0** "Incontournable" | 6 + 10 + 11 | Après feedback v1.0 | CLI + docs + features avancées |

## Phase Numbering

- Integer phases (1, 2, 3…): Planned milestone work
- Phases suivent l'ordre des horizons : 1→4 (v0.1), puis 5+7+8+9 (v1.0), puis 6+10+11 (v2.0)
- Phase 6 délibérément après Phase 9 : features avancées validées par le feedback communauté v1.0

---

## HORIZON 1 — v0.1 "Proof of Concept"

> Un maker peut écrire un script Lua, déclencher un combat 1v1 sur une map jouable, sauvegarder sa progression.

### Phase 1: SDK.Core + SDK.Data

**Goal**: Un développeur peut créer un Pokémon, le persister en SQLite, le requêter avec un filtre génération, et lire son nom en 5 langues — SDK.Core sans aucune dépendance externe.
**Mode:** mvp (Walking Skeleton)
**Depends on**: Nothing (first phase)
**Requirements**: DATA-01, DATA-02, DATA-03, DATA-04, DATA-05, DATA-06, PLAT-01, PLAT-03

**Success Criteria:**
1. `dotnet ef database update` fonctionne depuis un clone propre — zéro SQL manuel
2. Un Pokémon peut être créé en C# et persisté via `PokemonDbContext`, puis requêté par ID
3. `GetAllByGeneration(1)` retourne uniquement les Pokémon Gen I — vérifié avec dataset seedé
4. `GetTranslations("fr")` retourne le bon nom français depuis la table `translations`
5. SDK.Core : zéro référence `MonoGame.*`, `MoonSharp.*`, `Avalonia.*` — vérifié en build

**Plans:** 4/4 plans
- Wave 1: Solution scaffold .NET 10 + SDK.Core domain models (PokemonSpecies, PokemonForm, Translation, enums) + CoreDependencyTests
- Wave 2: EF Core 10 + PokemonDbContext + Fluent API + Migration 001 (InitialCreate) + SqliteTestFixture
- Wave 3: DbContextExtensions (GetByGeneration/GetTranslations) + DataSeeder + seed command CLI
- Wave 4: End-to-end SDK test + PLAT-01/03 cross-target scan

**Status:** ✅ Complet (Phase 1 — 2026-06-05)

---

### Phase 2: Battle Engine Core

**Goal**: Un combat 1v1 headless tourne de start à KO avec données DB réelles, IA configurable, formules par génération.
**Mode:** mvp
**Depends on**: Phase 1
**Requirements**: BATTLE-01, BATTLE-02, BATTLE-03, BATTLE-07

**Success Criteria:**
1. Combat 1v1 complet en test unitaire : move selection, damage, PP, statuts (brûlure/paralysie/poison/sommeil/gel/confusion), KO — tout fonctionne
2. StoryMode et HardMode produisent des décisions IA différentes sur même matchup — vérifié sur 10+ tours
3. BattleConfig (disable items, disable flee, weather) modifie le comportement du combat
4. Gen1 et Gen4+ produisent des dégâts différents sur même move spécial (physique/spécial split)

**Plans:** 4/4 plans
- Wave 1: SDK.Core battle models (Move, Learnset, Ability, TypeEffectiveness, BattleConfig, enums)
- Wave 2: SDK.Data Migration 002 (types, moves, type chart 18×18, abilities, learnsets) + BattleDataSeeder
- Wave 3: SDK.Battle (BattleState immuable, IDamageFormula × 2, IDifficultyMode × 2, BattleEngine turn loop)
- Wave 4: SDK.Battle.Tests (6 fichiers : loop, damage, AI, config, status, switch)

**Correction critique (D-11):** Sleep/Freeze ne sautent PAS les tours — validé ancienne base.

**Status:** ✅ Complet (Phase 2 — 2026-06-05)

---

### Phase 3: World Foundation

**Goal**: Un joueur spawne sur une tilemap MonoGame, se déplace avec collision, déclenche une rencontre sauvage qui ouvre le battle engine.
**Mode:** mvp
**Depends on**: Phase 2
**Requirements**: MAP-01, MAP-02, MAP-03, PLAT-02

**Success Criteria:**
1. Renderer MonoGame : tilemap 480×270 interne → upscale xBR ×4 → 1920×1080, sprites 64x64 affichés
2. Joueur : mouvement 4 directions, collision tiles, warp tiles entre zones
3. Herbes hautes → rencontre sauvage selon tables d'heure et météo
4. Cycle jour/nuit et météo (pluie, neige, soleil) visibles à l'écran et modifient les rencontres
5. Build Windows + build Linux fonctionnels (PLAT-02)

**Plans:** 4/4 plans
- Wave 1: Migration 003 (encounter_zones, encounter_entries) + SDK.Core world primitives (TimeOfDay, Direction, IInputProvider, WorldConfig, EncounterZone)
- Wave 2: EncounterSystem + RealTimeClock + InternalClock + WeatherSystem + tests unitaires
- Wave 3: Game1 + WorldSystem + PlayerSystem + TransitionSystem + .tmx zones + RenderPipeline (xBR shader) + overlays jour/nuit
- Wave 4: HeadlessSmokeTester + CI GitHub Actions matrix (ubuntu-latest + windows-latest)

**Status:** ✅ Complet (Phase 3 — 2026-06-05)

---

### Phase 4: Scripting + Progression

**Goal**: Un script Lua déclenché sur NPC interaction modifie GameState, attribue un badge, et save/load restaure l'état complet.
**Mode:** mvp
**Depends on**: Phase 3
**Requirements**: SCRIPT-01, SCRIPT-02, SCRIPT-03

**Success Criteria:**
1. Script Lua en SoftSandbox : `os.exit()` et `io.open()` lèvent `ScriptRuntimeException`
2. Défaite Gym Leader → badge stocké dans GameState → route débloquée
3. Auto-save sur transition de zone
4. Load restaure position, équipe, badges, flags, inventaire — identique au pré-save

**Plans:** 3/3 plans
- Wave 1: SDK.Scripting — IScriptEngine + LuaScriptEngine (Preset_SoftSandbox) + GameState + coroutines
- Wave 2: Migration 004 (trainers/trainer_pokemon/badges) + seed gym_leader_1 + Lua badge/flag API + OnNpcInteraction
- Wave 3: ISaveSystem + SaveSystem JSON + DialogueBox UI + scripts Lua prod + Game1 wiring + auto-save

**Status:** ✅ Complet (Phase 4 — 2026-06-05) — 97 tests

> **🏁 TAG v0.1** — release GitHub avec binaires Windows + Linux après Phase 4 validée

---

## HORIZON 2 — v1.0 "Release"

> Un maker externe peut créer son fan-game sans cloner le repo, sans comprendre l'architecture interne, sans fournir ses assets dès le premier jour.

### Phase 5: Plugins + Characters

**Goal**: Nuzlocke/Randomizer/Turbo activables/désactivables via PluginRegistry, création dresseur/rival, antagonistes bloquant progression, Pokégear, objets terrain.
**Mode:** mvp
**Depends on**: Phase 4
**Requirements**: BATTLE-04, BATTLE-05, BATTLE-06, CHAR-01, CHAR-02, CHAR-03, ADV-01, ADV-02

**Success Criteria:**
1. `PluginRegistry.Register(new NuzlockePlugin())` → plugin actif. `PluginRegistry.Clear()` → désactivé. Moteur de combat inchangé dans les deux cas
2. Même seed Randomizer → même résultat garanti (species, trainers, moves, items, types)
3. Palette layer cosmétique indépendant — changer skin ne modifie pas cheveux
4. Antagonistes bloquent une route jusqu'à défaite, script Lua valide le déblocage
5. Pokégear : carte monde, lecteur musique, journal rencontres — 3 écrans navigables
6. Objet terrain en inventaire → action terrain débloquée (eau, arbre) sans HM

**Plans:** TBD — à planifier via `/paul:plan` en début de phase.

**Status:** 🔲 Not started

---

### Phase 7: Developer Experience — Asset Pipeline + Hot Reload

**Goal**: Asset pipeline automatique + SpriteValidator, hot reload Lua first-class + console REPL debug — la friction maker disparaît.
**Mode:** mvp
**Depends on**: Phase 4 (hot reload Lua) + Phase 5 (asset pipeline validé sur plugins)
**Requirements**: DX-01, DX-02

**Success Criteria:**
1. PNG déposés → `pokeforge asset-sync` → atlas + MGCB config + SQLite asset_key sync, sans intervention
2. `SpriteValidator` : taille incorrecte → ERROR bloquant avec suggestion. Nommage non conforme → WARNING
3. Modification `.lua` → rechargement en <500ms sans redémarrer
4. Erreur Lua → overlay ingame : fichier + ligne + message. Jamais un crash silencieux
5. `asset-validate` en CI : exit code 1 si ERROR, rapport `import.json` machine-readable

**Plans:** 4/4 plans
- Wave 1: `SDK.Tools.AssetPipeline` — scanner PNG + SpriteValidator (tailles 48×48/96×96/16×16/128×128, nommage `{dexid5}_{id}_{view}.png`, alpha) + atlas packer + MGCB config auto
- Wave 2: SQLite sync — sprite → `pokemon_forms.asset_key` auto-mappé + rapport `import.json`
- Wave 3: Hot reload Lua — `FileSystemWatcher` sur `data/scripts/` + `LuaScriptEngine.Reload(path)` + `LuaErrorOverlay` ingame (`#if DEBUG`)
- Wave 4: Console Lua REPL — toggle `~`, historique commandes, autocomplete API exposée, output coloré

**Status:** 🔲 Not started

---

### Phase 8: NuGet Distribution

**Goal**: SDK et plugins publiés sur NuGet.org, installables en une commande, CI publish automatique sur tag `v*`.
**Mode:** mvp
**Depends on**: Phase 5 (SDK stable avant publication)
**Requirements**: DX-03

**Success Criteria:**
1. `dotnet add package PokéForge.SDK` → SDK.Core + SDK.Data + SDK.Battle + SDK.Scripting installés
2. `dotnet add package PokéForge.Plugins.Nuzlocke` → plugin sans dépendance cachée
3. CI publie sur NuGet à chaque tag `v*` via secret `NUGET_API_KEY`
4. Smoke test consommateur externe réussi (projet qui installe via NuGet uniquement, zéro référence repo)

**Plans:** 4/4 plans
- Wave 1: `.csproj` NuGet metadata — `PackageId`, `Version` (SemVer), `Description`, `Authors`, `PackageTags`, icon
- Wave 2: `publish.yml` GitHub Actions — tag `v*` → build Release → `dotnet pack` → `dotnet nuget push`
- Wave 3: Packages plugins séparés + vérification dépendances transitives correctes
- Wave 4: `ConsumerTest/` — installe via NuGet uniquement, combat headless réussi

**Status:** 🔲 Not started

---

### Phase 9: Sample Project

**Goal**: `samples/StarterGame/` — jeu minimal jouable, assets placeholder CC0, commenté, intégré au CI.
**Mode:** mvp
**Depends on**: Phase 8 (sample consomme via NuGet, pas référence projet)
**Requirements**: DX-04

**Success Criteria:**
1. `dotnet restore && dotnet run` → jeu jouable immédiatement, zéro configuration
2. Démontre : map, combat 1v1, dialogue NPC, badge, save/load, cycle jour/nuit, NuzlockePlugin en option
3. Assets placeholder CC0 inclus (Kenney.nl sprites, tiles, BGM FreeMusicArchive) — zéro asset à fournir
4. Commentaires `// SDK: explication` sur chaque système clé — code pédagogique
5. Build + run headless dans le CI à chaque PR

**Plans:** 4/4 plans
- Wave 1: Scaffold `samples/StarterGame/` — csproj NuGet-only, assets placeholder CC0, MGCB config, `dotnet run` fonctionnel
- Wave 2: Overworld minimal — map 20×15 tiles, joueur, collision, warp, NPC + dialogue Lua commenté
- Wave 3: Combat intégré — rencontre sauvage, 1v1 complet, fuite, KO, XP, commenté
- Wave 4: Progression + plugin — gym leader scriptable, badge, NuzlockePlugin activable, save/load

> **🏁 TAG v1.0** — release GitHub majeure après Phase 9 validée. Annonce communautaire (Reddit PokéCommunity, Eevee Expo, Discord)

---

## HORIZON 3 — v2.0 "Incontournable"

> PokéForge est la référence — installable en 30 secondes, documenté, avec des features que personne d'autre n'a.
> **Planifier après retours communauté v1.0 — ne pas coder avant.**

### Phase 6: Advanced Systems

**Goal**: TTS voix off asynchrone, Fakemon assemblage 2D + export SQLite, smoke test multi-plateforme complet.
**Mode:** mvp
**Depends on**: Phase 5
**Requirements**: ADV-03, ADV-04

**Success Criteria:**
1. KO Gym Leader → TTS voix off asynchrone sans bloquer le game loop
2. Fakemon assemblé (tête + corps + membre) avec palette swap par couche indépendant
3. Export Fakemon → row complète `pokemon_species` + `pokemon_forms` en SQLite
4. Binaires Windows + Linux passent smoke test complet (launch → DB → combat headless → exit 0)

**Plans:** TBD — à définir après retours communauté v1.0.

**Status:** 🔲 Post-v1.0

---

### Phase 10: CLI `pokeforge`

**Goal**: `pokeforge new mon-jeu` génère un projet prêt à lancer en une commande.
**Mode:** mvp
**Depends on**: Phase 9 (template = sample stabilisé)
**Requirements**: DX-05

**Success Criteria:**
1. `dotnet tool install -g pokeforge` → CLI installé globalement
2. `pokeforge new mon-jeu` → `dotnet run` immédiat, zéro configuration
3. `pokeforge asset-sync`, `seed --gen 1`, `build --platform linux-x64`, `doctor` — toutes fonctionnelles

**Plans:** 4/4 plans
- Wave 1: `SDK.Cli` projet dotnet tool + commande `new` avec template embarqué (sample Phase 9)
- Wave 2: Commandes `asset-sync` + `seed` — wrappers SDK.Tools + PokeAPI seeder
- Wave 3: Commande `build` multi-plateforme (windows-x64, linux-x64) + commande `doctor`
- Wave 4: Publication NuGet tool + `pokeforge` disponible globalement

**Status:** 🔲 Post-v1.0

---

### Phase 11: Documentation & Tutoriel

**Goal**: Site docs public, tutoriel "premier jeu en 30 minutes", API reference auto-générée, guide migration Essentials/PSDK.
**Mode:** mvp
**Depends on**: Phase 10 (docs le tutoriel `pokeforge new`)
**Requirements**: DX-06

**Success Criteria:**
1. Site accessible publiquement (GitHub Pages)
2. Tutoriel 30min : `pokeforge new` → combat jouable, zéro prérequis dev
3. API reference auto-générée depuis XML docs C#
4. Guide migration depuis Pokémon Essentials + guide migration depuis PSDK

**Plans:** 4/4 plans
- Wave 1: Scaffold Docusaurus `docs/` + deploy GitHub Pages automatique sur push `main`
- Wave 2: Tutoriel "30 minutes" step by step avec captures d'écran
- Wave 3: Guides système (Battle Engine, Lua Scripting, Asset Pipeline, Plugins, Rendering HD)
- Wave 4: API reference auto-générée (`docfx`) + guide "créer son plugin" + guides migration Essentials/PSDK

> **🏁 TAG v2.0** — PokéForge est l'incontournable

**Status:** 🔲 Post-v1.0

---

## Progress

| Phase | Description | Plans | Status | Horizon |
|-------|-------------|-------|--------|---------|
| 1 | SDK.Core + SDK.Data | 4 | ✅ Complet | v0.1 |
| 2 | Battle Engine Core | 4 | ✅ Complet | v0.1 |
| 3 | World Foundation | 4 | ✅ Complet | v0.1 |
| 4 | Scripting + Progression | 3 | ✅ Complet — 97 tests | v0.1 |
| 5 | Plugins + Characters | TBD | 🔲 Not started | v1.0 |
| 7 | Developer Experience | 4 | 🔲 Not started | v1.0 |
| 8 | NuGet Distribution | 4 | 🔲 Not started | v1.0 |
| 9 | Sample Project | 4 | 🔲 Not started | v1.0 |
| 6 | Advanced Systems | TBD | 🔲 Post-v1.0 | v2.0 |
| 10 | CLI `pokeforge` | 4 | 🔲 Post-v1.0 | v2.0 |
| 11 | Documentation | 4 | 🔲 Post-v1.0 | v2.0 |

---
*Last updated: 2026-06-05 — Phases 1-4 marquées Complet. Tag v0.1 posé.*
