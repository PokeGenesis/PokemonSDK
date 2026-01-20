using PokemonSDK.Core.Models;

namespace PokemonSDK.Core.Battle;

/// <summary>
/// Represents a battle between trainers or wild Pokemon
/// </summary>
public class Battle
{
    public Guid Id { get; }
    public BattleType BattleType { get; set; }
    public List<BattleParticipant> Participants { get; set; } = new();
    public BattleState State { get; set; }
    public int CurrentTurn { get; set; }
    public Weather Weather { get; set; }
    public Terrain Terrain { get; set; }
    public List<BattleEvent> BattleLog { get; set; } = new();
    
    public Battle()
    {
        Id = Guid.NewGuid();
        State = BattleState.Starting;
        Weather = Weather.None;
        Terrain = Terrain.None;
    }
    
    /// <summary>
    /// Start the battle
    /// </summary>
    public void Start()
    {
        State = BattleState.SelectingAction;
        CurrentTurn = 1;
        AddBattleEvent("Battle started!");
    }
    
    /// <summary>
    /// End the battle
    /// </summary>
    public void End(int winningParticipantIndex)
    {
        State = BattleState.Ended;
        AddBattleEvent($"Battle ended! Winner: Participant {winningParticipantIndex}");
    }
    
    private void AddBattleEvent(string message)
    {
        BattleLog.Add(new BattleEvent
        {
            Turn = CurrentTurn,
            Message = message,
            Timestamp = DateTime.UtcNow
        });
    }
}

/// <summary>
/// Represents a participant in a battle (trainer or wild Pokemon)
/// </summary>
public class BattleParticipant
{
    public int TrainerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Pokemon> Party { get; set; } = new();
    public Pokemon? ActivePokemon { get; set; }
    public bool IsPlayer { get; set; }
}

/// <summary>
/// Represents the state of a battle
/// </summary>
public enum BattleState
{
    Starting = 0,
    SelectingAction = 1,
    ExecutingActions = 2,
    Ended = 3
}

/// <summary>
/// Represents types of battles
/// </summary>
public enum BattleType
{
    Wild = 0,
    Trainer = 1,
    Double = 2,
    Multi = 3
}

/// <summary>
/// Represents weather conditions in battle
/// </summary>
public enum Weather
{
    None = 0,
    Rain = 1,
    HarshSunlight = 2,
    Sandstorm = 3,
    Hail = 4,
    StrongWinds = 5
}

/// <summary>
/// Represents terrain effects in battle
/// </summary>
public enum Terrain
{
    None = 0,
    Electric = 1,
    Grassy = 2,
    Misty = 3,
    Psychic = 4
}

/// <summary>
/// Represents an event that occurred in battle
/// </summary>
public class BattleEvent
{
    public int Turn { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
