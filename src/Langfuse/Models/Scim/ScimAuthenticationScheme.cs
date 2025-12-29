using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Represents a SCIM authentication scheme.
/// </summary>
public class ScimAuthenticationScheme
{
    /// <summary>
    ///     Name of the authentication scheme.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     Description of the authentication scheme.
    /// </summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     URI to the specification for this scheme.
    /// </summary>
    [JsonPropertyName("specUri")]
    public string? SpecUri { get; set; }

    /// <summary>
    ///     Type of authentication scheme (e.g., httpbasic, oauthbearertoken).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     Indicates if this is the primary authentication scheme.
    /// </summary>
    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
}