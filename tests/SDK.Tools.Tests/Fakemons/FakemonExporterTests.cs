namespace SDK.Tools.Tests.Fakemons;

using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SDK.Data;
using SDK.Tools.Fakemons;
using SDK.Tools.Fakemons.Models;

public sealed class FakemonExporterTests : IDisposable
{
    private readonly string _tempDir;
    private readonly SqliteConnection _connection;
    private readonly PokemonDbContext _ctx;

    public FakemonExporterTests()
    {
        _tempDir = Path.Join(Path.GetTempPath(), $"fakemon_exporter_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
        var options = new DbContextOptionsBuilder<PokemonDbContext>().UseSqlite(_connection).Options;
        _ctx = new PokemonDbContext(options);
        _ctx.Database.EnsureCreated();

        // Seed type requis pour FK Type1Id
        _ctx.PokemonTypes.Add(new SDK.Core.Entities.PokemonType { Id = 15, Identifier = "dragon", Generation = 1 });
        _ctx.SaveChanges();
    }

    public void Dispose()
    {
        _ctx.Dispose();
        _connection.Dispose();
        Directory.Delete(_tempDir, recursive: true);
    }

    private FakemonAssemblyOptions Opts(string identifier = "test-dragon") => new(
        PartsDirectory: _tempDir,
        OutputDirectory: Path.Combine(_tempDir, "out"),
        Identifier: identifier,
        Generation: 1,
        Type1Id: 15,
        Type2Id: null,
        EggGroup1: "dragon",
        EggGroup2: null,
        IsLegendary: false,
        FilterExpression: null,
        TranslationsJsonPath: null,
        Strict: false
    );

    [Fact]
    public async Task ExportAsync_WritesPng_AndInsertsFakemonSpecies()
    {
        using var image = new Image<Rgba32>(16, 16);
        var opts = Opts();

        await FakemonExporter.ExportAsync(image, opts, _ctx);

        var pngPath = Path.Combine(opts.OutputDirectory, "fk_test-dragon_front.png");
        File.Exists(pngPath).Should().BeTrue();
        _ctx.FakemonSpecies.Any(f => f.Identifier == "test-dragon").Should().BeTrue();
    }

    [Fact]
    public async Task ExportAsync_InsertsSixD22Translations()
    {
        using var image = new Image<Rgba32>(16, 16);
        var opts = Opts("trans-test");

        await FakemonExporter.ExportAsync(image, opts, _ctx);

        var entity = _ctx.FakemonSpecies.First(f => f.Identifier == "trans-test");
        var translations = _ctx.Translations
            .Where(t => t.EntityType == "FakemonSpecies" && t.EntityId == entity.Id)
            .ToList();
        translations.Should().HaveCount(6);
        translations.Select(t => t.Locale).Should().BeEquivalentTo(["en", "es", "fr", "de", "it", "ja"]);
    }

    [Fact]
    public async Task ExportAsync_DuplicateIdentifier_ThrowsFakemonAssemblyException()
    {
        using var img1 = new Image<Rgba32>(16, 16);
        await FakemonExporter.ExportAsync(img1, Opts("dup-test"), _ctx);

        using var img2 = new Image<Rgba32>(16, 16);
        var act = async () => await FakemonExporter.ExportAsync(img2, Opts("dup-test"), _ctx);

        await act.Should().ThrowAsync<FakemonAssemblyException>()
            .WithMessage("*dup-test*");
    }
}
