namespace SDK.Core.Entities;

public class VillainMember
{
    public int Id { get; set; }
    public int CharacterId { get; set; }
    public Character Character { get; set; } = null!;
    public int VillainGroupId { get; set; }
    public VillainGroup VillainGroup { get; set; } = null!;
}
