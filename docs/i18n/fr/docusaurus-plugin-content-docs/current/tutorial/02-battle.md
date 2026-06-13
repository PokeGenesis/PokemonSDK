---
sidebar_position: 3
---

# Étape 2 : Premier combat

Ouvrez `src/Game1.cs` et localisez la méthode `Initialize()` (ou le bloc de démarrage appelé depuis `LoadContent`).

Ajoutez le code suivant :

```csharp
using Microsoft.Extensions.DependencyInjection;
using PokeForge.SDK.Battle;
using PokeForge.SDK.Battle.Formulas;
using PokeForge.SDK.Battle.Difficulty;
using PokeForge.SDK.Data;

// Résoudre DbContext depuis DI
var db = Services.GetRequiredService<PokemonDbContext>();

// Charger deux Pokémon par numéro de Dex
var bulbasaur  = await db.PokemonSpecies.FindAsync(1)
    ?? throw new Exception("Lancez 'pokeforge seed' d'abord.");
var charmander = await db.PokemonSpecies.FindAsync(4)
    ?? throw new Exception("Lancez 'pokeforge seed' d'abord.");

// Construire les combattants
var player   = BattlePokemon.FromSpecies(bulbasaur,  level: 5);
var opponent = BattlePokemon.FromSpecies(charmander, level: 5);

// Configurer le combat
var config  = new BattleConfig { MaxTurns = 200 };
var formula = new StandardDamageFormula();
var mode    = new NormalDifficultyMode();
var engine  = new BattleEngine(player, opponent, formula, mode, config);

// Jouer les tours jusqu'à la fin du combat
while (!engine.State.IsOver)
{
    var move = engine.State.PlayerPokemon.Moves[0]; // utiliser toujours la première capacité
    engine.ExecuteTurn(move);
}

var result = engine.State.Result!;
Console.WriteLine($"Vainqueur : {(result.PlayerWon ? player.Name : opponent.Name)}");
Console.WriteLine($"Tours :     {result.TurnsElapsed}");
```

## Lancer

```bash
dotnet run -- --headless
```

Sortie console attendue :

```
Vainqueur : Bulbasaur
Tours :     7
```

Le vainqueur exact et le nombre de tours dépendent du niveau et du RNG. Le combat se termine toujours.

## Comprendre le code

**BattleState est immuable.** Chaque appel à `ExecuteTurn` retourne un nouveau `BattleState` via `record` + `with` en C#. L'état précédent n'est jamais muté : les replays et l'annulation sont gratuits.

**IDamageFormula est substituable.** Remplacez `StandardDamageFormula` par `Gen1DamageFormula` pour reproduire le comportement de troncature entière de la Génération 1, ou implémentez la vôtre.

**BattleConfig contrôle les limites.** `MaxTurns = 200` empêche les boucles infinies quand deux Pokémon ratent continuellement. `Result.EndReason` vaut `"MaxTurns"` si le timeout se déclenche.

:::note
Voir la [référence du package Battle](../packages/battle) pour toutes les options de BattleEngine, les stratégies de sélection de capacités et les hooks de plugins.
:::

Suivant : [Étape 3 : Script Lua et badge](./lua-badge)
