using FluentAssertions;
using PokeForge.Cli.Commands;

namespace SDK.Cli.Tests;

public class DoctorCommandTests : IDisposable
{
    private readonly string _originalDir;
    private readonly string _tempDir;

    public DoctorCommandTests()
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
        DoctorCommand.Execute("nonexistent/import.json").Should().Be(1);
    }

    [Fact]
    public void Execute_ValidConfigNoDb_ReturnsZero()
    {
        var configPath = Path.Combine(_tempDir, "import.json");
        var spritesDir = Path.Combine(_tempDir, "sprites");
        var dbPath     = Path.Combine(_tempDir, "absent.db");
        Directory.CreateDirectory(spritesDir);

        var json = $$$"""
            {
              "sprites_root": "{{{spritesDir}}}",
              "output_dir":   "{{{_tempDir}}}",
              "db_path":      "{{{dbPath}}}",
              "include_views": ["front"]
            }
            """;
        File.WriteAllText(configPath, json);

        DoctorCommand.Execute(configPath).Should().Be(0);
    }

    [Fact]
    public void Execute_ValidConfigAllPresent_ReturnsZero()
    {
        var configPath = Path.Combine(_tempDir, "import.json");
        var spritesDir = Path.Combine(_tempDir, "sprites");
        var dbPath     = Path.Combine(_tempDir, "game.db");
        Directory.CreateDirectory(spritesDir);
        File.WriteAllBytes(dbPath, []);

        var json = $$$"""
            {
              "sprites_root": "{{{spritesDir}}}",
              "output_dir":   "{{{_tempDir}}}",
              "db_path":      "{{{dbPath}}}",
              "include_views": ["front"]
            }
            """;
        File.WriteAllText(configPath, json);

        DoctorCommand.Execute(configPath).Should().Be(0);
    }
}
