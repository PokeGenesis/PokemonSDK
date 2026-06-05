namespace SDK.Core.Entities;

public class Trainer
{
    public int Id { get; set; }
    public string Identifier { get; set; } = string.Empty;
    public int Generation { get; set; }
    public ICollection<Badge> Badges { get; set; } = new List<Badge>();
}
