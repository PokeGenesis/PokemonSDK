namespace SDK.Tools.Fakemons;

using SDK.Core.Entities;
using SDK.Data;
using SDK.Tools.Fakemons.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Text.Json;

public static class FakemonExporter
{
    private static readonly string[] RequiredLocales = ["en", "es", "fr", "de", "it", "ja"];

    public static async Task<string> ExportAsync(Image<Rgba32> image, FakemonAssemblyOptions opts, PokemonDbContext ctx)
    {
        if (ctx.FakemonSpecies.Any(f => f.Identifier == opts.Identifier))
            throw new FakemonAssemblyException($"Fakemon '{opts.Identifier}' existe déjà en DB");

        var entity = new FakemonSpecies
        {
            Identifier = opts.Identifier,
            Generation = opts.Generation,
            Type1Id = opts.Type1Id,
            Type2Id = opts.Type2Id,
            EggGroup1 = opts.EggGroup1,
            EggGroup2 = opts.EggGroup2,
            IsLegendary = opts.IsLegendary,
        };

        ctx.FakemonSpecies.Add(entity);
        await ctx.SaveChangesAsync();

        // PNG écrit après l'insert DB — pas de fichier orphelin si l'insert échoue
        Directory.CreateDirectory(opts.OutputDirectory);
        var outputPath = Path.Combine(opts.OutputDirectory, $"fk_{opts.Identifier}_front.png");
        await image.SaveAsPngAsync(outputPath);

        Dictionary<string, string>? translationMap = null;
        if (opts.TranslationsJsonPath is not null && File.Exists(opts.TranslationsJsonPath))
        {
            var json = await File.ReadAllTextAsync(opts.TranslationsJsonPath);
            translationMap = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        }

        foreach (var locale in RequiredLocales)
        {
            var value = translationMap?.TryGetValue(locale, out var v) == true ? v! : opts.Identifier;
            ctx.Translations.Add(new Translation
            {
                EntityType = "FakemonSpecies",
                EntityId = entity.Id,
                Locale = locale,
                Field = "name",
                Value = value
            });
        }

        await ctx.SaveChangesAsync();
        return outputPath;
    }
}
