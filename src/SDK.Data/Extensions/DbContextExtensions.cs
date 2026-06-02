namespace SDK.Data.Extensions;

using Microsoft.EntityFrameworkCore;
using SDK.Core.Entities;

public static class DbContextExtensions
{
    // Espèces introduites jusqu'à la génération maxGeneration (<=)
    public static IQueryable<PokemonSpecies> GetSpeciesByGeneration(
        this PokemonDbContext ctx, int maxGeneration)
        => ctx.PokemonSpecies.Where(s => s.Generation <= maxGeneration);

    // Types disponibles jusqu'à la génération maxGeneration (<=)
    // Ex : Fée (gen=6) exclue si maxGeneration=5
    public static IQueryable<PokemonType> GetTypesByGeneration(
        this PokemonDbContext ctx, int maxGeneration)
        => ctx.PokemonTypes.Where(t => t.Generation <= maxGeneration);

    // Toutes les traductions pour une entité + locale
    public static IQueryable<Translation> GetTranslations(
        this PokemonDbContext ctx, string entityType, int entityId, string locale)
        => ctx.Translations.Where(t =>
            t.EntityType == entityType &&
            t.EntityId == entityId &&
            t.Locale == locale);

    // Raccourci : valeur d'un champ unique (null si absent)
    public static string? GetTranslation(
        this PokemonDbContext ctx, string entityType, int entityId, string locale, string field)
        => ctx.Translations
            .Where(t => t.EntityType == entityType && t.EntityId == entityId &&
                        t.Locale == locale && t.Field == field)
            .Select(t => t.Value)
            .FirstOrDefault();
}
