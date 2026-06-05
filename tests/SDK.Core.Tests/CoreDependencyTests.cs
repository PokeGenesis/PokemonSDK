using System.Xml.Linq;
using FluentAssertions;

namespace SDK.Core.Tests;

public class CoreDependencyTests
{
    [Fact]
    public void SdkCore_HasNoExternalNuGetPackages()
    {
        var csprojPath = Path.GetFullPath(
            Path.Combine(AppContext.BaseDirectory,
                "..", "..", "..", "..", "..",
                "src", "SDK.Core", "SDK.Core.csproj"));

        File.Exists(csprojPath).Should()
            .BeTrue($"SDK.Core.csproj introuvable à {csprojPath}");

        var doc = XDocument.Load(csprojPath);
        var packageRefs = doc.Descendants("PackageReference").ToList();

        packageRefs.Should().BeEmpty(
            "SDK.Core must have zero external NuGet packages (D-01 constraint)");
    }
}
