using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Represents a SCIM schema resource.
/// </summary>
public class ScimSchemaResource
{
    /// <summary>
    ///     Unique identifier (URI) of the schema.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    ///     Name of the schema.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Description of the schema.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     List of attributes defined in this schema.
    /// </summary>
    [JsonPropertyName("attributes")]
    public object[] Attributes { get; set; } = Array.Empty<object>();

    /// <summary>
    ///     Metadata about this schema resource.
    /// </summary>
    [JsonPropertyName("meta")]
    public ScimResourceMeta? Meta { get; set; }
}