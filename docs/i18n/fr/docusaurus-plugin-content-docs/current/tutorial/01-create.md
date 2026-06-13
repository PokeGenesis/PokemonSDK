---
sidebar_position: 2
---

# Étape 1 : Créer un projet

## Scaffold

Lancez la commande `pokeforge new` pour créer un projet fan game :

```bash
pokeforge new MonJeu
```

Cela crée la structure suivante :

```
MonJeu/
  MonJeu.csproj        # Référence les packages NuGet PokeForge.SDK.*
  Game1.cs             # Point d'entrée : classe MonoGame Game
  Program.cs           # Racine de composition DI
  scripts/             # Les scripts Lua vont ici
  assets/              # Sprites, tilemaps, sons
```

## Remplir la base de données

Allez dans le dossier du projet et remplissez la base de données Pokémon :

```bash
cd MonJeu && pokeforge seed
```

Cette commande remplit `PokemonSDK.db` avec les 9 générations de Pokémon, capacités, talents et tableaux de types.

Sortie attendue :

```
Seeding generation 1... OK (151 species)
Seeding generation 2... OK (100 species)
...
Seeding generation 9... OK (103 species)
Seed complete. 1010 species total.
```

## Vérifier la santé du projet

Lancez la commande doctor pour confirmer que tout est correctement configuré :

```bash
pokeforge doctor
```

Sortie attendue (tout vert) :

```
[OK] .NET SDK 10.x found
[OK] PokemonSDK.db exists and is readable
[OK] EF Core migrations applied (8 tables)
[OK] pokeforge CLI version matches SDK
```

Si un élément affiche `[ERROR]`, la sortie inclut un conseil de correction. Le problème le plus courant est une base de données absente : relancez `pokeforge seed`.

## Lancer en mode headless

Démarrez le projet sans fenêtre graphique :

```bash
dotnet run -- --headless
```

Sortie attendue :

```
[PokemonSDK] Headless mode active
[World] Loaded: 0 tilemaps
[Game] Loop started
```

:::tip
Pour lancer avec une fenêtre graphique, omettez `--headless`. Cela requiert SDL2 et un GPU compatible OpenGL. Pour ce tutoriel, le mode headless suffit.
:::

Suivant : [Étape 2 : Premier combat](./battle)
