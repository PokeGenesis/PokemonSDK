---
sidebar_position: 8
---

# Fakemons: Custom Species

This guide shows how to name Fakemon sprites, assemble composite sprites from parts, and export custom species to SQLite with 6-locale translations.

## Install

```bash
dotnet add package PokeForge.SDK.Tools
```

## Sprite Naming

Fakemon sprites follow the `fk_` prefix rule from the naming convention:

```
fk_{identifier}_{view}.png
```

| Part | Description | Example |
|------|-------------|---------|
| `fk_` | Fakemon prefix (required) | `fk_` |
| `identifier` | lowercase slug | `dragoncat` |
| `view` | one of the 5 views | `front` |

Place Fakemon sprites under `assets/sprites/fakemons/`:

```
assets/sprites/fakemons/
  fk_dragoncat_front.png
  fk_dragoncat_back.png
  fk_dragoncat_overworld.png
  fk_dragoncat_portrait.png
  fk_dragoncat_icon.png      ← always 32x32
```

## List Available Parts

The CLI scans a parts catalog directory and lists composable body parts:

```bash
pokeforge fakemon list-parts --catalog assets/parts/
```

Output:

```
Bodies:   fk_dragon_body, fk_cat_body, fk_fish_body
Heads:    fk_dragon_head, fk_cat_head, fk_bunny_head
Tails:    fk_dragon_tail, fk_fish_tail
Wings:    fk_bat_wings
```

## Assemble a Sprite

Combine parts into a new Fakemon sprite:

```bash
pokeforge fakemon assemble \
  --base fk_dragon_body \
  --head fk_cat_head \
  --tail fk_dragon_tail \
  --view front \
  --output assets/sprites/fakemons/fk_dragoncat_front.png
```

Repeat for each view (`front`, `back`, `overworld`, `portrait`, `icon`) you need.

## Export to SQLite

`FakemonExporter` writes a `FakemonSpecies` entity and its translations to the database. Exactly 6 locales are required:

```csharp
using PokeForge.SDK.Tools;

var exporter = new FakemonExporter(dbContext);

await exporter.ExportAsync(new FakemonSpecies
{
    Identifier   = "dragoncat",
    BaseHp       = 65,
    BaseAttack   = 70,
    BaseDefense  = 55,
    BaseSpeed    = 85,
    PrimaryType  = PokemonType.Dragon,
    SecondaryType = PokemonType.Normal,
    Translations = new Dictionary<string, string>
    {
        ["en"] = "Dragoncat",
        ["fr"] = "Dracofélin",
        ["es"] = "Dragogato",
        ["de"] = "Drachenkatz",
        ["it"] = "Dragogatto",
        ["ja"] = "ドラゴンキャット",
    }
});
```

All six locales (`en`, `es`, `fr`, `de`, `it`, `ja`) are required. The exporter throws `InvalidOperationException` if any locale is missing.

## FakemonAssemblyPipeline in Code

For batch workflows, use the pipeline classes directly:

```csharp
// 1. Scan the parts catalog
var catalog = FakemonPartsCatalog.Scan("assets/parts/");

// 2. Filter to parts matching your criteria
var filter = new FakemonFilter(catalog);
var dragonParts = filter.ByTag("dragon");

// 3. Assemble a composite sprite
var assembler = new FakemonAssembler();
var assembled = assembler.Assemble(new AssemblySpec
{
    BasePart = dragonParts.Bodies.First(),
    HeadPart = catalog.Heads["fk_cat_head"],
    View     = SpriteView.Front,
    Output   = "assets/sprites/fakemons/fk_dragoncat_front.png"
});

// 4. Export all 5 views to SQLite
var exporter = new FakemonExporter(dbContext);
await exporter.ExportAsync(assembled.ToSpecies(translations));
```
