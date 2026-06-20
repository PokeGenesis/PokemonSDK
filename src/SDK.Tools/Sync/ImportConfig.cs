namespace SDK.Tools.Sync;

using System.Text.Json.Serialization;

public sealed class ImportConfig
{
    [JsonPropertyName("sprites_root")]
    public string SpritesRoot { get; set; } = "assets/sprites";

    [JsonPropertyName("output_dir")]
    public string OutputDir { get; set; } = "Content/atlas";

    [JsonPropertyName("db_path")]
    public string DbPath { get; set; } = "src/SDK.Data/data/PokemonSDK.db";

    [JsonPropertyName("include_views")]
    public string[] IncludeViews { get; set; } = ["front", "back", "overworld", "portrait", "icon"];

    [JsonPropertyName("resize_to_target")]
    public bool ResizeToTarget { get; set; } = false;
}
