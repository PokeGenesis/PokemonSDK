---
sidebar_position: 5
---

# pokeforge asset-sync

Valide les noms de fichiers sprites, les regroupe dans un atlas de texture, et synchronise les métadonnées dans la base SQLite. À exécuter avant de livrer ou après avoir ajouté des sprites.

## Utilisation

```bash
pokeforge asset-sync [--sprites-dir <chemin>] [--db <chemin>] [--dry-run]
```

| Option | Défaut | Description |
|--------|--------|-------------|
| `--sprites-dir` | `assets/sprites/` | Répertoire racine des sprites |
| `--db` | `data/PokemonSDK.db` | Base SQLite à mettre à jour |
| `--dry-run` | false | Valider et packer sans écrire dans la base |

## Ce que ça fait

1. **Valider**: `SpriteValidator` vérifie chaque PNG dans `--sprites-dir` contre la convention de nommage D-16. Toute violation est affichée et la commande se termine avec le code `1`.
2. **Packer**: `AtlasPacker` regroupe les sprites valides dans `assets/atlas.png` + `assets/atlas.json`.
3. **Synchroniser**: `SqliteSyncer` met à jour la table `sprites` avec les coordonnées UV depuis `atlas.json`.

## Convention de nommage

Espèces réelles : `{dexid5}_{identifiant}_{vue}.png`

```
00025_pikachu_front.png
00025_pikachu_back.png
00025_pikachu_overworld.png
00025_pikachu_portrait.png
00025_pikachu_icon.png          ← doit faire 32×32 pixels
```

Fakemons : `fk_{identifiant}_{vue}.png`

```
fk_dragon-electrique_front.png
fk_dragon-electrique_icon.png
```

Vues valides : `front`, `back`, `overworld`, `portrait`, `icon`.

## Exemples

```bash
# Synchronisation complète
pokeforge asset-sync

# Validation uniquement (sans écriture)
pokeforge asset-sync --dry-run

# Chemins personnalisés
pokeforge asset-sync --sprites-dir contenu/sprites/ --db jeu.db
```

## Exemple de sortie

```
pokeforge asset-sync

✓ 142 sprites validés
✓ Atlas packagé → assets/atlas.png (2048×2048)
✓ 142 enregistrements de sprites synchronisés dans data/PokemonSDK.db
```
