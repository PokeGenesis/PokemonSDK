---
sidebar_position: 5
---

# Pipeline d'assets : sprites et atlas

Ce guide couvre les conventions de nommage des sprites, la génération d'atlas et la synchronisation SQLite avec `PokeForge.SDK.Tools`.

## Installer

```bash
dotnet add package PokeForge.SDK.Tools
```

## Convention de nommage des sprites

Tous les fichiers de sprites suivent une règle de nommage stricte appliquée par `SpriteValidator` :

```
{dexid5}_{identifier}_{vue}.png
```

| Partie | Description | Exemple |
|--------|-------------|---------|
| `dexid5` | Numéro Pokédex National sur 5 chiffres | `00025` |
| `identifier` | Identifiant en minuscules | `pikachu` |
| `vue` | Une des 5 vues | `front` |

Vues valides : `front`, `back`, `overworld`, `portrait`, `icon`.

La vue `icon` est toujours en 32x32 pixels, utilisée dans les écrans de l'équipe, les boîtes PC et la liste du Pokédex.

Regex complète appliquée par `SpriteValidator` :

```
^(\d{5}_[a-z0-9-]+|fk_[a-z0-9-]+)_(front|back|overworld|portrait|icon)\.png$
```

Exemples de noms valides :

```
00025_pikachu_front.png
00006_charizard_overworld.png
00006_charizard-mega_front.png
fk_dragoncat_front.png
fk_dragoncat_icon.png
```

Stockez les sprites Pokémon officiels sous `assets/sprites/` et les sprites Fakemon sous `assets/sprites/fakemons/`.

## Valider les sprites

La CLI scanne votre répertoire d'assets et signale les violations de nommage :

```bash
pokeforge asset-sync --validate-only
```

Exemple de sortie :

```
[ERROR] 0025_pikachu_front.png -- dexid doit avoir 5 chiffres (4 trouvés)
[WARN]  00025_Pikachu_front.png -- identifier doit être en minuscules
[OK]    00025_pikachu_front.png
```

Code de sortie 1 si au moins une erreur `ERROR` ; code 0 si seulement des `WARN` ou `OK`.

## Générer un atlas

`AtlasPacker` combine les sprites individuels en une seule texture atlas pour un rendu GPU efficace :

```csharp
using PokeForge.SDK.Tools;

var packer = new AtlasPacker("assets/sprites/", "build/atlas/");
var result = packer.Pack();

// result.AtlasPath  → "build/atlas/sprites.png"
// result.ImportPath → "build/atlas/import.json"
```

Le fichier `import.json` associe chaque nom de sprite à ses coordonnées UV dans l'atlas.

Ou depuis la CLI :

```bash
pokeforge asset-sync --pack-atlas
```

## Synchroniser avec SQLite

`SqliteSyncer` lit `import.json` et écrit les chemins de sprites dans la base de données :

```csharp
var syncer = new SqliteSyncer(dbContext, "build/atlas/import.json");
await syncer.SyncAsync();
```

Chaque entité Pokémon voit sa colonne `SpritePath` mise à jour. Exécutez cette commande après chaque reconstruction d'atlas.

## CLI asset-sync : pipeline complet

Le pipeline complet (valider, packager, synchroniser) s'exécute en une seule commande :

```bash
pokeforge asset-sync
```

Codes de sortie :

| Code | Signification |
|------|---------------|
| `0` | Succès (ou avertissements seulement) |
| `1` | Une ou plusieurs erreurs de nommage |
| `2` | Échec de la génération d'atlas |
| `3` | Échec de la synchronisation SQLite |
