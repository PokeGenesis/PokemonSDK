---
sidebar_position: 2
---

# BattleEngine : combat 1v1

Ce guide montre comment configurer les formules et la difficulté, lancer une boucle de combat, et intégrer le moteur de combat avec MonoGame.

## Installer

```bash
dotnet add package PokeForge.SDK.Battle
```

## Configurer les formules et la difficulté

`BattleConfig` accepte n'importe quelle `IDamageFormula` et `IDifficultyMode`. Deux implémentations sont fournies avec le SDK :

| Type | Options |
|------|---------|
| `IDamageFormula` | `StandardDamageFormula`, `Gen1DamageFormula` |
| `IDifficultyMode` | `NormalDifficultyMode`, `HardDifficultyMode` |

```csharp
using PokeForge.SDK.Battle;

var config = new BattleConfig(
    formula: new StandardDamageFormula(),
    difficulty: new NormalDifficultyMode()
);

var engine = new BattleEngine(config);
```

Utilisez `Gen1DamageFormula` pour reproduire les calculs de dégâts de la Génération 1, y compris la formule de coup critique.

## Lancer une boucle de combat

`BattleState` est un record immuable : chaque appel à `ExecuteTurn` retourne un nouvel état via des expressions `with`. Ne mutez jamais l'objet état directement.

```csharp
var state = BattleState.Start(playerPokemon, opponentPokemon, config);

while (!state.IsOver)
{
    var moveIndex = ChooseMove(state);            // votre UI choisit une attaque
    state = engine.ExecuteTurn(state, moveIndex);
    RenderBattleFrame(state);                    // votre passe de rendu
}

Console.WriteLine($"Gagnant : {state.Winner?.Name ?? "Égalité"}");
```

`BattleState.IsOver` devient `true` quand un côté n'a plus de PV ou que tous ses Pokémon ont perdu conscience.

Note sur les effets de statut : Sommeil et Gel ne sautent pas le tour affecté automatiquement. Le moteur attend que le joueur choisisse une attaque avant de résoudre le statut, reproduisant le comportement réel des jeux.

## Lire les événements de combat

`state.LastEvent` contient des données structurées sur ce qui s'est passé lors du tour précédent :

```csharp
switch (state.LastEvent)
{
    case MoveEvent mv:
        Console.WriteLine($"{mv.User.Name} utilise {mv.MoveName}");
        break;
    case DamageEvent dmg:
        Console.WriteLine($"{dmg.Target.Name} perd {dmg.Amount} PV");
        break;
    case StatusEvent st:
        Console.WriteLine($"{st.Pokemon.Name} est maintenant {st.Status}");
        break;
}
```

## Intégrer avec MonoGame

Appelez `ExecuteTurn` dans `Update()` après la résolution des entrées, puis lisez l'état dans `Draw()` :

```csharp
// Game1.Update()
if (_inputReady && !_battleState.IsOver)
{
    _battleState = _engine.ExecuteTurn(_battleState, _selectedMove);
    _inputReady = false;
}

// Game1.Draw()
_battleRenderer.Draw(_battleState, spriteBatch);
```

Maintenez `ExecuteTurn` hors de `Draw()`. La mise à jour de l'état de combat est de la logique, pas du rendu.
