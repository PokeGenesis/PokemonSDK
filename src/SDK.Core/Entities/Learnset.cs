namespace SDK.Core.Entities;

public class Learnset
{
    public int Id { get; set; }
    public int SpeciesId { get; set; }
    public int MoveId { get; set; }
    public int LearnLevel { get; set; }
    public int Generation { get; set; }
    public PokemonSpecies Species { get; set; } = null!;
    public Move Move { get; set; } = null!;
}
