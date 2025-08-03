using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimUserEmail
{
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
}