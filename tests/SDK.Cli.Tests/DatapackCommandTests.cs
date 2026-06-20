using FluentAssertions;
using PokeForge.Cli.Commands;

namespace SDK.Cli.Tests;

public class DatapackCommandTests : IDisposable
{
    private readonly string _originalDir;
    private readonly string _tempDir;

    public DatapackCommandTests()
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
    public void Execute_MissingDatapack_ReturnsOne()
    {
        DatapackCommand.Execute("/nonexistent/datapack", "test.db", "Content").Should().Be(1);
    }

    [Fact]
    public void Execute_EmptyDatapack_ReturnsZero()
    {
        var datapackDir = Path.Combine(_tempDir, "DataPack");
        Directory.CreateDirectory(Path.Combine(datapackDir, "sprites", "front"));
        Directory.CreateDirectory(Path.Combine(datapackDir, "sprites", "back"));
        var dbPath = Path.Combine(_tempDir, "test.db");
        var outDir = Path.Combine(_tempDir, "Content");

        DatapackCommand.Execute(datapackDir, dbPath, outDir).Should().Be(0);
    }

    [Fact]
    public void Execute_MissingFrontDir_SkipsWithoutError()
    {
        var datapackDir = Path.Combine(_tempDir, "DataPack");
        Directory.CreateDirectory(datapackDir);
        var dbPath = Path.Combine(_tempDir, "test.db");
        var outDir = Path.Combine(_tempDir, "Content");

        DatapackCommand.Execute(datapackDir, dbPath, outDir).Should().Be(0);
    }
}
