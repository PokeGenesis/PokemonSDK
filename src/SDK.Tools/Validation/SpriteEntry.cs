namespace SDK.Tools.Validation;

public sealed record SpriteEntry(
    string FilePath,
    string FileName,
    string? DexId,
    string? Identifier,
    string? View // front|back|overworld|portrait|icon|null
);
