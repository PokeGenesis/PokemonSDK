namespace SDK.Core.Entities;

public class Badge
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public int Generation { get; set; }
    public int GymLeaderId { get; set; }
    public Trainer GymLeader { get; set; } = null!;
}
