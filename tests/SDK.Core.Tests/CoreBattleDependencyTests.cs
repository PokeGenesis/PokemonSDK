namespace SDK.Core.Tests;

using System.Xml.Linq;
using FluentAssertions;

public class CoreBattleDependencyTests
{
    [Fact]
    public void SdkBattle_HasZeroNuGetPackageReferences()
    {
        var root = GetRepoRoot();
        var csproj = Path.Join(root, "src", "SDK.Battle", "SDK.Battle.csproj");
        File.Exists(csproj).Should().BeTrue($"SDK.Battle.csproj introuvable à {csproj}");

        var doc = XDocument.Load(csproj);
        var packages = doc.Descendants("PackageReference").ToList();
        packages.Should().BeEmpty("SDK.Battle doit rester sans NuGet externe (règle dépendances)");
    }

    private static string GetRepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        for (int i = 0; i < 6; i++)
        {
            if (File.Exists(Path.Join(dir!.FullName, "PokemonSDK.slnx")))
                return dir.FullName;
            dir = dir.Parent;
        }
        throw new InvalidOperationException("Racine du repo introuvable (PokemonSDK.slnx absent).");
    }
}
