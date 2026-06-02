namespace SDK.Core.Tests;

using System.Xml.Linq;
using FluentAssertions;

public class PlatformTests
{
    [Fact]
    public void AllProjects_TargetNet10()
    {
        var root = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

        var csprojFiles = Directory.GetFiles(root, "*.csproj", SearchOption.AllDirectories)
            .Where(f => !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar))
            .ToList();

        csprojFiles.Should().NotBeEmpty("le repo doit contenir des .csproj");

        foreach (var f in csprojFiles)
        {
            var doc = XDocument.Load(f);
            var tf = doc.Descendants("TargetFramework").FirstOrDefault()?.Value;
            tf.Should().Be("net10.0", $"{Path.GetFileName(f)} doit cibler net10.0 (D-01)");
        }
    }

    [Fact]
    public void SourceFiles_ContainNoHardcodedWindowsPaths()
    {
        var root = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", ".."));

        var srcDir = Path.Combine(root, "src");
        var csFiles = Directory.GetFiles(srcDir, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar))
            .ToList();

        csFiles.Should().NotBeEmpty("src/ doit contenir des .cs");

        foreach (var f in csFiles)
        {
            var content = File.ReadAllText(f);
            var relativeName = Path.GetRelativePath(root, f);
            content.Should().NotContain("C:\\",
                $"{relativeName} ne doit pas contenir de chemin Windows absolu (PLAT-03)");
            content.Should().NotContain("\"C:/",
                $"{relativeName} ne doit pas contenir de chemin Windows absolu (PLAT-03)");
        }
    }
}
