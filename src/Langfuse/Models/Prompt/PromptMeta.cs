using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Prompt;

/// <summary>
///     Metadata about a prompt including its versions and labels.
/// </summary>
public class PromptMeta
{
    /// <summary>
    ///     Name of the prompt.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Indicates whether the prompt is a text or chat prompt.
    /// </summary>
    [JsonPropertyName("type")]
    public PromptType Type { get; set; }

    /// <summary>
    ///     List of version numbers for this prompt.
    /// </summary>
    [JsonPropertyName("versions")]
    public List<int> Versions { get; set; } = new();

    /// <summary>
    ///     List of tags applied to all versions of this prompt.
    /// </summary>
    [JsonPropertyName("tags")]
    public List<string> Tags { get; set; } = new();

    /// <summary>
    ///     List of deployment labels for this prompt.
    /// </summary>
    [JsonPropertyName("labels")]
    public List<string> Labels { get; set; } = new();

    /// <summary>
    ///     Timestamp of the last update to any version of this prompt.
    /// </summary>
    [JsonPropertyName("lastUpdatedAt")]
    public DateTime LastUpdatedAt { get; set; }

    /// <summary>
    ///     Config object of the most recent prompt version that matches the filters.
    /// </summary>
    [JsonPropertyName("lastConfig")]
    public object? LastConfig { get; set; }
}