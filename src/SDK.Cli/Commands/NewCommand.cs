using System.CommandLine;
using System.IO.Compression;
using System.Reflection;
using System.Text;

namespace PokeForge.Cli.Commands;

public static class NewCommand
{
    public static void Register(RootCommand root)
    {
        var cmd = new Command("new", "Scaffold a new PokeForge game project");
        var nameArg = new Argument<string>("name", "Project name (e.g. mon-jeu)");
        cmd.AddArgument(nameArg);
        cmd.SetHandler((name) => Environment.Exit(Execute(name)), nameArg);
        root.AddCommand(cmd);
    }

    internal static int Execute(string name)
    {
        if (Directory.Exists(name))
        {
            Console.Error.WriteLine($"Directory '{name}' already exists.");
            return 1;
        }

        var pascalName = ToPascalCase(name);
        var projectRoot = Path.GetFullPath(name);
        var rootWithSep = projectRoot + Path.DirectorySeparatorChar;

        using var stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream("starter-template.zip")!;

        using var archive = new ZipArchive(stream, ZipArchiveMode.Read);

        var textExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".cs", ".csproj", ".json", ".md", ".mgcb", ".lua", ".config"
        };

        foreach (var entry in archive.Entries)
        {
            var entryRelative = entry.FullName.Replace('\\', '/').TrimStart('/');
            var safePath = Path.GetFullPath(Path.Combine(projectRoot, entryRelative));
            if (!safePath.StartsWith(rootWithSep, StringComparison.Ordinal))
                continue;

            if (entry.FullName.EndsWith('/'))
            {
                Directory.CreateDirectory(safePath);
                continue;
            }

            var destRelative = entryRelative;

            if (Path.GetFileName(destRelative) == "StarterGame.csproj")
                destRelative = Path.Combine(
                    Path.GetDirectoryName(destRelative) ?? "",
                    $"{pascalName}.csproj");

            var destPath = Path.GetFullPath(Path.Combine(projectRoot, destRelative));
            if (!destPath.StartsWith(rootWithSep, StringComparison.Ordinal))
                continue;
            Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);

            var ext = Path.GetExtension(entry.FullName).ToLowerInvariant();
            var fileName = Path.GetFileName(entry.FullName);
            var isText = textExtensions.Contains(ext)
                || fileName.Equals("nuget.config", StringComparison.OrdinalIgnoreCase);

            if (fileName.Equals("nuget.config", StringComparison.OrdinalIgnoreCase))
            {
                File.WriteAllText(destPath,
                    """
                    <?xml version="1.0" encoding="utf-8"?>
                    <configuration>
                      <packageSources>
                        <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
                      </packageSources>
                    </configuration>
                    """,
                    Encoding.UTF8);
            }
            else if (isText)
            {
                using var reader = new StreamReader(entry.Open(), Encoding.UTF8);
                var content = reader.ReadToEnd().Replace("StarterGame", pascalName);
                File.WriteAllText(destPath, content, Encoding.UTF8);
            }
            else
            {
                using var src = entry.Open();
                using var dst = File.Create(destPath);
                src.CopyTo(dst);
            }
        }

        Console.WriteLine($"Project '{name}' created. Run: cd {name} && dotnet run");
        return 0;
    }

    internal static string ToPascalCase(string name)
    {
        return string.Concat(name.Split('-').Select(w =>
            w.Length == 0 ? w : char.ToUpperInvariant(w[0]) + w.Substring(1)));
    }
}
