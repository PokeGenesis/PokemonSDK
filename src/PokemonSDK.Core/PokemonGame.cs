using PokemonSDK.Core.Data;
using PokemonSDK.Core.Events;
using PokemonSDK.Core.Models;
using PokemonSDK.Core.Battle;
using PokemonSDK.Core.Enums;

namespace PokemonSDK.Core;

/// <summary>
/// Main game class that manages the Pokemon game state and systems
/// </summary>
public class PokemonGame
{
    public DataManager DataManager { get; }
    public SaveManager SaveManager { get; }
    public EventManager EventManager { get; }
    public Trainer? Player { get; private set; }
    public Battle.Battle? CurrentBattle { get; private set; }
    
    public PokemonGame()
    {
        DataManager = new DataManager();
        SaveManager = new SaveManager();
        EventManager = new EventManager();
    }
    
    /// <summary>
    /// Initialize a new game with a player
    /// </summary>
    public void NewGame(string playerName, Gender gender)
    {
        Player = new Trainer
        {
            Id = 1,
            Name = playerName,
            Gender = gender,
            Money = 3000
        };
        
        // Subscribe to game events for logging or other purposes
        SetupEventHandlers();
    }
    
    /// <summary>
    /// Load a saved game
    /// </summary>
    public bool LoadGame(string savePath)
    {
        Player = SaveManager.LoadTrainer(savePath);
        
        if (Player != null)
        {
            SetupEventHandlers();
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Save the current game
    /// </summary>
    public void SaveGame(string savePath)
    {
        if (Player != null)
        {
            SaveManager.SaveTrainer(Player, savePath);
        }
    }
    
    /// <summary>
    /// Start a wild Pokemon battle
    /// </summary>
    public Battle.Battle StartWildBattle(Pokemon wildPokemon)
    {
        if (Player == null)
        {
            throw new InvalidOperationException("No player initialized");
        }
        
        CurrentBattle = new Battle.Battle
        {
            BattleType = BattleType.Wild,
            Participants = new List<BattleParticipant>
            {
                new BattleParticipant
                {
                    TrainerId = Player.Id,
                    Name = Player.Name,
                    Party = Player.Party,
                    ActivePokemon = Player.Party.FirstOrDefault(),
                    IsPlayer = true
                },
                new BattleParticipant
                {
                    Name = "Wild Pokemon",
                    Party = new List<Pokemon> { wildPokemon },
                    ActivePokemon = wildPokemon,
                    IsPlayer = false
                }
            }
        };
        
        CurrentBattle.Start();
        
        EventManager.Publish(new BattleStartedEvent
        {
            BattleId = CurrentBattle.Id,
            BattleType = BattleType.Wild
        });
        
        return CurrentBattle;
    }
    
    /// <summary>
    /// Start a trainer battle
    /// </summary>
    public Battle.Battle StartTrainerBattle(Trainer opponent)
    {
        if (Player == null)
        {
            throw new InvalidOperationException("No player initialized");
        }
        
        CurrentBattle = new Battle.Battle
        {
            BattleType = BattleType.Trainer,
            Participants = new List<BattleParticipant>
            {
                new BattleParticipant
                {
                    TrainerId = Player.Id,
                    Name = Player.Name,
                    Party = Player.Party,
                    ActivePokemon = Player.Party.FirstOrDefault(),
                    IsPlayer = true
                },
                new BattleParticipant
                {
                    TrainerId = opponent.Id,
                    Name = opponent.Name,
                    Party = opponent.Party,
                    ActivePokemon = opponent.Party.FirstOrDefault(),
                    IsPlayer = false
                }
            }
        };
        
        CurrentBattle.Start();
        
        EventManager.Publish(new BattleStartedEvent
        {
            BattleId = CurrentBattle.Id,
            BattleType = BattleType.Trainer
        });
        
        return CurrentBattle;
    }
    
    /// <summary>
    /// Add a Pokemon to the player's party or PC
    /// </summary>
    public void AddPokemon(Pokemon pokemon)
    {
        if (Player == null)
        {
            throw new InvalidOperationException("No player initialized");
        }
        
        if (Player.Party.Count < 6)
        {
            Player.Party.Add(pokemon);
        }
        else
        {
            Player.PC.Add(pokemon);
        }
        
        EventManager.Publish(new PokemonCaughtEvent
        {
            Pokemon = pokemon,
            TrainerId = Player.Id
        });
    }
    
    private void SetupEventHandlers()
    {
        // Example: Log all events (can be customized)
        EventManager.Subscribe<PokemonCaughtEvent>(e => 
        {
            Console.WriteLine($"Pokemon caught: {e.Pokemon.Nickname}");
        });
        
        EventManager.Subscribe<BattleStartedEvent>(e =>
        {
            Console.WriteLine($"Battle started: {e.BattleType}");
        });
    }
}
