---
sidebar_position: 2
---

# pokeforge seed

Remplit la base SQLite avec les espèces, capacités, talents, objets et traductions Pokémon pour les 9 générations en 6 locales.

## Utilisation

```bash
pokeforge seed [--db <chemin>]
```

| Option | Défaut | Description |
|--------|--------|-------------|
| `--db` | `data/PokemonSDK.db` | Chemin vers la base SQLite |

## Données insérées

| Table | Contenu |
|-------|---------|
| `species` | 9 générations d'espèces avec stats de base et types |
| `moves` | Toutes les capacités avec puissance, précision, PP et catégorie |
| `items` | Tous les objets avec leurs drapeaux d'effet |
| `abilities` | Tous les talents |
| `translations` | Noms + descriptions en en, es, fr, de, it, ja |

## Exemples

```bash
# Remplir la base par défaut
pokeforge seed

# Remplir une base personnalisée
pokeforge seed --db /home/user/jeu/jeu.db
```

## Seed Fakemon

Pour insérer des espèces Fakemon personnalisées, fournissez un manifeste JSON :

```bash
pokeforge seed --fakemons data/fakemons.json
```

Les entrées Fakemon suivent le même schéma que les espèces normales mais avec `id: 0` (auto-assigné) et `is_fakemon: true`.

## Idempotent

Ré-exécuter `seed` sur une base déjà remplie est sans danger: les lignes existantes sont ignorées via `INSERT OR IGNORE`.
