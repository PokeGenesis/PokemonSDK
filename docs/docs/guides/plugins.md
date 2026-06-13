---
sidebar_position: 4
---

# Plugins: Extend the Battle Engine

This guide shows how to implement `IBattlePlugin`, register plugins, and use the bundled Nuzlocke, Randomizer, and Turbo plugins.

## Install

```bash
dotnet add package PokeForge.SDK.Plugins
```

## IBattlePlugin Interface

`IBattlePlugin` exposes hooks called by `BattleEngine` at key moments:

```csharp
public interface IBattlePlugin
{
    void OnBattleStart(BattleState state);
    BattleState OnTurnStart(BattleState state);
    BattleState OnDamageApplied(BattleState state, DamageEvent damage);
    void OnBattleEnd(BattleState state);
}
```

Each hook that returns `BattleState` can modify the state by returning `state with { ... }`. Hooks that return `void` are notification-only.

## Implement a Plugin

```csharp
using PokeForge.SDK.Plugins;

public class LoggingPlugin : IBattlePlugin
{
    public void OnBattleStart(BattleState state)
        => Console.WriteLine($"Battle started: {state.Player.Name} vs {state.Opponent.Name}");

    public BattleState OnTurnStart(BattleState state) => state;  // no change

    public BattleState OnDamageApplied(BattleState state, DamageEvent damage)
    {
        Console.WriteLine($"{damage.Target.Name} took {damage.Amount} damage");
        return state;
    }

    public void OnBattleEnd(BattleState state)
        => Console.WriteLine($"Winner: {state.Winner?.Name ?? "Draw"}");
}
```

## Register with PluginRegistry

```csharp
var registry = new PluginRegistry();
registry.Register(new LoggingPlugin());

// Pass the registry to BattleEngine
var engine = new BattleEngine(config, registry);
```

Multiple plugins can be registered. They are called in registration order.

## Bundled Plugins

Three plugins ship with `PokeForge.SDK.Plugins`. These are plugins, not hardcoded modes: they are always opt-in and can coexist.

### NuzlockePlugin

Enforces permadeath: any Pokémon that faints is permanently unavailable.

```csharp
registry.Register(new NuzlockePlugin());
```

### RandomizerPlugin

Replaces wild and trainer Pokémon species with seed-deterministic random selections:

```csharp
registry.Register(new RandomizerPlugin(seed: 42));
```

The same seed always produces the same randomized run.

### TurboPlugin

Accelerates text speed and animation by a configurable multiplier:

```csharp
registry.Register(new TurboPlugin(textSpeedMultiplier: 4));
```

Useful for speedruns and content creators.
