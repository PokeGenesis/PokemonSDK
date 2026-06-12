namespace SDK.Tools.Tests.Fakemons;

using FluentAssertions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SDK.Tools.Fakemons;
using SDK.Tools.Fakemons.Models;

public sealed class FakemonAssemblerTests : IDisposable
{
    private readonly string _tempDir;

    public FakemonAssemblerTests()
    {
        _tempDir = Path.Join(Path.GetTempPath(), $"fakemon_assembler_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    private string CreatePng(string name, int w = 16, int h = 16)
    {
        var path = Path.Combine(_tempDir, name);
        using var img = new Image<Rgba32>(w, h);
        img.SaveAsPng(path);
        return path;
    }

    [Fact]
    public void Assemble_SingleLayer_ReturnsSameSize()
    {
        var path = CreatePng("layer1.png", 16, 16);
        var layers = new[] { new FakemonPartLayer(path, 0) };

        using var result = FakemonAssembler.Assemble(layers);

        result.Width.Should().Be(16);
        result.Height.Should().Be(16);
    }

    [Fact]
    public void Assemble_ThreeLayers_ReturnsCompositeNonNull()
    {
        var p1 = CreatePng("l1.png", 32, 32);
        var p2 = CreatePng("l2.png", 32, 32);
        var p3 = CreatePng("l3.png", 32, 32);
        var layers = new[]
        {
            new FakemonPartLayer(p1, 1),
            new FakemonPartLayer(p2, 2),
            new FakemonPartLayer(p3, 0),
        };

        using var result = FakemonAssembler.Assemble(layers);

        result.Should().NotBeNull();
        result.Width.Should().Be(32);
        result.Height.Should().Be(32);
    }

    [Fact]
    public void Assemble_MissingPath_ThrowsWithPathInMessage()
    {
        var missingPath = Path.Combine(_tempDir, "nonexistent.png");
        var layers = new[] { new FakemonPartLayer(missingPath, 0) };

        var act = () => FakemonAssembler.Assemble(layers);

        act.Should().Throw<FakemonAssemblyException>()
            .WithMessage($"*{missingPath}*");
    }

    [Fact]
    public void Assemble_EmptyLayers_ThrowsFakemonAssemblyException()
    {
        var act = () => FakemonAssembler.Assemble([]);

        act.Should().Throw<FakemonAssemblyException>()
            .WithMessage("*Aucune couche*");
    }

    [Fact]
    public void Assemble_ZOrderSorted_DoesNotThrow()
    {
        var p1 = CreatePng("z_low.png", 16, 16);
        var p2 = CreatePng("z_high.png", 16, 16);
        // ZOrder reversed in input list — should still work
        var layers = new[]
        {
            new FakemonPartLayer(p2, 2),
            new FakemonPartLayer(p1, 1),
        };

        using var result = FakemonAssembler.Assemble(layers);
        result.Should().NotBeNull();
    }
}
