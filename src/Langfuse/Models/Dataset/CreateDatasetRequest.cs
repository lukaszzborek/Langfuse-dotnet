using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Dataset;

public class CreateDatasetRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }
}