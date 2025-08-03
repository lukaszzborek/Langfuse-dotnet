using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimSchema
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public List<ScimAttribute> Attributes { get; set; } = new();
}