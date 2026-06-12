# Requirements: PokemonSDK

**Defined:** 2026-05-24
**Last updated:** 2026-06-05 — v0.1 requirements marqués ✅ (Phases 1-4 complètes, 97 tests)
**Core Value:** Un développeur peut brancher ce SDK et obtenir immédiatement un moteur de combat, une base de données Pokémon multilingue et un système de quêtes fonctionnel — sans réimplémenter les règles de base.

---

## v1 Requirements

### DATA — Fondation SDK

- [x] **DATA-01**: La solution C# est structurée en projets séparés (SDK.Core, SDK.Data, SDK.Battle, SDK.Scripting, SDK.MonoGame) avec séparation stricte des dépendances — MonoGame absent de SDK.Core
- [x] **DATA-02**: Le schéma SQLite couvre les 9 générations Pokémon avec champ `generation` INT NOT NULL obligatoire sur toutes les entités concernées
- [x] **DATA-03**: Les formes Pokémon (régionales, spéciales, légendaires, Fakemons) sont des entités `pokemon_forms` indépendantes — jamais des colonnes nullables sur `pokemon_species`
- [x] **DATA-04**: Les migrations EF Core permettent de créer, versionner et appliquer le schéma SQLite sans perte de données
- [x] **DATA-05**: Le filtre par génération fonctionne sur toutes les requêtes data (Pokémon, capacités, objets, types)
- [x] **DATA-06**: La table `translations` centrale gère les noms et descriptions en EN, ES, FR, DE, IT, JA via (entity_type, entity_id, locale, field) — aucune colonne locale sur les tables d'entités

### PLAT — Plateforme & Cross-Platform

- [x] **PLAT-01**: Le SDK cible .NET 10 LTS avec MonoGame.Framework.DesktopGL (OpenGL) — architecture cross-platform Windows + Linux + macOS dès v1
- [x] **PLAT-02**: Les builds sont vérifiés sur Windows et Linux (CI matrix GitHub Actions)
- [x] **PLAT-03**: Les chemins de fichiers et accès SQLite sont cross-platform — pas de chemins Windows codés en dur

### BATTLE — Moteur de combat

- [x] **BATTLE-01**: Le moteur de combat 1v1 gère les tours, calcul de dommages, PP, statuts (brûlure, paralysie, poison, gel, sommeil, confusion) et KO
- [x] **BATTLE-02**: Les mécaniques de combat sont configurables par génération (formules Gen 1-9, règles de critiques, séparation physique/spécial Gen 4+)
- [x] **BATTLE-03**: Les modes de difficulté Histoire (IA simple) et Hard (IA stratégique) sont implémentés via interface IDifficultyMode
- [ ] **BATTLE-04**: Le **plugin** Nuzlocke applique mort permanente et règle une capture par zone — activable/désactivable via PluginRegistry sans modifier le moteur core
- [ ] **BATTLE-05**: Le **plugin** Randomizer full randomise Pokémon + dresseurs + capacités + objets + types avec graine (seed) reproductible
- [ ] **BATTLE-06**: Le **plugin** Turbo accélère les animations et transitions du combat
- [x] **BATTLE-07**: Les paramètres de combat (activation items, fuites, critiques, météo en combat, etc.) sont configurables par projet via BattleConfig

### MAP — Rendu & Monde

- [x] **MAP-01**: Le renderer MonoGame affiche en résolution interne 480×270 upscalée ×4 → 1920×1080 via shader xBR. Sprites 96×96 combat, 48×48 overworld, tiles 16×16
- [x] **MAP-02**: Le joueur peut se déplacer sur la carte et déclencher des rencontres sauvages selon les tables de rencontre par zone (heure + météo)
- [x] **MAP-03**: Le cycle jour/nuit et la météo dynamique sont fonctionnels (tint shader DayNight, configurable temps réel PC ou horloge interne)

### SCRIPT — Scripting & Progression

- [x] **SCRIPT-01**: Le moteur Lua MoonSharp est intégré en sandbox sécurisé (Preset_SoftSandbox + allowlist explicite) pour les événements et quêtes scriptables
- [x] **SCRIPT-02**: Le système de badges track la progression du joueur : une arène par type Pokémon, validation de badge requise pour progresser
- [x] **SCRIPT-03**: La sauvegarde automatique locale persiste l'état complet du jeu (position, équipe, badges, flags, inventaire)

### CHAR — Personnages & Narrative

- [ ] **CHAR-01**: Le dresseur principal (fille ou garçon) est créable avec système de palettes cosmétiques par couche (peau, yeux, cheveux, tenues, accessoires) — chaque couche indépendante
- [ ] **CHAR-02**: Deux rivaux sont personnalisables au même niveau de profondeur que le dresseur principal
- [ ] **CHAR-03**: Le groupe antagoniste possède une structure hiérarchique (boss + sbires) avec mécaniques de blocage de progression via scripts Lua

### ADV — Systèmes Avancés

- [ ] **ADV-01**: Le Pokégear intègre lecteur de musique, journal de rencontres et carte du monde navigable
- [ ] **ADV-02**: Les objets de terrain remplacent les CS/HMs — les actions sur le terrain sont déclenchées par des objets inventaire
- [ ] **ADV-03**: Le système TTS génère des voix off via synthèse vocale intégrée pour les combats importants et les dialogues clés — asynchrone, non-bloquant
- [ ] **ADV-04**: Le générateur de Fakemons permet d'assembler des parties 2D, de remapper les palettes par couche et d'exporter le Fakemon vers la base SQLite

### DX — Developer Experience

- [ ] **DX-01**: L'asset pipeline automatique (SpriteScanner + SpriteValidator + AtlasPacker + SqliteSyncer) génère atlas MonoGame, config MGCB et sync SQLite depuis un dépôt de PNG — zéro configuration manuelle. SpriteValidator en CI avec exit code 1 si ERROR
- [ ] **DX-02**: Le hot reload Lua (<500ms) recharge les scripts sans redémarrage. La console REPL ingame (toggle `~`) permet d'inspecter et modifier le GameState live. Les erreurs Lua affichent fichier + ligne + message en overlay
- [ ] **DX-03**: Le SDK et tous ses plugins sont publiés sur NuGet.org avec versioning SemVer. Le CI publie automatiquement sur tag `v*`. Un projet consommateur externe peut installer via `dotnet add package PokéForge.SDK`
- [ ] **DX-04**: `samples/StarterGame/` est un jeu minimal jouable avec assets placeholder CC0, commentaires pédagogiques `// SDK:`, et intégré au CI. Consomme le SDK via NuGet uniquement
- [ ] **DX-05**: Le CLI `pokeforge` (`dotnet tool install -g pokeforge`) permet `new`, `asset-sync`, `seed`, `build`, `doctor` — `pokeforge new mon-jeu` génère un projet prêt à lancer
- [ ] **DX-06**: Un site docs public (Docusaurus, GitHub Pages) inclut le tutoriel "premier jeu en 30 minutes", une API reference auto-générée, et des guides de migration depuis Pokémon Essentials et PSDK

---

## v2 Requirements

Déférés — prévus mais hors scope v1.

### MAP — Éditeur & Import

- **MAP-V2-01**: Import de cartes Tiled (.tmx natif) dans le SDK
- **MAP-V2-02**: Éditeur de cartes intégré dans l'UI Avalonia (repo éditeur)

### QUEST — Quêtes & Narration

- **QUEST-V2-01**: Système de quêtes principales et secondaires scriptables avec suivi de flags
- **QUEST-V2-02**: Les quêtes secondaires récompensent des objets ou CTs utiles au joueur

### CHAR — Social & Échanges

- **CHAR-V2-01**: Trade/échange local avec PNJ inconnu

### BATTLE — Modes Avancés

- **BATTLE-V2-01**: Combats 2v2 avec ciblage adjacent/étendu et IA adaptée
- **BATTLE-V2-02**: Architecture hook pour capacités et objets tenus (centaines d'interactions Gen 1-9)

### PROG — End-Game

- **PROG-V2-01**: Conseil des types (8 membres experts post-arènes)
- **PROG-V2-02**: Maître Ligue Pokémon (boss final de progression)

---

## v3 Requirements

*Gameplay Complete — v3.0 "Premier jeu complet"*

### BTLUI — Interface de combat

- [ ] **BTLUI-01**: `BattleScene` — HUD HP bars style GBA/DS, PP display, sprites front (ennemi 96×96) + back (joueur 96×96), move selection menu 4 capacités, statuts visuels (brûlure/gel/dodo/paralysie/confusion), transitions overworld→combat et retour
- [ ] **BTLUI-02**: EXP + Level-up + Évolution — gain EXP post-combat (formule Gen configurable), barre EXP animée, écran level-up (stats Δ affichées), triggers évolution (niveau/pierre/échange/bonheur), animation évolution sprite cross-fade
- [ ] **BTLUI-03**: Items en combat + Bag + Shop — `ItemRegistry` effets dans SDK.Core, `BagScene` catégorisée (Poké Balls/soins/CTs/objets tenus), sélection item depuis BattleScene menu, `PokéMart` scriptable Lua

### UI — Scènes de jeu

- [ ] **UI-01**: `PartyScene` — 6 slots Pokémon, HP bar + statut par slot, réorganisation ordre, accès depuis menu principal, D-22 noms multilingues
- [ ] **UI-02**: `PCScene` — boîtes PC défilables, dépôt/retrait Pokémon, renommage, organisation par boîte
- [ ] **UI-03**: `PokédexScene` — entrée par numéro dex, nom multilingue (D-22), description multilingue, sprite front/back + shiny, stats de base, types, taux de capture

### QUEST — Système de quêtes

- [ ] **QUEST-01**: `SDK.Plugins.Quests` (QuestPlugin) — quêtes chaînables scriptables Lua (`quest:start(id)`, `quest:update(id, key, val)`, `quest:complete(id)`), stockées dans `GameState.Flags`, récompenses (items/badges/EXP) distribuées par script
- [ ] **QUEST-02**: Quest tracker UI — liste quêtes actives + complètes en journal, objectifs lisibles texte multilingue D-22, icône statut, récompenses affichées

### DATA-EXT — Extension données réelles

- [ ] **DATA-07**: Pipeline import réel — `pokeforge import --source pokeapi` ingère 1010 Pokémon + moves + abilities + types depuis PokeAPI REST v2 vers SQLite, génération correcte par espèce, D-22 6 locales, idempotent (re-import = no-op si aucun changement)

---

## v4 Requirements

*Content & Extensions — v4.0 "Extensions"*

### SFX — Audio complet

- [ ] **SFX-01**: Audio complet — cries OGG joués en début de combat (D-24) + victoire KO, SFX UI (sélection menu/damage hit/statut), `SoundManager` non-bloquant (thread audio séparé), zéro freeze game loop

### MOD — Mécaniques modernes (plugins)

- [ ] **MOD-01**: `MegaPlugin` via `IBattlePlugin` — Méga-Évolution déclenche boost stats + changement type si Méga-Pierre tenue, cooldown 1 par combat, BattleState immuable respecté (D-05)
- [ ] **MOD-02**: `ZMovePlugin` via `IBattlePlugin` — Z-Capacités consomment Z-Crystal, override dégâts base (formule officielle), effets Z-Status appliqués
- [ ] **MOD-03**: `DynamaxPlugin` via `IBattlePlugin` — Dynamax 3 tours, HP doublé, G-Capacités remplacent moveset, Max Raid boss avec compteur bouclier

### DUNGEON — Mode donjon procédural

- [ ] **DUNGEON-01**: `SDK.Plugins.Dungeon` — `IDungeonMode` override `WorldSystem`, génération procédurale de floors (algo BSP + corridors), mouvement 1 step = 1 tour, `IDungeonBattleResolver` remplace BattleEngine standard en donjon, faim décroissante par step
- [ ] **DUNGEON-02**: `DungeonFloorGenerator` — seed reproductible, salles avec items/Pokémon ennemis (table spawn configurable par floor), boss de floor, escalier étage suivant

### STREAM — Plugin streamer

- [ ] **STREAM-01**: `SDK.Plugins.Streamer` — lit équipe active depuis `GameState`, push titre stream via Twitch Helix API (`PATCH /channels`) et YouTube Data API (`videos.update`), `StreamerConfig { TwitchClientId, TwitchToken, YouTubeToken, GameTitle }` injecté par consumer, zéro coupling SDK.MonoGame
- [ ] **STREAM-02**: HUD overlay streamer SDK.MonoGame — 6 sprites `icon` (D-23 32×32) équipe active + nom du jeu en coin configurable, toggle ingame par touche paramétrable

---

## v5 Requirements

*Network — v5.0 "En ligne"*

### NET — Couche réseau

- [ ] **NET-01**: `SDK.Network` — infrastructure combat en ligne tour-par-tour (WebSocket), trade bilatéral authentifié (échange Pokémon confirmé des deux côtés), GTS minimal (dépôt + recherche par espèce/niveau)

---

## Out of Scope

| Feature | Reason |
|---------|--------|
| Electron / CEF | UI web embarquée non désirée — Avalonia est la cible |
| Moteur 3D / moteur fermé | MonoGame 2D uniquement |
| Connexion online temps réel (MMO, PvP massif) | Hors périmètre — uniquement combat 1v1 + trade + GTS prévus en v5.0 |
| Données officielles Nintendo/Game Freak | SDK gère la structure, assets fournis par l'utilisateur |
| Support mobile (iOS/Android) | Desktop seulement — Windows/Linux/macOS |
| RPG Maker compatibility | Format propriétaire tiers, hors périmètre |
| Double battles en v1 | Architecturalement coûteux — 1v1 stable d'abord |

---

## Traceability

| Requirement | Horizon | Phase | Status |
|-------------|---------|-------|--------|
| DATA-01 | v0.1 | Phase 1 | ✅ |
| DATA-02 | v0.1 | Phase 1 | ✅ |
| DATA-03 | v0.1 | Phase 1 | ✅ |
| DATA-04 | v0.1 | Phase 1 | ✅ |
| DATA-05 | v0.1 | Phase 1 | ✅ |
| DATA-06 | v0.1 | Phase 1 | ✅ |
| PLAT-01 | v0.1 | Phase 1 | ✅ |
| PLAT-03 | v0.1 | Phase 1 | ✅ |
| BATTLE-01 | v0.1 | Phase 2 | ✅ |
| BATTLE-02 | v0.1 | Phase 2 | ✅ |
| BATTLE-03 | v0.1 | Phase 2 | ✅ |
| BATTLE-07 | v0.1 | Phase 2 | ✅ |
| MAP-01 | v0.1 | Phase 3 | ✅ RenderPipeline xBR (Phase 3) |
| MAP-02 | v0.1 | Phase 3 | ✅ PlayerSystem + EncounterSystem (Phase 3) |
| MAP-03 | v0.1 | Phase 3 | ✅ IGameClock + WeatherSystem (Phase 3) |
| PLAT-02 | v0.1 | Phase 3 | ✅ CI matrix ubuntu + windows |
| SCRIPT-01 | v0.1 | Phase 4 | ✅ LuaScriptEngine SoftSandbox (Phase 4) |
| SCRIPT-02 | v0.1 | Phase 4 | ✅ BadgeApi + NpcInteractionRunner (Phase 4) |
| SCRIPT-03 | v0.1 | Phase 4 | ✅ SaveSystem JSON (Phase 4) |
| BATTLE-04 | v1.0 | Phase 5 | ✅ NuzlockePlugin (Phase 5) |
| BATTLE-05 | v1.0 | Phase 5 | ✅ RandomizerPlugin (Phase 5) |
| BATTLE-06 | v1.0 | Phase 5 | ✅ TurboPlugin (Phase 5) |
| CHAR-01 | v1.0 | Phase 5 | ✅ Character + cosmétiques (Phase 5) |
| CHAR-02 | v1.0 | Phase 5 | ✅ Rivals (Phase 5) |
| CHAR-03 | v1.0 | Phase 5 | ✅ VillainGroup + VillainMember (Phase 5) |
| ADV-01 | v1.0 | Phase 5 | ✅ Pokégear (Phase 5) |
| ADV-02 | v1.0 | Phase 5 | ✅ Objets terrain (Phase 5) |
| DX-01 | v1.0 | Phase 7 | ✅ SpriteValidator + AtlasPacker + SqliteSyncer (Phase 7) |
| DX-02 | v1.0 | Phase 7 | ✅ LuaHotReloader + LuaConsole REPL (Phase 7) |
| DX-03 | v1.0 | Phase 8 | ✅ 7 packages NuGet publiables (Phase 8) |
| DX-04 | v1.0 | Phase 9 | ✅ StarterGame NuGet-only (Phase 9) |
| ADV-03 | v0.3 | Phase 6 | 🔲 v0.3 / Phase 6 |
| ADV-04 | v0.3 | Phase 6 | 🔲 v0.3 / Phase 6 |
| DX-05 | v2.0 | Phase 10 | 🔲 Post-v1.0 |
| DX-06 | v2.0 | Phase 11 | 🔲 Post-v1.0 |
| BTLUI-01 | v3.0 | Phase 12 | 🔲 Post-v2.0 |
| BTLUI-02 | v3.0 | Phase 13 | 🔲 Post-v2.0 |
| BTLUI-03 | v3.0 | Phase 14 | 🔲 Post-v2.0 |
| UI-01 | v3.0 | Phase 15 | 🔲 Post-v2.0 |
| UI-02 | v3.0 | Phase 15 | 🔲 Post-v2.0 |
| UI-03 | v3.0 | Phase 15 | 🔲 Post-v2.0 |
| QUEST-01 | v3.0 | Phase 16 | 🔲 Post-v2.0 |
| QUEST-02 | v3.0 | Phase 16 | 🔲 Post-v2.0 |
| DATA-07 | v3.0 | Phase 17 | 🔲 Post-v2.0 |
| SFX-01 | v4.0 | Phase 18 | 🔲 Post-v3.0 |
| MOD-01 | v4.0 | Phase 19 | 🔲 Post-v3.0 |
| MOD-02 | v4.0 | Phase 19 | 🔲 Post-v3.0 |
| MOD-03 | v4.0 | Phase 19 | 🔲 Post-v3.0 |
| DUNGEON-01 | v4.0 | Phase 20 | 🔲 Post-v3.0 |
| DUNGEON-02 | v4.0 | Phase 20 | 🔲 Post-v3.0 |
| STREAM-01 | v4.0 | Phase 21 | 🔲 Post-v3.0 |
| STREAM-02 | v4.0 | Phase 21 | 🔲 Post-v3.0 |
| NET-01 | v5.0 | Phase 22 | 🔲 Post-v4.0 |

**Coverage:**
- v0.1 requirements: 16 total (Phases 1→4) ✅
- v1.0 requirements: 12 total (Phases 5+7+8+9) ✅
- v2.0 requirements: 4 total (Phases 6+10+11) 🔲
- v3.0 requirements: 9 total (Phases 12→17) 🔲
- v4.0 requirements: 8 total (Phases 18→21) 🔲
- v5.0 requirements: 1 total (Phase 22) 🔲
- Total mappé: 50/50
- Non mappé: 0

---
*Requirements defined: 2026-05-24*
*Last updated: 2026-06-07 — v3→v5 requirements ajoutés (BTLUI, UI, QUEST, DATA-07, SFX, MOD, DUNGEON, STREAM, NET). 50 requirements total. Traceability Phases 12→22.*
