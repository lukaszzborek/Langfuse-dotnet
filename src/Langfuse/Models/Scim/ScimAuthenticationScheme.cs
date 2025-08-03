using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimAuthenticationScheme
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("specUri")]
    public string? SpecUri { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
}