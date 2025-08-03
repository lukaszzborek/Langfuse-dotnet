using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimResourceMeta
{
    [JsonPropertyName("resourceType")]
    public string ResourceType { get; set; } = string.Empty;

    [JsonPropertyName("location")]
    public string? Location { get; set; }
}