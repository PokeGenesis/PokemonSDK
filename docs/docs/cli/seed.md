---
sidebar_position: 2
---

# pokeforge seed

Populates the SQLite database with Pokémon species, moves, abilities, items, and translations for all 9 generations in 6 locales.

## Usage

```bash
pokeforge seed [--db <path>]
```

| Flag | Default | Description |
|------|---------|-------------|
| `--db` | `data/PokemonSDK.db` | Path to the SQLite database file |

## What it seeds

| Table | Records |
|-------|---------|
| `species` | 9 generations of species with base stats and types |
| `moves` | All moves with power, accuracy, PP, and category |
| `items` | All items with effect flags |
| `abilities` | All abilities |
| `translations` | Names + descriptions in en, es, fr, de, it, ja |

## Example

```bash
# Seed the default database
pokeforge seed

# Seed a custom path
pokeforge seed --db /home/user/game/game.db
```

## Fakemon seed

To seed custom Fakemon species, provide a JSON manifest:

```bash
pokeforge seed --fakemons data/fakemons.json
```

Fakemon entries follow the same schema as regular species but with `id: 0` (auto-assigned) and `is_fakemon: true`.

## Idempotent

Re-running `seed` on an already-seeded database is safe — existing rows are skipped via `INSERT OR IGNORE`.
