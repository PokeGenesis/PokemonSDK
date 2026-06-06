namespace SDK.Core.Entities;

public class VillainGroup
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public int Generation { get; set; }
    public ICollection<VillainMember> Members { get; set; } = new List<VillainMember>();
}
