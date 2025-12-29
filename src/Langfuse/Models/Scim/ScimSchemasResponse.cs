using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Response containing SCIM schemas.
/// </summary>
public class ScimSchemasResponse
{
    /// <summary>
    ///     SCIM schema URIs for this response.
    /// </summary>
    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new();

    /// <summary>
    ///     Total number of schemas.
    /// </summary>
    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }

    /// <summary>
    ///     List of SCIM schema resources.
    /// </summary>
    [JsonPropertyName("Resources")]
    public List<ScimSchemaResource> Resources { get; set; } = new();
}