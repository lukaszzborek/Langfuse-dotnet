using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimSchemasResponse
{
    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new();

    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }

    [JsonPropertyName("Resources")]
    public List<ScimSchemaResource> Resources { get; set; } = new();
}