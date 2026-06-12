namespace SDK.Tools.Validation;

using System.Text.RegularExpressions;

public sealed class SpriteValidator
{
    private static readonly Regex D16Pattern =
        new(@"^(\d{5})_([a-z0-9_-]+)_(front|back|overworld|portrait|icon)\.png$",
            RegexOptions.Compiled);

    private static readonly Regex FakemonPattern =
        new(@"^(fk_[a-z0-9-]+)_(front|back|overworld|portrait|icon)\.png$",
            RegexOptions.Compiled);

    private static readonly byte[] PngSignature = [137, 80, 78, 71, 13, 10, 26, 10];

    private static readonly Dictionary<string, (int W, int H)> ExpectedSizes = new()
    {
        ["front"]     = (96, 96),
        ["back"]      = (96, 96),
        ["overworld"] = (48, 48),
        ["portrait"]  = (128, 128),
        ["icon"]      = (32, 32),
    };

    public IReadOnlyList<SpriteValidationResult> ValidateAll(IEnumerable<string> filePaths)
        => filePaths.Select(Validate).ToList();

    public SpriteValidationResult Validate(string filePath)
    {
        var fileName = Path.GetFileName(filePath);
        var entry    = ParseEntry(filePath, fileName);

        byte[] header;
        try { header = ReadPngHeader(filePath); }
        catch { return new(entry, SeverityLevel.Error, "PNG corrompu ou illisible"); }

        if (!IsPngSignatureValid(header))
            return new(entry, SeverityLevel.Error, "Signature PNG invalide — fichier corrompu");

        if (entry.View is null)
            return new(entry, SeverityLevel.Warn,
                "Nommage non conforme D-16 (attendu: {dexid5}_{identifier}_{view}.png)");

        var colorType = header[25];
        bool hasAlpha = colorType == 4 || colorType == 6;
        if (!hasAlpha)
            return new(entry, SeverityLevel.Error,
                $"Canal alpha absent (color type={colorType}, attendu 4 ou 6)");

        int width  = ReadInt32BigEndian(header, 16);
        int height = ReadInt32BigEndian(header, 20);

        if (ExpectedSizes.TryGetValue(entry.View, out var expected))
        {
            if (width != expected.W || height != expected.H)
                return new(entry, SeverityLevel.Error,
                    $"Taille incorrecte {width}×{height} (attendu {expected.W}×{expected.H} pour view={entry.View})");
        }

        return new(entry, SeverityLevel.Ok, "OK");
    }

    private static SpriteEntry ParseEntry(string filePath, string fileName)
    {
        var m = D16Pattern.Match(fileName);
        if (m.Success)
            return new(filePath, fileName, m.Groups[1].Value, m.Groups[2].Value, m.Groups[3].Value);

        var fm = FakemonPattern.Match(fileName);
        if (fm.Success)
            return new(filePath, fileName, null, fm.Groups[1].Value, fm.Groups[2].Value);

        return new(filePath, fileName, null, null, null);
    }

    private static byte[] ReadPngHeader(string filePath)
    {
        using var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        var buf = new byte[26];
        int read = fs.Read(buf, 0, 26);
        if (read < 26) throw new InvalidDataException("Fichier trop court");
        return buf;
    }

    private static bool IsPngSignatureValid(byte[] header)
    {
        for (int i = 0; i < PngSignature.Length; i++)
            if (header[i] != PngSignature[i]) return false;
        return true;
    }

    private static int ReadInt32BigEndian(byte[] buf, int offset) =>
        (buf[offset] << 24) | (buf[offset + 1] << 16) | (buf[offset + 2] << 8) | buf[offset + 3];
}
