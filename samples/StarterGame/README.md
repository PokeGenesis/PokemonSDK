# StarterGame — PokeForge SDK Demo

Démo jouable du SDK via NuGet uniquement (D-19).

## Prérequis

- .NET 10 SDK
- `dotnet tool restore` (installe dotnet-mgcb 3.8.4.1)

## Lancer

```bash
cd samples/StarterGame
dotnet run
```

## Contrôles

| Touche | Action |
|--------|--------|
| Flèches | Déplacer le joueur (jaune) |
| Espace | Interagir avec le NPC (magenta) |
| F5 | Sauvegarder |
| F9 | Charger |
| Esc | Quitter |

## Ce que ça démontre

- **SDK.Battle** : BattleEngine 1v1 headless + NuzlockePlugin
- **SDK.Scripting** : `badges:AwardBadge('boulder')` via Lua SoftSandbox
- **ISaveSystem** : save/load GameState JSON
- Assets **CC0** : Kenney Tiny Town tileset + Music Jingles NES00

> PokeForge SDK — MIT License — <https://github.com/PokeGenesis/PokemonSDK>
