namespace SDK.Tools.Validation;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;

public sealed class SpriteResizer
{
    private static readonly Regex ViewPattern =
        new(@"_(front|back|overworld|portrait|icon)\.png$", RegexOptions.Compiled);

    private static readonly Dictionary<string, (int W, int H)> TargetSizes = new()
    {
        ["front"]     = (96, 96),
        ["back"]      = (96, 96),
        ["overworld"] = (48, 48),
        ["portrait"]  = (128, 128),
        ["icon"]      = (32, 32),
    };

    public void ResizeAll(IEnumerable<string> filePaths, string tempDir)
    {
        Directory.CreateDirectory(tempDir);

        foreach (var path in filePaths)
        {
            var fileName = Path.GetFileName(path);
            var m        = ViewPattern.Match(fileName);
            var destPath = Path.Combine(tempDir, fileName);

            if (m.Success && TargetSizes.TryGetValue(m.Groups[1].Value, out var target))
            {
                using var img = Image.Load<Rgba32>(path);
                if (img.Width != target.W || img.Height != target.H)
                    img.Mutate(x => x.Resize(target.W, target.H));
                img.SaveAsPng(destPath, new PngEncoder { ColorType = PngColorType.RgbWithAlpha });
            }
            else
            {
                using var img2 = Image.Load<Rgba32>(path);
                img2.SaveAsPng(destPath, new PngEncoder { ColorType = PngColorType.RgbWithAlpha });
            }
        }
    }
}
