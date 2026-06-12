using FluentAssertions;
using PokeForge.Cli.Commands;

namespace SDK.Cli.Tests;

public class AssetSyncCommandTests : IDisposable
{
    private readonly string _originalDir;
    private readonly string _tempDir;

    public AssetSyncCommandTests()
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
    public void Execute_MissingConfig_ReturnsOne()
    {
        AssetSyncCommand.Execute("nonexistent/import.json").Should().Be(1);
    }

    [Fact]
    public void Execute_ValidConfigEmptySprites_ReturnsZero()
    {
        var configPath = Path.Combine(_tempDir, "import.json");
        var spritesDir = Path.Combine(_tempDir, "sprites");
        var outDir     = Path.Combine(_tempDir, "out");
        var dbPath     = Path.Combine(_tempDir, "test.db");
        Directory.CreateDirectory(spritesDir);

        var json = $$$"""
            {
              "sprites_root": "{{{spritesDir}}}",
              "output_dir":   "{{{outDir}}}",
              "db_path":      "{{{dbPath}}}",
              "include_views": ["front"]
            }
            """;
        File.WriteAllText(configPath, json);

        AssetSyncCommand.Execute(configPath).Should().Be(0);
    }
}
