namespace SDK.Tools.Fakemons;

using SDK.Data;
using SDK.Tools.Fakemons.Models;

public static class FakemonAssemblyPipeline
{
    public static async Task<string> RunAsync(FakemonAssemblyOptions opts, PokemonDbContext ctx)
    {
        var catalogInstance = FakemonPartsCatalog.Scan(opts.PartsDirectory);
        var catalog = catalogInstance.Layers;

        var filtered = FakemonFilter.Apply(catalog, catalogInstance, opts.FilterExpression);

        if (filtered.Count == 0)
        {
            Console.WriteLine("[WARN] Aucune partie compatible avec le filtre");
            if (opts.Strict)
                throw new FakemonAssemblyException("0 parties après filtre, --strict activé");
            return string.Empty;
        }

        using var image = FakemonAssembler.Assemble(filtered);
        return await FakemonExporter.ExportAsync(image, opts, ctx);
    }
}
