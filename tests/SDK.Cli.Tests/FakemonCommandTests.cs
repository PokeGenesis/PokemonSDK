namespace SDK.Cli.Tests;

using FluentAssertions;
using PokeForge.Cli.Commands;
using SDK.Tools.Fakemons;
using SDK.Tools.Fakemons.Models;

public sealed class FakemonCommandTests : IDisposable
{
    private readonly string _tempDir;

    public FakemonCommandTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"fakemon_cmd_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose() => Directory.Delete(_tempDir, recursive: true);

    [Fact]
    public void ListParts_EmptyDir_ReturnsZero()
    {
        var emptyDir = Path.Combine(_tempDir, "empty");
        Directory.CreateDirectory(emptyDir);

        var result = FakemonCommand.ExecuteListParts(emptyDir, null);

        result.Should().Be(0);
    }

    [Fact]
    public void ListParts_WithOnePng_ReturnsZero()
    {
        var pngPath = Path.Combine(_tempDir, "body.png");
        File.WriteAllBytes(pngPath, new byte[1]);

        var result = FakemonCommand.ExecuteListParts(_tempDir, null);

        result.Should().Be(0);
    }

    [Fact]
    public async Task Assemble_Strict_EmptyDir_ThrowsFakemonAssemblyException()
    {
        var emptyDir = Path.Combine(_tempDir, "strict_empty");
        Directory.CreateDirectory(emptyDir);
        var opts = new FakemonAssemblyOptions(
            PartsDirectory: emptyDir,
            OutputDirectory: Path.Combine(_tempDir, "out"),
            Identifier: "test-strict",
            Generation: 1,
            Type1Id: 1,
            Type2Id: null,
            EggGroup1: "field",
            EggGroup2: null,
            IsLegendary: false,
            FilterExpression: null,
            TranslationsJsonPath: null,
            Strict: true);

        // ctx not accessed when strict=true + 0 parts (exception thrown before Exporter)
        var act = async () => await FakemonAssemblyPipeline.RunAsync(opts, null!);

        await act.Should().ThrowAsync<FakemonAssemblyException>()
            .WithMessage("*strict*");
    }
}
