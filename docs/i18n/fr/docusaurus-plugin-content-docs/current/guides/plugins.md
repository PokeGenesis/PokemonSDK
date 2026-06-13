---
sidebar_position: 4
---

# Plugins : étendre le moteur de combat

Ce guide montre comment implémenter `IBattlePlugin`, enregistrer des plugins, et utiliser les plugins fournis Nuzlocke, Randomizer et Turbo.

## Installer

```bash
dotnet add package PokeForge.SDK.Plugins
```

## Interface IBattlePlugin

`IBattlePlugin` expose des hooks appelés par `BattleEngine` aux moments clés :

```csharp
public interface IBattlePlugin
{
    void OnBattleStart(BattleState state);
    BattleState OnTurnStart(BattleState state);
    BattleState OnDamageApplied(BattleState state, DamageEvent damage);
    void OnBattleEnd(BattleState state);
}
```

Chaque hook qui retourne `BattleState` peut modifier l'état en retournant `state with { ... }`. Les hooks qui retournent `void` sont des notifications uniquement.

## Implémenter un plugin

```csharp
using PokeForge.SDK.Plugins;

public class LoggingPlugin : IBattlePlugin
{
    public void OnBattleStart(BattleState state)
        => Console.WriteLine($"Combat démarré : {state.Player.Name} vs {state.Opponent.Name}");

    public BattleState OnTurnStart(BattleState state) => state;  // pas de changement

    public BattleState OnDamageApplied(BattleState state, DamageEvent damage)
    {
        Console.WriteLine($"{damage.Target.Name} reçoit {damage.Amount} dégâts");
        return state;
    }

    public void OnBattleEnd(BattleState state)
        => Console.WriteLine($"Gagnant : {state.Winner?.Name ?? "Égalité"}");
}
```

## Enregistrer avec PluginRegistry

```csharp
var registry = new PluginRegistry();
registry.Register(new LoggingPlugin());

// Passer le registry à BattleEngine
var engine = new BattleEngine(config, registry);
```

Plusieurs plugins peuvent être enregistrés. Ils sont appelés dans l'ordre d'enregistrement.

## Plugins fournis

Trois plugins sont livrés avec `PokeForge.SDK.Plugins`. Ce sont des plugins, pas des modes codés en dur : ils sont toujours opt-in et peuvent coexister.

### NuzlockePlugin

Applique la mort permanente : tout Pokémon qui perd conscience devient définitivement indisponible.

```csharp
registry.Register(new NuzlockePlugin());
```

### RandomizerPlugin

Remplace les espèces de Pokémon sauvages et de dresseurs par des sélections aléatoires déterministes par graine :

```csharp
registry.Register(new RandomizerPlugin(seed: 42));
```

La même graine produit toujours la même run randomisée.

### TurboPlugin

Accélère la vitesse du texte et les animations par un multiplicateur configurable :

```csharp
registry.Register(new TurboPlugin(textSpeedMultiplier: 4));
```

Utile pour les speedruns et les créateurs de contenu.
