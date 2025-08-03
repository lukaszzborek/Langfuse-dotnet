using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

public class ScimResourceType
{
    [JsonPropertyName("schemas")]
    public List<string>? Schemas { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("schema")]
    public string Schema { get; set; } = string.Empty;

    [JsonPropertyName("schemaExtensions")]
    public List<ScimSchemaExtension> SchemaExtensions { get; set; } = new();

    [JsonPropertyName("meta")]
    public ScimResourceMeta? Meta { get; set; }
}