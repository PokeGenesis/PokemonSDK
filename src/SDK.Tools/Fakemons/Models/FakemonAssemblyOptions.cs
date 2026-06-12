namespace SDK.Tools.Fakemons.Models;

public record FakemonAssemblyOptions(
    string PartsDirectory,
    string OutputDirectory,
    string Identifier,
    int Generation,
    int Type1Id,
    int? Type2Id,
    string EggGroup1,
    string? EggGroup2,
    bool IsLegendary,
    string? FilterExpression,
    string? TranslationsJsonPath,
    bool Strict
);
