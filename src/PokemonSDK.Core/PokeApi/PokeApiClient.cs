using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace PokemonSDK.Core.PokeApi;

/// <summary>
/// Client for fetching Pokemon data from PokeAPI
/// </summary>
public class PokeApiClient
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://pokeapi.co/api/v2/";
    
    public PokeApiClient(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }
    
    /// <summary>
    /// Fetch a Pokemon species by ID or name
    /// </summary>
    public async Task<PokeApiPokemonSpecies?> GetPokemonSpeciesAsync(string idOrName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PokeApiPokemonSpecies>(
                $"pokemon-species/{idOrName}", 
                cancellationToken
            );
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Fetch Pokemon details by ID or name
    /// </summary>
    public async Task<PokeApiPokemon?> GetPokemonAsync(string idOrName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PokeApiPokemon>(
                $"pokemon/{idOrName}", 
                cancellationToken
            );
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Fetch a move by ID or name
    /// </summary>
    public async Task<PokeApiMove?> GetMoveAsync(string idOrName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PokeApiMove>(
                $"move/{idOrName}", 
                cancellationToken
            );
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Fetch an ability by ID or name
    /// </summary>
    public async Task<PokeApiAbility?> GetAbilityAsync(string idOrName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PokeApiAbility>(
                $"ability/{idOrName}", 
                cancellationToken
            );
        }
        catch
        {
            return null;
        }
    }
    
    /// <summary>
    /// Fetch list of Pokemon
    /// </summary>
    public async Task<PokeApiNamedResourceList?> GetPokemonListAsync(int limit = 151, int offset = 0, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<PokeApiNamedResourceList>(
                $"pokemon-species?limit={limit}&offset={offset}", 
                cancellationToken
            );
        }
        catch
        {
            return null;
        }
    }
}

#region PokeAPI Response Models

public class PokeApiNamedResourceList
{
    [JsonPropertyName("count")]
    public int Count { get; set; }
    
    [JsonPropertyName("results")]
    public List<PokeApiNamedResource> Results { get; set; } = new();
}

public class PokeApiNamedResource
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public class PokeApiPokemonSpecies
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("names")]
    public List<PokeApiName> Names { get; set; } = new();
    
    [JsonPropertyName("flavor_text_entries")]
    public List<PokeApiFlavorText> FlavorTextEntries { get; set; } = new();
    
    [JsonPropertyName("capture_rate")]
    public int CaptureRate { get; set; }
    
    [JsonPropertyName("base_happiness")]
    public int BaseHappiness { get; set; }
    
    [JsonPropertyName("growth_rate")]
    public PokeApiNamedResource? GrowthRate { get; set; }
    
    [JsonPropertyName("egg_groups")]
    public List<PokeApiNamedResource> EggGroups { get; set; } = new();
    
    [JsonPropertyName("evolution_chain")]
    public PokeApiUrlResource? EvolutionChain { get; set; }
}

public class PokeApiPokemon
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("types")]
    public List<PokeApiTypeSlot> Types { get; set; } = new();
    
    [JsonPropertyName("stats")]
    public List<PokeApiStat> Stats { get; set; } = new();
    
    [JsonPropertyName("abilities")]
    public List<PokeApiAbilitySlot> Abilities { get; set; } = new();
    
    [JsonPropertyName("moves")]
    public List<PokeApiMoveSlot> Moves { get; set; } = new();
}

public class PokeApiMove
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("names")]
    public List<PokeApiName> Names { get; set; } = new();
    
    [JsonPropertyName("flavor_text_entries")]
    public List<PokeApiFlavorText> FlavorTextEntries { get; set; } = new();
    
    [JsonPropertyName("type")]
    public PokeApiNamedResource? Type { get; set; }
    
    [JsonPropertyName("power")]
    public int? Power { get; set; }
    
    [JsonPropertyName("pp")]
    public int? Pp { get; set; }
    
    [JsonPropertyName("accuracy")]
    public int? Accuracy { get; set; }
    
    [JsonPropertyName("priority")]
    public int Priority { get; set; }
    
    [JsonPropertyName("damage_class")]
    public PokeApiNamedResource? DamageClass { get; set; }
}

public class PokeApiAbility
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("names")]
    public List<PokeApiName> Names { get; set; } = new();
    
    [JsonPropertyName("flavor_text_entries")]
    public List<PokeApiFlavorText> FlavorTextEntries { get; set; } = new();
}

public class PokeApiName
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("language")]
    public PokeApiNamedResource? Language { get; set; }
}

public class PokeApiFlavorText
{
    [JsonPropertyName("flavor_text")]
    public string FlavorText { get; set; } = string.Empty;
    
    [JsonPropertyName("language")]
    public PokeApiNamedResource? Language { get; set; }
}

public class PokeApiTypeSlot
{
    [JsonPropertyName("slot")]
    public int Slot { get; set; }
    
    [JsonPropertyName("type")]
    public PokeApiNamedResource? Type { get; set; }
}

public class PokeApiStat
{
    [JsonPropertyName("base_stat")]
    public int BaseStat { get; set; }
    
    [JsonPropertyName("stat")]
    public PokeApiNamedResource? Stat { get; set; }
}

public class PokeApiAbilitySlot
{
    [JsonPropertyName("is_hidden")]
    public bool IsHidden { get; set; }
    
    [JsonPropertyName("slot")]
    public int Slot { get; set; }
    
    [JsonPropertyName("ability")]
    public PokeApiNamedResource? Ability { get; set; }
}

public class PokeApiMoveSlot
{
    [JsonPropertyName("move")]
    public PokeApiNamedResource? Move { get; set; }
}

public class PokeApiUrlResource
{
    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

#endregion
