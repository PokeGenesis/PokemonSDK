---
sidebar_position: 5
---

# pokeforge asset-sync

Validates sprite filenames, packs them into a texture atlas, and syncs atlas metadata to the SQLite database. Run before shipping or after adding sprites.

## Usage

```bash
pokeforge asset-sync [--sprites-dir <path>] [--db <path>] [--dry-run]
```

| Flag | Default | Description |
|------|---------|-------------|
| `--sprites-dir` | `assets/sprites/` | Root sprite directory |
| `--db` | `data/PokemonSDK.db` | SQLite database to update |
| `--dry-run` | false | Validate and pack without writing to the database |

## What it does

1. **Validate** — `SpriteValidator` checks every PNG in `--sprites-dir` against the D-16 naming convention. Any violation is printed and the command exits with code `1`.
2. **Pack** — `AtlasPacker` combines valid sprites into `assets/atlas.png` + `assets/atlas.json`.
3. **Sync** — `SqliteSyncer` updates the `sprites` table with UV coordinates from `atlas.json`.

## Naming convention

Real species: `{dexid5}_{identifier}_{view}.png`

```
00025_pikachu_front.png
00025_pikachu_back.png
00025_pikachu_overworld.png
00025_pikachu_portrait.png
00025_pikachu_icon.png          ← must be 32×32
```

Fakemons: `fk_{identifier}_{view}.png`

```
fk_dragon-electric_front.png
fk_dragon-electric_icon.png
```

Valid views: `front`, `back`, `overworld`, `portrait`, `icon`.

## Example

```bash
# Full sync
pokeforge asset-sync

# Validate only (no writes)
pokeforge asset-sync --dry-run

# Custom paths
pokeforge asset-sync --sprites-dir content/sprites/ --db game.db
```

## Example output

```
pokeforge asset-sync

✓ Validated 142 sprites
✓ Packed atlas → assets/atlas.png (2048×2048)
✓ Synced 142 sprite records to data/PokemonSDK.db
```
