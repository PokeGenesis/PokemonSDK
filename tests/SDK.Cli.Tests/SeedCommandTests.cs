using FluentAssertions;
using PokeForge.Cli.Commands;

namespace SDK.Cli.Tests;

public class SeedCommandTests : IDisposable
{
    private readonly string _originalDir;
    private readonly string _tempDir;

    public SeedCommandTests()
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
    public void Execute_CreatesDbAndReturnsZero()
    {
        var dbPath = Path.Combine(_tempDir, "test-seed.db");

        SeedCommand.Execute(dbPath).Should().Be(0);

        File.Exists(dbPath).Should().BeTrue();
    }
}
