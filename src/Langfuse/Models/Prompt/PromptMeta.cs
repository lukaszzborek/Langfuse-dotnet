using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

public class PromptMeta
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Indicates whether the prompt is a text or chat prompt.
    /// </summary>
    [JsonPropertyName("type")]
    public PromptType Type { get; set; }

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