using PokemonSDK.Core.Models;

namespace PokemonSDK.Core.Battle;

/// <summary>
/// Represents an action a Pokemon can take in battle
/// </summary>
public abstract class BattleAction
{
    public int Priority { get; set; }
    public Pokemon User { get; set; } = null!;
    
    public abstract void Execute(Battle battle);
}

/// <summary>
/// Represents using a move in battle
/// </summary>
public class MoveAction : BattleAction
{
    public Move Move { get; set; } = null!;
    public Pokemon Target { get; set; } = null!;
    
    public override void Execute(Battle battle)
    {
        // Move execution logic would go here
        // This is simplified for the core SDK
        Priority = Move.Priority;
    }
}

/// <summary>
/// Represents switching Pokemon
/// </summary>
public class SwitchAction : BattleAction
{
    public Pokemon SwitchTo { get; set; } = null!;
    
    public override void Execute(Battle battle)
    {
        // Switch logic would go here
        Priority = 6; // Switches have highest priority
    }
}

/// <summary>
/// Represents using an item in battle
/// </summary>
public class ItemAction : BattleAction
{
    public int ItemId { get; set; }
    public Pokemon? Target { get; set; }
    
    public override void Execute(Battle battle)
    {
        // Item usage logic would go here
        Priority = 6; // Items have high priority
    }
}

/// <summary>
/// Represents fleeing from battle
/// </summary>
public class FleeAction : BattleAction
{
    public override void Execute(Battle battle)
    {
        // Flee logic would go here
        Priority = 6; // Fleeing has high priority
    }
}
