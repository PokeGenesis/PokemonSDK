namespace SDK.Tools.Tests.Fakemons;

using FluentAssertions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SDK.Tools.Fakemons;
using System.Text.Json;

public sealed class FakemonFilterTests : IDisposable
{
    private readonly string _tempDir;

    public FakemonFilterTests()
    {
        _tempDir = Path.Join(Path.GetTempPath(), $"fakemon_filter_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private string CreatePng(string name)
    {
        var path = Path.Combine(_tempDir, name);
        using var img = new Image<Rgba32>(8, 8);
        img.SaveAsPng(path);
        return path;
    }

    private void WriteSidecar(string pngPath, object data)
        => File.WriteAllText(Path.ChangeExtension(pngPath, ".json"), JsonSerializer.Serialize(data));

    [Fact]
    public void Apply_NullFilter_ReturnsAllLayers()
    {
        CreatePng("a.png");
        CreatePng("b.png");
        var catalogInstance = FakemonPartsCatalog.Scan(_tempDir);

        var result = FakemonFilter.Apply(catalogInstance.Layers, catalogInstance, null);

        result.Should().HaveCount(2);
    }

    [Fact]
    public void Apply_TypeFireFilter_ReturnsFirePlusNoSidecar()
    {
        var pFire = CreatePng("fire_part.png");
        WriteSidecar(pFire, new { type = "fire" });
        var pNoSidecar = CreatePng("generic.png");
        var pWater = CreatePng("water_part.png");
        WriteSidecar(pWater, new { type = "water" });

        var catalogInstance = FakemonPartsCatalog.Scan(_tempDir);

        var result = FakemonFilter.Apply(catalogInstance.Layers, catalogInstance, "type:fire");

        result.Should().HaveCount(2);
        result.Should().Contain(l => l.Path == pFire);
        result.Should().Contain(l => l.Path == pNoSidecar);
        result.Should().NotContain(l => l.Path == pWater);
    }

    [Fact]
    public void Apply_TypeFireFilter_NoFirePng_ReturnsOnlyNoSidecar()
    {
        var pWater = CreatePng("water.png");
        WriteSidecar(pWater, new { type = "water" });
        var pNoSidecar = CreatePng("no_sidecar.png");

        var catalogInstance = FakemonPartsCatalog.Scan(_tempDir);

        var result = FakemonFilter.Apply(catalogInstance.Layers, catalogInstance, "type:fire");

        result.Should().HaveCount(1);
        result.Should().Contain(l => l.Path == pNoSidecar);
    }

    [Fact]
    public void Scan_WithZOrderSidecar_ParsesZOrder()
    {
        var p = CreatePng("layered.png");
        WriteSidecar(p, new { type = "grass", @object = "z-order", content = 3 }); // direct z-order
        File.WriteAllText(Path.ChangeExtension(p, ".json"), """{"z-order":3,"type":"grass"}""");

        var catalogInstance = FakemonPartsCatalog.Scan(_tempDir);

        catalogInstance.Layers.Should().HaveCount(1);
        catalogInstance.Layers[0].ZOrder.Should().Be(3);
    }
}
