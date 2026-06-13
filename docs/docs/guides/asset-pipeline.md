---
sidebar_position: 5
---

# Asset Pipeline: Sprites and Atlases

This guide covers sprite naming conventions, atlas generation, and SQLite sync using `PokeForge.SDK.Tools`.

## Install

```bash
dotnet add package PokeForge.SDK.Tools
```

## Sprite Naming Convention

All sprite files follow a strict naming rule enforced by `SpriteValidator`:

```
{dexid5}_{identifier}_{view}.png
```

| Part | Description | Example |
|------|-------------|---------|
| `dexid5` | 5-digit National Dex ID | `00025` |
| `identifier` | lowercase slug | `pikachu` |
| `view` | one of the 5 views | `front` |

Valid views: `front`, `back`, `overworld`, `portrait`, `icon`.

The `icon` view is always 32x32 pixels, used in party screens, PC boxes, and the Pokédex list.

Full regex enforced by `SpriteValidator`:

```
^(\d{5}_[a-z0-9-]+|fk_[a-z0-9-]+)_(front|back|overworld|portrait|icon)\.png$
```

Example valid filenames:

```
00025_pikachu_front.png
00006_charizard_overworld.png
00006_charizard-mega_front.png
fk_dragoncat_front.png
fk_dragoncat_icon.png
```

Store canonical Pokémon sprites under `assets/sprites/` and Fakemon sprites under `assets/sprites/fakemons/`.

## Validate Sprites

The CLI scans your asset directory and reports naming violations:

```bash
pokeforge asset-sync --validate-only
```

Sample output:

```
[ERROR] 0025_pikachu_front.png — dexid must be 5 digits (found 4)
[WARN]  00025_Pikachu_front.png — identifier must be lowercase
[OK]    00025_pikachu_front.png
```

Exit code 1 if any `ERROR`; exit code 0 if only `WARN` or `OK`.

## Generate Atlas

`AtlasPacker` combines individual sprites into a single texture atlas for efficient GPU rendering:

```csharp
using PokeForge.SDK.Tools;

var packer = new AtlasPacker("assets/sprites/", "build/atlas/");
var result = packer.Pack();

// result.AtlasPath  → "build/atlas/sprites.png"
// result.ImportPath → "build/atlas/import.json"
```

The `import.json` maps each sprite name to its UV coordinates in the atlas.

Or from the CLI:

```bash
pokeforge asset-sync --pack-atlas
```

## Sync to SQLite

`SqliteSyncer` reads `import.json` and writes sprite paths to the database:

```csharp
var syncer = new SqliteSyncer(dbContext, "build/atlas/import.json");
await syncer.SyncAsync();
```

Each Pokémon entity gets its `SpritePath` column updated. Run this after every atlas rebuild.

## CLI Asset-Sync: Full Pipeline

The full pipeline (validate, pack, sync) runs as a single command:

```bash
pokeforge asset-sync
```

Exit codes:

| Code | Meaning |
|------|---------|
| `0` | Success (or warnings only) |
| `1` | One or more naming errors |
| `2` | Atlas generation failed |
| `3` | SQLite sync failed |
