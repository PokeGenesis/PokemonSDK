using Microsoft.EntityFrameworkCore;
using PokemonSDK.Core.Database;
using PokemonSDK.Core.Models;
using PokemonSDK.Core.PokeApi;
using PokemonSDK.Core.Enums;
using PokemonSDK.Core.Localization;

namespace PokemonSDK.Core.Data;

/// <summary>
/// Enhanced data manager with PokeAPI integration and database storage
/// </summary>
public class EnhancedDataManager
{
    private readonly PokemonDbContext _dbContext;
    private readonly PokeApiClient _pokeApiClient;
    
    public EnhancedDataManager(PokemonDbContext dbContext, PokeApiClient? pokeApiClient = null)
    {
        _dbContext = dbContext;
        _pokeApiClient = pokeApiClient ?? new PokeApiClient();
    }
    
    /// <summary>
    /// Import Pokemon species from PokeAPI to database
    /// </summary>
    public async Task<PokemonSpecies?> ImportSpeciesFromPokeApiAsync(int id, CancellationToken cancellationToken = default)
    {
        // Fetch from PokeAPI
        var apiSpecies = await _pokeApiClient.GetPokemonSpeciesAsync(id.ToString(), cancellationToken);
        var apiPokemon = await _pokeApiClient.GetPokemonAsync(id.ToString(), cancellationToken);
        
        if (apiSpecies == null || apiPokemon == null)
            return null;
        
        // Check if already exists
        var existingSpecies = await _dbContext.Species.FindAsync(id);
        if (existingSpecies != null)
            return existingSpecies;
        
        // Map to our model
        var species = new PokemonSpecies
        {
            Id = apiSpecies.Id,
            Name = apiSpecies.Name,
            NameFrench = GetLocalizedName(apiSpecies.Names, "fr"),
            NameSpanish = GetLocalizedName(apiSpecies.Names, "es"),
            Description = GetLocalizedFlavorText(apiSpecies.FlavorTextEntries, "en"),
            DescriptionFrench = GetLocalizedFlavorText(apiSpecies.FlavorTextEntries, "fr"),
            DescriptionSpanish = GetLocalizedFlavorText(apiSpecies.FlavorTextEntries, "es"),
            CatchRate = apiSpecies.CaptureRate,
            GrowthRate = apiSpecies.GrowthRate?.Name ?? "medium-fast",
            EggGroups = apiSpecies.EggGroups.Select(eg => eg.Name).ToList(),
            Generation = DetermineGeneration(apiSpecies.Id),
            IsEnabled = true
        };
        
        // Map stats
        if (apiPokemon.Stats.Count >= 6)
        {
            species.BaseHP = apiPokemon.Stats.First(s => s.Stat?.Name == "hp").BaseStat;
            species.BaseAttack = apiPokemon.Stats.First(s => s.Stat?.Name == "attack").BaseStat;
            species.BaseDefense = apiPokemon.Stats.First(s => s.Stat?.Name == "defense").BaseStat;
            species.BaseSpecialAttack = apiPokemon.Stats.First(s => s.Stat?.Name == "special-attack").BaseStat;
            species.BaseSpecialDefense = apiPokemon.Stats.First(s => s.Stat?.Name == "special-defense").BaseStat;
            species.BaseSpeed = apiPokemon.Stats.First(s => s.Stat?.Name == "speed").BaseStat;
        }
        
        // Map types
        if (apiPokemon.Types.Count > 0)
        {
            species.PrimaryType = MapPokemonType(apiPokemon.Types.First().Type?.Name ?? "normal");
            if (apiPokemon.Types.Count > 1)
            {
                species.SecondaryType = MapPokemonType(apiPokemon.Types.Last().Type?.Name ?? "normal");
            }
        }
        
        // Save to database
        _dbContext.Species.Add(species);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return species;
    }
    
    /// <summary>
    /// Import move from PokeAPI to database
    /// </summary>
    public async Task<Move?> ImportMoveFromPokeApiAsync(int id, CancellationToken cancellationToken = default)
    {
        var apiMove = await _pokeApiClient.GetMoveAsync(id.ToString(), cancellationToken);
        if (apiMove == null)
            return null;
        
        var existingMove = await _dbContext.Moves.FindAsync(id);
        if (existingMove != null)
            return existingMove;
        
        var move = new Move
        {
            Id = apiMove.Id,
            Name = apiMove.Name,
            NameFrench = GetLocalizedName(apiMove.Names, "fr"),
            NameSpanish = GetLocalizedName(apiMove.Names, "es"),
            Description = GetLocalizedFlavorText(apiMove.FlavorTextEntries, "en"),
            DescriptionFrench = GetLocalizedFlavorText(apiMove.FlavorTextEntries, "fr"),
            DescriptionSpanish = GetLocalizedFlavorText(apiMove.FlavorTextEntries, "es"),
            Type = MapPokemonType(apiMove.Type?.Name ?? "normal"),
            Power = apiMove.Power ?? 0,
            PP = apiMove.Pp ?? 0,
            Accuracy = apiMove.Accuracy ?? 100,
            Priority = apiMove.Priority,
            Category = MapMoveCategory(apiMove.DamageClass?.Name ?? "status"),
            IsEnabled = true
        };
        
        _dbContext.Moves.Add(move);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return move;
    }
    
    /// <summary>
    /// Import ability from PokeAPI to database
    /// </summary>
    public async Task<Ability?> ImportAbilityFromPokeApiAsync(int id, CancellationToken cancellationToken = default)
    {
        var apiAbility = await _pokeApiClient.GetAbilityAsync(id.ToString(), cancellationToken);
        if (apiAbility == null)
            return null;
        
        var existingAbility = await _dbContext.Abilities.FindAsync(id);
        if (existingAbility != null)
            return existingAbility;
        
        var ability = new Ability
        {
            Id = apiAbility.Id,
            Name = apiAbility.Name,
            NameFrench = GetLocalizedName(apiAbility.Names, "fr"),
            NameSpanish = GetLocalizedName(apiAbility.Names, "es"),
            Description = GetLocalizedFlavorText(apiAbility.FlavorTextEntries, "en"),
            DescriptionFrench = GetLocalizedFlavorText(apiAbility.FlavorTextEntries, "fr"),
            DescriptionSpanish = GetLocalizedFlavorText(apiAbility.FlavorTextEntries, "es"),
            IsEnabled = true
        };
        
        _dbContext.Abilities.Add(ability);
        await _dbContext.SaveChangesAsync(cancellationToken);
        
        return ability;
    }
    
    /// <summary>
    /// Get filtered Pokemon species from database
    /// </summary>
    public IQueryable<PokemonSpecies> GetFilteredSpecies(DataFilterOptions? options = null)
    {
        var query = _dbContext.Species.AsQueryable();
        
        if (options?.EnabledOnly == true)
        {
            query = query.Where(s => s.IsEnabled);
        }
        
        if (!string.IsNullOrEmpty(options?.Generation))
        {
            query = query.Where(s => s.Generation == options.Generation);
        }
        
        if (!string.IsNullOrEmpty(options?.TypeFilter))
        {
            // Parse type filter
            if (Enum.TryParse<PokemonType>(options.TypeFilter, true, out var typeEnum))
            {
                query = query.Where(s => s.PrimaryType == typeEnum || s.SecondaryType == typeEnum);
            }
        }
        
        return query;
    }
    
    /// <summary>
    /// Get filtered moves from database
    /// </summary>
    public IQueryable<Move> GetFilteredMoves(DataFilterOptions? options = null)
    {
        var query = _dbContext.Moves.AsQueryable();
        
        if (options?.EnabledOnly == true)
        {
            query = query.Where(m => m.IsEnabled);
        }
        
        if (!string.IsNullOrEmpty(options?.TypeFilter))
        {
            if (Enum.TryParse<PokemonType>(options.TypeFilter, true, out var typeEnum))
            {
                query = query.Where(m => m.Type == typeEnum);
            }
        }
        
        return query;
    }
    
    /// <summary>
    /// Get filtered abilities from database
    /// </summary>
    public IQueryable<Ability> GetFilteredAbilities(DataFilterOptions? options = null)
    {
        var query = _dbContext.Abilities.AsQueryable();
        
        if (options?.EnabledOnly == true)
        {
            query = query.Where(a => a.IsEnabled);
        }
        
        return query;
    }
    
    /// <summary>
    /// Get filtered items from database
    /// </summary>
    public IQueryable<Inventory.Item> GetFilteredItems(DataFilterOptions? options = null)
    {
        var query = _dbContext.Items.AsQueryable();
        
        if (options?.EnabledOnly == true)
        {
            query = query.Where(i => i.IsEnabled);
        }
        
        return query;
    }
    
    private string GetLocalizedName(List<PokeApiName> names, string languageCode)
    {
        var name = names.FirstOrDefault(n => n.Language?.Name == languageCode);
        return name?.Name ?? string.Empty;
    }
    
    private string GetLocalizedFlavorText(List<PokeApiFlavorText> flavorTexts, string languageCode)
    {
        var text = flavorTexts.FirstOrDefault(f => f.Language?.Name == languageCode);
        if (text == null) return string.Empty;
        
        // Clean up formatting characters
        return System.Text.RegularExpressions.Regex.Replace(text.FlavorText, @"[\n\f]+", " ");
    }
    
    private PokemonType MapPokemonType(string apiTypeName) => apiTypeName.ToLower() switch
    {
        "normal" => PokemonType.Normal,
        "fighting" => PokemonType.Fighting,
        "flying" => PokemonType.Flying,
        "poison" => PokemonType.Poison,
        "ground" => PokemonType.Ground,
        "rock" => PokemonType.Rock,
        "bug" => PokemonType.Bug,
        "ghost" => PokemonType.Ghost,
        "steel" => PokemonType.Steel,
        "fire" => PokemonType.Fire,
        "water" => PokemonType.Water,
        "grass" => PokemonType.Grass,
        "electric" => PokemonType.Electric,
        "psychic" => PokemonType.Psychic,
        "ice" => PokemonType.Ice,
        "dragon" => PokemonType.Dragon,
        "dark" => PokemonType.Dark,
        "fairy" => PokemonType.Fairy,
        _ => PokemonType.Normal
    };
    
    private MoveCategory MapMoveCategory(string apiCategoryName) => apiCategoryName.ToLower() switch
    {
        "physical" => MoveCategory.Physical,
        "special" => MoveCategory.Special,
        _ => MoveCategory.Status
    };
    
    private string DetermineGeneration(int id)
    {
        if (id <= 151) return "1";
        if (id <= 251) return "2";
        if (id <= 386) return "3";
        if (id <= 493) return "4";
        if (id <= 649) return "5";
        if (id <= 721) return "6";
        if (id <= 809) return "7";
        if (id <= 905) return "8";
        return "9";
    }
}
