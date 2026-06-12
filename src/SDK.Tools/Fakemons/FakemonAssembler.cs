namespace SDK.Tools.Fakemons;

using SDK.Tools.Fakemons.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

public static class FakemonAssembler
{
    public static Image<Rgba32> Assemble(IReadOnlyList<FakemonPartLayer> layers)
    {
        if (layers.Count == 0)
            throw new FakemonAssemblyException("Aucune couche à assembler");

        var loaded = new List<(Image<Rgba32> img, int zOrder)>();
        try
        {
            foreach (var layer in layers)
            {
                if (!File.Exists(layer.Path))
                    throw new FakemonAssemblyException($"Partie manquante : {layer.Path}");
                loaded.Add((Image.Load<Rgba32>(layer.Path), layer.ZOrder));
            }

            var sorted = loaded.OrderBy(l => l.zOrder).ToList();

            int width = sorted.Max(l => l.img.Width);
            int height = sorted.Max(l => l.img.Height);

            var output = new Image<Rgba32>(width, height);
            foreach (var (img, _) in sorted)
                output.Mutate(ctx => ctx.DrawImage(img, new Point(0, 0), 1f));

            return output;
        }
        finally
        {
            foreach (var (img, _) in loaded)
                img.Dispose();
        }
    }
}
