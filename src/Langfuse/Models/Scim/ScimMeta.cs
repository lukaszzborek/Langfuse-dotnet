using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Metadata about a SCIM user resource.
/// </summary>
public class ScimMeta
{
    /// <summary>
    ///     Type of the resource.
    /// </summary>
    [JsonPropertyName("resourceType")]
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    ///     Timestamp when the resource was created.
    /// </summary>
    [JsonPropertyName("created")]
    public string? Created { get; set; }

    /// <summary>
    ///     Timestamp when the resource was last modified.
    /// </summary>
    [JsonPropertyName("lastModified")]
    public string? LastModified { get; set; }
}