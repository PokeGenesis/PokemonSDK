# PokemonSDK — Moteur & SDK Fan-Game Pokémon

## What This Is

PokemonSDK est un SDK open-source en C#/.NET 10 pour créer des fan-games Pokémon complets, à la manière de Pokémon Studio. Il fournit le moteur de données (SQLite), le moteur de combat universel, le runtime jeu (MonoGame), un système de plugins modulaire et tous les outils nécessaires pour qu'un maker Pokémon — développeur ou non — puisse créer son fan-game sans réimplémenter les règles de base.

Un second repo — l'éditeur visuel Avalonia (**PokeForge-Editor**) — construira un outil no-code complet au-dessus du SDK. Les deux repos sont strictement découplés.

## Core Value

Un développeur ou créateur peut brancher ce SDK et obtenir immédiatement un moteur de combat, une base de données Pokémon multilingue et un système de quêtes fonctionnel — sans réimplémenter les règles de base.

## Horizons

| Horizon | Goal | Phases |
|---------|------|--------|
| **v0.1** "Proof of Concept" | Moteur core jouable, release GitHub | 1 + 2 + 3 + 4 (~3-4 mois) |
| **v1.0** "Release" | SDK distribuable NuGet, maker crée son jeu | 5 + 7 + 8 + 9 (~3-4 mois de plus) |
| **v2.0** "Incontournable" | CLI + docs + features avancées | 6 + 10 + 11 (post feedback v1.0) |

## Requirements

### Validated (ancienne base .NET 8 — à recréer sur .NET 10)

**Phase 1 — SDK.Core + SDK.Data:**
- [x] DATA-01 : Structure multi-projets, séparation stricte
- [x] DATA-02 : Schéma SQLite 9 générations, `generation` INT NOT NULL
- [x] DATA-03 : Formes comme entités indépendantes (`pokemon_forms`)
- [x] DATA-04 : Migrations EF Core versionnées
- [x] DATA-05 : Filtre génération sur toutes les requêtes
- [x] DATA-06 : Table `translations` centrale, 5 langues (FR/EN/ES/DE/IT)
- [x] PLAT-01 : .NET 10 + MonoGame.DesktopGL cross-platform
- [x] PLAT-03 : Chemins cross-platform

**Phase 2 — Battle Engine Core:**
- [x] BATTLE-01 : Combat 1v1, tours, PP, statuts, KO
- [x] BATTLE-02 : Formules Gen 1-9, séparation physique/spécial Gen 4+
- [x] BATTLE-03 : IDifficultyMode : StoryModeAI + HardModeAI
- [x] BATTLE-07 : BattleConfig configurable

**Phase 4 — Scripting + Progression:**
- [x] SCRIPT-01 : MoonSharp Preset_SoftSandbox
- [x] SCRIPT-02 : Système de badges
- [x] SCRIPT-03 : SaveSystem JSON + auto-save

### Active (v0.1 — à implémenter)

- [ ] MAP-01, MAP-02, MAP-03 : Renderer MonoGame + overworld + jour/nuit (Phase 3)
- [ ] PLAT-02 : Builds vérifiés Windows + Linux (Phase 3)

### Active (v1.0 — à implémenter après v0.1)

- [ ] BATTLE-04 : Plugin Nuzlocke
- [ ] BATTLE-05 : Plugin Randomizer
- [ ] BATTLE-06 : Plugin Turbo
- [ ] CHAR-01, CHAR-02, CHAR-03 : Personnages personnalisables
- [ ] ADV-01, ADV-02 : Pokégear + objets terrain
- [ ] DX-01 : Asset pipeline + SpriteValidator
- [ ] DX-02 : Hot reload Lua + console REPL + overlay erreur
- [ ] DX-03 : NuGet publication
- [ ] DX-04 : Sample project StarterGame

### Active (v2.0 — post feedback v1.0)

- [ ] ADV-03, ADV-04 : TTS + Fakemon generator
- [ ] DX-05 : CLI `pokeforge`
- [ ] DX-06 : Site docs + tutoriel 30min

### Out of Scope

- Electron / CEF — Avalonia est la cible
- Moteur 3D / gros moteur fermé — MonoGame 2D uniquement
- Connexion online / trades réseau — local uniquement
- Données officielles Game Freak / Nintendo — SDK gère la structure, assets fournis par l'utilisateur
- Support mobile (iOS/Android) — Desktop seulement
- RPG Maker compatibility
- Double battles en v1

## Context

- **Environnement** : WSL + VSCode, développement sous Linux, cible Windows principale
- **Deux repos** : Ce repo (PokemonSDK) = SDK/core. PokeForge-Editor = projet séparé ultérieur
- **Référence** : Pokémon Studio (PSDK, Ruby/RGSS) — PokéForge vise à le surclasser sur perf, DX et rendu
- **9 générations** : Gen 1 (Kanto) → Gen 9 (Paldea), toutes formes régionales et spéciales
- **Rendering HD** : Résolution interne 480×270 → shader xBR ×4 → 1920×1080. Sprites 96×96 combat, 48×48 overworld

## Constraints

- **Tech Stack** : C# / .NET 10, EF Core 10, SQLite, MonoGame.DesktopGL, MoonSharp — pas de déviation
- **Plateforme** : Windows cible principale, Linux/macOS dès l'architecture (cross-platform natif .NET)
- **Séparation** : SDK et éditeur en repos séparés — pas de couplage fort
- **Assets** : Sprites et audio fournis par l'utilisateur ou générés — SDK ne bundle pas de contenu officiel
- **Plugins** : Nuzlocke/Randomizer/Turbo = plugins activables/désactivables, jamais des modes hardcodés
- **NuGet** : SemVer strict, pas de breaking change sans bump majeur de version

## Key Decisions

| # | Decision | Rationale |
|---|----------|-----------|
| D-01 | **.NET 10** — pas de retour en arrière | LTS, migration unique au démarrage |
| D-02 | **MonoGame.DesktopGL** — jamais WindowsDX | Cross-platform obligatoire dès v1 |
| D-03 | **EF Core + migrations** — jamais Dapper seul | Schéma 9 générations = migrations indispensables |
| D-04 | **MoonSharp Preset_SoftSandbox** | Zero native DLL, sandboxing C# natif |
| D-05 | **BattleState immuable** (record + `with`) | Battle engine pur, testable sans effets de bord |
| D-06 | **SDK.MonoGame ne référence pas SDK.Scripting** | Composition via `Func<IScriptEngine>` dans Game1 |
| D-07 | **Table `translations` centrale** | Localisation propre, jamais colonnes locales sur entités |
| D-08 | **Deux repos distincts** SDK / Éditeur | SDK consommable sans l'éditeur |
| D-09 | **`generation` INT NOT NULL** partout | Filtre génération dès le départ, partout |
| D-10 | **System.Text.Json** — jamais Newtonsoft | Intégré .NET 10, zéro dépendance |
| D-11 | **Sleep/Freeze ne sautent pas les tours** | Correction critique validée Phase 2 ancienne base |
| D-12 | **`GameState.Flags = Dictionary<string, JsonElement>`** | Type-safe, JSON-serializable |
| D-13 | **Nuzlocke/Randomizer/Turbo = plugins** | Moteur core inchangé, ouvre à plugins communautaires |
| D-14 | **Résolution interne 480×270 → ×4 → 1920×1080** | Multiplicateur entier = pixels nets. Figé dès le début |
| D-15 | **xBR comme shader d'upscaling** | Surclasse nearest-neighbor de PSDK, intégré RenderPipeline |
| D-16 | **Convention nommage sprites : `{dexid5}_{id}_{view}.png`** | Asset pipeline auto exige convention stricte |
| D-17 | **SDK.Tools sans MonoGame** | SpriteValidator tourne en CI headless |
| D-18 | **NuGet SemVer strict** | Pas de breaking change sans bump majeur |
| D-19 | **Sample consomme via NuGet** | Valide l'expérience réelle d'un maker externe |
| D-20 | **CLI scaffold depuis template = sample stabilisé** | `pokeforge new` produit toujours quelque chose qui tourne |
| D-21 | **Docs documentent uniquement les APIs stables** | Une API documentée ne se casse plus sans deprecation |

## Evolution

Ce document évolue aux transitions de phase et aux milestones.

**Après chaque transition de phase** (via `/paul:unify`) :
1. Requirements invalidés ? → Déplacer en Out of Scope avec raison
2. Requirements validés ? → Déplacer en Validated avec référence de phase
3. Nouveaux requirements ? → Ajouter en Active
4. Décisions à logger ? → Ajouter en Key Decisions
5. "What This Is" toujours exact ? → Mettre à jour si dérivé

**Aux milestones v0.1, v1.0, v2.0** :
1. Revue complète de toutes les sections
2. Core Value check — toujours la bonne priorité ?
3. Audit Out of Scope — raisons toujours valides ?
4. Annonce communautaire si release publique

---
*Last updated: 2026-06-01 — Restructuré en 3 horizons, DX-01→06 ajoutés, 21 décisions architecturales*
