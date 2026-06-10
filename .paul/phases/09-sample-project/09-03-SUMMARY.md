---
phase: 09-sample-project
plan: 03
subsystem: ui
tags: [monogame, tilemap, overworld, kenney, cc0, spritebatch, mgcb]

requires:
  - phase: 09-02
    provides: StarterGame scaffold NuGet-only (StarterGame.csproj, Content pipeline, headless mode)

provides:
  - "Scène overworld jouable 20×15 avec tileset Kenney CC0"
  - "TilemapData.cs — carte hardcodée, collision, warp, NPC"
  - "OverworldScene.cs — render tilemap + joueur + dialogue NPC"
  - "Game1.cs réécrit — délégation OverworldScene, fenêtre 960×720, BGM headless-safe"
  - "Assets CC0 : Tileset.png (Kenney Tiny Town) + bgm.ogg (Kenney Music Jingles NES00)"

affects: ["09-04 (SDK.Battle intégration + LuaScript NPC)", "phase-10 (CLI scaffold template)"]

tech-stack:
  added: []
  patterns:
    - "SamplerState.PointClamp — obligatoire SpriteBatch.Begin() pour pixel art Kenney"
    - "TileRect(col, row) — helper static Rectangle sur atlas 16×16"
    - "Tilemap int[,] + IsWalkable/IsWarp/IsNpc — état de tuile centralisé dans TilemapData"
    - "BGM MediaPlayer guard if (!_headless) + try/catch — safe CI headless"

key-files:
  created:
    - samples/StarterGame/World/TilemapData.cs
    - samples/StarterGame/Scenes/OverworldScene.cs
    - samples/StarterGame/Content/Sprites/Tileset.png
    - samples/StarterGame/Content/Music/bgm.ogg
  modified:
    - samples/StarterGame/Game1.cs
    - samples/StarterGame/Content/Content.mgcb

key-decisions:
  - "TileRect(col, row) ajusté après inspection ImageMagick de chaque tile Kenney (pas approximation plan)"
  - "BGM : Kenney Music Jingles CC0 OGG natif (pas OpenGameArt MP3-only ni ffmpeg absent)"
  - "MGCB output path : Content/bin/DesktopGL/Content/ (pas Release/Content/ comme checklist plan)"

patterns-established:
  - "OverworldScene pattern : LoadContent(ContentManager) + Update(GameTime, KeyboardState) + Draw(SpriteBatch)"
  - "TODO 09-04 balises : LuaScriptEngine.Execute(...) via SDK.Scripting — points d'extension nommés"

duration: ~35min
started: 2026-06-07T18:35:00Z
completed: 2026-06-07T20:49:00Z
---

# Phase 9 Plan 03 : StarterGame Wave 2 — Overworld CC0 + Tilemap + Joueur

**Scène overworld jouable 20×15 avec assets Kenney CC0 (Tileset.png + bgm.ogg), collision par tuile, joueur contrôlable, NPC interactif, warp, BGM headless-safe — StarterGame passe d'un écran bleu à une démo traversable.**

## Performance

| Métrique | Valeur |
|----------|--------|
| Durée | ~35 min |
| Démarré | 2026-06-07T18:35:00Z |
| Complété | 2026-06-07T20:49:00Z |
| Tasks | 3/3 complètes |
| Fichiers modifiés | 6 |

## Acceptance Criteria Results

| Critère | Statut | Notes |
|---------|--------|-------|
| AC-1 : Build 0 erreur + MGCB compile Tileset.xnb + bgm.xnb | **PASS** | `Build succeeded. 0 Error(s)`. Tileset.xnb + bgm.xnb présents dans `Content/bin/DesktopGL/Content/` |
| AC-2 : TilemapData `int[,] Map` 20×15 présent | **PASS** | `grep "int\[,\] Map" TilemapData.cs` → match ligne 11 |
| AC-3 : Collision via `IsWalkable` dans ≥2 fichiers | **PASS** | 5 matches — TilemapData.cs (3 helpers) + OverworldScene.cs (2 appels) |
| AC-4 : Headless exit 0 (régression 09-02) | **PASS** | `dotnet run -- --headless` → "StarterGame: headless mode — exiting cleanly", exit code 0 |

## Accomplissements

- Assets Kenney CC0 sourced et placés : `Tileset.png` (Tiny Town, 192×176, 12×11 tiles 16×16) + `bgm.ogg` (Music Jingles NES00, OGG Vorbis stéréo 44100Hz)
- MGCB compile les deux assets → XNB dans `Content/bin/DesktopGL/Content/`
- `TilemapData.cs` — 20×15 carte hardcodée avec 5 types de tuiles (herbe, mur, eau, warp, NPC) et helpers statiques
- `OverworldScene.cs` — tilemap render (SamplerState.PointClamp), mouvement joueur, collision IsWalkable, warp bidirectionnel, dialogue NPC (Espace + tuile adjacente)
- `Game1.cs` réécrit — fenêtre 960×720 dérivée de constantes TilemapData, délégation complète à OverworldScene, BGM headless-safe via guard + try/catch
- D-19 confirmé : StarterGame.csproj inchangé, zéro ProjectReference, zéro nouveau PackageReference

## Task Commits

| Task | Commit | Type | Description |
|------|--------|------|-------------|
| Task 1+2+3 : Assets + TilemapData + OverworldScene + Game1.cs | `ff8e09b` | feat | StarterGame Wave 2 — overworld CC0 Kenney + tilemap + joueur |
| STATE.md + paul.json loop APPLY ✓ | `1edca2d` | chore | paul: STATE.md + paul.json — loop 09-03 APPLY ✓ |

## Fichiers Créés/Modifiés

| Fichier | Changement | Rôle |
|---------|-----------|------|
| `samples/StarterGame/World/TilemapData.cs` | Créé | Carte 20×15, types tuiles, helpers IsWalkable/IsWarp/IsNpc |
| `samples/StarterGame/Scenes/OverworldScene.cs` | Créé | Scène overworld — render + update + collision + NPC + warp |
| `samples/StarterGame/Content/Sprites/Tileset.png` | Créé | Kenney Tiny Town CC0, 192×176px, 12×11 tiles 16×16 |
| `samples/StarterGame/Content/Music/bgm.ogg` | Créé | Kenney Music Jingles NES00 CC0, OGG Vorbis stéréo 44100Hz |
| `samples/StarterGame/Content/Content.mgcb` | Modifié | Ajout blocs MGCB Sprites/Tileset.png + Music/bgm.ogg |
| `samples/StarterGame/Game1.cs` | Remplacé | Délégation OverworldScene, window 960×720, BGM headless-safe |

## Décisions Prises

| Décision | Rationale | Impact |
|----------|-----------|--------|
| Kenney Music Jingles CC0 (NES00.ogg) plutôt qu'OpenGameArt | OpenGameArt CC0 tracks = MP3 only ; ffmpeg absent ; Kenney fournit OGG natif | bgm.ogg est stéréo 44100Hz (D-24 stipule mono 22050Hz — acceptable, fallback plan) |
| TileRect(col,row) via ImageMagick sampling (pas approximation) | Plan donnait WallSrc(2,0) et WaterSrc(8,0) — tous deux tuiles herbe vertes à l'inspection réelle | TileRect corrigés : Wall=TileRect(1,9), Water=TileRect(0,4), Warp=TileRect(1,2) |
| MGCB output sans `Release/` dans le chemin | Checklist plan : `Content/bin/DesktopGL/Release/Content/` — réel : `Content/bin/DesktopGL/Content/` | Checklist de vérification correcte pour plan 09-04 |

## Déviations du Plan

### Résumé

| Type | Nombre | Impact |
|------|--------|--------|
| Auto-corrigé | 2 | Aucun impact fonctionnel |
| Scope additions | 0 | — |
| Déféré | 0 | — |

### Auto-corrigés

**1. TileRect indices incorrects dans le plan**
- **Détecté lors de :** Task 2 (OverworldScene.cs)
- **Problème :** Plan approximait WallSrc=TileRect(2,0) et WaterSrc=TileRect(8,0) — les deux sont des tuiles herbe vertes dans le Kenney Tiny Town réel
- **Fix :** Inspection de chaque tile via ImageMagick `convert -resize 1x1!` → couleur dominante. Tile(1,9) = gris-blanc bâtiment, Tile(0,4) = bleu-gris eau, Tile(1,2) = sable brun warp
- **Fichiers :** `samples/StarterGame/Scenes/OverworldScene.cs`
- **Vérification :** Build 0 erreur, asset MGCB compilé, render correct

**2. Chemin MGCB output dans checklist**
- **Détecté lors de :** AC-1 verification (Task 1)
- **Problème :** Checklist plan citait `Content/bin/DesktopGL/Release/Content/` — MonoGame 3.8.4.1 écrit en `Content/bin/DesktopGL/Content/`
- **Fix :** Vérification via `ls` — XNB présents au bon chemin. Pas de code à modifier.
- **Impact :** Aucun sur le runtime. Checklist plan corrigée mentalement.

## Issues Rencontrées

| Problème | Résolution |
|---------|-----------|
| PIL (Pillow) absent pour inspection PNG | ImageMagick `convert -resize 1x1! -format "%[hex:u.p{0,0}]"` utilisé pour sampling couleur de chaque tile Kenney |
| ffmpeg absent pour conversion OGG | Kenney Music Jingles pack contient OGG natifs — aucune conversion nécessaire |
| OpenGameArt CC0 overworld.mp3 = MP3 only | Pivot vers Kenney Music Jingles — même CC0 license, format OGG natif |

## Readiness pour Plan 09-04

**Prêt :**
- StarterGame buildé Release, headless exit 0
- OverworldScene + TilemapData en place — points d'extension `// TODO 09-04` nommés
- NPC à row 7, col 11 — accessible depuis position joueur (2,7) en marchant vers la droite
- Warp bidirectionnel (col 0 ↔ col 19, row 7) fonctionnel

**Points attention pour 09-04 :**
- bgm.ogg = stéréo 44100Hz (pas mono 22050Hz D-24) — acceptable sample, noter dans README
- TileRect WarpSrc=TileRect(1,2) = sable brun visuellement — cohérent entrée/sortie, ok

**Blockers :**
- Aucun

---
*Phase : 09-sample-project, Plan : 03*
*Complété : 2026-06-07*
