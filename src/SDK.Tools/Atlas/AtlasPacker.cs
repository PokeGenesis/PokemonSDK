namespace SDK.Tools.Atlas;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.Json;

public sealed class AtlasPacker
{
    private const int AtlasMaxWidth = 1024;

    public IReadOnlyList<AtlasEntry> Pack(IEnumerable<string> filePaths, string outputDir)
    {
        var paths = filePaths.ToList();
        if (paths.Count == 0)
            return [];

        var placements = new List<(string path, int x, int y, int w, int h, string assetKey, string view)>();
        int curX = 0, curY = 0, rowH = 0;

        foreach (var path in paths)
        {
            using var img = Image.Load<Rgba32>(path);
            int w = img.Width, h = img.Height;

            if (curX + w > AtlasMaxWidth)
            {
                curX = 0;
                curY += rowH;
                rowH = 0;
            }

            rowH = Math.Max(rowH, h);

            var fn = Path.GetFileNameWithoutExtension(path);
            var lastUnderscore = fn.LastIndexOf('_');
            var view = lastUnderscore >= 0 ? fn[(lastUnderscore + 1)..] : "unknown";
            var assetKey = fn;

            placements.Add((path, curX, curY, w, h, assetKey, view));
            curX += w;
        }

        int totalW = placements.Max(p => p.x + p.w);
        int totalH = placements.Max(p => p.y + p.h);
        int atlasW = NextPow2(Math.Min(AtlasMaxWidth, totalW));
        int atlasH = NextPow2(totalH);

        Directory.CreateDirectory(outputDir);

        using var atlas = new Image<Rgba32>(atlasW, atlasH);
        var result = new List<AtlasEntry>();

        foreach (var (path, x, y, w, h, assetKey, view) in placements)
        {
            using var sprite = Image.Load<Rgba32>(path);
            atlas.Mutate(ctx => ctx.DrawImage(sprite, new Point(x, y), 1f));
            result.Add(new AtlasEntry(assetKey, view, x, y, w, h));
        }

        atlas.SaveAsPng(Path.Combine(outputDir, "atlas.png"));

        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(Path.Combine(outputDir, "atlas-manifest.json"), json);

        return result;
    }

    private static int NextPow2(int n)
    {
        if (n <= 0) return 1;
        int p = 1;
        while (p < n) p <<= 1;
        return p;
    }
}
