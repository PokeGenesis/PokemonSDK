namespace SDK.Tools.Tests;

using FluentAssertions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SDK.Tools.Atlas;
using System.Text.Json;

public sealed class AtlasPackerTests : IDisposable
{
    private readonly string _tempDir;

    public AtlasPackerTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"atlastest_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private string CreatePng(string name, int w, int h)
    {
        var path = Path.Combine(_tempDir, name);
        using var img = new Image<Rgba32>(w, h);
        img.SaveAsPng(path);
        return path;
    }

    private string OutputDir => Path.Combine(_tempDir, "out");

    [Fact]
    public void Pack_SingleSprite_CreatesAtlasAndManifest()
    {
        var packer = new AtlasPacker();
        var png = CreatePng("00025_pikachu_front.png", 96, 96);

        var entries = packer.Pack([png], OutputDir);

        entries.Should().HaveCount(1);
        File.Exists(Path.Combine(OutputDir, "atlas.png")).Should().BeTrue();
        File.Exists(Path.Combine(OutputDir, "atlas-manifest.json")).Should().BeTrue();
    }

    [Fact]
    public void Pack_SingleSprite_EntryCoordinatesCorrect()
    {
        var packer = new AtlasPacker();
        var png = CreatePng("00001_bulbasaur_front.png", 96, 96);

        var entries = packer.Pack([png], OutputDir);

        var e = entries[0];
        e.X.Should().Be(0);
        e.Y.Should().Be(0);
        e.Width.Should().Be(96);
        e.Height.Should().Be(96);
    }

    [Fact]
    public void Pack_MultipleSprites_DifferentCoordinates()
    {
        var packer = new AtlasPacker();
        var p1 = CreatePng("00025_pikachu_front.png", 96, 96);
        var p2 = CreatePng("00025_pikachu_back.png", 96, 96);
        var p3 = CreatePng("00025_pikachu_icon.png", 32, 32);

        var entries = packer.Pack([p1, p2, p3], OutputDir);

        entries.Should().HaveCount(3);
        // Sprites packés côte à côte — x différents
        entries[0].X.Should().Be(0);
        entries[1].X.Should().Be(96);
        entries[2].X.Should().Be(192);
    }

    [Fact]
    public void Pack_AtlasDimensions_PowerOfTwo()
    {
        var packer = new AtlasPacker();
        // 3 sprites de 96×96 → total width 288 → NextPow2 = 512
        var pngs = new[]
        {
            CreatePng("00001_bulbasaur_front.png", 96, 96),
            CreatePng("00002_ivysaur_front.png", 96, 96),
            CreatePng("00003_venusaur_front.png", 96, 96),
        };

        packer.Pack(pngs, OutputDir);

        using var atlas = Image.Load(Path.Combine(OutputDir, "atlas.png"));
        IsPowerOfTwo(atlas.Width).Should().BeTrue();
        IsPowerOfTwo(atlas.Height).Should().BeTrue();
    }

    [Fact]
    public void Pack_StripsWrap_WhenExceedsMaxWidth()
    {
        var packer = new AtlasPacker();
        // 11 sprites × 96px = 1056 > 1024 → wrap
        var pngs = Enumerable.Range(1, 11)
            .Select(i => CreatePng($"{i:D5}_mon_front.png", 96, 96))
            .ToArray();

        var entries = packer.Pack(pngs, OutputDir);

        // Le 11e sprite doit être sur la deuxième rangée (y > 0)
        var last = entries[^1];
        last.Y.Should().BeGreaterThan(0);
    }

    [Fact]
    public void Pack_ManifestJson_DeserializesCorrectly()
    {
        var packer = new AtlasPacker();
        var png = CreatePng("00006_charizard_overworld.png", 48, 48);

        packer.Pack([png], OutputDir);

        var json = File.ReadAllText(Path.Combine(OutputDir, "atlas-manifest.json"));
        var entries = JsonSerializer.Deserialize<List<AtlasEntry>>(json);
        entries.Should().NotBeNull();
        entries!.Should().HaveCount(1);
        entries[0].AssetKey.Should().Be("00006_charizard_overworld");
        entries[0].View.Should().Be("overworld");
        entries[0].Width.Should().Be(48);
        entries[0].Height.Should().Be(48);
    }

    [Fact]
    public void Pack_MixedSizes_RowHeightFollowsTallestSprite()
    {
        var packer = new AtlasPacker();
        var big = CreatePng("00001_bulbasaur_portrait.png", 128, 128);
        var small = CreatePng("00001_bulbasaur_icon.png", 32, 32);

        var entries = packer.Pack([big, small], OutputDir);

        // Les deux sur la même rangée car 128+32=160 < 1024
        entries[0].Y.Should().Be(0);
        entries[1].Y.Should().Be(0);
        // La petite icône est placée après le portrait
        entries[1].X.Should().Be(128);
    }

    private static bool IsPowerOfTwo(int n) => n > 0 && (n & (n - 1)) == 0;
}
