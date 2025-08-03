using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

public class PromptMeta
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("versions")]
    public List<int> Versions { get; set; } = new();

    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = new();

    [JsonPropertyName("lastUpdatedAt")]
    public DateTime LastUpdatedAt { get; set; }

    [JsonPropertyName("lastConfig")]
    public object? LastConfig { get; set; }
}