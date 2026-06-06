namespace SDK.Battle.Plugins;

using SDK.Core.Interfaces;
using SDK.Core.ValueObjects;

public sealed class PluginRegistry
{
    private readonly List<IPlugin> _plugins = [];
    private readonly List<IBattlePlugin> _battlePlugins = [];

    public void Register(IPlugin plugin)
    {
        if (_plugins.Any(p => p.Name == plugin.Name))
            throw new InvalidOperationException(
                $"Plugin '{plugin.Name}' is already registered.");
        _plugins.Add(plugin);
        if (plugin is IBattlePlugin bp) _battlePlugins.Add(bp);
    }

    public void Unregister(string name)
    {
        _plugins.RemoveAll(p => p.Name == name);
        _battlePlugins.RemoveAll(p => p.Name == name);
    }

    public bool IsRegistered(string name) =>
        _plugins.Any(p => p.Name == name);

    // Observers (void) — dispatch vers IBattlePlugin uniquement
    public void NotifyBattleStart(BattleState state)
    {
        foreach (var p in _battlePlugins) p.OnBattleStart(state);
    }

    public void NotifyTurnStart(BattleState state)
    {
        foreach (var p in _battlePlugins) p.OnTurnStart(state);
    }

    public void NotifyTurnEnd(BattleState state)
    {
        foreach (var p in _battlePlugins) p.OnTurnEnd(state);
    }

    public void NotifyBattleEnd(BattleState s, BattleResult r)
    {
        foreach (var p in _battlePlugins) p.OnBattleEnd(s, r);
    }

    public void NotifyFainted(BattleState s, BattlePokemon f)
    {
        foreach (var p in _battlePlugins) p.OnPokemonFainted(s, f);
    }

    public void NotifyCaught(BattleState s, BattlePokemon c, string zone)
    {
        foreach (var p in _battlePlugins) p.OnPokemonCaught(s, c, zone);
    }

    // Chain state — null retour d'un plugin = pas de modif, passe au suivant
    public BattleState ApplyBeforeMove(BattleState state, BattleAction action)
    {
        foreach (var next in _battlePlugins
            .Select(p => p.OnBeforeMove(state, action))
            .Where(n => n is not null))
            state = next!;
        return state;
    }

    public BattleState ApplyBeforeDamage(BattleState state, DamageResult damage)
    {
        foreach (var next in _battlePlugins
            .Select(p => p.OnBeforeDamage(state, damage))
            .Where(n => n is not null))
            state = next!;
        return state;
    }
}
