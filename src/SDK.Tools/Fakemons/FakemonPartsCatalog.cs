namespace SDK.Tools.Fakemons;

using System.Text.Json;

public sealed class FakemonPartsCatalog
{
    private const int MaxPngLimit = 500;
    private readonly Dictionary<string, JsonDocument> _metadata = [];

    public IReadOnlyList<FakemonPartLayer> Layers { get; private set; } = [];

    public static FakemonPartsCatalog Scan(string directory)
    {
        var instance = new FakemonPartsCatalog();
        var pngPaths = Directory.EnumerateFiles(directory, "*.png", SearchOption.AllDirectories).ToList();

        if (pngPaths.Count > MaxPngLimit)
        {
            Console.WriteLine("[WARN] catalog scan dépasse 500 PNG, tronqué");
            pngPaths = pngPaths.Take(MaxPngLimit).ToList();
        }

        var layers = new List<FakemonPartLayer>();
        foreach (var path in pngPaths)
        {
            var sidecarPath = Path.ChangeExtension(path, ".json");
            int zOrder = 0;

            if (File.Exists(sidecarPath))
            {
                var doc = JsonDocument.Parse(File.ReadAllText(sidecarPath));
                instance._metadata[path] = doc;
                if (doc.RootElement.TryGetProperty("z-order", out var zProp))
                    zOrder = zProp.GetInt32();
            }

            layers.Add(new FakemonPartLayer(path, zOrder));
        }

        instance.Layers = layers;
        return instance;
    }

    public JsonDocument? GetMetadata(string path)
        => _metadata.TryGetValue(path, out var doc) ? doc : null;
}
