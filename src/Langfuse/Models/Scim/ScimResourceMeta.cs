using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Metadata for a SCIM resource type.
/// </summary>
public class ScimResourceMeta
{
    /// <summary>
    ///     Type of the resource.
    /// </summary>
    [JsonPropertyName("resourceType")]
    public string ResourceType { get; set; } = string.Empty;

    /// <summary>
    ///     Location URL of the resource.
    /// </summary>
    [JsonPropertyName("location")]
    public string? Location { get; set; }
}