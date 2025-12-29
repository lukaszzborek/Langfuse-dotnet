using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Represents a SCIM resource type definition.
/// </summary>
public class ScimResourceType
{
    /// <summary>
    ///     SCIM schema URIs for this resource type.
    /// </summary>
    [JsonPropertyName("schemas")]
    public List<string>? Schemas { get; set; }

    /// <summary>
    ///     Unique identifier of the resource type.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the resource type.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     API endpoint for this resource type.
    /// </summary>
    [JsonPropertyName("endpoint")]
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    ///     Description of the resource type.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     Primary schema URI for this resource type.
    /// </summary>
    [JsonPropertyName("schema")]
    public string Schema { get; set; } = string.Empty;

    /// <summary>
    ///     List of schema extensions for this resource type.
    /// </summary>
    [JsonPropertyName("schemaExtensions")]
    public List<ScimSchemaExtension> SchemaExtensions { get; set; } = new();

    /// <summary>
    ///     Metadata about this resource type.
    /// </summary>
    [JsonPropertyName("meta")]
    public ScimResourceMeta Meta { get; set; } = new();
}