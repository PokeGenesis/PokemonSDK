---
sidebar_position: 4
---

# pokeforge fakemon

Manage Fakemon sprites: list available parts and assemble composite sprites.

## list-parts

Lists all part images found in a parts directory, grouped by category.

```bash
pokeforge fakemon list-parts [--parts-dir <path>]
```

| Flag | Default | Description |
|------|---------|-------------|
| `--parts-dir` | `assets/fakemon-parts/` | Directory containing part PNG images |

### Example output

```
pokeforge fakemon list-parts

body/
  dragon_base.png
  serpent_base.png
  feline_base.png

head/
  electric_head.png
  fire_head.png
  psychic_head.png

tail/
  spiked_tail.png
  flame_tail.png
```

## assemble

Assembles a composite Fakemon sprite by layering part images.

```bash
pokeforge fakemon assemble \
  --identifier <id> \
  --parts-dir <path> \
  --output <dir> \
  [--body <part>] \
  [--head <part>] \
  [--tail <part>] \
  [--views front,back,overworld,portrait,icon]
```

| Flag | Description |
|------|-------------|
| `--identifier` | Fakemon identifier (e.g. `dragon-electric`) |
| `--parts-dir` | Directory of part images |
| `--output` | Output directory (sprites are saved as `fk_{identifier}_{view}.png`) |
| `--body` | Body part filename (without extension) |
| `--head` | Head part filename |
| `--tail` | Tail part filename (optional) |
| `--views` | Comma-separated views to generate (default: `front,back,overworld,portrait,icon`) |

### Example

```bash
pokeforge fakemon assemble \
  --identifier dragon-electric \
  --body dragon_base \
  --head electric_head \
  --tail spiked_tail \
  --parts-dir assets/fakemon-parts/ \
  --output assets/sprites/fakemons/
```

Output files follow the D-16 naming convention:

```
assets/sprites/fakemons/
  fk_dragon-electric_front.png
  fk_dragon-electric_back.png
  fk_dragon-electric_overworld.png
  fk_dragon-electric_portrait.png
  fk_dragon-electric_icon.png     ← 32×32
```
