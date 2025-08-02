using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Responses;

public class ScimSchemasResponse
{
    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new();

    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }

    [JsonPropertyName("Resources")]
    public List<ScimSchemaResource> Resources { get; set; } = new();
}

public class ScimSchemaResource
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("attributes")]
    public object[] Attributes { get; set; } = Array.Empty<object>();

    [JsonPropertyName("meta")]
    public ScimResourceMeta? Meta { get; set; }
}