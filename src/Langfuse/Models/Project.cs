using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models;

public class Project
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    /// <summary>
    ///     Number of days to retain data. Null or 0 means no retention. Omitted if no retention is configured.
    /// </summary>
    [JsonPropertyName("retentionDays")]
    public int? RetentionDays { get; set; }
}