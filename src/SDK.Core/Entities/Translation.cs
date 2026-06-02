namespace SDK.Core.Entities;

// UNIQUE(EntityType, EntityId, Locale, Field) — collision = erreur, pas d'override silencieux
public class Translation
{
    public int Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string Locale { get; set; } = string.Empty;
    public string Field { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
