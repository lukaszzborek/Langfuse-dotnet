using System.Text.Json.Serialization;

namespace zborek.Langfuse.Models.Scim;

/// <summary>
///     Represents a user's name in SCIM format.
/// </summary>
public class ScimUserName
{
    /// <summary>
    ///     The full formatted name of the user.
    /// </summary>
    [JsonPropertyName("formatted")]
    public string? Formatted { get; set; }
}