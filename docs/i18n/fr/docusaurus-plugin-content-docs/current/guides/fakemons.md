---
sidebar_position: 8
---

# Fakemons : espèces personnalisées

Ce guide montre comment nommer les sprites Fakemon, assembler des sprites composites à partir de pièces, et exporter des espèces personnalisées vers SQLite avec 6 traductions de locale.

## Installer

```bash
dotnet add package PokeForge.SDK.Tools
```

## Nommage des sprites

Les sprites Fakemon suivent la règle de préfixe `fk_` de la convention de nommage :

```
fk_{identifier}_{vue}.png
```

| Partie | Description | Exemple |
|--------|-------------|---------|
| `fk_` | Préfixe Fakemon (obligatoire) | `fk_` |
| `identifier` | Identifiant en minuscules | `dragoncat` |
| `vue` | Une des 5 vues | `front` |

Placez les sprites Fakemon sous `assets/sprites/fakemons/` :

```
assets/sprites/fakemons/
  fk_dragoncat_front.png
  fk_dragoncat_back.png
  fk_dragoncat_overworld.png
  fk_dragoncat_portrait.png
  fk_dragoncat_icon.png      <- toujours 32x32
```

## Lister les pièces disponibles

La CLI scanne un répertoire de catalogue de pièces et liste les parties du corps composables :

```bash
pokeforge fakemon list-parts --catalog assets/parts/
```

Sortie :

```
Bodies:   fk_dragon_body, fk_cat_body, fk_fish_body
Heads:    fk_dragon_head, fk_cat_head, fk_bunny_head
Tails:    fk_dragon_tail, fk_fish_tail
Wings:    fk_bat_wings
```

## Assembler un sprite

Combinez des pièces en un nouveau sprite Fakemon :

```bash
pokeforge fakemon assemble \
  --base fk_dragon_body \
  --head fk_cat_head \
  --tail fk_dragon_tail \
  --view front \
  --output assets/sprites/fakemons/fk_dragoncat_front.png
```

Répétez pour chaque vue (`front`, `back`, `overworld`, `portrait`, `icon`) dont vous avez besoin.

## Exporter vers SQLite

`FakemonExporter` écrit une entité `FakemonSpecies` et ses traductions dans la base de données. Exactement 6 locales sont requises :

```csharp
using PokeForge.SDK.Tools;

var exporter = new FakemonExporter(dbContext);

await exporter.ExportAsync(new FakemonSpecies
{
    Identifier    = "dragoncat",
    BaseHp        = 65,
    BaseAttack    = 70,
    BaseDefense   = 55,
    BaseSpeed     = 85,
    PrimaryType   = PokemonType.Dragon,
    SecondaryType = PokemonType.Normal,
    Translations  = new Dictionary<string, string>
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

Les six locales (`en`, `es`, `fr`, `de`, `it`, `ja`) sont obligatoires. L'exporteur lève une `InvalidOperationException` si une locale est manquante.

## FakemonAssemblyPipeline en code

Pour les workflows par lots, utilisez directement les classes du pipeline :

```csharp
// 1. Scanner le catalogue de pièces
var catalog = FakemonPartsCatalog.Scan("assets/parts/");

// 2. Filtrer les pièces correspondant à vos critères
var filter = new FakemonFilter(catalog);
var dragonParts = filter.ByTag("dragon");

// 3. Assembler un sprite composite
var assembler = new FakemonAssembler();
var assembled = assembler.Assemble(new AssemblySpec
{
    BasePart = dragonParts.Bodies.First(),
    HeadPart = catalog.Heads["fk_cat_head"],
    View     = SpriteView.Front,
    Output   = "assets/sprites/fakemons/fk_dragoncat_front.png"
});

// 4. Exporter les 5 vues vers SQLite
var exporter = new FakemonExporter(dbContext);
await exporter.ExportAsync(assembled.ToSpecies(translations));
```
