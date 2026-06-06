namespace SDK.Tools.Atlas;

public sealed record AtlasEntry(
    string AssetKey,
    string View,
    int X, int Y,
    int Width, int Height
);
