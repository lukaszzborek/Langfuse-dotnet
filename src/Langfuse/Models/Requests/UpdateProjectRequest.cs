using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Requests;

public class UpdateProjectRequest
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("metadata")]
    public object? Metadata { get; set; }

    [JsonPropertyName("retention")]
    public int Retention { get; set; }
}