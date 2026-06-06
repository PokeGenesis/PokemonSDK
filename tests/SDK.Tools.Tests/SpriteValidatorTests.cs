using FluentAssertions;
using SDK.Tools.Validation;

namespace SDK.Tools.Tests;

public class SpriteValidatorTests : IDisposable
{
    private readonly SpriteValidator _validator = new();
    private readonly List<string> _tempFiles = [];

    // Construit un header PNG synthétique de 26 bytes
    private string CreateTempPng(string fileName, int w, int h, byte colorType, bool validSignature = true)
    {
        var path = Path.Combine(Path.GetTempPath(), fileName);
        var header = new byte[26];

        if (validSignature)
        {
            byte[] sig = [137, 80, 78, 71, 13, 10, 26, 10];
            sig.CopyTo(header, 0);
        }
        // IHDR length
        header[8] = 0; header[9] = 0; header[10] = 0; header[11] = 13;
        // "IHDR"
        header[12] = 73; header[13] = 72; header[14] = 68; header[15] = 82;
        // Width big-endian
        header[16] = (byte)(w >> 24); header[17] = (byte)(w >> 16);
        header[18] = (byte)(w >> 8);  header[19] = (byte)w;
        // Height big-endian
        header[20] = (byte)(h >> 24); header[21] = (byte)(h >> 16);
        header[22] = (byte)(h >> 8);  header[23] = (byte)h;
        // Bit depth = 8
        header[24] = 8;
        // Color type
        header[25] = colorType;

        File.WriteAllBytes(path, header);
        _tempFiles.Add(path);
        return path;
    }

    // ── Nommage D-16 ──────────────────────────────────────────────────────────

    [Fact]
    public void Naming_ConformD16Front_ReturnsOk()
    {
        var path = CreateTempPng("00025_pikachu_front.png", 96, 96, 6);
        var r = _validator.Validate(path);
        r.Severity.Should().Be(SeverityLevel.Ok);
    }

    [Fact]
    public void Naming_NoDexId_ReturnsWarn()
    {
        var path = CreateTempPng("pikachu_front.png", 96, 96, 6);
        var r = _validator.Validate(path);
        r.Severity.Should().Be(SeverityLevel.Warn);
        r.Message.Should().Contain("D-16");
    }

    [Fact]
    public void Naming_ShortDexId_ReturnsWarn()
    {
        var path = CreateTempPng("025_pikachu_front.png", 96, 96, 6);
        var r = _validator.Validate(path);
        r.Severity.Should().Be(SeverityLevel.Warn);
    }

    [Fact]
    public void Naming_InvalidView_ReturnsWarn()
    {
        var path = CreateTempPng("00025_pikachu_jump.png", 96, 96, 6);
        var r = _validator.Validate(path);
        r.Severity.Should().Be(SeverityLevel.Warn);
    }

    [Fact]
    public void Naming_TilesetWithoutDexId_ReturnsWarn()
    {
        var path = CreateTempPng("route_01.png", 16, 16, 6);
        var r = _validator.Validate(path);
        r.Severity.Should().Be(SeverityLevel.Warn);
    }

    // ── Tailles ───────────────────────────────────────────────────────────────

    [Fact]
    public void Size_Front96x96_ReturnsOk()
    {
        var path = CreateTempPng("00025_pikachu_front.png", 96, 96, 6);
        _validator.Validate(path).Severity.Should().Be(SeverityLevel.Ok);
    }

    [Fact]
    public void Size_Front64x64_ReturnsError()
    {
        var path = CreateTempPng("00025_pikachu_front.png", 64, 64, 6);
        var r = _validator.Validate(path);
        r.Severity.Should().Be(SeverityLevel.Error);
        r.Message.Should().Contain("64×64");
    }

    [Fact]
    public void Size_Overworld48x48_ReturnsOk()
    {
        var path = CreateTempPng("00025_pikachu_overworld.png", 48, 48, 6);
        _validator.Validate(path).Severity.Should().Be(SeverityLevel.Ok);
    }

    [Fact]
    public void Size_Portrait128x128_ReturnsOk()
    {
        var path = CreateTempPng("00130_gyarados_portrait.png", 128, 128, 6);
        _validator.Validate(path).Severity.Should().Be(SeverityLevel.Ok);
    }

    [Fact]
    public void Size_Icon32x32_ReturnsOk()
    {
        var path = CreateTempPng("00025_pikachu_icon.png", 32, 32, 6);
        _validator.Validate(path).Severity.Should().Be(SeverityLevel.Ok);
    }

    // ── Canal alpha ───────────────────────────────────────────────────────────

    [Fact]
    public void Alpha_ColorType6_RGBA_ReturnsOk()
    {
        var path = CreateTempPng("00025_pikachu_front.png", 96, 96, colorType: 6);
        _validator.Validate(path).Severity.Should().Be(SeverityLevel.Ok);
    }

    [Fact]
    public void Alpha_ColorType4_GrayAlpha_ReturnsOk()
    {
        var path = CreateTempPng("00025_pikachu_front.png", 96, 96, colorType: 4);
        _validator.Validate(path).Severity.Should().Be(SeverityLevel.Ok);
    }

    [Fact]
    public void Alpha_ColorType2_RGB_ReturnsError()
    {
        var path = CreateTempPng("00025_pikachu_front.png", 96, 96, colorType: 2);
        var r = _validator.Validate(path);
        r.Severity.Should().Be(SeverityLevel.Error);
        r.Message.Should().Contain("alpha");
    }

    // ── Corruption ────────────────────────────────────────────────────────────

    [Fact]
    public void Corruption_InvalidSignature_ReturnsError()
    {
        var path = CreateTempPng("00025_pikachu_front.png", 96, 96, 6, validSignature: false);
        var r = _validator.Validate(path);
        r.Severity.Should().Be(SeverityLevel.Error);
        r.Message.Should().Contain("corrompu");
    }

    [Fact]
    public void Corruption_TooShort_ReturnsError()
    {
        var path = Path.Combine(Path.GetTempPath(), "00025_pikachu_front.png");
        File.WriteAllBytes(path, [137, 80, 78, 71]); // seulement 4 bytes
        _tempFiles.Add(path);
        var r = _validator.Validate(path);
        r.Severity.Should().Be(SeverityLevel.Error);
    }

    // ── Shiny / formes ────────────────────────────────────────────────────────

    [Fact]
    public void Naming_ShinyFront_ReturnsOk()
    {
        var path = CreateTempPng("00025_pikachu_shiny_front.png", 96, 96, 6);
        _validator.Validate(path).Severity.Should().Be(SeverityLevel.Ok);
    }

    [Fact]
    public void Naming_MegaForm_ReturnsOk()
    {
        var path = CreateTempPng("00006_charizard_mega_x_front.png", 96, 96, 6);
        _validator.Validate(path).Severity.Should().Be(SeverityLevel.Ok);
    }

    public void Dispose()
    {
        foreach (var f in _tempFiles.Where(File.Exists))
            File.Delete(f);
    }
}
