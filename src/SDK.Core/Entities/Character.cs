namespace SDK.Core.Entities;

public class Character
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public int Generation { get; set; }
    public ICollection<VillainMember> VillainMemberships { get; set; } = new List<VillainMember>();
}
