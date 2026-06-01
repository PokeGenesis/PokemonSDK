# Système de Plugins — PokemonSDK

## Principe (D-13)

Nuzlocke, Randomizer, Turbo sont des **plugins**, pas des modes hardcodés.
Le moteur de combat (`BattleEngine`) ne connaît que `PluginRegistry` — jamais les plugins concrets.
Chaque plugin = projet NuGet indépendant dans `src/SDK.Plugins.*`.

---

## Interfaces — SDK.Battle/Plugins/

```csharp
// IBattlePlugin.cs
public interface IBattlePlugin
{
    string Name { get; }

    // Hooks cycle de combat
    void         OnBattleStart(BattleState state);
    void         OnTurnStart(BattleState state);
    void         OnTurnEnd(BattleState state);
    void         OnBattleEnd(BattleState state, BattleResult result);

    // Hooks actions
    BattleState? OnBeforeMove(BattleState state, BattleAction action);   // null = pas de modif
    BattleState? OnBeforeDamage(BattleState state, DamageResult damage); // null = pas de modif

    // Hooks Pokémon
    void OnPokemonFainted(BattleState state, BattlePokemon fainted);
    void OnPokemonCaught(BattleState state, BattlePokemon caught, string zone);
    void OnPokemonLevelUp(BattlePokemon pokemon, int oldLevel, int newLevel);
}

// PluginRegistry.cs
public sealed class PluginRegistry
{
    private readonly List<IBattlePlugin> _plugins = new();

    public void Register(IBattlePlugin plugin)
    {
        if (_plugins.Any(p => p.Name == plugin.Name))
            throw new InvalidOperationException($"Plugin '{plugin.Name}' déjà enregistré.");
        _plugins.Add(plugin);
    }

    public void Unregister(string name) =>
        _plugins.RemoveAll(p => p.Name == name);

    public void Clear() => _plugins.Clear();

    public bool IsRegistered(string name) =>
        _plugins.Any(p => p.Name == name);

    // Méthodes appelées par BattleEngine
    public void NotifyBattleStart(BattleState state)      => _plugins.ForEach(p => p.OnBattleStart(state));
    public void NotifyTurnStart(BattleState state)        => _plugins.ForEach(p => p.OnTurnStart(state));
    public void NotifyPokemonFainted(BattleState state, BattlePokemon fainted)
        => _plugins.ForEach(p => p.OnPokemonFainted(state, fainted));
    public void NotifyPokemonCaught(BattleState state, BattlePokemon caught, string zone)
        => _plugins.ForEach(p => p.OnPokemonCaught(state, caught, zone));

    // Hooks avec modification de state — chaîne les plugins
    public BattleState ApplyBeforeDamage(BattleState state, DamageResult damage)
    {
        foreach (var plugin in _plugins)
        {
            var modified = plugin.OnBeforeDamage(state, damage);
            if (modified is not null) state = modified;
        }
        return state;
    }
}
```

---

## Plugin Nuzlocke — SDK.Plugins.Nuzlocke/

```csharp
// NuzlockePlugin.cs
// Ce projet référence SDK.Core + SDK.Battle uniquement — JAMAIS SDK.MonoGame
public sealed class NuzlockePlugin : IBattlePlugin
{
    private readonly HashSet<string> _caughtZones = new();

    public string Name => "Nuzlocke";

    public void OnPokemonFainted(BattleState state, BattlePokemon fainted)
    {
        // Mort permanente — le Pokémon ne peut plus être utilisé
        // Le flag est persisté via GameState dans l'appelant
        fainted.IsPermanentlyDead = true;
    }

    public void OnPokemonCaught(BattleState state, BattlePokemon caught, string zone)
    {
        if (_caughtZones.Contains(zone))
            throw new NuzlockeViolationException(
                $"Règle Nuzlocke : déjà une capture dans la zone '{zone}'.");
        _caughtZones.Add(zone);
    }

    // Hooks no-op pour ce plugin
    public void OnBattleStart(BattleState state)  { }
    public void OnTurnStart(BattleState state)    { }
    public void OnTurnEnd(BattleState state)      { }
    public void OnBattleEnd(BattleState state, BattleResult result) { }
    public BattleState? OnBeforeMove(BattleState state, BattleAction action) => null;
    public BattleState? OnBeforeDamage(BattleState state, DamageResult damage) => null;
    public void OnPokemonLevelUp(BattlePokemon pokemon, int oldLevel, int newLevel) { }
}

public sealed class NuzlockeViolationException : Exception
{
    public NuzlockeViolationException(string message) : base(message) { }
}
```

---

## Plugin Randomizer — SDK.Plugins.Randomizer/

```csharp
// RandomizerPlugin.cs
public sealed class RandomizerPlugin : IBattlePlugin
{
    private readonly SeededRandom _rng;
    public string Name => "Randomizer";

    public RandomizerPlugin(long seed)
    {
        _rng = new SeededRandom(seed);
        // Même seed = même résultat garanti (D-13)
    }

    // La randomisation se fait à l'initialisation du jeu, pas pendant les combats
    // RandomizerPlugin expose des méthodes appelées depuis le composition root :
    public IReadOnlyList<PokemonSpecies> RandomizeEncounterTable(
        IReadOnlyList<PokemonSpecies> original)
    {
        return original.OrderBy(_ => _rng.NextDouble()).ToList();
    }

    public Move RandomizeMove(IReadOnlyList<Move> allMoves)
        => allMoves[_rng.Next(0, allMoves.Count)];

    // Hooks combat no-op (le randomizer agit à l'init, pas pendant le combat)
    public void OnBattleStart(BattleState state) { }
    public void OnTurnStart(BattleState state)   { }
    // ... autres hooks no-op
}
```

---

## Activation dans Game1 (composition root)

```csharp
// Game1.cs — SEUL endroit où les plugins sont configurés
public class Game1 : Microsoft.Xna.Framework.Game
{
    private readonly PluginRegistry _pluginRegistry = new();

    protected override void Initialize()
    {
        // Activation selon config utilisateur (lu depuis settings.json)
        if (_settings.NuzlockeEnabled)
            _pluginRegistry.Register(new NuzlockePlugin());

        if (_settings.RandomizerEnabled)
            _pluginRegistry.Register(new RandomizerPlugin(_settings.RandomizerSeed));

        if (_settings.TurboEnabled)
            _pluginRegistry.Register(new TurboPlugin(_settings.TurboSpeed));

        // Injecter le registry dans le BattleEngine
        var battleEngine = new BattleEngine(_pluginRegistry, _damageFormula, _difficultyMode);

        base.Initialize();
    }
}
```

---

## Règles plugins (NON NÉGOCIABLES)

1. Un plugin référence **uniquement** `SDK.Core` + `SDK.Battle` — jamais `SDK.MonoGame`
2. Chaque plugin = projet `.csproj` indépendant dans `src/SDK.Plugins.*`
3. `BattleEngine` ne référence jamais un plugin concret — uniquement `PluginRegistry`
4. Les hooks reçoivent `BattleState` et retournent `BattleState?` (null = pas de modification)
5. L'immuabilité de `BattleState` est préservée — les hooks utilisent `with` s'ils modifient
6. Tous les plugins doivent implémenter **tous** les hooks (no-op si non utilisé)
7. `PluginRegistry.Register()` lève si le même nom est déjà enregistré
