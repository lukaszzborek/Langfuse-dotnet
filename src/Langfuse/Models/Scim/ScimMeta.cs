using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimMeta
{
    [JsonPropertyName("resourceType")]
    public string ResourceType { get; set; } = string.Empty;

    [JsonPropertyName("created")]
    public string? Created { get; set; }

    [JsonPropertyName("lastModified")]
    public string? LastModified { get; set; }
}