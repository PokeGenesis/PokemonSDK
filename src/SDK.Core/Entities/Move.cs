namespace SDK.Core.Entities;

using SDK.Core.Enums;

public class Move
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public int TypeId { get; set; }
    public MoveCategory Category { get; set; }
    public int? Power { get; set; }
    public int Accuracy { get; set; }
    public int PP { get; set; }
    public int Generation { get; set; }
    public PokemonType Type { get; set; } = null!;
}
