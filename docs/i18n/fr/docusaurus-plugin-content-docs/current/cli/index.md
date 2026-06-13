---
sidebar_position: 1
---

# CLI — pokeforge

`pokeforge` est un outil .NET global pour créer et maintenir des projets PokemonSDK.

## Installation

```bash
dotnet tool install -g PokeForge.CLI
```

Vérification :

```bash
pokeforge --version
```

## Commandes

| Commande | Description |
|----------|-------------|
| [`pokeforge new`](#new) | Créer un projet depuis le template starter |
| [`pokeforge seed`](./seed.md) | Remplir la base SQLite avec les données Pokémon |
| [`pokeforge doctor`](./doctor.md) | Vérifier les dépendances runtime |
| [`pokeforge asset-sync`](./asset-sync.md) | Valider, packer et synchroniser les sprites |
| [`pokeforge fakemon list-parts`](./fakemon.md) | Lister les parties Fakemon disponibles |
| [`pokeforge fakemon assemble`](./fakemon.md) | Assembler un sprite Fakemon depuis des parties |

## `pokeforge new` {#new}

Crée un projet complet depuis le template starter du SDK.

```bash
pokeforge new MonJeu
cd MonJeu
dotnet run
```

Le projet généré référence les packages PokemonSDK via NuGet (pas de références projet) et inclut des répertoires `data/`, `scripts/` et `assets/` préconfigurés.

## Mise à jour

```bash
dotnet tool update -g PokeForge.CLI
```
