namespace SDK.Core.Entities;

public sealed class SpriteAtlasEntry
{
    public int Id { get; set; }
    public string AssetKey { get; set; } = "";
    public string View { get; set; } = "";
    public string AtlasPath { get; set; } = "";
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}
