# Requirements: PokemonSDK

**Defined:** 2026-05-24
**Last updated:** 2026-06-01 — DX-01→06 ajoutés, BATTLE-04/05/06 renommés Plugin, horizons v0.1/v1.0/v2.0
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
- [ ] **PLAT-02**: Les builds sont vérifiés sur Windows et Linux (CI matrix GitHub Actions)
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

- [ ] **MAP-01**: Le renderer MonoGame affiche en résolution interne 480×270 upscalée ×4 → 1920×1080 via shader xBR. Sprites 96×96 combat, 48×48 overworld, tiles 16×16
- [ ] **MAP-02**: Le joueur peut se déplacer sur la carte et déclencher des rencontres sauvages selon les tables de rencontre par zone (heure + météo)
- [ ] **MAP-03**: Le cycle jour/nuit et la météo dynamique sont fonctionnels (tint shader DayNight, configurable temps réel PC ou horloge interne)

### SCRIPT — Scripting & Progression

- [ ] **SCRIPT-01**: Le moteur Lua MoonSharp est intégré en sandbox sécurisé (Preset_SoftSandbox + allowlist explicite) pour les événements et quêtes scriptables
- [ ] **SCRIPT-02**: Le système de badges track la progression du joueur : une arène par type Pokémon, validation de badge requise pour progresser
- [ ] **SCRIPT-03**: La sauvegarde automatique locale persiste l'état complet du jeu (position, équipe, badges, flags, inventaire)

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

## Out of Scope

| Feature | Reason |
|---------|--------|
| Electron / CEF | UI web embarquée non désirée — Avalonia est la cible |
| Moteur 3D / moteur fermé | MonoGame 2D uniquement |
| Connexion online / trades réseau | Local uniquement pour v1 et v2 |
| Données officielles Nintendo/Game Freak | SDK gère la structure, assets fournis par l'utilisateur |
| Support mobile (iOS/Android) | Desktop seulement — Windows/Linux/macOS |
| RPG Maker compatibility | Format propriétaire tiers, hors périmètre |
| Double battles en v1 | Architecturalement coûteux — 1v1 stable d'abord |

---

## Traceability

| Requirement | Horizon | Phase | Status |
|-------------|---------|-------|--------|
| DATA-01 | v0.1 | Phase 1 | 🔲 À recréer (.NET 10) |
| DATA-02 | v0.1 | Phase 1 | 🔲 À recréer |
| DATA-03 | v0.1 | Phase 1 | 🔲 À recréer |
| DATA-04 | v0.1 | Phase 1 | 🔲 À recréer |
| DATA-05 | v0.1 | Phase 1 | 🔲 À recréer |
| DATA-06 | v0.1 | Phase 1 | 🔲 À recréer |
| PLAT-01 | v0.1 | Phase 1 | 🔲 À recréer |
| PLAT-03 | v0.1 | Phase 1 | 🔲 À recréer |
| BATTLE-01 | v0.1 | Phase 2 | 🔲 À recréer |
| BATTLE-02 | v0.1 | Phase 2 | 🔲 À recréer |
| BATTLE-03 | v0.1 | Phase 2 | 🔲 À recréer |
| BATTLE-07 | v0.1 | Phase 2 | 🔲 À recréer |
| MAP-01 | v0.1 | Phase 3 | 🔲 Not started |
| MAP-02 | v0.1 | Phase 3 | 🔲 Not started |
| MAP-03 | v0.1 | Phase 3 | 🔲 Not started |
| PLAT-02 | v0.1 | Phase 3 | 🔲 Not started |
| SCRIPT-01 | v0.1 | Phase 4 | 🔲 À recréer |
| SCRIPT-02 | v0.1 | Phase 4 | 🔲 À recréer |
| SCRIPT-03 | v0.1 | Phase 4 | 🔲 À recréer |
| BATTLE-04 | v1.0 | Phase 5 | 🔲 Not started |
| BATTLE-05 | v1.0 | Phase 5 | 🔲 Not started |
| BATTLE-06 | v1.0 | Phase 5 | 🔲 Not started |
| CHAR-01 | v1.0 | Phase 5 | 🔲 Not started |
| CHAR-02 | v1.0 | Phase 5 | 🔲 Not started |
| CHAR-03 | v1.0 | Phase 5 | 🔲 Not started |
| ADV-01 | v1.0 | Phase 5 | 🔲 Not started |
| ADV-02 | v1.0 | Phase 5 | 🔲 Not started |
| DX-01 | v1.0 | Phase 7 | 🔲 Not started |
| DX-02 | v1.0 | Phase 7 | 🔲 Not started |
| DX-03 | v1.0 | Phase 8 | 🔲 Not started |
| DX-04 | v1.0 | Phase 9 | 🔲 Not started |
| ADV-03 | v2.0 | Phase 6 | 🔲 Post-v1.0 |
| ADV-04 | v2.0 | Phase 6 | 🔲 Post-v1.0 |
| DX-05 | v2.0 | Phase 10 | 🔲 Post-v1.0 |
| DX-06 | v2.0 | Phase 11 | 🔲 Post-v1.0 |

**Coverage:**
- v0.1 requirements: 14 total (Phases 1→4)
- v1.0 requirements: 14 total (Phases 5+7+8+9)
- v2.0 requirements: 6 total (Phases 6+10+11)
- Total mapped: 34/34
- Unmapped: 0

---
*Requirements defined: 2026-05-24*
*Last updated: 2026-06-01 — DX-01→06 ajoutés, BATTLE-04/05/06 renommés en Plugin, traceability avec horizons v0.1/v1.0/v2.0*
