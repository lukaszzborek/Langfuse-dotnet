using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Response containing SCIM resource types.
/// </summary>
public class ScimResourceTypesResponse
{
    /// <summary>
    ///     SCIM schema URIs for this response.
    /// </summary>
    [JsonPropertyName("schemas")]
    public List<string> Schemas { get; set; } = new();

    /// <summary>
    ///     Total number of resource types.
    /// </summary>
    [JsonPropertyName("totalResults")]
    public int TotalResults { get; set; }

    /// <summary>
    ///     List of SCIM resource types.
    /// </summary>
    [JsonPropertyName("Resources")]
    public List<ScimResourceType> Resources { get; set; } = new();
}