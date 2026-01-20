namespace PokemonSDK.Core.Events;

/// <summary>
/// Base class for all game events
/// </summary>
public abstract class GameEvent
{
    public Guid Id { get; }
    public DateTime Timestamp { get; }
    
    protected GameEvent()
    {
        Id = Guid.NewGuid();
        Timestamp = DateTime.UtcNow;
    }
}

/// <summary>
/// Event raised when a Pokemon is caught
/// </summary>
public class PokemonCaughtEvent : GameEvent
{
    public Models.Pokemon Pokemon { get; set; } = null!;
    public int TrainerId { get; set; }
}

/// <summary>
/// Event raised when a Pokemon evolves
/// </summary>
public class PokemonEvolvedEvent : GameEvent
{
    public Models.Pokemon Pokemon { get; set; } = null!;
    public int OldSpeciesId { get; set; }
    public int NewSpeciesId { get; set; }
}

/// <summary>
/// Event raised when a Pokemon levels up
/// </summary>
public class PokemonLevelUpEvent : GameEvent
{
    public Models.Pokemon Pokemon { get; set; } = null!;
    public int OldLevel { get; set; }
    public int NewLevel { get; set; }
}

/// <summary>
/// Event raised when a battle starts
/// </summary>
public class BattleStartedEvent : GameEvent
{
    public Guid BattleId { get; set; }
    public Battle.BattleType BattleType { get; set; }
}

/// <summary>
/// Event raised when a battle ends
/// </summary>
public class BattleEndedEvent : GameEvent
{
    public Guid BattleId { get; set; }
    public bool PlayerWon { get; set; }
    public int ExperienceGained { get; set; }
    public int MoneyGained { get; set; }
}

/// <summary>
/// Event raised when an item is obtained
/// </summary>
public class ItemObtainedEvent : GameEvent
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public int TrainerId { get; set; }
}
