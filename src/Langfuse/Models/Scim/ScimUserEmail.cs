using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Represents a user's email address in SCIM format.
/// </summary>
public class ScimUserEmail
{
    /// <summary>
    ///     The email address value.
    /// </summary>
    [JsonPropertyName("value")]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    ///     Type of email address (e.g., work, home).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    ///     Indicates if this is the primary email address.
    /// </summary>
    [JsonPropertyName("primary")]
    public bool Primary { get; set; }
}