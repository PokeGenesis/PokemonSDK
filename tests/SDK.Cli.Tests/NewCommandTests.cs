using FluentAssertions;
using PokeForge.Cli.Commands;

namespace SDK.Cli.Tests;

public class NewCommandTests : IDisposable
{
    private readonly string _originalDir;
    private readonly string _tempDir;

    public NewCommandTests()
    {
        _originalDir = Directory.GetCurrentDirectory();
        _tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_tempDir);
        Directory.SetCurrentDirectory(_tempDir);
    }

    public void Dispose()
    {
        Directory.SetCurrentDirectory(_originalDir);
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, recursive: true);
    }

    [Fact]
    public void ToPascalCase_KebabCase()
    {
        NewCommand.ToPascalCase("mon-jeu").Should().Be("MonJeu");
    }

    [Fact]
    public void ToPascalCase_SingleWord()
    {
        NewCommand.ToPascalCase("single").Should().Be("Single");
    }

    [Fact]
    public void ToPascalCase_MultiSegment()
    {
        NewCommand.ToPascalCase("my-pokemon-game").Should().Be("MyPokemonGame");
    }

    [Fact]
    public void Execute_DirectoryAlreadyExists_ReturnsOne()
    {
        Directory.CreateDirectory("existing-project");
        var result = NewCommand.Execute("existing-project");
        result.Should().Be(1);
    }

    [Fact]
    public void Execute_NewProject_CreatesDirectory()
    {
        var result = NewCommand.Execute("my-game");
        result.Should().Be(0);
        Directory.Exists("my-game").Should().BeTrue();
        File.Exists(Path.Combine("my-game", "MyGame.csproj")).Should().BeTrue();
    }

    [Fact]
    public void Execute_NewProject_NoStarGameStrings()
    {
        NewCommand.Execute("clean-game");

        var textFiles = Directory.GetFiles("clean-game", "*.cs", SearchOption.AllDirectories)
            .Concat(Directory.GetFiles("clean-game", "*.csproj", SearchOption.AllDirectories));

        foreach (var file in textFiles)
        {
            var content = File.ReadAllText(file);
            content.Should().NotContain("StarterGame",
                because: $"{file} should not contain 'StarterGame' after rename");
        }
    }

    [Fact]
    public void Execute_NewProject_BinariesUnchanged()
    {
        NewCommand.Execute("binary-test");

        var generatedOgg = Path.Combine("binary-test", "Content", "Music", "bgm.ogg");
        File.Exists(generatedOgg).Should().BeTrue();

        var generatedBytes = new FileInfo(generatedOgg).Length;
        generatedBytes.Should().BeGreaterThan(0);

        var originalOgg = Path.Combine(
            AppContext.BaseDirectory,
            "..", "..", "..", "..", "..", "..", "samples", "StarterGame", "Content", "Music", "bgm.ogg");

        if (File.Exists(originalOgg))
        {
            var originalBytes = new FileInfo(originalOgg).Length;
            generatedBytes.Should().Be(originalBytes,
                because: "binary files must be copied byte-for-byte");
        }
    }
}
