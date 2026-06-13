---
sidebar_position: 1
---

# Tutoriel : un fan game fonctionnel en 30 minutes

Ce tutoriel part de zéro et aboutit à un fan game qui exécute un vrai combat et attribue un badge via Lua.

Durée estimée : 30 minutes.

## Ce que vous allez construire

- Un projet fan game scaffoldé via la CLI `pokeforge`
- Un combat 1v1 entre deux Pokémon résolu par programme
- Un script Lua qui attribue un badge et persiste un fichier de sauvegarde

## Prérequis

| Prérequis | Version |
|-----------|---------|
| .NET SDK | 10+ |
| SQLite | 3.x |
| CLI pokeforge | latest |
| Terminal | quelconque |

Installer la CLI :

```bash
dotnet tool install -g PokeForge.CLI
```

Vérifier :

```bash
pokeforge --version
```

## Étapes

| Étape | Ce que vous faites | Durée |
|-------|--------------------|-------|
| [Étape 1 : Créer un projet](tutorial/create) | Scaffold, seed, lancer headless | 10 min |
| [Étape 2 : Premier combat](tutorial/battle) | Brancher BattleEngine, lancer une boucle 1v1 | 10 min |
| [Étape 3 : Script Lua et badge](tutorial/lua-badge) | Écrire un script Lua, attribuer un badge, sauvegarder | 10 min |

Commencez par [l'Étape 1](tutorial/create).
