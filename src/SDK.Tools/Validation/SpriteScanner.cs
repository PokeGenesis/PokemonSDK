namespace SDK.Tools.Validation;

public sealed class SpriteScanner
{
    public IEnumerable<string> Scan(string rootPath)
    {
        if (!Directory.Exists(rootPath))
            throw new DirectoryNotFoundException($"Dossier assets introuvable : {rootPath}");
        return Directory.EnumerateFiles(rootPath, "*.png", SearchOption.AllDirectories);
    }
}
